using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(BattleInventory),typeof(WorldInputGate))]
public sealed class InventoryWindow : MonoBehaviour
{
    public RectTransform panel;
    public InventorySlotView[] slots;
    public TextMeshProUGUI details, goldLabel, message;
    public Button splitButton, useButton, closeButton;
    public Image dragIcon;
    public event Action<int> UseRequested;
    public bool IsOpen => panel!=null && panel.gameObject.activeSelf;
    public int SelectedIndex {get;private set;}
    public BattleInventory Inventory {get;private set;}
    private WorldInputGate gate;
    private BattleUnit unit;
    private int dragging=-1;

    private void Awake()
    {
        Inventory=GetComponent<BattleInventory>();gate=GetComponent<WorldInputGate>();unit=GetComponent<BattleUnit>();
        for(int i=0;i<(slots?.Length ?? 0);i++)slots[i].Bind(this,i);
        if(splitButton!=null)splitButton.onClick.AddListener(SplitSelected);
        if(useButton!=null)useButton.onClick.AddListener(RequestUse);
        if(closeButton!=null)closeButton.onClick.AddListener(Close);
        Close();
    }
    private void OnEnable(){if(Application.isPlaying && Inventory!=null)Inventory.State.Changed+=Refresh;}
    private void OnDisable(){if(Inventory!=null)Inventory.State.Changed-=Refresh;Close();}
    private void OnDestroy()
    {
        if(splitButton!=null)splitButton.onClick.RemoveListener(SplitSelected);
        if(useButton!=null)useButton.onClick.RemoveListener(RequestUse);
        if(closeButton!=null)closeButton.onClick.RemoveListener(Close);
    }
    private void Update()
    {
        var keyboard=Keyboard.current;if(keyboard==null)return;
        if(keyboard.iKey.wasPressedThisFrame){if(IsOpen)Close();else Open();return;}
        if(!IsOpen)return;
        if(keyboard.escapeKey.wasPressedThisFrame){Close();return;}
        int offset=keyboard.leftArrowKey.wasPressedThisFrame?-1:keyboard.rightArrowKey.wasPressedThisFrame?1:
            keyboard.upArrowKey.wasPressedThisFrame?-6:keyboard.downArrowKey.wasPressedThisFrame?6:0;
        if(offset!=0)Select(Mathf.Clamp(SelectedIndex+offset,0,29));
    }
    public bool Open()
    {
        if(panel==null || !isActiveAndEnabled || unit==null || !unit.IsAlive ||
            (BattleTransition.Instance!=null && BattleTransition.Instance.EncounterActive))return false;
        panel.gameObject.SetActive(true);gate.Acquire(this);message.text="";Refresh();return true;
    }
    public void Close()
    {
        EndDrag();if(panel!=null)panel.gameObject.SetActive(false);if(gate!=null)gate.Release(this);
    }
    public void Select(int index){if(!IsOpen || index<0 || index>=30)return;SelectedIndex=index;Refresh();}
    public void Refresh()
    {
        if(!IsOpen || Inventory==null)return;
        for(int i=0;i<(slots?.Length ?? 0);i++)slots[i].Render(Inventory.State.GetSlot(i),i==SelectedIndex);
        var selected=Inventory.State.GetSlot(SelectedIndex);var item=selected.Item;
        goldLabel.text="GOLD "+Inventory.State.Gold;
        details.text=selected.IsEmpty?"Пустой слот\nВыберите предмет.":item.displayName+"  ×"+selected.Count+"\n"+item.category+
            "\n\n"+item.description+"\n\nСтопка: "+item.StackLimit+"   Купить: "+item.buyPrice+"   Продать: "+(item.CanSell?item.sellPrice.ToString():"—");
        splitButton.interactable=!selected.IsEmpty && selected.Count>1 && item.stackable && Inventory.State.FirstEmptySlot()>=0;
        useButton.interactable=!selected.IsEmpty && item is BattleConsumable && UseRequested!=null;
    }
    public void SplitSelected()
    {
        if(!IsOpen)return;var source=Inventory.State.GetSlot(SelectedIndex);
        int destination=Inventory.State.FirstEmptySlot();
        bool ok=destination>=0 && Inventory.State.TrySplit(SelectedIndex,destination,source.Count/2);
        ShowMessage(ok?"Стопка разделена пополам.":"Для деления нужны хотя бы2 предмета и пустой слот.");
    }
    private void RequestUse(){if(IsOpen)UseRequested?.Invoke(SelectedIndex);}
    public void ShowMessage(string text){if(message!=null)message.text=text;}
    public void BeginDrag(int index,Vector2 position)
    {
        if(!IsOpen || Inventory.State.GetSlot(index).IsEmpty)return;
        Select(index);dragging=index;dragIcon.sprite=Inventory.State.GetSlot(index).Item.icon;
        dragIcon.gameObject.SetActive(true);MoveDrag(position);
    }
    public void MoveDrag(Vector2 position)
    {
        if(dragging<0 || dragIcon==null)return;
        var parent=dragIcon.transform.parent as RectTransform;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,position,null,out var local))dragIcon.rectTransform.anchoredPosition=local;
    }
    public bool DropOn(int destination)
    {
        if(!IsOpen || dragging<0)return false;
        bool ok=Inventory.State.TryMoveOrMerge(dragging,destination);EndDrag();
        if(ok)Select(destination);else ShowMessage("Перенос невозможен: проверьте слот и лимит стопки.");return ok;
    }
    public void EndDrag(){dragging=-1;if(dragIcon!=null)dragIcon.gameObject.SetActive(false);}

#if UNITY_EDITOR
    public void BuildLayout()
    {
        if(panel!=null)throw new InvalidOperationException("Инвентарь уже построен; сохранённые объекты не заменяются.");
        var obj=new GameObject("InventoryCanvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        var canvas=obj.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=60;canvas.pixelPerfect=true;
        var scaler=obj.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(960,540);scaler.matchWidthOrHeight=1f;
        panel=Panel(obj.transform,"InventoryPanel",new Color32(22,20,31,250));
        panel.anchorMin=panel.anchorMax=new Vector2(0.5f,0.5f);panel.pivot=new Vector2(0.5f,0.5f);panel.sizeDelta=new Vector2(960,540);
        Label(panel,"Title","INVENTORY  /  I",24,36,490,600,30);
        goldLabel=Label(panel,"Gold","GOLD 0",18,740,490,184,30);goldLabel.alignment=TextAlignmentOptions.MidlineRight;
        slots=new InventorySlotView[30];
        for(int i=0;i<30;i++)
        {
            var rect=Panel(panel,"Slot"+i,new Color32(32,29,41,255));Place(rect,36+(i%6)*72,414-(i/6)*72,64,64);
            var view=rect.gameObject.AddComponent<InventorySlotView>();view.background=rect.GetComponent<Image>();
            view.icon=Panel(rect,"Icon",Color.white).GetComponent<Image>();Place(view.icon.rectTransform,12,16,40,40);view.icon.raycastTarget=false;view.icon.preserveAspect=true;
            view.quantity=Label(rect,"Quantity","",12,4,2,56,16);view.quantity.alignment=TextAlignmentOptions.MidlineRight;slots[i]=view;
        }
        details=Label(panel,"Details","Выберите предмет.",18,500,268,424,210);
        useButton=Button(panel,"Use","Использовать",36,76,130,30);
        splitButton=Button(panel,"Split","Разделить ½",182,76,130,30);
        closeButton=Button(panel,"Close","Закрыть",328,76,132,30);
        message=Label(panel,"Message","",14,36,40,888,28);
        Label(panel,"Controls","I / Esc — закрыть • Стрелки — выбор • Мышь — перенос / обмен / слияние",12,36,12,888,20);
        dragIcon=Panel(obj.transform,"DragIcon",new Color(1,1,1,0.85f)).GetComponent<Image>();
        var ghost=dragIcon.rectTransform;ghost.anchorMin=ghost.anchorMax=ghost.pivot=new Vector2(0.5f,0.5f);ghost.sizeDelta=new Vector2(48,48);dragIcon.raycastTarget=false;dragIcon.preserveAspect=true;dragIcon.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);UnityEditor.EditorUtility.SetDirty(this);
    }
    private static RectTransform Panel(Transform parent,string name,Color color)
    {
        var obj=new GameObject(name,typeof(RectTransform),typeof(Image));obj.transform.SetParent(parent,false);var image=obj.GetComponent<Image>();image.color=color;
        var outline=obj.AddComponent<Outline>();outline.effectColor=new Color32(126,111,85,220);outline.effectDistance=new Vector2(1,-1);return image.rectTransform;
    }
    private static void Place(RectTransform rect,float x,float y,float width,float height)
    {
        rect.anchorMin=rect.anchorMax=rect.pivot=Vector2.zero;rect.anchoredPosition=new Vector2(x,y);rect.sizeDelta=new Vector2(width,height);
    }
    private static TextMeshProUGUI Label(Transform parent,string name,string text,int size,float x,float y,float w,float h)
    {
        var obj=new GameObject(name,typeof(RectTransform),typeof(TextMeshProUGUI));obj.transform.SetParent(parent,false);
        var label=obj.GetComponent<TextMeshProUGUI>();label.font=TMP_Settings.defaultFontAsset;label.text=text;label.fontSize=size;label.color=new Color32(230,219,197,255);
        label.enableAutoSizing=true;label.fontSizeMin=10;label.fontSizeMax=size;label.raycastTarget=false;label.alignment=TextAlignmentOptions.TopLeft;Place(label.rectTransform,x,y,w,h);return label;
    }
    private static Button Button(Transform parent,string name,string text,float x,float y,float w,float h)
    {
        var rect=Panel(parent,name,new Color32(59,45,70,255));Place(rect,x,y,w,h);
        var label=Label(rect,"Label",text,14,4,2,w-8,h-4);label.alignment=TextAlignmentOptions.Center;
        var button=rect.gameObject.AddComponent<Button>();button.targetGraphic=rect.GetComponent<Image>();button.navigation=new Navigation{mode=Navigation.Mode.None};return button;
    }
#endif
}

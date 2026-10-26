using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(BattleInventory),typeof(WorldInputGate))]
public sealed partial class InventoryWindow : MonoBehaviour
{
    public RectTransform panel;
    public InventorySlotView[] slots;
    public TextMeshProUGUI details, goldLabel, message;
    public Button splitButton, useButton, closeButton;
    public Image dragIcon;
    public Button equipButton;
    public Button[] equipmentButtons;
    public TextMeshProUGUI equipmentStats;
    public TextMeshProUGUI resourcesLabel;
    public Image modalBackdrop;
    public InventoryTooltip tooltip;
    public Image characterPortrait;
    public Sprite portraitSprite;
    public TextMeshProUGUI characterLabel, bagLabel;
    public AttributeRowView[] attributeRows;
    public event Action<int> UseRequested;
    public Func<int,bool> CanUseSelection;
    public event Action<int,EquipmentSlot?> EquipRequested;
    public event Action<EquipmentSlot> UnequipRequested;
    public bool IsOpen => panel!=null && panel.gameObject.activeSelf;
    public int SelectedIndex {get;private set;}
    public BattleInventory Inventory {get;private set;}
    private WorldInputGate gate;
    private BattleUnit unit;
    public BattleUnit Unit => unit;
    private int dragging=-1;
    private EquipmentSlot? draggingEquipment;
    private ItemData draggedItem;
    private UnityEngine.Events.UnityAction[] equipmentClicks;
    private int shownHP=-1, shownMaxHP=-1, shownMP=-1, shownMaxMP=-1;

    private void Awake()
    {
        Inventory=GetComponent<BattleInventory>();gate=GetComponent<WorldInputGate>();unit=GetComponent<BattleUnit>();
        for(int i=0;i<(slots?.Length ?? 0);i++)slots[i].Bind(this,i);
        if(splitButton!=null)splitButton.onClick.AddListener(SplitSelected);
        if(useButton!=null)useButton.onClick.AddListener(RequestUse);
        if(closeButton!=null)closeButton.onClick.AddListener(Close);
        if(equipButton!=null)equipButton.onClick.AddListener(RequestEquip);
        equipmentClicks=new UnityEngine.Events.UnityAction[equipmentButtons?.Length ?? 0];
        for(int i=0;i<equipmentClicks.Length;i++)
        {
            var equipmentSlot=(EquipmentSlot)i;equipmentClicks[i]=()=>ClickEquipmentSlot(equipmentSlot);
            equipmentButtons[i].onClick.AddListener(equipmentClicks[i]);
        }
        Close();
    }
    private void OnEnable(){if(Application.isPlaying && Inventory!=null)Inventory.State.Changed+=Refresh;if(unit!=null){unit.ProgressChanged+=OnProgressChanged;unit.StatsChanged+=Refresh;}}
    private void OnDisable(){if(Inventory!=null)Inventory.State.Changed-=Refresh;if(unit!=null){unit.ProgressChanged-=OnProgressChanged;unit.StatsChanged-=Refresh;}Close();}
    private void OnProgressChanged(){if(!IsOpen)return;Refresh();if(unit.LevelsGainedLastAward>0)ShowMessage(GameText.Get("character.level_up",unit.level));}
    private void OnDestroy()
    {
        if(splitButton!=null)splitButton.onClick.RemoveListener(SplitSelected);
        if(useButton!=null)useButton.onClick.RemoveListener(RequestUse);
        if(closeButton!=null)closeButton.onClick.RemoveListener(Close);
        if(equipButton!=null)equipButton.onClick.RemoveListener(RequestEquip);
        for(int i=0;i<(equipmentClicks?.Length ?? 0);i++)if(equipmentButtons[i]!=null)equipmentButtons[i].onClick.RemoveListener(equipmentClicks[i]);
    }
    private void Update()
    {
        if(IsOpen && characterLabel!=null)characterLabel.text=GameText.Get(unit.growthRules!=null?"character.header_progress":"character.header", unit.LocalizedName, unit.level, unit.CurrentExperience,
            unit.AtMaximumLevel?GameText.Get("character.max_level"):GameText.Get("character.next_experience",unit.ExperienceToNextLevel));
        if(IsOpen)RefreshResources();
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
        if(modalBackdrop!=null)modalBackdrop.gameObject.SetActive(true);
        panel.gameObject.SetActive(true);gate.Acquire(this);message.text="";Refresh();return true;
    }
    public void Close()
    {
        if(tooltip!=null)tooltip.Hide();
        EndDrag();if(panel!=null)panel.gameObject.SetActive(false);
        if(modalBackdrop!=null)modalBackdrop.gameObject.SetActive(false);if(gate!=null)gate.Release(this);
    }
    public void Select(int index){if(!IsOpen || index<0 || index>=30)return;SelectedIndex=index;Refresh();}
    public void Refresh()
    {
        if(!IsOpen || Inventory==null)return;
        for(int i=0;i<(slots?.Length ?? 0);i++)slots[i].Render(Inventory.State.GetSlot(i),i==SelectedIndex);
        var selected=Inventory.State.GetSlot(SelectedIndex);var item=selected.Item;
        goldLabel.text=GameText.Get("inventory.gold", Inventory.State.Gold);
        RefreshResources();
        details.text=selected.IsEmpty?GameText.Get("inventory.empty"):
            GameText.Get("inventory.selected",item.LocalizedName,selected.Count,ItemPresentation.TypeName(item),GameText.Get("rarity."+item.rarity.ToString().ToLowerInvariant()),ItemPresentation.BonusText(item));
        if(bagLabel!=null){int occupied=0;for(int i=0;i<InventoryState.Capacity;i++)if(!Inventory.State.GetSlot(i).IsEmpty)occupied++;bagLabel.text=GameText.Get("inventory.bag", occupied, InventoryState.Capacity);}
        if(characterPortrait!=null){var portrait=portraitSprite!=null?portraitSprite:unit.battleSprite;characterPortrait.sprite=portrait;characterPortrait.color=unit.spriteColor;characterPortrait.gameObject.SetActive(portrait!=null);}
        splitButton.interactable=!selected.IsEmpty && selected.Count>1 && item.stackable && Inventory.State.FirstEmptySlot()>=0;
        useButton.interactable=!selected.IsEmpty && item is BattleConsumable && UseRequested!=null && (CanUseSelection==null || CanUseSelection(SelectedIndex));
        var equipment=GetComponent<EquipmentController>();
        if(equipButton!=null)equipButton.interactable=!selected.IsEmpty && item.IsEquipment && item.requiredLevel<=unit.level && EquipRequested!=null;
        for(int i=0;i<(equipmentButtons?.Length ?? 0);i++)
        {
            var slot=(EquipmentSlot)i;var worn=equipment!=null?equipment.State.GetSlot(slot):null;
            var view=equipmentButtons[i].GetComponent<EquipmentSlotView>();
            if(view!=null)view.Render(worn);
            else equipmentButtons[i].GetComponentInChildren<TextMeshProUGUI>().text=slot+": "+(worn!=null?worn.LocalizedName:"—");
            equipmentButtons[i].interactable=equipment!=null && (worn!=null || (item!=null && item.Fits(slot) && item.requiredLevel<=unit.level));
        }
        if(equipmentStats!=null)equipmentStats.text=GameText.Get(attributeRows!=null && attributeRows.Length==CharacterAttributeValues.Count?"character.derived_summary":"character.attributes", unit.EffectiveAttack, unit.EffectiveDefense, unit.EffectiveMagicAttack, unit.EffectiveMagicDefense, unit.Speed);
        if(attributeRows!=null){var values=unit.PrimaryValues;var gear=unit.EquipmentAttributes;foreach(var row in attributeRows)if(row!=null)row.Render(values,gear);}
        if(tooltip!=null)tooltip.RefreshContent();
    }
    private void RefreshResources()
    {
        if(resourcesLabel==null || unit==null || unit.Health==null)return;
        int hp=unit.Health.currentHealth,maxHP=unit.Health.maxHealth,mp=unit.CurrentMP,maxMP=unit.MaxMP;
        if(hp==shownHP && maxHP==shownMaxHP && mp==shownMP && maxMP==shownMaxMP)return;
        shownHP=hp;shownMaxHP=maxHP;shownMP=mp;shownMaxMP=maxMP;
        resourcesLabel.text=GameText.Get("resources.hp_mp", hp, maxHP, mp, maxMP);
    }
    public void SplitSelected()
    {
        if(!IsOpen)return;var source=Inventory.State.GetSlot(SelectedIndex);
        int destination=Inventory.State.FirstEmptySlot();
        bool ok=destination>=0 && Inventory.State.TrySplit(SelectedIndex,destination,source.Count/2);
        ShowMessage(ok?GameText.Get("inventory.split_success"):GameText.Get("inventory.split_failed"));
    }
    private void RequestUse(){if(IsOpen)UseRequested?.Invoke(SelectedIndex);}
    private void RequestEquip(){if(IsOpen)EquipRequested?.Invoke(SelectedIndex,null);}
    private void ClickEquipmentSlot(EquipmentSlot slot)
    {
        if(!IsOpen)return;var selected=Inventory.State.GetSlot(SelectedIndex);
        if(!selected.IsEmpty && selected.Item.Fits(slot))EquipRequested?.Invoke(SelectedIndex,slot);
        else UnequipRequested?.Invoke(slot);
    }
    public void ShowMessage(string text){if(message!=null)message.text=text;}
    public void BeginDrag(int index,Vector2 position)
    {
        if(!IsOpen || index<0 || index>=InventoryState.Capacity || Inventory.State.GetSlot(index).IsEmpty)return;
        EndDrag();
        if(tooltip!=null)tooltip.Hide();
        Select(index);dragging=index;draggedItem=Inventory.State.GetSlot(index).Item;dragIcon.sprite=draggedItem.icon;
        dragIcon.gameObject.SetActive(true);MoveDrag(position);
        HighlightEquipmentTargets();
    }
    public void BeginEquipmentDrag(EquipmentSlot slot,Vector2 position)
    {
        if(!IsOpen || (int)slot<0 || (int)slot>=EquipmentState.SlotCount)return;
        var equipment=GetComponent<EquipmentController>();if(equipment==null || equipment.State.GetSlot(slot)==null)return;
        EndDrag();if(tooltip!=null)tooltip.Hide();draggingEquipment=slot;draggedItem=equipment.State.GetSlot(slot);
        dragIcon.sprite=draggedItem.icon;dragIcon.gameObject.SetActive(true);MoveDrag(position);HighlightEquipmentTargets();
    }
    private bool DragSourceValid()
    {
        if(draggedItem==null)return false;
        return draggingEquipment.HasValue?GetComponent<EquipmentController>().State.GetSlot(draggingEquipment.Value)==draggedItem:
            dragging>=0 && Inventory.State.GetSlot(dragging).Item==draggedItem;
    }
    private void HighlightEquipmentTargets()
    {
        var equipment=GetComponent<EquipmentController>();
        if(equipment==null || equipmentButtons==null)return;
        foreach(var button in equipmentButtons)
        {
            if(button==null)continue;var view=button.GetComponent<EquipmentSlotView>();if(view==null)continue;
            view.SetDropAllowed(draggedItem==null?(bool?)null:draggingEquipment.HasValue?
                equipment.CanMove(draggingEquipment.Value,view.slot):equipment.CanEquip(dragging,view.slot));
        }
    }
    public void MoveDrag(Vector2 position)
    {
        if((dragging<0 && !draggingEquipment.HasValue) || dragIcon==null)return;
        var parent=dragIcon.transform.parent as RectTransform;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,position,null,out var local))dragIcon.rectTransform.anchoredPosition=local;
    }
    public bool DropOn(int destination)
    {
        if(!IsOpen || !DragSourceValid()){EndDrag();return false;}
        bool gear=draggingEquipment.HasValue;
        bool ok=gear?GetComponent<EquipmentController>().TryUnequipTo(draggingEquipment.Value,destination):Inventory.State.TryMoveOrMerge(dragging,destination);
        EndDrag();if(ok)Select(destination);else ShowMessage(GameText.Get(gear && Inventory.State.FirstEmptySlot()<0?"inventory.full":"inventory.move_failed"));Refresh();return ok;
    }
    public bool DropOnEquipment(EquipmentSlot slot)
    {
        if(!IsOpen || !DragSourceValid()){EndDrag();return false;}
        var equipment=GetComponent<EquipmentController>();bool ok=draggingEquipment.HasValue?
            equipment.TryMove(draggingEquipment.Value,slot):equipment.TryEquip(dragging,slot);
        EndDrag();ShowMessage(GameText.Get(ok?"equipment.equipped":"equipment.equip_failed"));Refresh();return ok;
    }
    public void EndDrag(){dragging=-1;draggingEquipment=null;draggedItem=null;if(dragIcon!=null)dragIcon.gameObject.SetActive(false);HighlightEquipmentTargets();}

#if UNITY_EDITOR
    public void BuildModalBackdrop()
    {
        if(panel==null)throw new InvalidOperationException("Сначала нужен готовый инвентарь.");
        var existing=panel.parent.Find("InventoryBackdrop");
        modalBackdrop=existing!=null?existing.GetComponent<Image>():Panel(panel.parent,"InventoryBackdrop",new Color32(12,10,18,255)).GetComponent<Image>();
        modalBackdrop.transform.SetAsFirstSibling();var rect=modalBackdrop.rectTransform;
        rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
        modalBackdrop.gameObject.SetActive(false);UnityEditor.EditorUtility.SetDirty(this);
    }
    public void BuildResourceHeader()
    {
        if(panel==null)throw new InvalidOperationException("Сначала нужен готовый инвентарь.");
        var existing=panel.Find("Resources");
        resourcesLabel=existing!=null?existing.GetComponent<TextMeshProUGUI>():Label(panel,"Resources","HP / MP",16,330,490,390,30);
        Place(panel.Find("Title").GetComponent<RectTransform>(),36,490,270,30);
        UnityEditor.EditorUtility.SetDirty(this);
    }
    public void BuildEquipmentLayout()
    {
        if(panel==null || equipButton!=null)throw new InvalidOperationException("Нужен готовый инвентарь без панели экипировки.");
        var buttons=new[]{useButton,splitButton,closeButton};var xs=new[]{36f,142f,356f};var widths=new[]{98f,98f,104f};
        for(int i=0;i<3;i++)
        {
            Place(buttons[i].GetComponent<RectTransform>(),xs[i],76,widths[i],30);
            Place(buttons[i].GetComponentInChildren<TextMeshProUGUI>().rectTransform,4,2,widths[i]-8,26);
        }
        equipButton=Button(panel,"Equip",GameText.Get("inventory.equip"),248,76,100,30);
        Label(panel,"EquipmentTitle",GameText.Get("inventory.equipment_hint"),12,500,244,424,20);
        equipmentButtons=new Button[7];
        for(int i=0;i<7;i++)equipmentButtons[i]=Button(panel,"Equipment"+(EquipmentSlot)i,(EquipmentSlot)i+": —",500,216-i*23,424,20);
        equipmentStats=Label(panel,"EquipmentStats","",12,500,45,424,28);
        Place(message.rectTransform,36,40,424,28);UnityEditor.EditorUtility.SetDirty(this);
    }
    public void BuildLayout()
    {
        if(panel!=null)throw new InvalidOperationException("Инвентарь уже построен; сохранённые объекты не заменяются.");
        var obj=new GameObject("InventoryCanvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        var canvas=obj.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=60;canvas.pixelPerfect=true;
        var scaler=obj.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(960,540);scaler.matchWidthOrHeight=1f;
        panel=Panel(obj.transform,"InventoryPanel",new Color32(22,20,31,250));
        panel.anchorMin=panel.anchorMax=new Vector2(0.5f,0.5f);panel.pivot=new Vector2(0.5f,0.5f);panel.sizeDelta=new Vector2(960,540);
        Label(panel,"Title",GameText.Get("inventory.title"),24,36,490,600,30);
        goldLabel=Label(panel,"Gold","GOLD 0",18,740,490,184,30);goldLabel.alignment=TextAlignmentOptions.MidlineRight;
        slots=new InventorySlotView[30];
        for(int i=0;i<30;i++)
        {
            var rect=Panel(panel,"Slot"+i,new Color32(32,29,41,255));Place(rect,36+(i%6)*72,414-(i/6)*72,64,64);
            var view=rect.gameObject.AddComponent<InventorySlotView>();view.background=rect.GetComponent<Image>();
            view.icon=Panel(rect,"Icon",Color.white).GetComponent<Image>();Place(view.icon.rectTransform,12,16,40,40);view.icon.raycastTarget=false;view.icon.preserveAspect=true;
            view.quantity=Label(rect,"Quantity","",12,4,2,56,16);view.quantity.alignment=TextAlignmentOptions.MidlineRight;slots[i]=view;
        }
        details=Label(panel,"Details",GameText.Get("inventory.select"),18,500,268,424,210);
        useButton=Button(panel,"Use",GameText.Get("inventory.use"),36,76,130,30);
        splitButton=Button(panel,"Split",GameText.Get("inventory.split"),182,76,130,30);
        closeButton=Button(panel,"Close",GameText.Get("menu.close"),328,76,132,30);
        message=Label(panel,"Message","",14,36,40,888,28);
        Label(panel,"Controls",GameText.Get("inventory.controls"),12,36,12,888,20);
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

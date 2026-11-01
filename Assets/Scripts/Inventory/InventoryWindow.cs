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
    public Button useButton, closeButton;
    public Image dragIcon;
    public Button equipButton;
    public Button[] equipmentButtons;
    public TextMeshProUGUI equipmentStats;
    public TextMeshProUGUI resourcesLabel;
    public Image modalBackdrop;
    public InventoryTooltip tooltip;
    public TextMeshProUGUI bagLabel;
    public event Action<int> UseRequested;
    public Func<int,bool> CanUseSelection;
    public event Action<int,EquipmentSlot?> EquipRequested;
    public event Action<EquipmentSlot> UnequipRequested;
    public bool IsOpen => panel!=null && panel.gameObject.activeSelf;
    public int SelectedIndex {get;private set;}
    public BattleInventory Inventory {get;private set;}
    private WorldInputGate gate;
    private BattleUnit unit;
    private int dragging=-1;
    private UnityEngine.Events.UnityAction[] equipmentClicks;
    private int shownHP=-1, shownMaxHP=-1, shownMP=-1, shownMaxMP=-1;

    private void Awake()
    {
        Inventory=GetComponent<BattleInventory>();gate=GetComponent<WorldInputGate>();unit=GetComponent<BattleUnit>();
        for(int i=0;i<(slots?.Length ?? 0);i++)slots[i].Bind(this,i);
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
    private void OnEnable(){if(Application.isPlaying && Inventory!=null)Inventory.State.Changed+=Refresh;}
    private void OnDisable(){if(Inventory!=null)Inventory.State.Changed-=Refresh;Close();}
    private void OnDestroy()
    {
        if(useButton!=null)useButton.onClick.RemoveListener(RequestUse);
        if(closeButton!=null)closeButton.onClick.RemoveListener(Close);
        if(equipButton!=null)equipButton.onClick.RemoveListener(RequestEquip);
        for(int i=0;i<(equipmentClicks?.Length ?? 0);i++)if(equipmentButtons[i]!=null)equipmentButtons[i].onClick.RemoveListener(equipmentClicks[i]);
    }
    private void Update()
    {
        if(IsOpen)RefreshResources();
        var keyboard=Keyboard.current;if(keyboard==null)return;
        if(keyboard.iKey.wasPressedThisFrame){if(IsOpen)Close();else Open();return;}
        if(!IsOpen)return;
        if(keyboard.escapeKey.wasPressedThisFrame){Close();return;}
        int offset=keyboard.leftArrowKey.wasPressedThisFrame?-1:keyboard.rightArrowKey.wasPressedThisFrame?1:
            keyboard.upArrowKey.wasPressedThisFrame?-6:keyboard.downArrowKey.wasPressedThisFrame?6:0;
        if(offset!=0)Select(Mathf.Clamp(SelectedIndex+offset,0,InventoryState.Capacity-1));
    }
    public bool Open()
    {
        if(panel==null || !isActiveAndEnabled || unit==null || !unit.IsAlive ||
            (BattleTransition.Instance!=null && BattleTransition.Instance.EncounterActive))return false;
        if(modalBackdrop!=null)modalBackdrop.gameObject.SetActive(true);
        panel.gameObject.SetActive(true);gate.Acquire(this);if(message!=null)message.text="";Refresh();return true;
    }
    public void Close()
    {
        if(tooltip!=null)tooltip.Hide();
        EndDrag();if(panel!=null)panel.gameObject.SetActive(false);
        if(modalBackdrop!=null)modalBackdrop.gameObject.SetActive(false);if(gate!=null)gate.Release(this);
    }
    public void Select(int index){if(!IsOpen || index<0 || index>=InventoryState.Capacity)return;SelectedIndex=index;Refresh();}
    public void Refresh()
    {
        if(!IsOpen || Inventory==null)return;
        for(int i=0;i<(slots?.Length ?? 0);i++)slots[i].Render(Inventory.State.GetSlot(i),i==SelectedIndex);
        var selected=Inventory.State.GetSlot(SelectedIndex);var item=selected.Item;
        if(goldLabel!=null)goldLabel.text=$"GOLD {Inventory.State.Gold}";
        RefreshResources();
        if(details!=null)details.text=selected.IsEmpty?"Select an item.":ItemPresentation.Description(item,selected.Count);
        if(bagLabel!=null){int occupied=0;for(int i=0;i<InventoryState.Capacity;i++)if(!Inventory.State.GetSlot(i).IsEmpty)occupied++;bagLabel.text=$"BAG   {occupied} / {InventoryState.Capacity}";}
        if(useButton!=null)useButton.interactable=!selected.IsEmpty && item is BattleConsumable && UseRequested!=null && (CanUseSelection==null || CanUseSelection(SelectedIndex));
        var equipment=GetComponent<EquipmentController>();
        if(equipButton!=null)equipButton.interactable=!selected.IsEmpty && item.IsEquipment && EquipRequested!=null;
        for(int i=0;i<(equipmentButtons?.Length ?? 0);i++)
        {
            var slot=(EquipmentSlot)i;var worn=equipment!=null?equipment.State.GetSlot(slot):null;
            var view=equipmentButtons[i].GetComponent<EquipmentSlotView>();
            if(view!=null)view.Render(worn);
            else equipmentButtons[i].GetComponentInChildren<TextMeshProUGUI>().text=slot+": "+(worn!=null?worn.displayName:"—");
            equipmentButtons[i].interactable=equipment!=null && (worn!=null || (item!=null && item.Fits(slot)));
        }
        if(equipmentStats!=null && unit!=null)equipmentStats.text=$"ATK {unit.EffectiveAttack}     DEF {unit.EffectiveDefense}";
        if(tooltip!=null)tooltip.RefreshContent();
    }
    private void RefreshResources()
    {
        if(resourcesLabel==null || unit==null || unit.Health==null)return;
        int hp=unit.Health.currentHealth,maxHP=unit.Health.maxHealth,mp=unit.CurrentMP,maxMP=unit.MaxMP;
        if(hp==shownHP && maxHP==shownMaxHP && mp==shownMP && maxMP==shownMaxMP)return;
        shownHP=hp;shownMaxHP=maxHP;shownMP=mp;shownMaxMP=maxMP;
        resourcesLabel.text=$"HP {hp} / {maxHP}    MP {mp} / {maxMP}";
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
        if(!IsOpen || Inventory.State.GetSlot(index).IsEmpty)return;
        if(tooltip!=null)tooltip.Hide();
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
        if(ok)Select(destination);else ShowMessage("Cannot move this item. Check the slot and stack limit.");return ok;
    }
    public void EndDrag(){dragging=-1;if(dragIcon!=null)dragIcon.gameObject.SetActive(false);}

}

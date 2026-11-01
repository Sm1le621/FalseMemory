using UnityEngine;

[RequireComponent(typeof(BattleUnit),typeof(BattleInventory))]
public sealed class EquipmentController : MonoBehaviour
{
    public EquipmentState State {get;}=new EquipmentState();
    private BattleUnit unit;
    private BattleInventory inventory;
    private InventoryWindow window;
    private void Awake(){unit=GetComponent<BattleUnit>();inventory=GetComponent<BattleInventory>();window=GetComponent<InventoryWindow>();}
    private void OnEnable()
    {
        if(window==null)return;
        window.EquipRequested+=HandleEquip;window.UnequipRequested+=HandleUnequip;State.Changed+=window.Refresh;
    }
    private void OnDisable()
    {
        if(window==null)return;
        window.EquipRequested-=HandleEquip;window.UnequipRequested-=HandleUnequip;State.Changed-=window.Refresh;
    }
    private bool CanChange => isActiveAndEnabled && unit!=null && unit.IsAlive &&
        (BattleTransition.Instance==null || !BattleTransition.Instance.EncounterActive);
    public bool TryEquip(int index,EquipmentSlot? target=null)
    {
        if(!CanChange || index<0 || index>=InventoryState.Capacity)return false;
        var item=inventory.State.GetSlot(index).Item;if(item==null)return false;
        var slot=target ?? (item.category==ItemCategory.Weapon?EquipmentSlot.Weapon:item.equipmentSlot);
        return State.TryEquip(inventory.State,index,slot);
    }
    public bool TryUnequip(EquipmentSlot slot) => CanChange && State.TryUnequip(inventory.State,slot);
    private void HandleEquip(int index,EquipmentSlot? target)
    {
        bool ok=TryEquip(index,target);window.ShowMessage(ok?"Item equipped.":"Cannot equip this item.");window.Refresh();
    }
    private void HandleUnequip(EquipmentSlot slot)
    {
        bool ok=TryUnequip(slot);window.ShowMessage(ok?"Item unequipped.":"Cannot unequip: inventory is full or the slot is empty.");window.Refresh();
    }
}

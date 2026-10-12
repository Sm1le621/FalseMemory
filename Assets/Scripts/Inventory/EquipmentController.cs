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
        if(!CanChange || index<0 || index>=30)return false;
        var item=inventory.State.GetSlot(index).Item;if(item==null)return false;
        var slot=target ?? (item.category==ItemCategory.Weapon?EquipmentSlot.Weapon:item.category==ItemCategory.Accessory?
            (State.GetSlot(EquipmentSlot.Accessory1)==null?EquipmentSlot.Accessory1:State.GetSlot(EquipmentSlot.Accessory2)==null?EquipmentSlot.Accessory2:EquipmentSlot.Accessory1):item.equipmentSlot);
        return State.TryEquip(inventory.State,index,slot,unit.level);
    }
    public bool TryUnequip(EquipmentSlot slot) => CanChange && State.TryUnequip(inventory.State,slot);
    private void HandleEquip(int index,EquipmentSlot? target)
    {
        bool ok=TryEquip(index,target);window.ShowMessage(ok?GameText.Get("equipment.equipped"):GameText.Get("equipment.equip_failed"));window.Refresh();
    }
    private void HandleUnequip(EquipmentSlot slot)
    {
        bool ok=TryUnequip(slot);window.ShowMessage(ok?GameText.Get("equipment.unequipped"):GameText.Get("equipment.unequip_failed"));window.Refresh();
    }
}

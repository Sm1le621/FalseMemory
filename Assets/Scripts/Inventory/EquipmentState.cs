using System;
using UnityEngine;

[Serializable]
public sealed class EquipmentState
{
    public const int SlotCount=9;
    [SerializeField] private ItemData[] slots=new ItemData[SlotCount];
    private bool changing;
    private Func<ItemData,CharacterAttributeValues,bool> requirementsCheck;
    public void BindRequirements(Func<ItemData,CharacterAttributeValues,bool> check) => requirementsCheck=check;
    public CharacterAttributeValues AttributesExcluding(EquipmentSlot excluded, EquipmentSlot? emptied=null, ItemData replacement=null)
    {
        EnsureSize();var values=new CharacterAttributeValues();
        for(int i=0;i<slots.Length;i++)
        {
            if(i==(int)excluded)continue;
            var item=emptied.HasValue && i==(int)emptied.Value?replacement:slots[i];
            if(item!=null)values+=item.bonuses.attributes;
        }
        return values;
    }
    private bool RequirementsAllow(ItemData item,EquipmentSlot target,EquipmentSlot? emptied=null,ItemData replacement=null)
        => item==null || requirementsCheck==null || requirementsCheck(item,AttributesExcluding(target,emptied,replacement));
    public event Action Changed;
    public ItemData GetSlot(EquipmentSlot slot)
    {
        if(!Valid(slot))throw new ArgumentOutOfRangeException(nameof(slot));EnsureSize();return slots[(int)slot];
    }
    public ItemStatBonus Bonuses
    {
        get {EnsureSize();var total=new ItemStatBonus();foreach(var item in slots)if(item!=null)total+=item.bonuses;return total;}
    }
    private void EnsureSize(){if(slots==null || slots.Length<SlotCount)Array.Resize(ref slots,SlotCount);}
    private bool HandsAllow(EquipmentSlot target,ItemData incoming,EquipmentSlot? emptied=null)
    {
        var weapon=target==EquipmentSlot.Weapon?incoming:emptied==EquipmentSlot.Weapon?null:GetSlot(EquipmentSlot.Weapon);
        var offhand=target==EquipmentSlot.OffHand?incoming:emptied==EquipmentSlot.OffHand?null:GetSlot(EquipmentSlot.OffHand);
        return weapon==null || !weapon.twoHanded || offhand==null;
    }
    public bool CanEquip(InventoryState inventory,int index,EquipmentSlot slot,int level)
    {
        if(changing || inventory==null || index<0 || index>=InventoryState.Capacity || !Valid(slot))return false;
        var source=inventory.GetSlot(index);var item=source.Item;
        return !source.IsEmpty && source.Count==1 && item.Fits(slot) && level>=Math.Max(1,item.requiredLevel) && HandsAllow(slot,item) && RequirementsAllow(item,slot);
    }
    public bool TryEquip(InventoryState inventory,int index,EquipmentSlot slot,int level)
    {
        if(!CanEquip(inventory,index,slot,level))return false;
        var source=inventory.GetSlot(index);var item=source.Item;
        if(source.IsEmpty || source.Count!=1 || !item.Fits(slot) || level<Math.Max(1,item.requiredLevel))return false;
        var previous=slots[(int)slot];changing=true;
        try
        {
            slots[(int)slot]=item;
            if(!inventory.TryExchangeSingle(index,item,previous)){slots[(int)slot]=previous;return false;}
            Changed?.Invoke();return true;
        }
        finally { changing=false; }
    }
    public bool TryUnequip(InventoryState inventory,EquipmentSlot slot)
    {
        if(changing || inventory==null || !Valid(slot))return false;
        var item=GetSlot(slot);if(item==null || !inventory.CanAdd(item,1))return false;
        changing=true;
        try
        {
            slots[(int)slot]=null;
            if(!inventory.TryAdd(item,1)){slots[(int)slot]=item;return false;}
            Changed?.Invoke();return true;
        }
        finally { changing=false; }
    }
    public bool CanMove(EquipmentSlot from,EquipmentSlot to,int level)
    {
        if(changing || !Valid(from) || !Valid(to))return false;
        var item=GetSlot(from);var other=GetSlot(to);
        if(item==null || !item.Fits(to) || level<Math.Max(1,item.requiredLevel))return false;
        if(from==to)return true;
        if(other!=null && (!other.Fits(from) || level<Math.Max(1,other.requiredLevel)))return false;
        var weapon=from==EquipmentSlot.Weapon?other:to==EquipmentSlot.Weapon?item:GetSlot(EquipmentSlot.Weapon);
        var offhand=from==EquipmentSlot.OffHand?other:to==EquipmentSlot.OffHand?item:GetSlot(EquipmentSlot.OffHand);
        return (weapon==null || !weapon.twoHanded || offhand==null) && RequirementsAllow(item,to,from,other) && RequirementsAllow(other,from,to,item);
    }
    public bool TryMove(EquipmentSlot from,EquipmentSlot to,int level)
    {
        if(!CanMove(from,to,level))return false;if(from==to)return true;
        changing=true;
        try {var previous=slots[(int)to];slots[(int)to]=slots[(int)from];slots[(int)from]=previous;Changed?.Invoke();return true;}
        finally {changing=false;}
    }
    public bool TryUnequipTo(InventoryState inventory,EquipmentSlot from,int destination,int level)
    {
        if(changing || inventory==null || !Valid(from) || destination<0 || destination>=InventoryState.Capacity)return false;
        var item=GetSlot(from);if(item==null)return false;var entry=inventory.GetSlot(destination);var replacement=entry.Item;
        if(!entry.IsEmpty && (entry.Count!=1 || !replacement.Fits(from) || level<Math.Max(1,replacement.requiredLevel)))return false;
        if(!HandsAllow(from,replacement) || !RequirementsAllow(replacement,from))return false;
        changing=true;
        try
        {
            slots[(int)from]=replacement;
            if(!inventory.TryExchangeSingle(destination,replacement,item)){slots[(int)from]=item;return false;}
            Changed?.Invoke();return true;
        }
        finally {changing=false;}
    }
    private static bool Valid(EquipmentSlot slot) => (int)slot>=0 && (int)slot<SlotCount;
}

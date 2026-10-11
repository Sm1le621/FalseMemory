using System;
using UnityEngine;

[Serializable]
public sealed class EquipmentState
{
    [SerializeField] private ItemData[] slots=new ItemData[7];
    private bool changing;
    public event Action Changed;
    public ItemData GetSlot(EquipmentSlot slot)
    {
        if(!Valid(slot))throw new ArgumentOutOfRangeException(nameof(slot));return slots[(int)slot];
    }
    public ItemStatBonus Bonuses
    {
        get {var total=new ItemStatBonus();foreach(var item in slots)if(item!=null)total+=item.bonuses;return total;}
    }
    public bool TryEquip(InventoryState inventory,int index,EquipmentSlot slot,int level)
    {
        if(changing || inventory==null || index<0 || index>=InventoryState.Capacity || !Valid(slot))return false;
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
        var item=slots[(int)slot];if(item==null || !inventory.CanAdd(item,1))return false;
        changing=true;
        try
        {
            slots[(int)slot]=null;
            if(!inventory.TryAdd(item,1)){slots[(int)slot]=item;return false;}
            Changed?.Invoke();return true;
        }
        finally { changing=false; }
    }
    private static bool Valid(EquipmentSlot slot) => (int)slot>=0 && (int)slot<7;
}

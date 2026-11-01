using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class InventorySlot
{
    [SerializeField] private ItemData item;
    [SerializeField] private int count;
    public ItemData Item => item;
    public int Count => count;
    public bool IsEmpty => item==null || count<=0;
    internal InventorySlot(ItemData definition,int amount) { item=definition;count=amount; }
}

// One serializable session model, independent of scene lifetime and UI.
[Serializable]
public sealed class InventoryState
{
    public const int Capacity=30;
    private static readonly InventorySlot Empty=new InventorySlot(null,0);
    [SerializeField] private InventorySlot[] slots=new InventorySlot[Capacity];
    [SerializeField] private int gold;
    public int Gold => gold;
    public event Action Changed;
    public bool TryAddGold(int amount)
    {
        if(amount<=0 || (long)gold+amount>int.MaxValue)return false;
        gold+=amount;Changed?.Invoke();return true;
    }
    public bool TrySpendGold(int amount)
    {
        if(amount<=0 || amount>gold)return false;
        gold-=amount;Changed?.Invoke();return true;
    }
    public IEnumerable<ItemData> Definitions
    {
        get { var seen=new HashSet<ItemData>();foreach(var slot in slots)if(slot!=null && !slot.IsEmpty && seen.Add(slot.Item))yield return slot.Item; }
    }
    public InventorySlot GetSlot(int index)
    {
        if(!Valid(index))throw new ArgumentOutOfRangeException(nameof(index));
        return slots[index] ?? Empty;
    }
    public int Count(ItemData item)
    {
        if(item==null)return 0;
        long total=0;foreach(var slot in slots)if(slot!=null && slot.Item==item)total+=slot.Count;
        return (int)Math.Min(int.MaxValue,total);
    }
    private bool Conflicts(ItemData item)
    {
        if(string.IsNullOrWhiteSpace(item.id))return false;
        foreach(var other in Definitions)if(other!=item && other.id==item.id)return true;
        return false;
    }
    public bool CanAdd(ItemData item,int amount)
    {
        if(item==null || amount<=0 || Conflicts(item))return false;
        long space=0;
        foreach(var slot in slots)
            if(slot==null || slot.IsEmpty)space+=item.StackLimit;
            else if(slot.Item==item)space+=Math.Max(0,item.StackLimit-slot.Count);
        return space>=amount;
    }
    public bool TryAdd(ItemData item,int amount)
    {
        if(!CanAdd(item,amount))return false;
        int remaining=amount;
        for(int i=0;i<Capacity && remaining>0;i++)
        {
            var slot=GetSlot(i);if(slot.Item!=item || slot.IsEmpty)continue;
            int add=Math.Min(remaining,Math.Max(0,item.StackLimit-slot.Count));
            slots[i]=new InventorySlot(item,slot.Count+add);remaining-=add;
        }
        for(int i=0;i<Capacity && remaining>0;i++)if(GetSlot(i).IsEmpty)
        {
            int add=Math.Min(remaining,item.StackLimit);slots[i]=new InventorySlot(item,add);remaining-=add;
        }
        Changed?.Invoke();return true;
    }
    public bool TryRemove(ItemData item,int amount)
    {
        if(item==null || amount<=0 || Count(item)<amount)return false;
        for(int i=0;i<Capacity && amount>0;i++)
        {
            var slot=GetSlot(i);if(slot.Item!=item)continue;
            int take=Math.Min(amount,slot.Count);int left=slot.Count-take;
            slots[i]=left==0?null:new InventorySlot(item,left);amount-=take;
        }
        Changed?.Invoke();return true;
    }
    public bool TryMoveOrMerge(int from,int to)
    {
        if(!Valid(from) || !Valid(to) || from==to || GetSlot(from).IsEmpty)return false;
        var source=GetSlot(from);var target=GetSlot(to);
        if(!target.IsEmpty && source.Item==target.Item && source.Item.stackable)
        {
            int transfer=Math.Min(source.Count,Math.Max(0,target.Item.StackLimit-target.Count));
            if(transfer==0)return false;
            slots[to]=new InventorySlot(target.Item,target.Count+transfer);
            slots[from]=source.Count==transfer?null:new InventorySlot(source.Item,source.Count-transfer);
        }
        else {slots[to]=source;slots[from]=target.IsEmpty?null:target;}
        Changed?.Invoke();return true;
    }
    internal bool TryExchangeSingle(int index,ItemData expected,ItemData replacement)
    {
        if(!Valid(index) || GetSlot(index).Item!=expected || GetSlot(index).Count!=1 ||
            (replacement!=null && (replacement.StackLimit!=1 || Conflicts(replacement))))return false;
        slots[index]=replacement==null?null:new InventorySlot(replacement,1);Changed?.Invoke();return true;
    }
    private static bool Valid(int index) => index>=0 && index<Capacity;
}

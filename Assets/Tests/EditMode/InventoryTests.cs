using NUnit.Framework;
using UnityEngine;

public sealed class InventoryTests
{
    private ItemData item, other;
    private InventoryState inventory;
    [SetUp] public void Setup()
    {
        inventory=new InventoryState();item=ScriptableObject.CreateInstance<ItemData>();other=ScriptableObject.CreateInstance<ItemData>();
        item.id="test.item";other.id="test.other";item.maxStack=other.maxStack=10;
    }
    [TearDown] public void Cleanup(){Object.DestroyImmediate(item);Object.DestroyImmediate(other);}
    [Test] public void ThirtySlotsFillAndRejectWithoutPartialAddition()
    {
        Assert.That(inventory.TryAdd(item,299),Is.True);
        Assert.That(inventory.TryAdd(item,2),Is.False);Assert.That(inventory.Count(item),Is.EqualTo(299));
        Assert.That(inventory.TryAdd(item,1),Is.True);Assert.That(inventory.FirstEmptySlot(),Is.EqualTo(-1));
        Assert.That(inventory.TryAdd(other,1),Is.False);Assert.That(inventory.Count(other),Is.Zero);
    }
    [Test] public void NonStackableAndEquipmentUseOneSlotEach()
    {
        item.category=ItemCategory.Weapon;Assert.That(inventory.TryAdd(item,30),Is.True);
        for(int i=0;i<30;i++)Assert.That(inventory.GetSlot(i).Count,Is.EqualTo(1));
        Assert.That(inventory.TryAdd(item,1),Is.False);
    }
    [Test] public void MoveSwapMergeAndSplitPreserveQuantities()
    {
        inventory.TryAdd(item,15);inventory.TryAdd(other,3);
        Assert.That(inventory.TryMoveOrMerge(0,8),Is.True);
        Assert.That(inventory.GetSlot(0).IsEmpty,Is.True);
        Assert.That(inventory.TryMoveOrMerge(8,2),Is.True);
        Assert.That(inventory.GetSlot(8).Item,Is.SameAs(other));
        Assert.That(inventory.TryMoveOrMerge(1,2),Is.False,"A full same-item stack stays unchanged");
        Assert.That(inventory.TrySplit(2,0,4),Is.True);
        Assert.That(inventory.TryMoveOrMerge(1,2),Is.True);
        Assert.That(inventory.GetSlot(2).Count,Is.EqualTo(10));Assert.That(inventory.GetSlot(1).Count,Is.EqualTo(1));
        Assert.That(inventory.Count(item),Is.EqualTo(15));Assert.That(inventory.Count(other),Is.EqualTo(3));
    }
    [Test] public void RejectedTransactionsDoNotRaiseEventsOrMutate()
    {
        int events=0;inventory.Changed+=()=>events++;
        inventory.TryAdd(item,5);
        Assert.That(inventory.TryAdd(null,1),Is.False);Assert.That(inventory.TryAdd(item,-1),Is.False);
        Assert.That(inventory.TryRemove(item,6),Is.False);Assert.That(inventory.TrySplit(0,1,5),Is.False);
        Assert.That(inventory.TryMoveOrMerge(-1,30),Is.False);Assert.That(inventory.TrySplit(0,31,2),Is.False);
        Assert.That(inventory.Count(item),Is.EqualTo(5));Assert.That(events,Is.EqualTo(1));
    }
    [Test] public void RemovalAcrossStacksIsAtomicAndFreesSlots()
    {
        inventory.TryAdd(item,15);Assert.That(inventory.TryRemove(item,16),Is.False);
        Assert.That(inventory.TryRemove(item,12),Is.True);Assert.That(inventory.Count(item),Is.EqualTo(3));
        Assert.That(inventory.GetSlot(0).IsEmpty,Is.True);Assert.That(inventory.GetSlot(1).Count,Is.EqualTo(3));
    }
    [Test] public void ConflictingStableIdIsRejected()
    {
        inventory.TryAdd(item,1);other.id=item.id;
        Assert.That(inventory.TryAdd(other,1),Is.False);Assert.That(inventory.Count(item),Is.EqualTo(1));
    }
    [Test] public void GoldHasOneBalanceAndRejectsUnderflowAndOverflow()
    {
        Assert.That(inventory.TrySpendGold(1),Is.False);Assert.That(inventory.TryAddGold(-1),Is.False);
        Assert.That(inventory.TryAddGold(int.MaxValue),Is.True);Assert.That(inventory.TryAddGold(1),Is.False);
        Assert.That(inventory.TrySpendGold(int.MaxValue),Is.True);Assert.That(inventory.Gold,Is.Zero);
    }
}

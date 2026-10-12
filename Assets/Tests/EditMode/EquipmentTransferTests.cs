using NUnit.Framework;
using UnityEngine;

public sealed class EquipmentTransferTests
{
    private ItemData sword,other,shield,filler;
    [SetUp] public void Setup()
    {
        sword=ScriptableObject.CreateInstance<ItemData>();sword.id="transfer.sword";sword.category=ItemCategory.Weapon;
        other=ScriptableObject.CreateInstance<ItemData>();other.id="transfer.other";other.category=ItemCategory.Weapon;
        shield=ScriptableObject.CreateInstance<ItemData>();shield.id="transfer.shield";shield.category=ItemCategory.Armor;shield.equipmentSlot=EquipmentSlot.OffHand;
        filler=ScriptableObject.CreateInstance<ItemData>();filler.id="transfer.filler";filler.stackable=false;
    }
    [TearDown] public void Cleanup(){Object.DestroyImmediate(sword);Object.DestroyImmediate(other);Object.DestroyImmediate(shield);Object.DestroyImmediate(filler);}
    [Test] public void OffHandIsAppendedAndTwoHandedConflictsPreserveAllItems()
    {
        Assert.That((int)EquipmentSlot.Gloves,Is.EqualTo(7));Assert.That((int)EquipmentSlot.OffHand,Is.EqualTo(8));
        var bag=new InventoryState();var gear=new EquipmentState();bag.TryAdd(sword,1);bag.TryAdd(shield,1);bag.TryAdd(other,1);
        Assert.That(gear.TryEquip(bag,0,EquipmentSlot.Weapon,1),Is.True);Assert.That(gear.TryEquip(bag,1,EquipmentSlot.OffHand,1),Is.True);
        other.twoHanded=true;Assert.That(gear.TryEquip(bag,2,EquipmentSlot.Weapon,1),Is.False);
        Assert.That(gear.GetSlot(EquipmentSlot.Weapon),Is.SameAs(sword));Assert.That(gear.GetSlot(EquipmentSlot.OffHand),Is.SameAs(shield));Assert.That(bag.Count(other),Is.EqualTo(1));
        Assert.That(gear.TryUnequipTo(bag,EquipmentSlot.OffHand,7,1),Is.True);Assert.That(gear.TryEquip(bag,2,EquipmentSlot.Weapon,1),Is.True);
        Assert.That(gear.TryEquip(bag,7,EquipmentSlot.OffHand,1),Is.False);Assert.That(bag.GetSlot(7).Item,Is.SameAs(shield));
    }
    [Test] public void EquipmentReturnsToExactEmptyCellOrSwapsCompatibleSingleWithoutDuplication()
    {
        var bag=new InventoryState();var gear=new EquipmentState();bag.TryAdd(sword,1);bag.TryAdd(other,1);
        gear.TryEquip(bag,0,EquipmentSlot.Weapon,1);
        Assert.That(gear.TryUnequipTo(bag,EquipmentSlot.Weapon,1,1),Is.True);
        Assert.That(gear.GetSlot(EquipmentSlot.Weapon),Is.SameAs(other));Assert.That(bag.GetSlot(1).Item,Is.SameAs(sword));
        Assert.That(gear.TryUnequipTo(bag,EquipmentSlot.Weapon,27,1),Is.True);
        Assert.That(bag.GetSlot(27).Item,Is.SameAs(other));Assert.That(bag.Count(other),Is.EqualTo(1));Assert.That(bag.Count(sword),Is.EqualTo(1));
        Assert.That(gear.TryUnequipTo(bag,EquipmentSlot.Weapon,28,1),Is.False);Assert.That(bag.GetSlot(28).IsEmpty,Is.True);
    }
    [Test] public void AccessoryTransferAndSwapAreAtomicAndRejectReentry()
    {
        sword.category=other.category=ItemCategory.Accessory;sword.bonuses=new ItemStatBonus{attack=3};other.bonuses=new ItemStatBonus{attack=4};
        var bag=new InventoryState();var gear=new EquipmentState();bag.TryAdd(sword,1);bag.TryAdd(other,1);
        gear.TryEquip(bag,0,EquipmentSlot.Accessory1,1);gear.TryEquip(bag,1,EquipmentSlot.Accessory2,1);
        bool nested=true;gear.Changed+=()=>nested=gear.TryMove(EquipmentSlot.Accessory2,EquipmentSlot.Accessory1,1);
        Assert.That(gear.TryMove(EquipmentSlot.Accessory1,EquipmentSlot.Accessory2,1),Is.True);Assert.That(nested,Is.False);
        Assert.That(gear.GetSlot(EquipmentSlot.Accessory1),Is.SameAs(other));Assert.That(gear.GetSlot(EquipmentSlot.Accessory2),Is.SameAs(sword));Assert.That(gear.Bonuses.attack,Is.EqualTo(7));
        Assert.That(gear.TryMove(EquipmentSlot.Accessory1,EquipmentSlot.Gloves,1),Is.False);Assert.That(bag.Count(sword),Is.Zero);
    }
    [Test] public void FullBagAndLegacyEightSlotDataCannotLoseGearOrBreakOffHand()
    {
        var bag=new InventoryState();var gear=new EquipmentState();bag.TryAdd(sword,1);gear.TryEquip(bag,0,EquipmentSlot.Weapon,1);bag.TryAdd(filler,30);
        Assert.That(gear.TryUnequipTo(bag,EquipmentSlot.Weapon,29,1),Is.False);Assert.That(gear.GetSlot(EquipmentSlot.Weapon),Is.SameAs(sword));Assert.That(bag.Count(filler),Is.EqualTo(30));
        var legacy=new EquipmentState();JsonUtility.FromJsonOverwrite("{\"slots\":[null,null,null,null,null,null,null,null]}",legacy);
        Assert.That(legacy.GetSlot(EquipmentSlot.OffHand),Is.Null);Assert.That(legacy.Bonuses.attack,Is.Zero);
    }
}

using NUnit.Framework;
using UnityEngine;

public sealed class EquipmentTests
{
    private InventoryState inventory;
    private EquipmentState equipment;
    private ItemData sword, armor, filler;
    [SetUp] public void Setup()
    {
        inventory=new InventoryState();equipment=new EquipmentState();
        sword=ScriptableObject.CreateInstance<ItemData>();sword.id="test.sword";sword.category=ItemCategory.Weapon;sword.equipmentSlot=EquipmentSlot.Weapon;sword.bonuses=new ItemStatBonus{attack=5};
        armor=ScriptableObject.CreateInstance<ItemData>();armor.id="test.armor";armor.category=ItemCategory.Armor;armor.equipmentSlot=EquipmentSlot.Chest;armor.bonuses=new ItemStatBonus{defense=4};
        filler=ScriptableObject.CreateInstance<ItemData>();filler.id="test.filler";filler.stackable=false;
    }
    [TearDown] public void Cleanup(){Object.DestroyImmediate(sword);Object.DestroyImmediate(armor);Object.DestroyImmediate(filler);}
    [TestCase(EquipmentSlot.Weapon)] [TestCase(EquipmentSlot.Chest)]
    public void EachEquipmentSlotAcceptsOnlyMatchingItemAndReturnsIt(EquipmentSlot slot)
    {
        var item=slot==EquipmentSlot.Weapon?sword:armor;
        inventory.TryAdd(item,1);
        Assert.That(equipment.TryEquip(inventory,0,slot),Is.True);Assert.That(inventory.Count(item),Is.Zero);
        Assert.That(equipment.GetSlot(slot),Is.SameAs(item));
        Assert.That(equipment.TryUnequip(inventory,slot),Is.True);Assert.That(inventory.Count(item),Is.EqualTo(1));
        Assert.That(equipment.GetSlot(slot),Is.Null);Assert.That(equipment.Bonuses.attack,Is.Zero);Assert.That(equipment.Bonuses.defense,Is.Zero);
    }
    [Test] public void WeaponCannotFillChestAndChestCannotFillWeapon()
    {
        inventory.TryAdd(sword,1);inventory.TryAdd(armor,1);
        Assert.That(equipment.TryEquip(inventory,0,EquipmentSlot.Chest),Is.False);
        Assert.That(equipment.TryEquip(inventory,1,EquipmentSlot.Weapon),Is.False);
        Assert.That(inventory.Count(sword),Is.EqualTo(1));Assert.That(inventory.Count(armor),Is.EqualTo(1));
    }
    [Test] public void FullInventorySwapsWeaponButCannotUnequipUntilSpaceIsFree()
    {
        var secondSword=ScriptableObject.CreateInstance<ItemData>();
        try
        {
            secondSword.id="test.sword2";secondSword.category=ItemCategory.Weapon;secondSword.bonuses=new ItemStatBonus{attack=2};
            inventory.TryAdd(sword,1);inventory.TryAdd(secondSword,1);inventory.TryAdd(filler,28);
            equipment.TryEquip(inventory,0,EquipmentSlot.Weapon);inventory.TryAdd(filler,1);
            Assert.That(equipment.TryEquip(inventory,1,EquipmentSlot.Weapon),Is.True);
            Assert.That(inventory.GetSlot(1).Item,Is.SameAs(sword));Assert.That(equipment.GetSlot(EquipmentSlot.Weapon),Is.SameAs(secondSword));
            Assert.That(equipment.TryUnequip(inventory,EquipmentSlot.Weapon),Is.False);
            Assert.That(equipment.Bonuses.attack,Is.EqualTo(2));Assert.That(inventory.Count(secondSword),Is.Zero);
            inventory.TryRemove(filler,1);Assert.That(equipment.TryUnequip(inventory,EquipmentSlot.Weapon),Is.True);
            Assert.That(inventory.Count(secondSword),Is.EqualTo(1));Assert.That(inventory.Count(sword),Is.EqualTo(1));
        }
        finally {Object.DestroyImmediate(secondSword);}
    }
    [Test] public void InvalidSlotsAndIndicesNeverLoseItemsOrRaiseEvents()
    {
        inventory.TryAdd(sword,1);int changes=0;equipment.Changed+=()=>changes++;
        Assert.That(equipment.TryEquip(inventory,0,EquipmentSlot.Chest),Is.False);
        Assert.That(equipment.TryEquip(inventory,-1,EquipmentSlot.Weapon),Is.False);
        Assert.That(equipment.TryEquip(inventory,0,(EquipmentSlot)99),Is.False);
        Assert.That(equipment.TryEquip(inventory,InventoryState.Capacity,EquipmentSlot.Weapon),Is.False);
        Assert.That(inventory.Count(sword),Is.EqualTo(1));Assert.That(changes,Is.Zero);
    }
    [Test] public void WeaponAndChestBonusesStackOnceAndRepeatedReadsNeverAccumulate()
    {
        inventory.TryAdd(sword,1);inventory.TryAdd(armor,1);
        equipment.TryEquip(inventory,0,EquipmentSlot.Weapon);equipment.TryEquip(inventory,1,EquipmentSlot.Chest);
        for(int i=0;i<20;i++){Assert.That(equipment.Bonuses.attack,Is.EqualTo(5));Assert.That(equipment.Bonuses.defense,Is.EqualTo(4));}
        equipment.TryUnequip(inventory,EquipmentSlot.Weapon);Assert.That(equipment.Bonuses.attack,Is.Zero);Assert.That(equipment.Bonuses.defense,Is.EqualTo(4));
        equipment.TryUnequip(inventory,EquipmentSlot.Chest);Assert.That(equipment.Bonuses.defense,Is.Zero);
    }
    [Test] public void InventoryNotificationCannotReenterEquipmentTransaction()
    {
        inventory.TryAdd(sword,1);bool nested=true;int attempts=0;
        inventory.Changed+=()=>{attempts++;nested=equipment.TryUnequip(inventory,EquipmentSlot.Weapon);};
        Assert.That(equipment.TryEquip(inventory,0,EquipmentSlot.Weapon),Is.True);
        Assert.That(attempts,Is.EqualTo(1));Assert.That(nested,Is.False);
        Assert.That(equipment.GetSlot(EquipmentSlot.Weapon),Is.SameAs(sword));Assert.That(inventory.Count(sword),Is.Zero);
    }
}

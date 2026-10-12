using NUnit.Framework;
using UnityEngine;

public sealed class EquipmentTests
{
    private InventoryState inventory;
    private EquipmentState equipment;
    private ItemData sword, staff, filler;
    [SetUp] public void Setup()
    {
        inventory=new InventoryState();equipment=new EquipmentState();
        sword=ScriptableObject.CreateInstance<ItemData>();sword.id="test.sword";sword.category=ItemCategory.Weapon;sword.bonuses=new ItemStatBonus{attack=5};
        staff=ScriptableObject.CreateInstance<ItemData>();staff.id="test.staff";staff.category=ItemCategory.Weapon;staff.bonuses=new ItemStatBonus{magicAttack=4};
        filler=ScriptableObject.CreateInstance<ItemData>();filler.id="test.filler";filler.stackable=false;
    }
    [TearDown] public void Cleanup(){Object.DestroyImmediate(sword);Object.DestroyImmediate(staff);Object.DestroyImmediate(filler);}
    [TestCase(EquipmentSlot.Weapon)] [TestCase(EquipmentSlot.Head)] [TestCase(EquipmentSlot.Chest)]
    [TestCase(EquipmentSlot.Legs)] [TestCase(EquipmentSlot.Boots)] [TestCase(EquipmentSlot.Accessory1)] [TestCase(EquipmentSlot.Accessory2)] [TestCase(EquipmentSlot.Gloves)] [TestCase(EquipmentSlot.OffHand)]
    public void EachEquipmentSlotAcceptsOnlyMatchingCategoryAndReturnsItem(EquipmentSlot slot)
    {
        sword.category=slot==EquipmentSlot.Weapon?ItemCategory.Weapon:
            slot==EquipmentSlot.Accessory1 || slot==EquipmentSlot.Accessory2?ItemCategory.Accessory:ItemCategory.Armor;
        sword.equipmentSlot=slot;inventory.TryAdd(sword,1);
        Assert.That(equipment.TryEquip(inventory,0,slot,1),Is.True);Assert.That(inventory.Count(sword),Is.Zero);
        Assert.That(equipment.GetSlot(slot),Is.SameAs(sword));
        Assert.That(equipment.TryUnequip(inventory,slot),Is.True);Assert.That(inventory.Count(sword),Is.EqualTo(1));
        Assert.That(equipment.GetSlot(slot),Is.Null);Assert.That(equipment.Bonuses.attack,Is.Zero);
    }
    [Test] public void FullInventorySwapsWeaponButCannotUnequipUntilSpaceIsFree()
    {
        inventory.TryAdd(sword,1);inventory.TryAdd(staff,1);inventory.TryAdd(filler,28);
        equipment.TryEquip(inventory,0,EquipmentSlot.Weapon,1);inventory.TryAdd(filler,1);
        Assert.That(equipment.TryEquip(inventory,1,EquipmentSlot.Weapon,1),Is.True);
        Assert.That(inventory.GetSlot(1).Item,Is.SameAs(sword));Assert.That(equipment.GetSlot(EquipmentSlot.Weapon),Is.SameAs(staff));
        Assert.That(equipment.TryUnequip(inventory,EquipmentSlot.Weapon),Is.False);
        Assert.That(equipment.Bonuses.magicAttack,Is.EqualTo(4));Assert.That(inventory.Count(staff),Is.Zero);
        inventory.TryRemove(filler,1);Assert.That(equipment.TryUnequip(inventory,EquipmentSlot.Weapon),Is.True);
        Assert.That(inventory.Count(staff),Is.EqualTo(1));Assert.That(inventory.Count(sword),Is.EqualTo(1));
    }
    [Test] public void WrongSlotLevelAndIndicesNeverLoseItemsOrRaiseEvents()
    {
        sword.requiredLevel=3;inventory.TryAdd(sword,1);int changes=0;equipment.Changed+=()=>changes++;
        Assert.That(equipment.TryEquip(inventory,0,EquipmentSlot.Weapon,1),Is.False);
        Assert.That(equipment.TryEquip(inventory,0,EquipmentSlot.Chest,3),Is.False);
        Assert.That(equipment.TryEquip(inventory,-1,EquipmentSlot.Weapon,3),Is.False);
        Assert.That(equipment.TryEquip(inventory,0,(EquipmentSlot)99,3),Is.False);
        Assert.That(inventory.Count(sword),Is.EqualTo(1));Assert.That(changes,Is.Zero);
    }
    [Test] public void TwoAccessoriesAddOnceAndRepeatedReadsNeverAccumulate()
    {
        sword.category=ItemCategory.Accessory;inventory.TryAdd(sword,2);
        equipment.TryEquip(inventory,0,EquipmentSlot.Accessory1,1);equipment.TryEquip(inventory,1,EquipmentSlot.Accessory2,1);
        for(int i=0;i<20;i++)Assert.That(equipment.Bonuses.attack,Is.EqualTo(10));
        equipment.TryUnequip(inventory,EquipmentSlot.Accessory1);Assert.That(equipment.Bonuses.attack,Is.EqualTo(5));
        equipment.TryUnequip(inventory,EquipmentSlot.Accessory2);Assert.That(equipment.Bonuses.attack,Is.Zero);Assert.That(inventory.Count(sword),Is.EqualTo(2));
    }
    [Test] public void InventoryNotificationCannotReenterEquipmentTransaction()
    {
        inventory.TryAdd(sword,1);bool nested=true;int attempts=0;
        inventory.Changed+=()=>{attempts++;nested=equipment.TryUnequip(inventory,EquipmentSlot.Weapon);};
        Assert.That(equipment.TryEquip(inventory,0,EquipmentSlot.Weapon,1),Is.True);
        Assert.That(attempts,Is.EqualTo(1));Assert.That(nested,Is.False);
        Assert.That(equipment.GetSlot(EquipmentSlot.Weapon),Is.SameAs(sword));Assert.That(inventory.Count(sword),Is.Zero);
    }
}

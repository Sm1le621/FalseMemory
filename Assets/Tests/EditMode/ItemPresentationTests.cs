using NUnit.Framework;
using UnityEngine;

public sealed class ItemPresentationTests
{
    [Test] public void WeaponAndArmorDescribeOnlyAttackAndDefenseBonuses()
    {
        var weapon=ScriptableObject.CreateInstance<ItemData>();
        var armor=ScriptableObject.CreateInstance<ItemData>();
        try
        {
            weapon.category=ItemCategory.Weapon;weapon.equipmentSlot=EquipmentSlot.Weapon;weapon.bonuses=new ItemStatBonus{attack=5};
            armor.category=ItemCategory.Armor;armor.equipmentSlot=EquipmentSlot.Chest;armor.bonuses=new ItemStatBonus{defense=4};
            Assert.That(ItemPresentation.Description(weapon,1),Does.Contain("Weapon").And.Contain("ATK +5").And.Not.Contain("DEF"));
            Assert.That(ItemPresentation.Description(armor,1),Does.Contain("Chest").And.Contain("DEF +4").And.Not.Contain("ATK"));
            Assert.That(ItemPresentation.SlotName(EquipmentSlot.Weapon),Is.EqualTo("Weapon"));
            Assert.That(ItemPresentation.SlotName(EquipmentSlot.Chest),Is.EqualTo("Chest"));
            Assert.That(weapon.StackLimit,Is.EqualTo(1));Assert.That(weapon.bonuses.attack,Is.EqualTo(5));
        }
        finally {Object.DestroyImmediate(weapon);Object.DestroyImmediate(armor);}
    }
    [Test] public void TooltipHidesEmptyFieldsEscapesMarkupAndReportsRealConsumableEffect()
    {
        var item=ScriptableObject.CreateInstance<BattleConsumable>();
        try
        {
            item.displayName="<size=200>Potion</size>";item.description="";item.healing=30;
            var text=ItemPresentation.Description(item,3);
            Assert.That(text,Does.Contain("×3").And.Contain("Restores up to 30 HP"));
            Assert.That(text,Does.Not.Contain("<size=").And.Not.Contain("Buy:").And.Not.Contain("Sell:").And.Not.Contain("MATK"));
            item.healing=0;item.manaRestoration=20;item.buyPrice=50;item.sellPrice=25;
            Assert.That(ItemPresentation.Description(item,1),Does.Contain("Restores up to 20 MP").And.Contain("Buy: 50").And.Contain("Sell: 25").And.Not.Contain(" HP"));
        }
        finally {Object.DestroyImmediate(item);}
    }
    [Test] public void ArmorFitsOnlyItsConfiguredChestSlot()
    {
        var item=ScriptableObject.CreateInstance<ItemData>();
        try
        {
            item.category=ItemCategory.Armor;item.equipmentSlot=EquipmentSlot.Chest;
            Assert.That(item.Fits(EquipmentSlot.Chest),Is.True);Assert.That(item.Fits(EquipmentSlot.Weapon),Is.False);
            item.category=ItemCategory.Weapon;Assert.That(item.Fits(EquipmentSlot.Weapon),Is.True);Assert.That(item.Fits(EquipmentSlot.Chest),Is.False);
            Assert.That(ItemPresentation.Description(null,1),Is.Empty);
        }
        finally {Object.DestroyImmediate(item);}
    }
}

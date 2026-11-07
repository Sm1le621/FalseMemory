using NUnit.Framework;
using UnityEngine;

// Тести текстів описів предметів
public sealed class ItemPresentationTests
{
    // Зброя показує тільки атаку, броня - тільки захист
    [Test]
    public void WeaponAndArmorDescribeOnlyAttackAndDefenseBonuses()
    {
        ItemData weapon = ScriptableObject.CreateInstance<ItemData>();
        ItemData armor = ScriptableObject.CreateInstance<ItemData>();
        try
        {
            weapon.category = ItemCategory.Weapon;
            weapon.equipmentSlot = EquipmentSlot.Weapon;
            ItemStatBonus weaponBonus = new ItemStatBonus();
            weaponBonus.attack = 5;
            weapon.bonuses = weaponBonus;

            armor.category = ItemCategory.Armor;
            armor.equipmentSlot = EquipmentSlot.Chest;
            ItemStatBonus armorBonus = new ItemStatBonus();
            armorBonus.defense = 4;
            armor.bonuses = armorBonus;

            string weaponText = ItemPresentation.Description(weapon, 1);
            Assert.That(weaponText, Does.Contain("Weapon").And.Contain("ATK +5").And.Not.Contain("DEF"));

            string armorText = ItemPresentation.Description(armor, 1);
            Assert.That(armorText, Does.Contain("Chest").And.Contain("DEF +4").And.Not.Contain("ATK"));

            Assert.That(ItemPresentation.SlotName(EquipmentSlot.Weapon), Is.EqualTo("Weapon"));
            Assert.That(ItemPresentation.SlotName(EquipmentSlot.Chest), Is.EqualTo("Chest"));
            Assert.That(weapon.StackLimit, Is.EqualTo(1));
            Assert.That(weapon.bonuses.attack, Is.EqualTo(5));
        }
        finally
        {
            Object.DestroyImmediate(weapon);
            Object.DestroyImmediate(armor);
        }
    }

    // Підказка: порожні поля не показуються, теги в назві не працюють, ефект зілля правильний
    [Test]
    public void TooltipHidesEmptyFieldsEscapesMarkupAndReportsRealConsumableEffect()
    {
        BattleConsumable item = ScriptableObject.CreateInstance<BattleConsumable>();
        try
        {
            // Назва з тегом - він не має спрацювати
            item.displayName = "<size=200>Potion</size>";
            item.description = "";
            item.healing = 30;

            string text = ItemPresentation.Description(item, 3);
            Assert.That(text, Does.Contain("×3").And.Contain("Restores up to 30 HP"));
            Assert.That(text, Does.Not.Contain("<size=").And.Not.Contain("Buy:").And.Not.Contain("Sell:").And.Not.Contain("MATK"));

            item.healing = 0;
            item.manaRestoration = 20;
            string manaText = ItemPresentation.Description(item, 1);
            Assert.That(manaText, Does.Contain("Restores up to 20 MP").And.Not.Contain("Buy:").And.Not.Contain("Sell:").And.Not.Contain(" HP"));
        }
        finally
        {
            Object.DestroyImmediate(item);
        }
    }

    // Броня підходить тільки у свій слот
    [Test]
    public void ArmorFitsOnlyItsConfiguredChestSlot()
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        try
        {
            item.category = ItemCategory.Armor;
            item.equipmentSlot = EquipmentSlot.Chest;
            Assert.That(item.Fits(EquipmentSlot.Chest), Is.True);
            Assert.That(item.Fits(EquipmentSlot.Weapon), Is.False);

            item.category = ItemCategory.Weapon;
            Assert.That(item.Fits(EquipmentSlot.Weapon), Is.True);
            Assert.That(item.Fits(EquipmentSlot.Chest), Is.False);

            Assert.That(ItemPresentation.Description(null, 1), Is.Empty);
        }
        finally
        {
            Object.DestroyImmediate(item);
        }
    }
}

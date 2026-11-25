using NUnit.Framework;
using UnityEngine;

// Тести даних предметів і назв слотів
public sealed class ItemPresentationTests
{
    // Назви слотів і стопка зброї
    [Test]
    public void SlotNamesAndWeaponStackLimit()
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
        }
        finally
        {
            Object.DestroyImmediate(item);
        }
    }
}

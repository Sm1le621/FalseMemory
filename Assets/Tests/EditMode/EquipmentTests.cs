using NUnit.Framework;
using UnityEngine;

// Тести екіпіровки (без сцени, тільки дані)
public sealed class EquipmentTests
{
    private InventoryState inventory;
    private EquipmentState equipment;
    private ItemData sword;
    private ItemData armor;
    private ItemData filler;

    [SetUp]
    public void Setup()
    {
        inventory = new InventoryState();
        equipment = new EquipmentState();

        // Меч: +5 атаки
        sword = ScriptableObject.CreateInstance<ItemData>();
        sword.id = "test.sword";
        sword.category = ItemCategory.Weapon;
        sword.equipmentSlot = EquipmentSlot.Weapon;
        ItemStatBonus swordBonus = new ItemStatBonus();
        swordBonus.attack = 5;
        sword.bonuses = swordBonus;

        // Броня: +4 захисту
        armor = ScriptableObject.CreateInstance<ItemData>();
        armor.id = "test.armor";
        armor.category = ItemCategory.Armor;
        armor.equipmentSlot = EquipmentSlot.Chest;
        ItemStatBonus armorBonus = new ItemStatBonus();
        armorBonus.defense = 4;
        armor.bonuses = armorBonus;

        // "Сміття" для заповнення інвентаря (1 штука на слот)
        filler = ScriptableObject.CreateInstance<ItemData>();
        filler.id = "test.filler";
        filler.stackable = false;
    }

    [TearDown]
    public void Cleanup()
    {
        Object.DestroyImmediate(sword);
        Object.DestroyImmediate(armor);
        Object.DestroyImmediate(filler);
    }

    // Кожен слот приймає тільки свій предмет і повертає його назад
    [TestCase(EquipmentSlot.Weapon)]
    [TestCase(EquipmentSlot.Chest)]
    public void EachEquipmentSlotAcceptsOnlyMatchingItemAndReturnsIt(EquipmentSlot slot)
    {
        ItemData item;
        if (slot == EquipmentSlot.Weapon)
        {
            item = sword;
        }
        else
        {
            item = armor;
        }
        inventory.TryAdd(item, 1);

        Assert.That(equipment.TryEquip(inventory, 0, slot), Is.True);
        Assert.That(inventory.Count(item), Is.Zero);
        Assert.That(equipment.GetSlot(slot), Is.SameAs(item));

        Assert.That(equipment.TryUnequip(inventory, slot), Is.True);
        Assert.That(inventory.Count(item), Is.EqualTo(1));
        Assert.That(equipment.GetSlot(slot), Is.Null);
        Assert.That(equipment.Bonuses.attack, Is.Zero);
        Assert.That(equipment.Bonuses.defense, Is.Zero);
    }

    // Меч не одягається в слот броні і навпаки
    [Test]
    public void WeaponCannotFillChestAndChestCannotFillWeapon()
    {
        inventory.TryAdd(sword, 1);
        inventory.TryAdd(armor, 1);

        Assert.That(equipment.TryEquip(inventory, 0, EquipmentSlot.Chest), Is.False);
        Assert.That(equipment.TryEquip(inventory, 1, EquipmentSlot.Weapon), Is.False);
        Assert.That(inventory.Count(sword), Is.EqualTo(1));
        Assert.That(inventory.Count(armor), Is.EqualTo(1));
    }

    // При повному інвентарі можна поміняти зброю, але не можна зняти, поки не звільниться місце
    [Test]
    public void FullInventorySwapsWeaponButCannotUnequipUntilSpaceIsFree()
    {
        ItemData secondSword = ScriptableObject.CreateInstance<ItemData>();
        try
        {
            secondSword.id = "test.sword2";
            secondSword.category = ItemCategory.Weapon;
            ItemStatBonus secondBonus = new ItemStatBonus();
            secondBonus.attack = 2;
            secondSword.bonuses = secondBonus;

            inventory.TryAdd(sword, 1);
            inventory.TryAdd(secondSword, 1);
            inventory.TryAdd(filler, 28);
            equipment.TryEquip(inventory, 0, EquipmentSlot.Weapon);
            // Тепер інвентар повністю заповнений
            inventory.TryAdd(filler, 1);

            // Заміна меча працює (старий іде на місце нового)
            Assert.That(equipment.TryEquip(inventory, 1, EquipmentSlot.Weapon), Is.True);
            Assert.That(inventory.GetSlot(1).Item, Is.SameAs(sword));
            Assert.That(equipment.GetSlot(EquipmentSlot.Weapon), Is.SameAs(secondSword));

            // Зняти не можна - немає місця
            Assert.That(equipment.TryUnequip(inventory, EquipmentSlot.Weapon), Is.False);
            Assert.That(equipment.Bonuses.attack, Is.EqualTo(2));
            Assert.That(inventory.Count(secondSword), Is.Zero);

            // Звільнили місце - тепер можна
            inventory.TryRemove(filler, 1);
            Assert.That(equipment.TryUnequip(inventory, EquipmentSlot.Weapon), Is.True);
            Assert.That(inventory.Count(secondSword), Is.EqualTo(1));
            Assert.That(inventory.Count(sword), Is.EqualTo(1));
        }
        finally
        {
            Object.DestroyImmediate(secondSword);
        }
    }

    // Неправильні слоти і номери нічого не ламають і не викликають подію
    [Test]
    public void InvalidSlotsAndIndicesNeverLoseItemsOrRaiseEvents()
    {
        inventory.TryAdd(sword, 1);
        int changes = 0;
        equipment.Changed += () => changes++;

        Assert.That(equipment.TryEquip(inventory, 0, EquipmentSlot.Chest), Is.False);
        Assert.That(equipment.TryEquip(inventory, -1, EquipmentSlot.Weapon), Is.False);
        Assert.That(equipment.TryEquip(inventory, 0, (EquipmentSlot)99), Is.False);
        Assert.That(equipment.TryEquip(inventory, InventoryState.Capacity, EquipmentSlot.Weapon), Is.False);

        Assert.That(inventory.Count(sword), Is.EqualTo(1));
        Assert.That(changes, Is.Zero);
    }

    // Бонуси рахуються один раз і не накопичуються при повторному читанні
    [Test]
    public void WeaponAndChestBonusesStackOnceAndRepeatedReadsNeverAccumulate()
    {
        inventory.TryAdd(sword, 1);
        inventory.TryAdd(armor, 1);
        equipment.TryEquip(inventory, 0, EquipmentSlot.Weapon);
        equipment.TryEquip(inventory, 1, EquipmentSlot.Chest);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(equipment.Bonuses.attack, Is.EqualTo(5));
            Assert.That(equipment.Bonuses.defense, Is.EqualTo(4));
        }

        equipment.TryUnequip(inventory, EquipmentSlot.Weapon);
        Assert.That(equipment.Bonuses.attack, Is.Zero);
        Assert.That(equipment.Bonuses.defense, Is.EqualTo(4));

        equipment.TryUnequip(inventory, EquipmentSlot.Chest);
        Assert.That(equipment.Bonuses.defense, Is.Zero);
    }

    // Під час одягання не можна почати ще одну зміну екіпіровки (з події інвентаря)
    [Test]
    public void InventoryNotificationCannotReenterEquipmentTransaction()
    {
        inventory.TryAdd(sword, 1);
        bool nested = true;
        int attempts = 0;

        // Коли інвентар змінюється - пробуємо зняти меч (це має не вийти)
        inventory.Changed += () =>
        {
            attempts++;
            nested = equipment.TryUnequip(inventory, EquipmentSlot.Weapon);
        };

        Assert.That(equipment.TryEquip(inventory, 0, EquipmentSlot.Weapon), Is.True);
        Assert.That(attempts, Is.EqualTo(1));
        Assert.That(nested, Is.False);
        Assert.That(equipment.GetSlot(EquipmentSlot.Weapon), Is.SameAs(sword));
        Assert.That(inventory.Count(sword), Is.Zero);
    }
}

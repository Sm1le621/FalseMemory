using System;
using UnityEngine;

// Тип предмета
public enum ItemCategory { Consumable, Weapon, Armor }

// Слоти екіпіровки
public enum EquipmentSlot { Weapon, Chest }

// Де можна використати предмет. [Flags] - значення можна поєднувати
// (Anywhere = Exploration + Battle = 1 + 2 = 3)
[Flags]
public enum ItemUseLocation { None = 0, Exploration = 1, Battle = 2, Anywhere = 3 }

// Бонуси до характеристик від предмета
[Serializable]
public struct ItemStatBonus
{
    [Min(0)]
    public int attack;
    [Min(0)]
    public int defense;

    // Оператор "+" для бонусів: можна писати bonusA + bonusB
    public static ItemStatBonus operator +(ItemStatBonus a, ItemStatBonus b)
    {
        ItemStatBonus result = new ItemStatBonus();
        result.attack = Sum(a.attack, b.attack);
        result.defense = Sum(a.defense, b.defense);
        return result;
    }

    // Безпечне додавання: від'ємні числа вважаються 0, і немає переповнення int
    public static int Sum(int a, int b)
    {
        long total = (long)Math.Max(0, a) + Math.Max(0, b);
        if (total > int.MaxValue)
        {
            return int.MaxValue;
        }
        return (int)total;
    }
}

// Опис предмета (створюється через меню Create).
// Тут тільки "статичні" дані. Кількість і що одягнено - зберігається в інвентарі, а не тут.
[CreateAssetMenu(menuName = "FalseMemory/Item")]
public class ItemData : ScriptableObject
{
    // Унікальний id (два різні предмети з однаковим id не можуть бути в інвентарі)
    public string id;
    public string displayName = "Item";
    [TextArea]
    public string description;
    public Sprite icon;
    public ItemCategory category = ItemCategory.Consumable;
    // Чи можна складати в стопку
    public bool stackable = true;
    // Максимум у одній стопці
    [Min(1)]
    public int maxStack = 99;
    public ItemUseLocation useLocations = ItemUseLocation.Anywhere;
    // Для броні: в який слот одягається
    public EquipmentSlot equipmentSlot = EquipmentSlot.Chest;
    public ItemStatBonus bonuses;

    // Чи це екіпіровка (зброя або броня)
    public bool IsEquipment
    {
        get { return category == ItemCategory.Weapon || category == ItemCategory.Armor; }
    }

    // Скільки штук влазить в один слот
    public int StackLimit
    {
        get
        {
            // Екіпіровка і нескладні предмети - по одному
            if (IsEquipment || !stackable)
            {
                return 1;
            }
            return Math.Max(1, maxStack);
        }
    }

    // Чи підходить предмет у цей слот
    public bool Fits(EquipmentSlot slot)
    {
        if (category == ItemCategory.Weapon)
        {
            // Зброя - тільки в слот зброї
            return slot == EquipmentSlot.Weapon;
        }

        // Броня - тільки в свій слот. Інші предмети не одягаються
        return category == ItemCategory.Armor && equipmentSlot == slot;
    }
}

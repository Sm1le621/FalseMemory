using System;
using UnityEngine;

// Дані екіпіровки: що одягнено в кожному слоті.
// Тут немає Unity-компонентів, тому це легко тестувати.
[Serializable]
public sealed class EquipmentState
{
    // Кількість слотів (Weapon і Chest)
    public const int SlotCount = 2;

    [SerializeField] private ItemData[] slots = new ItemData[SlotCount];

    // true - зараз іде зміна екіпіровки (щоб не почати ще одну зміну всередині)
    private bool changing;

    // Подія "екіпіровка змінилась"
    public event Action Changed;

    // Що одягнено в слоті
    public ItemData GetSlot(EquipmentSlot slot)
    {
        if (!Valid(slot))
        {
            throw new ArgumentOutOfRangeException(nameof(slot));
        }
        return slots[(int)slot];
    }

    // Сума бонусів від усіх одягнених речей
    public ItemStatBonus Bonuses
    {
        get
        {
            ItemStatBonus total = new ItemStatBonus();
            foreach (ItemData item in slots)
            {
                if (item != null)
                {
                    total = total + item.bonuses;
                }
            }
            return total;
        }
    }

    // Одягнути предмет зі слота інвентаря index у слот екіпіровки slot.
    // Якщо в слоті вже щось було - воно повертається в інвентар на те ж місце.
    public bool TryEquip(InventoryState inventory, int index, EquipmentSlot slot)
    {
        if (changing || inventory == null || index < 0 || index >= InventoryState.Capacity || !Valid(slot))
        {
            return false;
        }

        InventorySlot source = inventory.GetSlot(index);
        ItemData item = source.Item;

        // Одягнути можна тільки один предмет, який підходить у цей слот
        if (source.IsEmpty || source.Count != 1 || !item.Fits(slot))
        {
            return false;
        }

        // Що було одягнено раніше
        ItemData previous = slots[(int)slot];
        changing = true;
        try
        {
            slots[(int)slot] = item;

            // Міняємо місцями: новий предмет - на тіло, старий - в інвентар
            if (!inventory.TryExchangeSingle(index, item, previous))
            {
                // Не вийшло - повертаємо як було
                slots[(int)slot] = previous;
                return false;
            }

            if (Changed != null)
            {
                Changed.Invoke();
            }
            return true;
        }
        finally
        {
            // finally виконується завжди, навіть після return
            changing = false;
        }
    }

    // Зняти предмет і покласти в інвентар
    public bool TryUnequip(InventoryState inventory, EquipmentSlot slot)
    {
        if (changing || inventory == null || !Valid(slot))
        {
            return false;
        }

        ItemData item = slots[(int)slot];
        // Слот порожній або в інвентарі немає місця
        if (item == null || !inventory.CanAdd(item, 1))
        {
            return false;
        }

        changing = true;
        try
        {
            slots[(int)slot] = null;

            if (!inventory.TryAdd(item, 1))
            {
                // Не вийшло - одягаємо назад
                slots[(int)slot] = item;
                return false;
            }

            if (Changed != null)
            {
                Changed.Invoke();
            }
            return true;
        }
        finally
        {
            changing = false;
        }
    }

    // Чи існує такий слот
    private static bool Valid(EquipmentSlot slot)
    {
        return (int)slot >= 0 && (int)slot < SlotCount;
    }
}

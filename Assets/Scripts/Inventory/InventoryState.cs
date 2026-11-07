using System;
using System.Collections.Generic;
using UnityEngine;

// Один слот інвентаря: який предмет і скільки штук.
// Слот незмінний: щоб змінити кількість, створюється новий слот.
[Serializable]
public sealed class InventorySlot
{
    [SerializeField] private ItemData item;
    [SerializeField] private int count;

    public ItemData Item
    {
        get { return item; }
    }

    public int Count
    {
        get { return count; }
    }

    // Порожній, якщо немає предмета або кількість 0
    public bool IsEmpty
    {
        get { return item == null || count <= 0; }
    }

    // internal - створювати слоти може тільки код цієї збірки
    internal InventorySlot(ItemData definition, int amount)
    {
        item = definition;
        count = amount;
    }
}

// Інвентар на 30 слотів. Не залежить від сцени і від UI.
[Serializable]
public sealed class InventoryState
{
    // Кількість слотів
    public const int Capacity = 30;

    // Спільний "порожній" слот, щоб не повертати null
    private static readonly InventorySlot Empty = new InventorySlot(null, 0);

    // null у масиві = порожній слот
    [SerializeField] private InventorySlot[] slots = new InventorySlot[Capacity];

    // Подія "інвентар змінився"
    public event Action Changed;

    // Всі різні предмети в інвентарі (кожен по одному разу), по порядку слотів
    public IEnumerable<ItemData> Definitions
    {
        get
        {
            List<ItemData> result = new List<ItemData>();
            HashSet<ItemData> seen = new HashSet<ItemData>();

            foreach (InventorySlot slot in slots)
            {
                if (slot != null && !slot.IsEmpty)
                {
                    // Add повертає false, якщо такий предмет уже був
                    if (seen.Add(slot.Item))
                    {
                        result.Add(slot.Item);
                    }
                }
            }
            return result;
        }
    }

    // Слот за номером (порожній слот замість null)
    public InventorySlot GetSlot(int index)
    {
        if (!Valid(index))
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (slots[index] == null)
        {
            return Empty;
        }
        return slots[index];
    }

    // Скільки всього штук цього предмета (у всіх слотах)
    public int Count(ItemData item)
    {
        if (item == null)
        {
            return 0;
        }

        // long - щоб не було переповнення
        long total = 0;
        foreach (InventorySlot slot in slots)
        {
            if (slot != null && slot.Item == item)
            {
                total = total + slot.Count;
            }
        }

        if (total > int.MaxValue)
        {
            return int.MaxValue;
        }
        return (int)total;
    }

    // Чи є в інвентарі ІНШИЙ предмет з таким самим id (так не можна)
    private bool Conflicts(ItemData item)
    {
        if (string.IsNullOrWhiteSpace(item.id))
        {
            return false;
        }

        foreach (ItemData other in Definitions)
        {
            if (other != item && other.id == item.id)
            {
                return true;
            }
        }
        return false;
    }

    // Чи влізе amount штук предмета
    public bool CanAdd(ItemData item, int amount)
    {
        if (item == null || amount <= 0 || Conflicts(item))
        {
            return false;
        }

        // Рахуємо, скільки місця є всього
        long space = 0;
        foreach (InventorySlot slot in slots)
        {
            if (slot == null || slot.IsEmpty)
            {
                // Порожній слот вміщає цілу стопку
                space = space + item.StackLimit;
            }
            else if (slot.Item == item)
            {
                // Неповна стопка того ж предмета
                space = space + Math.Max(0, item.StackLimit - slot.Count);
            }
        }
        return space >= amount;
    }

    // Додати предмет. Або додається ВСЕ, або нічого
    public bool TryAdd(ItemData item, int amount)
    {
        if (!CanAdd(item, amount))
        {
            return false;
        }

        int remaining = amount;

        // 1) Спочатку доповнюємо вже існуючі стопки
        for (int i = 0; i < Capacity && remaining > 0; i++)
        {
            InventorySlot slot = GetSlot(i);
            if (slot.Item != item || slot.IsEmpty)
            {
                continue;
            }

            int add = Math.Min(remaining, Math.Max(0, item.StackLimit - slot.Count));
            slots[i] = new InventorySlot(item, slot.Count + add);
            remaining = remaining - add;
        }

        // 2) Те, що лишилось, кладемо в порожні слоти
        for (int i = 0; i < Capacity && remaining > 0; i++)
        {
            if (GetSlot(i).IsEmpty)
            {
                int add = Math.Min(remaining, item.StackLimit);
                slots[i] = new InventorySlot(item, add);
                remaining = remaining - add;
            }
        }

        if (Changed != null)
        {
            Changed.Invoke();
        }
        return true;
    }

    // Забрати amount штук. Якщо стільки немає - нічого не забираємо
    public bool TryRemove(ItemData item, int amount)
    {
        if (item == null || amount <= 0 || Count(item) < amount)
        {
            return false;
        }

        for (int i = 0; i < Capacity && amount > 0; i++)
        {
            InventorySlot slot = GetSlot(i);
            if (slot.Item != item)
            {
                continue;
            }

            int take = Math.Min(amount, slot.Count);
            int left = slot.Count - take;

            if (left == 0)
            {
                // Стопка закінчилась - слот стає порожнім
                slots[i] = null;
            }
            else
            {
                slots[i] = new InventorySlot(item, left);
            }
            amount = amount - take;
        }

        if (Changed != null)
        {
            Changed.Invoke();
        }
        return true;
    }

    // Перенести предмет з from у to: об'єднати стопки або поміняти місцями
    public bool TryMoveOrMerge(int from, int to)
    {
        if (!Valid(from) || !Valid(to) || from == to || GetSlot(from).IsEmpty)
        {
            return false;
        }

        InventorySlot source = GetSlot(from);
        InventorySlot target = GetSlot(to);

        if (!target.IsEmpty && source.Item == target.Item && source.Item.stackable)
        {
            // Той самий предмет - доливаємо в цільову стопку, скільки влізе
            int transfer = Math.Min(source.Count, Math.Max(0, target.Item.StackLimit - target.Count));
            if (transfer == 0)
            {
                // Цільова стопка вже повна
                return false;
            }

            slots[to] = new InventorySlot(target.Item, target.Count + transfer);

            if (source.Count == transfer)
            {
                slots[from] = null;
            }
            else
            {
                slots[from] = new InventorySlot(source.Item, source.Count - transfer);
            }
        }
        else
        {
            // Різні предмети (або порожньо) - міняємо місцями
            slots[to] = source;
            if (target.IsEmpty)
            {
                slots[from] = null;
            }
            else
            {
                slots[from] = target;
            }
        }

        if (Changed != null)
        {
            Changed.Invoke();
        }
        return true;
    }

    // Замінити один предмет expected у слоті index на replacement (або на порожнечу).
    // Використовується екіпіровкою, коли міняємо зброю.
    internal bool TryExchangeSingle(int index, ItemData expected, ItemData replacement)
    {
        if (!Valid(index))
        {
            return false;
        }
        if (GetSlot(index).Item != expected || GetSlot(index).Count != 1)
        {
            return false;
        }
        // Замінник має бути "поштучним" і без конфлікту id
        if (replacement != null && (replacement.StackLimit != 1 || Conflicts(replacement)))
        {
            return false;
        }

        if (replacement == null)
        {
            slots[index] = null;
        }
        else
        {
            slots[index] = new InventorySlot(replacement, 1);
        }

        if (Changed != null)
        {
            Changed.Invoke();
        }
        return true;
    }

    // Чи існує слот з таким номером
    private static bool Valid(int index)
    {
        return index >= 0 && index < Capacity;
    }
}

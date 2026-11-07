using System;
using System.Collections.Generic;
using UnityEngine;

// Старий формат "предмет + кількість" (залишився для сумісності зі сценою)
[Serializable]
public sealed class BattleItemStack
{
    public BattleConsumable definition;

    [Min(0)]
    public int count;
}

// Інвентар гравця на час сесії. Висить на гравці у світі.
// Якщо об'єкт ховається/показується - кількість предметів НЕ скидається.
[DisallowMultipleComponent]
public sealed class BattleInventory : MonoBehaviour
{
    // Старі записи зі сцени. Тепер це просто стартові предмети
    // (залишені, щоб не зламати посилання в сцені та ассетах).
    public List<BattleItemStack> items = new List<BattleItemStack>();

    // Нові стартові предмети
    public List<StartingItem> startingItems = new List<StartingItem>();

    // Сам інвентар (30 слотів). Створюється при першому зверненні
    [NonSerialized]
    private InventoryState state;

    public InventoryState State
    {
        get
        {
            // Якщо вже створили - просто повертаємо
            if (state != null)
            {
                return state;
            }

            InventoryState candidate = new InventoryState();

            // Спочатку кладемо предмети зі старого списку
            if (items != null)
            {
                foreach (BattleItemStack seed in items)
                {
                    if (seed != null && seed.definition != null && seed.count > 0)
                    {
                        bool added = candidate.TryAdd(seed.definition, seed.count);
                        if (!added)
                        {
                            throw new InvalidOperationException("Starting items do not fit into the 30-slot inventory.");
                        }
                    }
                }
            }

            // Потім - з нового списку
            if (startingItems != null)
            {
                foreach (StartingItem seed in startingItems)
                {
                    if (seed != null && seed.definition != null && seed.count > 0)
                    {
                        bool added = candidate.TryAdd(seed.definition, seed.count);
                        if (!added)
                        {
                            throw new InvalidOperationException("Starting items do not fit into the 30-slot inventory.");
                        }
                    }
                }
            }

            state = candidate;
            return state;
        }
    }

    // Список всіх витратних предметів, які є або були в інвентарі (без повторів).
    // Спочатку ті, що лежать в інвентарі, потім ті, що є у старому списку items.
    public IEnumerable<BattleConsumable> Definitions
    {
        get
        {
            List<BattleConsumable> result = new List<BattleConsumable>();

            // 1) Предмети з інвентаря, які є BattleConsumable
            foreach (ItemData definition in State.Definitions)
            {
                BattleConsumable consumable = definition as BattleConsumable;
                if (consumable != null)
                {
                    if (!result.Contains(consumable))
                    {
                        result.Add(consumable);
                    }
                }
            }

            // 2) Предмети зі старого списку items
            if (items != null)
            {
                foreach (BattleItemStack stack in items)
                {
                    if (stack != null && stack.definition != null)
                    {
                        if (!result.Contains(stack.definition))
                        {
                            result.Add(stack.definition);
                        }
                    }
                }
            }

            return result;
        }
    }

    // Скільки штук такого предмета є
    public int Count(ItemData definition)
    {
        return State.Count(definition);
    }

    // Забрати 1 штуку предмета
    public bool TryConsume(BattleConsumable definition)
    {
        return State.TryRemove(definition, 1);
    }

    // Додати предмет
    public bool TryAdd(ItemData definition, int amount)
    {
        return State.TryAdd(definition, amount);
    }
}

// Стартовий предмет: що і скільки покласти в інвентар на початку
[Serializable]
public sealed class StartingItem
{
    public ItemData definition;

    [Min(0)]
    public int count = 1;
}

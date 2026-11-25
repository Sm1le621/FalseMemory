using System;
using System.Collections.Generic;
using UnityEngine;

// Інвентар гравця на час сесії. Висить на гравці у світі.
// Якщо об'єкт ховається/показується - кількість предметів НЕ скидається.
[DisallowMultipleComponent]
public sealed class BattleInventory : MonoBehaviour
{
    // Стартові предмети (задаються в інспекторі)
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

            // Кладемо стартові предмети
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

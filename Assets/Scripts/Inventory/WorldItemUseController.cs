using UnityEngine;

// Використання предметів у світі (не в бою) через кнопку "Use" у вікні інвентаря.
[RequireComponent(typeof(BattleUnit), typeof(InventoryWindow))]
public sealed class WorldItemUseController : MonoBehaviour
{
    private BattleUnit unit;
    private InventoryWindow window;
    // В якому кадрі останній раз використали предмет (щоб не використати двічі за кадр)
    private int lastUseFrame = -1;

    private void Awake()
    {
        unit = GetComponent<BattleUnit>();
        window = GetComponent<InventoryWindow>();
    }

    private void OnEnable()
    {
        if (window == null)
        {
            return;
        }

        window.UseRequested += HandleUse;
        // Вікно буде питати в нас, чи можна натиснути "Use"
        window.CanUseSelection = CanUse;
    }

    private void OnDisable()
    {
        if (window == null)
        {
            return;
        }

        window.UseRequested -= HandleUse;
        // Прибираємо перевірку, тільки якщо це досі наша
        if (window.CanUseSelection == CanUse)
        {
            window.CanUseSelection = null;
        }
    }

    // Чи можна використати предмет зі слота index
    public bool CanUse(int index)
    {
        if (!isActiveAndEnabled || unit == null || window == null || !window.IsOpen)
        {
            return false;
        }
        if (index < 0 || index >= InventoryState.Capacity)
        {
            return false;
        }
        // У бою через цей скрипт нічого не використовуємо
        if (BattleTransition.Instance != null && BattleTransition.Instance.EncounterActive)
        {
            return false;
        }

        BattleConsumable item = unit.Inventory.State.GetSlot(index).Item as BattleConsumable;
        return BattleAbilities.CanUseItem(unit, unit, item, ItemUseLocation.Exploration);
    }

    // Використати предмет зі слота index
    public bool TryUse(int index)
    {
        if (lastUseFrame == Time.frameCount || !CanUse(index))
        {
            return false;
        }

        BattleConsumable item = (BattleConsumable)unit.Inventory.State.GetSlot(index).Item;
        lastUseFrame = Time.frameCount;

        int amount;
        bool ok = BattleAbilities.TryUseItem(unit, unit, item, out amount, ItemUseLocation.Exploration);

        if (ok)
        {
            string resource;
            if (item.manaRestoration > 0)
            {
                resource = "MP";
            }
            else
            {
                resource = "HP";
            }
            window.ShowMessage(item.displayName + ": +" + amount + " " + resource);
        }
        else
        {
            window.ShowMessage("This item cannot be used now.");
        }

        window.Refresh();
        return ok;
    }

    private void HandleUse(int index)
    {
        TryUse(index);
    }
}

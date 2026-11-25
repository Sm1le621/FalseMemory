using UnityEngine;

// Дії з вікна інвентаря: одягнути / зняти екіпіровку і використати зілля у світі.
// Висить на гравці разом з BattleUnit, BattleInventory і InventoryWindow.
// Сам нічого не малює - тільки слухає події вікна і виконує дію.
[RequireComponent(typeof(BattleUnit), typeof(BattleInventory))]
public sealed class InventoryActions : MonoBehaviour
{
    // Що зараз одягнено (зброя і броня)
    public EquipmentState State { get; } = new EquipmentState();

    private BattleUnit unit;
    private BattleInventory inventory;
    private InventoryWindow window;
    // В якому кадрі останній раз використали предмет (щоб не використати двічі за кадр)
    private int lastUseFrame = -1;

    private void Awake()
    {
        unit = GetComponent<BattleUnit>();
        inventory = GetComponent<BattleInventory>();
        window = GetComponent<InventoryWindow>();
    }

    // Підписуємось на кнопки вікна
    private void OnEnable()
    {
        if (window == null)
        {
            return;
        }

        window.EquipRequested += HandleEquip;
        window.UnequipRequested += HandleUnequip;
        window.UseRequested += HandleUse;
        // Вікно буде питати в нас, чи можна натиснути "Use"
        window.CanUseSelection = CanUse;
        // Коли змінюється екіпіровка - перемальовуємо вікно
        State.Changed += window.Refresh;
    }

    // Відписуємось (обов'язково, інакше будуть помилки після видалення об'єкта)
    private void OnDisable()
    {
        if (window == null)
        {
            return;
        }

        window.EquipRequested -= HandleEquip;
        window.UnequipRequested -= HandleUnequip;
        window.UseRequested -= HandleUse;
        // Прибираємо перевірку, тільки якщо це досі наша
        if (window.CanUseSelection == CanUse)
        {
            window.CanUseSelection = null;
        }
        State.Changed -= window.Refresh;
    }

    // Чи можна зараз щось робити з інвентарем (не в бою і гравець живий)
    private bool CanAct
    {
        get
        {
            if (!isActiveAndEnabled || unit == null || !unit.IsAlive)
            {
                return false;
            }
            if (BattleTransition.Instance != null && BattleTransition.Instance.EncounterActive)
            {
                return false;
            }
            return true;
        }
    }

    // ===== Екіпіровка =====

    // Одягнути предмет зі слота інвентаря index.
    // target - в який слот (якщо null - вибираємо автоматично)
    public bool TryEquip(int index, EquipmentSlot? target = null)
    {
        if (!CanAct || index < 0 || index >= InventoryState.Capacity)
        {
            return false;
        }

        ItemData item = inventory.State.GetSlot(index).Item;
        if (item == null)
        {
            return false;
        }

        EquipmentSlot slot;
        if (target.HasValue)
        {
            slot = target.Value;
        }
        else if (item.category == ItemCategory.Weapon)
        {
            // Зброя завжди йде в слот зброї
            slot = EquipmentSlot.Weapon;
        }
        else
        {
            // Броня - в свій слот
            slot = item.equipmentSlot;
        }

        return State.TryEquip(inventory.State, index, slot);
    }

    // Зняти предмет зі слота
    public bool TryUnequip(EquipmentSlot slot)
    {
        if (!CanAct)
        {
            return false;
        }
        return State.TryUnequip(inventory.State, slot);
    }

    // Натиснули "одягнути" у вікні
    private void HandleEquip(int index, EquipmentSlot? target)
    {
        bool ok = TryEquip(index, target);
        if (ok)
        {
            window.ShowMessage("Item equipped.");
        }
        else
        {
            window.ShowMessage("Cannot equip this item.");
        }
        window.Refresh();
    }

    // Натиснули "зняти" у вікні
    private void HandleUnequip(EquipmentSlot slot)
    {
        bool ok = TryUnequip(slot);
        if (ok)
        {
            window.ShowMessage("Item unequipped.");
        }
        else
        {
            window.ShowMessage("Cannot unequip: inventory is full or the slot is empty.");
        }
        window.Refresh();
    }

    // ===== Зілля у світі =====

    // Чи можна використати предмет зі слота index
    public bool CanUse(int index)
    {
        if (!CanAct || window == null || !window.IsOpen)
        {
            return false;
        }
        if (index < 0 || index >= InventoryState.Capacity)
        {
            return false;
        }

        BattleConsumable item = inventory.State.GetSlot(index).Item as BattleConsumable;
        return BattleCalculator.CanUseItem(unit, item);
    }

    // Використати предмет зі слота index
    public bool TryUse(int index)
    {
        if (lastUseFrame == Time.frameCount || !CanUse(index))
        {
            return false;
        }

        BattleConsumable item = (BattleConsumable)inventory.State.GetSlot(index).Item;
        lastUseFrame = Time.frameCount;

        int amount;
        bool ok = BattleCalculator.TryUseItem(unit, item, out amount);

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

    // Натиснули "Use" у вікні
    private void HandleUse(int index)
    {
        TryUse(index);
    }
}

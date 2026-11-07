using UnityEngine;

// Екіпіровка гравця: зв'язує вікно інвентаря з EquipmentState.
// Висить на гравці разом з BattleUnit і BattleInventory.
[RequireComponent(typeof(BattleUnit), typeof(BattleInventory))]
public sealed class EquipmentController : MonoBehaviour
{
    // Що зараз одягнено (зброя і броня)
    public EquipmentState State { get; } = new EquipmentState();

    private BattleUnit unit;
    private BattleInventory inventory;
    private InventoryWindow window;

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
        State.Changed -= window.Refresh;
    }

    // Чи можна зараз міняти екіпіровку (не в бою і гравець живий)
    private bool CanChange
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

    // Одягнути предмет зі слота інвентаря index.
    // target - в який слот (якщо null - вибираємо автоматично)
    public bool TryEquip(int index, EquipmentSlot? target = null)
    {
        if (!CanChange || index < 0 || index >= InventoryState.Capacity)
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
        if (!CanChange)
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
}

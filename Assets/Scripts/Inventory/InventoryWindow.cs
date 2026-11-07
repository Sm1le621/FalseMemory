using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Вікно інвентаря (відкривається клавішею I).
// 30 клітинок, кнопки Use / Equip, слоти екіпіровки, перетягування предметів.
[RequireComponent(typeof(BattleInventory), typeof(WorldInputGate))]
public sealed partial class InventoryWindow : MonoBehaviour
{
    // Головна панель вікна
    public RectTransform panel;
    // 30 клітинок
    public InventorySlotView[] slots;
    // Опис вибраного предмета
    public TextMeshProUGUI details;
    // Повідомлення ("Item equipped." і т.д.)
    public TextMeshProUGUI message;
    public Button useButton;
    public Button closeButton;
    // Іконка, яка їде за мишкою при перетягуванні
    public Image dragIcon;
    public Button equipButton;
    // Кнопки слотів екіпіровки (0 - зброя, 1 - броня)
    public Button[] equipmentButtons;
    // Текст "ATK 20  DEF 12"
    public TextMeshProUGUI equipmentStats;
    // Текст "HP 100 / 100  MP 30 / 30"
    public TextMeshProUGUI resourcesLabel;
    // Темний фон за вікном
    public Image modalBackdrop;
    public InventoryTooltip tooltip;
    // Текст "BAG 3 / 30"
    public TextMeshProUGUI bagLabel;

    // Події, на які підписуються інші скрипти
    // Натиснули "Use" (передається номер слота)
    public event Action<int> UseRequested;
    // Функція-перевірка: чи можна використати предмет у слоті
    public Func<int, bool> CanUseSelection;
    // Натиснули "Equip" (номер слота і, можливо, конкретний слот екіпіровки)
    public event Action<int, EquipmentSlot?> EquipRequested;
    // Попросили зняти предмет
    public event Action<EquipmentSlot> UnequipRequested;

    // Чи відкрите вікно
    public bool IsOpen
    {
        get { return panel != null && panel.gameObject.activeSelf; }
    }

    // Номер вибраної клітинки
    public int SelectedIndex { get; private set; }
    public BattleInventory Inventory { get; private set; }

    private WorldInputGate gate;
    private BattleUnit unit;
    // Номер клітинки, яку зараз тягнемо (-1 = нічого не тягнемо)
    private int dragging = -1;
    // Збережені обробники кнопок екіпіровки (щоб потім відписатись)
    private UnityEngine.Events.UnityAction[] equipmentClicks;
    // Що зараз показано в resourcesLabel (щоб не оновлювати текст кожен кадр)
    private int shownHP = -1;
    private int shownMaxHP = -1;
    private int shownMP = -1;
    private int shownMaxMP = -1;

    private void Awake()
    {
        Inventory = GetComponent<BattleInventory>();
        gate = GetComponent<WorldInputGate>();
        unit = GetComponent<BattleUnit>();

        // Кажемо кожній клітинці її номер
        int slotCount = 0;
        if (slots != null)
        {
            slotCount = slots.Length;
        }
        for (int i = 0; i < slotCount; i++)
        {
            slots[i].Bind(this, i);
        }

        // Підписуємо кнопки
        if (useButton != null)
        {
            useButton.onClick.AddListener(RequestUse);
        }
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
        if (equipButton != null)
        {
            equipButton.onClick.AddListener(RequestEquip);
        }

        int equipmentCount = 0;
        if (equipmentButtons != null)
        {
            equipmentCount = equipmentButtons.Length;
        }
        equipmentClicks = new UnityEngine.Events.UnityAction[equipmentCount];
        for (int i = 0; i < equipmentClicks.Length; i++)
        {
            // Окрема змінна, щоб кожна кнопка запам'ятала свій слот
            EquipmentSlot equipmentSlot = (EquipmentSlot)i;
            equipmentClicks[i] = () => ClickEquipmentSlot(equipmentSlot);
            equipmentButtons[i].onClick.AddListener(equipmentClicks[i]);
        }

        // На старті вікно закрите
        Close();
    }

    private void OnEnable()
    {
        if (Application.isPlaying && Inventory != null)
        {
            // Інвентар змінився - перемальовуємо
            Inventory.State.Changed += Refresh;
        }
    }

    private void OnDisable()
    {
        if (Inventory != null)
        {
            Inventory.State.Changed -= Refresh;
        }
        Close();
    }

    // Відписуємо всі кнопки
    private void OnDestroy()
    {
        if (useButton != null)
        {
            useButton.onClick.RemoveListener(RequestUse);
        }
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Close);
        }
        if (equipButton != null)
        {
            equipButton.onClick.RemoveListener(RequestEquip);
        }

        int clickCount = 0;
        if (equipmentClicks != null)
        {
            clickCount = equipmentClicks.Length;
        }
        for (int i = 0; i < clickCount; i++)
        {
            if (equipmentButtons[i] != null)
            {
                equipmentButtons[i].onClick.RemoveListener(equipmentClicks[i]);
            }
        }
    }

    private void Update()
    {
        if (IsOpen)
        {
            RefreshResources();
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        // I - відкрити / закрити
        if (keyboard.iKey.wasPressedThisFrame)
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
            return;
        }

        if (!IsOpen)
        {
            return;
        }

        // Esc - закрити
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            Close();
            return;
        }

        // Стрілки - рух по клітинках (сітка 6 в ряд)
        int offset = 0;
        if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            offset = -1;
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            offset = 1;
        }
        else if (keyboard.upArrowKey.wasPressedThisFrame)
        {
            offset = -6;
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            offset = 6;
        }

        if (offset != 0)
        {
            Select(Mathf.Clamp(SelectedIndex + offset, 0, InventoryState.Capacity - 1));
        }
    }

    // Відкрити вікно. Не відкривається в бою і коли гравець мертвий
    public bool Open()
    {
        if (panel == null || !isActiveAndEnabled || unit == null || !unit.IsAlive)
        {
            return false;
        }
        if (BattleTransition.Instance != null && BattleTransition.Instance.EncounterActive)
        {
            return false;
        }

        if (modalBackdrop != null)
        {
            modalBackdrop.gameObject.SetActive(true);
        }
        panel.gameObject.SetActive(true);
        // Блокуємо рух гравця, поки вікно відкрите
        gate.Acquire(this);
        if (message != null)
        {
            message.text = "";
        }
        Refresh();
        return true;
    }

    // Закрити вікно
    public void Close()
    {
        if (tooltip != null)
        {
            tooltip.Hide();
        }
        EndDrag();

        if (panel != null)
        {
            panel.gameObject.SetActive(false);
        }
        if (modalBackdrop != null)
        {
            modalBackdrop.gameObject.SetActive(false);
        }
        // Знову дозволяємо рух
        if (gate != null)
        {
            gate.Release(this);
        }
    }

    // Вибрати клітинку
    public void Select(int index)
    {
        if (!IsOpen || index < 0 || index >= InventoryState.Capacity)
        {
            return;
        }

        SelectedIndex = index;
        Refresh();
    }

    // Перемалювати все вікно
    public void Refresh()
    {
        if (!IsOpen || Inventory == null)
        {
            return;
        }

        // 1) Клітинки
        int slotCount = 0;
        if (slots != null)
        {
            slotCount = slots.Length;
        }
        for (int i = 0; i < slotCount; i++)
        {
            slots[i].Render(Inventory.State.GetSlot(i), i == SelectedIndex);
        }

        InventorySlot selected = Inventory.State.GetSlot(SelectedIndex);
        ItemData item = selected.Item;

        // 2) HP / MP
        RefreshResources();

        // 3) Опис вибраного предмета
        if (details != null)
        {
            if (selected.IsEmpty)
            {
                details.text = "Select an item.";
            }
            else
            {
                details.text = ItemPresentation.Description(item, selected.Count);
            }
        }

        // 4) Скільки клітинок зайнято
        if (bagLabel != null)
        {
            int occupied = 0;
            for (int i = 0; i < InventoryState.Capacity; i++)
            {
                if (!Inventory.State.GetSlot(i).IsEmpty)
                {
                    occupied = occupied + 1;
                }
            }
            bagLabel.text = "BAG   " + occupied + " / " + InventoryState.Capacity;
        }

        // 5) Кнопка "Use": тільки для витратних предметів, і якщо хтось слухає подію
        if (useButton != null)
        {
            bool canUse = !selected.IsEmpty && item is BattleConsumable && UseRequested != null;
            if (canUse && CanUseSelection != null)
            {
                canUse = CanUseSelection(SelectedIndex);
            }
            useButton.interactable = canUse;
        }

        EquipmentController equipment = GetComponent<EquipmentController>();

        // 6) Кнопка "Equip": тільки для екіпіровки
        if (equipButton != null)
        {
            equipButton.interactable = !selected.IsEmpty && item.IsEquipment && EquipRequested != null;
        }

        // 7) Слоти екіпіровки
        int equipmentCount = 0;
        if (equipmentButtons != null)
        {
            equipmentCount = equipmentButtons.Length;
        }
        for (int i = 0; i < equipmentCount; i++)
        {
            EquipmentSlot slot = (EquipmentSlot)i;

            // Що одягнено в цьому слоті
            ItemData worn = null;
            if (equipment != null)
            {
                worn = equipment.State.GetSlot(slot);
            }

            EquipmentSlotView view = equipmentButtons[i].GetComponent<EquipmentSlotView>();
            if (view != null)
            {
                view.Render(worn);
            }
            else
            {
                // Якщо немає EquipmentSlotView - просто пишемо текст на кнопці
                string wornName;
                if (worn != null)
                {
                    wornName = worn.displayName;
                }
                else
                {
                    wornName = "—";
                }
                equipmentButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = slot + ": " + wornName;
            }

            // Слот можна натиснути, якщо в ньому щось є (зняти) або вибраний предмет підходить (одягнути)
            bool clickable = false;
            if (equipment != null)
            {
                if (worn != null)
                {
                    clickable = true;
                }
                else if (item != null && item.Fits(slot))
                {
                    clickable = true;
                }
            }
            equipmentButtons[i].interactable = clickable;
        }

        // 8) Атака і захист з урахуванням екіпіровки
        if (equipmentStats != null && unit != null)
        {
            equipmentStats.text = "ATK " + unit.EffectiveAttack + "     DEF " + unit.EffectiveDefense;
        }

        // 9) Оновлюємо підказку
        if (tooltip != null)
        {
            tooltip.RefreshContent();
        }
    }

    // Оновити текст HP / MP (тільки якщо щось змінилось)
    private void RefreshResources()
    {
        if (resourcesLabel == null || unit == null || unit.Health == null)
        {
            return;
        }

        int hp = unit.Health.currentHealth;
        int maxHP = unit.Health.maxHealth;
        int mp = unit.CurrentMP;
        int maxMP = unit.MaxMP;

        // Нічого не змінилось - не чіпаємо текст
        if (hp == shownHP && maxHP == shownMaxHP && mp == shownMP && maxMP == shownMaxMP)
        {
            return;
        }

        shownHP = hp;
        shownMaxHP = maxHP;
        shownMP = mp;
        shownMaxMP = maxMP;
        resourcesLabel.text = "HP " + hp + " / " + maxHP + "\nMP " + mp + " / " + maxMP;
    }

    // Натиснули "Use"
    private void RequestUse()
    {
        if (IsOpen && UseRequested != null)
        {
            UseRequested.Invoke(SelectedIndex);
        }
    }

    // Натиснули "Equip"
    private void RequestEquip()
    {
        if (IsOpen && EquipRequested != null)
        {
            EquipRequested.Invoke(SelectedIndex, null);
        }
    }

    // Натиснули на слот екіпіровки
    private void ClickEquipmentSlot(EquipmentSlot slot)
    {
        if (!IsOpen)
        {
            return;
        }

        InventorySlot selected = Inventory.State.GetSlot(SelectedIndex);

        if (!selected.IsEmpty && selected.Item.Fits(slot))
        {
            // Вибраний предмет підходить - одягаємо його в цей слот
            if (EquipRequested != null)
            {
                EquipRequested.Invoke(SelectedIndex, slot);
            }
        }
        else
        {
            // Інакше - знімаємо те, що в слоті
            if (UnequipRequested != null)
            {
                UnequipRequested.Invoke(slot);
            }
        }
    }

    public void ShowMessage(string text)
    {
        if (message != null)
        {
            message.text = text;
        }
    }

    // Почати перетягування з клітинки index
    public void BeginDrag(int index, Vector2 position)
    {
        if (!IsOpen || Inventory.State.GetSlot(index).IsEmpty)
        {
            return;
        }

        if (tooltip != null)
        {
            tooltip.Hide();
        }

        Select(index);
        dragging = index;
        dragIcon.sprite = Inventory.State.GetSlot(index).Item.icon;
        dragIcon.gameObject.SetActive(true);
        MoveDrag(position);
    }

    // Іконка їде за мишкою
    public void MoveDrag(Vector2 position)
    {
        if (dragging < 0 || dragIcon == null)
        {
            return;
        }

        RectTransform parent = dragIcon.transform.parent as RectTransform;
        Vector2 local;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, position, null, out local))
        {
            dragIcon.rectTransform.anchoredPosition = local;
        }
    }

    // Кинули предмет на клітинку destination
    public bool DropOn(int destination)
    {
        if (!IsOpen || dragging < 0)
        {
            return false;
        }

        bool ok = Inventory.State.TryMoveOrMerge(dragging, destination);
        EndDrag();

        if (ok)
        {
            Select(destination);
        }
        else
        {
            ShowMessage("Cannot move this item. Check the slot and stack limit.");
        }
        return ok;
    }

    // Закінчити перетягування
    public void EndDrag()
    {
        dragging = -1;
        if (dragIcon != null)
        {
            dragIcon.gameObject.SetActive(false);
        }
    }
}

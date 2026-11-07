using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Підказка з описом предмета, яка з'являється біля мишки.
public sealed class InventoryTooltip : MonoBehaviour
{
    public InventoryWindow owner;
    // Сама панель підказки
    public RectTransform panel;
    // Текст опису
    public TextMeshProUGUI content;
    public Outline border;

    // Над якою клітинкою зараз мишка (InventorySlotView або EquipmentSlotView)
    private Object source;
    // Позиція мишки на екрані
    private Vector2 pointer;

    // Чи видно підказку
    public bool IsVisible
    {
        get { return panel != null && panel.gameObject.activeSelf; }
    }

    // Показати підказку для клітинки інвентаря
    public void Show(InventorySlotView slot, Vector2 position)
    {
        source = slot;
        pointer = position;
        RefreshContent();
    }

    // Показати підказку для клітинки екіпіровки
    public void Show(EquipmentSlotView slot, Vector2 position)
    {
        source = slot;
        pointer = position;
        RefreshContent();
    }

    // Сховати підказку.
    // Якщо передали caller - ховаємо тільки якщо підказка саме від нього
    // (щоб одна клітинка не сховала підказку іншої).
    public void Hide(Object caller = null)
    {
        if (caller != null && caller != source)
        {
            return;
        }

        source = null;
        if (panel != null)
        {
            panel.gameObject.SetActive(false);
        }
    }

    // Оновити текст і розмір підказки
    public void RefreshContent()
    {
        // Вікно закрите або зараз щось перетягуємо - підказка не потрібна
        if (owner == null || !owner.IsOpen || owner.dragIcon.gameObject.activeSelf)
        {
            Hide();
            return;
        }

        ItemData item = null;
        int count = 1;

        // Якщо це клітинка інвентаря - беремо предмет і кількість з інвентаря
        InventorySlotView bag = source as InventorySlotView;
        if (bag != null)
        {
            InventorySlot entry = owner.Inventory.State.GetSlot(bag.Index);
            item = entry.Item;
            count = entry.Count;
        }

        // Якщо це клітинка екіпіровки - беремо одягнений предмет
        EquipmentSlotView equipment = source as EquipmentSlotView;
        if (equipment != null)
        {
            item = equipment.Item;
        }

        // Порожня клітинка - нічого показувати
        if (item == null)
        {
            Hide();
            return;
        }

        content.text = ItemPresentation.Description(item, count);

        if (border != null)
        {
            border.effectColor = new Color32(150, 133, 101, 255);
        }

        // Розмір панелі під текст, але не більше за екран
        RectTransform parent = panel.parent as RectTransform;
        float width = Mathf.Min(320, parent.rect.width - 24);
        float textHeight = content.GetPreferredValues(content.text, width - 32, 0).y;
        float height = Mathf.Min(textHeight + 32, parent.rect.height - 24);
        panel.sizeDelta = new Vector2(width, height);

        // Відступи тексту від країв панелі
        content.rectTransform.offsetMin = new Vector2(16, 16);
        content.rectTransform.offsetMax = new Vector2(-16, -16);

        panel.gameObject.SetActive(true);
        // Поверх усього
        panel.SetAsLastSibling();
        Position(pointer);
    }

    private void LateUpdate()
    {
        if (!IsVisible)
        {
            return;
        }

        if (owner == null || !owner.IsOpen)
        {
            Hide();
            return;
        }

        // Підказка їде за мишкою
        if (Mouse.current != null)
        {
            pointer = Mouse.current.position.ReadValue();
        }
        Position(pointer);
    }

    // Поставити підказку біля точки на екрані, але так, щоб вона не вилізла за край
    public void Position(Vector2 screenPosition)
    {
        RectTransform parent = panel.parent as RectTransform;
        if (parent == null)
        {
            return;
        }

        // Переводимо екранні координати в координати батьківського об'єкта
        Vector2 local;
        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPosition, null, out local);
        if (!inside)
        {
            return;
        }

        // Трохи правіше і нижче курсора
        local = local + new Vector2(22, -22);

        Rect bounds = parent.rect;
        local.x = Mathf.Clamp(local.x, bounds.xMin + 8, bounds.xMax - panel.rect.width - 8);
        local.y = Mathf.Clamp(local.y, bounds.yMin + panel.rect.height + 8, bounds.yMax - 8);

        panel.anchoredPosition = local;
    }
}

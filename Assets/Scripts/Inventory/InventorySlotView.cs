using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Одна клітинка інвентаря у вікні.
// Реагує на мишку: клік (вибір), наведення (підказка), перетягування (drag & drop).
public sealed class InventorySlotView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image background;
    public Image icon;
    // Кількість предметів у клітинці
    public TextMeshProUGUI quantity;
    public Outline border;

    private bool hovered;
    private bool selected;
    private ItemData item;

    // Вікно, якому належить клітинка
    public InventoryWindow Owner { get; private set; }
    // Номер клітинки (від 0 до 29)
    public int Index { get; private set; }

    // Прив'язати клітинку до вікна
    public void Bind(InventoryWindow owner, int index)
    {
        Owner = owner;
        Index = index;
    }

    // Намалювати вміст слота
    public void Render(InventorySlot slot, bool selected)
    {
        this.selected = selected;
        item = slot.Item;
        RefreshColors();

        if (slot.Item != null)
        {
            icon.sprite = slot.Item.icon;
        }
        else
        {
            icon.sprite = null;
        }
        icon.gameObject.SetActive(icon.sprite != null);

        // Для порожньої клітинки число не показуємо
        if (slot.IsEmpty)
        {
            quantity.text = "";
        }
        else
        {
            quantity.text = slot.Count.ToString();
        }
    }

    // Кольори фону і рамки
    private void RefreshColors()
    {
        background.color = ItemPresentation.CellColor(item, selected || hovered);

        if (border != null)
        {
            if (selected)
            {
                // Вибрана клітинка - товста золота рамка
                border.effectColor = new Color32(214, 178, 110, 255);
                border.effectDistance = new Vector2(2, -2);
            }
            else
            {
                border.effectColor = new Color32(73, 66, 56, 255);
                border.effectDistance = new Vector2(1, -1);
            }
        }
    }

    private void OnDisable()
    {
        hovered = false;
        RefreshColors();
        if (Owner != null && Owner.tooltip != null)
        {
            Owner.tooltip.Hide(this);
        }
    }

    // Мишка навелась
    public void OnPointerEnter(PointerEventData e)
    {
        hovered = true;
        RefreshColors();
        if (Owner != null && Owner.tooltip != null)
        {
            Owner.tooltip.Show(this, e.position);
        }
    }

    // Мишка пішла
    public void OnPointerExit(PointerEventData e)
    {
        hovered = false;
        RefreshColors();
        if (Owner != null && Owner.tooltip != null)
        {
            Owner.tooltip.Hide(this);
        }
    }

    // Клік лівою кнопкою - вибрати клітинку
    public void OnPointerClick(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Left)
        {
            Owner.Select(Index);
        }
    }

    // Почали тягнути
    public void OnBeginDrag(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Left)
        {
            Owner.BeginDrag(Index, e.position);
        }
    }

    // Тягнемо
    public void OnDrag(PointerEventData e)
    {
        Owner.MoveDrag(e.position);
    }

    // Відпустили (неважливо де)
    public void OnEndDrag(PointerEventData e)
    {
        Owner.EndDrag();
    }

    // На цю клітинку щось кинули
    public void OnDrop(PointerEventData e)
    {
        // Звідки тягнули
        InventorySlotView source = null;
        if (e.pointerDrag != null)
        {
            source = e.pointerDrag.GetComponent<InventorySlotView>();
        }

        // Приймаємо тільки клітинки з цього ж вікна
        if (source != null && source.Owner == Owner)
        {
            Owner.DropOn(Index);
        }
    }
}

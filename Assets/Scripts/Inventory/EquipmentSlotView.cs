using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Клітинка екіпіровки у вікні (зброя або броня).
// Реагує на наведення мишки (показує підказку).
public sealed class EquipmentSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryWindow owner;
    // Який це слот
    public EquipmentSlot slot;
    public Image icon;
    public Image background;
    public Outline border;
    // Підпис "Weapon" / "Chest"
    public TextMeshProUGUI slotLabel;

    // Що зараз намальовано в клітинці
    private ItemData item;
    // Чи наведена мишка
    private bool hovered;

    // Предмет, який зараз реально одягнено в цьому слоті
    public ItemData Item
    {
        get
        {
            if (owner != null && owner.GetComponent<EquipmentController>() != null)
            {
                return owner.GetComponent<EquipmentController>().State.GetSlot(slot);
            }
            return null;
        }
    }

    // Намалювати клітинку з предметом worn (null - порожньо)
    public void Render(ItemData worn)
    {
        item = worn;

        if (item != null)
        {
            icon.sprite = item.icon;
        }
        else
        {
            icon.sprite = null;
        }
        // Без картинки іконку ховаємо
        icon.gameObject.SetActive(icon.sprite != null);

        slotLabel.text = ItemPresentation.SlotName(slot);
        RefreshColors();
    }

    // Кольори фону і рамки
    private void RefreshColors()
    {
        background.color = ItemPresentation.CellColor(item, hovered);

        if (border != null)
        {
            if (item != null)
            {
                border.effectColor = new Color32(150, 133, 101, 255);
            }
            else
            {
                border.effectColor = new Color32(109, 95, 74, 255);
            }
        }
    }

    private void OnDisable()
    {
        hovered = false;
        RefreshColors();
        HideTooltip();
    }

    // Мишка навелась на клітинку
    public void OnPointerEnter(PointerEventData e)
    {
        hovered = true;
        RefreshColors();
        if (owner != null && owner.tooltip != null)
        {
            owner.tooltip.Show(this, e.position);
        }
    }

    // Мишка пішла з клітинки
    public void OnPointerExit(PointerEventData e)
    {
        hovered = false;
        RefreshColors();
        HideTooltip();
    }

    private void HideTooltip()
    {
        if (owner != null && owner.tooltip != null)
        {
            owner.tooltip.Hide(this);
        }
    }
}

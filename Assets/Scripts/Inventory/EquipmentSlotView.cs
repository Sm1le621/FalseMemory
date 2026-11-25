using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Клітинка екіпіровки у вікні (зброя або броня).
// Тільки малює предмет; клік обробляє Button на цьому ж об'єкті.
public sealed class EquipmentSlotView : MonoBehaviour
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

    // Предмет, який зараз реально одягнено в цьому слоті
    public ItemData Item
    {
        get
        {
            if (owner != null && owner.GetComponent<InventoryActions>() != null)
            {
                return owner.GetComponent<InventoryActions>().State.GetSlot(slot);
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
        background.color = ItemPresentation.CellColor(item, false);

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
}

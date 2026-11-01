using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class EquipmentSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryWindow owner;
    public EquipmentSlot slot;
    public Image icon, background;
    public Outline border;
    public TextMeshProUGUI slotLabel;
    private ItemData item;
    private bool hovered;
    public ItemData Item=>owner!=null && owner.GetComponent<EquipmentController>()!=null?owner.GetComponent<EquipmentController>().State.GetSlot(slot):null;
    public void Render(ItemData worn)
    {
        item=worn;icon.sprite=item!=null?item.icon:null;icon.gameObject.SetActive(icon.sprite!=null);
        slotLabel.text=ItemPresentation.SlotName(slot);RefreshColors();
    }
    private void RefreshColors(){background.color=ItemPresentation.CellColor(item,hovered);if(border!=null)border.effectColor=item!=null?new Color32(150,133,101,255):new Color32(109,95,74,255);}
    private void OnDisable(){hovered=false;RefreshColors();if(owner!=null && owner.tooltip!=null)owner.tooltip.Hide(this);}
    public void OnPointerEnter(PointerEventData e){hovered=true;RefreshColors();if(owner!=null && owner.tooltip!=null)owner.tooltip.Show(this,e.position);}
    public void OnPointerExit(PointerEventData e){hovered=false;RefreshColors();if(owner!=null && owner.tooltip!=null)owner.tooltip.Hide(this);}
}

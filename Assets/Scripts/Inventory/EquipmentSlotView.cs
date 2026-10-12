using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class EquipmentSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler,IDropHandler
{
    public InventoryWindow owner;
    public EquipmentSlot slot;
    public Image icon, background;
    public Outline rarityBorder;
    public TextMeshProUGUI slotLabel;
    public Sprite emptySprite;
    public bool? DropAllowed {get;private set;}
    private ItemData item;
    private bool hovered;
    public ItemData Item=>owner!=null && owner.GetComponent<EquipmentController>()!=null?owner.GetComponent<EquipmentController>().State.GetSlot(slot):null;
    public void Render(ItemData worn)
    {
        item=worn;icon.sprite=item!=null?item.icon:emptySprite;icon.color=item!=null?Color.white:new Color(.65f,.65f,.65f,.4f);icon.gameObject.SetActive(icon.sprite!=null);
        slotLabel.text=ItemPresentation.SlotName(slot);RefreshColors();
    }
    public void SetDropAllowed(bool? value){DropAllowed=value;RefreshColors();}
    private void RefreshColors(){background.color=DropAllowed.HasValue?(DropAllowed.Value?new Color32(40,76,48,255):new Color32(91,39,43,255)):ItemPresentation.CellColor(item,hovered);if(rarityBorder!=null)rarityBorder.effectColor=item!=null?ItemPresentation.RarityColor(item.rarity):new Color32(109,95,74,255);}
    private void OnDisable(){hovered=false;RefreshColors();if(owner!=null && owner.tooltip!=null)owner.tooltip.Hide(this);}
    public void OnPointerEnter(PointerEventData e){hovered=true;RefreshColors();if(owner!=null && owner.tooltip!=null)owner.tooltip.Show(this,e.position);}
    public void OnPointerExit(PointerEventData e){hovered=false;RefreshColors();if(owner!=null && owner.tooltip!=null)owner.tooltip.Hide(this);}
    public void OnBeginDrag(PointerEventData e){if(e.button==PointerEventData.InputButton.Left && owner!=null)owner.BeginEquipmentDrag(slot,e.position);}
    public void OnDrag(PointerEventData e){if(owner!=null)owner.MoveDrag(e.position);}
    public void OnEndDrag(PointerEventData e){if(owner!=null)owner.EndDrag();}
    public void OnDrop(PointerEventData e)
    {
        if(owner==null || e.pointerDrag==null)return;
        var bag=e.pointerDrag.GetComponent<InventorySlotView>();var gear=e.pointerDrag.GetComponent<EquipmentSlotView>();
        if((bag!=null && bag.Owner==owner)||(gear!=null && gear.owner==owner))owner.DropOnEquipment(slot);
    }
}

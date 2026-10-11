using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class InventorySlotView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image background, icon;
    public TextMeshProUGUI quantity;
    public InventoryWindow Owner {get;private set;}
    public int Index {get;private set;}
    public void Bind(InventoryWindow owner,int index){Owner=owner;Index=index;}
    public void Render(InventorySlot slot,bool selected)
    {
        background.color=selected?new Color32(82,64,86,255):new Color32(32,29,41,255);
        icon.sprite=slot.Item!=null?slot.Item.icon:null;icon.gameObject.SetActive(icon.sprite!=null);
        quantity.text=slot.IsEmpty?"":slot.Count.ToString();
    }
    public void OnPointerClick(PointerEventData e){if(e.button==PointerEventData.InputButton.Left)Owner.Select(Index);}
    public void OnBeginDrag(PointerEventData e){if(e.button==PointerEventData.InputButton.Left)Owner.BeginDrag(Index,e.position);}
    public void OnDrag(PointerEventData e)=>Owner.MoveDrag(e.position);
    public void OnEndDrag(PointerEventData e)=>Owner.EndDrag();
    public void OnDrop(PointerEventData e)
    {
        var source=e.pointerDrag!=null?e.pointerDrag.GetComponent<InventorySlotView>():null;
        if(source!=null && source.Owner==Owner)Owner.DropOn(Index);
    }
}

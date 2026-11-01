using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class InventorySlotView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image background, icon;
    public TextMeshProUGUI quantity;
    public Outline border;
    private bool hovered, selected;
    private ItemData item;
    public InventoryWindow Owner {get;private set;}
    public int Index {get;private set;}
    public void Bind(InventoryWindow owner,int index){Owner=owner;Index=index;}
    public void Render(InventorySlot slot,bool selected)
    {
        this.selected=selected;item=slot.Item;RefreshColors();
        icon.sprite=slot.Item!=null?slot.Item.icon:null;icon.gameObject.SetActive(icon.sprite!=null);
        quantity.text=slot.IsEmpty?"":slot.Count.ToString();
    }
    private void RefreshColors()
    {
        background.color=ItemPresentation.CellColor(item,selected || hovered);
        if(border!=null){border.effectColor=selected?new Color32(214,178,110,255):new Color32(73,66,56,255);border.effectDistance=selected?new Vector2(2,-2):new Vector2(1,-1);}
    }
    private void OnDisable(){hovered=false;RefreshColors();if(Owner!=null && Owner.tooltip!=null)Owner.tooltip.Hide(this);}
    public void OnPointerEnter(PointerEventData e){hovered=true;RefreshColors();if(Owner!=null && Owner.tooltip!=null)Owner.tooltip.Show(this,e.position);}
    public void OnPointerExit(PointerEventData e){hovered=false;RefreshColors();if(Owner!=null && Owner.tooltip!=null)Owner.tooltip.Hide(this);}
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

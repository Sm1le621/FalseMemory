using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class InventoryTooltip : MonoBehaviour
{
    public InventoryWindow owner;
    public RectTransform panel;
    public TextMeshProUGUI content;
    public Outline rarityBorder;
    private Object source;
    private Vector2 pointer;
    public bool IsVisible=>panel!=null && panel.gameObject.activeSelf;
    public void Show(InventorySlotView slot,Vector2 position){source=slot;pointer=position;RefreshContent();}
    public void Show(EquipmentSlotView slot,Vector2 position){source=slot;pointer=position;RefreshContent();}
    public void Hide(Object caller=null){if(caller!=null && caller!=source)return;source=null;if(panel!=null)panel.gameObject.SetActive(false);}
    public void RefreshContent()
    {
        if(owner==null || !owner.IsOpen || owner.dragIcon.gameObject.activeSelf){Hide();return;}
        ItemData item=null;int count=1;
        var bag=source as InventorySlotView;
        if(bag!=null){var entry=owner.Inventory.State.GetSlot(bag.Index);item=entry.Item;count=entry.Count;}
        var equipment=source as EquipmentSlotView;if(equipment!=null)item=equipment.Item;
        if(item==null){Hide();return;}
        content.text=ItemPresentation.Description(item,count);rarityBorder.effectColor=ItemPresentation.RarityColor(item.rarity);
        var parent=panel.parent as RectTransform;
        float width=Mathf.Min(320,parent.rect.width-24);
        float height=Mathf.Min(content.GetPreferredValues(content.text,width-32,0).y+32,parent.rect.height-24);
        panel.sizeDelta=new Vector2(width,height);content.rectTransform.offsetMin=new Vector2(16,16);content.rectTransform.offsetMax=new Vector2(-16,-16);
        panel.gameObject.SetActive(true);panel.SetAsLastSibling();Position(pointer);
    }
    private void LateUpdate(){if(!IsVisible)return;if(owner==null || !owner.IsOpen){Hide();return;}if(Mouse.current!=null)pointer=Mouse.current.position.ReadValue();Position(pointer);}
    public void Position(Vector2 screenPosition)
    {
        var parent=panel.parent as RectTransform;
        if(parent==null || !RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,screenPosition,null,out var local))return;
        local+=new Vector2(22,-22);var bounds=parent.rect;
        local.x=Mathf.Clamp(local.x,bounds.xMin+8,bounds.xMax-panel.rect.width-8);
        local.y=Mathf.Clamp(local.y,bounds.yMin+panel.rect.height+8,bounds.yMax-8);
        panel.anchoredPosition=local;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class CharacterAttributeView : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public CharacterWindow owner;
    public CharacterAttribute attribute;
    public TextMeshProUGUI label;
    public Button increase;
    public void OnPointerEnter(PointerEventData data){if(owner!=null)owner.ShowAttribute(attribute,data.position,true);}
    public void OnPointerExit(PointerEventData data){if(owner!=null)owner.HideTooltip();}
}

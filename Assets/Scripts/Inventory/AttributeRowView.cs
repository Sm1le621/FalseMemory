using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class AttributeRowView : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public InventoryWindow owner;
    public CharacterAttribute attribute;
    public TextMeshProUGUI label;
    public string Key => "attribute."+attribute.ToString().ToLowerInvariant();
    public void Render(CharacterAttributeValues values,CharacterAttributeValues gear)
    {
        int bonus=gear.Get(attribute);
        string suffix=bonus==0?"":GameText.Get(bonus>0?"character.gear_positive":"character.gear_negative",bonus);
        label.text=GameText.Get("character.attribute_row",GameText.Get(Key+".abbr"),GameText.Get(Key+".name"),values.Get(attribute),suffix);
    }
    public string Description()
    {
        var unit=owner.GetComponent<BattleUnit>();var primary=unit.Attributes;
        if(primary==null)return "";
        if(primary.rules!=null && primary.rules.classlessProgression)
            return GameText.Get("character.attribute_tooltip_classless",GameText.Get(Key+".abbr"),GameText.Get(Key+".name"),GameText.Get(Key+".description"),
                primary.BaseValues.Get(attribute),unit.EquipmentAttributes.Get(attribute),primary.TemporaryBonuses.Get(attribute),primary.rules.Curve(attribute).NextCap(unit.PrimaryValues.Get(attribute)));
        return GameText.Get("character.attribute_tooltip",GameText.Get(Key+".abbr"),GameText.Get(Key+".name"),GameText.Get(Key+".description"),
            primary.BaseValues.Get(attribute),primary.Growth(unit.growthRules!=null?Mathf.Clamp(unit.level,1,unit.growthRules.MaximumLevel):Mathf.Max(1,unit.level)).Get(attribute),
            unit.EquipmentAttributes.Get(attribute),primary.ClassBonuses.Get(attribute),primary.TemporaryBonuses.Get(attribute));
    }
    public void OnPointerEnter(PointerEventData data){if(owner!=null && owner.tooltip!=null)owner.tooltip.Show(this,data.position);}
    public void OnPointerExit(PointerEventData data){if(owner!=null && owner.tooltip!=null)owner.tooltip.Hide(this);}
}

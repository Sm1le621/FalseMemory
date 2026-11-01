using System.Collections.Generic;
using UnityEngine;

// Presentation only: item text and colors never change combat values.
public static class ItemPresentation
{
    public static Color CellColor(ItemData item,bool highlighted)
    {
        Color dark=new Color32(27,27,31,255);
        Color filled=new Color32(89,77,63,255);
        Color empty=new Color32(60,55,48,255);
        return Color.Lerp(dark,item!=null?filled:empty,highlighted?0.55f:0.25f);
    }
    public static string SlotName(EquipmentSlot slot) => slot==EquipmentSlot.Weapon?"Weapon":"Chest";
    public static string TypeName(ItemData item)
    {
        if(item==null)return "";
        switch(item.category)
        {
            case ItemCategory.Weapon:return "Weapon";
            case ItemCategory.Armor:return SlotName(item.equipmentSlot);
            default:return "Consumable";
        }
    }
    public static string BonusText(ItemData item)
    {
        if(item==null)return "";
        var parts=new List<string>();var b=item.bonuses;
        Add(parts,"ATK",b.attack);Add(parts,"DEF",b.defense);
        var consumable=item as BattleConsumable;
        if(consumable!=null)
        {
            if(consumable.healing>0)parts.Add($"Restores up to {consumable.healing} HP");
            if(consumable.manaRestoration>0)parts.Add($"Restores up to {consumable.manaRestoration} MP");
        }
        return string.Join("  •  ",parts);
    }
    private static void Add(List<string> parts,string name,int value){if(value>0)parts.Add($"{name} +{value}");}
    public static string Description(ItemData item,int count)
    {
        if(item==null)return "";
        // Assets are content, not TMP markup. Do not allow names/descriptions to alter layout.
        var lines=new List<string>{"<b>"+Escape(item.displayName)+"</b>"+(count>1?"  ×"+count:""),TypeName(item)};
        string bonuses=BonusText(item);if(bonuses.Length>0)lines.Add(bonuses);
        if(!string.IsNullOrWhiteSpace(item.description))lines.Add("\n"+Escape(item.description));
        if(item.buyPrice>0)lines.Add($"Buy: {item.buyPrice} gold");
        if(item.CanSell)lines.Add($"Sell: {item.sellPrice} gold");
        return string.Join("\n",lines);
    }
    private static string Escape(string text)=>string.IsNullOrEmpty(text)?"":text.Replace("<","‹").Replace(">","›");
}

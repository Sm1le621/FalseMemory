using System.Collections.Generic;
using UnityEngine;

// Presentation only: rarity never changes prices, damage or loot probabilities.
public static class ItemPresentation
{
    public static Color RarityColor(ItemRarity rarity)
    {
        switch(rarity)
        {
            case ItemRarity.Uncommon:return new Color32(99,157,111,255);
            case ItemRarity.Rare:return new Color32(96,164,203,255);
            case ItemRarity.Epic:return new Color32(167,119,198,255);
            case ItemRarity.Legendary:return new Color32(217,154,78,255);
            case ItemRarity.Mythic:return new Color32(185,79,87,255);
            default:return new Color32(142,145,148,255);
        }
    }
    public static Color CellColor(ItemData item,bool highlighted)
    {
        Color dark=new Color32(27,27,31,255);
        return Color.Lerp(dark,item!=null?RarityColor(item.rarity):new Color32(150,133,101,255),highlighted?0.36f:0.12f);
    }
    public static string SlotName(EquipmentSlot slot)
    {
        switch(slot)
        {
            case EquipmentSlot.Weapon:return GameText.Get("equipment.slot.weapon");case EquipmentSlot.Head:return GameText.Get("equipment.slot.head");
            case EquipmentSlot.Chest:return GameText.Get("equipment.slot.chest");case EquipmentSlot.Legs:return GameText.Get("equipment.slot.legs");
            case EquipmentSlot.Boots:return GameText.Get("equipment.slot.boots");case EquipmentSlot.Gloves:return GameText.Get("equipment.slot.gloves");
            case EquipmentSlot.Accessory1:return GameText.Get("equipment.slot.accessory1");case EquipmentSlot.Accessory2:return GameText.Get("equipment.slot.accessory2");
            default:return GameText.Get("equipment.slot.default");
        }
    }
    public static string TypeName(ItemData item)
    {
        switch(item.category)
        {
            case ItemCategory.Weapon:return GameText.Get("equipment.slot.weapon");case ItemCategory.Armor:return SlotName(item.equipmentSlot);
            case ItemCategory.Accessory:return GameText.Get("item.type.accessory");case ItemCategory.Material:return GameText.Get("item.type.material");
            case ItemCategory.QuestItem:return GameText.Get("item.type.quest");default:return GameText.Get("item.type.consumable");
        }
    }
    public static string BonusText(ItemData item)
    {
        var parts=new List<string>();var b=item.bonuses;
        Add(parts,"ATK",b.attack);Add(parts,"DEF",b.defense);Add(parts,"MATK",b.magicAttack);Add(parts,"MDEF",b.magicDefense);Add(parts,"SPD",b.speed);
        var consumable=item as BattleConsumable;
        if(consumable!=null){if(consumable.healing>0)parts.Add(GameText.Get("item.restore_hp", consumable.healing));if(consumable.manaRestoration>0)parts.Add(GameText.Get("item.restore_mp", consumable.manaRestoration));}
        return string.Join("  •  ",parts);
    }
    private static void Add(List<string> parts,string name,int value){if(value>0)parts.Add(GameText.Get("item.bonus",GameText.Get("stat."+name.ToLowerInvariant()),value));}
    public static string Description(ItemData item,int count)
    {
        if(item==null)return "";
        // Assets are content, not TMP markup. Do not allow names/descriptions to alter layout.
        var lines=new List<string>{"<b>"+Escape(item.LocalizedName)+"</b>"+(count>1?"  ×"+count:""),
            "<color=#"+ColorUtility.ToHtmlStringRGB(RarityColor(item.rarity))+">"+GameText.Get("rarity."+item.rarity.ToString().ToLowerInvariant())+"</color>  •  "+TypeName(item)};
        string bonuses=BonusText(item);if(bonuses.Length>0)lines.Add(bonuses);
        if(!string.IsNullOrWhiteSpace(item.LocalizedDescription))lines.Add("\n"+Escape(item.LocalizedDescription));
        lines.Add("\n"+GameText.Get("item.level", Mathf.Max(1,item.requiredLevel)));
        if(item.buyPrice>0)lines.Add(GameText.Get("item.buy_price", item.buyPrice));
        if(item.CanSell)lines.Add(GameText.Get("item.sell_price", item.sellPrice));
        return string.Join("\n",lines);
    }
    private static string Escape(string text)=>string.IsNullOrEmpty(text)?"":text.Replace("<","‹").Replace(">","›");
}

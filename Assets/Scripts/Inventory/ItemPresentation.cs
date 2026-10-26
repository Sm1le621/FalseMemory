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
            case EquipmentSlot.OffHand:return GameText.Get("equipment.slot.offhand");
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
        for(int i=0;i<CharacterAttributeValues.Count;i++){var attribute=(CharacterAttribute)i;int value=b.attributes.Get(attribute);if(value!=0)parts.Add(GameText.Get(value>0?"item.bonus":"character.signed_bonus",GameText.Get("attribute."+attribute.ToString().ToLowerInvariant()+".abbr"),value));}
        Add(parts,"HP",b.maximumHealth);Add(parts,"MP",b.maximumMana);
        AddFloat(parts,"move",b.movementSpeed,false);AddFloat(parts,"critical",b.criticalChance,true);AddFloat(parts,"evasion",b.evasion,true);
        var consumable=item as BattleConsumable;
        if(consumable!=null){if(consumable.healing>0)parts.Add(GameText.Get("item.restore_hp", consumable.healing));if(consumable.manaRestoration>0)parts.Add(GameText.Get("item.restore_mp", consumable.manaRestoration));}
        return string.Join("  •  ",parts);
    }
    private static void Add(List<string> parts,string name,int value){if(value>0)parts.Add(GameText.Get("item.bonus",GameText.Get("stat."+name.ToLowerInvariant()),value));}
    private static void AddFloat(List<string> parts,string key,float value,bool percentage)
    {
        if(value==0 || float.IsNaN(value) || float.IsInfinity(value))return;
        parts.Add(GameText.Get(percentage?"item.percent_bonus":"item.float_bonus",GameText.Get("stat."+key),percentage?value*100:value));
    }
    public static string Description(ItemData item,int count,BattleUnit viewer=null)
    {
        if(item==null)return "";
        // Assets are content, not TMP markup. Do not allow names/descriptions to alter layout.
        var lines=new List<string>{"<b>"+Escape(item.LocalizedName)+"</b>"+(count>1?"  ×"+count:""),
            "<color=#"+ColorUtility.ToHtmlStringRGB(RarityColor(item.rarity))+">"+GameText.Get("rarity."+item.rarity.ToString().ToLowerInvariant())+"</color>  •  "+TypeName(item)};
        string bonuses=BonusText(item);if(bonuses.Length>0)lines.Add(bonuses);
        if(item.HasWeaponScaling || item.HasArmorScaling)lines.Add(ScalingText(item,viewer));
        if(!string.IsNullOrWhiteSpace(item.LocalizedDescription))lines.Add("\n"+Escape(item.LocalizedDescription));
        if(item.category==ItemCategory.Weapon && item.twoHanded)lines.Add(GameText.Get("equipment.two_handed"));
        lines.Add("\n"+GameText.Get("item.level", Mathf.Max(1,item.requiredLevel)));
        if(item.buyPrice>0)lines.Add(GameText.Get("item.buy_price", item.buyPrice));
        if(item.CanSell)lines.Add(GameText.Get("item.sell_price", item.sellPrice));
        return string.Join("\n",lines);
    }
    public static string ScalingText(ItemData item,BattleUnit viewer)
    {
        if(item==null || (!item.HasWeaponScaling && !item.HasArmorScaling))return "";
        if(item.HasArmorScaling)return ArmorText(item,viewer);
        var weapon=item.weapon;var lines=new List<string>();
        var values=viewer!=null && viewer.UsesPrimaryAttributes?
            viewer.Attributes.Calculate(viewer.level,(viewer.Equipment!=null?viewer.Equipment.State.AttributesExcluding(EquipmentSlot.Weapon):new CharacterAttributeValues())+item.bonuses.attributes):new CharacterAttributeValues();
        int total=weapon.Damage(values,viewer!=null && viewer.UsesPrimaryAttributes?viewer.Attributes.rules:null);
        lines.Add(GameText.Get("scaling.damage",GameText.Get("scaling.type."+weapon.damageType.ToString().ToLowerInvariant()),Mathf.Max(0,weapon.baseDamage),total-Mathf.Max(0,weapon.baseDamage),total));
        if(weapon.holyBaseDamage>0)
        {
            int holy=weapon.HolyDamage(values,viewer!=null && viewer.UsesPrimaryAttributes?viewer.Attributes.rules:null);
            lines.Add(GameText.Get("scaling.holy_damage",weapon.holyBaseDamage,holy-weapon.holyBaseDamage,holy));
        }
        lines.Add(GameText.Get("scaling.weapon_type",GameText.Get("scaling.weapon."+weapon.weaponType)));
        var grades=new List<string>();
        foreach(var entry in weapon.scaling ?? System.Array.Empty<AttributeScalingEntry>())
            if(entry.grade!=ScalingGrade.None)grades.Add(GameText.Get("attribute."+entry.attribute.ToString().ToLowerInvariant()+".abbr")+": "+entry.grade);
        lines.Add(GameText.Get("scaling.grades",grades.Count>0?string.Join(" / ",grades):GameText.Get("scaling.none")));
        lines.Add(GameText.Get("scaling.requires",RequirementsText(item,viewer)));
        return string.Join("\n",lines);
    }
    public static string RequirementsText(ItemData item,BattleUnit viewer)
    {
        if(item==null || (!item.HasWeaponScaling && !item.HasArmorScaling))return GameText.Get("scaling.none");
        var requirementsValues=item.HasWeaponScaling?item.weapon.requirements:item.armor.requirements;
        var slot=item.HasWeaponScaling?EquipmentSlot.Weapon:item.equipmentSlot;
        var available=viewer!=null && viewer.Attributes!=null?viewer.Attributes.Calculate(viewer.level,viewer.Equipment!=null?viewer.Equipment.State.AttributesExcluding(slot):new CharacterAttributeValues()):new CharacterAttributeValues();
        var requirements=new List<string>();
        for(int i=0;i<CharacterAttributeValues.Count;i++)
        {
            var attribute=(CharacterAttribute)i;int required=requirementsValues.Get(attribute);
            if(required>0)requirements.Add(GameText.Get("scaling.requirement",GameText.Get("attribute."+attribute.ToString().ToLowerInvariant()+".abbr"),required,available.Get(attribute)));
        }
        return requirements.Count>0?string.Join(" / ",requirements):GameText.Get("scaling.none");
    }
    private static string ArmorText(ItemData item,BattleUnit viewer)
    {
        var values=viewer!=null && viewer.UsesPrimaryAttributes?viewer.Attributes.Calculate(viewer.level,
            (viewer.Equipment!=null?viewer.Equipment.State.AttributesExcluding(item.equipmentSlot):new CharacterAttributeValues())+item.bonuses.attributes):new CharacterAttributeValues();
        var bonus=item.armor.Bonuses(values,viewer!=null && viewer.UsesPrimaryAttributes?viewer.Attributes.rules:null);
        var grades=new List<string>();
        foreach(var entry in item.armor.physicalScaling ?? System.Array.Empty<AttributeScalingEntry>())
            if(entry.grade!=ScalingGrade.None)grades.Add(GameText.Get("attribute."+entry.attribute.ToString().ToLowerInvariant()+".abbr")+": "+entry.grade);
        foreach(var entry in item.armor.magicScaling ?? System.Array.Empty<AttributeScalingEntry>())
            if(entry.grade!=ScalingGrade.None)grades.Add(GameText.Get("attribute."+entry.attribute.ToString().ToLowerInvariant()+".abbr")+": "+entry.grade);
        foreach(var entry in item.armor.evasionScaling ?? System.Array.Empty<AttributeScalingEntry>())
            if(entry.grade!=ScalingGrade.None)grades.Add(GameText.Get("attribute."+entry.attribute.ToString().ToLowerInvariant()+".abbr")+": "+entry.grade);
        return GameText.Get("armor.defense",item.armor.physicalDefense,bonus.defense,item.armor.magicDefense,bonus.magicDefense,bonus.evasion*100)+"\n"+
            GameText.Get("scaling.grades",grades.Count>0?string.Join(" / ",grades):GameText.Get("scaling.none"))+"\n"+GameText.Get("scaling.requires",RequirementsText(item,viewer));
    }
    private static string Escape(string text)=>string.IsNullOrEmpty(text)?"":text.Replace("<","‹").Replace(">","›");
}

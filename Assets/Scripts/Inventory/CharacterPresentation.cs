using System;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterPresentation
{
    public static string AttributeName(CharacterAttribute attribute)=>GameText.Get("attribute."+attribute.ToString().ToLowerInvariant()+".name");
    public static string AttributeDescription(BattleUnit unit,CharacterAttribute attribute)
    {
        if(unit==null || !unit.UsesPrimaryAttributes)return "";
        var data=unit.Attributes;var rules=data.rules;var values=unit.PrimaryValues;var next=values;
        next.Set(attribute,CharacterAttributeValues.Clamp((long)values.Get(attribute)+1));
        string key="attribute."+attribute.ToString().ToLowerInvariant();var effects=new List<string>();
        foreach(DerivedAttributeStat stat in Enum.GetValues(typeof(DerivedAttributeStat)))
        {
            if(stat==DerivedAttributeStat.Attack && unit.UsesWeaponScaling && !unit.MagicalWeaponAttack ||
               stat==DerivedAttributeStat.MagicAttack && unit.MagicalWeaponAttack)continue;
            int delta=rules.Adjustment(next,stat)-rules.Adjustment(values,stat);
            if(delta!=0)effects.Add(GameText.Get("character.next_delta",GameText.Get("derived."+stat.ToString().ToLowerInvariant()),delta));
        }
        if(unit.UsesWeaponScaling)
        {
            int delta=unit.EquippedWeapon.Damage(next,rules)-unit.EquippedWeapon.Damage(values,rules);
            if(delta!=0)effects.Add(GameText.Get("character.next_delta",GameText.Get("character.weapon_power"),delta));
        }
        if(unit.Equipment!=null)
        {
            var before=unit.Equipment.State.ScaledBonuses(values,rules);var after=unit.Equipment.State.ScaledBonuses(next,rules);
            if(after.defense!=before.defense)effects.Add(GameText.Get("character.next_delta",GameText.Get("derived.defense"),after.defense-before.defense));
            if(after.magicDefense!=before.magicDefense)effects.Add(GameText.Get("character.next_delta",GameText.Get("derived.magicdefense"),after.magicDefense-before.magicDefense));
            Percent(effects,"character.evasion",after.evasion-before.evasion);
        }
        Percent(effects,"character.critical",rules.CriticalBonus(next)-rules.CriticalBonus(values));
        Percent(effects,"character.evasion",rules.Evasion(next)-rules.Evasion(values));
        Percent(effects,"character.accuracy",rules.AccuracyBonus(next)-rules.AccuracyBonus(values));
        Percent(effects,"character.holy",rules.HolyBonus(next)-rules.HolyBonus(values));
        foreach(var spell in unit.KnownAbilities)
        {
            int cost=rules.ManaCost(spell.mpCost,next)-unit.SpellCost(spell);
            if(cost!=0)effects.Add(GameText.Get("character.next_mp",spell.LocalizedName,cost));
        }
        int heal=rules.HealingBonus(next)-rules.HealingBonus(values);
        if(heal!=0)effects.Add(GameText.Get("character.next_delta",GameText.Get("character.healing"),heal));
        string effect=data.BaseValues.Get(attribute)>=99?GameText.Get("character.base_cap"):
            effects.Count==0?GameText.Get("character.no_immediate_change"):string.Join("\n",effects);
        return GameText.Get("character.attribute_detail",AttributeName(attribute),GameText.Get(key+".description"),data.BaseValues.Get(attribute),
            unit.EquipmentAttributes.Get(attribute),data.TemporaryBonuses.Get(attribute),values.Get(attribute),rules.Curve(attribute).NextCap(values.Get(attribute)),effect);
    }
    private static void Percent(List<string> lines,string key,double delta)
    {if(Math.Abs(delta)>.000001)lines.Add(GameText.Get("character.next_percent",GameText.Get(key),delta*100));}
    public static string CombatStats(BattleUnit unit)=>GameText.Get("character.combat_values",unit.EffectiveAttack,unit.EffectiveMagicAttack,
        unit.EffectiveDefense,unit.EffectiveMagicDefense,unit.EffectiveCriticalChance*100,unit.Evasion*100,unit.Speed,unit.HolyBonus*100,
        unit.EffectiveMissChance*100);
    public static string AbilityState(AbilityAvailability state)=>GameText.Get(state==AbilityAvailability.Learned?"ability.state.learned":
        state==AbilityAvailability.RequirementsNotMet?"ability.state.requirements":state==AbilityAvailability.NotEnoughPoints?"ability.state.points":"ability.state.available");
    public static string AbilityDescription(BattleUnit unit,BattleSpell spell)
    {
        if(spell==null)return "";var requirements=new List<string>();
        for(int i=0;i<CharacterAttributeValues.Count;i++)
        {
            var stat=(CharacterAttribute)i;int minimum=spell.requirements.Get(stat);
            if(minimum>0)requirements.Add(GameText.Get("scaling.requirement",GameText.Get("attribute."+stat.ToString().ToLowerInvariant()+".abbr"),minimum,unit.PrimaryValues.Get(stat)));
        }
        return GameText.Get("character.ability_detail",spell.LocalizedName,spell.LocalizedDescription,unit.SpellCost(spell),
            GameText.Get("ability.requirements",spell.requiredLevel,requirements.Count>0?string.Join(" / ",requirements):GameText.Get("scaling.none")),
            AbilityState(unit.Learning.Availability(spell)),GameText.Get("scaling.type."+spell.damageType.ToString().ToLowerInvariant()));
    }
}

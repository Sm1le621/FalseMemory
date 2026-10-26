// Data-driven effects: adding a spell with an existing effect needs no manager changes.
public static class BattleAbilities
{
    public static bool CanUseItem(BattleUnit user, BattleUnit target, BattleConsumable item, ItemUseLocation location=ItemUseLocation.Battle)
    {
        return user != null && target != null && item != null && user.IsAlive && target.IsAlive &&
               item.category==ItemCategory.Consumable && user.level>=System.Math.Max(1,item.requiredLevel) &&
               (location==ItemUseLocation.Exploration || location==ItemUseLocation.Battle) && (item.useLocations & location)==location &&
               item.healing >= 0 && item.manaRestoration>=0 && (item.healing>0 || item.manaRestoration>0) &&
               !(item.healing>0 && item.manaRestoration>0) && (item.target == BattleTarget.Self ? user == target : user != target) &&
               user.Inventory != null && user.Inventory.Count(item) > 0;
    }

    public static bool TryUseItem(BattleUnit user, BattleUnit target, BattleConsumable item, out int amount, ItemUseLocation location=ItemUseLocation.Battle)
    {
        amount = 0;
        if (!CanUseItem(user, target, item,location) || !user.Inventory.TryConsume(item)) return false;
        amount = item.manaRestoration>0 ? target.RestoreMP(item.manaRestoration) : target.Health.Heal(item.healing);
        return true;
    }

    public static bool CanCast(BattleUnit caster, BattleUnit target, BattleSpell spell)
    {
        return caster != null && target != null && spell != null && caster.IsAlive && target.IsAlive &&
               (caster.Learning==null || caster.Learning.Knows(spell)) &&
               spell.mpCost >= 0 && caster.SpellCost(spell)>=0 && caster.CurrentMP >= caster.SpellCost(spell) &&
               (spell.target == BattleTarget.Self ? caster == target : caster != target) &&
               (spell.effect == BattleEffect.MagicDamage || spell.effect == BattleEffect.RestoreHealth || spell.effect==BattleEffect.PhysicalDamage);
    }

    public static bool TryCast(BattleUnit caster, BattleUnit target, BattleSpell spell, BattleRules rules, out int amount,IBattleRandom random=null)
    {
        amount = 0;
        if (rules == null || !CanCast(caster, target, spell) || !caster.TrySpendMP(caster.SpellCost(spell))) return false;
        caster.LastAbilityMissed=caster.LastAbilityCritical=false;
        if (spell.effect == BattleEffect.RestoreHealth) amount = target.Health.Heal(caster.SpellHealing(spell));
        else
        {
            if(spell.effect==BattleEffect.PhysicalDamage)
            {
                double multiplier=float.IsNaN(spell.attackMultiplier)||float.IsInfinity(spell.attackMultiplier)?1:System.Math.Max(0,spell.attackMultiplier);
                int power=(int)System.Math.Min(int.MaxValue,System.Math.Round(((double)caster.EffectiveAttack+spell.ScaledPower(caster))*multiplier,System.MidpointRounding.AwayFromZero));
                var result=BattleCalculator.ResolvePhysical(power,spell.damageType==WeaponDamageType.Holy?0:target.EffectiveDefense,target.IsDefending,rules,
                    caster.EffectiveCriticalChance,UnityEngine.Mathf.Clamp01(caster.EffectiveMissChance+target.Evasion-spell.accuracyBonus),random??UnityBattleRandom.Instance);
                caster.LastAbilityMissed=result.Missed;caster.LastAbilityCritical=result.Critical;amount=result.Damage;
            }
            else amount = BattleCalculator.MagicalDamage(caster.EffectiveMagicAttack,spell.damageType==WeaponDamageType.Holy?0:target.EffectiveMagicDefense, spell.ScaledPower(caster), rules);
            amount=caster.DealDamage(target,amount,caster.LastAbilityMissed?0:spell.holyPower,spell.damageType==WeaponDamageType.Holy).Total;
        }
        return true;
    }
}

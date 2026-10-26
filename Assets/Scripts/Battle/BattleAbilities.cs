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
               spell.mpCost >= 0 && caster.SpellCost(spell)>=0 && caster.CurrentMP >= caster.SpellCost(spell) &&
               (spell.target == BattleTarget.Self ? caster == target : caster != target) &&
               (spell.effect == BattleEffect.MagicDamage || spell.effect == BattleEffect.RestoreHealth);
    }

    public static bool TryCast(BattleUnit caster, BattleUnit target, BattleSpell spell, BattleRules rules, out int amount)
    {
        amount = 0;
        if (rules == null || !CanCast(caster, target, spell) || !caster.TrySpendMP(caster.SpellCost(spell))) return false;
        if (spell.effect == BattleEffect.RestoreHealth) amount = target.Health.Heal(caster.SpellHealing(spell));
        else
        {
            amount = BattleCalculator.MagicalDamage(caster.EffectiveMagicAttack, target.EffectiveMagicDefense, spell.power, rules);
            amount=caster.DealDamage(target,amount).Total;
        }
        return true;
    }
}

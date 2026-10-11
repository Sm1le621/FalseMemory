// Data-driven effects: adding a spell with an existing effect needs no manager changes.
public static class BattleAbilities
{
    public static bool CanUseItem(BattleUnit user, BattleUnit target, BattleConsumable item)
    {
        return user != null && target != null && item != null && user.IsAlive && target.IsAlive &&
               item.healing >= 0 && (item.target == BattleTarget.Self ? user == target : user != target) &&
               user.Inventory != null && user.Inventory.Count(item) > 0;
    }

    public static bool TryUseItem(BattleUnit user, BattleUnit target, BattleConsumable item, out int amount)
    {
        amount = 0;
        if (!CanUseItem(user, target, item) || !user.Inventory.TryConsume(item)) return false;
        amount = target.Health.Heal(item.healing);
        return true;
    }

    public static bool CanCast(BattleUnit caster, BattleUnit target, BattleSpell spell)
    {
        return caster != null && target != null && spell != null && caster.IsAlive && target.IsAlive &&
               spell.mpCost >= 0 && caster.CurrentMP >= spell.mpCost &&
               (spell.target == BattleTarget.Self ? caster == target : caster != target) &&
               (spell.effect == BattleEffect.MagicDamage || spell.effect == BattleEffect.RestoreHealth);
    }

    public static bool TryCast(BattleUnit caster, BattleUnit target, BattleSpell spell, BattleRules rules, out int amount)
    {
        amount = 0;
        if (rules == null || !CanCast(caster, target, spell) || !caster.TrySpendMP(spell.mpCost)) return false;
        if (spell.effect == BattleEffect.RestoreHealth) amount = target.Health.Heal(spell.power);
        else
        {
            amount = BattleCalculator.MagicalDamage(caster.magicAttack, target.magicDefense, spell.power, rules);
            target.Health.TakeDamage(amount);
        }
        return true;
    }
}

using System;

public static class BattleCalculator
{
    // A successful physical attack always does at least minimumDamage.
    // Defend halves post-defense damage; rounding is explicitly upwards.
    public static int PhysicalDamage(int attack, int defense, bool defending, BattleRules rules)
    {
        if (rules == null) throw new ArgumentNullException(nameof(rules));
        double raw = Math.Max(0, attack) * Math.Max(0f, rules.attackMultiplier)
                   - Math.Max(0, defense) * Math.Max(0f, rules.defenseMultiplier);
        double damage = Math.Max(Math.Max(1, rules.minimumDamage), raw);
        if (defending) damage *= Math.Max(0f, Math.Min(1f, rules.defendMultiplier));
        return (int)Math.Min(int.MaxValue, Math.Max(1, Math.Ceiling(damage)));
    }
}

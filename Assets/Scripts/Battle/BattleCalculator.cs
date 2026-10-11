using System;

public static class BattleCalculator
{
    public static float FleeChance(int playerSpeed, int enemySpeed, BattleRules rules)
    {
        if (rules == null) throw new ArgumentNullException(nameof(rules));
        double minimum = Math.Max(0f, Math.Min(1f, rules.minimumFleeChance));
        double maximum = Math.Max(minimum, Math.Max(0f, Math.Min(1f, rules.maximumFleeChance)));
        double difference = (double)Math.Max(0, playerSpeed) - Math.Max(0, enemySpeed);
        double chance = rules.fleeBaseChance + difference * Math.Max(0f, rules.fleeSpeedCoefficient);
        return (float)Math.Max(minimum, Math.Min(maximum, chance));
    }

    public static bool ChanceSucceeds(float chance, float sample)
    {
        chance = Math.Max(0f, Math.Min(1f, chance));
        sample = Math.Max(0f, Math.Min(1f, sample));
        return chance >= 1f || sample < chance;
    }

    public static PhysicalAttackResult ResolvePhysical(int attack, int defense, bool defending, BattleRules rules,
        float criticalChance, float missChance, IBattleRandom random)
    {
        if (rules == null) throw new ArgumentNullException(nameof(rules));
        if (random == null) throw new ArgumentNullException(nameof(random));
        // Do not even sample critical chance after a miss.
        if (ChanceSucceeds(missChance, random.Next01())) return new PhysicalAttackResult(0, true, false);
        bool critical = ChanceSucceeds(criticalChance, random.Next01());
        int damage = PhysicalDamage(attack, defense, defending, rules, critical ? Math.Max(1f, rules.criticalMultiplier) : 1f);
        return new PhysicalAttackResult(damage, false, critical);
    }
    // Fire bypasses physical Defend. Spell power and coefficients are data.
    public static int MagicalDamage(int attack, int defense, int power, BattleRules rules)
    {
        if (rules == null) throw new ArgumentNullException(nameof(rules));
        double raw = Math.Max(0, power) + Math.Max(0, attack) * Math.Max(0f, rules.magicAttackMultiplier)
                   - Math.Max(0, defense) * Math.Max(0f, rules.magicDefenseMultiplier);
        return (int)Math.Min(int.MaxValue, Math.Ceiling(Math.Max(Math.Max(1, rules.minimumMagicDamage), raw)));
    }

    // A successful physical attack always does at least minimumDamage.
    // Defend halves post-defense damage; rounding is explicitly upwards.
    public static int PhysicalDamage(int attack, int defense, bool defending, BattleRules rules)
        => PhysicalDamage(attack, defense, defending, rules, 1f);

    private static int PhysicalDamage(int attack, int defense, bool defending, BattleRules rules, float criticalMultiplier)
    {
        if (rules == null) throw new ArgumentNullException(nameof(rules));
        double raw = Math.Max(0, attack) * Math.Max(0f, rules.attackMultiplier)
                   - Math.Max(0, defense) * Math.Max(0f, rules.defenseMultiplier);
        double damage = Math.Max(Math.Max(1, rules.minimumDamage), raw);
        damage *= criticalMultiplier;
        if (defending) damage *= Math.Max(0f, Math.Min(1f, rules.defendMultiplier));
        return (int)Math.Min(int.MaxValue, Math.Max(1, Math.Ceiling(damage)));
    }
}

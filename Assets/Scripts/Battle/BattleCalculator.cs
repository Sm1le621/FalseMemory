using System;

// Всі формули бою: шанс втечі, фізична і магічна шкода.
// Тут немає Unity-об'єктів, тому ці формули легко тестувати.
public static class BattleCalculator
{
    // Шанс втекти з бою (від 0 до 1)
    public static float FleeChance(int playerSpeed, int enemySpeed, BattleRules rules)
    {
        if (rules == null)
        {
            throw new ArgumentNullException(nameof(rules));
        }

        // Мінімум і максимум обрізаємо в межі від 0 до 1
        double minimum = Clamp01(rules.minimumFleeChance);
        double maximum = Clamp01(rules.maximumFleeChance);
        // Максимум не може бути меншим за мінімум
        if (maximum < minimum)
        {
            maximum = minimum;
        }

        // Швидкості не можуть бути від'ємними
        int safePlayerSpeed = Math.Max(0, playerSpeed);
        int safeEnemySpeed = Math.Max(0, enemySpeed);
        double difference = (double)safePlayerSpeed - safeEnemySpeed;

        // Базовий шанс + різниця швидкостей * коефіцієнт
        double coefficient = Math.Max(0f, rules.fleeSpeedCoefficient);
        double chance = rules.fleeBaseChance + difference * coefficient;

        // Обрізаємо результат між мінімумом і максимумом
        if (chance > maximum)
        {
            chance = maximum;
        }
        if (chance < minimum)
        {
            chance = minimum;
        }
        return (float)chance;
    }

    // Чи спрацював шанс. sample - випадкове число від 0 до 1
    public static bool ChanceSucceeds(float chance, float sample)
    {
        chance = Clamp01(chance);
        sample = Clamp01(sample);

        // Шанс 100% спрацьовує завжди
        if (chance >= 1f)
        {
            return true;
        }
        return sample < chance;
    }

    // Повний розрахунок фізичної атаки: промах -> крит -> шкода
    public static PhysicalAttackResult ResolvePhysical(int attack, int defense, bool defending, BattleRules rules,
        float criticalChance, float missChance, IBattleRandom random)
    {
        if (rules == null)
        {
            throw new ArgumentNullException(nameof(rules));
        }
        if (random == null)
        {
            throw new ArgumentNullException(nameof(random));
        }

        // Спочатку перевіряємо промах.
        // Якщо промах - шанс криту навіть не рахуємо (не беремо зайве випадкове число)
        if (ChanceSucceeds(missChance, random.Next01()))
        {
            return new PhysicalAttackResult(0, true, false);
        }

        bool critical = ChanceSucceeds(criticalChance, random.Next01());

        float multiplier = 1f;
        if (critical)
        {
            // Множник криту не може бути меншим за 1
            multiplier = Math.Max(1f, rules.criticalMultiplier);
        }

        int damage = PhysicalDamage(attack, defense, defending, rules, multiplier);
        return new PhysicalAttackResult(damage, false, critical);
    }

    // Магічна шкода (наприклад, Fire).
    // Захист (Defend) на магію НЕ діє. Сила і коефіцієнти беруться з даних.
    public static int MagicalDamage(int attack, int defense, int power, BattleRules rules)
    {
        if (rules == null)
        {
            throw new ArgumentNullException(nameof(rules));
        }

        // Важливо: рахуємо саме у float (як і було в оригіналі), щоб округлення не змінилось
        int spellPower = Math.Max(0, power);
        float attackPart = Math.Max(0, attack) * Math.Max(0f, rules.magicAttackMultiplier);
        float defensePart = Math.Max(0, defense) * Math.Max(0f, rules.magicDefenseMultiplier);
        float raw = spellPower + attackPart - defensePart;

        // Шкода не може бути меншою за мінімальну (і мінімальна - хоча б 1)
        double minimum = Math.Max(1, rules.minimumMagicDamage);
        double damage = Math.Max(minimum, raw);

        // Округлюємо вгору і не даємо вийти за межі int
        return (int)Math.Min(int.MaxValue, Math.Ceiling(damage));
    }

    // Фізична шкода без криту.
    // Успішна атака завжди наносить хоча б minimumDamage.
    // Defend зменшує шкоду (за замовчуванням вдвічі), округлення - вгору.
    public static int PhysicalDamage(int attack, int defense, bool defending, BattleRules rules)
    {
        return PhysicalDamage(attack, defense, defending, rules, 1f);
    }

    // Фізична шкода з множником криту
    private static int PhysicalDamage(int attack, int defense, bool defending, BattleRules rules, float criticalMultiplier)
    {
        if (rules == null)
        {
            throw new ArgumentNullException(nameof(rules));
        }

        // Рахуємо у float (як і було в оригіналі), щоб округлення не змінилось
        float attackPart = Math.Max(0, attack) * Math.Max(0f, rules.attackMultiplier);
        float defensePart = Math.Max(0, defense) * Math.Max(0f, rules.defenseMultiplier);
        float raw = attackPart - defensePart;

        double minimum = Math.Max(1, rules.minimumDamage);
        double damage = Math.Max(minimum, raw);

        // Спочатку крит
        damage = damage * criticalMultiplier;

        // Потім захист
        if (defending)
        {
            damage = damage * Clamp01(rules.defendMultiplier);
        }

        // Округлюємо вгору, мінімум 1, і не більше за int.MaxValue
        double rounded = Math.Max(1, Math.Ceiling(damage));
        return (int)Math.Min(int.MaxValue, rounded);
    }

    // Обрізає число в межі від 0 до 1
    private static float Clamp01(float value)
    {
        return Math.Max(0f, Math.Min(1f, value));
    }
}

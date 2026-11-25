using System;

// Всі правила бою в одному місці: шанси, фізична і магічна шкода,
// вибір дії ворога, заклинання і використання зілля.
// Тут немає Unity-об'єктів на сцені, тому ці правила легко тестувати.
public static class BattleCalculator
{
    // ===== Шанси і шкода =====

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
        float criticalChance, float missChance, BattleRandom random)
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

    // ===== Дія ворога =====

    // "Мозок" ворога: з шансом enemyDefendChance захищається, інакше атакує
    public static BattleAction ChooseEnemyAction(BattleRules rules, BattleRandom random)
    {
        bool defend = ChanceSucceeds(rules.enemyDefendChance, random.Next01());

        if (defend)
        {
            return BattleAction.Defend;
        }
        return BattleAction.Attack;
    }

    // ===== Заклинання =====

    // Чи може caster скастувати spell на target (заклинання б'ють тільки противника)
    public static bool CanCast(BattleUnit caster, BattleUnit target, BattleSpell spell)
    {
        if (caster == null || target == null || spell == null)
        {
            return false;
        }

        if (!caster.IsAlive || !target.IsAlive)
        {
            return false;
        }

        // Вартість не може бути від'ємною, і мани має вистачати
        if (spell.mpCost < 0)
        {
            return false;
        }
        if (caster.CurrentMP < spell.mpCost)
        {
            return false;
        }

        // Не можна вдарити самого себе
        if (caster == target)
        {
            return false;
        }

        return true;
    }

    // Спробувати скастувати заклинання. amount - скільки шкоди нанесено
    public static bool TryCast(BattleUnit caster, BattleUnit target, BattleSpell spell, BattleRules rules, out int amount)
    {
        amount = 0;

        if (rules == null)
        {
            return false;
        }
        if (!CanCast(caster, target, spell))
        {
            return false;
        }
        // Знімаємо ману
        if (!caster.TrySpendMP(spell.mpCost))
        {
            return false;
        }

        amount = MagicalDamage(caster.EffectiveMagicAttack, target.EffectiveMagicDefense, spell.power, rules);
        target.Health.TakeDamage(amount);
        return true;
    }

    // ===== Зілля (використовуються з інвентаря на себе) =====

    // Чи може user зараз використати предмет item на себе
    public static bool CanUseItem(BattleUnit user, BattleConsumable item)
    {
        // 1) Всі об'єкти мають існувати, гравець має бути живий
        if (user == null || item == null)
        {
            return false;
        }
        if (!user.IsAlive)
        {
            return false;
        }

        // 2) Це має бути саме витратний предмет
        if (item.category != ItemCategory.Consumable)
        {
            return false;
        }

        // 3) Значення не можуть бути від'ємними
        if (item.healing < 0 || item.manaRestoration < 0)
        {
            return false;
        }

        // 4) Предмет має щось робити: або лікувати, або відновлювати ману
        if (item.healing <= 0 && item.manaRestoration <= 0)
        {
            return false;
        }

        // 5) Але НЕ обидва одразу (такий "гібрид" вважаємо помилкою)
        if (item.healing > 0 && item.manaRestoration > 0)
        {
            return false;
        }

        // 6) Предмет має бути в інвентарі
        if (user.Inventory == null)
        {
            return false;
        }
        if (user.Inventory.Count(item) <= 0)
        {
            return false;
        }

        return true;
    }

    // Спробувати використати предмет на себе. amount - скільки HP/MP відновилось
    public static bool TryUseItem(BattleUnit user, BattleConsumable item, out int amount)
    {
        amount = 0;

        if (!CanUseItem(user, item))
        {
            return false;
        }

        // Забираємо 1 штуку з інвентаря
        if (!user.Inventory.TryConsume(item))
        {
            return false;
        }

        if (item.manaRestoration > 0)
        {
            amount = user.RestoreMP(item.manaRestoration);
        }
        else
        {
            amount = user.Health.Heal(item.healing);
        }
        return true;
    }

    // Обрізає число в межі від 0 до 1
    private static float Clamp01(float value)
    {
        return Math.Max(0f, Math.Min(1f, value));
    }
}

// Які дії можна вибрати в бою
public enum BattleAction { Attack, Defend, Magic, Flee }

// Стан бою
public enum BattleState { Starting, Resolving, AwaitingPlayer, Victory, Defeat, Escaped }

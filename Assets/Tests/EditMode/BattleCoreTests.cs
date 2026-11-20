using NUnit.Framework;
using UnityEngine;

// Тести основи бою: формула шкоди, черга ходів, "ворота" команд, захист
public sealed class BattleCoreTests
{
    private BattleRules rules;

    [SetUp]
    public void Setup()
    {
        rules = ScriptableObject.CreateInstance<BattleRules>();
    }

    [TearDown]
    public void Cleanup()
    {
        Object.DestroyImmediate(rules);
    }

    // Кожен TestCase - окремий запуск тесту з іншими числами:
    // атака, захист, чи захищається ціль, очікувана шкода
    [TestCase(15, 5, false, 10)]
    [TestCase(12, 8, false, 4)]
    [TestCase(12, 8, true, 2)]
    [TestCase(15, 8, true, 4)]
    [TestCase(0, 1000, false, 1)]
    [TestCase(-10, -100, true, 1)]
    public void PhysicalDamageIsNonNegativeAndDefenseIsApplied(int attack, int defense, bool guarding, int expected)
    {
        int damage = BattleCalculator.PhysicalDamage(attack, defense, guarding, rules);
        Assert.That(damage, Is.EqualTo(expected));
    }

    // Множники з BattleRules реально впливають на шкоду
    [Test]
    public void DamageMultipliersAreConfigurable()
    {
        rules.attackMultiplier = 2f;
        rules.defenseMultiplier = 0.5f;
        // 15 * 2 - 8 * 0.5 = 26
        Assert.That(BattleCalculator.PhysicalDamage(15, 8, false, rules), Is.EqualTo(26));
    }

    // Простий учасник для тестів черги ходів (без Unity)
    private sealed class Participant : IBattleParticipant
    {
        public bool IsAlive { get; set; } = true;
    }

    // Черга зберігає порядок, у якому передали учасників, і кожен ходить один раз
    [Test]
    public void RoundKeepsGivenOrderWithOneTurnPerUnit()
    {
        Participant a = new Participant();
        Participant b = new Participant();
        Participant c = new Participant();
        BattleTurnOrder<Participant> queue = new BattleTurnOrder<Participant>();

        // b передаємо двічі - він все одно має ходити один раз
        queue.BeginRound(new Participant[] { a, b, c, b });

        Participant unit;
        Assert.That(queue.TryNext(out unit), Is.True);
        Assert.That(unit, Is.SameAs(a));
        Assert.That(queue.TryNext(out unit), Is.True);
        Assert.That(unit, Is.SameAs(b));
        Assert.That(queue.TryNext(out unit), Is.True);
        Assert.That(unit, Is.SameAs(c));
        Assert.That(queue.TryNext(out unit), Is.False);
    }

    // Мертві пропускаються, а в новому раунді першим знову ходить перший у списку
    [Test]
    public void KilledUnitsAreSkippedAndFirstUnitStartsEveryRound()
    {
        Participant player = new Participant();
        Participant enemy = new Participant();
        BattleTurnOrder<Participant> queue = new BattleTurnOrder<Participant>();
        queue.BeginRound(new Participant[] { player, enemy });

        Participant unit;
        queue.TryNext(out unit);
        Assert.That(unit, Is.SameAs(player));

        enemy.IsAlive = false;
        Assert.That(queue.TryNext(out unit), Is.False);

        enemy.IsAlive = true;
        queue.BeginRound(new Participant[] { player, enemy });
        queue.TryNext(out unit);
        Assert.That(unit, Is.SameAs(player));
    }

    // Одну команду можна прийняти тільки один раз
    [Test]
    public void DuplicateCommandsAreRejectedUntilNextTurn()
    {
        BattleCommandGate gate = new BattleCommandGate();
        Assert.That(gate.TryConsume(), Is.False);

        gate.Open();
        Assert.That(gate.TryConsume(), Is.True);
        Assert.That(gate.TryConsume(), Is.False);

        gate.Open();
        Assert.That(gate.TryConsume(), Is.True);

        gate.Close();
        Assert.That(gate.TryConsume(), Is.False);
    }

    // Захист тримається до свого наступного ходу, а HP - спільне з Health
    [Test]
    public void DefendPersistsUntilOwnTurnAndHealthIsShared()
    {
        GameObject obj = new GameObject("Battle test unit");
        try
        {
            BattleUnit unit = obj.AddComponent<BattleUnit>();
            Health health = obj.GetComponent<Health>();
            health.maxHealth = 100;
            health.currentHealth = 100;
            Assert.That(unit.Health, Is.SameAs(health));

            unit.Defend();
            health.TakeDamage(BattleCalculator.PhysicalDamage(12, 8, unit.IsDefending, rules));
            Assert.That(health.currentHealth, Is.EqualTo(98));
            Assert.That(unit.IsDefending, Is.True);

            unit.BeginTurn();
            Assert.That(unit.IsDefending, Is.False);
        }
        finally
        {
            Object.DestroyImmediate(obj);
        }
    }
}

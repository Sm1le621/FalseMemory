using NUnit.Framework;
using UnityEngine;

public sealed class BattleCoreTests
{
    private BattleRules rules;
    [SetUp] public void Setup() => rules = ScriptableObject.CreateInstance<BattleRules>();
    [TearDown] public void Cleanup() => Object.DestroyImmediate(rules);

    [TestCase(15, 5, false, 10)]
    [TestCase(12, 8, false, 4)]
    [TestCase(12, 8, true, 2)]
    [TestCase(15, 8, true, 4)]
    [TestCase(0, 1000, false, 1)]
    [TestCase(-10, -100, true, 1)]
    public void PhysicalDamageIsNonNegativeAndDefenseIsApplied(int attack, int defense, bool guarding, int expected)
        => Assert.That(BattleCalculator.PhysicalDamage(attack, defense, guarding, rules), Is.EqualTo(expected));

    [Test] public void DamageMultipliersAreConfigurable()
    {
        rules.attackMultiplier = 2f; rules.defenseMultiplier = 0.5f;
        Assert.That(BattleCalculator.PhysicalDamage(15, 8, false, rules), Is.EqualTo(26));
    }

    private sealed class Participant : IBattleParticipant
    {
        public int Speed { get; set; }
        public bool IsAlive { get; set; } = true;
    }

    [Test] public void RoundUsesSpeedAndStableTiesWithOneTurnPerUnit()
    {
        var a = new Participant { Speed = 7 }; var b = new Participant { Speed = 10 };
        var c = new Participant { Speed = 10 }; var queue = new BattleTurnOrder<Participant>();
        queue.BeginRound(new[] { a, b, c, b });
        Assert.That(queue.TryNext(out var unit), Is.True); Assert.That(unit, Is.SameAs(b));
        Assert.That(queue.TryNext(out unit), Is.True); Assert.That(unit, Is.SameAs(c));
        Assert.That(queue.TryNext(out unit), Is.True); Assert.That(unit, Is.SameAs(a));
        Assert.That(queue.TryNext(out unit), Is.False);
    }

    [Test] public void KilledUnitsAreSkippedAndSpeedReordersOnlyAtNextRound()
    {
        var a = new Participant { Speed = 10 }; var b = new Participant { Speed = 7 };
        var queue = new BattleTurnOrder<Participant>(); queue.BeginRound(new[] { a, b });
        b.Speed = 20;
        queue.TryNext(out var unit); Assert.That(unit, Is.SameAs(a));
        b.IsAlive = false; Assert.That(queue.TryNext(out unit), Is.False);
        b.IsAlive = true; queue.BeginRound(new[] { a, b });
        queue.TryNext(out unit); Assert.That(unit, Is.SameAs(b));
    }

    [Test] public void DuplicateCommandsAreRejectedUntilNextTurn()
    {
        var gate = new BattleCommandGate(); Assert.That(gate.TryConsume(), Is.False);
        gate.Open(); Assert.That(gate.TryConsume(), Is.True); Assert.That(gate.TryConsume(), Is.False);
        gate.Open(); Assert.That(gate.TryConsume(), Is.True);
        gate.Close(); Assert.That(gate.TryConsume(), Is.False);
    }

    [Test] public void DefendPersistsUntilOwnTurnAndHealthIsShared()
    {
        var obj = new GameObject("Battle test unit");
        try
        {
            var unit = obj.AddComponent<BattleUnit>(); var health = obj.GetComponent<Health>();
            health.maxHealth = health.currentHealth = 100;
            Assert.That(unit.Health, Is.SameAs(health));
            unit.Defend(); health.TakeDamage(BattleCalculator.PhysicalDamage(12, 8, unit.IsDefending, rules));
            Assert.That(health.currentHealth, Is.EqualTo(98)); Assert.That(unit.IsDefending, Is.True);
            unit.BeginTurn(); Assert.That(unit.IsDefending, Is.False);
        }
        finally { Object.DestroyImmediate(obj); }
    }
}

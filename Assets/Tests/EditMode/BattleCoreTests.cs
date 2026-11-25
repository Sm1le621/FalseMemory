using NUnit.Framework;
using UnityEngine;

// Тести основи бою: формула шкоди і захист
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

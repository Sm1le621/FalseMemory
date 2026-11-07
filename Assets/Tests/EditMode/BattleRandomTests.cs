using NUnit.Framework;
using UnityEngine;

// Тести випадковості: крит, промах, межі шансів, seed
public sealed class BattleRandomTests
{
    // "Рандом", який завжди повертає одне й те саме число і рахує виклики
    private sealed class FixedRandom : IBattleRandom
    {
        public float sample;
        public int calls;

        public float Next01()
        {
            calls = calls + 1;
            return sample;
        }
    }

    // Крит множить шкоду, а захист діє вже після криту
    [Test]
    public void CriticalMultipliesDamageAndDefendAppliesAfterIt()
    {
        BattleRules rules = ScriptableObject.CreateInstance<BattleRules>();
        try
        {
            FixedRandom source = new FixedRandom { sample = 0.5f };

            PhysicalAttackResult normal = BattleCalculator.ResolvePhysical(15, 5, false, rules, 0f, 0f, source);
            PhysicalAttackResult critical = BattleCalculator.ResolvePhysical(15, 5, false, rules, 1f, 0f, source);
            PhysicalAttackResult defended = BattleCalculator.ResolvePhysical(15, 5, true, rules, 1f, 0f, source);

            Assert.That(normal.Damage, Is.EqualTo(10));
            Assert.That(critical.Damage, Is.EqualTo(15));
            Assert.That(critical.Critical, Is.True);
            Assert.That(critical.Missed, Is.False);
            Assert.That(defended.Damage, Is.EqualTo(8));
        }
        finally
        {
            Object.DestroyImmediate(rules);
        }
    }

    // Промах - 0 шкоди, і шанс криту після промаху навіть не перевіряється
    [Test]
    public void MissDoesZeroDamageAndNeverRollsCritical()
    {
        BattleRules rules = ScriptableObject.CreateInstance<BattleRules>();
        try
        {
            FixedRandom source = new FixedRandom { sample = 0.1f };
            PhysicalAttackResult result = BattleCalculator.ResolvePhysical(15, 5, false, rules, 1f, 0.5f, source);

            Assert.That(result.Damage, Is.Zero);
            Assert.That(result.Missed, Is.True);
            Assert.That(result.Critical, Is.False);
            // Рандом викликали тільки 1 раз (для промаху)
            Assert.That(source.calls, Is.EqualTo(1));
        }
        finally
        {
            Object.DestroyImmediate(rules);
        }
    }

    // Шанс, випадкове число, очікуваний результат
    [TestCase(-1f, 0f, false)]
    [TestCase(0f, 0f, false)]
    [TestCase(1f, 1f, true)]
    [TestCase(2f, 1f, true)]
    public void ChanceBoundsIncludeGuaranteedSuccessAndFailure(float chance, float sample, bool expected)
    {
        Assert.That(BattleCalculator.ChanceSucceeds(chance, sample), Is.EqualTo(expected));
    }

    // Однаковий seed - однакові числа
    [Test]
    public void SameSeedProducesSameStream()
    {
        SeededBattleRandom a = new SeededBattleRandom(42);
        SeededBattleRandom b = new SeededBattleRandom(42);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(a.Next01(), Is.EqualTo(b.Next01()));
        }
    }
}

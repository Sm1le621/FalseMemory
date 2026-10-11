using NUnit.Framework;
using UnityEngine;

public sealed class BattleRandomTests
{
    private sealed class FixedRandom : IBattleRandom
    {
        public float sample;
        public int calls;
        public float Next01() { calls++; return sample; }
    }

    [Test] public void CriticalMultipliesDamageAndDefendAppliesAfterIt()
    {
        var rules = ScriptableObject.CreateInstance<BattleRules>();
        try
        {
            var source = new FixedRandom {sample = 0.5f};
            var normal = BattleCalculator.ResolvePhysical(15, 5, false, rules, 0f, 0f, source);
            var critical = BattleCalculator.ResolvePhysical(15, 5, false, rules, 1f, 0f, source);
            var defended = BattleCalculator.ResolvePhysical(15, 5, true, rules, 1f, 0f, source);
            Assert.That(normal.Damage, Is.EqualTo(10)); Assert.That(critical.Damage, Is.EqualTo(15));
            Assert.That(critical.Critical, Is.True); Assert.That(critical.Missed, Is.False);
            Assert.That(defended.Damage, Is.EqualTo(8));
        }
        finally { Object.DestroyImmediate(rules); }
    }

    [Test] public void MissDoesZeroDamageAndNeverRollsCritical()
    {
        var rules = ScriptableObject.CreateInstance<BattleRules>();
        try
        {
            var source = new FixedRandom {sample = 0.1f};
            var result = BattleCalculator.ResolvePhysical(15, 5, false, rules, 1f, 0.5f, source);
            Assert.That(result.Damage, Is.Zero); Assert.That(result.Missed, Is.True); Assert.That(result.Critical, Is.False);
            Assert.That(source.calls, Is.EqualTo(1));
        }
        finally { Object.DestroyImmediate(rules); }
    }

    [TestCase(-1f, 0f, false)] [TestCase(0f, 0f, false)] [TestCase(1f, 1f, true)] [TestCase(2f, 1f, true)]
    public void ChanceBoundsIncludeGuaranteedSuccessAndFailure(float chance, float sample, bool expected)
        => Assert.That(BattleCalculator.ChanceSucceeds(chance, sample), Is.EqualTo(expected));

    [Test] public void SameSeedProducesSameStream()
    {
        var a = new SeededBattleRandom(42); var b = new SeededBattleRandom(42);
        for (int i = 0; i < 20; i++) Assert.That(a.Next01(), Is.EqualTo(b.Next01()));
    }
}

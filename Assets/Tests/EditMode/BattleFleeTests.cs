using NUnit.Framework;
using UnityEngine;

public sealed class BattleFleeTests
{
    [TestCase(10, 7, 0.74f)] [TestCase(7, 7, 0.65f)] [TestCase(0, 1000, 0.05f)] [TestCase(1000, 0, 0.95f)]
    public void SpeedAffectsEscapeWithReasonableLimits(int playerSpeed, int enemySpeed, float expected)
    {
        var rules = ScriptableObject.CreateInstance<BattleRules>();
        try { Assert.That(BattleCalculator.FleeChance(playerSpeed, enemySpeed, rules), Is.EqualTo(expected).Within(0.0001f)); }
        finally { Object.DestroyImmediate(rules); }
    }

    [Test] public void InvalidConfiguredBoundsCannotProduceProbabilitiesOutsideZeroToOne()
    {
        var rules = ScriptableObject.CreateInstance<BattleRules>();
        try
        {
            rules.minimumFleeChance = -1f; rules.maximumFleeChance = 2f; rules.fleeBaseChance = 5f;
            Assert.That(BattleCalculator.FleeChance(int.MaxValue, 0, rules), Is.EqualTo(1f));
            rules.fleeBaseChance = -5f;
            Assert.That(BattleCalculator.FleeChance(0, int.MaxValue, rules), Is.EqualTo(0f));
        }
        finally { Object.DestroyImmediate(rules); }
    }
}

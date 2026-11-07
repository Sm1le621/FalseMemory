using NUnit.Framework;
using UnityEngine;

// Тести шансу втечі
public sealed class BattleFleeTests
{
    // Швидкість гравця, швидкість ворога, очікуваний шанс
    [TestCase(10, 7, 0.74f)]
    [TestCase(7, 7, 0.65f)]
    [TestCase(0, 1000, 0.05f)]
    [TestCase(1000, 0, 0.95f)]
    public void SpeedAffectsEscapeWithReasonableLimits(int playerSpeed, int enemySpeed, float expected)
    {
        BattleRules rules = ScriptableObject.CreateInstance<BattleRules>();
        try
        {
            float chance = BattleCalculator.FleeChance(playerSpeed, enemySpeed, rules);
            // Within - допускаємо маленьку похибку float
            Assert.That(chance, Is.EqualTo(expected).Within(0.0001f));
        }
        finally
        {
            Object.DestroyImmediate(rules);
        }
    }

    // Навіть з неправильними налаштуваннями шанс завжди від 0 до 1
    [Test]
    public void InvalidConfiguredBoundsCannotProduceProbabilitiesOutsideZeroToOne()
    {
        BattleRules rules = ScriptableObject.CreateInstance<BattleRules>();
        try
        {
            rules.minimumFleeChance = -1f;
            rules.maximumFleeChance = 2f;
            rules.fleeBaseChance = 5f;
            Assert.That(BattleCalculator.FleeChance(int.MaxValue, 0, rules), Is.EqualTo(1f));

            rules.fleeBaseChance = -5f;
            Assert.That(BattleCalculator.FleeChance(0, int.MaxValue, rules), Is.EqualTo(0f));
        }
        finally
        {
            Object.DestroyImmediate(rules);
        }
    }
}

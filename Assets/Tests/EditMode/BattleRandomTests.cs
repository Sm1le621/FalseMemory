using NUnit.Framework;
using UnityEngine;

// Тести випадковості: крит, промах, межі шансів, seed
public sealed class BattleRandomTests
{
    // Крит множить шкоду, а захист діє вже після криту
    [Test]
    public void CriticalMultipliesDamageAndDefendAppliesAfterIt()
    {
        BattleRules rules = ScriptableObject.CreateInstance<BattleRules>();
        try
        {
            // Шанси 0 і 1 не залежать від випадкового числа, тому seed може бути будь-яким
            BattleRandom source = new BattleRandom(7);

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
            // Два однакові рандоми: source використовує атака, reference - для порівняння
            BattleRandom source = new BattleRandom(7);
            BattleRandom reference = new BattleRandom(7);
            PhysicalAttackResult result = BattleCalculator.ResolvePhysical(15, 5, false, rules, 1f, 1f, source);

            Assert.That(result.Damage, Is.Zero);
            Assert.That(result.Missed, Is.True);
            Assert.That(result.Critical, Is.False);
            // Рандом викликали тільки 1 раз (для промаху):
            // наступне число source дорівнює другому числу reference
            reference.Next01();
            Assert.That(source.Next01(), Is.EqualTo(reference.Next01()));
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
        BattleRandom a = new BattleRandom(42);
        BattleRandom b = new BattleRandom(42);

        for (int i = 0; i < 20; i++)
        {
            Assert.That(a.Next01(), Is.EqualTo(b.Next01()));
        }
    }
}

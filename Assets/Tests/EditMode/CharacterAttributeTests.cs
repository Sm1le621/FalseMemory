using NUnit.Framework;
using UnityEngine;

public sealed class CharacterAttributeTests
{
    [Test] public void ReferenceAttributesPreserveExistingBalanceAndGrowthMatchesLegacyRates()
    {
        var rules=ScriptableObject.CreateInstance<CharacterAttributeRules>();
        try
        {
            var neutral=CharacterAttributeValues.Uniform(10);
            foreach(DerivedAttributeStat stat in System.Enum.GetValues(typeof(DerivedAttributeStat)))Assert.That(rules.Adjustment(neutral,stat),Is.Zero);
            var level2=neutral+rules.growthPerLevel;
            Assert.That(rules.Adjustment(level2,DerivedAttributeStat.Attack),Is.EqualTo(2));
            Assert.That(rules.Adjustment(level2,DerivedAttributeStat.MaximumHealth),Is.EqualTo(5));
            Assert.That(rules.Adjustment(level2,DerivedAttributeStat.MaximumMana),Is.EqualTo(2));
            Assert.That(rules.ManaCost(8,neutral),Is.EqualTo(8));
        }
        finally {Object.DestroyImmediate(rules);}
    }
    [Test] public void DistinctAttributesAffectTheirIntendedFormulasAndManaCostsStaySafe()
    {
        var rules=ScriptableObject.CreateInstance<CharacterAttributeRules>();
        try
        {
            var values=CharacterAttributeValues.Uniform(10);values.strength=13;values.agility=14;values.dexterity=12;values.magicControl=20;
            Assert.That(rules.Adjustment(values,DerivedAttributeStat.Attack),Is.EqualTo(5));
            Assert.That(rules.Adjustment(values,DerivedAttributeStat.MaximumHealth),Is.Zero);
            Assert.That(rules.MovementAdjustment(values),Is.Zero);
            Assert.That(rules.Evasion(values),Is.EqualTo(.02f).Within(.0001f));
            Assert.That(rules.AccuracyBonus(values),Is.EqualTo(.01f).Within(.0001f));
            Assert.That(rules.ManaCost(8,values),Is.EqualTo(7));values.magicControl=int.MaxValue;
            Assert.That(rules.ManaCost(8,values),Is.EqualTo(6));Assert.That(rules.ManaCost(0,values),Is.Zero);Assert.That(rules.ManaCost(-1,values),Is.EqualTo(-1));
        }
        finally {Object.DestroyImmediate(rules);}
    }
    [Test] public void PrimaryRestoreAndModifiersNeverAccumulateDerivedGrowth()
    {
        var go=new GameObject("AttributeTest");var rules=ScriptableObject.CreateInstance<CharacterAttributeRules>();
        try
        {
            var attributes=go.AddComponent<CharacterAttributes>();attributes.rules=rules;
            var equipment=new CharacterAttributeValues{strength=3};
            attributes.SetTemporaryBonuses(new CharacterAttributeValues{strength=-2});
            Assert.That(attributes.Calculate(4,equipment).strength,Is.EqualTo(14));
            string json=JsonUtility.ToJson(attributes.BaseValues);attributes.SetBase(CharacterAttribute.Strength,99);
            attributes.RestoreBaseValues(JsonUtility.FromJson<CharacterAttributeValues>(json));
            Assert.That(attributes.Calculate(4,equipment).strength,Is.EqualTo(14));
            Assert.That(attributes.BaseValues.strength,Is.EqualTo(10));
            Assert.That((new CharacterAttributeValues{strength=int.MaxValue}+equipment).strength,Is.EqualTo(int.MaxValue));
        }
        finally {Object.DestroyImmediate(go);Object.DestroyImmediate(rules);}
    }
}

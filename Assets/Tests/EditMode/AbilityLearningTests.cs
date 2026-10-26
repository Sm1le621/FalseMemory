using NUnit.Framework;
using UnityEngine;

public sealed class AbilityLearningTests
{
    [Test] public void LearningUsesIndependentPointsRequirementsStableIdsAndRejectsReentry()
    {
        var go=new GameObject("Learning test");var rules=ScriptableObject.CreateInstance<CharacterAttributeRules>();var spell=ScriptableObject.CreateInstance<BattleSpell>();
        try
        {
            rules.classlessProgression=true;var attributes=go.AddComponent<CharacterAttributes>();attributes.rules=rules;
            var unit=go.AddComponent<BattleUnit>();unit.Health.currentHealth=100;
            var learn=go.AddComponent<AbilityLearning>();spell.id="test.power";spell.requirements=new CharacterAttributeValues{strength=20};learn.catalog=new[]{spell};
            Assert.That(learn.Availability(spell),Is.EqualTo(AbilityAvailability.RequirementsNotMet));Assert.That(learn.TryLearn(spell),Is.False);
            attributes.SetBase(CharacterAttribute.Strength,20);attributes.SynchronizeLevel(4);int statPoints=attributes.UnspentPoints;int points=learn.Points;
            bool nested=true;learn.Changed+=()=>nested=learn.TryLearn(spell);
            Assert.That(learn.TryLearn(spell),Is.True);Assert.That(nested,Is.False);Assert.That(learn.TryLearn(spell),Is.False);
            Assert.That(learn.Points,Is.EqualTo(points-1));Assert.That(attributes.UnspentPoints,Is.EqualTo(statPoints));Assert.That(learn.LearnedIds,Does.Contain("test.power"));
            attributes.SetBase(CharacterAttribute.Strength,10);Assert.That(learn.Knows(spell),Is.True,"Learning requirements do not erase a learned ability.");
            var unknown=ScriptableObject.CreateInstance<BattleSpell>();try {Assert.That(learn.TryLearn(unknown),Is.False);}finally{Object.DestroyImmediate(unknown);}
        }
        finally {Object.DestroyImmediate(go);Object.DestroyImmediate(rules);Object.DestroyImmediate(spell);}
    }
    [Test] public void LegacyKnownFireIgnoresNewLearningRequirementsAndInsufficientCurrencyCannotLearn()
    {
        var go=new GameObject("Legacy known");var fire=ScriptableObject.CreateInstance<BattleSpell>();var other=ScriptableObject.CreateInstance<BattleSpell>();
        try
        {
            var unit=go.AddComponent<BattleUnit>();unit.Health.currentHealth=100;fire.id="spell.fire";fire.requirements=new CharacterAttributeValues{magicPower=99};unit.spells=new[]{fire};
            var learn=go.AddComponent<AbilityLearning>();other.id="test.expensive";other.learningCost=100;learn.catalog=new[]{fire,other};
            Assert.That(learn.Knows(fire),Is.True);Assert.That(learn.Availability(fire),Is.EqualTo(AbilityAvailability.Learned));Assert.That(learn.TryLearn(fire),Is.False);
            Assert.That(learn.Availability(other),Is.EqualTo(AbilityAvailability.NotEnoughPoints));Assert.That(learn.TryLearn(other),Is.False);Assert.That(learn.Points,Is.EqualTo(3));
        }
        finally {Object.DestroyImmediate(go);Object.DestroyImmediate(fire);Object.DestroyImmediate(other);}
    }
}

using NUnit.Framework;
using UnityEngine;

public sealed class CharacterGrowthTests
{
    [Test] public void CurveSupportsBoundariesMultipleLevelsAndUnreachableLargeThresholds()
    {
        var rules=ScriptableObject.CreateInstance<CharacterGrowthRules>();
        try
        {
            Assert.That(rules.TotalForLevel(1),Is.Zero);Assert.That(rules.TotalForLevel(2),Is.EqualTo(100));Assert.That(rules.TotalForLevel(4),Is.EqualTo(450));
            Assert.That(rules.LevelFor(99,1),Is.EqualTo(1));Assert.That(rules.LevelFor(100,1),Is.EqualTo(2));Assert.That(rules.LevelFor(450,1),Is.EqualTo(4));
            Assert.That(rules.Remaining(110,2),Is.EqualTo(140));rules.maximumLevel=3;Assert.That(rules.LevelFor(int.MaxValue,1),Is.EqualTo(3));
            Assert.That(rules.Remaining(int.MaxValue,3),Is.Zero);rules.firstLevelExperience=int.MaxValue;rules.experienceStep=int.MaxValue;
            Assert.That(rules.LevelFor(0,5),Is.EqualTo(5),"Changing the curve must not erase existing levels");
            Assert.That(rules.TotalForLevel(3),Is.GreaterThan(int.MaxValue));Assert.That(rules.LevelFor(int.MaxValue,1),Is.EqualTo(2));
            rules.firstLevelExperience=-1;rules.experienceStep=-1;Assert.That(rules.TotalForLevel(3),Is.EqualTo(2));
        }
        finally{Object.DestroyImmediate(rules);}
    }
    [Test] public void GrowthIsDerivedAndExperienceNotificationsRejectReentryWithoutHealing()
    {
        var rules=ScriptableObject.CreateInstance<CharacterGrowthRules>();var obj=new GameObject("Growth test",typeof(Health),typeof(BattleUnit));var unit=obj.GetComponent<BattleUnit>();
        try
        {
            unit.growthRules=rules;unit.maxMP=30;unit.Health.currentHealth=80;int calls=0;int nested=-1;
            unit.ProgressChanged+=()=>{calls++;nested=unit.AddExperience(100);};
            Assert.That(unit.AddExperience(450),Is.EqualTo(450));Assert.That(unit.level,Is.EqualTo(4));Assert.That(unit.LevelsGainedLastAward,Is.EqualTo(3));
            Assert.That(unit.EffectiveAttack,Is.EqualTo(21));Assert.That(unit.attack,Is.EqualTo(15));Assert.That(unit.Health.maxHealth,Is.EqualTo(115));
            Assert.That(unit.Health.currentHealth,Is.EqualTo(80));Assert.That(unit.MaxMP,Is.EqualTo(36));Assert.That(nested,Is.Zero);Assert.That(calls,Is.EqualTo(1));
            Assert.That(unit.AddExperience(1),Is.EqualTo(1));Assert.That(unit.EffectiveAttack,Is.EqualTo(21));Assert.That(unit.Health.maxHealth,Is.EqualTo(115));
            Assert.That(unit.AddExperience(-1),Is.Zero);unit.Health.TakeDamage(80);Assert.That(unit.AddExperience(50),Is.Zero);Assert.That(unit.level,Is.EqualTo(4));
        }
        finally{Object.DestroyImmediate(obj);Object.DestroyImmediate(rules);}
    }
    [Test] public void ExtremeBonusesAndCapacityNeverOverflowOrResurrect()
    {
        var rules=ScriptableObject.CreateInstance<CharacterGrowthRules>();
        try{Assert.That(rules.Growth(int.MaxValue,99),Is.EqualTo(int.MaxValue));Assert.That(rules.Growth(-10,5),Is.Zero);}
        finally{Object.DestroyImmediate(rules);}
        var obj=new GameObject("Health capacity test",typeof(Health));var health=obj.GetComponent<Health>();
        try{health.currentHealth=0;health.SetMaximumHealth(200);Assert.That(health.currentHealth,Is.Zero);health.currentHealth=90;health.SetMaximumHealth(60);Assert.That(health.currentHealth,Is.EqualTo(60));}
        finally{Object.DestroyImmediate(obj);}
    }
}

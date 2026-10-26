using NUnit.Framework;
using UnityEngine;

public sealed class ArmorHolyTests
{
    private CharacterAttributeRules attributes;private BattleRules rules;
    [SetUp] public void Setup(){attributes=ScriptableObject.CreateInstance<CharacterAttributeRules>();attributes.classlessProgression=true;rules=ScriptableObject.CreateInstance<BattleRules>();rules.useDiminishingDefense=true;}
    [TearDown] public void Cleanup(){Object.DestroyImmediate(attributes);Object.DestroyImmediate(rules);}
    [TestCase(10,0)][TestCase(20,.05)][TestCase(30,.1)][TestCase(40,.125)][TestCase(50,.15)][TestCase(60,.175)][TestCase(80,.195)][TestCase(99,.214)]
    public void WisdomHolyBonusMatchesAllRequestedAnchors(int wis,double expected)
    {var values=CharacterAttributeValues.Uniform(10);values.wisdom=wis;Assert.That(attributes.HolyBonus(values),Is.EqualTo(expected).Within(.0000001));}
    [TestCase(CreatureCategory.Undead,10)][TestCase(CreatureCategory.Unholy,10)][TestCase(CreatureCategory.Human,0)][TestCase(CreatureCategory.Beast,0)][TestCase(CreatureCategory.None,0)]
    public void HolyIsAnIndependentComponentAndOnlyEligibleCategoriesReceiveIt(CreatureCategory category,int holy)
    {var result=BattleDamage.Compose(100,.1,category,0);Assert.That(result.BaseDamage,Is.EqualTo(100));Assert.That(result.HolyDamage,Is.EqualTo(holy));Assert.That(result.Total,Is.EqualTo(100+holy));}
    [Test] public void IntrinsicHolyResistanceAndPureHolyCannotLeakIntoHumanDamage()
    {
        Assert.That(BattleDamage.Compose(100,.1,CreatureCategory.Undead,.5f,20).HolyDamage,Is.EqualTo(15));
        Assert.That(BattleDamage.Compose(100,.1,CreatureCategory.Human,0,20).HolyDamage,Is.Zero);
        Assert.That(BattleDamage.Compose(100,.1,CreatureCategory.Human,0,20,true).Total,Is.Zero);
        Assert.That(BattleDamage.Compose(100,.1,CreatureCategory.Undead,1,20,true).Total,Is.Zero);
    }
    [Test] public void DefenseRatingHasDiminishingReturnsAndNeverGivesInvulnerability()
    {
        Assert.That(BattleCalculator.PhysicalDamage(15,5,false,rules),Is.EqualTo(12));
        Assert.That(BattleCalculator.PhysicalDamage(12,8,false,rules),Is.EqualTo(9));
        Assert.That(BattleCalculator.PhysicalDamage(12,8,true,rules),Is.EqualTo(5));
        Assert.That(BattleCalculator.MagicalDamage(12,3,10,rules),Is.EqualTo(20));
        Assert.That(BattleCalculator.PhysicalDamage(100,20,false,rules),Is.EqualTo(50));
        Assert.That(BattleCalculator.PhysicalDamage(100,60,false,rules),Is.EqualTo(25));
        Assert.That(BattleCalculator.PhysicalDamage(100,int.MaxValue,false,rules),Is.EqualTo(1));
    }
    [Test] public void ArmorAggregationHasNoFeedbackIntoPrimaryAndRemovingItRemovesTheBonus()
    {
        var item=ScriptableObject.CreateInstance<ItemData>();
        try
        {
            item.id="test.scaledarmor";item.category=ItemCategory.Armor;item.equipmentSlot=EquipmentSlot.Chest;item.useArmorScaling=true;
            item.armor=new ArmorScalingData{physicalDefense=20,magicDefense=10,evasion=.03f,
                physicalScaling=new[]{new AttributeScalingEntry{attribute=CharacterAttribute.Endurance,grade=ScalingGrade.A}},
                magicScaling=new[]{new AttributeScalingEntry{attribute=CharacterAttribute.Wisdom,grade=ScalingGrade.S}},
                evasionScaling=new[]{new AttributeScalingEntry{attribute=CharacterAttribute.Agility,grade=ScalingGrade.A}}};
            var values=CharacterAttributeValues.Uniform(30);var bag=new InventoryState();var gear=new EquipmentState();bag.TryAdd(item,1);
            for(int i=0;i<4;i++)
            {
                Assert.That(gear.TryEquip(bag,0,EquipmentSlot.Chest,1),Is.True);var b=gear.ScaledBonuses(values,attributes);
                Assert.That(b.defense,Is.EqualTo(30));Assert.That(b.magicDefense,Is.EqualTo(16));Assert.That(b.evasion,Is.EqualTo(.0444f).Within(.0001));
                Assert.That(gear.ScaledBonuses(values,attributes).defense,Is.EqualTo(b.defense));Assert.That(gear.Bonuses.defense,Is.Zero);
                Assert.That(gear.TryUnequipTo(bag,EquipmentSlot.Chest,0,1),Is.True);Assert.That(gear.ScaledBonuses(values,attributes).defense,Is.Zero);
            }
        }
        finally {Object.DestroyImmediate(item);}
    }
}

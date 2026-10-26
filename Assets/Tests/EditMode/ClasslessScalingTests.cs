using System;
using NUnit.Framework;
using UnityEngine;

public sealed class ClasslessScalingTests
{
    private CharacterAttributeRules rules;
    [SetUp] public void Setup(){rules=ScriptableObject.CreateInstance<CharacterAttributeRules>();rules.classlessProgression=true;}
    [TearDown] public void Cleanup(){UnityEngine.Object.DestroyImmediate(rules);}

    [Test] public void EverySoftCapHasPositiveNonIncreasingMarginalGainOnBothSides()
    {
        foreach(CharacterAttribute stat in Enum.GetValues(typeof(CharacterAttribute)))
        {
            var curve=rules.Curve(stat);
            foreach(int cap in curve.thresholds)
            {
                double before=curve.Evaluate(cap)-curve.Evaluate(cap-1),after=curve.Evaluate(cap+1)-curve.Evaluate(cap);
                Assert.That(before,Is.GreaterThan(0),stat+" before "+cap);Assert.That(after,Is.GreaterThan(0),stat+" after "+cap);
                Assert.That(after,Is.LessThanOrEqualTo(before),stat+" cap "+cap);
            }
        }
    }
    [TestCase(10,100)][TestCase(20,180)][TestCase(40,340)][TestCase(60,420)][TestCase(99,459)]
    public void VitalityUsesRequestedHealthCurve(int value,int hp)
    {var values=CharacterAttributeValues.Uniform(10);values.vitality=value;Assert.That(100+rules.Adjustment(values,DerivedAttributeStat.MaximumHealth),Is.EqualTo(hp));}
    [TestCase(10,40)][TestCase(40,160)][TestCase(60,200)][TestCase(61,201)][TestCase(99,220)]
    public void IntelligenceUsesManaCurveWithSingleRoundingAndWisdomDoesNotAddMana(int value,int mp)
    {var values=CharacterAttributeValues.Uniform(10);values.intelligence=value;values.wisdom=99;Assert.That(40+rules.Adjustment(values,DerivedAttributeStat.MaximumMana),Is.EqualTo(mp));}
    [Test] public void AgilityNeverChangesWalkSpeedAndPaidSpellsKeepPositiveThirtyPercentFloor()
    {
        foreach(int value in new[]{0,10,30,60,99,int.MaxValue})
        {var values=CharacterAttributeValues.Uniform(10);values.agility=values.magicControl=value;Assert.That(rules.MovementAdjustment(values),Is.Zero);Assert.That(rules.ManaCost(8,values),Is.GreaterThanOrEqualTo(6));Assert.That(rules.ManaCost(1,values),Is.EqualTo(1));}
    }
    [Test] public void PointsAwardOnceAcrossLevelsAndReentrantAllocationCannotSpendTwice()
    {
        var go=new GameObject("ClasslessPoints");
        try
        {
            var attributes=go.AddComponent<CharacterAttributes>();attributes.rules=rules;attributes.SynchronizeLevel(4);attributes.SynchronizeLevel(4);
            Assert.That(attributes.UnspentPoints,Is.EqualTo(3));Assert.That(attributes.Growth(4).strength,Is.Zero);
            bool nested=true;attributes.Changed+=()=>nested=attributes.TryIncreaseBase(CharacterAttribute.Strength);
            Assert.That(attributes.TryIncreaseBase(CharacterAttribute.Strength),Is.True);Assert.That(nested,Is.False);
            Assert.That(attributes.UnspentPoints,Is.EqualTo(2));Assert.That(attributes.BaseValues.strength,Is.EqualTo(11));
            attributes.SetBase(CharacterAttribute.Strength,1000);Assert.That(attributes.BaseValues.strength,Is.EqualTo(99));
            Assert.That(attributes.TryIncreaseBase(CharacterAttribute.Strength),Is.False);Assert.That(attributes.UnspentPoints,Is.EqualTo(2));
            attributes.SetClassBonuses(new CharacterAttributeValues{strength=50});Assert.That(attributes.Calculate(4,new CharacterAttributeValues()).strength,Is.EqualTo(99));
        }
        finally {UnityEngine.Object.DestroyImmediate(go);}
    }
    [Test] public void ScalingGradesAreOrderedNoneIsBaseAndPrimarySpecializationBeatsWeakSecondary()
    {
        var weapon=new WeaponScalingData{baseDamage=40,scaling=new[]{new AttributeScalingEntry{attribute=CharacterAttribute.Strength,grade=ScalingGrade.S},new AttributeScalingEntry{attribute=CharacterAttribute.Dexterity,grade=ScalingGrade.E}}};
        var strength=CharacterAttributeValues.Uniform(10);strength.strength=50;
        var hybrid=CharacterAttributeValues.Uniform(10);hybrid.strength=30;hybrid.dexterity=30;
        Assert.That(weapon.Damage(strength,rules),Is.GreaterThan(weapon.Damage(hybrid,rules)));
        int previous=40;
        foreach(ScalingGrade grade in Enum.GetValues(typeof(ScalingGrade)))
        {weapon.scaling=new[]{new AttributeScalingEntry{attribute=CharacterAttribute.Strength,grade=grade}};int damage=weapon.Damage(strength,rules);Assert.That(damage,Is.GreaterThanOrEqualTo(previous));if(grade==ScalingGrade.None)Assert.That(damage,Is.EqualTo(40));previous=damage;}
        weapon.scaling=new[]{new AttributeScalingEntry{attribute=CharacterAttribute.Strength,grade=ScalingGrade.S},new AttributeScalingEntry{attribute=CharacterAttribute.Strength,grade=ScalingGrade.S}};
        int once=weapon.Damage(strength,rules);weapon.scaling=new[]{weapon.scaling[0]};Assert.That(weapon.Damage(strength,rules),Is.EqualTo(once));
    }
    [Test] public void RequirementsExcludeTheCandidateAndReplacementItemAndRejectWithoutLosingGear()
    {
        var item=ScriptableObject.CreateInstance<ItemData>();var previous=ScriptableObject.CreateInstance<ItemData>();
        try
        {
            item.id="test.requirements";item.category=ItemCategory.Weapon;item.weapon=new WeaponScalingData{requirements=new CharacterAttributeValues{strength=25}};item.bonuses=new ItemStatBonus{attributes=new CharacterAttributeValues{strength=100}};
            previous.id="test.previous";previous.category=ItemCategory.Weapon;previous.bonuses=new ItemStatBonus{attributes=new CharacterAttributeValues{strength=20}};
            var state=new EquipmentState();var bag=new InventoryState();bag.TryAdd(previous,1);bag.TryAdd(item,1);
            state.BindRequirements((candidate,gear)=>candidate.weapon==null||candidate.weapon.MeetsRequirements(CharacterAttributeValues.Uniform(10)+gear));
            Assert.That(state.TryEquip(bag,0,EquipmentSlot.Weapon,1),Is.True);
            Assert.That(state.TryEquip(bag,1,EquipmentSlot.Weapon,1),Is.False);Assert.That(bag.Count(item),Is.EqualTo(1));Assert.That(state.GetSlot(EquipmentSlot.Weapon),Is.SameAs(previous));
            Assert.That(state.TryUnequipTo(bag,EquipmentSlot.Weapon,1,1),Is.False);Assert.That(bag.Count(item),Is.EqualTo(1));
        }
        finally {UnityEngine.Object.DestroyImmediate(item);UnityEngine.Object.DestroyImmediate(previous);}
    }
    [Test] public void OldSerializedItemsCannotImplicitlyEnableScaling()
    {
        var item=ScriptableObject.CreateInstance<ItemData>();
        try
        {
            item.category=ItemCategory.Weapon;item.weapon=new WeaponScalingData{baseDamage=1};
            Assert.That(item.HasWeaponScaling,Is.False,"Unity may materialize old nested fields; an explicit opt-in is required.");
            item.useWeaponScaling=true;Assert.That(item.HasWeaponScaling,Is.True);
        }
        finally {UnityEngine.Object.DestroyImmediate(item);}
    }
}

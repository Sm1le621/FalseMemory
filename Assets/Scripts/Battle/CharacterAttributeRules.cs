using System;
using UnityEngine;

// Coefficients apply deviations from referenceValue, preserving the existing level-one balance.
[CreateAssetMenu(menuName="FalseMemory/Character Attribute Rules")]
public sealed class CharacterAttributeRules : ScriptableObject
{
    [Header("Classless progression (opt-in migration of existing profiles)")]
    public bool classlessProgression;
    [Min(0)] public int attributePointsPerLevel = 1;
    [Min(0)] public int referenceMana = 40;
    [Min(1)] public float scalingReferencePoints = 50f;
    [Min(0)] public float maximumWeaponScalingBonus = 1.5f;
    public double[] scalingGrades = { 0, .1, .2, .4, .6, .8, 1 };
    public AttributeSoftCapCurve[] softCaps = DefaultCurves();
    [Min(0)] public int referenceValue=10;
    public CharacterAttributeValues growthPerLevel=CharacterAttributeValues.Uniform(1);
    [Min(0)] public float strengthAttack=1f, dexterityAttack=1f, enduranceDefense=1f;
    [Min(0)] public float magicPowerAttack=1f, wisdomDefense=1f, agilitySpeed=1f;
    [Min(0)] public float vitalityHealth=5f, intelligenceMana=1f, wisdomMana=1f;
    [Min(0)] public float agilityMovement=.03f, dexterityAccuracy=.005f;
    [Min(0)] public float maximumMovementSpeed=12f;
    [Min(0)] public float dexterityCritical=.002f, agilityCritical=.001f, agilityEvasion=.005f;
    [Min(0)] public float magicControlEfficiency=.02f, magicPowerHealing=1f;
    [Range(0,1)] public float maximumManaReduction=.3f, maximumCritical=.95f, maximumEvasion=.5f;
    [Header("Prepared checks: no stamina, dialogue, disease or spell-miss systems yet")]
    [Min(0)] public float strengthCapacity=2f,enduranceStamina=5f,enduranceRecovery=.1f;
    [Min(0)] public float vitalityResistance=1f,intelligenceKnowledge=1f,wisdomPerception=1f;
    [Min(0)] public float charismaPersuasion=1f,magicControlPrecision=1f;

    private static AttributeSoftCapCurve[] DefaultCurves()
    {
        var result = new AttributeSoftCapCurve[CharacterAttributeValues.Count];
        for (int i = 0; i < result.Length; i++) result[i] = AttributeSoftCapCurve.Default((CharacterAttribute)i);
        return result;
    }
    public AttributeSoftCapCurve Curve(CharacterAttribute attribute)
    {
        foreach (var curve in softCaps ?? Array.Empty<AttributeSoftCapCurve>())
            if (curve != null && curve.attribute == attribute) return curve;
        return AttributeSoftCapCurve.Default(attribute);
    }
    public double GradeCoefficient(ScalingGrade grade)
    {
        int index = (int)grade;
        if (index <= 0 || scalingGrades == null || index >= scalingGrades.Length) return 0;
        double value = scalingGrades[index];
        return double.IsNaN(value) || double.IsInfinity(value) ? 0 : Math.Max(0, value);
    }
    public double ScalingScore(CharacterAttributeValues values, CharacterAttribute attribute)
    {
        // HP/MP curves have their own units; weapon affinity uses normalized marginal progress.
        var curve = Curve(attribute);
        double firstPoint = curve.Evaluate(1);
        return firstPoint <= 0 ? 0 : curve.Evaluate(values.Get(attribute)) / firstPoint / Math.Max(1, Coefficient(scalingReferencePoints));
    }
    private double Delta(CharacterAttributeValues values,CharacterAttribute attribute) => classlessProgression
        ? Curve(attribute).Evaluate(values.Get(attribute)) - Curve(attribute).Evaluate(Math.Max(0,referenceValue))
        : (double)values.Get(attribute)-Math.Max(0,referenceValue);
    private static double Coefficient(float value) => float.IsNaN(value)||float.IsInfinity(value)?0:Math.Max(0,value);
    public int Adjustment(CharacterAttributeValues values,DerivedAttributeStat stat)
    {
        double amount;
        switch(stat)
        {
            case DerivedAttributeStat.Attack:amount=Delta(values,CharacterAttribute.Strength)*Coefficient(strengthAttack)+Delta(values,CharacterAttribute.Dexterity)*Coefficient(dexterityAttack);break;
            case DerivedAttributeStat.Defense:amount=Delta(values,CharacterAttribute.Endurance)*Coefficient(enduranceDefense);break;
            case DerivedAttributeStat.MagicAttack:amount=Delta(values,CharacterAttribute.MagicPower)*Coefficient(magicPowerAttack);break;
            case DerivedAttributeStat.MagicDefense:amount=Delta(values,CharacterAttribute.Wisdom)*Coefficient(wisdomDefense);break;
            case DerivedAttributeStat.Speed:amount=Delta(values,CharacterAttribute.Agility)*Coefficient(agilitySpeed);break;
            case DerivedAttributeStat.MaximumHealth:amount=Delta(values,CharacterAttribute.Vitality)*(classlessProgression?1:Coefficient(vitalityHealth));break;
            case DerivedAttributeStat.MaximumMana:amount=classlessProgression?Delta(values,CharacterAttribute.Intelligence):Delta(values,CharacterAttribute.Intelligence)*Coefficient(intelligenceMana)+Delta(values,CharacterAttribute.Wisdom)*Coefficient(wisdomMana);break;
            default:throw new ArgumentOutOfRangeException(nameof(stat));
        }
        return (int)Math.Max(int.MinValue,Math.Min(int.MaxValue,Math.Round(amount,MidpointRounding.AwayFromZero)));
    }
    // Kept for serialized/API compatibility. AGI never changes world movement speed.
    public float MovementAdjustment(CharacterAttributeValues values) => 0f;
    public float AccuracyBonus(CharacterAttributeValues values) => (float)Delta(values,CharacterAttribute.Dexterity)*(float)Coefficient(dexterityAccuracy);
    public float CriticalBonus(CharacterAttributeValues values) => (float)(Delta(values,CharacterAttribute.Dexterity)*Coefficient(dexterityCritical)+Delta(values,CharacterAttribute.Agility)*Coefficient(agilityCritical));
    public float Evasion(CharacterAttributeValues values) => Mathf.Clamp((float)(Delta(values,CharacterAttribute.Agility)*Coefficient(agilityEvasion)),0,Mathf.Clamp01(maximumEvasion));
    public int ManaCost(int baseCost,CharacterAttributeValues values)
    {
        if(baseCost<0)return -1;
        double reduction=Math.Max(0,Math.Min(Mathf.Clamp01(maximumManaReduction),Delta(values,CharacterAttribute.MagicControl)*Coefficient(magicControlEfficiency)));
        return baseCost==0?0:Math.Max(1,(int)Math.Ceiling(baseCost*(1-reduction)));
    }
    public int HealingBonus(CharacterAttributeValues values) => (int)Math.Max(int.MinValue,Math.Min(int.MaxValue,Math.Round(Delta(values,CharacterAttribute.MagicPower)*Coefficient(magicPowerHealing),MidpointRounding.AwayFromZero)));
    // These scores are data/interfaces for future mechanics, not gameplay that exists today.
    public double PreparedScore(CharacterAttributeValues values,CharacterAttribute attribute)
    {
        double factor;
        switch(attribute)
        {
            case CharacterAttribute.Strength:factor=Coefficient(strengthCapacity);break;
            case CharacterAttribute.Endurance:factor=Coefficient(enduranceStamina);break;
            case CharacterAttribute.Vitality:factor=Coefficient(vitalityResistance);break;
            case CharacterAttribute.Intelligence:factor=Coefficient(intelligenceKnowledge);break;
            case CharacterAttribute.Wisdom:factor=Coefficient(wisdomPerception);break;
            case CharacterAttribute.Charisma:factor=Coefficient(charismaPersuasion);break;
            case CharacterAttribute.MagicControl:factor=Coefficient(magicControlPrecision);break;
            default:factor=1;break;
        }
        return Math.Max(0,values.Get(attribute))*factor;
    }
}

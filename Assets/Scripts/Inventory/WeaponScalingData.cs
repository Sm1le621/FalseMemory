using System;

public enum ScalingGrade { None, E, D, C, B, A, S }
public enum WeaponDamageType { Physical, Magic, Holy }

[Serializable]
public struct AttributeScalingEntry
{
    public CharacterAttribute attribute;
    public ScalingGrade grade;
}

// Optional data on the existing ItemData. No quantities or equipped state here.
[Serializable]
public sealed class WeaponScalingData
{
    public string weaponType = "sword";
    public WeaponDamageType damageType;
    public int baseDamage = 1;
    public int holyBaseDamage;
    public CharacterAttributeValues requirements;
    public AttributeScalingEntry[] scaling = Array.Empty<AttributeScalingEntry>();

    public bool MeetsRequirements(CharacterAttributeValues values)
    {
        for (int i = 0; i < CharacterAttributeValues.Count; i++)
            if (values.Get((CharacterAttribute)i) < Math.Max(0, requirements.Get((CharacterAttribute)i))) return false;
        return true;
    }
    public int Damage(CharacterAttributeValues values, CharacterAttributeRules rules)
        => Scale(baseDamage,values,scaling,rules);
    public int HolyDamage(CharacterAttributeValues values, CharacterAttributeRules rules)
        => Scale(holyBaseDamage,values,scaling,rules);
    public static int Scale(int basis,CharacterAttributeValues values,AttributeScalingEntry[] entries,CharacterAttributeRules rules)
    {
        double bonus = 0;
        var used = new bool[CharacterAttributeValues.Count];
        foreach (var entry in entries ?? Array.Empty<AttributeScalingEntry>())
        {
            int index = (int)entry.attribute;
            if (index < 0 || index >= used.Length || used[index] || rules == null) continue;
            used[index] = true;
            bonus += rules.GradeCoefficient(entry.grade) * rules.ScalingScore(values, entry.attribute);
        }
        bonus = Math.Max(0, Math.Min(rules != null ? Math.Max(0, rules.maximumWeaponScalingBonus) : 0, bonus));
        return (int)Math.Min(int.MaxValue, Math.Round(Math.Max(0, basis) * (1 + bonus), MidpointRounding.AwayFromZero));
    }
}

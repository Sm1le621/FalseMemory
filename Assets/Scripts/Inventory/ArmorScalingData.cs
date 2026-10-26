using System;

[Serializable]
public sealed class ArmorScalingData
{
    public int physicalDefense, magicDefense;
    public float evasion;
    public CharacterAttributeValues requirements;
    public AttributeScalingEntry[] physicalScaling=Array.Empty<AttributeScalingEntry>();
    public AttributeScalingEntry[] magicScaling=Array.Empty<AttributeScalingEntry>();
    public AttributeScalingEntry[] evasionScaling=Array.Empty<AttributeScalingEntry>();
    public bool MeetsRequirements(CharacterAttributeValues values)
    {
        for(int i=0;i<CharacterAttributeValues.Count;i++)if(values.Get((CharacterAttribute)i)<Math.Max(0,requirements.Get((CharacterAttribute)i)))return false;
        return true;
    }
    public ItemStatBonus Bonuses(CharacterAttributeValues values,CharacterAttributeRules rules)
    {
        int basis=float.IsNaN(evasion)||float.IsInfinity(evasion)?0:(int)Math.Round(Math.Max(0,Math.Min(1,evasion))*100000);
        return new ItemStatBonus{defense=WeaponScalingData.Scale(physicalDefense,values,physicalScaling,rules),
            magicDefense=WeaponScalingData.Scale(magicDefense,values,magicScaling,rules),
            evasion=Math.Min(1,WeaponScalingData.Scale(basis,values,evasionScaling,rules)/100000f)};
    }
}

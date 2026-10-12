using System;

// Stable numeric values for future save data; append rather than reorder.
public enum CharacterAttribute { Strength, Agility, Dexterity, Endurance, Vitality, Intelligence, Wisdom, Charisma, MagicPower, MagicControl }
public enum DerivedAttributeStat { Attack, Defense, MagicAttack, MagicDefense, Speed, MaximumHealth, MaximumMana }

[Serializable]
public struct CharacterAttributeValues
{
    public int strength, agility, dexterity, endurance, vitality, intelligence, wisdom, charisma, magicPower, magicControl;
    public static CharacterAttributeValues Uniform(int value) => new CharacterAttributeValues
    {
        strength=value,agility=value,dexterity=value,endurance=value,vitality=value,
        intelligence=value,wisdom=value,charisma=value,magicPower=value,magicControl=value
    };
    public int Get(CharacterAttribute attribute)
    {
        switch(attribute)
        {
            case CharacterAttribute.Strength:return strength;
            case CharacterAttribute.Agility:return agility;
            case CharacterAttribute.Dexterity:return dexterity;
            case CharacterAttribute.Endurance:return endurance;
            case CharacterAttribute.Vitality:return vitality;
            case CharacterAttribute.Intelligence:return intelligence;
            case CharacterAttribute.Wisdom:return wisdom;
            case CharacterAttribute.Charisma:return charisma;
            case CharacterAttribute.MagicPower:return magicPower;
            case CharacterAttribute.MagicControl:return magicControl;
            default:throw new ArgumentOutOfRangeException(nameof(attribute));
        }
    }
    public void Set(CharacterAttribute attribute,int value)
    {
        switch(attribute)
        {
            case CharacterAttribute.Strength:strength=value;break;
            case CharacterAttribute.Agility:agility=value;break;
            case CharacterAttribute.Dexterity:dexterity=value;break;
            case CharacterAttribute.Endurance:endurance=value;break;
            case CharacterAttribute.Vitality:vitality=value;break;
            case CharacterAttribute.Intelligence:intelligence=value;break;
            case CharacterAttribute.Wisdom:wisdom=value;break;
            case CharacterAttribute.Charisma:charisma=value;break;
            case CharacterAttribute.MagicPower:magicPower=value;break;
            case CharacterAttribute.MagicControl:magicControl=value;break;
            default:throw new ArgumentOutOfRangeException(nameof(attribute));
        }
    }
    public CharacterAttributeValues NonNegative()
    {
        var result=this;
        foreach(CharacterAttribute attribute in Enum.GetValues(typeof(CharacterAttribute)))result.Set(attribute,Math.Max(0,Get(attribute)));
        return result;
    }
    public static int Clamp(long value) => (int)Math.Max(int.MinValue,Math.Min(int.MaxValue,value));
    public static CharacterAttributeValues operator +(CharacterAttributeValues a,CharacterAttributeValues b)
    {
        var result=new CharacterAttributeValues();
        foreach(CharacterAttribute attribute in Enum.GetValues(typeof(CharacterAttribute)))result.Set(attribute,Clamp((long)a.Get(attribute)+b.Get(attribute)));
        return result;
    }
    public CharacterAttributeValues Scale(int count)
    {
        var result=new CharacterAttributeValues();count=Math.Max(0,count);
        foreach(CharacterAttribute attribute in Enum.GetValues(typeof(CharacterAttribute)))result.Set(attribute,Clamp((long)Get(attribute)*count));
        return result;
    }
}

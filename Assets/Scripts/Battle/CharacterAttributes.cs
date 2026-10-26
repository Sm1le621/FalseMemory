using System;
using UnityEngine;

// Primary data only. BattleUnit remains responsible for Level/EXP/MP and Health for HP.
[DisallowMultipleComponent]
public sealed class CharacterAttributes : MonoBehaviour
{
    public CharacterAttributeRules rules;
    [SerializeField] private CharacterAttributeValues baseValues=CharacterAttributeValues.Uniform(10);
    [SerializeField] private CharacterAttributeValues classBonuses,temporaryBonuses;
    public event Action Changed;
    private bool inspectorDirty;
    [SerializeField, Min(0)] private int unspentPoints;
    [SerializeField, Min(1)] private int highestPointLevel = 1;
    private bool allocating;
    public int UnspentPoints => Math.Max(0, unspentPoints);
    public CharacterAttributeValues BaseValues
    {
        get {var values=baseValues.NonNegative();for(int i=0;i<CharacterAttributeValues.Count;i++)values.Set((CharacterAttribute)i,Math.Min(99,values.Get((CharacterAttribute)i)));return values;}
    }
    public CharacterAttributeValues ClassBonuses => classBonuses;
    public CharacterAttributeValues TemporaryBonuses => temporaryBonuses;
    public CharacterAttributeValues Growth(int level) => rules!=null && !rules.classlessProgression?rules.growthPerLevel.Scale(Math.Max(1,level)-1):new CharacterAttributeValues();
    public CharacterAttributeValues Calculate(int level,CharacterAttributeValues equipment)
        => (BaseValues+Growth(level)+(rules!=null && rules.classlessProgression?new CharacterAttributeValues():classBonuses)+equipment+temporaryBonuses).NonNegative();
    public void SynchronizeLevel(int level)
    {
        if (rules == null || !rules.classlessProgression || level <= highestPointLevel) return;
        long award = (long)(Math.Max(1, level) - Math.Max(1, highestPointLevel)) * Math.Max(0, rules.attributePointsPerLevel);
        highestPointLevel = Math.Max(1, level);
        unspentPoints = (int)Math.Min(int.MaxValue, UnspentPoints + award);
    }
    public bool TryIncreaseBase(CharacterAttribute attribute)
    {
        if (allocating || rules == null || !rules.classlessProgression || UnspentPoints == 0 ||
            (int)attribute < 0 || (int)attribute >= CharacterAttributeValues.Count || BaseValues.Get(attribute) >= 99) return false;
        allocating = true;
        try { unspentPoints--; baseValues.Set(attribute, BaseValues.Get(attribute) + 1); Changed?.Invoke(); return true; }
        finally { allocating = false; }
    }
    public void SetBase(CharacterAttribute attribute,int value)
    {
        value=Math.Min(99,Math.Max(0,value));if(baseValues.Get(attribute)==value)return;
        baseValues.Set(attribute,value);Changed?.Invoke();
    }
    public void SetClassBonuses(CharacterAttributeValues values){classBonuses=values;Changed?.Invoke();}
    public void SetTemporaryBonuses(CharacterAttributeValues values){temporaryBonuses=values;Changed?.Invoke();}
    // Restore only primary base data. Derived growth is recalculated, never written into base.
    public void RestoreBaseValues(CharacterAttributeValues values)
    {
        baseValues=values.NonNegative();
        for(int i=0;i<CharacterAttributeValues.Count;i++)baseValues.Set((CharacterAttribute)i,Math.Min(99,baseValues.Get((CharacterAttribute)i)));
        Changed?.Invoke();
    }
    private void OnValidate(){inspectorDirty=true;}
    private void Update(){if(!inspectorDirty)return;inspectorDirty=false;Changed?.Invoke();}
}

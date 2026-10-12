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
    public CharacterAttributeValues BaseValues => baseValues.NonNegative();
    public CharacterAttributeValues ClassBonuses => classBonuses;
    public CharacterAttributeValues TemporaryBonuses => temporaryBonuses;
    public CharacterAttributeValues Growth(int level) => rules!=null?rules.growthPerLevel.Scale(Math.Max(1,level)-1):new CharacterAttributeValues();
    public CharacterAttributeValues Calculate(int level,CharacterAttributeValues equipment)
        => (BaseValues+Growth(level)+classBonuses+equipment+temporaryBonuses).NonNegative();
    public void SetBase(CharacterAttribute attribute,int value)
    {
        value=Math.Max(0,value);if(baseValues.Get(attribute)==value)return;
        baseValues.Set(attribute,value);Changed?.Invoke();
    }
    public void SetClassBonuses(CharacterAttributeValues values){classBonuses=values;Changed?.Invoke();}
    public void SetTemporaryBonuses(CharacterAttributeValues values){temporaryBonuses=values;Changed?.Invoke();}
    // Restore only primary base data. Derived growth is recalculated, never written into base.
    public void RestoreBaseValues(CharacterAttributeValues values){baseValues=values.NonNegative();Changed?.Invoke();}
    private void OnValidate(){inspectorDirty=true;}
    private void Update(){if(!inspectorDirty)return;inspectorDirty=false;Changed?.Invoke();}
}

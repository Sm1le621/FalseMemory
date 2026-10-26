using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AbilityAvailability { Learned, RequirementsNotMet, NotEnoughPoints, Available }

// Independent learning currency; existing spells remain a compatibility source of known abilities.
[DisallowMultipleComponent,RequireComponent(typeof(BattleUnit))]
public sealed class AbilityLearning : MonoBehaviour
{
    public BattleSpell[] catalog=Array.Empty<BattleSpell>();
    [Min(0)] public int pointsPerLevel=1;
    [SerializeField,Min(0)] private int learningPoints=3;
    [SerializeField,Min(1)] private int highestAwardLevel=1;
    [SerializeField] private List<string> learnedIds=new List<string>();
    private BattleUnit unit;
    private bool learning;
    public event Action Changed;
    public int Points=>Math.Max(0,learningPoints);
    public IReadOnlyList<string> LearnedIds=>learnedIds.AsReadOnly();
    private BattleUnit Unit=>unit!=null?unit:unit=GetComponent<BattleUnit>();
    public bool Knows(BattleSpell ability)=>ability!=null &&
        ((Unit.spells ?? Array.Empty<BattleSpell>()).Contains(ability) ||
         (!string.IsNullOrWhiteSpace(ability.StableId) && learnedIds.Contains(ability.StableId)));
    public IEnumerable<BattleSpell> KnownAbilities=>(Unit.spells ?? Array.Empty<BattleSpell>())
        .Concat((catalog ?? Array.Empty<BattleSpell>()).Where(Knows)).Where(s=>s!=null).Distinct();
    public AbilityAvailability Availability(BattleSpell ability)
    {
        if(Knows(ability))return AbilityAvailability.Learned;
        if(ability==null || string.IsNullOrWhiteSpace(ability.StableId) || !(catalog ?? Array.Empty<BattleSpell>()).Contains(ability) ||
            !ability.MeetsRequirements(Unit.level,Unit.PrimaryValues))return AbilityAvailability.RequirementsNotMet;
        return Points<Math.Max(0,ability.learningCost)?AbilityAvailability.NotEnoughPoints:AbilityAvailability.Available;
    }
    public bool TryLearn(BattleSpell ability)
    {
        if(learning || !Unit.IsAlive || (BattleTransition.Instance!=null && BattleTransition.Instance.EncounterActive) ||
            Availability(ability)!=AbilityAvailability.Available)return false;
        learning=true;
        try {learningPoints-=Math.Max(0,ability.learningCost);learnedIds.Add(ability.StableId);Changed?.Invoke();return true;}
        finally {learning=false;}
    }
    private void SynchronizeLevel()
    {
        if(Unit.level<=highestAwardLevel)return;
        learningPoints=(int)Math.Min(int.MaxValue,(long)Points+(long)(Unit.level-Math.Max(1,highestAwardLevel))*Math.Max(0,pointsPerLevel));
        highestAwardLevel=Unit.level;Changed?.Invoke();
    }
    private void Awake(){unit=GetComponent<BattleUnit>();SynchronizeLevel();}
    private void OnEnable(){Unit.ProgressChanged+=SynchronizeLevel;}
    private void OnDisable(){if(unit!=null)unit.ProgressChanged-=SynchronizeLevel;}
}

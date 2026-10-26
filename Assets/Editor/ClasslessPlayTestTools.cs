using System;
using UnityEditor;
using UnityEngine;

// Temporary session-only progress through the real EXP pipeline, for owner acceptance.
public static class ClasslessPlayTestTools
{
    [MenuItem("FalseMemory/Testing/Grant 60 Levels (Play Mode Only)")]
    public static void GrantTestLevels()
    {
        if(!CanGrant())throw new InvalidOperationException("Requires a living Player in exploration Play Mode.");
        var player=GameObject.Find("Player").GetComponent<BattleUnit>();
        int target=Math.Min(player.growthRules.MaximumLevel,player.level+60);
        long difference=player.growthRules.TotalForLevel(target)-player.CurrentExperience;
        player.AddExperience((int)Math.Max(0,Math.Min(int.MaxValue,difference)));
    }
    [MenuItem("FalseMemory/Testing/Grant 60 Levels (Play Mode Only)",true)]
    private static bool CanGrant()
    {
        if(!EditorApplication.isPlaying || BattleTransition.Instance!=null && BattleTransition.Instance.EncounterActive)return false;
        var go=GameObject.Find("Player");var player=go!=null?go.GetComponent<BattleUnit>():null;
        return player!=null && player.IsAlive && player.growthRules!=null && player.UsesPrimaryAttributes && player.Attributes.rules.classlessProgression;
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "FalseMemory/Battle Rules")]
public sealed class BattleRules : ScriptableObject
{
    [Header("Stage 1: physical combat")]
    [Min(0f)] public float attackMultiplier = 1f;
    [Min(0f)] public float defenseMultiplier = 1f;
    [Min(1)] public int minimumDamage = 1;
    [Range(0f, 1f)] public float defendMultiplier = 0.5f;
    [Range(0f, 1f)] public float enemyDefendChance = 0.2f;
    [Min(0.1f)] public float actionDelay = 0.8f;
    [Min(0f)] public float fadeDuration = 0.3f;
}

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
    [Header("Magic")]
    [Min(0f)] public float magicAttackMultiplier = 1f;
    [Min(0f)] public float magicDefenseMultiplier = 1f;
    [Min(1)] public int minimumMagicDamage = 1;
    [Header("Critical hits")]
    [Min(1f)] public float criticalMultiplier = 1.5f;
    [Header("Escape")]
    [Range(0f, 1f)] public float fleeBaseChance = 0.65f;
    [Min(0f)] public float fleeSpeedCoefficient = 0.03f;
    [Range(0f, 1f)] public float minimumFleeChance = 0.05f;
    [Range(0f, 1f)] public float maximumFleeChance = 0.95f;
    [Min(0.1f)] public float fleeProtectionSeconds = 3f;
    [Min(0.1f)] public float fleeSafeDistance = 2f;
}

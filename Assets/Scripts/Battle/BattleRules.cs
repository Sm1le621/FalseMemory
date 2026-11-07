using UnityEngine;

// Файл з усіма налаштуваннями бою (множники, шанси, затримки).
// Його можна змінювати в інспекторі без зміни коду.
[CreateAssetMenu(menuName = "FalseMemory/Battle Rules")]
public sealed class BattleRules : ScriptableObject
{
    [Header("Stage 1: physical combat")]
    // Множник атаки
    [Min(0f)]
    public float attackMultiplier = 1f;
    // Множник захисту
    [Min(0f)]
    public float defenseMultiplier = 1f;
    // Мінімальна фізична шкода
    [Min(1)]
    public int minimumDamage = 1;
    // На скільки множиться шкода, коли ціль захищається (0.5 = вдвічі менше)
    [Range(0f, 1f)]
    public float defendMultiplier = 0.5f;
    // Шанс, що ворог вибере захист замість атаки
    [Range(0f, 1f)]
    public float enemyDefendChance = 0.2f;
    // Пауза між діями (секунди)
    [Min(0.1f)]
    public float actionDelay = 0.8f;
    // Тривалість затемнення екрана
    [Min(0f)]
    public float fadeDuration = 0.3f;

    [Header("Magic")]
    [Min(0f)]
    public float magicAttackMultiplier = 1f;
    [Min(0f)]
    public float magicDefenseMultiplier = 1f;
    [Min(1)]
    public int minimumMagicDamage = 1;

    [Header("Critical hits")]
    // На скільки множиться шкода при криті
    [Min(1f)]
    public float criticalMultiplier = 1.5f;

    [Header("Escape")]
    // Базовий шанс втечі
    [Range(0f, 1f)]
    public float fleeBaseChance = 0.65f;
    // Скільки додає кожна одиниця різниці швидкостей
    [Min(0f)]
    public float fleeSpeedCoefficient = 0.03f;
    [Range(0f, 1f)]
    public float minimumFleeChance = 0.05f;
    [Range(0f, 1f)]
    public float maximumFleeChance = 0.95f;
    // Скільки секунд після втечі ворог не може знову почати бій
    [Min(0.1f)]
    public float fleeProtectionSeconds = 3f;
    // На яку відстань від ворога ставимо гравця після втечі
    [Min(0.1f)]
    public float fleeSafeDistance = 2f;
}

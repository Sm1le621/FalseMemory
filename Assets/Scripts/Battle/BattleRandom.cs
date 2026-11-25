using System;

// Джерело випадкових чисел для бою.
// У грі використовується звичайний рандом Unity (BattleRandom.Unity).
// У тестах створюється new BattleRandom(seed): з однаковим seed завжди однакові числа,
// і стан UnityEngine.Random при цьому не змінюється.
public sealed class BattleRandom
{
    // Спільний екземпляр зі звичайним рандомом Unity
    public static readonly BattleRandom Unity = new BattleRandom();

    // Рандом із seed. null - значить беремо UnityEngine.Random
    private readonly Random seeded;

    // Звичайний рандом Unity
    private BattleRandom()
    {
        seeded = null;
    }

    // Передбачуваний рандом із seed (для тестів)
    public BattleRandom(int seed)
    {
        seeded = new Random(seed);
    }

    // Повертає випадкове число від 0 до 1
    public float Next01()
    {
        if (seeded == null)
        {
            return UnityEngine.Random.value;
        }
        return (float)seeded.NextDouble();
    }
}

// Результат фізичної атаки: скільки шкоди, чи був промах, чи був крит
public readonly struct PhysicalAttackResult
{
    public int Damage { get; }
    public bool Missed { get; }
    public bool Critical { get; }

    public PhysicalAttackResult(int damage, bool missed, bool critical)
    {
        Damage = damage;
        Missed = missed;
        Critical = critical;
    }
}

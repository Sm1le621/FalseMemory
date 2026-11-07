using System;

// Інтерфейс "джерело випадкових чисел".
// Потрібен, щоб у тестах можна було підставити передбачувані числа.
public interface IBattleRandom
{
    // Повертає випадкове число від 0 до 1
    float Next01();
}

// Звичайний рандом Unity (використовується в грі)
public sealed class UnityBattleRandom : IBattleRandom
{
    // Один спільний екземпляр на всю гру
    public static readonly UnityBattleRandom Instance = new UnityBattleRandom();

    public float Next01()
    {
        return UnityEngine.Random.value;
    }
}

// Рандом із "зерном" (seed): з однаковим seed завжди дає однакові числа.
// Він окремий і НЕ змінює стан UnityEngine.Random.
public sealed class SeededBattleRandom : IBattleRandom
{
    private readonly Random random;

    public SeededBattleRandom(int seed)
    {
        random = new Random(seed);
    }

    public float Next01()
    {
        return (float)random.NextDouble();
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

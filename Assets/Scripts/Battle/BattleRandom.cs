using System;

public interface IBattleRandom { float Next01(); }

public sealed class UnityBattleRandom : IBattleRandom
{
    public static readonly UnityBattleRandom Instance = new UnityBattleRandom();
    public float Next01() => UnityEngine.Random.value;
}

// Local seeded stream: does not change Unity's random state or other mechanics.
public sealed class SeededBattleRandom : IBattleRandom
{
    private readonly Random random;
    public SeededBattleRandom(int seed) => random = new Random(seed);
    public float Next01() => (float)random.NextDouble();
}

public readonly struct PhysicalAttackResult
{
    public int Damage { get; }
    public bool Missed { get; }
    public bool Critical { get; }
    public PhysicalAttackResult(int damage, bool missed, bool critical)
    { Damage = damage; Missed = missed; Critical = critical; }
}

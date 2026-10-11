using UnityEngine;

public interface IBattleParticipant
{
    int Speed { get; }
    bool IsAlive { get; }
}

// Health remains the only owner of HP, in both exploration and battle.
[RequireComponent(typeof(Health))]
public sealed class BattleUnit : MonoBehaviour, IBattleParticipant
{
    public string displayName = "Player";
    [Min(1)] public int level = 1;
    [Min(0)] public int attack = 15;
    [Min(0)] public int defense = 8;
    [Min(0)] public int speed = 10;
    public Sprite battleSprite;
    public Color spriteColor = Color.white;
    public bool flipInBattle;

    public Health Health => GetComponent<Health>();
    public int Speed => Mathf.Max(0, speed);
    public bool IsAlive => Health != null && Health.currentHealth > 0;
    public bool IsDefending { get; private set; }

    public void BeginTurn() => IsDefending = false;
    public void Defend() => IsDefending = true;
    public void ResetBattleState() => IsDefending = false;
}

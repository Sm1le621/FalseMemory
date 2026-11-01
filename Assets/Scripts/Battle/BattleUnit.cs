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
    [Min(0)] public int attack = 15;
    [Min(0)] public int defense = 8;
    [Min(0)] public int speed = 10;
    [Header("Magic")]
    [Min(0)] public int maxMP;
    [SerializeField, Min(0)] private int currentMP;
    [Min(0)] public int magicAttack;
    [Min(0)] public int magicDefense;
    public BattleSpell[] spells = System.Array.Empty<BattleSpell>();
    [Header("Physical attack chances")]
    [Range(0f, 1f)] public float criticalChance = 0.1f;
    [Range(0f, 1f)] public float missChance = 0.05f;
    public Sprite battleSprite;
    public SpriteAnimationSet visualAnimations;
    public Color spriteColor = Color.white;
    public bool flipInBattle;

    public Health Health => GetComponent<Health>();
    public BattleInventory Inventory => GetComponent<BattleInventory>();
    public EquipmentController Equipment => GetComponent<EquipmentController>();
    private ItemStatBonus EquipmentBonuses => Equipment!=null?Equipment.State.Bonuses:new ItemStatBonus();
    public int EffectiveAttack => ItemStatBonus.Sum(attack,EquipmentBonuses.attack);
    public int EffectiveDefense => ItemStatBonus.Sum(defense,EquipmentBonuses.defense);
    public int EffectiveMagicAttack => magicAttack;
    public int EffectiveMagicDefense => magicDefense;
    public int Speed => speed;
    public bool IsAlive => Health != null && Health.currentHealth > 0;
    public bool IsDefending { get; private set; }
    public int MaxMP => maxMP;
    public int CurrentMP => Mathf.Clamp(currentMP, 0, MaxMP);

    private void Awake()
    {
        currentMP = MaxMP;
    }

    public bool TrySpendMP(int cost)
    {
        if (!IsAlive || cost < 0 || CurrentMP < cost) return false;
        currentMP = CurrentMP - cost;
        return true;
    }

    public int RestoreMP(int amount)
    {
        if (!IsAlive || amount <= 0) return 0;
        int before = CurrentMP;
        currentMP = (int)System.Math.Min(MaxMP, (long)before + amount);
        return currentMP - before;
    }

    public void BeginTurn() => IsDefending = false;
    public void Defend() => IsDefending = true;
    public void ResetBattleState() => IsDefending = false;
}

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
    public string localizationId;
    public string LocalizedName=>GameText.Content(localizationId+".name",displayName);
    [Min(1)] public int level = 1;
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
    [Header("Experience")]
    [SerializeField, Min(0)] private int currentExperience;
    [Min(0)] public int experienceReward;
    public Sprite battleSprite;
    public SpriteAnimationSet visualAnimations;
    public Color spriteColor = Color.white;
    public bool flipInBattle;
    public CharacterGrowthRules growthRules;
    public event System.Action ProgressChanged;
    public int LevelsGainedLastAward {get;private set;}
    private int baseMaximumHealth=-1;
    private bool awardingExperience;

    public Health Health => GetComponent<Health>();
    public BattleInventory Inventory => GetComponent<BattleInventory>();
    public EquipmentController Equipment => GetComponent<EquipmentController>();
    private ItemStatBonus EquipmentBonuses => Equipment!=null?Equipment.State.Bonuses:new ItemStatBonus();
    private ItemStatBonus LevelBonuses => growthRules!=null?growthRules.Bonuses(level):new ItemStatBonus();
    public int EffectiveAttack => ItemStatBonus.Sum(ItemStatBonus.Sum(attack,LevelBonuses.attack),EquipmentBonuses.attack);
    public int EffectiveDefense => ItemStatBonus.Sum(ItemStatBonus.Sum(defense,LevelBonuses.defense),EquipmentBonuses.defense);
    public int EffectiveMagicAttack => ItemStatBonus.Sum(ItemStatBonus.Sum(magicAttack,LevelBonuses.magicAttack),EquipmentBonuses.magicAttack);
    public int EffectiveMagicDefense => ItemStatBonus.Sum(ItemStatBonus.Sum(magicDefense,LevelBonuses.magicDefense),EquipmentBonuses.magicDefense);
    public int Speed => ItemStatBonus.Sum(ItemStatBonus.Sum(speed,LevelBonuses.speed),EquipmentBonuses.speed);
    public bool IsAlive => Health != null && Health.currentHealth > 0;
    public bool IsDefending { get; private set; }
    public int MaxMP => ItemStatBonus.Sum(maxMP,growthRules!=null?growthRules.Growth(growthRules.manaPerLevel,level):0);
    public int CurrentMP => Mathf.Clamp(currentMP, 0, MaxMP);
    public int CurrentExperience => Mathf.Max(0, currentExperience);
    public int ExperienceToNextLevel => growthRules!=null?growthRules.Remaining(CurrentExperience,level):0;
    public bool AtMaximumLevel => growthRules!=null && level>=growthRules.MaximumLevel;

    public int AddExperience(int amount)
    {
        if(awardingExperience)return 0;
        LevelsGainedLastAward=0;
        if (!IsAlive || amount <= 0) return 0;
        awardingExperience=true;
        try
        {
            EnsureGrowthBaseline();int before=CurrentExperience;
            currentExperience=(int)System.Math.Min(int.MaxValue,(long)before+amount);
            int gained=currentExperience-before;if(gained==0)return 0;
            if(growthRules!=null){int previous=level;level=growthRules.LevelFor(currentExperience,level);LevelsGainedLastAward=System.Math.Max(0,level-previous);ApplyHealthGrowth();}
            ProgressChanged?.Invoke();return gained;
        }
        finally{awardingExperience=false;}
    }

    private void EnsureGrowthBaseline(){if(baseMaximumHealth<0 && Health!=null)baseMaximumHealth=System.Math.Max(1,Health.maxHealth);}
    private void ApplyHealthGrowth()
    {
        if(growthRules!=null && Health!=null)Health.SetMaximumHealth(ItemStatBonus.Sum(baseMaximumHealth,growthRules.Growth(growthRules.healthPerLevel,level)));
    }
    private void Awake(){EnsureGrowthBaseline();ApplyHealthGrowth();currentMP=MaxMP;}

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

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
    public event System.Action StatsChanged;
    public int LevelsGainedLastAward {get;private set;}
    private int baseMaximumHealth=-1;
    private bool awardingExperience;
    private CharacterAttributes boundAttributes;
    private EquipmentController boundEquipment;

    public Health Health => GetComponent<Health>();
    public BattleInventory Inventory => GetComponent<BattleInventory>();
    public EquipmentController Equipment => GetComponent<EquipmentController>();
    private ItemStatBonus EquipmentBonuses => Equipment!=null?Equipment.State.Bonuses:new ItemStatBonus();
    public CharacterAttributes Attributes => GetComponent<CharacterAttributes>();
    public bool UsesPrimaryAttributes => Attributes!=null && Attributes.rules!=null;
    public CharacterAttributeValues EquipmentAttributes => EquipmentBonuses.attributes;
    public CharacterAttributeValues PrimaryValues => UsesPrimaryAttributes?Attributes.Calculate(growthRules!=null?Mathf.Clamp(level,1,growthRules.MaximumLevel):Mathf.Max(1,level),EquipmentAttributes):new CharacterAttributeValues();
    private ItemStatBonus LevelBonuses => !UsesPrimaryAttributes && growthRules!=null?growthRules.Bonuses(level):new ItemStatBonus();
    private int Derived(int baseValue,int growth,int gear,DerivedAttributeStat stat)
        => (int)System.Math.Max(0,System.Math.Min(int.MaxValue,(long)System.Math.Max(0,baseValue)+growth+gear+(UsesPrimaryAttributes?Attributes.rules.Adjustment(PrimaryValues,stat):0)));
    public int EffectiveAttack => Derived(attack,LevelBonuses.attack,EquipmentBonuses.attack,DerivedAttributeStat.Attack);
    public int EffectiveDefense => Derived(defense,LevelBonuses.defense,EquipmentBonuses.defense,DerivedAttributeStat.Defense);
    public int EffectiveMagicAttack => Derived(magicAttack,LevelBonuses.magicAttack,EquipmentBonuses.magicAttack,DerivedAttributeStat.MagicAttack);
    public int EffectiveMagicDefense => Derived(magicDefense,LevelBonuses.magicDefense,EquipmentBonuses.magicDefense,DerivedAttributeStat.MagicDefense);
    public int Speed => Derived(speed,LevelBonuses.speed,EquipmentBonuses.speed,DerivedAttributeStat.Speed);
    private static float Finite(float value) => float.IsNaN(value)||float.IsInfinity(value)?0:value;
    public float MovementAdjustment => Finite(EquipmentBonuses.movementSpeed)+(UsesPrimaryAttributes?Attributes.rules.MovementAdjustment(PrimaryValues):0);
    public float EffectiveCriticalChance => Mathf.Clamp(Finite(criticalChance)+Finite(EquipmentBonuses.criticalChance)+(UsesPrimaryAttributes?Attributes.rules.CriticalBonus(PrimaryValues):0),0,UsesPrimaryAttributes?Mathf.Clamp01(Attributes.rules.maximumCritical):1);
    public float EffectiveMissChance => Mathf.Clamp01(Finite(missChance)-(UsesPrimaryAttributes?Attributes.rules.AccuracyBonus(PrimaryValues):0));
    public float Evasion => Mathf.Clamp(Finite(EquipmentBonuses.evasion)+(UsesPrimaryAttributes?Attributes.rules.Evasion(PrimaryValues):0),0,UsesPrimaryAttributes?Mathf.Clamp01(Attributes.rules.maximumEvasion):1);
    public int SpellCost(BattleSpell spell) => spell==null?-1:UsesPrimaryAttributes?Attributes.rules.ManaCost(spell.mpCost,PrimaryValues):spell.mpCost;
    public int SpellHealing(BattleSpell spell) => spell==null?0:(int)System.Math.Max(0,System.Math.Min(int.MaxValue,(long)System.Math.Max(0,spell.power)+(UsesPrimaryAttributes?Attributes.rules.HealingBonus(PrimaryValues):0)));
    public bool IsAlive => Health != null && Health.currentHealth > 0;
    public bool IsDefending { get; private set; }
    public int MaxMP => Derived(maxMP,!UsesPrimaryAttributes && growthRules!=null?growthRules.Growth(growthRules.manaPerLevel,level):0,EquipmentBonuses.maximumMana,DerivedAttributeStat.MaximumMana);
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
            if(growthRules!=null){int previous=level;level=growthRules.LevelFor(currentExperience,level);LevelsGainedLastAward=System.Math.Max(0,level-previous);RefreshDerivedStats();}
            ProgressChanged?.Invoke();return gained;
        }
        finally{awardingExperience=false;}
    }

    private void EnsureGrowthBaseline(){if(baseMaximumHealth<0 && Health!=null)baseMaximumHealth=System.Math.Max(1,Health.maxHealth);}
    private void ApplyHealthGrowth()
    {
        if(Health!=null)
            Health.SetMaximumHealth(System.Math.Max(1,Derived(baseMaximumHealth,!UsesPrimaryAttributes && growthRules!=null?growthRules.Growth(growthRules.healthPerLevel,level):0,EquipmentBonuses.maximumHealth,DerivedAttributeStat.MaximumHealth)));
    }
    public void RefreshDerivedStats(){EnsureGrowthBaseline();ApplyHealthGrowth();currentMP=CurrentMP;StatsChanged?.Invoke();}
    private void Awake()
    {
        EnsureGrowthBaseline();ApplyHealthGrowth();currentMP=MaxMP;
        boundAttributes=Attributes;boundEquipment=Equipment;
        if(boundAttributes!=null)boundAttributes.Changed+=RefreshDerivedStats;
        if(boundEquipment!=null)boundEquipment.State.Changed+=RefreshDerivedStats;
    }
    private void OnDestroy()
    {
        if(boundAttributes!=null)boundAttributes.Changed-=RefreshDerivedStats;
        if(boundEquipment!=null)boundEquipment.State.Changed-=RefreshDerivedStats;
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

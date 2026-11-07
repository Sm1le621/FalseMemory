using UnityEngine;

// Все, що потрібно черзі ходів від учасника бою
public interface IBattleParticipant
{
    int Speed { get; }
    bool IsAlive { get; }
}

// Учасник бою (гравець або ворог): характеристики, мана, заклинання, спрайти.
// HP тут НЕ зберігається - воно завжди в компоненті Health (і у світі, і в бою).
[RequireComponent(typeof(Health))]
public sealed class BattleUnit : MonoBehaviour, IBattleParticipant
{
    // Ім'я, яке показується в бою
    public string displayName = "Player";

    [Min(0)]
    public int attack = 15;
    [Min(0)]
    public int defense = 8;
    [Min(0)]
    public int speed = 10;

    [Header("Magic")]
    [Min(0)]
    public int maxMP;
    // Поточна мана (private, щоб змінювати її тільки через методи нижче)
    [SerializeField, Min(0)]
    private int currentMP;
    [Min(0)]
    public int magicAttack;
    [Min(0)]
    public int magicDefense;
    // Список заклинань
    public BattleSpell[] spells = System.Array.Empty<BattleSpell>();

    [Header("Physical attack chances")]
    [Range(0f, 1f)]
    public float criticalChance = 0.1f;
    [Range(0f, 1f)]
    public float missChance = 0.05f;

    // Картинка в бою
    public Sprite battleSprite;
    // Набір кадрів анімацій (рух, атака, біль, смерть)
    public SpriteAnimationSet visualAnimations;
    public Color spriteColor = Color.white;
    // Чи віддзеркалити картинку в бою
    public bool flipInBattle;

    // Компонент здоров'я на цьому ж об'єкті
    public Health Health
    {
        get { return GetComponent<Health>(); }
    }

    // Інвентар (є тільки у гравця, у ворога буде null)
    public BattleInventory Inventory
    {
        get { return GetComponent<BattleInventory>(); }
    }

    // Екіпіровка (є тільки у гравця)
    public EquipmentController Equipment
    {
        get { return GetComponent<EquipmentController>(); }
    }

    // Бонуси від одягнених речей (якщо екіпіровки немає - нулі)
    private ItemStatBonus EquipmentBonuses
    {
        get
        {
            if (Equipment != null)
            {
                return Equipment.State.Bonuses;
            }
            return new ItemStatBonus();
        }
    }

    // Атака з урахуванням зброї
    public int EffectiveAttack
    {
        get { return ItemStatBonus.Sum(attack, EquipmentBonuses.attack); }
    }

    // Захист з урахуванням броні
    public int EffectiveDefense
    {
        get { return ItemStatBonus.Sum(defense, EquipmentBonuses.defense); }
    }

    // На магію речі поки не впливають
    public int EffectiveMagicAttack
    {
        get { return magicAttack; }
    }

    public int EffectiveMagicDefense
    {
        get { return magicDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    // Живий, якщо HP більше 0
    public bool IsAlive
    {
        get { return Health != null && Health.currentHealth > 0; }
    }

    // Чи захищається зараз
    public bool IsDefending { get; private set; }

    public int MaxMP
    {
        get { return maxMP; }
    }

    // Поточна мана, але завжди в межах від 0 до MaxMP
    public int CurrentMP
    {
        get { return Mathf.Clamp(currentMP, 0, MaxMP); }
    }

    private void Awake()
    {
        // На старті мана повна
        currentMP = MaxMP;
    }

    // Витратити ману. Якщо не вистачає - false і нічого не змінюється
    public bool TrySpendMP(int cost)
    {
        if (!IsAlive || cost < 0 || CurrentMP < cost)
        {
            return false;
        }

        currentMP = CurrentMP - cost;
        return true;
    }

    // Відновити ману. Повертає, скільки реально відновилось
    public int RestoreMP(int amount)
    {
        if (!IsAlive || amount <= 0)
        {
            return 0;
        }

        int before = CurrentMP;

        // Рахуємо в long, щоб не було переповнення
        long newMP = (long)before + amount;
        if (newMP > MaxMP)
        {
            newMP = MaxMP;
        }
        currentMP = (int)newMP;

        return currentMP - before;
    }

    // На початку свого ходу захист знімається
    public void BeginTurn()
    {
        IsDefending = false;
    }

    // Стати в захист
    public void Defend()
    {
        IsDefending = true;
    }

    // Скинути бойовий стан (перед боєм і після нього)
    public void ResetBattleState()
    {
        IsDefending = false;
    }
}

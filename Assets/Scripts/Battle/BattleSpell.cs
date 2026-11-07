using UnityEngine;

// Що робить заклинання (поки є тільки магічна шкода)
public enum BattleEffect { MagicDamage }

// На кого діє: на суперника або на себе
public enum BattleTarget { Opponent, Self }

// Файл з даними одного заклинання (створюється через меню Create)
[CreateAssetMenu(menuName = "FalseMemory/Battle Spell")]
public sealed class BattleSpell : ScriptableObject
{
    // Назва в меню
    public string displayName = "Fire";

    // Скільки мани коштує
    [Min(0)]
    public int mpCost = 8;

    public BattleEffect effect = BattleEffect.MagicDamage;
    public BattleTarget target = BattleTarget.Opponent;

    // Сила заклинання (додається до шкоди)
    [Min(0)]
    public int power = 10;

    // Колір тексту/ефекту
    public Color effectColor = new Color(1f, 0.3f, 0.05f);

    // Анімація ефекту
    public BattleVFXProfile visualEffect;
}

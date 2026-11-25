using UnityEngine;

// Файл з даними одного заклинання (створюється через меню Create).
// Заклинання завжди наносить магічну шкоду противнику.
[CreateAssetMenu(menuName = "FalseMemory/Battle Spell")]
public sealed class BattleSpell : ScriptableObject
{
    // Назва в меню
    public string displayName = "Fire";

    // Скільки мани коштує
    [Min(0)]
    public int mpCost = 8;

    // Сила заклинання (додається до шкоди)
    [Min(0)]
    public int power = 10;

    // Колір тексту/ефекту
    public Color effectColor = new Color(1f, 0.3f, 0.05f);

    // Анімація ефекту
    public BattleVFXProfile visualEffect;
}

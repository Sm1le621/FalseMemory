using UnityEngine;

// Витратний предмет для бою (наприклад, зілля HP або MP).
// Успадковується від ItemData, тобто це теж звичайний предмет інвентаря.
[CreateAssetMenu(menuName = "FalseMemory/Battle Consumable")]
public sealed class BattleConsumable : ItemData
{
    // На кого діє предмет
    public BattleTarget target = BattleTarget.Self;

    // Скільки HP відновлює
    [Min(0)]
    public int healing = 30;

    // Скільки MP відновлює
    [Min(0)]
    public int manaRestoration;

    // Візуальний ефект при використанні
    public BattleVFXProfile visualEffect;

    // Колір ефекту (зелений за замовчуванням)
    public Color effectColor = new Color(0.3f, 1f, 0.45f);
}

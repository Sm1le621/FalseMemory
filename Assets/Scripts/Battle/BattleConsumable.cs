using UnityEngine;

// Витратний предмет (наприклад, зілля HP або MP). Використовується з інвентаря на себе.
// Успадковується від ItemData, тобто це теж звичайний предмет інвентаря.
[CreateAssetMenu(menuName = "FalseMemory/Battle Consumable")]
public sealed class BattleConsumable : ItemData
{
    // Скільки HP відновлює
    [Min(0)]
    public int healing = 30;

    // Скільки MP відновлює
    [Min(0)]
    public int manaRestoration;
}

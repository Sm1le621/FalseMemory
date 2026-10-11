using UnityEngine;

[CreateAssetMenu(menuName = "FalseMemory/Battle Consumable")]
public sealed class BattleConsumable : ItemData
{
    public BattleTarget target = BattleTarget.Self;
    [Min(0)] public int healing = 30;
    [Min(0)] public int manaRestoration;
    public Color effectColor = new Color(0.3f, 1f, 0.45f);
}

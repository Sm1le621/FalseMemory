using UnityEngine;

[CreateAssetMenu(menuName = "FalseMemory/Battle Consumable")]
public sealed class BattleConsumable : ScriptableObject
{
    public string displayName = "Potion";
    public BattleTarget target = BattleTarget.Self;
    [Min(0)] public int healing = 30;
    public Color effectColor = new Color(0.3f, 1f, 0.45f);
}

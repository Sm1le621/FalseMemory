using UnityEngine;

public enum BattleEffect { MagicDamage, RestoreHealth }
public enum BattleTarget { Opponent, Self }

[CreateAssetMenu(menuName = "FalseMemory/Battle Spell")]
public sealed class BattleSpell : ScriptableObject
{
    public string displayName = "Fire";
    public string localizationId;
    public string LocalizedName=>GameText.Content(localizationId+".name",displayName);
    [Min(0)] public int mpCost = 8;
    public BattleEffect effect = BattleEffect.MagicDamage;
    public BattleTarget target = BattleTarget.Opponent;
    [Min(0)] public int power = 10;
    public Color effectColor = new Color(1f, 0.3f, 0.05f);
    public BattleVFXProfile visualEffect;
}

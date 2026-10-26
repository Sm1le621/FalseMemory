using UnityEngine;

public enum BattleEffect { MagicDamage, RestoreHealth, PhysicalDamage }
public enum BattleTarget { Opponent, Self }

[CreateAssetMenu(menuName = "FalseMemory/Battle Spell")]
public sealed class BattleSpell : ScriptableObject
{
    public string displayName = "Fire";
    public string localizationId;
    public string id;
    [TextArea] public string description;
    public string StableId=>string.IsNullOrWhiteSpace(id)?localizationId:id;
    public string LocalizedName=>GameText.Content(localizationId+".name",displayName);
    public string LocalizedDescription=>GameText.Content(localizationId+".description",description);
    [Min(1)] public int requiredLevel=1;
    [Min(0)] public int learningCost=1;
    public CharacterAttributeValues requirements;
    public WeaponDamageType damageType=WeaponDamageType.Magic;
    public AttributeScalingEntry[] scaling=System.Array.Empty<AttributeScalingEntry>();
    [Min(0)] public float attackMultiplier=1f;
    [Range(0,1)] public float accuracyBonus;
    [Min(0)] public int holyPower;
    [Min(0)] public int mpCost = 8;
    public BattleEffect effect = BattleEffect.MagicDamage;
    public BattleTarget target = BattleTarget.Opponent;
    [Min(0)] public int power = 10;
    public Color effectColor = new Color(1f, 0.3f, 0.05f);
    public BattleVFXProfile visualEffect;
    public bool MeetsRequirements(int level,CharacterAttributeValues values)
    {
        if(level<System.Math.Max(1,requiredLevel))return false;
        for(int i=0;i<CharacterAttributeValues.Count;i++)if(values.Get((CharacterAttribute)i)<System.Math.Max(0,requirements.Get((CharacterAttribute)i)))return false;
        return true;
    }
    public int ScaledPower(BattleUnit unit)=>WeaponScalingData.Scale(power,unit.PrimaryValues,scaling,unit.UsesPrimaryAttributes?unit.Attributes.rules:null);
}

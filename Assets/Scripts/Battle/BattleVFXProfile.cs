using UnityEngine;

public enum BattleVFXShape { Rising, Orbit, Burst }

[CreateAssetMenu(menuName="FalseMemory/Battle VFX Profile")]
public sealed class BattleVFXProfile : ScriptableObject
{
    public BattleVFXShape shape;
    public Color color = Color.white;
    public Sprite particleSprite;
    [Range(0f,2f)] public float duration=0.45f;
    [Range(4,24)] public int particleCount=12;
    [Range(1f,24f)] public float particleSize=8f;
    [Range(0f,100f)] public float radius=42f;
}

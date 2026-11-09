using UnityEngine;

// Види анімацій. Анімації "стоїть" (Idle) навмисно немає
public enum SpritePose { Move, Attack, Hurt, Death }

// Набір анімацій персонажа. Кадри лежать у .anim кліпах,
// а цей ассет зберігає готові Animator-контролери.
[CreateAssetMenu(menuName = "FalseMemory/Sprite Animation Set")]
public sealed class SpriteAnimationSet : ScriptableObject
{
    // Контролер для світу: кадри малює SpriteRenderer
    public RuntimeAnimatorController worldController;
    // Контролер для бою: кадри малює UI Image
    public RuntimeAnimatorController portraitController;
}

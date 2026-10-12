using UnityEngine;

public enum SpritePose { Idle, Move, Attack, Hurt, Death }

[CreateAssetMenu(menuName="FalseMemory/Sprite Animation Set")]
public sealed class SpriteAnimationSet : ScriptableObject
{
    public Sprite[] idle=System.Array.Empty<Sprite>(), move=System.Array.Empty<Sprite>(),
        attack=System.Array.Empty<Sprite>(), hurt=System.Array.Empty<Sprite>(), death=System.Array.Empty<Sprite>();
    [Range(1f,24f)] public float framesPerSecond=8f;
    [Range(1f,8f)] public float idleFramesPerSecond=2f;
    public Sprite[] Frames(SpritePose pose)
    {
        switch(pose){case SpritePose.Move:return move;case SpritePose.Attack:return attack;
            case SpritePose.Hurt:return hurt;case SpritePose.Death:return death;default:return idle;}
    }
    public Sprite At(SpritePose pose,float progress,Sprite fallback=null)
    {
        var frames=Frames(pose);
        if(frames==null || frames.Length==0)return fallback;
        return frames[Mathf.Min(frames.Length-1,Mathf.FloorToInt(Mathf.Clamp01(progress)*frames.Length))] ?? fallback;
    }
    public Sprite Loop(SpritePose pose,float seconds,Sprite fallback=null)
    {
        var frames=Frames(pose);
        if(frames==null || frames.Length==0)return fallback;
        float rate=pose==SpritePose.Idle?idleFramesPerSecond:framesPerSecond;
        int index=(int)(System.Math.Max(0d,seconds)*Mathf.Clamp(rate,1,24)%frames.Length);
        return frames[index] ?? fallback;
    }
}

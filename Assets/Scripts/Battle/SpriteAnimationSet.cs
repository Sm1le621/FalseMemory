using UnityEngine;

public enum SpritePose { Idle, Move, Attack, Hurt, Death }

[System.Serializable]
public sealed class DirectionalSpriteFrames
{
    public Vector2 direction=Vector2.down;
    public Sprite[] idle=System.Array.Empty<Sprite>(), walk=System.Array.Empty<Sprite>();
}

[CreateAssetMenu(menuName="FalseMemory/Sprite Animation Set")]
public sealed class SpriteAnimationSet : ScriptableObject
{
    public Sprite[] idle=System.Array.Empty<Sprite>(), move=System.Array.Empty<Sprite>(),
        attack=System.Array.Empty<Sprite>(), hurt=System.Array.Empty<Sprite>(), death=System.Array.Empty<Sprite>();
    [Range(1f,24f)] public float framesPerSecond=8f;
    [Range(1f,24f)] public float idleFramesPerSecond=2f;
    public bool useFrameTiming;
    [Range(0f,1f)] public float attackContactProgress=1f;
    public DirectionalSpriteFrames[] worldDirections=System.Array.Empty<DirectionalSpriteFrames>();
    public bool HasWorldDirections
    {
        get { if(worldDirections==null)return false;foreach(var entry in worldDirections)
            if(entry!=null && entry.direction.sqrMagnitude>0 && entry.walk!=null && entry.walk.Length>0)return true;return false; }
    }
    public Sprite AttackAt(float progress,bool recovering,Sprite fallback=null)
    {
        float contact=useFrameTiming && attack!=null && attack.Length>0
            ? AttackContactFrame/(float)attack.Length : Mathf.Clamp01(attackContactProgress);
        return At(SpritePose.Attack,recovering?Mathf.Lerp(contact,1f,Mathf.Clamp01(progress)):Mathf.Lerp(0f,contact,Mathf.Clamp01(progress)),fallback);
    }
    public int AttackContactFrame => attack==null || attack.Length==0 ? 0 :
        Mathf.Min(attack.Length-1,Mathf.FloorToInt(Mathf.Clamp01(attackContactProgress)*attack.Length));
    public float Duration(SpritePose pose)
    {
        var frames=Frames(pose);
        return frames==null ? 0f : frames.Length/Mathf.Clamp(pose==SpritePose.Idle?idleFramesPerSecond:framesPerSecond,1f,24f);
    }
    public float AttackPhaseDuration(bool recovering)
    {
        if(attack==null || attack.Length==0)return 0f;
        return (recovering?attack.Length-AttackContactFrame:AttackContactFrame)/Mathf.Clamp(framesPerSecond,1f,24f);
    }
    public Sprite LoopWorld(Vector2 direction,bool moving,float seconds,Sprite fallback=null)
    {
        DirectionalSpriteFrames selected=null;float best=float.NegativeInfinity;
        if(direction.sqrMagnitude<0.0001f)direction=Vector2.down;
        if(worldDirections!=null)foreach(var entry in worldDirections)
        {
            if(entry==null || entry.direction.sqrMagnitude<0.0001f || entry.walk==null || entry.walk.Length==0)continue;
            float dot=Vector2.Dot(direction.normalized,entry.direction.normalized);
            if(dot>best){best=dot;selected=entry;}
        }
        if(selected==null)return Loop(moving?SpritePose.Move:SpritePose.Idle,seconds,fallback);
        if(!moving && (selected.idle==null || selected.idle.Length==0))return selected.walk[0] ?? fallback;
        return LoopFrames(moving?selected.walk:selected.idle,seconds,moving?framesPerSecond:idleFramesPerSecond,fallback);
    }
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
        float rate=pose==SpritePose.Idle?idleFramesPerSecond:framesPerSecond;
        return LoopFrames(Frames(pose),seconds,rate,fallback);
    }
    private static Sprite LoopFrames(Sprite[] frames,float seconds,float rate,Sprite fallback)
    {
        if(frames==null || frames.Length==0)return fallback;
        int index=(int)(System.Math.Max(0d,seconds)*Mathf.Clamp(rate,1,24)%frames.Length);
        return frames[index] ?? fallback;
    }
}

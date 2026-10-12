using UnityEngine;

// World appearance only: physics, AI and combat remain in their existing owners.
[RequireComponent(typeof(SpriteRenderer),typeof(BattleUnit))]
[DisallowMultipleComponent]
public sealed class EnemySpriteAnimation : MonoBehaviour
{
    [Min(0f)] public float movementThreshold=0.001f;
    [Range(0.05f,0.5f)] public float hurtDuration=0.16f;
    [Range(0.1f,1f)] public float deathDuration=0.4f;
    public bool faceMovement=true;
    [Range(0.01f,0.2f)] public float movementHoldDuration=0.08f;
    private SpriteRenderer visual;
    private BattleUnit unit;
    private Vector3 previousPosition;
    private Vector2 lastDirection=Vector2.down;
    private int previousHP;
    private float clock,hurtUntil,movingUntil,deathStarted=-1f;
    private void Awake(){visual=GetComponent<SpriteRenderer>();unit=GetComponent<BattleUnit>();}
    private void OnEnable(){previousPosition=transform.position;previousHP=unit!=null?unit.Health.currentHealth:0;clock=0;hurtUntil=0;movingUntil=0;deathStarted=-1;}
    private void LateUpdate()
    {
        if(unit==null || visual==null || unit.visualAnimations==null)return;
        var displacement=transform.position-previousPosition;previousPosition=transform.position;clock+=Time.deltaTime;
        bool displaced=displacement.sqrMagnitude>movementThreshold*movementThreshold;
        if(displaced)
        {
            movingUntil=clock+Mathf.Clamp(movementHoldDuration,0.01f,0.2f);
            if(faceMovement)lastDirection=new Vector2(displacement.x,displacement.y).normalized;
        }
        int hp=unit.Health.currentHealth;
        if(hp<previousHP)hurtUntil=clock+hurtDuration;
        previousHP=hp;
        var set=unit.visualAnimations;
        if(hp<=0){if(deathStarted<0)deathStarted=clock;visual.sprite=set.At(SpritePose.Death,(clock-deathStarted)/Mathf.Max(0.1f,deathDuration),unit.battleSprite);return;}
        deathStarted=-1;
        if(clock<hurtUntil){visual.sprite=set.At(SpritePose.Hurt,1-(hurtUntil-clock)/Mathf.Max(0.05f,hurtDuration),unit.battleSprite);return;}
        bool moving=clock<movingUntil;
        if(set.HasWorldDirections)
        {
            visual.flipX=false;
            visual.sprite=set.LoopWorld(lastDirection,moving,clock,unit.battleSprite);
        }
        else
        {
            if(faceMovement && moving && Mathf.Abs(displacement.x)>movementThreshold)visual.flipX=displacement.x<0;
            visual.sprite=set.Loop(moving?SpritePose.Move:SpritePose.Idle,clock,unit.battleSprite);
        }
    }
}

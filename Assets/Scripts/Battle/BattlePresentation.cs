using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Visual poses of the existing UI portraits; no combat resources are written here.
[DisallowMultipleComponent]
public sealed class BattlePresentation : MonoBehaviour
{
    public BattleUI ui;
    public Image playerGuard, enemyGuard;
    [Range(0f, 1f)] public float attackDuration = 0.4f;
    [Range(0f, 1f)] public float hitDuration = 0.18f;
    [Min(0f)] public float lungeDistance = 42f;
    [Min(0f)] public float reactionDistance = 8f;
    [Range(0f,1f)] public float outcomeDuration = 0.35f;
    [Range(0f,2f)] public float messageDuration = 0.75f;
    [Range(0f,2f)] public float enemyThinkDuration = 0.5f;
    public bool allowFastForward = true;
    [Range(1f,6f)] public float fastForwardMultiplier = 3f;
    private Vector2 playerHome, enemyHome;
    private Vector3 playerScale, enemyScale;
    private BattleUnit player, enemy;
    private bool bound;
    private float idleClock;
    private bool playerAttacking,enemyAttacking;
    public int HitEffectsPlayed { get; private set; }
    public bool IsAnimating { get; private set; }
    public float VisualDelta => Time.unscaledDeltaTime *
        (allowFastForward && Keyboard.current != null && Keyboard.current.spaceKey.isPressed
            ? Mathf.Clamp(fastForwardMultiplier,1f,6f) : 1f);

    public IEnumerator ReadPause(bool enemyThinking, float pace)
    {
        float duration = Mathf.Clamp(enemyThinking ? enemyThinkDuration : messageDuration,0f,2f) * Pace(pace);
        for(float t=0;t<duration;t+=VisualDelta) yield return null;
    }

    public void Bind(BattleUnit hero, BattleUnit foe)
    {
        player = hero; enemy = foe;
        if (ui == null || ui.playerPortrait == null || ui.enemyPortrait == null) return;
        playerHome = ui.playerPortrait.rectTransform.anchoredPosition;
        enemyHome = ui.enemyPortrait.rectTransform.anchoredPosition;
        playerScale = ui.playerPortrait.rectTransform.localScale;
        enemyScale = ui.enemyPortrait.rectTransform.localScale;
        bound = true;
    }

    private void LateUpdate()
    {
        if(bound && !IsAnimating){idleClock+=Time.unscaledDeltaTime;IdleFrame(true);IdleFrame(false);}
        if (playerGuard != null) playerGuard.gameObject.SetActive(player != null && player.IsAlive && player.IsDefending);
        if (enemyGuard != null) enemyGuard.gameObject.SetActive(enemy != null && enemy.IsAlive && enemy.IsDefending);
    }

    private void IdleFrame(bool onPlayer)
    {
        var unit=onPlayer?player:enemy;var portrait=ui!=null?(onPlayer?ui.playerPortrait:ui.enemyPortrait):null;
        if(unit!=null && unit.IsAlive && portrait!=null && unit.visualAnimations!=null)
            portrait.sprite=unit.visualAnimations.Loop(SpritePose.Idle,idleClock,unit.battleSprite);
    }
    private void PoseFrame(bool onPlayer,SpritePose pose,float progress)
    {
        var unit=onPlayer?player:enemy;var portrait=ui!=null?(onPlayer?ui.playerPortrait:ui.enemyPortrait):null;
        if(unit!=null && portrait!=null && unit.visualAnimations!=null)portrait.sprite=unit.visualAnimations.At(pose,progress,unit.battleSprite);
    }

    public IEnumerator Windup(bool onPlayer, BattleAction action, float pace)
    {
        if (!isActiveAndEnabled || !bound || ui == null) yield break;
        var portrait = onPlayer ? ui.playerPortrait : ui.enemyPortrait;
        if (portrait == null) yield break;
        IsAnimating = true;
        if(onPlayer)playerAttacking=action==BattleAction.Attack;else enemyAttacking=action==BattleAction.Attack;
        float duration = Mathf.Clamp(attackDuration, 0f, 1f) * 0.5f * Pace(pace);
        Vector2 home = onPlayer ? playerHome : enemyHome;
        float direction = onPlayer ? -1f : 1f;
        Vector2 destination = home + Vector2.right * direction * (action == BattleAction.Attack ? Mathf.Clamp(lungeDistance,0,100) : 0f);
        for (float t=0;t<duration;t+=VisualDelta)
        {
            if (portrait == null) yield break;
            if(action==BattleAction.Attack)PoseFrame(onPlayer,SpritePose.Attack,t/duration);
            portrait.rectTransform.anchoredPosition = Vector2.Lerp(home,destination,Mathf.SmoothStep(0,1,t/duration));
            if (action == BattleAction.Defend) portrait.rectTransform.localScale = Vector3.Scale(onPlayer ? playerScale : enemyScale, new Vector3(1f,1f-0.08f*Mathf.Sin(t/duration*Mathf.PI),1f));
            if (action == BattleAction.Magic) portrait.color=Color.Lerp(onPlayer?player.spriteColor:enemy.spriteColor,new Color(0.78f,0.7f,1f),Mathf.Sin(t/duration*Mathf.PI)*0.7f);
            yield return null;
        }
        if(action==BattleAction.Attack)PoseFrame(onPlayer,SpritePose.Attack,1f);
        if (portrait != null) portrait.rectTransform.anchoredPosition = destination;
    }

    public IEnumerator Recover(bool attackerIsPlayer, bool successfulHit, bool critical, float pace)
    {
        if (!isActiveAndEnabled || !bound || ui == null) { ResetPoses(); yield break; }
        var attacker = attackerIsPlayer ? ui.playerPortrait : ui.enemyPortrait;
        var target = attackerIsPlayer ? ui.enemyPortrait : ui.playerPortrait;
        if (attacker == null || target == null) { ResetPoses(); yield break; }
        Vector2 attackerStart = attacker.rectTransform.anchoredPosition;
        Vector2 attackerHome = attackerIsPlayer ? playerHome : enemyHome;
        Vector2 targetHome = attackerIsPlayer ? enemyHome : playerHome;
        Color targetColor = attackerIsPlayer ? enemy.spriteColor : player.spriteColor;
        if(successfulHit) HitEffectsPlayed++;
        float duration = Mathf.Max(Mathf.Clamp(attackDuration,0,1)*0.5f,Mathf.Clamp(hitDuration,0,1)) * Pace(pace);
        for(float t=0;t<duration;t+=VisualDelta)
        {
            if(attacker == null || target == null) yield break;
            float progress=t/duration;
            if(attackerIsPlayer?playerAttacking:enemyAttacking)PoseFrame(attackerIsPlayer,SpritePose.Attack,1f-progress);
            attacker.rectTransform.anchoredPosition=Vector2.Lerp(attackerStart,attackerHome,progress);
            if(successfulHit)
            {
                PoseFrame(!attackerIsPlayer,SpritePose.Hurt,progress);
                float recoil=Mathf.Sin(progress*Mathf.PI)*Mathf.Clamp(reactionDistance,0,30)*(critical?1.7f:1f);
                target.rectTransform.anchoredPosition=targetHome+Vector2.right*(attackerIsPlayer?-1f:1f)*recoil;
                target.color=progress<0.55f?Color.Lerp(targetColor,Color.white,0.8f):targetColor;
            }
            yield return null;
        }
        ResetPoses();
    }

    // Finite presentation timings also scale with the existing test/action pace.
    private static float Pace(float delay) => Mathf.Clamp(delay/0.8f,0.05f,3f);
    public IEnumerator Finish(BattleState outcome,float pace)
    {
        if(!isActiveAndEnabled || !bound || ui==null)yield break;
        var portrait=outcome==BattleState.Victory?ui.enemyPortrait:ui.playerPortrait;
        if(portrait==null)yield break;
        IsAnimating=true;
        Color color=portrait.color;float initialAlpha=color.a;Vector3 scale=portrait.rectTransform.localScale;Vector2 home=portrait.rectTransform.anchoredPosition;
        float duration=Mathf.Clamp(outcomeDuration,0,1)*Pace(pace);
        for(float t=0;t<duration;t+=VisualDelta)
        {
            float progress=t/duration;color.a=initialAlpha*(1f-progress);portrait.color=color;
            PoseFrame(outcome!=BattleState.Victory,outcome==BattleState.Escaped?SpritePose.Move:SpritePose.Death,progress);
            if(outcome==BattleState.Escaped)portrait.rectTransform.anchoredPosition=home+Vector2.right*progress*40f;
            else portrait.rectTransform.localScale=Vector3.Scale(scale,new Vector3(1f,1f-progress*0.15f,1f));
            yield return null;
        }
        color.a=0f;portrait.color=color;
        PoseFrame(outcome!=BattleState.Victory,outcome==BattleState.Escaped?SpritePose.Move:SpritePose.Death,1f);IsAnimating=false;
    }
    public void ResetPoses()
    {
        IsAnimating = false;
        playerAttacking=enemyAttacking=false;
        IdleFrame(true);IdleFrame(false);
        if(!bound || ui==null) return;
        if(ui.playerPortrait!=null) {ui.playerPortrait.rectTransform.anchoredPosition=playerHome;ui.playerPortrait.rectTransform.localScale=playerScale;ui.playerPortrait.color=player!=null&&player.IsAlive?player.spriteColor:new Color(0.3f,0.3f,0.3f,0.3f);}
        if(ui.enemyPortrait!=null) {ui.enemyPortrait.rectTransform.anchoredPosition=enemyHome;ui.enemyPortrait.rectTransform.localScale=enemyScale;ui.enemyPortrait.color=enemy!=null&&enemy.IsAlive?enemy.spriteColor:new Color(0.3f,0.3f,0.3f,0.3f);}
    }
    private void OnDisable() => ResetPoses();

}

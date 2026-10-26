using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest]
    public IEnumerator FourCardinalIdleAndWalkCyclesShowAllFramesWithoutIdlePhysicalSteps()
    {
        var player=Player();var controller=player.GetComponent<PlayerController>();var animator=player.GetComponent<Animator>();
        var body=player.GetComponent<Rigidbody2D>();var visual=player.GetComponent<SpriteRenderer>();var keyboard=InputSystem.AddDevice<Keyboard>();
        body.position=new Vector2(-30,-30);
        try
        {
            yield return new WaitForSeconds(.15f);
            var keys=new[]{Key.S,Key.A,Key.D,Key.W};var names=new[]{"Down","Left","Right","Up"};
            var directions=new[]{Vector2.down,Vector2.left,Vector2.right,Vector2.up};
            for(int i=0;i<4;i++)
            {
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys[i]));
                yield return WaitUntil(()=>animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DirectionalWalk"));
                string neutral="ApprovedHero_Idle_"+names[i]+"_00",right="ApprovedHero_"+names[i]+"_RightStep",left="ApprovedHero_"+names[i]+"_LeftStep";
                var walk=new System.Collections.Generic.HashSet<string>();
                var changes=new System.Collections.Generic.List<string>();string previous="";float until=Time.time+1.1f;
                while(Time.time<until)
                {
                    yield return new WaitForEndOfFrame();walk.Add(visual.sprite.name);
                    string current=visual.sprite.name;
                    Assert.That(current==neutral || current==right || current==left,Is.True,"Only the requested poses may be shown.");
                    if(current!=previous){changes.Add(current);previous=current;}
                }
                Assert.That(walk.Count,Is.EqualTo(3));
                Assert.That(string.Join("|",changes),Does.Contain(neutral+"|"+right+"|"+left+"|"+neutral),"Real runtime order must be stance / right / left / stance.");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSeconds(.12f);
                var start=body.position;var shown=new System.Collections.Generic.HashSet<string>();until=Time.time+2.2f;
                while(Time.time<until)
                {
                    yield return new WaitForEndOfFrame();shown.Add(visual.sprite.name);
                    Assert.That(visual.sprite.name,Does.StartWith("ApprovedHero_Idle_"+names[i]+"_"));
                    Assert.That(Vector2.Distance(body.position,start),Is.LessThan(.0001f),"Breathing must never move the physics root.");
                }
                Assert.That(shown.Count,Is.EqualTo(4));
                Assert.That(controller.LastDirection,Is.EqualTo(directions[i]));
                Assert.That(animator.GetFloat("WalkSpeed"),Is.Zero,"Idle must animate despite the stopped gait multiplier.");
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DirectionalIdle"),Is.True);
            }
        }
        finally {InputSystem.RemoveDevice(keyboard);}
    }
}

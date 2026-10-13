using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest]
    public IEnumerator IdleDownPlaysThirtyFramesWithoutPhysicalStepsAndResumesAfterWalking()
    {
        var player=Player();var controller=player.GetComponent<PlayerController>();var animator=player.GetComponent<Animator>();
        var body=player.GetComponent<Rigidbody2D>();var visual=player.GetComponent<SpriteRenderer>();var keyboard=InputSystem.AddDevice<Keyboard>();
        body.position=new Vector2(-30,-30);
        try
        {
            yield return new WaitForSeconds(.15f);var start=body.position;
            var shown=new System.Collections.Generic.HashSet<string>();float until=Time.time+4.2f;
            while(Time.time<until)
            {
                yield return null;shown.Add(visual.sprite.name);
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DirectionalIdle"),Is.True);
                Assert.That(Vector2.Distance(body.position,start),Is.LessThan(.0001f),"Breathing must never move the physics root.");
            }
            Assert.That(shown.Count,Is.EqualTo(30));Assert.That(shown,Is.All.StartsWith("ApprovedHero_Idle_Down_"));
            Assert.That(animator.GetFloat("WalkSpeed"),Is.Zero,"Idle must animate despite the stopped gait multiplier.");
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.S));yield return new WaitForSeconds(.2f);
            Assert.That(visual.sprite.name,Does.StartWith("ApprovedHero_Down_"));
            Assert.That(Vector2.Distance(body.position,start),Is.GreaterThan(.3f));
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSeconds(.12f);
            Assert.That(controller.LastDirection,Is.EqualTo(Vector2.down));
            Assert.That(visual.sprite.name,Does.StartWith("ApprovedHero_Idle_Down_"));
            Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DirectionalIdle"),Is.True);
        }
        finally {InputSystem.RemoveDevice(keyboard);}
    }
}

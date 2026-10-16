using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest]
    public IEnumerator WalkRateCandidatesAdvanceAndTurningKeepsCyclePhase()
    {
        var player=Player();var body=player.GetComponent<Rigidbody2D>();var controller=player.GetComponent<PlayerController>();
        var animator=player.GetComponent<Animator>();var keyboard=InputSystem.AddDevice<Keyboard>();
        var reference=typeof(PlayerController).GetField("animationReferenceSpeed",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
        float original=(float)reference.GetValue(controller);body.position=new Vector2(-40,-40);
        try
        {
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.D));yield return new WaitForSeconds(.15f);
            foreach(float fps in new[]{8f,10f,12f,15f,16f})
            {
                reference.SetValue(controller,64f/fps);yield return new WaitForSeconds(.08f);
                float phase=animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                var shown=new System.Collections.Generic.HashSet<Sprite>();
                float start=Time.time,end=start+16f/fps;
                while(Time.time<end){yield return null;shown.Add(player.GetComponent<SpriteRenderer>().sprite);}
                float cycles=animator.GetCurrentAnimatorStateInfo(0).normalizedTime-phase;
                Assert.That(cycles,Is.EqualTo((Time.time-start)*fps/8f).Within(.2f));
                Assert.That(shown.Count,Is.EqualTo(8),"All poses must advance at "+fps+" FPS.");
            }
            float before=animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));yield return new WaitForSeconds(.08f);
            Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DirectionalWalk"),Is.True);
            Assert.That(animator.GetCurrentAnimatorStateInfo(0).normalizedTime,Is.GreaterThan(before),"Turning must retain the ongoing walk phase.");
        }
        finally {reference.SetValue(controller,original);InputSystem.RemoveDevice(keyboard);}
    }

    [UnityTest]
    public IEnumerator MovementRampsMaintainsDiagonalSpeedAndModalStopsImmediately()
    {
        var player=Player();var control=player.GetComponent<PlayerController>();var body=player.GetComponent<Rigidbody2D>();
        var animator=player.GetComponent<Animator>();var keyboard=InputSystem.AddDevice<Keyboard>();
        body.position=new Vector2(-30,-30);
        try
        {
            yield return new WaitForSeconds(.1f);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.D));yield return null;
            yield return new WaitForFixedUpdate();yield return null;
            Assert.That(control.CurrentMovementSpeed,Is.GreaterThan(0f).And.LessThanOrEqualTo(4f));
            yield return new WaitForSeconds(.2f);
            var start=body.position;yield return new WaitForSeconds(.4f);
            float straight=Vector2.Distance(start,body.position);
            Assert.That(straight,Is.InRange(1.45f,1.8f));
            Assert.That(control.ActualMovementSpeed,Is.EqualTo(4f).Within(.015f));
            Assert.That(animator.GetFloat("WalkSpeed"),Is.EqualTo(1f).Within(.015f));
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W,Key.D));yield return new WaitForSeconds(.1f);
            start=body.position;yield return new WaitForSeconds(.4f);
            Assert.That(Vector2.Distance(start,body.position),Is.EqualTo(straight).Within(.15f));
            Assert.That(control.MovementInput.magnitude,Is.EqualTo(1f).Within(.001f));
            player.GetComponent<InventoryWindow>().Open();yield return null;
            start=body.position;yield return new WaitForSeconds(.12f);
            Assert.That(body.position,Is.EqualTo(start));Assert.That(control.CurrentMovementSpeed,Is.Zero);
            Assert.That(animator.GetBool("isMoving"),Is.False);
        }
        finally {InputSystem.RemoveDevice(keyboard);player.GetComponent<InventoryWindow>().Close();}
    }

    [UnityTest]
    public IEnumerator CollisionStopsActualGaitAndBrakingKeepsFacing()
    {
        var player=Player();var body=player.GetComponent<Rigidbody2D>();var control=player.GetComponent<PlayerController>();
        var keyboard=InputSystem.AddDevice<Keyboard>();var wall=new GameObject("MovementTestWall");
        var collider=wall.AddComponent<BoxCollider2D>();collider.size=new Vector2(.1f,10f);
        body.position=new Vector2(-30,-30);wall.transform.position=new Vector3(-29,-30,0);Physics2D.SyncTransforms();
        try
        {
            yield return new WaitForSeconds(.1f);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.D));yield return new WaitForSeconds(.7f);
            Assert.That(body.position.x,Is.LessThan(-29.45f),"Player must not cross a thin wall.");
            Assert.That(control.ActualMovementSpeed,Is.LessThan(.03f));
            Assert.That(player.GetComponent<Animator>().GetBool("isMoving"),Is.False,"Pushing a wall must not run a full-speed gait.");
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.A));yield return new WaitForSeconds(.2f);
            Assert.That(control.ActualMovementSpeed,Is.GreaterThan(3.9f));
            float release=body.position.x;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSeconds(.12f);
            Assert.That(release-body.position.x,Is.InRange(0f,.16f));
            Assert.That(control.CurrentMovementSpeed,Is.Zero);Assert.That(control.LastDirection,Is.EqualTo(Vector2.left));
        }
        finally {InputSystem.RemoveDevice(keyboard);Object.Destroy(wall);}
    }

    [UnityTest]
    public IEnumerator SkeletonChasesWithPhysicsAndStopsAfterTriggerExit()
    {
        var enemy=Enemy();var player=Player();var body=enemy.GetComponent<Rigidbody2D>();
        var ai=enemy.GetComponent<EnemyAI>();var set=enemy.visualAnimations;
        body.position=new Vector2(-30,-30);player.GetComponent<Rigidbody2D>().position=new Vector2(-30,-26);
        Physics2D.SyncTransforms();yield return new WaitForSeconds(.15f);
        var start=body.position;var frames=new System.Collections.Generic.HashSet<Sprite>();
        float end=Time.time+.5f;
        while(Time.time<end){yield return null;frames.Add(enemy.GetComponent<SpriteRenderer>().sprite);}
        Assert.That(body.position.y-start.y,Is.InRange(1.35f,1.7f));
        Assert.That(ai.moveSpeed,Is.EqualTo(3f));Assert.That(frames.Count,Is.GreaterThan(3));
        player.GetComponent<Rigidbody2D>().position=new Vector2(-30,-10);Physics2D.SyncTransforms();
        yield return new WaitForSeconds(.15f);start=body.position;yield return new WaitForSeconds(.15f);
        Assert.That(Vector2.Distance(start,body.position),Is.LessThan(.01f));
        Assert.That(enemy.GetComponent<SpriteRenderer>().sprite,Is.SameAs(set.LoopWorld(Vector2.up,false,0)));
    }
}

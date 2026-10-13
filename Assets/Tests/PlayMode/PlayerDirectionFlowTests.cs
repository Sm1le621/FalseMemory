using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest]
    public IEnumerator EightDirectionsNormalizeInputSelectMotionAndRetainLastFacing()
    {
        var player=Player();var control=player.GetComponent<PlayerController>();
        var animator=player.GetComponent<Animator>();var keyboard=InputSystem.AddDevice<Keyboard>();
        var keys=new[]{new[]{Key.W},new[]{Key.S},new[]{Key.D},new[]{Key.A},
            new[]{Key.W,Key.D},new[]{Key.W,Key.A},new[]{Key.S,Key.D},new[]{Key.S,Key.A}};
        var directions=new[]{Vector2.up,Vector2.down,Vector2.right,Vector2.left,
            new Vector2(1,1).normalized,new Vector2(-1,1).normalized,
            new Vector2(1,-1).normalized,new Vector2(-1,-1).normalized};
        try
        {
            for(int i=0;i<keys.Length;i++)
            {
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys[i]));yield return new WaitForSeconds(.12f);
                Assert.That(Vector2.Distance(control.MovementInput,directions[i]),Is.LessThan(.0001f));
                Assert.That(control.MovementInput.magnitude,Is.EqualTo(1f).Within(.0001f));
                Assert.That(animator.GetFloat("MoveX"),Is.EqualTo(directions[i].x).Within(.0001f));
                Assert.That(animator.GetFloat("MoveY"),Is.EqualTo(directions[i].y).Within(.0001f));
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DirectionalWalk"),Is.True);
                Assert.That(player.GetComponent<SpriteRenderer>().flipX,Is.False);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSeconds(.12f);
                Assert.That(control.MovementInput,Is.EqualTo(Vector2.zero));
                Assert.That(Vector2.Distance(control.LastDirection,directions[i]),Is.LessThan(.0001f));
                Assert.That(animator.GetFloat("LastMoveX"),Is.EqualTo(directions[i].x).Within(.0001f));
                Assert.That(animator.GetFloat("LastMoveY"),Is.EqualTo(directions[i].y).Within(.0001f));
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DirectionalIdle"),Is.True);
                var names=new[]{"Up","Down","Right","Left","UpRight","UpLeft","DownRight","DownLeft"};
                Assert.That(player.GetComponent<SpriteRenderer>().sprite.name,Is.EqualTo("ApprovedHero_"+names[i]+"_00"),"Idle must use the actual direction's approved hero frame.");
            }
            player.GetComponent<InventoryWindow>().Open();
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W,Key.A));yield return null;yield return null;
            Assert.That(control.MovementInput,Is.EqualTo(Vector2.zero));
            Assert.That(control.LastDirection,Is.EqualTo(directions[7]),"Modal block must retain last facing.");
        }
        finally {InputSystem.RemoveDevice(keyboard);player.GetComponent<InventoryWindow>().Close();}
    }

    [UnityTest]
    public IEnumerator ApprovedWalkChangesFramesAndStoppingKeepsDirectionalIdle()
    {
        var player=Player();player.transform.position=new Vector3(-10,-10,0);
        var keyboard=InputSystem.AddDevice<Keyboard>();
        var shown=new System.Collections.Generic.HashSet<string>();
        try
        {
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.S));
            float until=Time.time+.85f;
            while(Time.time<until)
            {
                yield return null;
                shown.Add(player.GetComponent<SpriteRenderer>().sprite.name);
            }
            Assert.That(shown.Count,Is.EqualTo(8),"Walk must actually advance through all eight source poses.");
            Assert.That(shown,Is.All.StartsWith("ApprovedHero_Down_"));
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSeconds(.12f);
            Assert.That(player.GetComponent<SpriteRenderer>().sprite.name,Is.EqualTo("ApprovedHero_Down_00"));
            Assert.That(player.GetComponent<PlayerController>().MovementSpeed,Is.EqualTo(4f).Within(.001f),"Artwork resolution must not change gameplay speed.");
        }
        finally{InputSystem.RemoveDevice(keyboard);}
    }
}

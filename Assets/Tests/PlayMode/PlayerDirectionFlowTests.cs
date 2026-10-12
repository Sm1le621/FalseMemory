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
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys[i]));yield return null;yield return null;
                Assert.That(Vector2.Distance(control.MovementInput,directions[i]),Is.LessThan(.0001f));
                Assert.That(control.MovementInput.magnitude,Is.EqualTo(1f).Within(.0001f));
                Assert.That(animator.GetFloat("MoveX"),Is.EqualTo(directions[i].x).Within(.0001f));
                Assert.That(animator.GetFloat("MoveY"),Is.EqualTo(directions[i].y).Within(.0001f));
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DirectionalWalk"),Is.True);
                Assert.That(player.GetComponent<SpriteRenderer>().flipX,Is.False);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return null;
                Assert.That(control.MovementInput,Is.EqualTo(Vector2.zero));
                Assert.That(Vector2.Distance(control.LastDirection,directions[i]),Is.LessThan(.0001f));
                Assert.That(animator.GetFloat("LastMoveX"),Is.EqualTo(directions[i].x).Within(.0001f));
                Assert.That(animator.GetFloat("LastMoveY"),Is.EqualTo(directions[i].y).Within(.0001f));
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_DirectionalIdle"),Is.True);
                Assert.That(player.GetComponent<SpriteRenderer>().sprite.name,Is.EqualTo("myproject1idle"),"Fallback stays original; this does not prove eight unique views.");
            }
            player.GetComponent<InventoryWindow>().Open();
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W,Key.A));yield return null;yield return null;
            Assert.That(control.MovementInput,Is.EqualTo(Vector2.zero));
            Assert.That(control.LastDirection,Is.EqualTo(directions[7]),"Modal block must retain last facing.");
        }
        finally {InputSystem.RemoveDevice(keyboard);player.GetComponent<InventoryWindow>().Close();}
    }
}

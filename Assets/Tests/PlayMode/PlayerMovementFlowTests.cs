using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator WalkInputDrivesExistingAnimatorStatesAndRestoresIdleSprite()
    {
        var player = Player();
        var controller = player.GetComponent<PlayerController>();
        var animator = player.GetComponent<Animator>();
        var spriteRenderer = player.GetComponent<SpriteRenderer>();
        var window = player.GetComponent<InventoryWindow>();
        Assert.That(controller, Is.Not.Null);
        Assert.That(animator, Is.Not.Null);
        Assert.That(window, Is.Not.Null);
        yield return null;
        Assert.That(animator.enabled, Is.False);
        bool hasIsMoving = HasAnimatorParameter(animator, "isMoving");
        if (hasIsMoving) Assert.That(animator.GetBool("isMoving"), Is.False);
        var idle = spriteRenderer.sprite;

        var keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
            yield return null;
            Assert.That(animator.enabled, Is.True);
            if (hasIsMoving)
            {
                Assert.That(animator.GetBool("isMoving"), Is.True);

                float deadline = Time.realtimeSinceStartup + 2f;
                while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Walk") && Time.realtimeSinceStartup < deadline)
                    yield return null;
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Walk"), Is.True, "isMoving must select the Walk state");
                Assert.That(spriteRenderer.sprite, Is.Not.SameAs(idle), "Walk clip must drive the sprite");
            }

            Assert.That(window.Open(), Is.True);
            yield return null;
            Assert.That(animator.enabled, Is.False, "Opening the inventory must stop the walk animation");
            if (hasIsMoving) Assert.That(animator.GetBool("isMoving"), Is.False);
            Assert.That(spriteRenderer.sprite, Is.SameAs(idle), "Opening the inventory must show the idle sprite");
            window.Close();

            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
            yield return null;
            Assert.That(animator.enabled, Is.True, "Closing the inventory must restore walk animation");
            if (hasIsMoving) Assert.That(animator.GetBool("isMoving"), Is.True);

            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
            Assert.That(animator.enabled, Is.False);
            if (hasIsMoving) Assert.That(animator.GetBool("isMoving"), Is.False);
            Assert.That(spriteRenderer.sprite, Is.SameAs(idle), "Stopping must restore the configured idle sprite");
        }
        finally
        {
            if (window != null) window.Close();
            InputSystem.RemoveDevice(keyboard);
        }
    }

    private static bool HasAnimatorParameter(Animator animator, string parameterName)
    {
        foreach (var parameter in animator.parameters)
            if (parameter.name == parameterName) return true;
        return false;
    }
}

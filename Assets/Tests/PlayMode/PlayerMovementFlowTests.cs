using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

// Тест анімації ходьби гравця (частина класу BattleFlowTests)
public sealed partial class BattleFlowTests
{
    // Рух вмикає аніматор зі станом Walk, а зупинка або відкритий інвентар повертають спрайт "стоїть"
    [UnityTest]
    public IEnumerator WalkInputDrivesExistingAnimatorStatesAndRestoresIdleSprite()
    {
        BattleUnit player = Player();
        PlayerController controller = player.GetComponent<PlayerController>();
        Animator animator = player.GetComponent<Animator>();
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        Assert.That(controller, Is.Not.Null);
        Assert.That(animator, Is.Not.Null);
        Assert.That(window, Is.Not.Null);
        yield return null;

        // Гравець стоїть - аніматор вимкнений
        Assert.That(animator.enabled, Is.False);
        bool hasIsMoving = HasAnimatorParameter(animator, "isMoving");
        if (hasIsMoving)
        {
            Assert.That(animator.GetBool("isMoving"), Is.False);
        }
        Sprite idle = spriteRenderer.sprite;

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            // Тиснемо D - гравець іде
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
            yield return null;
            Assert.That(animator.enabled, Is.True);

            if (hasIsMoving)
            {
                Assert.That(animator.GetBool("isMoving"), Is.True);

                // Чекаємо (до 2 секунд), поки аніматор перейде в стан Player_Walk
                float deadline = Time.realtimeSinceStartup + 2f;
                while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Walk") && Time.realtimeSinceStartup < deadline)
                {
                    yield return null;
                }
                Assert.That(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Walk"), Is.True, "isMoving must select the Walk state");
                Assert.That(spriteRenderer.sprite, Is.Not.SameAs(idle), "Walk clip must drive the sprite");
            }

            // Відкрили інвентар - анімація зупиняється
            Assert.That(window.Open(), Is.True);
            yield return null;
            Assert.That(animator.enabled, Is.False, "Opening the inventory must stop the walk animation");
            if (hasIsMoving)
            {
                Assert.That(animator.GetBool("isMoving"), Is.False);
            }
            Assert.That(spriteRenderer.sprite, Is.SameAs(idle), "Opening the inventory must show the idle sprite");
            window.Close();

            // Закрили - D досі натиснута, анімація повертається
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
            yield return null;
            Assert.That(animator.enabled, Is.True, "Closing the inventory must restore walk animation");
            if (hasIsMoving)
            {
                Assert.That(animator.GetBool("isMoving"), Is.True);
            }

            // Відпустили всі клавіші - стоїмо
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
            Assert.That(animator.enabled, Is.False);
            if (hasIsMoving)
            {
                Assert.That(animator.GetBool("isMoving"), Is.False);
            }
            Assert.That(spriteRenderer.sprite, Is.SameAs(idle), "Stopping must restore the configured idle sprite");
        }
        finally
        {
            if (window != null)
            {
                window.Close();
            }
            InputSystem.RemoveDevice(keyboard);
        }
    }

    // Чи є в аніматорі параметр з таким ім'ям
    private static bool HasAnimatorParameter(Animator animator, string parameterName)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName)
            {
                return true;
            }
        }
        return false;
    }
}

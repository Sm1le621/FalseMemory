using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

// PlayMode-тести переходу між сценами: SampleScene -> Dungeon -> SampleScene.
// Гравець заходить у справжні тригери входу і виходу з печери.
public sealed class SceneTransitionFlowTests
{
    // Скільки секунд чекаємо перехід, перш ніж тест провалиться
    private const float Timeout = 10f;

    // Перед тестом: завантажуємо світ
    [UnitySetUp]
    public IEnumerator Setup()
    {
        yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
        yield return null;
    }

    // Після тесту: прибираємо менеджер переходів і залишаємо порожню сцену,
    // щоб інші тести завантажували світ з чистого стану
    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        if (SceneTransitionManager.Instance != null)
        {
            Object.Destroy(SceneTransitionManager.Instance.gameObject);
        }

        Scene anchor = SceneManager.CreateScene("SceneTransitionTestAnchor");
        SceneManager.SetActiveScene(anchor);

        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene != anchor && scene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }
        yield return null;
    }

    // Вхід у печеру веде в Dungeon, вихід - назад у SampleScene.
    // Гравець той самий, HP зберігається, копій немає, гравець не "відскакує" назад.
    [UnityTest]
    public IEnumerator CaveEntranceAndExitCarrySamePlayerBothWays()
    {
        PlayerController player = Object.FindAnyObjectByType<PlayerController>();
        Assert.That(player, Is.Not.Null);
        Health health = player.GetComponent<Health>();
        health.TakeDamage(10);
        int expectedHealth = health.currentHealth;

        // 1) Заходимо у вхід печери в SampleScene
        SceneTransition entrance = FindTransition("Dungeon");
        Assert.That(entrance, Is.Not.Null, "SampleScene must have a cave entrance");
        MovePlayerInto(player, entrance);
        yield return WaitForScene("Dungeon");

        Assert.That(CountPlayers(), Is.EqualTo(1));
        Assert.That(Object.FindAnyObjectByType<PlayerController>(), Is.SameAs(player));
        Assert.That(health.currentHealth, Is.EqualTo(expectedHealth));
        Assert.That(Vector2.Distance(player.transform.position, Vector2.zero), Is.LessThan(0.05f));

        // 2) Вихід у Dungeon: є, веде у SampleScene, стоїть вище точки появи і не зачіпає гравця
        SceneTransition exit = FindTransition("SampleScene");
        Assert.That(exit, Is.Not.Null, "Dungeon must have a cave exit");
        Collider2D exitCollider = exit.GetComponent<Collider2D>();
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        Assert.That(exit.transform.position.y, Is.GreaterThan(player.transform.position.y));
        Assert.That(exitCollider.bounds.min.y, Is.GreaterThan(playerCollider.bounds.max.y));

        // Вихід виглядає так само, як вхід (той самий спрайт печери)
        SpriteRenderer exitSprite = exit.transform.parent.GetComponent<SpriteRenderer>();
        Assert.That(exitSprite, Is.Not.Null);
        Assert.That(exitSprite.sprite.name, Is.EqualTo("Cave"));

        // Чекаємо, поки мине захист після прибуття, і заходимо у вихід
        yield return new WaitForSecondsRealtime(0.6f);
        MovePlayerInto(player, exit);
        yield return WaitForScene("SampleScene");

        // 3) Знову у світі: той самий гравець, HP те саме, одна копія інтерфейсу
        Assert.That(CountPlayers(), Is.EqualTo(1));
        Assert.That(Object.FindAnyObjectByType<PlayerController>(), Is.SameAs(player));
        Assert.That(health.currentHealth, Is.EqualTo(expectedHealth));
        Assert.That(BattleTransition.Instance, Is.Not.Null);
        Assert.That(BattleTransition.Instance.enabled, Is.True);
        Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length, Is.EqualTo(1));

        // Гравець стоїть під входом і не в його тригері
        SceneTransition entranceAgain = FindTransition("Dungeon");
        Collider2D entranceCollider = entranceAgain.GetComponent<Collider2D>();
        Assert.That(playerCollider.bounds.Intersects(entranceCollider.bounds), Is.False);
        Assert.That(player.transform.position.y, Is.LessThan(entranceAgain.transform.position.y));

        // Кілька фізичних кроків - гравець не повертається в Dungeon сам
        yield return new WaitForSecondsRealtime(0.3f);
        Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("SampleScene"));
    }

    // Знайти тригер переходу, який веде у вказану сцену
    private static SceneTransition FindTransition(string targetScene)
    {
        SceneTransition[] transitions = Object.FindObjectsByType<SceneTransition>(FindObjectsSortMode.None);
        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].TargetSceneName == targetScene)
            {
                return transitions[i];
            }
        }
        return null;
    }

    // Поставити гравця в центр тригера
    private static void MovePlayerInto(PlayerController player, SceneTransition transition)
    {
        Collider2D trigger = transition.GetComponent<Collider2D>();
        Collider2D body = player.GetComponent<Collider2D>();
        // Зсув між центром коллайдера гравця і його позицією
        Vector2 offset = (Vector2)body.bounds.center - (Vector2)player.transform.position;
        Vector2 target = (Vector2)trigger.bounds.center - offset;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.position = target;
        player.transform.position = new Vector3(target.x, target.y, player.transform.position.z);
        Physics2D.SyncTransforms();
    }

    // Скільки гравців зараз є у грі
    private static int CountPlayers()
    {
        return Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None).Length;
    }

    // Чекати, поки сцена стане активною і перехід завершиться
    private static IEnumerator WaitForScene(string sceneName)
    {
        float started = Time.realtimeSinceStartup;
        while (true)
        {
            SceneTransitionManager manager = SceneTransitionManager.Instance;
            bool done = SceneManager.GetActiveScene().name == sceneName;
            if (manager != null && manager.IsTransitioning)
            {
                done = false;
            }
            if (done)
            {
                break;
            }
            if (Time.realtimeSinceStartup - started > Timeout)
            {
                Assert.Fail("Scene '" + sceneName + "' was not loaded in time.");
            }
            yield return null;
        }
        // Ще кадр, щоб копії з нової сцени точно видалились
        yield return null;
    }
}

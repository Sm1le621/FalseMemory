using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// PlayMode-тести: запускають справжню сцену гри і перевіряють бій "вживу".
// Клас partial - його частини розкидані по кількох файлах у цій папці.
public sealed partial class BattleFlowTests
{
    private Scene previousScene;
    private BattleRules rules;
    // Старі значення налаштувань (повертаємо їх після тесту)
    private float previousDelay;
    private float previousFade;
    private float previousDefendChance;
    private InputSettings.BackgroundBehavior savedBackgroundBehavior;
#if UNITY_EDITOR
    private InputSettings.EditorInputBehaviorInPlayMode savedEditorInputBehavior;
#endif

    // Перед кожним тестом: завантажуємо сцену світу і робимо бій швидким і передбачуваним
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Штучні натискання клавіш мають працювати навіть без фокусу на вікні гри.
        // Ці налаштування повертаємо після тесту і НЕ зберігаємо у файл.
        savedBackgroundBehavior = InputSystem.settings.backgroundBehavior;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
#if UNITY_EDITOR
        savedEditorInputBehavior = InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif

        previousScene = SceneManager.GetActiveScene();
        yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive);

        Scene world = SceneManager.GetSceneByName("SampleScene");
        SceneManager.SetActiveScene(world);
        yield return null;

        rules = BattleTransition.Instance.rules;
        previousDelay = rules.actionDelay;
        previousFade = rules.fadeDuration;
        previousDefendChance = rules.enemyDefendChance;

        // Швидкий бій без затемнення, і ворог ніколи не захищається
        rules.actionDelay = 0.1f;
        rules.fadeDuration = 0f;
        rules.enemyDefendChance = 0f;

        // Старі тести не мають залежати від випадкових критів і промахів
        Player().criticalChance = 0f;
        Player().missChance = 0f;
        Enemy().criticalChance = 0f;
        Enemy().missChance = 0f;
    }

    // Після кожного тесту: повертаємо налаштування і вивантажуємо сцени
    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        if (rules != null)
        {
            rules.actionDelay = previousDelay;
            rules.fadeDuration = previousFade;
            rules.enemyDefendChance = previousDefendChance;
        }

        // Рестарт і вихід у меню завантажують сцену в режимі Single, і сцена тест-раннера зникає.
        // Тому створюємо порожню сцену, щоб можна було вивантажити світ.
        if (!previousScene.IsValid() || !previousScene.isLoaded)
        {
            previousScene = SceneManager.CreateScene("BattleFlowTestAnchor");
        }
        SceneManager.SetActiveScene(previousScene);

        Scene battle = SceneManager.GetSceneByName("BattleScene");
        if (battle.IsValid() && battle.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(battle);
        }

        Scene world = SceneManager.GetSceneByName("SampleScene");
        if (world.IsValid() && world.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(world);
        }

        InputSystem.settings.backgroundBehavior = savedBackgroundBehavior;
#if UNITY_EDITOR
        InputSystem.settings.editorInputBehaviorInPlayMode = savedEditorInputBehavior;
#endif
    }

    // Чекаємо, поки умова стане true (але не довше 15 секунд)
    private static IEnumerator WaitUntil(System.Func<bool> predicate)
    {
        float deadline = Time.realtimeSinceStartup + 15f;
        while (!predicate())
        {
            Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline), "Battle flow timed out");
            yield return null;
        }
    }

    // Швидкий доступ до гравця, ворога і менеджера бою
    private static BattleUnit Player()
    {
        return GameObject.Find("Player").GetComponent<BattleUnit>();
    }

    private static BattleUnit Enemy()
    {
        return GameObject.Find("Enemy").GetComponent<BattleUnit>();
    }

    private static BattleManager Manager()
    {
        return Object.FindAnyObjectByType<BattleManager>();
    }

    // Зіткнення починає ОДИН бій, а після перемоги світ і HP залишаються як були
    [UnityTest]
    public IEnumerator ContactStartsOneBattleThenVictoryPreservesWorldAndHealth()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        Health health = player.Health;
        BattleTransition transition = BattleTransition.Instance;

        // Справжнє фізичне зіткнення, а не прямий виклик TryBegin
        Assert.That(player.GetComponent<PlayerController>().enabled, Is.True);
        Vector3 pos = enemy.transform.position;
        player.GetComponent<Rigidbody2D>().position = new Vector2(pos.x, pos.y);
        Physics2D.SyncTransforms();
        yield return WaitUntil(() => transition.EncounterActive);

        // Другий бій почати не можна
        Assert.That(transition.TryBegin(player, enemy), Is.False);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);

        BattleManager manager = Manager();
        Vector3 worldPosition = player.transform.position;
        Assert.That(manager.ui.enemyPortrait.sprite.texture.filterMode, Is.EqualTo(FilterMode.Point));
        Assert.That(player.gameObject.activeInHierarchy, Is.False);
        Assert.That(health.currentHealth, Is.EqualTo(100), "Legacy contact damage must not run");
        Assert.That(manager.Player.Health, Is.SameAs(health));

        // Атака приймається тільки один раз
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.False);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));
        Assert.That(health.currentHealth, Is.EqualTo(96));

        // Захист зменшує шкоду вдвічі
        Assert.That(manager.TryPlayerAction(BattleAction.Defend), Is.True);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(health.currentHealth, Is.EqualTo(94), "Defend halves the enemy's physical attack");
        Assert.That(player.IsDefending, Is.False, "Defend expires at next player turn");

        // Б'ємо, поки не переможемо
        float deadline = Time.realtimeSinceStartup + 15f;
        while (manager.State != BattleState.Victory)
        {
            Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline), "Victory timed out");
            if (manager.State == BattleState.AwaitingPlayer)
            {
                manager.TryPlayerAction(BattleAction.Attack);
            }
            yield return null;
        }

        int remaining = health.currentHealth;
        Assert.That(transition.ReturnAfterVictory(), Is.True);
        yield return WaitUntil(() => !transition.EncounterActive);

        // Світ повернувся, ворог зник, гравець на тому ж місці з тим самим HP
        Assert.That(player.gameObject.activeSelf, Is.True);
        Assert.That(enemy.gameObject.activeSelf, Is.False);
        Assert.That(player.transform.position, Is.EqualTo(worldPosition));
        Assert.That(health.currentHealth, Is.EqualTo(remaining));
        Assert.That(SceneManager.GetSceneByName("BattleScene").isLoaded, Is.False);
        Assert.That(SceneManager.GetSceneByName("SampleScene").isLoaded, Is.True);
        Assert.That(player.GetComponent<PlayerController>().enabled, Is.True);

        // Перевіряємо, що гравець знову може ходити (тиснемо D)
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
            yield return null;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.That(player.transform.position.x, Is.GreaterThan(worldPosition.x), "Exploration movement must resume after victory");
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard);
        }
    }

    // Поразка зупиняє бій і пропонує New Game та Main Menu
    [UnityTest]
    public IEnumerator DefeatStopsTurnsAndOffersRestartAndMenu()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        player.Health.currentHealth = 1;

        Assert.That(BattleTransition.Instance.TryBegin(player, enemy), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);

        BattleManager manager = Manager();
        manager.TryPlayerAction(BattleAction.Attack);
        yield return WaitUntil(() => manager.State == BattleState.Defeat);

        Assert.That(player.Health.currentHealth, Is.Zero);
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.False);
        Assert.That(manager.ui.commandLabels[0].text, Does.Contain("New Game"));
        Assert.That(manager.ui.commandLabels[1].text, Does.Contain("Main Menu"));

        // Ворог більше не ходить
        int enemyHP = enemy.Health.currentHealth;
        yield return new WaitForSecondsRealtime(0.4f);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(enemyHP));

        // New Game - все з нуля
        BattleTransition.Instance.RestartGame();
        yield return WaitUntil(() => SceneManager.GetActiveScene().name == "SampleScene");
        yield return null;
        Assert.That(Player().Health.currentHealth, Is.EqualTo(100));
        Assert.That(Enemy().Health.currentHealth, Is.EqualTo(60));
        Assert.That(Player().GetComponent<PlayerController>().enabled, Is.True);
        Assert.That(BattleTransition.Instance.EncounterActive, Is.False);
    }

    // Затиснутий Enter - це тільки одна дія, на наступному ході він не спрацьовує
    [UnityTest]
    public IEnumerator HeldEnterConsumesOneActionAndDoesNotAttackOnNextTurn()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        Assert.That(BattleTransition.Instance.TryBegin(player, enemy), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);

        BattleManager manager = Manager();
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            yield return null;
            yield return null;
            yield return null;

            // Натискаємо і тримаємо Enter
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return WaitUntil(() => enemy.Health.currentHealth < 60);
            yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
            Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));

            yield return new WaitForSecondsRealtime(0.4f);
            Assert.That(enemy.Health.currentHealth, Is.EqualTo(50), "Holding Enter must not submit a second turn");
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard);
        }
    }

    // Після поразки можна вийти в меню і почати нову гру
    [UnityTest]
    public IEnumerator DefeatCanReturnToMainMenuAndStartNewGame()
    {
        BattleUnit player = Player();
        player.Health.currentHealth = 1;

        Assert.That(BattleTransition.Instance.TryBegin(player, Enemy()), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);

        BattleManager manager = Manager();
        manager.TryPlayerAction(BattleAction.Attack);
        yield return WaitUntil(() => manager.State == BattleState.Defeat);

        BattleTransition.Instance.ReturnToMainMenu();
        yield return WaitUntil(() => SceneManager.GetActiveScene().name == "MainMenu");
        yield return null;
        yield return null;

        MainMenuController menu = Object.FindAnyObjectByType<MainMenuController>();
        Assert.That(menu, Is.Not.Null, "Existing menu bootstrap must still work after assembly extraction");

        menu.OnStart();
        yield return WaitUntil(() => SceneManager.GetActiveScene().name == "SampleScene");
        yield return null;
        Assert.That(Player().Health.currentHealth, Is.EqualTo(100));
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public sealed partial class BattleFlowTests
{
    private Scene previousScene;
    private BattleRules rules;
    private float previousDelay, previousFade, previousDefendChance;
    private InputSettings.BackgroundBehavior savedBackgroundBehavior;
#if UNITY_EDITOR
    private InputSettings.EditorInputBehaviorInPlayMode savedEditorInputBehavior;
#endif

    [UnitySetUp] public IEnumerator Setup()
    {
        // Synthetic keyboard input must work without Game View focus.
        // Restore these transient values after each test; never save the asset.
        savedBackgroundBehavior = InputSystem.settings.backgroundBehavior;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
#if UNITY_EDITOR
        savedEditorInputBehavior = InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
        previousScene = SceneManager.GetActiveScene();
        yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive);
        var world = SceneManager.GetSceneByName("SampleScene"); SceneManager.SetActiveScene(world);
        yield return null;
        rules = BattleTransition.Instance.rules;
        previousDelay = rules.actionDelay; previousFade = rules.fadeDuration; previousDefendChance = rules.enemyDefendChance;
        rules.actionDelay = 0.1f; rules.fadeDuration = 0f; rules.enemyDefendChance = 0f;
        // Existing deterministic scenarios should not depend on random criticals/misses.
        Player().criticalChance = Player().missChance = 0f;
        Enemy().criticalChance = Enemy().missChance = 0f;
    }

    [UnityTearDown] public IEnumerator Cleanup()
    {
        if (rules != null) { rules.actionDelay = previousDelay; rules.fadeDuration = previousFade; rules.enemyDefendChance = previousDefendChance; }
        // Restart/menu intentionally load a single scene, unloading the test runner's
        // original scene. Keep an empty scene loaded so cleanup can unload the world.
        if (!previousScene.IsValid() || !previousScene.isLoaded)
            previousScene = SceneManager.CreateScene("BattleFlowTestAnchor");
        SceneManager.SetActiveScene(previousScene);
        var battle = SceneManager.GetSceneByName("BattleScene");
        if (battle.IsValid() && battle.isLoaded) yield return SceneManager.UnloadSceneAsync(battle);
        var world = SceneManager.GetSceneByName("SampleScene");
        if (world.IsValid() && world.isLoaded) yield return SceneManager.UnloadSceneAsync(world);
        InputSystem.settings.backgroundBehavior = savedBackgroundBehavior;
#if UNITY_EDITOR
        InputSystem.settings.editorInputBehaviorInPlayMode = savedEditorInputBehavior;
#endif
    }

    private static IEnumerator WaitUntil(System.Func<bool> predicate)
    {
        float deadline = Time.realtimeSinceStartup + 15f;
        while (!predicate()) { Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline), "Battle flow timed out"); yield return null; }
    }

    private static BattleUnit Player() => GameObject.Find("Player").GetComponent<BattleUnit>();
    private static BattleUnit Enemy() => GameObject.Find("Enemy").GetComponent<BattleUnit>();
    private static BattleManager Manager() => Object.FindAnyObjectByType<BattleManager>();

    [UnityTest] public IEnumerator ContactStartsOneBattleThenVictoryPreservesWorldAndHealth()
    {
        var player = Player(); var enemy = Enemy(); var health = player.Health;
        var transition = BattleTransition.Instance;
        // Actual physics collision, not a direct encounter call.
        Assert.That(player.GetComponent<PlayerController>().enabled, Is.True);
        var pos = enemy.transform.position;
        player.GetComponent<Rigidbody2D>().position = new Vector2(pos.x, pos.y);
        Physics2D.SyncTransforms();
        yield return WaitUntil(() => transition.EncounterActive);
        Assert.That(transition.TryBegin(player, enemy), Is.False);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);
        var manager = Manager(); var worldPosition = player.transform.position;
        Assert.That(manager.ui.enemyPortrait.sprite.texture.filterMode, Is.EqualTo(FilterMode.Point));
        Assert.That(player.gameObject.activeInHierarchy, Is.False);
        Assert.That(health.currentHealth, Is.EqualTo(100), "Legacy contact damage must not run");
        Assert.That(manager.Player.Health, Is.SameAs(health));
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.False);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));
        Assert.That(health.currentHealth, Is.EqualTo(96));
        Assert.That(manager.TryPlayerAction(BattleAction.Defend), Is.True);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(health.currentHealth, Is.EqualTo(94), "Defend halves the enemy's physical attack");
        Assert.That(player.IsDefending, Is.False, "Defend expires at next player turn");
        float deadline = Time.realtimeSinceStartup + 15f;
        while (manager.State != BattleState.Victory)
        {
            Assert.That(Time.realtimeSinceStartup, Is.LessThan(deadline), "Victory timed out");
            if (manager.State == BattleState.AwaitingPlayer) manager.TryPlayerAction(BattleAction.Attack);
            yield return null;
        }
        int remaining = health.currentHealth;
        Assert.That(transition.ReturnAfterVictory(), Is.True);
        yield return WaitUntil(() => !transition.EncounterActive);
        Assert.That(player.gameObject.activeSelf, Is.True); Assert.That(enemy.gameObject.activeSelf, Is.False);
        Assert.That(player.transform.position, Is.EqualTo(worldPosition));
        Assert.That(health.currentHealth, Is.EqualTo(remaining));
        Assert.That(SceneManager.GetSceneByName("BattleScene").isLoaded, Is.False);
        Assert.That(SceneManager.GetSceneByName("SampleScene").isLoaded, Is.True);
        Assert.That(player.GetComponent<PlayerController>().enabled, Is.True);
        var keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
            yield return null;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.That(player.transform.position.x, Is.GreaterThan(worldPosition.x), "Exploration movement must resume after victory");
        }
        finally { InputSystem.RemoveDevice(keyboard); }
    }

    [UnityTest] public IEnumerator DefeatStopsTurnsAndOffersRestartAndMenu()
    {
        var player = Player(); var enemy = Enemy();
        player.Health.currentHealth = 1;
        Assert.That(BattleTransition.Instance.TryBegin(player, enemy), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);
        var manager = Manager(); manager.TryPlayerAction(BattleAction.Attack);
        yield return WaitUntil(() => manager.State == BattleState.Defeat);
        Assert.That(player.Health.currentHealth, Is.Zero);
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.False);
        Assert.That(manager.ui.commandLabels[0].text, Does.Contain("Начать заново"));
        Assert.That(manager.ui.commandLabels[1].text, Does.Contain("Главное меню"));
        int enemyHP = enemy.Health.currentHealth;
        yield return new WaitForSecondsRealtime(0.4f);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(enemyHP));
        BattleTransition.Instance.RestartGame();
        yield return WaitUntil(() => SceneManager.GetActiveScene().name == "SampleScene");
        yield return null;
        Assert.That(Player().Health.currentHealth, Is.EqualTo(100));
        Assert.That(Enemy().Health.currentHealth, Is.EqualTo(60));
        Assert.That(Player().GetComponent<PlayerController>().enabled, Is.True);
        Assert.That(BattleTransition.Instance.EncounterActive, Is.False);
    }

    [UnityTest] public IEnumerator HeldEnterConsumesOneActionAndDoesNotAttackOnNextTurn()
    {
        var player = Player(); var enemy = Enemy();
        Assert.That(BattleTransition.Instance.TryBegin(player, enemy), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);
        var manager = Manager();
        var keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            yield return null; yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return WaitUntil(() => enemy.Health.currentHealth < 60);
            yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
            Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));
            yield return new WaitForSecondsRealtime(0.4f);
            Assert.That(enemy.Health.currentHealth, Is.EqualTo(50), "Holding Enter must not submit a second turn");
        }
        finally { InputSystem.RemoveDevice(keyboard); }
    }

    [UnityTest] public IEnumerator DefeatCanReturnToMainMenuAndStartNewGame()
    {
        var player = Player(); player.Health.currentHealth = 1;
        Assert.That(BattleTransition.Instance.TryBegin(player, Enemy()), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);
        var manager = Manager(); manager.TryPlayerAction(BattleAction.Attack);
        yield return WaitUntil(() => manager.State == BattleState.Defeat);
        BattleTransition.Instance.ReturnToMainMenu();
        yield return WaitUntil(() => SceneManager.GetActiveScene().name == "MainMenu");
        yield return null; yield return null;
        var menu = Object.FindAnyObjectByType<MainMenuController>();
        Assert.That(menu, Is.Not.Null, "Existing menu bootstrap must still work after assembly extraction");
        menu.OnStart();
        yield return WaitUntil(() => SceneManager.GetActiveScene().name == "SampleScene");
        yield return null;
        Assert.That(Player().Health.currentHealth, Is.EqualTo(100));
    }
}

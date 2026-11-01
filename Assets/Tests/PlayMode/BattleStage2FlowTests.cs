using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    private static BattleSpell Fire(BattleUnit player) => player.spells.First(s => s.effect == BattleEffect.MagicDamage);
    private static BattleConsumable Potion(BattleUnit player) => player.Inventory.Definitions.OfType<BattleConsumable>().First();
    private static ItemData Sword(BattleUnit player) => player.Inventory.State.Definitions.First(i => i.category == ItemCategory.Weapon);
    private static ItemData Armor(BattleUnit player) => player.Inventory.State.Definitions.First(i => i.category == ItemCategory.Armor);
    private sealed class ConstantBattleRandom : IBattleRandom
    {
        private readonly float value;
        public ConstantBattleRandom(float value) => this.value = value;
        public float Next01() => value;
    }

    private static IEnumerator BeginBattle(BattleUnit player, BattleUnit enemy)
    {
        Assert.That(BattleTransition.Instance.TryBegin(player, enemy), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);
        yield return null; yield return null; yield return null;
    }

    [UnityTest] public IEnumerator FailedFleeConsumesOneTurnAndDoesNotSpendResources()
    {
        var player = Player(); var enemy = Enemy();
        yield return BeginBattle(player, enemy); var manager = Manager(); manager.SetRandomSource(new ConstantBattleRandom(1f));
        Assert.That(manager.TryPlayerAction(BattleAction.Flee), Is.True);
        Assert.That(manager.TryPlayerAction(BattleAction.Flee), Is.False);
        yield return WaitUntil(() => manager.ui.messageText.text.Contains("Couldn't escape!"));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(manager.Round, Is.EqualTo(2)); Assert.That(player.Health.currentHealth, Is.EqualTo(96));
        Assert.That(player.CurrentMP, Is.EqualTo(30)); Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
    }

    [UnityTest] public IEnumerator SuccessfulFleePreservesResourcesAndEnemyThenProtectsAndAllowsAnotherBattle()
    {
        var player = Player(); var enemy = Enemy(); var transition = BattleTransition.Instance;
        player.Health.TakeDamage(20); player.TrySpendMP(8); player.Inventory.TryConsume(Potion(player));
        enemy.Health.TakeDamage(10);
        player.GetComponent<Rigidbody2D>().position = enemy.transform.position; Physics2D.SyncTransforms();
        yield return BeginBattle(player, enemy); var manager = Manager(); var enemyPosition = enemy.transform.position;
        manager.SetRandomSource(new ConstantBattleRandom(0f));
        Assert.That(manager.TryPlayerAction(BattleAction.Flee), Is.True);
        yield return WaitUntil(() => manager.State == BattleState.Escaped);
        Assert.That(manager.ui.messageText.text, Does.Contain("Escaped!"));
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
        yield return WaitUntil(() => !transition.EncounterActive);
        Assert.That(enemy.gameObject.activeSelf, Is.True); Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));
        Assert.That(enemy.transform.position, Is.EqualTo(enemyPosition));
        Assert.That(player.Health.currentHealth, Is.EqualTo(80)); Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(2));
        Assert.That(player.GetComponent<PlayerController>().enabled, Is.True);
        Assert.That(transition.IsEncounterProtected, Is.True); Assert.That(transition.TryBegin(player, enemy), Is.False);
        Assert.That(Vector2.Distance(player.transform.position, enemy.transform.position), Is.GreaterThanOrEqualTo(rules.fleeSafeDistance - 0.01f));
        var collider = player.GetComponent<Collider2D>();
        Assert.That(Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0f).Any(c => !c.isTrigger && c.gameObject != player.gameObject), Is.False);
        yield return WaitUntil(() => !transition.IsEncounterProtected);
        Assert.That(transition.TryBegin(player, enemy), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);
        Assert.That(Manager().Player.CurrentMP, Is.EqualTo(22));
        Assert.That(Manager().Player.Inventory.Count(Potion(player)), Is.EqualTo(2));
        Assert.That(Manager().Enemy.Health.currentHealth, Is.EqualTo(50));
    }

    [UnityTest] public IEnumerator VictoryPreservesAllSessionStateAndShowsResults()
    {
        var player = Player(); var enemy = Enemy(); var transition = BattleTransition.Instance;
        player.Health.TakeDamage(20);
        yield return BeginBattle(player, enemy); var manager = Manager();
        manager.TryPlayerSpell(Fire(player)); yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        manager.TryPlayerSpell(Fire(player)); yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        while (manager.State == BattleState.AwaitingPlayer)
        {
            manager.TryPlayerAction(BattleAction.Attack);
            yield return WaitUntil(() => manager.State != BattleState.Resolving);
        }
        Assert.That(manager.State, Is.EqualTo(BattleState.Victory));
        Assert.That(player.CurrentMP, Is.EqualTo(14)); Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        Assert.That(player.Health.currentHealth, Is.InRange(1, 99));
        Assert.That(manager.ui.resultPanel.gameObject.activeSelf, Is.True);
        Assert.That(manager.ui.resultText.text, Does.Contain("VICTORY").And.Not.Contain("EXP"));
        Assert.That(manager.TryPlayerAction(BattleAction.Flee), Is.False);
        yield return new WaitForSecondsRealtime(0.3f);
        int health = player.Health.currentHealth;
        Assert.That(transition.ReturnAfterVictory(), Is.True); Assert.That(transition.ReturnAfterVictory(), Is.False);
        yield return WaitUntil(() => !transition.EncounterActive);
        Assert.That(player.Health.currentHealth, Is.EqualTo(health)); Assert.That(player.CurrentMP, Is.EqualTo(14));
        Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        Assert.That(enemy.gameObject.activeSelf, Is.False);
        Assert.That(player.Health.hpSlider.value, Is.EqualTo((float)health));
        Assert.That(transition.TryBegin(player, enemy), Is.False);
    }

    [UnityTest] public IEnumerator TargetDeathBeforeQueuedSpellCancelsManaSpendAndFurtherActions()
    {
        var player = Player(); var enemy = Enemy();
        yield return BeginBattle(player, enemy); var manager = Manager();
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.True);
        enemy.Health.TakeDamage(60);
        yield return WaitUntil(() => manager.State == BattleState.Victory);
        Assert.That(player.CurrentMP, Is.EqualTo(30)); Assert.That(player.Health.currentHealth, Is.EqualTo(100));
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
    }

    [UnityTest] public IEnumerator HoldingEnterAcrossMagicMenuDoesNotCastUntilASeparatePress()
    {
        var player = Player(); var enemy = Enemy();
        yield return BeginBattle(player, enemy); var manager = Manager();
        var keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.DownArrow)); yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return new WaitForSecondsRealtime(0.4f);
            Assert.That(manager.ui.commandLabels[0].text, Does.Contain("Fire"));
            Assert.That(player.CurrentMP, Is.EqualTo(30)); Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return WaitUntil(() => player.CurrentMP == 22);
            yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(player.CurrentMP, Is.EqualTo(22)); Assert.That(enemy.Health.currentHealth, Is.EqualTo(41));
        }
        finally { InputSystem.RemoveDevice(keyboard); }
    }

    [UnityTest] public IEnumerator ExpandedSpellListScrollsAndBackDoesNotSpendResources()
    {
        var player = Player(); var enemy = Enemy(); var originalSpells = player.spells;
        var extraSpells = Enumerable.Range(0, 6).Select(i => ScriptableObject.CreateInstance<BattleSpell>()).ToArray();
        var keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            player.spells = extraSpells;
            yield return BeginBattle(player, enemy); var manager = Manager(); manager.OpenMagicMenu();
            yield return null; yield return null; yield return null;
            Assert.That(manager.ui.MenuCount, Is.EqualTo(7));
            for (int i = 0; i < 6; i++)
            {
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.DownArrow)); yield return null; yield return null;
                InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
            }
            Assert.That(manager.ui.SelectedIndex, Is.EqualTo(6));
            Assert.That(manager.ui.commandLabels[4].text, Does.Contain("Back"));
            manager.ui.commandButtons[4].onClick.Invoke();
            Assert.That(manager.ui.MenuCount, Is.EqualTo(4)); Assert.That(manager.Round, Is.EqualTo(1));
            Assert.That(player.CurrentMP, Is.EqualTo(30)); Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        }
        finally
        {
            player.spells = originalSpells; InputSystem.RemoveDevice(keyboard);
            foreach (var spell in extraSpells) Object.Destroy(spell);
        }
    }

    [UnityTest] public IEnumerator DeathWhileSelectingCancelsPendingTurnAndRejectsActions()
    {
        var player = Player(); var enemy = Enemy();
        yield return BeginBattle(player, enemy); var manager = Manager();
        player.Health.TakeDamage(100);
        yield return WaitUntil(() => manager.State == BattleState.Defeat);
        Assert.That(manager.ui.playerStats.text, Does.Contain("HP 0 / 100"));
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.False);
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
        Assert.That(player.CurrentMP, Is.EqualTo(30)); Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
    }

    [UnityTest] public IEnumerator CriticalAndMissHaveCorrectDamageMessagesAndEachSpendOneTurn()
    {
        var player = Player(); var enemy = Enemy();
        yield return BeginBattle(player, enemy); var manager = Manager(); manager.SetRandomSource(new ConstantBattleRandom(0.5f));
        player.criticalChance = 1f;
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        yield return WaitUntil(() => enemy.Health.currentHealth == 45);
        Assert.That(manager.ui.messageText.text, Does.Contain("CRITICAL!"));
        Assert.That(manager.ui.feedbackText.text, Does.Contain("CRITICAL!"));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(manager.Round, Is.EqualTo(2)); Assert.That(player.Health.currentHealth, Is.EqualTo(96));
        player.missChance = 1f;
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        yield return WaitUntil(() => manager.ui.messageText.text.Contains("MISS"));
        Assert.That(manager.ui.feedbackText.text, Is.EqualTo("MISS"));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(45)); Assert.That(player.Health.currentHealth, Is.EqualTo(92));
        Assert.That(manager.Round, Is.EqualTo(3));
    }

    [UnityTest] public IEnumerator FireConsumesMPOnceAndReturnsWithSameHealthAndMana()
    {
        var player = Player(); var enemy = Enemy(); var transition = BattleTransition.Instance;
        player.criticalChance = player.missChance = 1f; // Fire ignores physical critical/miss chances.
        yield return BeginBattle(player, enemy); var manager = Manager();
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.True);
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP, Is.EqualTo(22)); Assert.That(enemy.Health.currentHealth, Is.EqualTo(41));
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));
        enemy.Health.currentHealth = 1;
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.True);
        yield return WaitUntil(() => manager.State == BattleState.Victory);
        Assert.That(player.CurrentMP, Is.EqualTo(14));
        Assert.That(transition.ReturnAfterVictory(), Is.True);
        yield return WaitUntil(() => !transition.EncounterActive);
        Assert.That(player.CurrentMP, Is.EqualTo(14)); Assert.That(player.Health.currentHealth, Is.EqualTo(96));
        Assert.That(enemy.gameObject.activeSelf, Is.False);
    }

    [UnityTest] public IEnumerator InsufficientManaDoesNotConsumeTurnOrSpendMP()
    {
        var player = Player(); player.Health.TakeDamage(5);
        yield return BeginBattle(player, Enemy()); var manager = Manager();
        player.TrySpendMP(24); int round = manager.Round;
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
        yield return new WaitForSecondsRealtime(0.25f);
        Assert.That(manager.State, Is.EqualTo(BattleState.AwaitingPlayer)); Assert.That(manager.Round, Is.EqualTo(round));
        Assert.That(player.CurrentMP, Is.EqualTo(6));
        Assert.That(manager.TryPlayerAction(BattleAction.Defend), Is.True);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP, Is.EqualTo(6)); Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
    }

    [UnityTest] public IEnumerator MagicEscapeAndDisabledSpellDoNotSpendTurnOrDoubleUseMana()
    {
        var player = Player(); var enemy = Enemy();
        yield return BeginBattle(player, enemy); var manager = Manager(); int round = manager.Round;
        Assert.That(manager.OpenMagicMenu(), Is.True);
        var keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            yield return null; yield return null; yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Escape));
            yield return null; yield return null;
            Assert.That(manager.ui.commandLabels[0].text, Does.Contain("Attack"));
            Assert.That(manager.Round, Is.EqualTo(round)); Assert.That(player.CurrentMP, Is.EqualTo(30));
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null;
            player.TrySpendMP(23); manager.OpenMagicMenu();
            yield return null; yield return null; yield return null;
            Assert.That(manager.ui.commandButtons[0].interactable, Is.False);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.That(manager.State, Is.EqualTo(BattleState.AwaitingPlayer)); Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
        }
        finally { InputSystem.RemoveDevice(keyboard); }
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator TargetDeathDuringWindupCancelsDamageAndManaSpend()
    {
        var player=Player();var enemy=Enemy();yield return BeginBattle(player,enemy);
        var manager=Manager();manager.presentation.attackDuration=1f;
        Assert.That(manager.TryPlayerSpell(Fire(player)),Is.True);
        yield return WaitUntil(()=>manager.presentation.IsAnimating);
        Assert.That(manager.TryPlayerSpell(Fire(player)),Is.False);
        enemy.Health.TakeDamage(60);
        yield return WaitUntil(()=>manager.State==BattleState.Victory);
        Assert.That(player.CurrentMP,Is.EqualTo(30));Assert.That(player.CurrentExperience,Is.EqualTo(20));
        Assert.That(manager.presentation.IsAnimating,Is.False);
    }

    [UnityTest] public IEnumerator MissingPresentationDoesNotBlockOrDuplicatePhysicalAction()
    {
        var player=Player();var enemy=Enemy();yield return BeginBattle(player,enemy);
        var manager=Manager();manager.presentation=null;
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.True);
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.False);
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth,Is.EqualTo(50));Assert.That(player.Health.currentHealth,Is.EqualTo(96));
        Assert.That(manager.Round,Is.EqualTo(2));
    }

    [UnityTest] public IEnumerator HUDAnimatesReadOnlyBarsWithoutChangingResources()
    {
        var player = Player(); var enemy = Enemy();
        yield return BeginBattle(player, enemy);
        var manager = Manager(); var hud = manager.ui.hud;
        Assert.That(hud, Is.Not.Null);
        Assert.That(hud.hpBar.interactable || hud.mpBar.interactable || hud.enemyHPBar.interactable, Is.False);
        player.Health.TakeDamage(80); player.TrySpendMP(8);
        manager.ui.RefreshStats(player, enemy, manager.Round);
        yield return new WaitForSecondsRealtime(0.35f);
        Assert.That(player.Health.currentHealth, Is.EqualTo(20)); Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(hud.hpBar.value, Is.EqualTo(0.2f).Within(0.001f));
        Assert.That(hud.mpBar.value, Is.EqualTo(22f/30f).Within(0.001f));
        Assert.That(hud.lowHealthText.gameObject.activeSelf, Is.True);
        Assert.That(manager.ui.playerStats.text, Does.Contain("HP  20 / 100"));
        manager.OpenMagicMenu(); manager.ShowMainMenu();
        yield return new WaitForSecondsRealtime(0.2f);
        Assert.That(hud.menuGroup.alpha, Is.EqualTo(1f));
        Assert.That(manager.State, Is.EqualTo(BattleState.AwaitingPlayer));
        Assert.That(player.CurrentMP, Is.EqualTo(22));
    }
}

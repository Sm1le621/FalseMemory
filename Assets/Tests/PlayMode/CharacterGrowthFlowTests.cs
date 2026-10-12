using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator VictoryLevelsOnceAndPreservesProgressResourcesEquipmentAndPosition()
    {
        var player=Player();var enemy=Enemy();var inventory=player.Inventory.State;var equipment=player.Equipment.State;
        Assert.That(player.growthRules,Is.Not.Null);player.AddExperience(90);player.Health.TakeDamage(20);player.TrySpendMP(8);
        var position=player.transform.position;enemy.Health.TakeDamage(59);yield return BeginBattle(player,enemy);var manager=Manager();
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.True);yield return WaitUntil(()=>manager.State==BattleState.Victory);
        Assert.That(player.level,Is.EqualTo(2));Assert.That(player.CurrentExperience,Is.EqualTo(110));Assert.That(player.ExperienceToNextLevel,Is.EqualTo(140));
        Assert.That(player.Health.maxHealth,Is.EqualTo(105));Assert.That(player.Health.currentHealth,Is.EqualTo(80));Assert.That(player.MaxMP,Is.EqualTo(32));Assert.That(player.CurrentMP,Is.EqualTo(22));
        Assert.That(player.EffectiveAttack,Is.EqualTo(17));Assert.That(manager.ui.resultText.text,Does.Contain("Level Up!").And.Contain("Level 2").And.Contain("140 EXP"));
        int reward;Assert.That(BattleTransition.Instance.TryGrantVictoryExperience(out reward),Is.False);Assert.That(player.CurrentExperience,Is.EqualTo(110));
        Assert.That(BattleTransition.Instance.ReturnAfterVictory(),Is.True);yield return WaitUntil(()=>!UnityEngine.SceneManagement.SceneManager.GetSceneByName("BattleScene").isLoaded);
        yield return WaitUntil(()=>!BattleTransition.Instance.EncounterActive && !BattleTransition.Instance.IsTransitioning);
        Assert.That(player.transform.position,Is.EqualTo(position));Assert.That(player.level,Is.EqualTo(2));Assert.That(player.Inventory.State,Is.SameAs(inventory));Assert.That(player.Equipment.State,Is.SameAs(equipment));
        Assert.That(player.Health.hpText.text,Is.EqualTo("80 / 105"));var window=player.GetComponent<InventoryWindow>();Assert.That(window.Open(),Is.True);yield return null;
        Assert.That(window.characterLabel.text,Does.Contain("Next: 140 EXP"));AssertEnglishActiveUi();
    }
    [UnityTest] public IEnumerator SeveralLevelsRefreshInventoryWithoutStackingOrRestoringResources()
    {
        var player=Player();player.Health.TakeDamage(25);player.TrySpendMP(10);var window=player.GetComponent<InventoryWindow>();window.Open();
        Assert.That(player.AddExperience(450),Is.EqualTo(450));yield return null;
        Assert.That(player.level,Is.EqualTo(4));Assert.That(player.LevelsGainedLastAward,Is.EqualTo(3));
        Assert.That(player.Health.currentHealth,Is.EqualTo(75));Assert.That(player.Health.maxHealth,Is.EqualTo(115));Assert.That(player.CurrentMP,Is.EqualTo(20));
        Assert.That(window.message.text,Is.EqualTo("Level Up! Level 4"));Assert.That(player.EffectiveAttack,Is.EqualTo(21));
        player.AddExperience(1);yield return null;Assert.That(player.Health.maxHealth,Is.EqualTo(115));Assert.That(player.EffectiveAttack,Is.EqualTo(21));AssertEnglishActiveUi();
    }
}

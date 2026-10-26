using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator LearnedPhysicalAbilityEntersRealBattleMenuPaysManaOnceAndSurvivesWorldReturn()
    {
        var player=Player();var enemy=Enemy();var learn=player.Learning;var power=learn.catalog.First(s=>s.id=="ability.powerstrike");
        Assert.That(learn.Knows(Fire(player)),Is.True);Assert.That(learn.TryLearn(power),Is.False);
        player.Attributes.SetBase(CharacterAttribute.Strength,20);player.Attributes.SetBase(CharacterAttribute.MagicControl,99);
        int points=learn.Points;Assert.That(learn.TryLearn(power),Is.True);Assert.That(learn.TryLearn(power),Is.False);Assert.That(learn.Points,Is.EqualTo(points-1));
        Assert.That(player.SpellCost(power),Is.EqualTo(5));
        yield return BeginBattle(player,enemy);var manager=Manager();manager.OpenMagicMenu();
        Assert.That(manager.ui.MenuCount,Is.EqualTo(4));int mana=player.CurrentMP;
        Assert.That(manager.TryPlayerSpell(power),Is.True);Assert.That(manager.TryPlayerSpell(power),Is.False);
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP,Is.EqualTo(mana-5));Assert.That(enemy.Health.currentHealth,Is.EqualTo(29));
        Assert.That(learn.TryLearn(learn.catalog.Last()),Is.False,"Learning is locked during an encounter.");
        enemy.Health.TakeDamage(999);yield return WaitUntil(()=>manager.State==BattleState.Victory);
        BattleTransition.Instance.ReturnAfterVictory();yield return WaitUntil(()=>!BattleTransition.Instance.EncounterActive);
        Assert.That(learn.Knows(power),Is.True);Assert.That(learn.Points,Is.EqualTo(points-1));
    }
    [UnityTest] public IEnumerator NewlyLearnedHealAndSacredStrikeUseRequirementsAndOnlyDamageUndeadWithHoly()
    {
        var player=Player();var learn=player.Learning;var sacred=learn.catalog.First(s=>s.id=="ability.sacredstrike");var greater=learn.catalog.First(s=>s.id=="ability.greaterheal");
        player.Attributes.SetBase(CharacterAttribute.Strength,15);player.Attributes.SetBase(CharacterAttribute.Wisdom,25);player.Attributes.SetBase(CharacterAttribute.MagicPower,20);
        Assert.That(learn.TryLearn(sacred),Is.True);Assert.That(learn.TryLearn(greater),Is.True);
        var enemy=Enemy();enemy.Health.TakeDamage(10);player.Health.TakeDamage(60);
        Assert.That(BattleAbilities.TryCast(player,player,greater,rules,out int healed),Is.True);Assert.That(healed,Is.EqualTo(60));Assert.That(enemy.Health.currentHealth,Is.EqualTo(50));
        Assert.That(BattleAbilities.TryCast(player,enemy,sacred,rules,out int damage,new ConstantBattleRandom(.5f)),Is.True);
        Assert.That(player.LastDamageDealt.HolyDamage,Is.GreaterThanOrEqualTo(8));Assert.That(damage,Is.EqualTo(player.LastDamageDealt.Total));
        yield return null;
    }
}

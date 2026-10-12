using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator WorldSkeletonMovesAndBattleUsesAttackHurtAndDeathFrames()
    {
        var player=Player();var enemy=Enemy();var visual=enemy.GetComponent<SpriteRenderer>();var set=enemy.visualAnimations;
        Assert.That(enemy.GetComponent<EnemySpriteAnimation>(),Is.Not.Null);Assert.That(set,Is.Not.Null);
        Assert.That(enemy.transform.localScale,Is.EqualTo(new Vector3(2,2,1)));Assert.That(enemy.GetComponents<Collider2D>().Length,Is.EqualTo(2));
        enemy.GetComponent<EnemyAI>().enabled=false;var position=enemy.transform.position;
        enemy.transform.position+=Vector3.right*0.1f;yield return new WaitForEndOfFrame();
        Assert.That(visual.sprite.name,Does.Contain("Move"));enemy.transform.position=position;
        yield return BeginBattle(player,enemy);var manager=Manager();manager.presentation.attackDuration=0.8f;
        Assert.That(manager.ui.enemyPortrait.sprite.texture.width,Is.EqualTo(64));
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.True);
        yield return WaitUntil(()=>manager.ui.enemyPortrait.sprite.name.Contains("Hurt"));
        Assert.That(enemy.Health.currentHealth,Is.EqualTo(50));
        yield return WaitUntil(()=>manager.CurrentActor==enemy && manager.presentation.IsAnimating && manager.ui.enemyPortrait.sprite.name.Contains("Attack"));
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth,Is.EqualTo(50));Assert.That(player.Health.currentHealth,Is.EqualTo(96));
        enemy.Health.TakeDamage(50);yield return WaitUntil(()=>manager.State==BattleState.Victory);
        Assert.That(manager.ui.enemyPortrait.sprite,Is.SameAs(set.death[3]));Assert.That(manager.ui.enemyPortrait.color.a,Is.Zero);
        Assert.That(manager.presentation.IsAnimating,Is.False);
    }
    [UnityTest] public IEnumerator AnimatedFireHealAndDisabledVfxDoNotDuplicateEffectsOrResources()
    {
        var player=Player();yield return BeginBattle(player,Enemy());var manager=Manager();var vfx=manager.vfx;
        Assert.That(manager.TryPlayerSpell(Fire(player)),Is.True);Assert.That(manager.TryPlayerSpell(Fire(player)),Is.False);
        yield return WaitUntil(()=>vfx.HasAnimatedFrame);Assert.That(vfx.CurrentFrame,Is.InRange(0,7));
        Assert.That(manager.ui.feedbackGlow.gameObject.activeSelf,Is.False,"Magic uses pixel VFX instead of a solid portrait rectangle");
        Assert.That(player.CurrentMP,Is.EqualTo(22));Assert.That(manager.Enemy.Health.currentHealth,Is.EqualTo(41));
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);yield return WaitUntil(()=>!vfx.HasAnimatedFrame);
        Assert.That(vfx.ActiveParticles,Is.Zero);Assert.That(vfx.EffectsPlayed,Is.EqualTo(1));
        player.Health.TakeDamage(20);Assert.That(manager.TryPlayerSpell(Heal(player)),Is.True);
        yield return WaitUntil(()=>vfx.HasAnimatedFrame);Assert.That(player.CurrentMP,Is.EqualTo(16));
        vfx.enabled=false;Assert.That(vfx.HasAnimatedFrame,Is.False);Assert.That(vfx.ActiveParticles,Is.Zero);
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);Assert.That(player.Health.currentHealth,Is.EqualTo(96));
        Assert.That(vfx.EffectsPlayed,Is.EqualTo(2));Assert.That(manager.TryPlayerAction(BattleAction.Defend),Is.True);
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);Assert.That(player.CurrentMP,Is.EqualTo(16));
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// Тести піксельних анімацій скелета і вогню (частина класу BattleFlowTests)
public sealed partial class BattleFlowTests
{
    // Скелет у світі показує кадри ходьби, а в бою - кадри атаки, болю і смерті
    [UnityTest]
    public IEnumerator WorldSkeletonMovesAndBattleUsesAttackHurtAndDeathFrames()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        SpriteRenderer visual = enemy.GetComponent<SpriteRenderer>();
        SpriteAnimationSet set = enemy.visualAnimations;

        Assert.That(enemy.GetComponent<EnemySpriteAnimation>(), Is.Not.Null);
        Assert.That(set, Is.Not.Null);
        Assert.That(enemy.transform.localScale, Is.EqualTo(new Vector3(2, 2, 1)));
        Assert.That(enemy.GetComponents<Collider2D>().Length, Is.EqualTo(2));

        // Трохи зсуваємо скелета вручну - має з'явитись кадр ходьби
        enemy.GetComponent<EnemyAI>().enabled = false;
        Vector3 position = enemy.transform.position;
        enemy.transform.position = enemy.transform.position + Vector3.right * 0.1f;
        yield return new WaitForEndOfFrame();
        Assert.That(visual.sprite.name, Does.Contain("Move"));
        enemy.transform.position = position;

        yield return BeginBattle(player, enemy);
        BattleManager manager = Manager();
        manager.presentation.attackDuration = 0.8f;
        Assert.That(manager.ui.enemyPortrait.sprite.texture.width, Is.EqualTo(64));

        // Б'ємо - скелет показує біль
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        yield return WaitUntil(() => manager.ui.enemyPortrait.sprite.name.Contains("Hurt"));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));

        // Хід скелета - кадри атаки
        yield return WaitUntil(() => manager.CurrentActor == enemy && manager.presentation.IsAnimating && manager.ui.enemyPortrait.sprite.name.Contains("Attack"));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));

        // Вбиваємо - останній кадр смерті, портрет прозорий
        enemy.Health.TakeDamage(50);
        yield return WaitUntil(() => manager.State == BattleState.Victory);
        Assert.That(manager.ui.enemyPortrait.sprite, Is.Not.Null);
        Assert.That(manager.ui.enemyPortrait.sprite.name, Does.Contain("Death"));
        Assert.That(manager.ui.enemyPortrait.color.a, Is.Zero);
        Assert.That(manager.presentation.IsAnimating, Is.False);
    }

    // Анімований вогонь програється один раз, а вимкнений VFX не ламає бій і не дублює ефекти
    [UnityTest]
    public IEnumerator AnimatedFireAndDisabledVfxDoNotDuplicateEffectsOrResources()
    {
        BattleUnit player = Player();
        yield return BeginBattle(player, Enemy());
        BattleManager manager = Manager();
        BattleVFXController vfx = manager.vfx;

        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.True);
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);

        yield return WaitUntil(() => vfx.HasAnimatedFrame);
        Assert.That(vfx.CurrentFrame, Is.InRange(0, 7));
        Assert.That(manager.ui.feedbackGlow.gameObject.activeSelf, Is.False, "Magic uses pixel VFX instead of a solid portrait rectangle");
        Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(manager.Enemy.Health.currentHealth, Is.EqualTo(41));

        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        yield return WaitUntil(() => !vfx.HasAnimatedFrame);
        Assert.That(vfx.ActiveParticles, Is.Zero);
        Assert.That(vfx.EffectsPlayed, Is.EqualTo(1));

        // Вимикаємо VFX
        player.Health.TakeDamage(20);
        manager.vfx.enabled = false;
        Assert.That(vfx.HasAnimatedFrame, Is.False);
        Assert.That(vfx.ActiveParticles, Is.Zero);

        Assert.That(manager.TryPlayerAction(BattleAction.Defend), Is.True);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(vfx.EffectsPlayed, Is.EqualTo(1));
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// Тести піксельної графіки: статичний скелет і анімований вогонь (частина класу BattleFlowTests)
public sealed partial class BattleFlowTests
{
    // Скелет без власної анімації: у світі і в бою завжди один статичний Idle-спрайт.
    // Ефекти ударів і рух портрета при цьому залишаються.
    [UnityTest]
    public IEnumerator SkeletonUsesStaticIdleSpriteInWorldAndBattle()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        SpriteRenderer visual = enemy.GetComponent<SpriteRenderer>();

        // Як у BossSkeleton - набору анімацій немає
        Assert.That(enemy.visualAnimations, Is.Null);
        Assert.That(enemy.battleSprite.name, Does.Contain("Idle"));
        Assert.That(enemy.transform.localScale, Is.EqualTo(new Vector3(2, 2, 1)));
        Assert.That(enemy.GetComponents<Collider2D>().Length, Is.EqualTo(2));

        // Трохи зсуваємо скелета вручну - кадр НЕ має змінитись
        enemy.GetComponent<EnemyAI>().enabled = false;
        Vector3 position = enemy.transform.position;
        enemy.transform.position = enemy.transform.position + Vector3.right * 0.1f;
        yield return new WaitForEndOfFrame();
        Assert.That(visual.sprite, Is.SameAs(enemy.battleSprite));
        enemy.transform.position = position;

        yield return BeginBattle(player, enemy);
        BattleManager manager = Manager();
        manager.presentation.attackDuration = 0.8f;
        Assert.That(manager.ui.enemyPortrait.sprite.texture.width, Is.EqualTo(64));

        // Б'ємо - урон є, але кадр портрета той самий
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        yield return WaitUntil(() => enemy.Health.currentHealth == 50);
        Assert.That(manager.ui.enemyPortrait.sprite, Is.SameAs(enemy.battleSprite));

        // Хід скелета - теж без кадрів атаки
        yield return WaitUntil(() => manager.CurrentActor == enemy && manager.presentation.IsAnimating);
        Assert.That(manager.ui.enemyPortrait.sprite, Is.SameAs(enemy.battleSprite));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));

        // Вбиваємо - спрайт той самий, портрет плавно зникає
        enemy.Health.TakeDamage(50);
        yield return WaitUntil(() => manager.State == BattleState.Victory);
        Assert.That(manager.ui.enemyPortrait.sprite, Is.SameAs(enemy.battleSprite));
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

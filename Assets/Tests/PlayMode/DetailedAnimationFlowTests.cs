using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest]
    public IEnumerator HeroAttackUsesSixteenFpsAndAppliesDamageAtContactOnce()
    {
        var player=Player();var enemy=Enemy();yield return BeginBattle(player,enemy);
        rules.actionDelay=.8f;rules.enemyDefendChance=0;player.criticalChance=player.missChance=0;
        var manager=Manager();var set=player.visualAnimations;
        Assert.That(set.useFrameTiming,Is.True);Assert.That(set.framesPerSecond,Is.EqualTo(16f));
        // A legacy presentation duration must not silently stretch the frame-timed hero attack.
        manager.presentation.attackDuration=.9f;manager.presentation.allowFastForward=false;
        var observed=new Dictionary<int,float>();float first=-1f,finished=-1f,maxRenderDelta=0f;
        int previousHP=enemy.Health.currentHealth,changes=0,contact=-1;
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.True);
        float deadline=Time.unscaledTime+3f;
        while(Time.unscaledTime<deadline)
        {
            yield return new WaitForEndOfFrame();maxRenderDelta=Mathf.Max(maxRenderDelta,Time.unscaledDeltaTime);
            int index=System.Array.IndexOf(set.attack,manager.ui.playerPortrait.sprite);
            if(index>=0)
            {
                if(first<0f)first=Time.unscaledTime;
                if(!observed.ContainsKey(index))observed.Add(index,Time.unscaledTime);
            }
            if(enemy.Health.currentHealth!=previousHP)
            {
                previousHP=enemy.Health.currentHealth;changes++;contact=index;
            }
            if(first>=0f && index<0){finished=Time.unscaledTime;break;}
        }
        Assert.That(observed.Count,Is.EqualTo(8),"All eight real attack sprites must appear.");
        float tolerance=Mathf.Max(.035f,maxRenderDelta+.01f);
        for(int i=1;i<8;i++)Assert.That(observed[i]-observed[i-1],Is.EqualTo(1f/16f).Within(tolerance),"Sprite interval "+i);
        Assert.That(finished-first,Is.EqualTo(.5f).Within(tolerance*2f));
        Assert.That(changes,Is.EqualTo(1));Assert.That(contact,Is.EqualTo(4));
        Assert.That(enemy.Health.currentHealth,Is.EqualTo(50));Assert.That(player.CurrentMP,Is.EqualTo(40));
    }

    [UnityTest]
    public IEnumerator WaitingActorsAnimateIdleAndBothAttacksReturnToIdleWithSingleDamage()
    {
        var player=Player();var enemy=Enemy();var inventory=player.GetComponent<InventoryWindow>();
        inventory.Open();yield return null;
        Assert.That(inventory.portraitSprite,Is.Not.Null);Assert.That(inventory.characterPortrait.sprite,Is.SameAs(inventory.portraitSprite));
        Assert.That(inventory.characterPortrait.sprite.rect.height,Is.GreaterThan(inventory.characterPortrait.sprite.rect.width),"Inventory keeps its existing full-height portrait.");
        inventory.Close();yield return BeginBattle(player,enemy);var manager=Manager();
        rules.actionDelay=.8f;rules.enemyDefendChance=0;player.criticalChance=player.missChance=0;enemy.criticalChance=enemy.missChance=0;
        var heroIdle=new HashSet<Sprite>();var foeIdle=new HashSet<Sprite>();float until=Time.time+1.1f;
        while(Time.time<until){yield return null;heroIdle.Add(manager.ui.playerPortrait.sprite);foeIdle.Add(manager.ui.enemyPortrait.sprite);}
        Assert.That(heroIdle.Count,Is.EqualTo(4));Assert.That(foeIdle.Count,Is.EqualTo(4));
        var heroAttack=new HashSet<Sprite>();var foeAttack=new HashSet<Sprite>();
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.True);Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.False);
        until=Time.time+5f;
        while(Time.time<until)
        {
            yield return null;
            if(manager.ui.playerPortrait.sprite.name.Contains("Attack"))heroAttack.Add(manager.ui.playerPortrait.sprite);
            if(manager.ui.enemyPortrait.sprite.name.Contains("Attack"))foeAttack.Add(manager.ui.enemyPortrait.sprite);
            if(manager.State==BattleState.AwaitingPlayer && foeAttack.Count>0)break;
        }
        Assert.That(manager.State,Is.EqualTo(BattleState.AwaitingPlayer));
        Assert.That(heroAttack.Count,Is.GreaterThanOrEqualTo(6));Assert.That(foeAttack.Count,Is.GreaterThanOrEqualTo(6));
        Assert.That(enemy.Health.currentHealth,Is.EqualTo(50));Assert.That(player.Health.currentHealth,Is.EqualTo(96));
        Assert.That(player.CurrentMP,Is.EqualTo(40));Assert.That(manager.presentation.IsAnimating,Is.False);
        Assert.That(player.visualAnimations.idle,Does.Contain(manager.ui.playerPortrait.sprite));
        Assert.That(enemy.visualAnimations.idle,Does.Contain(manager.ui.enemyPortrait.sprite));
    }
    [UnityTest]
    public IEnumerator SkeletonSelectsCardinalViewsRetainsFacingAndBridgesPhysicsGaps()
    {
        var enemy=Enemy();enemy.GetComponent<EnemyAI>().enabled=false;enemy.GetComponent<Rigidbody2D>().simulated=false;
        var visual=enemy.GetComponent<SpriteRenderer>();var set=enemy.visualAnimations;
        foreach(var entry in set.worldDirections)
        {
            enemy.transform.position+=(Vector3)entry.direction*.1f;yield return new WaitForEndOfFrame();
            Assert.That(entry.walk,Does.Contain(visual.sprite));Assert.That(visual.flipX,Is.False);
            yield return null;yield return new WaitForEndOfFrame();
            Assert.That(entry.walk,Does.Contain(visual.sprite),"No Idle flicker between physics steps.");
            yield return new WaitForSeconds(.12f);yield return new WaitForEndOfFrame();
            Assert.That(visual.sprite,Is.SameAs(entry.idle[0]));
        }
        Assert.That(enemy.GetComponents<Collider2D>().Length,Is.EqualTo(2));
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator SpaceSpeedsPresentationWithoutChoosingOrDuplicatingActions()
    {
        var player=Player();var enemy=Enemy();yield return BeginBattle(player,enemy);
        var manager=Manager();var portrait=manager.ui.playerPortrait.rectTransform;
        Vector2 home=portrait.anchoredPosition;float timeScale=Time.timeScale;
        var keyboard=InputSystem.AddDevice<Keyboard>();
        try
        {
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));yield return null;
            Assert.That(manager.State,Is.EqualTo(BattleState.AwaitingPlayer));
            Assert.That(manager.presentation.VisualDelta,Is.EqualTo(Time.unscaledDeltaTime*3f).Within(0.00001f));
            manager.presentation.allowFastForward=false;
            Assert.That(manager.presentation.VisualDelta,Is.EqualTo(Time.unscaledDeltaTime));
            manager.presentation.allowFastForward=true;
            Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.True);
            Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.False);
            yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.That(enemy.Health.currentHealth,Is.EqualTo(48));Assert.That(player.Health.currentHealth,Is.EqualTo(91));
            Assert.That(player.CurrentMP,Is.EqualTo(40));Assert.That(player.CurrentExperience,Is.Zero);
            Assert.That(player.Inventory.Summary,Does.Contain("3"));Assert.That(manager.Round,Is.EqualTo(2));
            Assert.That(portrait.anchoredPosition,Is.EqualTo(home));Assert.That(Time.timeScale,Is.EqualTo(timeScale));
        }
        finally { InputSystem.RemoveDevice(keyboard); }
    }

    [UnityTest] public IEnumerator MissShowsTextWithoutHitReactionOrDamage()
    {
        var player=Player();var enemy=Enemy();yield return BeginBattle(player,enemy);var manager=Manager();
        player.missChance=1f;manager.presentation.attackDuration=0.8f;
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.True);
        yield return WaitUntil(()=>manager.ui.feedbackText.gameObject.activeSelf && manager.ui.feedbackText.text=="MISS");
        Assert.That(enemy.Health.currentHealth,Is.EqualTo(60));
        Assert.That(manager.ui.feedbackGlow.gameObject.activeSelf,Is.False);
        Assert.That(manager.presentation.HitEffectsPlayed,Is.Zero);
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(manager.presentation.HitEffectsPlayed,Is.EqualTo(1),"Only the enemy's successful answer has a hit effect");
        Assert.That(player.Health.currentHealth,Is.EqualTo(91));Assert.That(enemy.Health.currentHealth,Is.EqualTo(60));
    }

    [UnityTest] public IEnumerator AssignedClipUsesMixerAndRespectsExistingVolume()
    {
        yield return BeginBattle(Player(),Enemy());var audio=Manager().audioController;
        var original=audio.profile;var profile=Object.Instantiate(original);
        var clip=AudioClip.Create("Test-only silent clip",256,1,44100,false);
        float volume=AudioListener.volume;
        try
        {
            profile.hit=clip;audio.profile=profile;
            Assert.That(audio.Play(BattleSound.Hit),Is.True);
            Assert.That(audio.LastEvent,Is.EqualTo(BattleSound.Hit));
            Assert.That(audio.GetComponent<AudioSource>().outputAudioMixerGroup,Is.SameAs(profile.output));
            Assert.That(AudioListener.volume,Is.EqualTo(volume));
            Assert.That(audio.GetComponent<AudioSource>().ignoreListenerVolume,Is.False);
            yield return null;
        }
        finally { audio.profile=original;Object.Destroy(profile);Object.Destroy(clip); }
    }

    [UnityTest] public IEnumerator PresentationEntranceAndOutcomesKeepTransitionLockedAndShowResults()
    {
        var player=Player();var enemy=Enemy();var transition=BattleTransition.Instance;
        Assert.That(transition.TryBegin(player,enemy),Is.True);
        for(int i=0;i<5;i++)Assert.That(transition.TryBegin(player,enemy),Is.False);
        yield return WaitUntil(()=>Manager()!=null && Manager().State==BattleState.AwaitingPlayer);
        var manager=Manager();Assert.That(manager.ui.hud.screenGroup.alpha,Is.EqualTo(1f));
        player.Health.TakeDamage(100);yield return WaitUntil(()=>manager.State==BattleState.Defeat);
        Assert.That(manager.ui.resultPanel.gameObject.activeSelf,Is.True);
        Assert.That(manager.ui.resultText.text,Does.Contain("DEFEAT"));
        Assert.That(manager.ui.playerPortrait.color.a,Is.Zero);
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.False);
    }

    [UnityTest] public IEnumerator MissingAudioClipsAreSafeAndListenerVolumeIsRespected()
    {
        var player=Player();yield return BeginBattle(player,Enemy());var manager=Manager();var audio=manager.audioController;
        Assert.That(audio,Is.Not.Null);float before=AudioListener.volume;
        foreach(BattleSound sound in System.Enum.GetValues(typeof(BattleSound)))Assert.That(audio.Play(sound),Is.False);
        Assert.That(AudioListener.volume,Is.EqualTo(before));
        Assert.That(audio.GetComponent<AudioSource>().ignoreListenerVolume,Is.False);
        Assert.That(audio.GetComponent<AudioSource>().spatialBlend,Is.Zero);
        manager.TryPlayerAction(BattleAction.Attack);yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(manager.Enemy.Health.currentHealth,Is.EqualTo(48));Assert.That(player.Health.currentHealth,Is.EqualTo(91));
    }

    [UnityTest] public IEnumerator MagicVFXFinishesAndMissingVFXNeverBlocksResourceRules()
    {
        var player=Player();var enemy=Enemy();yield return BeginBattle(player,enemy);var manager=Manager();
        manager.TryPlayerSpell(Fire(player));yield return WaitUntil(()=>player.CurrentMP==32);
        Assert.That(manager.vfx.EffectsPlayed,Is.EqualTo(1));Assert.That(manager.vfx.ActiveParticles,Is.GreaterThan(0));
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        yield return new WaitForSecondsRealtime(0.5f);
        Assert.That(manager.vfx.ActiveParticles,Is.Zero);Assert.That(enemy.Health.currentHealth,Is.EqualTo(40));
        manager.vfx=null;manager.TryPlayerSpell(Heal(player));
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP,Is.EqualTo(26));Assert.That(player.Health.currentHealth,Is.EqualTo(91));
    }

    [UnityTest] public IEnumerator TargetDeathDuringWindupCancelsDamageAndManaSpend()
    {
        var player=Player();var enemy=Enemy();yield return BeginBattle(player,enemy);
        var manager=Manager();manager.presentation.attackDuration=1f;
        Assert.That(manager.TryPlayerSpell(Fire(player)),Is.True);
        yield return WaitUntil(()=>manager.presentation.IsAnimating);
        Assert.That(manager.TryPlayerSpell(Fire(player)),Is.False);
        enemy.Health.TakeDamage(60);
        yield return WaitUntil(()=>manager.State==BattleState.Victory);
        Assert.That(player.CurrentMP,Is.EqualTo(40));Assert.That(player.CurrentExperience,Is.EqualTo(20));
        Assert.That(manager.presentation.IsAnimating,Is.False);
    }

    [UnityTest] public IEnumerator MissingPresentationDoesNotBlockOrDuplicatePhysicalAction()
    {
        var player=Player();var enemy=Enemy();yield return BeginBattle(player,enemy);
        var manager=Manager();manager.presentation=null;
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.True);
        Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.False);
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth,Is.EqualTo(48));Assert.That(player.Health.currentHealth,Is.EqualTo(91));
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
        Assert.That(player.Health.currentHealth, Is.EqualTo(20)); Assert.That(player.CurrentMP, Is.EqualTo(32));
        Assert.That(hud.hpBar.value, Is.EqualTo(0.2f).Within(0.001f));
        Assert.That(hud.mpBar.value, Is.EqualTo(32f/40f).Within(0.001f));
        Assert.That(hud.lowHealthText.gameObject.activeSelf, Is.True);
        Assert.That(manager.ui.playerStats.text, Does.Contain("HP  20 / 100"));
        manager.OpenMagicMenu(); manager.ShowMainMenu();
        yield return new WaitForSecondsRealtime(0.2f);
        Assert.That(hud.menuGroup.alpha, Is.EqualTo(1f));
        Assert.That(manager.State, Is.EqualTo(BattleState.AwaitingPlayer));
        Assert.That(player.CurrentMP, Is.EqualTo(32));
    }
}

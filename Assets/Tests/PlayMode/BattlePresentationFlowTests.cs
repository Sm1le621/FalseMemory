using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// Тести анімацій, звуку, VFX і HUD у бою (частина класу BattleFlowTests)
public sealed partial class BattleFlowTests
{
    // Пробіл прискорює анімації, але сам нічого не вибирає і не дублює дії
    [UnityTest]
    public IEnumerator SpaceSpeedsPresentationWithoutChoosingOrDuplicatingActions()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        RectTransform portrait = manager.ui.playerPortrait.rectTransform;
        Vector2 home = portrait.anchoredPosition;
        float timeScale = Time.timeScale;

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Space));
            yield return null;
            Assert.That(manager.State, Is.EqualTo(BattleState.AwaitingPlayer));

            // З пробілом - втричі швидше
            Assert.That(manager.presentation.VisualDelta, Is.EqualTo(Time.unscaledDeltaTime * 3f).Within(0.00001f));
            manager.presentation.allowFastForward = false;
            Assert.That(manager.presentation.VisualDelta, Is.EqualTo(Time.unscaledDeltaTime));
            manager.presentation.allowFastForward = true;

            Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
            Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.False);
            yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
            yield return new WaitForSecondsRealtime(0.2f);

            Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));
            Assert.That(player.Health.currentHealth, Is.EqualTo(96));
            Assert.That(player.CurrentMP, Is.EqualTo(30));
            Assert.That(manager.Round, Is.EqualTo(2));
            Assert.That(portrait.anchoredPosition, Is.EqualTo(home));
            // Time.timeScale не чіпали
            Assert.That(Time.timeScale, Is.EqualTo(timeScale));
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard);
        }
    }

    // Промах показує "MISS" без реакції на удар і без шкоди
    [UnityTest]
    public IEnumerator MissShowsTextWithoutHitReactionOrDamage()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        player.missChance = 1f;
        manager.presentation.attackDuration = 0.8f;

        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        yield return WaitUntil(() => manager.ui.feedbackText.gameObject.activeSelf && manager.ui.feedbackText.text == "MISS");

        Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
        Assert.That(manager.ui.feedbackGlow.gameObject.activeSelf, Is.False);
        Assert.That(manager.presentation.HitEffectsPlayed, Is.Zero);

        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(manager.presentation.HitEffectsPlayed, Is.EqualTo(1), "Only the enemy's successful answer has a hit effect");
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
    }

    // Звук іде через мікшер і не змінює загальну гучність
    [UnityTest]
    public IEnumerator AssignedClipUsesMixerAndRespectsExistingVolume()
    {
        yield return BeginBattle(Player(), Enemy());

        BattleAudioController audio = Manager().audioController;
        BattleAudioProfile original = audio.profile;
        BattleAudioProfile profile = Object.Instantiate(original);
        // Тихий тестовий звук
        AudioClip clip = AudioClip.Create("Test-only silent clip", 256, 1, 44100, false);
        float volume = AudioListener.volume;
        try
        {
            profile.hit = clip;
            audio.profile = profile;

            Assert.That(audio.Play(BattleSound.Hit), Is.True);
            Assert.That(audio.LastEvent, Is.EqualTo(BattleSound.Hit));
            Assert.That(audio.GetComponent<AudioSource>().outputAudioMixerGroup, Is.SameAs(profile.output));
            Assert.That(AudioListener.volume, Is.EqualTo(volume));
            Assert.That(audio.GetComponent<AudioSource>().ignoreListenerVolume, Is.False);
            yield return null;
        }
        finally
        {
            audio.profile = original;
            Object.Destroy(profile);
            Object.Destroy(clip);
        }
    }

    // Під час появи бою перехід заблокований, а в кінці показується результат
    [UnityTest]
    public IEnumerator PresentationEntranceAndOutcomesKeepTransitionLockedAndShowResults()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        BattleTransition transition = BattleTransition.Instance;

        Assert.That(transition.TryBegin(player, enemy), Is.True);
        // Повторні спроби не проходять
        for (int i = 0; i < 5; i++)
        {
            Assert.That(transition.TryBegin(player, enemy), Is.False);
        }
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);

        BattleManager manager = Manager();
        Assert.That(manager.ui.hud.screenGroup.alpha, Is.EqualTo(1f));

        player.Health.TakeDamage(100);
        yield return WaitUntil(() => manager.State == BattleState.Defeat);

        Assert.That(manager.ui.resultPanel.gameObject.activeSelf, Is.True);
        Assert.That(manager.ui.resultText.text, Does.Contain("DEFEAT"));
        Assert.That(manager.ui.playerPortrait.color.a, Is.Zero);
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.False);
    }

    // Якщо звуків немає - нічого не ламається
    [UnityTest]
    public IEnumerator MissingAudioClipsAreSafeAndListenerVolumeIsRespected()
    {
        BattleUnit player = Player();
        yield return BeginBattle(player, Enemy());

        BattleManager manager = Manager();
        BattleAudioController audio = manager.audioController;
        Assert.That(audio, Is.Not.Null);
        float before = AudioListener.volume;

        foreach (BattleSound sound in System.Enum.GetValues(typeof(BattleSound)))
        {
            Assert.That(audio.Play(sound), Is.False);
        }

        Assert.That(AudioListener.volume, Is.EqualTo(before));
        Assert.That(audio.GetComponent<AudioSource>().ignoreListenerVolume, Is.False);
        Assert.That(audio.GetComponent<AudioSource>().spatialBlend, Is.Zero);

        manager.TryPlayerAction(BattleAction.Attack);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(manager.Enemy.Health.currentHealth, Is.EqualTo(50));
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));
    }

    // Ефект магії закінчується, а без VFX правила мани все одно працюють
    [UnityTest]
    public IEnumerator MagicVFXFinishesAndMissingVFXNeverBlocksResourceRules()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        manager.TryPlayerSpell(Fire(player));
        yield return WaitUntil(() => player.CurrentMP == 22);
        Assert.That(manager.vfx.EffectsPlayed, Is.EqualTo(1));
        Assert.That(manager.vfx.ActiveParticles, Is.GreaterThan(0));

        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        yield return new WaitForSecondsRealtime(0.5f);
        Assert.That(manager.vfx.ActiveParticles, Is.Zero);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(41));

        // Прибираємо VFX зовсім
        manager.vfx = null;
        manager.TryPlayerAction(BattleAction.Defend);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(player.Health.currentHealth, Is.EqualTo(94));
    }

    // Якщо ціль померла під час замаху - шкоди немає і мана не витрачається
    [UnityTest]
    public IEnumerator TargetDeathDuringWindupCancelsDamageAndManaSpend()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        manager.presentation.attackDuration = 1f;

        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.True);
        yield return WaitUntil(() => manager.presentation.IsAnimating);
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);

        // Вбиваємо ворога посеред анімації
        enemy.Health.TakeDamage(60);
        yield return WaitUntil(() => manager.State == BattleState.Victory);
        Assert.That(player.CurrentMP, Is.EqualTo(30));
        Assert.That(manager.presentation.IsAnimating, Is.False);
    }

    // Без BattlePresentation бій теж працює, і дія не дублюється
    [UnityTest]
    public IEnumerator MissingPresentationDoesNotBlockOrDuplicatePhysicalAction()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        manager.presentation = null;

        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.False);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);

        Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));
        Assert.That(manager.Round, Is.EqualTo(2));
    }

    // Смужки HUD анімуються, але тільки показують значення, не змінюючи їх
    [UnityTest]
    public IEnumerator HUDAnimatesReadOnlyBarsWithoutChangingResources()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        BattleHUD hud = manager.ui.hud;
        Assert.That(hud, Is.Not.Null);
        // Гравець не може тягнути смужки мишкою
        Assert.That(hud.hpBar.interactable || hud.mpBar.interactable || hud.enemyHPBar.interactable, Is.False);

        player.Health.TakeDamage(80);
        player.TrySpendMP(8);
        manager.ui.RefreshStats(player, enemy, manager.Round);
        yield return new WaitForSecondsRealtime(0.35f);

        Assert.That(player.Health.currentHealth, Is.EqualTo(20));
        Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(hud.hpBar.value, Is.EqualTo(0.2f).Within(0.001f));
        Assert.That(hud.mpBar.value, Is.EqualTo(22f / 30f).Within(0.001f));
        Assert.That(hud.lowHealthText.gameObject.activeSelf, Is.True);
        Assert.That(manager.ui.playerStats.text, Does.Contain("HP 20 / 100"));

        manager.OpenMagicMenu();
        manager.ShowMainMenu();
        yield return new WaitForSecondsRealtime(0.2f);
        Assert.That(hud.menuGroup.alpha, Is.EqualTo(1f));
        Assert.That(manager.State, Is.EqualTo(BattleState.AwaitingPlayer));
        Assert.That(player.CurrentMP, Is.EqualTo(22));
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

// Тести втечі, магії, критів і меню заклинань (частина класу BattleFlowTests)
public sealed partial class BattleFlowTests
{
    // Перше заклинання гравця з магічною шкодою
    private static BattleSpell Fire(BattleUnit player)
    {
        foreach (BattleSpell spell in player.spells)
        {
            if (spell.effect == BattleEffect.MagicDamage)
            {
                return spell;
            }
        }
        throw new System.InvalidOperationException("Player has no damage spell.");
    }

    // Перше зілля в інвентарі гравця
    private static BattleConsumable Potion(BattleUnit player)
    {
        foreach (BattleConsumable consumable in player.Inventory.Definitions)
        {
            if (consumable != null)
            {
                return consumable;
            }
        }
        throw new System.InvalidOperationException("Player has no consumable.");
    }

    // Перша зброя в інвентарі гравця
    private static ItemData Sword(BattleUnit player)
    {
        foreach (ItemData item in player.Inventory.State.Definitions)
        {
            if (item.category == ItemCategory.Weapon)
            {
                return item;
            }
        }
        throw new System.InvalidOperationException("Player has no weapon.");
    }

    // Перша броня в інвентарі гравця
    private static ItemData Armor(BattleUnit player)
    {
        foreach (ItemData item in player.Inventory.State.Definitions)
        {
            if (item.category == ItemCategory.Armor)
            {
                return item;
            }
        }
        throw new System.InvalidOperationException("Player has no armor.");
    }

    // "Рандом", який завжди повертає одне число
    private sealed class ConstantBattleRandom : IBattleRandom
    {
        private readonly float value;

        public ConstantBattleRandom(float value)
        {
            this.value = value;
        }

        public float Next01()
        {
            return value;
        }
    }

    // Почати бій і дочекатись ходу гравця
    private static IEnumerator BeginBattle(BattleUnit player, BattleUnit enemy)
    {
        Assert.That(BattleTransition.Instance.TryBegin(player, enemy), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);
        yield return null;
        yield return null;
        yield return null;
    }

    // Невдала втеча забирає хід, але не витрачає ресурси
    [UnityTest]
    public IEnumerator FailedFleeConsumesOneTurnAndDoesNotSpendResources()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        // 1 - шанс ніколи не спрацює
        manager.SetRandomSource(new ConstantBattleRandom(1f));

        Assert.That(manager.TryPlayerAction(BattleAction.Flee), Is.True);
        Assert.That(manager.TryPlayerAction(BattleAction.Flee), Is.False);
        yield return WaitUntil(() => manager.ui.messageText.text.Contains("Couldn't escape!"));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);

        Assert.That(manager.Round, Is.EqualTo(2));
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));
        Assert.That(player.CurrentMP, Is.EqualTo(30));
        Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
    }

    // Успішна втеча: все зберігається, гравця відсуває, і через час можна знову битись
    [UnityTest]
    public IEnumerator SuccessfulFleePreservesResourcesAndEnemyThenProtectsAndAllowsAnotherBattle()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        BattleTransition transition = BattleTransition.Instance;

        player.Health.TakeDamage(20);
        player.TrySpendMP(8);
        player.Inventory.TryConsume(Potion(player));
        enemy.Health.TakeDamage(10);

        // Ставимо гравця прямо на ворога
        player.GetComponent<Rigidbody2D>().position = enemy.transform.position;
        Physics2D.SyncTransforms();

        yield return BeginBattle(player, enemy);
        BattleManager manager = Manager();
        Vector3 enemyPosition = enemy.transform.position;
        // 0 - шанс завжди спрацює
        manager.SetRandomSource(new ConstantBattleRandom(0f));

        Assert.That(manager.TryPlayerAction(BattleAction.Flee), Is.True);
        yield return WaitUntil(() => manager.State == BattleState.Escaped);
        Assert.That(manager.ui.messageText.text, Does.Contain("Escaped!"));
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
        yield return WaitUntil(() => !transition.EncounterActive);

        // Ворог живий і на місці, ресурси гравця не змінились
        Assert.That(enemy.gameObject.activeSelf, Is.True);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(50));
        Assert.That(enemy.transform.position, Is.EqualTo(enemyPosition));
        Assert.That(player.Health.currentHealth, Is.EqualTo(80));
        Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(2));
        Assert.That(player.GetComponent<PlayerController>().enabled, Is.True);

        // Діє захист від повторного бою
        Assert.That(transition.IsEncounterProtected, Is.True);
        Assert.That(transition.TryBegin(player, enemy), Is.False);

        // Гравця відсунуло на безпечну відстань
        float distance = Vector2.Distance(player.transform.position, enemy.transform.position);
        Assert.That(distance, Is.GreaterThanOrEqualTo(rules.fleeSafeDistance - 0.01f));

        // Гравець не застряг усередині стіни
        Collider2D collider = player.GetComponent<Collider2D>();
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0f);
        bool stuck = false;
        foreach (Collider2D c in overlaps)
        {
            if (!c.isTrigger && c.gameObject != player.gameObject)
            {
                stuck = true;
            }
        }
        Assert.That(stuck, Is.False);

        // Захист закінчився - можна знову битись, і все збереглося
        yield return WaitUntil(() => !transition.IsEncounterProtected);
        Assert.That(transition.TryBegin(player, enemy), Is.True);
        yield return WaitUntil(() => Manager() != null && Manager().State == BattleState.AwaitingPlayer);
        Assert.That(Manager().Player.CurrentMP, Is.EqualTo(22));
        Assert.That(Manager().Player.Inventory.Count(Potion(player)), Is.EqualTo(2));
        Assert.That(Manager().Enemy.Health.currentHealth, Is.EqualTo(50));
    }

    // Перемога зберігає HP, ману і предмети, і показує результат
    [UnityTest]
    public IEnumerator VictoryPreservesAllSessionStateAndShowsResults()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        BattleTransition transition = BattleTransition.Instance;
        player.Health.TakeDamage(20);

        yield return BeginBattle(player, enemy);
        BattleManager manager = Manager();

        // Два Fire
        manager.TryPlayerSpell(Fire(player));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        manager.TryPlayerSpell(Fire(player));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);

        // Далі звичайні атаки до кінця бою
        while (manager.State == BattleState.AwaitingPlayer)
        {
            manager.TryPlayerAction(BattleAction.Attack);
            yield return WaitUntil(() => manager.State != BattleState.Resolving);
        }

        Assert.That(manager.State, Is.EqualTo(BattleState.Victory));
        Assert.That(player.CurrentMP, Is.EqualTo(14));
        Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        Assert.That(player.Health.currentHealth, Is.InRange(1, 99));
        Assert.That(manager.ui.resultPanel.gameObject.activeSelf, Is.True);
        Assert.That(manager.ui.resultText.text, Does.Contain("VICTORY").And.Not.Contain("EXP"));
        Assert.That(manager.TryPlayerAction(BattleAction.Flee), Is.False);

        yield return new WaitForSecondsRealtime(0.3f);
        int health = player.Health.currentHealth;

        // Повернутись можна тільки один раз
        Assert.That(transition.ReturnAfterVictory(), Is.True);
        Assert.That(transition.ReturnAfterVictory(), Is.False);
        yield return WaitUntil(() => !transition.EncounterActive);

        Assert.That(player.Health.currentHealth, Is.EqualTo(health));
        Assert.That(player.CurrentMP, Is.EqualTo(14));
        Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        Assert.That(enemy.gameObject.activeSelf, Is.False);
        Assert.That(player.Health.hpSlider.value, Is.EqualTo((float)health));
        Assert.That(transition.TryBegin(player, enemy), Is.False);
    }

    // Якщо ціль померла до виконання заклинання - мана не витрачається
    [UnityTest]
    public IEnumerator TargetDeathBeforeQueuedSpellCancelsManaSpendAndFurtherActions()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.True);
        enemy.Health.TakeDamage(60);
        yield return WaitUntil(() => manager.State == BattleState.Victory);

        Assert.That(player.CurrentMP, Is.EqualTo(30));
        Assert.That(player.Health.currentHealth, Is.EqualTo(100));
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
    }

    // Затиснутий Enter при вході в меню магії не кастує заклинання - потрібне окреме натискання
    [UnityTest]
    public IEnumerator HoldingEnterAcrossMagicMenuDoesNotCastUntilASeparatePress()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            // Стрілка вниз (на "Magic"), відпустили
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.DownArrow));
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
            yield return null;

            // Enter - відкрилось меню магії, Enter досі затиснутий
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return new WaitForSecondsRealtime(0.4f);
            Assert.That(manager.ui.commandLabels[0].text, Does.Contain("Fire"));
            Assert.That(player.CurrentMP, Is.EqualTo(30));
            Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));

            // Відпустили і натиснули ще раз - тепер кастується
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return WaitUntil(() => player.CurrentMP == 22);
            yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
            yield return new WaitForSecondsRealtime(0.3f);
            Assert.That(player.CurrentMP, Is.EqualTo(22));
            Assert.That(enemy.Health.currentHealth, Is.EqualTo(41));
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard);
        }
    }

    // Довгий список заклинань прокручується, а "Back" нічого не витрачає
    [UnityTest]
    public IEnumerator ExpandedSpellListScrollsAndBackDoesNotSpendResources()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        BattleSpell[] originalSpells = player.spells;

        // 6 тестових заклинань
        BattleSpell[] extraSpells = new BattleSpell[6];
        for (int i = 0; i < extraSpells.Length; i++)
        {
            extraSpells[i] = ScriptableObject.CreateInstance<BattleSpell>();
        }

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            player.spells = extraSpells;
            yield return BeginBattle(player, enemy);

            BattleManager manager = Manager();
            manager.OpenMagicMenu();
            yield return null;
            yield return null;
            yield return null;

            // 6 заклинань + "Back"
            Assert.That(manager.ui.MenuCount, Is.EqualTo(7));

            // 6 разів вниз
            for (int i = 0; i < 6; i++)
            {
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.DownArrow));
                yield return null;
                yield return null;
                InputSystem.QueueStateEvent(keyboard, new KeyboardState());
                yield return null;
                yield return null;
            }

            Assert.That(manager.ui.SelectedIndex, Is.EqualTo(6));
            Assert.That(manager.ui.commandLabels[4].text, Does.Contain("Back"));

            manager.ui.commandButtons[4].onClick.Invoke();
            Assert.That(manager.ui.MenuCount, Is.EqualTo(4));
            Assert.That(manager.Round, Is.EqualTo(1));
            Assert.That(player.CurrentMP, Is.EqualTo(30));
            Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        }
        finally
        {
            player.spells = originalSpells;
            InputSystem.RemoveDevice(keyboard);
            foreach (BattleSpell spell in extraSpells)
            {
                Object.Destroy(spell);
            }
        }
    }

    // Смерть під час вибору дії скасовує хід і блокує команди
    [UnityTest]
    public IEnumerator DeathWhileSelectingCancelsPendingTurnAndRejectsActions()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        player.Health.TakeDamage(100);
        yield return WaitUntil(() => manager.State == BattleState.Defeat);

        Assert.That(manager.ui.playerStats.text, Does.Contain("HP 0 / 100"));
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.False);
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
        Assert.That(player.CurrentMP, Is.EqualTo(30));
        Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
    }

    // Крит і промах показують правильні повідомлення і забирають по одному ходу
    [UnityTest]
    public IEnumerator CriticalAndMissHaveCorrectDamageMessagesAndEachSpendOneTurn()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        manager.SetRandomSource(new ConstantBattleRandom(0.5f));

        // Крит
        player.criticalChance = 1f;
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        yield return WaitUntil(() => enemy.Health.currentHealth == 45);
        Assert.That(manager.ui.messageText.text, Does.Contain("CRITICAL!"));
        Assert.That(manager.ui.feedbackText.text, Does.Contain("CRITICAL!"));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(manager.Round, Is.EqualTo(2));
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));

        // Промах
        player.missChance = 1f;
        Assert.That(manager.TryPlayerAction(BattleAction.Attack), Is.True);
        yield return WaitUntil(() => manager.ui.messageText.text.Contains("MISS"));
        Assert.That(manager.ui.feedbackText.text, Is.EqualTo("MISS"));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(45));
        Assert.That(player.Health.currentHealth, Is.EqualTo(92));
        Assert.That(manager.Round, Is.EqualTo(3));
    }

    // Fire витрачає ману один раз, і після бою HP та мана зберігаються
    [UnityTest]
    public IEnumerator FireConsumesMPOnceAndReturnsWithSameHealthAndMana()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        BattleTransition transition = BattleTransition.Instance;
        // Fire не залежить від фізичних критів і промахів
        player.criticalChance = 1f;
        player.missChance = 1f;

        yield return BeginBattle(player, enemy);
        BattleManager manager = Manager();

        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.True);
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(41));
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));

        enemy.Health.currentHealth = 1;
        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.True);
        yield return WaitUntil(() => manager.State == BattleState.Victory);
        Assert.That(player.CurrentMP, Is.EqualTo(14));

        Assert.That(transition.ReturnAfterVictory(), Is.True);
        yield return WaitUntil(() => !transition.EncounterActive);
        Assert.That(player.CurrentMP, Is.EqualTo(14));
        Assert.That(player.Health.currentHealth, Is.EqualTo(96));
        Assert.That(enemy.gameObject.activeSelf, Is.False);
    }

    // Без мани Fire не працює і хід не забирається
    [UnityTest]
    public IEnumerator InsufficientManaDoesNotConsumeTurnOrSpendMP()
    {
        BattleUnit player = Player();
        player.Health.TakeDamage(5);
        yield return BeginBattle(player, Enemy());

        BattleManager manager = Manager();
        player.TrySpendMP(24);
        int round = manager.Round;

        Assert.That(manager.TryPlayerSpell(Fire(player)), Is.False);
        yield return new WaitForSecondsRealtime(0.25f);
        Assert.That(manager.State, Is.EqualTo(BattleState.AwaitingPlayer));
        Assert.That(manager.Round, Is.EqualTo(round));
        Assert.That(player.CurrentMP, Is.EqualTo(6));

        Assert.That(manager.TryPlayerAction(BattleAction.Defend), Is.True);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP, Is.EqualTo(6));
        Assert.That(player.Inventory.Count(Potion(player)), Is.EqualTo(3));
    }

    // Esc з меню магії і вимкнене заклинання не забирають хід і не витрачають ману
    [UnityTest]
    public IEnumerator MagicEscapeAndDisabledSpellDoNotSpendTurnOrDoubleUseMana()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        yield return BeginBattle(player, enemy);

        BattleManager manager = Manager();
        int round = manager.Round;
        Assert.That(manager.OpenMagicMenu(), Is.True);

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        try
        {
            yield return null;
            yield return null;
            yield return null;

            // Esc - назад у головне меню
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Escape));
            yield return null;
            yield return null;
            Assert.That(manager.ui.commandLabels[0].text, Does.Contain("Attack"));
            Assert.That(manager.Round, Is.EqualTo(round));
            Assert.That(player.CurrentMP, Is.EqualTo(30));

            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;

            // Мани мало - кнопка Fire вимкнена, Enter нічого не робить
            player.TrySpendMP(23);
            manager.OpenMagicMenu();
            yield return null;
            yield return null;
            yield return null;
            Assert.That(manager.ui.commandButtons[0].interactable, Is.False);

            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Enter));
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.That(manager.State, Is.EqualTo(BattleState.AwaitingPlayer));
            Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard);
        }
    }
}

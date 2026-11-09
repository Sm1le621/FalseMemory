using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

// Головний скрипт бою. Керує раундами і ходами,
// приймає команди гравця і рахує результат кожної дії.
public sealed class BattleManager : MonoBehaviour
{
    // Посилання на інші частини бою (призначаються в сцені)
    public BattleUI ui;
    public BattlePresentation presentation;
    public BattleVFXController vfx;
    public BattleAudioController audioController;

    // Поточний стан бою
    public BattleState State { get; private set; } = BattleState.Starting;
    public BattleUnit Player { get; private set; }
    public BattleUnit Enemy { get; private set; }
    // Хто зараз ходить
    public BattleUnit CurrentActor { get; private set; }
    // Номер раунду
    public int Round { get; private set; }

    private BattleRules rules;
    private BattleTransition transition;
    // Не дає прийняти команду гравця двічі
    private readonly BattleCommandGate gate = new BattleCommandGate();
    // Черга ходів
    private readonly BattleTurnOrder<BattleUnit> turns = new BattleTurnOrder<BattleUnit>();
    // Вибрана дія. null - ще нічого не вибрано
    private BattleAction? pendingAction;
    // Вибране заклинання (якщо дія - магія)
    private BattleSpell pendingSpell;
    // Джерело випадкових чисел (у тестах підміняється)
    private IBattleRandom random = UnityBattleRandom.Instance;

    // Підмінити рандом (використовується в тестах)
    public void SetRandomSource(IBattleRandom source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        random = source;
    }

    // Start як корутина: весь бій - це один довгий цикл з паузами
    private IEnumerator Start()
    {
        transition = BattleTransition.Instance;

        // Якщо сцену бою відкрили напряму (без зіткнення з ворогом) - режим перегляду
        if (transition == null || !transition.EncounterActive)
        {
            ui.ShowMessage("Preview mode. Start from the map and touch the skeleton to fight.");

            string[] previewNames = new string[] { "Explore Map", "Main Menu" };
            Action[] previewActions = new Action[]
            {
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene"),
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu")
            };
            ui.ShowMenu(previewNames, previewActions);
            yield break;
        }

        // Беремо учасників і правила з переходу
        Player = transition.Player;
        Enemy = transition.Enemy;
        rules = transition.rules;

        ui.ShowUnits(Player, Enemy);
        if (presentation != null)
        {
            presentation.Bind(Player, Enemy);
        }
        if (ui.hud != null)
        {
            ui.hud.HideForEntrance();
        }
        ui.ShowMessage("A skeleton bars your path.");
        ui.SetCommandsEnabled(false);

        // Чекаємо, поки закінчиться затемнення екрана
        while (transition.IsTransitioning)
        {
            yield return null;
        }

        if (ui.hud != null)
        {
            yield return ui.hud.RevealScreen(rules.actionDelay);
        }
        yield return Pause(false);

        // ===== Головний цикл бою: раунд за раундом, поки обидва живі =====
        while (Player.IsAlive && Enemy.IsAlive)
        {
            Round = Round + 1;
            turns.BeginRound(new BattleUnit[] { Player, Enemy });

            BattleUnit actor;
            while (turns.TryNext(out actor))
            {
                CurrentActor = actor;
                // Захист тримається рівно до початку свого наступного ходу
                actor.BeginTurn();
                ui.RefreshStats(Player, Enemy, Round);

                if (actor == Player)
                {
                    // --- Хід гравця: показуємо меню і чекаємо вибору ---
                    State = BattleState.AwaitingPlayer;
                    pendingAction = null;
                    pendingSpell = null;
                    ui.ShowMessage("Your turn.");
                    gate.Open();
                    ShowMainMenu();

                    while (!pendingAction.HasValue && Player.IsAlive && Enemy.IsAlive)
                    {
                        yield return null;
                    }
                }
                else
                {
                    // --- Хід ворога: "думає" і вибирає дію ---
                    State = BattleState.Resolving;
                    ui.ShowMessage(Enemy.displayName + " is thinking...");
                    yield return Pause(true);

                    if (!Player.IsAlive || !Enemy.IsAlive)
                    {
                        break;
                    }
                    pendingAction = EnemyBattleAI.Choose(rules, random);
                }

                if (!Player.IsAlive || !Enemy.IsAlive)
                {
                    break;
                }

                // --- Виконуємо вибрану дію ---
                State = BattleState.Resolving;
                gate.Close();
                ui.SetCommandsEnabled(false);

                // Ціль - той, хто НЕ ходить
                BattleUnit target;
                if (actor == Player)
                {
                    target = Enemy;
                }
                else
                {
                    target = Player;
                }

                bool hit = false;
                bool criticalHit = false;

                if (pendingAction == BattleAction.Attack && audioController != null)
                {
                    audioController.Play(BattleSound.Attack);
                }

                // Анімація замаху (для втечі не потрібна)
                if (presentation != null && pendingAction != BattleAction.Flee)
                {
                    yield return presentation.Windup(actor == Player, pendingAction.Value, rules.actionDelay);
                }

                // Поки йшла анімація, ціль могла померти. Тоді нічого не витрачаємо
                if (!Player.IsAlive || !Enemy.IsAlive)
                {
                    if (presentation != null)
                    {
                        presentation.ResetPoses();
                    }
                    break;
                }

                if (pendingAction == BattleAction.Flee)
                {
                    // ВТЕЧА
                    if (audioController != null)
                    {
                        audioController.Play(BattleSound.Flee);
                    }

                    float fleeChance = BattleCalculator.FleeChance(Player.Speed, Enemy.Speed, rules);
                    bool escaped = BattleCalculator.ChanceSucceeds(fleeChance, random.Next01());

                    if (escaped)
                    {
                        State = BattleState.Escaped;
                        CurrentActor = null;
                        ui.ShowMessage("Escaped! Returning to the map.");
                    }
                    else
                    {
                        ui.ShowMessage("Couldn't escape!");
                    }
                }
                else if (pendingAction == BattleAction.Defend)
                {
                    // ЗАХИСТ
                    actor.Defend();
                    if (audioController != null)
                    {
                        audioController.Play(BattleSound.Defend);
                    }

                    // Наприклад, множник 0.5 -> "зменшено на 50%"
                    int reduction = Mathf.RoundToInt((1f - Mathf.Clamp01(rules.defendMultiplier)) * 100f);
                    ui.ShowMessage(actor.displayName + " defends. Physical damage reduced by " + reduction + "%.");
                }
                else if (pendingAction == BattleAction.Magic)
                {
                    // МАГІЯ
                    int amount;
                    if (BattleAbilities.TryCast(actor, target, pendingSpell, rules, out amount))
                    {
                        if (audioController != null)
                        {
                            audioController.Play(BattleSound.Fire);
                        }
                        hit = true;
                        ui.ShowMessage(actor.displayName + " casts " + pendingSpell.displayName + ": " + amount + " damage.");
                        ui.ShowFeedback(target == Player, "−" + amount + " " + pendingSpell.displayName, pendingSpell.effectColor, false);

                        if (vfx != null)
                        {
                            vfx.Play(target == Player, pendingSpell.visualEffect, pendingSpell.effectColor);
                        }
                    }
                }
                else
                {
                    // ЗВИЧАЙНА АТАКА
                    PhysicalAttackResult result = BattleCalculator.ResolvePhysical(actor.EffectiveAttack, target.EffectiveDefense,
                        target.IsDefending, rules, actor.criticalChance, actor.missChance, random);

                    if (result.Missed)
                    {
                        // Промах
                        if (audioController != null)
                        {
                            audioController.Play(BattleSound.Miss);
                        }
                        ui.ShowMessage(actor.displayName + " attacks: MISS.");
                        ui.ShowFeedback(target == Player, "MISS", Color.white);
                    }
                    else
                    {
                        // Влучання
                        target.Health.TakeDamage(result.Damage);
                        hit = true;
                        criticalHit = result.Critical;

                        if (audioController != null)
                        {
                            if (result.Critical)
                            {
                                audioController.Play(BattleSound.Critical);
                            }
                            else
                            {
                                audioController.Play(BattleSound.Hit);
                            }
                        }

                        string message = "";
                        string feedback = "";
                        Color feedbackColor;
                        if (result.Critical)
                        {
                            message = "CRITICAL! ";
                            feedback = "CRITICAL!\n";
                            feedbackColor = new Color(1f, 0.82f, 0.2f);
                        }
                        else
                        {
                            feedbackColor = new Color(1f, 0.4f, 0.4f);
                        }
                        message = message + actor.displayName + " attacks " + target.displayName + ": " + result.Damage + " damage.";
                        feedback = feedback + "−" + result.Damage;

                        ui.ShowMessage(message);
                        ui.ShowFeedback(target == Player, feedback, feedbackColor);
                    }
                }

                ui.RefreshStats(Player, Enemy, Round);

                // Анімація повернення на місце / реакція на удар
                if (presentation != null)
                {
                    yield return presentation.Recover(actor == Player, hit, criticalHit, rules.actionDelay);
                }
                yield return Pause(false);

                // Якщо втекли - повертаємось у світ і закінчуємо бій
                if (State == BattleState.Escaped)
                {
                    if (presentation != null)
                    {
                        yield return presentation.Finish(BattleState.Escaped, rules.actionDelay);
                    }
                    transition.ReturnAfterEscape();
                    // Ні ходу ворога, ні нагороди, ні наступного раунду
                    yield break;
                }

                if (!Player.IsAlive || !Enemy.IsAlive)
                {
                    break;
                }
            }
        }

        // ===== Бій закінчився =====
        CurrentActor = null;
        gate.Close();
        ui.SetCommandsEnabled(false);

        if (Player.IsAlive)
        {
            // ПЕРЕМОГА
            if (presentation != null)
            {
                yield return presentation.Finish(BattleState.Victory, rules.actionDelay);
            }
            State = BattleState.Victory;
            if (audioController != null)
            {
                audioController.Play(BattleSound.Victory);
            }
            ui.RefreshStats(Player, Enemy, Round);
            ui.ShowVictoryResults(Player);
            ui.ShowMessage("Victory! The skeleton is defeated.");

            string[] victoryNames = new string[] { "Return to Map" };
            Action[] victoryActions = new Action[]
            {
                () =>
                {
                    if (transition.ReturnAfterVictory())
                    {
                        ui.SetCommandsEnabled(false);
                    }
                }
            };
            ui.ShowMenu(victoryNames, victoryActions);
        }
        else
        {
            // ПОРАЗКА
            if (presentation != null)
            {
                yield return presentation.Finish(BattleState.Defeat, rules.actionDelay);
            }
            State = BattleState.Defeat;
            if (audioController != null)
            {
                audioController.Play(BattleSound.Defeat);
            }
            ui.RefreshStats(Player, Enemy, Round);
            ui.ShowMessage("Defeat. Your journey ends here.");
            ui.ShowDefeatResults();

            string[] defeatNames = new string[] { "New Game", "Main Menu" };
            Action[] defeatActions = new Action[]
            {
                () =>
                {
                    ui.SetCommandsEnabled(false);
                    transition.RestartGame();
                },
                () =>
                {
                    ui.SetCommandsEnabled(false);
                    transition.ReturnToMainMenu();
                }
            };
            ui.ShowMenu(defeatNames, defeatActions);
        }
    }

    // Пауза, щоб гравець встиг прочитати повідомлення
    private IEnumerator Pause(bool enemyThinking)
    {
        if (presentation != null && presentation.isActiveAndEnabled)
        {
            yield return presentation.ReadPause(enemyThinking, rules.actionDelay);
        }
        else
        {
            yield return new WaitForSecondsRealtime(rules.actionDelay);
        }
    }

    // Гравець вибрав дію (атака, захист або втеча). Магія - через TryPlayerSpell
    public bool TryPlayerAction(BattleAction action)
    {
        if (!CanChoose)
        {
            return false;
        }
        if (action != BattleAction.Attack && action != BattleAction.Defend && action != BattleAction.Flee)
        {
            return false;
        }
        // Важливо: перевіряємо ворота ОСТАННІМИ, бо TryConsume їх закриває
        if (!gate.TryConsume())
        {
            return false;
        }

        pendingAction = action;
        // Закриваємо меню одразу, а не в наступному кадрі корутини
        State = BattleState.Resolving;
        ui.SetCommandsEnabled(false);
        return true;
    }

    // Чи може гравець зараз щось вибрати
    private bool CanChoose
    {
        get
        {
            return State == BattleState.AwaitingPlayer && CurrentActor == Player && Player.IsAlive && Enemy.IsAlive && gate.IsOpen;
        }
    }

    // Показати головне меню команд
    public bool ShowMainMenu()
    {
        if (!CanChoose)
        {
            return false;
        }

        ui.ShowMessage("Your turn.");

        string[] names = new string[] { "Attack", "Magic", "Defend", "Flee" };
        Action[] actions = new Action[]
        {
            () => TryPlayerAction(BattleAction.Attack),
            () => OpenMagicMenu(),
            () => TryPlayerAction(BattleAction.Defend),
            () => TryPlayerAction(BattleAction.Flee)
        };
        ui.ShowMenu(names, actions);
        return true;
    }

    // Показати меню заклинань
    public bool OpenMagicMenu()
    {
        if (!CanChoose)
        {
            return false;
        }

        List<string> names = new List<string>();
        List<Action> actions = new List<Action>();
        List<bool> enabled = new List<bool>();

        BattleSpell[] spellList = Player.spells;
        if (spellList == null)
        {
            spellList = Array.Empty<BattleSpell>();
        }

        foreach (BattleSpell spell in spellList)
        {
            if (spell == null)
            {
                continue;
            }

            // Копія в окрему змінну, щоб кожна кнопка "запам'ятала" своє заклинання
            BattleSpell entry = spell;
            names.Add(entry.displayName + "   " + entry.mpCost + " MP");
            actions.Add(() => TryPlayerSpell(entry));

            // На кого діє заклинання
            BattleUnit spellTarget;
            if (entry.target == BattleTarget.Self)
            {
                spellTarget = Player;
            }
            else
            {
                spellTarget = Enemy;
            }
            enabled.Add(BattleAbilities.CanCast(Player, spellTarget, entry));
        }

        if (names.Count == 0)
        {
            ui.ShowMessage("No spells available.");
        }
        else
        {
            ui.ShowMessage("Choose a spell. Esc to go back.");
        }

        // Остання кнопка - "Назад"
        names.Add("Back");
        actions.Add(() => ShowMainMenu());
        enabled.Add(true);

        ui.ShowMenu(names.ToArray(), actions.ToArray(), enabled.ToArray(), () => ShowMainMenu());
        return true;
    }

    // Гравець вибрав заклинання
    public bool TryPlayerSpell(BattleSpell spell)
    {
        if (!CanChoose)
        {
            return false;
        }
        if (spell == null || Player.spells == null)
        {
            return false;
        }
        // Заклинання має бути у списку гравця
        if (Array.IndexOf(Player.spells, spell) < 0)
        {
            return false;
        }

        BattleUnit spellTarget;
        if (spell.target == BattleTarget.Self)
        {
            spellTarget = Player;
        }
        else
        {
            spellTarget = Enemy;
        }
        if (!BattleAbilities.CanCast(Player, spellTarget, spell))
        {
            return false;
        }

        // Ворота - останніми (TryConsume їх закриває)
        if (!gate.TryConsume())
        {
            return false;
        }

        pendingSpell = spell;
        pendingAction = BattleAction.Magic;
        State = BattleState.Resolving;
        ui.SetCommandsEnabled(false);
        return true;
    }
}

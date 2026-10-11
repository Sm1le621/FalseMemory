using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class BattleManager : MonoBehaviour
{
    public BattleUI ui;
    public BattlePresentation presentation;
    public BattleVFXController vfx;
    public BattleState State { get; private set; } = BattleState.Starting;
    public BattleUnit Player { get; private set; }
    public BattleUnit Enemy { get; private set; }
    public BattleUnit CurrentActor { get; private set; }
    public int Round { get; private set; }

    private BattleRules rules;
    private BattleTransition transition;
    private readonly BattleCommandGate gate = new BattleCommandGate();
    private readonly BattleTurnOrder<BattleUnit> turns = new BattleTurnOrder<BattleUnit>();
    private BattleAction? pendingAction;
    private BattleSpell pendingSpell;
    private BattleConsumable pendingItem;
    private IBattleRandom random = UnityBattleRandom.Instance;
    public void SetRandomSource(IBattleRandom source) => random = source ?? throw new ArgumentNullException(nameof(source));

    private IEnumerator Start()
    {
        transition = BattleTransition.Instance;
        if (transition == null || !transition.EncounterActive)
        {
            ui.ShowMessage("Для боя запустите SampleScene и коснитесь скелета.");
            ui.ShowMenu(new[] { "Исследовать", "Главное меню" }, new System.Action[] {
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene"),
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu") });
            yield break;
        }
        Player = transition.Player; Enemy = transition.Enemy; rules = transition.rules;
        ui.ShowUnits(Player, Enemy);
        if (presentation != null) presentation.Bind(Player, Enemy);
        ui.ShowMessage("Скелет преграждает путь.");
        ui.SetCommandsEnabled(false);
        while (transition.IsTransitioning) yield return null;
        yield return new WaitForSecondsRealtime(rules.actionDelay);

        while (Player.IsAlive && Enemy.IsAlive)
        {
            Round++;
            turns.BeginRound(new[] { Player, Enemy });
            while (turns.TryNext(out var actor))
            {
                CurrentActor = actor;
                actor.BeginTurn(); // Defense survives until this exact point.
                ui.RefreshStats(Player, Enemy, Round);
                if (actor == Player)
                {
                    State = BattleState.AwaitingPlayer;
                    pendingAction = null;
                    pendingSpell = null;
                    pendingItem = null;
                    ui.ShowMessage("Ваш ход. Выберите действие.");
                    gate.Open();
                    ShowMainMenu();
                    while (!pendingAction.HasValue && Player.IsAlive && Enemy.IsAlive) yield return null;
                }
                else
                {
                    State = BattleState.Resolving;
                    ui.ShowMessage(Enemy.displayName + " выбирает действие...");
                    yield return new WaitForSecondsRealtime(rules.actionDelay);
                    if (!Player.IsAlive || !Enemy.IsAlive) break;
                    pendingAction = EnemyBattleAI.Choose(rules, random);
                }

                if (!Player.IsAlive || !Enemy.IsAlive) break;
                State = BattleState.Resolving;
                gate.Close();
                ui.SetCommandsEnabled(false);
                var target = actor == Player ? Enemy : Player;
                bool hit = false, criticalHit = false;
                if (presentation != null && pendingAction != BattleAction.Flee)
                    yield return presentation.Windup(actor == Player, pendingAction.Value, rules.actionDelay);
                // A target may die while the visual windup runs. Never spend resources then.
                if (!Player.IsAlive || !Enemy.IsAlive) { if (presentation != null) presentation.ResetPoses(); break; }
                if (pendingAction == BattleAction.Flee)
                {
                    bool escaped = BattleCalculator.ChanceSucceeds(BattleCalculator.FleeChance(Player.Speed, Enemy.Speed, rules), random.Next01());
                    if (escaped) { State = BattleState.Escaped; CurrentActor = null; }
                    ui.ShowMessage(escaped ? "Escaped! Возвращение на карту." : "Couldn't escape! Побег не удался.");
                }
                else if (pendingAction == BattleAction.Defend)
                {
                    actor.Defend();
                    int reduction = Mathf.RoundToInt((1f - Mathf.Clamp01(rules.defendMultiplier)) * 100f);
                    ui.ShowMessage(actor.displayName + " защищается. Физический урон снижен на " + reduction + "%.");
                }
                else if (pendingAction == BattleAction.Items)
                {
                    var itemTarget = pendingItem.target == BattleTarget.Self ? actor : target;
                    if (BattleAbilities.TryUseItem(actor, itemTarget, pendingItem, out int amount))
                    {
                        ui.ShowMessage(actor.displayName + " использует " + pendingItem.displayName + ": +" + amount + " HP.");
                        ui.ShowFeedback(itemTarget == Player, "+" + amount + " HP", pendingItem.effectColor);
                        if (vfx != null) vfx.Play(itemTarget == Player, null, pendingItem.effectColor, true);
                    }
                }
                else if (pendingAction == BattleAction.Magic)
                {
                    var spellTarget = pendingSpell.target == BattleTarget.Self ? actor : target;
                    if (BattleAbilities.TryCast(actor, spellTarget, pendingSpell, rules, out int amount))
                    {
                        bool healing = pendingSpell.effect == BattleEffect.RestoreHealth;
                        hit = !healing;
                        ui.ShowMessage(actor.displayName + " использует " + pendingSpell.displayName + ": " +
                            (healing ? "+" + amount + " HP." : amount + " магического урона."));
                        ui.ShowFeedback(spellTarget == Player, (healing ? "+" : "−") + amount + (healing ? " HP" : " " + pendingSpell.displayName), pendingSpell.effectColor);
                        if (vfx != null) vfx.Play(spellTarget == Player, pendingSpell.visualEffect, pendingSpell.effectColor, healing);
                    }
                }
                else
                {
                    var result = BattleCalculator.ResolvePhysical(actor.attack, target.defense, target.IsDefending, rules,
                        actor.criticalChance, actor.missChance, random);
                    if (result.Missed)
                    {
                        ui.ShowMessage(actor.displayName + " атакует: MISS.");
                        ui.ShowFeedback(target == Player, "MISS", Color.white);
                    }
                    else
                    {
                        target.Health.TakeDamage(result.Damage);
                        hit = true; criticalHit = result.Critical;
                        ui.ShowMessage((result.Critical ? "CRITICAL! " : "") + actor.displayName + " атакует " + target.displayName + ": " + result.Damage + " урона.");
                        ui.ShowFeedback(target == Player, (result.Critical ? "CRITICAL!\n" : "") + "−" + result.Damage,
                            result.Critical ? new Color(1f, 0.82f, 0.2f) : new Color(1f, 0.4f, 0.4f));
                    }
                }
                ui.RefreshStats(Player, Enemy, Round);
                if (presentation != null) yield return presentation.Recover(actor == Player, hit, criticalHit, rules.actionDelay);
                yield return new WaitForSecondsRealtime(rules.actionDelay);
                if (State == BattleState.Escaped)
                {
                    transition.ReturnAfterEscape();
                    yield break; // No enemy action, victory reward or further round.
                }
                if (!Player.IsAlive || !Enemy.IsAlive) break;
            }
        }
        CurrentActor = null;
        gate.Close();
        if (Player.IsAlive)
        {
            State = BattleState.Victory;
            transition.TryGrantVictoryExperience(out int gained);
            ui.RefreshStats(Player, Enemy, Round);
            ui.ShowVictoryResults(gained, Player);
            ui.ShowMessage("Victory — скелет побеждён!\nHP, MP, предметы и EXP сохранятся при возвращении.");
            ui.ShowMenu(new[] { "Вернуться на карту" }, new System.Action[] {
                () => { if (transition.ReturnAfterVictory()) ui.SetCommandsEnabled(false); } });
        }
        else
        {
            State = BattleState.Defeat;
            ui.RefreshStats(Player, Enemy, Round);
            ui.ShowMessage("Game Over — герой пал.");
            ui.ShowMenu(new[] { "Начать заново", "Главное меню" }, new System.Action[] {
                () => { ui.SetCommandsEnabled(false); transition.RestartGame(); },
                () => { ui.SetCommandsEnabled(false); transition.ReturnToMainMenu(); } });
        }
    }

    public bool TryPlayerAction(BattleAction action)
    {
        if (!CanChoose ||
            (action != BattleAction.Attack && action != BattleAction.Defend && action != BattleAction.Flee) || !gate.TryConsume()) return false;
        pendingAction = action;
        // Close immediately, not on the next coroutine update.
        State = BattleState.Resolving;
        ui.SetCommandsEnabled(false);
        return true;
    }

    private bool CanChoose => State == BattleState.AwaitingPlayer && CurrentActor == Player && Player.IsAlive && Enemy.IsAlive && gate.IsOpen;

    public bool ShowMainMenu()
    {
        if (!CanChoose) return false;
        ui.ShowMessage("Ваш ход. Выберите действие.");
        ui.ShowMenu(new[] {"Attack", "Magic", "Items", "Defend", "Flee"}, new Action[] {
            () => TryPlayerAction(BattleAction.Attack), () => OpenMagicMenu(), () => OpenItemsMenu(),
            () => TryPlayerAction(BattleAction.Defend), () => TryPlayerAction(BattleAction.Flee) });
        return true;
    }

    public bool OpenMagicMenu()
    {
        if (!CanChoose) return false;
        var names = new List<string>(); var actions = new List<Action>(); var enabled = new List<bool>();
        foreach (var spell in Player.spells ?? Array.Empty<BattleSpell>())
        {
            if (spell == null) continue;
            var entry = spell;
            names.Add(entry.displayName + "  " + entry.mpCost + " MP");
            actions.Add(() => TryPlayerSpell(entry));
            enabled.Add(BattleAbilities.CanCast(Player, entry.target == BattleTarget.Self ? Player : Enemy, entry));
        }
        if (names.Count == 0) ui.ShowMessage("Нет доступных заклинаний.");
        else ui.ShowMessage("Выберите заклинание. Esc — назад.");
        names.Add("Назад"); actions.Add(() => ShowMainMenu()); enabled.Add(true);
        ui.ShowMenu(names.ToArray(), actions.ToArray(), enabled.ToArray(), () => ShowMainMenu());
        return true;
    }

    public bool TryPlayerSpell(BattleSpell spell)
    {
        if (!CanChoose || spell == null || Player.spells == null || Array.IndexOf(Player.spells, spell) < 0 ||
            !BattleAbilities.CanCast(Player, spell.target == BattleTarget.Self ? Player : Enemy, spell) || !gate.TryConsume()) return false;
        pendingSpell = spell; pendingAction = BattleAction.Magic;
        State = BattleState.Resolving; ui.SetCommandsEnabled(false);
        return true;
    }

    public bool OpenItemsMenu()
    {
        if (!CanChoose) return false;
        var names = new List<string>(); var actions = new List<Action>(); var enabled = new List<bool>();
        bool anyAvailable = false;
        if (Player.Inventory != null)
        {
            foreach (var item in Player.Inventory.Definitions)
            {
                var entry = item;
                names.Add(entry.displayName + " ×" + Player.Inventory.Count(entry));
                actions.Add(() => TryPlayerItem(entry));
                bool canUse = BattleAbilities.CanUseItem(Player, entry.target == BattleTarget.Self ? Player : Enemy, entry);
                enabled.Add(canUse); anyAvailable |= canUse;
            }
        }
        ui.ShowMessage(anyAvailable ? "Выберите предмет. Esc — назад." : "Предметов нет. Esc — назад.");
        names.Add("Назад"); actions.Add(() => ShowMainMenu()); enabled.Add(true);
        ui.ShowMenu(names.ToArray(), actions.ToArray(), enabled.ToArray(), () => ShowMainMenu());
        return true;
    }

    public bool TryPlayerItem(BattleConsumable item)
    {
        if (!CanChoose || item == null || !BattleAbilities.CanUseItem(Player, item.target == BattleTarget.Self ? Player : Enemy, item) ||
            !gate.TryConsume()) return false;
        pendingItem = item; pendingAction = BattleAction.Items;
        State = BattleState.Resolving; ui.SetCommandsEnabled(false);
        return true;
    }
}

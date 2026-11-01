using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class BattleManager : MonoBehaviour
{
    public BattleUI ui;
    public BattlePresentation presentation;
    public BattleVFXController vfx;
    public BattleAudioController audioController;
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
    private IBattleRandom random = UnityBattleRandom.Instance;
    public void SetRandomSource(IBattleRandom source) => random = source ?? throw new ArgumentNullException(nameof(source));

    private IEnumerator Start()
    {
        transition = BattleTransition.Instance;
        if (transition == null || !transition.EncounterActive)
        {
            ui.ShowMessage("Preview mode. Start from the map and touch the skeleton to fight.");
            ui.ShowMenu(new[] { "Explore Map", "Main Menu" }, new System.Action[] {
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene"),
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu") });
            yield break;
        }
        Player = transition.Player; Enemy = transition.Enemy; rules = transition.rules;
        ui.ShowUnits(Player, Enemy);
        if (presentation != null) presentation.Bind(Player, Enemy);
        if (ui.hud != null) ui.hud.HideForEntrance();
        ui.ShowMessage("A skeleton bars your path.");
        ui.SetCommandsEnabled(false);
        while (transition.IsTransitioning) yield return null;
        if (ui.hud != null) yield return ui.hud.RevealScreen(rules.actionDelay);
        yield return Pause(false);

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
                    ui.ShowMessage("Your turn.");
                    gate.Open();
                    ShowMainMenu();
                    while (!pendingAction.HasValue && Player.IsAlive && Enemy.IsAlive) yield return null;
                }
                else
                {
                    State = BattleState.Resolving;
                    ui.ShowMessage(Enemy.displayName + " is thinking...");
                    yield return Pause(true);
                    if (!Player.IsAlive || !Enemy.IsAlive) break;
                    pendingAction = EnemyBattleAI.Choose(rules, random);
                }

                if (!Player.IsAlive || !Enemy.IsAlive) break;
                State = BattleState.Resolving;
                gate.Close();
                ui.SetCommandsEnabled(false);
                var target = actor == Player ? Enemy : Player;
                bool hit = false, criticalHit = false;
                if (pendingAction == BattleAction.Attack && audioController != null) audioController.Play(BattleSound.Attack);
                if (presentation != null && pendingAction != BattleAction.Flee)
                    yield return presentation.Windup(actor == Player, pendingAction.Value, rules.actionDelay);
                // A target may die while the visual windup runs. Never spend resources then.
                if (!Player.IsAlive || !Enemy.IsAlive) { if (presentation != null) presentation.ResetPoses(); break; }
                if (pendingAction == BattleAction.Flee)
                {
                    if (audioController != null) audioController.Play(BattleSound.Flee);
                    bool escaped = BattleCalculator.ChanceSucceeds(BattleCalculator.FleeChance(Player.Speed, Enemy.Speed, rules), random.Next01());
                    if (escaped) { State = BattleState.Escaped; CurrentActor = null; }
                    ui.ShowMessage(escaped ? "Escaped! Returning to the map." : "Couldn't escape!");
                }
                else if (pendingAction == BattleAction.Defend)
                {
                    actor.Defend();
                    if (audioController != null) audioController.Play(BattleSound.Defend);
                    int reduction = Mathf.RoundToInt((1f - Mathf.Clamp01(rules.defendMultiplier)) * 100f);
                    ui.ShowMessage(actor.displayName + " defends. Physical damage reduced by " + reduction + "%.");
                }
                else if (pendingAction == BattleAction.Magic)
                {
                    if (BattleAbilities.TryCast(actor, target, pendingSpell, rules, out int amount))
                    {
                        if (audioController != null) audioController.Play(BattleSound.Fire);
                        hit = true;
                        ui.ShowMessage(actor.displayName + " casts " + pendingSpell.displayName + ": " + amount + " damage.");
                        ui.ShowFeedback(target == Player, "−" + amount + " " + pendingSpell.displayName, pendingSpell.effectColor, false);
                        if (vfx != null) vfx.Play(target == Player, pendingSpell.visualEffect, pendingSpell.effectColor, false);
                    }
                }
                else
                {
                    var result = BattleCalculator.ResolvePhysical(actor.EffectiveAttack, target.EffectiveDefense, target.IsDefending, rules,
                        actor.criticalChance, actor.missChance, random);
                    if (result.Missed)
                    {
                        if (audioController != null) audioController.Play(BattleSound.Miss);
                        ui.ShowMessage(actor.displayName + " attacks: MISS.");
                        ui.ShowFeedback(target == Player, "MISS", Color.white);
                    }
                    else
                    {
                        target.Health.TakeDamage(result.Damage);
                        hit = true; criticalHit = result.Critical;
                        if (audioController != null) audioController.Play(result.Critical ? BattleSound.Critical : BattleSound.Hit);
                        ui.ShowMessage((result.Critical ? "CRITICAL! " : "") + actor.displayName + " attacks " + target.displayName + ": " + result.Damage + " damage.");
                        ui.ShowFeedback(target == Player, (result.Critical ? "CRITICAL!\n" : "") + "−" + result.Damage,
                            result.Critical ? new Color(1f, 0.82f, 0.2f) : new Color(1f, 0.4f, 0.4f));
                    }
                }
                ui.RefreshStats(Player, Enemy, Round);
                if (presentation != null) yield return presentation.Recover(actor == Player, hit, criticalHit, rules.actionDelay);
                yield return Pause(false);
                if (State == BattleState.Escaped)
                {
                    if(presentation!=null)yield return presentation.Finish(BattleState.Escaped,rules.actionDelay);
                    transition.ReturnAfterEscape();
                    yield break; // No enemy action, victory reward or further round.
                }
                if (!Player.IsAlive || !Enemy.IsAlive) break;
            }
        }
        CurrentActor = null;
        gate.Close();
        ui.SetCommandsEnabled(false);
        if (Player.IsAlive)
        {
            if(presentation!=null)yield return presentation.Finish(BattleState.Victory,rules.actionDelay);
            State = BattleState.Victory;
            if (audioController != null) audioController.Play(BattleSound.Victory);
            ui.RefreshStats(Player, Enemy, Round);
            ui.ShowVictoryResults(Player);
            ui.ShowMessage("Victory! The skeleton is defeated.");
            ui.ShowMenu(new[] { "Return to Map" }, new System.Action[] {
                () => { if (transition.ReturnAfterVictory()) ui.SetCommandsEnabled(false); } });
        }
        else
        {
            if(presentation!=null)yield return presentation.Finish(BattleState.Defeat,rules.actionDelay);
            State = BattleState.Defeat;
            if (audioController != null) audioController.Play(BattleSound.Defeat);
            ui.RefreshStats(Player, Enemy, Round);
            ui.ShowMessage("Defeat. Your journey ends here.");
            ui.ShowDefeatResults();
            ui.ShowMenu(new[] { "New Game", "Main Menu" }, new System.Action[] {
                () => { ui.SetCommandsEnabled(false); transition.RestartGame(); },
                () => { ui.SetCommandsEnabled(false); transition.ReturnToMainMenu(); } });
        }
    }

    private IEnumerator Pause(bool enemyThinking)
    {
        if (presentation != null && presentation.isActiveAndEnabled)
            yield return presentation.ReadPause(enemyThinking, rules.actionDelay);
        else yield return new WaitForSecondsRealtime(rules.actionDelay);
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
        ui.ShowMessage("Your turn.");
        ui.ShowMenu(new[] { "Attack", "Magic", "Defend", "Flee" }, new Action[] {
            () => TryPlayerAction(BattleAction.Attack), () => OpenMagicMenu(),
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
            names.Add(entry.displayName + "   " + entry.mpCost + " MP");
            actions.Add(() => TryPlayerSpell(entry));
            enabled.Add(BattleAbilities.CanCast(Player, entry.target == BattleTarget.Self ? Player : Enemy, entry));
        }
        if (names.Count == 0) ui.ShowMessage("No spells available.");
        else ui.ShowMessage("Choose a spell. Esc to go back.");
        names.Add("Back"); actions.Add(() => ShowMainMenu()); enabled.Add(true);
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
}

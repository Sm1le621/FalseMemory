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
    private BattleConsumable pendingItem;
    private IBattleRandom random = UnityBattleRandom.Instance;
    public void SetRandomSource(IBattleRandom source) => random = source ?? throw new ArgumentNullException(nameof(source));

    private IEnumerator Start()
    {
        transition = BattleTransition.Instance;
        if (transition == null || !transition.EncounterActive)
        {
            ui.ShowMessage(GameText.Get("battle.preview"));
            ui.ShowMenu(new[] { GameText.Get("menu.explore"), GameText.Get("menu.main") }, new System.Action[] {
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene"),
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu") });
            yield break;
        }
        Player = transition.Player; Enemy = transition.Enemy; rules = transition.rules;
        ui.ShowUnits(Player, Enemy);
        if (presentation != null) presentation.Bind(Player, Enemy);
        if (ui.hud != null) ui.hud.HideForEntrance();
        ui.ShowMessage(GameText.Get("battle.encounter"));
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
                    pendingItem = null;
                    ui.ShowMessage(GameText.Get("battle.your_turn"));
                    gate.Open();
                    ShowMainMenu();
                    while (!pendingAction.HasValue && Player.IsAlive && Enemy.IsAlive) yield return null;
                }
                else
                {
                    State = BattleState.Resolving;
                    ui.ShowMessage(GameText.Get("battle.thinking", Enemy.LocalizedName));
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
                    ui.ShowMessage(escaped ? GameText.Get("battle.escaped") : GameText.Get("battle.escape_failed"));
                }
                else if (pendingAction == BattleAction.Defend)
                {
                    actor.Defend();
                    if (audioController != null) audioController.Play(BattleSound.Defend);
                    int reduction = Mathf.RoundToInt((1f - Mathf.Clamp01(rules.defendMultiplier)) * 100f);
                    ui.ShowMessage(GameText.Get("battle.defend", actor.LocalizedName, reduction));
                }
                else if (pendingAction == BattleAction.Items)
                {
                    var itemTarget = pendingItem.target == BattleTarget.Self ? actor : target;
                    if (BattleAbilities.TryUseItem(actor, itemTarget, pendingItem, out int amount))
                    {
                        if (audioController != null) audioController.Play(BattleSound.Potion);
                        string resource=pendingItem.manaRestoration>0?"MP":"HP";
                        ui.ShowMessage(GameText.Get("battle.item_used", actor.LocalizedName, pendingItem.LocalizedName, amount, resource));
                        ui.ShowFeedback(itemTarget == Player, "+" + amount + " "+resource, pendingItem.effectColor,false);
                        if (vfx != null) vfx.Play(itemTarget == Player, pendingItem.visualEffect, pendingItem.effectColor, true);
                    }
                }
                else if (pendingAction == BattleAction.Magic)
                {
                    var spellTarget = pendingSpell.target == BattleTarget.Self ? actor : target;
                    if (BattleAbilities.TryCast(actor, spellTarget, pendingSpell, rules, out int amount))
                    {
                        bool healing = pendingSpell.effect == BattleEffect.RestoreHealth;
                        if (audioController != null) audioController.Play(healing ? BattleSound.Heal : BattleSound.Fire);
                        hit = !healing;
                        ui.ShowMessage(healing ? GameText.Get("battle.spell_heal",actor.LocalizedName,pendingSpell.LocalizedName,amount) : GameText.Get("battle.spell_damage",actor.LocalizedName,pendingSpell.LocalizedName,amount));
                        ui.ShowFeedback(spellTarget == Player, (healing ? "+" : "−") + amount + (healing ? " HP" : " " + pendingSpell.LocalizedName), pendingSpell.effectColor,false);
                        if (vfx != null) vfx.Play(spellTarget == Player, pendingSpell.visualEffect, pendingSpell.effectColor, healing);
                    }
                }
                else
                {
                    var result = BattleCalculator.ResolvePhysical(actor.EffectiveAttack, target.EffectiveDefense, target.IsDefending, rules,
                        actor.criticalChance, actor.missChance, random);
                    if (result.Missed)
                    {
                        if (audioController != null) audioController.Play(BattleSound.Miss);
                        ui.ShowMessage(GameText.Get("battle.missed", actor.LocalizedName));
                        ui.ShowFeedback(target == Player, GameText.Get("battle.miss"), Color.white);
                    }
                    else
                    {
                        target.Health.TakeDamage(result.Damage);
                        hit = true; criticalHit = result.Critical;
                        if (audioController != null) audioController.Play(result.Critical ? BattleSound.Critical : BattleSound.Hit);
                        ui.ShowMessage((result.Critical ? GameText.Get("battle.critical")+" " : "") + GameText.Get("battle.attack", actor.LocalizedName, target.LocalizedName, result.Damage));
                        ui.ShowFeedback(target == Player, (result.Critical ? GameText.Get("battle.critical")+"\n" : "") + "−" + result.Damage,
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
            transition.TryGrantVictoryExperience(out int gained);
            ui.RefreshStats(Player, Enemy, Round);
            ui.ShowVictoryResults(gained, Player);
            ui.ShowMessage(GameText.Get("battle.victory_message"));
            ui.ShowMenu(new[] { GameText.Get("menu.return_map") }, new System.Action[] {
                () => { if (transition.ReturnAfterVictory()) ui.SetCommandsEnabled(false); } });
        }
        else
        {
            if(presentation!=null)yield return presentation.Finish(BattleState.Defeat,rules.actionDelay);
            State = BattleState.Defeat;
            if (audioController != null) audioController.Play(BattleSound.Defeat);
            ui.RefreshStats(Player, Enemy, Round);
            ui.ShowMessage(GameText.Get("battle.defeat_message"));
            ui.ShowDefeatResults();
            ui.ShowMenu(new[] { GameText.Get("menu.new_game"), GameText.Get("menu.main") }, new System.Action[] {
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
        ui.ShowMessage(GameText.Get("battle.your_turn"));
        ui.ShowMenu(new[] {GameText.Get("command.attack"), GameText.Get("command.magic"), GameText.Get("command.items"), GameText.Get("command.defend"), GameText.Get("command.flee")}, new Action[] {
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
            names.Add(GameText.Get("battle.spell_entry", entry.LocalizedName, entry.mpCost));
            actions.Add(() => TryPlayerSpell(entry));
            enabled.Add(BattleAbilities.CanCast(Player, entry.target == BattleTarget.Self ? Player : Enemy, entry));
        }
        if (names.Count == 0) ui.ShowMessage(GameText.Get("battle.no_spells"));
        else ui.ShowMessage(GameText.Get("battle.choose_spell"));
        names.Add(GameText.Get("menu.back")); actions.Add(() => ShowMainMenu()); enabled.Add(true);
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
                names.Add(GameText.Get("item.quantity", entry.LocalizedName, Player.Inventory.Count(entry)));
                actions.Add(() => TryPlayerItem(entry));
                bool canUse = BattleAbilities.CanUseItem(Player, entry.target == BattleTarget.Self ? Player : Enemy, entry);
                enabled.Add(canUse); anyAvailable |= canUse;
            }
        }
        ui.ShowMessage(anyAvailable ? GameText.Get("battle.choose_item") : GameText.Get("battle.no_items"));
        names.Add(GameText.Get("menu.back")); actions.Add(() => ShowMainMenu()); enabled.Add(true);
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

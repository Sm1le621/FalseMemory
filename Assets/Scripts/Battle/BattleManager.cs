using System.Collections;
using UnityEngine;

public sealed class BattleManager : MonoBehaviour
{
    public BattleUI ui;
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

    private IEnumerator Start()
    {
        transition = BattleTransition.Instance;
        if (transition == null || !transition.EncounterActive)
        {
            ui.ShowMessage("For battle, run SampleScene and touch the skeleton.");
            ui.ShowMenu(new[] { "Investigate", "Main Menu" }, new System.Action[] {
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene"),
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu") });
            yield break;
        }
        Player = transition.Player; Enemy = transition.Enemy; rules = transition.rules;
        ui.ShowUnits(Player, Enemy);
        ui.ShowMessage("Skeleton blocks the way.");
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
                    ui.ShowMessage("Your turn. Choose an action.");
                    ui.ShowMenu(new[] { "Attack", "Defend" }, new System.Action[] {
                        () => TryPlayerAction(BattleAction.Attack), () => TryPlayerAction(BattleAction.Defend) });
                    gate.Open();
                    while (!pendingAction.HasValue) yield return null;
                }
                else
                {
                    State = BattleState.Resolving;
                    ui.ShowMessage(Enemy.displayName + " is choosing an action...");
                    yield return new WaitForSecondsRealtime(rules.actionDelay);
                    pendingAction = EnemyBattleAI.Choose(rules);
                }

                State = BattleState.Resolving;
                gate.Close();
                ui.SetCommandsEnabled(false);
                var target = actor == Player ? Enemy : Player;
                if (pendingAction == BattleAction.Defend)
                {
                    actor.Defend();
                    int reduction = Mathf.RoundToInt((1f - Mathf.Clamp01(rules.defendMultiplier)) * 100f);
                    ui.ShowMessage(actor.displayName + " Defends. Physical damage reduced by " + reduction + "%.");
                }
                else
                {
                    int damage = BattleCalculator.PhysicalDamage(actor.attack, target.defense, target.IsDefending, rules);
                    target.Health.TakeDamage(damage);
                    ui.ShowMessage(actor.displayName + " attacked " + target.displayName + ": " + damage + " damage.");
                }
                ui.RefreshStats(Player, Enemy, Round);
                yield return new WaitForSecondsRealtime(rules.actionDelay);
                if (!Player.IsAlive || !Enemy.IsAlive) break;
            }
        }
        CurrentActor = null;
        gate.Close();
        if (Player.IsAlive)
        {
            State = BattleState.Victory;
            ui.ShowMessage("Victory!");
            ui.ShowMenu(new[] { "Back to Map" }, new System.Action[] {
                () => { if (transition.ReturnAfterVictory()) ui.SetCommandsEnabled(false); } });
        }
        else
        {
            State = BattleState.Defeat;
            ui.ShowMessage("Game Over");
            ui.ShowMenu(new[] { "Try Again", "Main Menu" }, new System.Action[] {
                () => { ui.SetCommandsEnabled(false); transition.RestartGame(); },
                () => { ui.SetCommandsEnabled(false); transition.ReturnToMainMenu(); } });
        }
    }

    public bool TryPlayerAction(BattleAction action)
    {
        if (State != BattleState.AwaitingPlayer || CurrentActor != Player || !Player.IsAlive ||
            (action != BattleAction.Attack && action != BattleAction.Defend) || !gate.TryConsume()) return false;
        pendingAction = action;
        // Close immediately, not on the next coroutine update.
        State = BattleState.Resolving;
        ui.SetCommandsEnabled(false);
        return true;
    }
}

using UnityEngine;

public static class EnemyBattleAI
{
    // Called exclusively by BattleManager during this enemy's scheduled turn.
    public static BattleAction Choose(BattleRules rules)
        => Choose(rules, UnityBattleRandom.Instance);

    public static BattleAction Choose(BattleRules rules, IBattleRandom random)
    {
        return BattleCalculator.ChanceSucceeds(rules.enemyDefendChance, random.Next01()) ? BattleAction.Defend : BattleAction.Attack;
    }
}

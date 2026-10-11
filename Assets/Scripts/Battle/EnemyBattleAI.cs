using UnityEngine;

public static class EnemyBattleAI
{
    // Called exclusively by BattleManager during this enemy's scheduled turn.
    public static BattleAction Choose(BattleRules rules)
    {
        return Random.value < rules.enemyDefendChance ? BattleAction.Defend : BattleAction.Attack;
    }
}

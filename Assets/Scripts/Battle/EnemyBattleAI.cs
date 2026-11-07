using UnityEngine;

// "Мозок" ворога в бою: вирішує, атакувати чи захищатись.
public static class EnemyBattleAI
{
    // Викликається тільки з BattleManager, коли настає хід ворога.
    // Ця версія використовує звичайний рандом Unity.
    public static BattleAction Choose(BattleRules rules)
    {
        return Choose(rules, UnityBattleRandom.Instance);
    }

    // Ця версія приймає своє джерело випадкових чисел (зручно для тестів)
    public static BattleAction Choose(BattleRules rules, IBattleRandom random)
    {
        bool defend = BattleCalculator.ChanceSucceeds(rules.enemyDefendChance, random.Next01());

        if (defend)
        {
            return BattleAction.Defend;
        }
        return BattleAction.Attack;
    }
}

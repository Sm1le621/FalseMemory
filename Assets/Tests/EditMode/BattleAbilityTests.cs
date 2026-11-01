using NUnit.Framework;
using UnityEngine;

public sealed partial class BattleAbilityTests
{
    private GameObject playerObject, enemyObject;
    private BattleUnit player, enemy;
    private BattleSpell fire;
    private BattleRules rules;
    private BattleConsumable potion;

    [SetUp] public void Setup()
    {
        playerObject = new GameObject("Ability test player"); enemyObject = new GameObject("Ability test enemy");
        player = playerObject.AddComponent<BattleUnit>(); enemy = enemyObject.AddComponent<BattleUnit>();
        player.Health.maxHealth = player.Health.currentHealth = 100;
        enemy.Health.maxHealth = enemy.Health.currentHealth = 60;
        player.maxMP = 30; player.RestoreMP(30); player.magicAttack = 12; enemy.magicDefense = 3;
        rules = ScriptableObject.CreateInstance<BattleRules>();
        fire = ScriptableObject.CreateInstance<BattleSpell>();
        fire.displayName = "Fire"; fire.mpCost = 8; fire.effect = BattleEffect.MagicDamage;
        fire.target = BattleTarget.Opponent; fire.power = 10;
        potion = ScriptableObject.CreateInstance<BattleConsumable>();
        playerObject.AddComponent<BattleInventory>().items.Add(new BattleItemStack {definition = potion, count = 3});
    }

    [TearDown] public void Cleanup()
    {
        Object.DestroyImmediate(playerObject); Object.DestroyImmediate(enemyObject);
        Object.DestroyImmediate(fire); Object.DestroyImmediate(rules);
        Object.DestroyImmediate(potion);
    }

    [Test] public void FireCostsEightMPAndIgnoresPhysicalDefend()
    {
        enemy.Defend();
        Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out int amount), Is.True);
        Assert.That(amount, Is.EqualTo(19)); Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(41));
    }

    [Test] public void InsufficientMPRejectsFireWithoutChangingHealthOrMP()
    {
        player.TrySpendMP(23);
        Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out _), Is.False);
        Assert.That(player.CurrentMP, Is.EqualTo(7)); Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
    }

    [Test] public void DeadCasterOrTargetCannotCast()
    {
        enemy.Health.currentHealth = 0;
        Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out _), Is.False);
        player.Health.currentHealth = 0;
        Assert.That(BattleAbilities.TryCast(player, player, fire, rules, out _), Is.False);
        Assert.That(player.CurrentMP, Is.EqualTo(30));
    }

    [Test] public void SelfTargetedDamageSpellCannotBeCastOnTheOpponent()
    {
        fire.target = BattleTarget.Self;
        Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out _), Is.False);
        Assert.That(BattleAbilities.TryCast(player, player, fire, rules, out _), Is.True);
        Assert.That(player.Health.currentHealth, Is.EqualTo(78));
    }

    [Test] public void ManaNeverGoesNegativeOrAboveMaximum()
    {
        Assert.That(player.TrySpendMP(31), Is.False); Assert.That(player.TrySpendMP(-1), Is.False);
        Assert.That(player.TrySpendMP(30), Is.True); Assert.That(player.CurrentMP, Is.Zero);
        Assert.That(player.RestoreMP(int.MaxValue), Is.EqualTo(30));
        Assert.That(player.CurrentMP, Is.EqualTo(30));
    }
}

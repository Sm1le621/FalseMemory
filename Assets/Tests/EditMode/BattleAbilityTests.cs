using NUnit.Framework;
using UnityEngine;

public sealed partial class BattleAbilityTests
{
    private GameObject playerObject, enemyObject;
    private BattleUnit player, enemy;
    private BattleSpell fire, heal;
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
        heal = ScriptableObject.CreateInstance<BattleSpell>();
        heal.displayName = "Heal"; heal.mpCost = 6; heal.effect = BattleEffect.RestoreHealth;
        heal.target = BattleTarget.Self; heal.power = 25;
        potion = ScriptableObject.CreateInstance<BattleConsumable>();
        playerObject.AddComponent<BattleInventory>().items.Add(new BattleItemStack {definition = potion, count = 3});
    }

    [TearDown] public void Cleanup()
    {
        Object.DestroyImmediate(playerObject); Object.DestroyImmediate(enemyObject);
        Object.DestroyImmediate(fire); Object.DestroyImmediate(heal); Object.DestroyImmediate(rules);
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

    [TestCase(40, 65, 25)] [TestCase(95, 100, 5)] [TestCase(100, 100, 0)]
    public void HealCapsHealthAndReturnsActualAmount(int initial, int expected, int restored)
    {
        player.Health.currentHealth = initial;
        Assert.That(BattleAbilities.TryCast(player, player, heal, rules, out int amount), Is.True);
        Assert.That(amount, Is.EqualTo(restored)); Assert.That(player.Health.currentHealth, Is.EqualTo(expected));
        Assert.That(player.CurrentMP, Is.EqualTo(24));
    }

    [Test] public void DeadCasterOrTargetAndIncorrectTargetsCannotCast()
    {
        Assert.That(BattleAbilities.TryCast(player, enemy, heal, rules, out _), Is.False);
        enemy.Health.currentHealth = 0;
        Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out _), Is.False);
        player.Health.currentHealth = 0;
        Assert.That(BattleAbilities.TryCast(player, player, heal, rules, out _), Is.False);
        Assert.That(player.CurrentMP, Is.EqualTo(30));
    }

    [Test] public void ManaNeverGoesNegativeOrAboveMaximum()
    {
        Assert.That(player.TrySpendMP(31), Is.False); Assert.That(player.TrySpendMP(-1), Is.False);
        Assert.That(player.TrySpendMP(30), Is.True); Assert.That(player.CurrentMP, Is.Zero);
        Assert.That(player.RestoreMP(int.MaxValue), Is.EqualTo(30));
        Assert.That(player.CurrentMP, Is.EqualTo(30));
    }
}

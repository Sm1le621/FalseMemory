using NUnit.Framework;
using UnityEngine;

// Тести заклинань і мани.
// partial - продовження цього класу є у файлі BattleItemTests.cs
public sealed partial class BattleAbilityTests
{
    private GameObject playerObject;
    private GameObject enemyObject;
    private BattleUnit player;
    private BattleUnit enemy;
    private BattleSpell fire;
    private BattleRules rules;
    private BattleConsumable potion;

    // Викликається ПЕРЕД кожним тестом: створюємо гравця, ворога, заклинання і зілля
    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("Ability test player");
        enemyObject = new GameObject("Ability test enemy");
        player = playerObject.AddComponent<BattleUnit>();
        enemy = enemyObject.AddComponent<BattleUnit>();

        player.Health.maxHealth = 100;
        player.Health.currentHealth = 100;
        enemy.Health.maxHealth = 60;
        enemy.Health.currentHealth = 60;

        player.maxMP = 30;
        player.RestoreMP(30);
        player.magicAttack = 12;
        enemy.magicDefense = 3;

        rules = ScriptableObject.CreateInstance<BattleRules>();

        fire = ScriptableObject.CreateInstance<BattleSpell>();
        fire.displayName = "Fire";
        fire.mpCost = 8;
        fire.effect = BattleEffect.MagicDamage;
        fire.target = BattleTarget.Opponent;
        fire.power = 10;

        // 3 зілля в інвентарі гравця
        potion = ScriptableObject.CreateInstance<BattleConsumable>();
        BattleInventory inventory = playerObject.AddComponent<BattleInventory>();
        BattleItemStack stack = new BattleItemStack();
        stack.definition = potion;
        stack.count = 3;
        inventory.items.Add(stack);
    }

    // Викликається ПІСЛЯ кожного тесту: прибираємо все створене
    [TearDown]
    public void Cleanup()
    {
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(enemyObject);
        Object.DestroyImmediate(fire);
        Object.DestroyImmediate(rules);
        Object.DestroyImmediate(potion);
    }

    // Fire коштує 8 MP і ігнорує фізичний захист
    [Test]
    public void FireCostsEightMPAndIgnoresPhysicalDefend()
    {
        enemy.Defend();

        int amount;
        Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out amount), Is.True);
        // 10 (сила) + 12 (атака) - 3 (захист) = 19
        Assert.That(amount, Is.EqualTo(19));
        Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(41));
    }

    // Якщо мани мало - заклинання не кастується, і нічого не змінюється
    [Test]
    public void InsufficientMPRejectsFireWithoutChangingHealthOrMP()
    {
        player.TrySpendMP(23);

        int amount;
        Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out amount), Is.False);
        Assert.That(player.CurrentMP, Is.EqualTo(7));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(60));
    }

    // Мертвий не може кастувати, і на мертвого кастувати не можна
    [Test]
    public void DeadCasterOrTargetCannotCast()
    {
        int amount;

        enemy.Health.currentHealth = 0;
        Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out amount), Is.False);

        player.Health.currentHealth = 0;
        Assert.That(BattleAbilities.TryCast(player, player, fire, rules, out amount), Is.False);

        Assert.That(player.CurrentMP, Is.EqualTo(30));
    }

    // Заклинання "на себе" не можна кинути у ворога
    [Test]
    public void SelfTargetedDamageSpellCannotBeCastOnTheOpponent()
    {
        fire.target = BattleTarget.Self;

        int amount;
        Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out amount), Is.False);
        Assert.That(BattleAbilities.TryCast(player, player, fire, rules, out amount), Is.True);
        Assert.That(player.Health.currentHealth, Is.EqualTo(78));
    }

    // Мана не стає від'ємною і не більше максимуму
    [Test]
    public void ManaNeverGoesNegativeOrAboveMaximum()
    {
        Assert.That(player.TrySpendMP(31), Is.False);
        Assert.That(player.TrySpendMP(-1), Is.False);
        Assert.That(player.TrySpendMP(30), Is.True);
        Assert.That(player.CurrentMP, Is.Zero);
        Assert.That(player.RestoreMP(int.MaxValue), Is.EqualTo(30));
        Assert.That(player.CurrentMP, Is.EqualTo(30));
    }
}

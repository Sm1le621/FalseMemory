using NUnit.Framework;

public sealed partial class BattleAbilityTests
{
    [TestCase(40, 70, 30)] [TestCase(95, 100, 5)] [TestCase(100, 100, 0)]
    public void PotionHealsAndConsumesExactlyOneItem(int initial, int expected, int restored)
    {
        player.Health.currentHealth = initial;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out int amount), Is.True);
        Assert.That(amount, Is.EqualTo(restored)); Assert.That(player.Health.currentHealth, Is.EqualTo(expected));
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(2)); Assert.That(player.CurrentMP, Is.EqualTo(30));
    }

    [Test] public void EmptyPotionStackCannotHealOrBecomeNegative()
    {
        player.Inventory.items[0].count = 0; player.Health.currentHealth = 50;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out _), Is.False);
        Assert.That(player.Health.currentHealth, Is.EqualTo(50)); Assert.That(player.Inventory.Count(potion), Is.Zero);
    }

    [Test] public void PotionCannotReviveDeadUserOrTarget()
    {
        player.Health.currentHealth = 0;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out _), Is.False);
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(3));
    }

    [Test] public void DuplicateStacksConsumeOnlyOneAndNegativeStacksAreUnavailable()
    {
        player.Inventory.items[0].count = -1;
        player.Inventory.items.Add(new BattleItemStack {definition = potion, count = 2});
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(2));
        Assert.That(player.Inventory.TryConsume(potion), Is.True);
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(1));
    }
}

using NUnit.Framework;

public sealed partial class BattleAbilityTests
{
    [TestCase(0, 20, 20, 20)] [TestCase(15, 20, 35, 20)] [TestCase(int.MaxValue - 3, 20, int.MaxValue, 3)]
    public void ExperienceAccumulatesWithoutOverflowOrImplicitLevelUp(int initial, int reward, int expected, int added)
    {
        player.AddExperience(initial);
        Assert.That(player.AddExperience(reward), Is.EqualTo(added));
        Assert.That(player.CurrentExperience, Is.EqualTo(expected)); Assert.That(player.level, Is.EqualTo(1));
    }

    [Test] public void NegativeRewardOrDeadPlayerCannotGainExperience()
    {
        Assert.That(player.AddExperience(-20), Is.Zero);
        player.Health.currentHealth = 0;
        Assert.That(player.AddExperience(20), Is.Zero); Assert.That(player.CurrentExperience, Is.Zero);
    }
}

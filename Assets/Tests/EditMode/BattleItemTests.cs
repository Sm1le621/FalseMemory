using NUnit.Framework;

public sealed partial class BattleAbilityTests
{
    [Test] public void ManaPotionRestoresActualManaWithoutHealingAndConsumesOne()
    {
        potion.healing=0;potion.manaRestoration=20;player.TrySpendMP(25);
        Assert.That(BattleAbilities.TryUseItem(player,player,potion,out int amount),Is.True);
        Assert.That(amount,Is.EqualTo(20));Assert.That(player.CurrentMP,Is.EqualTo(25));
        Assert.That(player.Health.currentHealth,Is.EqualTo(100));Assert.That(player.Inventory.Count(potion),Is.EqualTo(2));
        Assert.That(BattleAbilities.TryUseItem(player,player,potion,out amount),Is.True);
        Assert.That(amount,Is.EqualTo(5));Assert.That(player.CurrentMP,Is.EqualTo(30));Assert.That(player.Inventory.Count(potion),Is.EqualTo(1));
    }
    [Test] public void ItemLocationsRejectWithoutResourceChanges()
    {
        potion.useLocations=ItemUseLocation.Exploration;player.Health.currentHealth=50;
        Assert.That(BattleAbilities.TryUseItem(player,player,potion,out _),Is.False);
        Assert.That(player.Inventory.Count(potion),Is.EqualTo(3));
        Assert.That(BattleAbilities.TryUseItem(player,player,potion,out _,ItemUseLocation.Exploration),Is.True);
        Assert.That(player.Health.currentHealth,Is.EqualTo(80));
        Assert.That(player.Inventory.Count(potion),Is.EqualTo(2));
        potion.useLocations=ItemUseLocation.Battle;
        Assert.That(BattleAbilities.TryUseItem(player,player,potion,out _,ItemUseLocation.Exploration),Is.False);
        Assert.That(player.Inventory.Count(potion),Is.EqualTo(2));
    }
    [Test] public void InvalidHybridAndNegativeEffectsNeverConsume()
    {
        potion.manaRestoration=20;
        Assert.That(BattleAbilities.TryUseItem(player,player,potion,out _),Is.False);
        potion.healing=-1;Assert.That(BattleAbilities.TryUseItem(player,player,potion,out _),Is.False);
        potion.healing=potion.manaRestoration=0;Assert.That(BattleAbilities.TryUseItem(player,player,potion,out _),Is.False);
        Assert.That(player.Inventory.Count(potion),Is.EqualTo(3));Assert.That(player.CurrentMP,Is.EqualTo(30));
    }
    [Test] public void EquipmentAttackBonusRaisesPhysicalAttackButNotSpellPower()
    {
        var sword=UnityEngine.ScriptableObject.CreateInstance<ItemData>();sword.id="test.sword";sword.category=ItemCategory.Weapon;sword.equipmentSlot=EquipmentSlot.Weapon;sword.bonuses=new ItemStatBonus{attack=5};
        try
        {
            player.Inventory.TryAdd(sword,1);var equipment=player.gameObject.AddComponent<EquipmentController>();
            int swordIndex=-1;for(int i=0;i<InventoryState.Capacity;i++)if(player.Inventory.State.GetSlot(i).Item==sword){swordIndex=i;break;}
            Assert.That(swordIndex,Is.GreaterThanOrEqualTo(0));
            Assert.That(equipment.State.TryEquip(player.Inventory.State,swordIndex,EquipmentSlot.Weapon),Is.True);
            Assert.That(player.EffectiveAttack,Is.EqualTo(20));
            Assert.That(BattleAbilities.TryCast(player,enemy,fire,rules,out int amount),Is.True);
            Assert.That(amount,Is.EqualTo(19));Assert.That(player.CurrentMP,Is.EqualTo(22));Assert.That(enemy.Health.currentHealth,Is.EqualTo(41));
            Assert.That(player.attack,Is.EqualTo(15));
        }
        finally {UnityEngine.Object.DestroyImmediate(sword);}
    }
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

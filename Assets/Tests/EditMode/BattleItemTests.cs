using NUnit.Framework;

// Тести предметів (зілля, екіпіровка).
// Це продовження класу BattleAbilityTests: гравець, ворог, Fire і зілля
// створюються в Setup() у файлі BattleAbilityTests.cs
public sealed partial class BattleAbilityTests
{
    // Зілля мани відновлює тільки ману (не більше максимуму) і витрачає 1 штуку
    [Test]
    public void ManaPotionRestoresActualManaWithoutHealingAndConsumesOne()
    {
        potion.healing = 0;
        potion.manaRestoration = 20;
        player.TrySpendMP(25);

        int amount;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount), Is.True);
        Assert.That(amount, Is.EqualTo(20));
        Assert.That(player.CurrentMP, Is.EqualTo(25));
        Assert.That(player.Health.currentHealth, Is.EqualTo(100));
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(2));

        // Друге зілля відновить тільки 5 (до максимуму 30)
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount), Is.True);
        Assert.That(amount, Is.EqualTo(5));
        Assert.That(player.CurrentMP, Is.EqualTo(30));
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(1));
    }

    // Предмет, дозволений тільки в певному місці, не працює в іншому
    [Test]
    public void ItemLocationsRejectWithoutResourceChanges()
    {
        int amount;

        potion.useLocations = ItemUseLocation.Exploration;
        player.Health.currentHealth = 50;

        // В бою - не можна
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount), Is.False);
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(3));

        // У світі - можна
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount, ItemUseLocation.Exploration), Is.True);
        Assert.That(player.Health.currentHealth, Is.EqualTo(80));
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(2));

        // Тепер навпаки: тільки в бою
        potion.useLocations = ItemUseLocation.Battle;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount, ItemUseLocation.Exploration), Is.False);
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(2));
    }

    // "Гібридні" та від'ємні предмети ніколи не витрачаються
    [Test]
    public void InvalidHybridAndNegativeEffectsNeverConsume()
    {
        int amount;

        // І HP, і MP одночасно - не можна
        potion.manaRestoration = 20;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount), Is.False);

        // Від'ємне лікування
        potion.healing = -1;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount), Is.False);

        // Предмет, що нічого не робить
        potion.healing = 0;
        potion.manaRestoration = 0;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount), Is.False);

        Assert.That(player.Inventory.Count(potion), Is.EqualTo(3));
        Assert.That(player.CurrentMP, Is.EqualTo(30));
    }

    // Меч додає до фізичної атаки, але не до магії
    [Test]
    public void EquipmentAttackBonusRaisesPhysicalAttackButNotSpellPower()
    {
        ItemData sword = UnityEngine.ScriptableObject.CreateInstance<ItemData>();
        sword.id = "test.sword";
        sword.category = ItemCategory.Weapon;
        sword.equipmentSlot = EquipmentSlot.Weapon;
        ItemStatBonus swordBonus = new ItemStatBonus();
        swordBonus.attack = 5;
        sword.bonuses = swordBonus;

        try
        {
            player.Inventory.TryAdd(sword, 1);
            EquipmentController equipment = player.gameObject.AddComponent<EquipmentController>();

            // Шукаємо, в який слот потрапив меч
            int swordIndex = -1;
            for (int i = 0; i < InventoryState.Capacity; i++)
            {
                if (player.Inventory.State.GetSlot(i).Item == sword)
                {
                    swordIndex = i;
                    break;
                }
            }
            Assert.That(swordIndex, Is.GreaterThanOrEqualTo(0));

            Assert.That(equipment.State.TryEquip(player.Inventory.State, swordIndex, EquipmentSlot.Weapon), Is.True);
            Assert.That(player.EffectiveAttack, Is.EqualTo(20));

            // Шкода Fire та сама, що й без меча
            int amount;
            Assert.That(BattleAbilities.TryCast(player, enemy, fire, rules, out amount), Is.True);
            Assert.That(amount, Is.EqualTo(19));
            Assert.That(player.CurrentMP, Is.EqualTo(22));
            Assert.That(enemy.Health.currentHealth, Is.EqualTo(41));

            // Базова атака не змінилась
            Assert.That(player.attack, Is.EqualTo(15));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(sword);
        }
    }

    // Початкове HP, очікуване HP після зілля, скільки відновилось
    [TestCase(40, 70, 30)]
    [TestCase(95, 100, 5)]
    [TestCase(100, 100, 0)]
    public void PotionHealsAndConsumesExactlyOneItem(int initial, int expected, int restored)
    {
        player.Health.currentHealth = initial;

        int amount;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount), Is.True);
        Assert.That(amount, Is.EqualTo(restored));
        Assert.That(player.Health.currentHealth, Is.EqualTo(expected));
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(2));
        Assert.That(player.CurrentMP, Is.EqualTo(30));
    }

    // Якщо зілля немає - лікування немає
    [Test]
    public void EmptyPotionStackCannotHealOrBecomeNegative()
    {
        player.Inventory.items[0].count = 0;
        player.Health.currentHealth = 50;

        int amount;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount), Is.False);
        Assert.That(player.Health.currentHealth, Is.EqualTo(50));
        Assert.That(player.Inventory.Count(potion), Is.Zero);
    }

    // Зілля не воскрешає
    [Test]
    public void PotionCannotReviveDeadUserOrTarget()
    {
        player.Health.currentHealth = 0;

        int amount;
        Assert.That(BattleAbilities.TryUseItem(player, player, potion, out amount), Is.False);
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(3));
    }

    // Кілька записів одного предмета: витрачається тільки 1, від'ємні записи ігноруються
    [Test]
    public void DuplicateStacksConsumeOnlyOneAndNegativeStacksAreUnavailable()
    {
        player.Inventory.items[0].count = -1;

        BattleItemStack extra = new BattleItemStack();
        extra.definition = potion;
        extra.count = 2;
        player.Inventory.items.Add(extra);

        Assert.That(player.Inventory.Count(potion), Is.EqualTo(2));
        Assert.That(player.Inventory.TryConsume(potion), Is.True);
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(1));
    }
}

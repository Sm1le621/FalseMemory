using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator PrimaryGearGrowthAndRestoreKeepOneSourceAndNeverHealOnRecalculation()
    {
        var player=Player();var attributes=player.Attributes;var item=ScriptableObject.CreateInstance<ItemData>();
        item.id="test_primary_gear";item.category=ItemCategory.Weapon;item.bonuses=new ItemStatBonus{attack=2,attributes=new CharacterAttributeValues{strength=3,dexterity=7,vitality=2,intelligence=2,wisdom=1,magicPower=4,agility=6}};
        try
        {
            Assert.That(player.UsesPrimaryAttributes,Is.True);player.missChance=.05f;player.Health.TakeDamage(30);player.TrySpendMP(8);
            Assert.That(player.Inventory.TryAdd(item,1),Is.True);Assert.That(player.Equipment.TryEquip(1),Is.True);
            Assert.That(player.EffectiveAttack,Is.EqualTo(27));Assert.That(player.Health.maxHealth,Is.EqualTo(110));Assert.That(player.MaxMP,Is.EqualTo(33));
            Assert.That(player.Health.currentHealth,Is.EqualTo(70));Assert.That(player.CurrentMP,Is.EqualTo(22));
            Assert.That(player.GetComponent<PlayerController>().MovementSpeed,Is.EqualTo(4.18f).Within(.0001f));
            Assert.That(player.EffectiveMissChance,Is.EqualTo(.015f).Within(.0001f));Assert.That(player.Evasion,Is.EqualTo(.03f).Within(.0001f));
            player.RefreshDerivedStats();player.RefreshDerivedStats();Assert.That(player.EffectiveAttack,Is.EqualTo(27));Assert.That(attributes.BaseValues.strength,Is.EqualTo(10));
            player.AddExperience(100);Assert.That(player.level,Is.EqualTo(2));Assert.That(player.EffectiveAttack,Is.EqualTo(29));
            Assert.That(player.Health.maxHealth,Is.EqualTo(115));Assert.That(player.MaxMP,Is.EqualTo(35));Assert.That(player.Health.currentHealth,Is.EqualTo(70));Assert.That(player.CurrentMP,Is.EqualTo(22));
            Assert.That(player.Equipment.TryUnequip(EquipmentSlot.Weapon),Is.True);Assert.That(player.EffectiveAttack,Is.EqualTo(17));
            Assert.That(player.Health.maxHealth,Is.EqualTo(105));Assert.That(player.MaxMP,Is.EqualTo(32));Assert.That(player.CurrentMP,Is.EqualTo(22));
            string snapshot=JsonUtility.ToJson(attributes.BaseValues);attributes.SetBase(CharacterAttribute.Strength,30);attributes.RestoreBaseValues(JsonUtility.FromJson<CharacterAttributeValues>(snapshot));
            Assert.That(player.EffectiveAttack,Is.EqualTo(17));Assert.That(player.PrimaryValues.strength,Is.EqualTo(11));
            player.Health.TakeDamage(70);attributes.SetBase(CharacterAttribute.Vitality,30);Assert.That(player.Health.currentHealth,Is.Zero);
        }
        finally {Object.Destroy(item);}
        yield return null;
    }
    [UnityTest] public IEnumerator PrimaryRowsShowTenValuesGearSourcesAndTooltipWithoutDuplicatingStats()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();
        player.Attributes.SetClassBonuses(new CharacterAttributeValues{strength=2});
        player.Attributes.SetTemporaryBonuses(new CharacterAttributeValues{strength=-1});
        window.Open();yield return null;
        Assert.That(window.attributeRows.Length,Is.EqualTo(10));
        Assert.That(window.attributeRows[0].label.text,Does.Contain("STR Strength: 11"));
        Assert.That(window.attributeRows[9].label.text,Does.Contain("MCR Magic Control: 10"));
        window.tooltip.Show(window.attributeRows[0],Vector2.zero);
        Assert.That(window.tooltip.IsVisible,Is.True);Assert.That(window.tooltip.content.text,Does.Contain("Base: 10").And.Contain("Class: 2").And.Contain("Temporary: -1"));
        Assert.That(player.EffectiveAttack,Is.EqualTo(16));window.Close();Assert.That(window.tooltip.IsVisible,Is.False);
    }
    [UnityTest] public IEnumerator MagicControlCostAndMagicPowerApplyToExistingFireAndHeal()
    {
        var player=Player();var enemy=Enemy();player.Attributes.SetBase(CharacterAttribute.MagicControl,20);player.Attributes.SetBase(CharacterAttribute.MagicPower,14);
        player.Health.TakeDamage(30);Assert.That(player.TrySpendMP(23),Is.True);yield return BeginBattle(player,enemy);var manager=Manager();
        Assert.That(player.SpellCost(player.spells[0]),Is.EqualTo(7));Assert.That(manager.OpenMagicMenu(),Is.True);
        Assert.That(manager.ui.commandButtons[0].GetComponentInChildren<TextMeshProUGUI>().text,Does.Contain("7 MP"));
        Assert.That(manager.TryPlayerSpell(player.spells[0]),Is.True);yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP,Is.Zero);Assert.That(enemy.Health.currentHealth,Is.EqualTo(37));Assert.That(player.Health.currentHealth,Is.EqualTo(66));
        player.RestoreMP(6);Assert.That(player.SpellCost(player.spells[1]),Is.EqualTo(5));
        Assert.That(manager.TryPlayerSpell(player.spells[1]),Is.True);yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(player.Health.currentHealth,Is.EqualTo(91));Assert.That(player.CurrentMP,Is.EqualTo(1));Assert.That(enemy.Health.currentHealth,Is.EqualTo(37));
    }
}

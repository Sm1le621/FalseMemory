using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator AllocatedPointsScalingAndRequirementsSurviveRealBattleAndNeverRestoreResources()
    {
        var player=Player();var attributes=player.Attributes;var enemy=Enemy();
        Assert.That(attributes.rules.classlessProgression,Is.True);Assert.That(player.MaxMP,Is.EqualTo(40));
        Assert.That(player.Health.maxHealth,Is.EqualTo(100));player.Health.TakeDamage(20);player.TrySpendMP(8);
        player.AddExperience(100);Assert.That(attributes.UnspentPoints,Is.EqualTo(1));Assert.That(player.PrimaryValues.vitality,Is.EqualTo(10));
        Assert.That(attributes.TryIncreaseBase(CharacterAttribute.Vitality),Is.True);Assert.That(attributes.TryIncreaseBase(CharacterAttribute.Vitality),Is.False);
        Assert.That(player.Health.maxHealth,Is.EqualTo(108));Assert.That(player.Health.currentHealth,Is.EqualTo(80));Assert.That(player.CurrentMP,Is.EqualTo(32));
        var weapon=GameObject.Find("Pickup_Scaling_Greatsword").GetComponent<WorldItemPickup>().item;
        player.Inventory.TryAdd(weapon,1);Assert.That(player.Equipment.TryEquip(1),Is.False);Assert.That(player.Inventory.Count(weapon),Is.EqualTo(1));
        attributes.SetBase(CharacterAttribute.Strength,25);Assert.That(player.Equipment.TryEquip(1),Is.True);
        int expected=weapon.weapon.Damage(player.PrimaryValues,attributes.rules);Assert.That(player.EffectiveAttack,Is.EqualTo(expected),"No second global STR/DEX damage contribution.");
        var snapshot=attributes.BaseValues;int points=attributes.UnspentPoints;
        yield return BeginBattle(player,enemy);Assert.That(Manager().TryPlayerAction(BattleAction.Attack),Is.True);
        yield return WaitUntil(()=>Manager().State==BattleState.Victory || Manager().State==BattleState.AwaitingPlayer);
        Assert.That(attributes.UnspentPoints,Is.EqualTo(points));Assert.That(attributes.BaseValues.strength,Is.EqualTo(snapshot.strength));
        if(Manager().State!=BattleState.Victory){enemy.Health.TakeDamage(999);yield return WaitUntil(()=>Manager().State==BattleState.Victory);}
        Assert.That(BattleTransition.Instance.ReturnAfterVictory(),Is.True);yield return WaitUntil(()=>!BattleTransition.Instance.EncounterActive);
        Assert.That(player.Equipment.State.GetSlot(EquipmentSlot.Weapon),Is.SameAs(weapon));Assert.That(player.CurrentMP,Is.EqualTo(32));
        var window=player.GetComponent<InventoryWindow>();Assert.That(window.Open(),Is.True);yield return null;
        window.tooltip.Show(window.equipmentButtons[0].GetComponent<EquipmentSlotView>(),Vector2.zero);
        Assert.That(window.tooltip.content.text,Does.Contain("STR: S").And.Contain("Requires:").And.Contain("Physical Damage:"));
        AssertEnglishActiveUi();
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator PhysicalAndFireDealHolyOnceOnlyToUndeadAndHealingNeverTriggersIt()
    {
        var player=Player();var enemy=Enemy();player.Attributes.SetBase(CharacterAttribute.Wisdom,30);
        Assert.That(enemy.creatureCategories,Is.EqualTo(CreatureCategory.Undead));
        yield return BeginBattle(player,enemy);var manager=Manager();
        manager.TryPlayerAction(BattleAction.Attack);yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(player.LastDamageDealt.BaseDamage,Is.EqualTo(12));Assert.That(player.LastDamageDealt.HolyDamage,Is.EqualTo(1));Assert.That(enemy.Health.currentHealth,Is.EqualTo(47));
        manager.TryPlayerSpell(Fire(player));yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(player.LastDamageDealt.BaseDamage,Is.EqualTo(20));Assert.That(player.LastDamageDealt.HolyDamage,Is.EqualTo(2));Assert.That(enemy.Health.currentHealth,Is.EqualTo(25));
        manager.TryPlayerSpell(Heal(player));yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth,Is.EqualTo(25),"Heal must never create a holy damage event.");
        enemy.creatureCategories=CreatureCategory.Human;manager.TryPlayerAction(BattleAction.Attack);yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(player.LastDamageDealt.HolyDamage,Is.Zero);Assert.That(enemy.Health.currentHealth,Is.EqualTo(13));
    }
    [UnityTest] public IEnumerator ArmorRequirementsDefenseAndEvasionUpdateWithoutAccumulation()
    {
        var player=Player();var armor=GameObject.Find("Pickup_Scaling_AssassinArmor").GetComponent<WorldItemPickup>().item;
        player.Inventory.TryAdd(armor,1);Assert.That(player.Equipment.TryEquip(1),Is.False);
        player.Attributes.SetBase(CharacterAttribute.Agility,30);player.Attributes.SetBase(CharacterAttribute.Dexterity,30);
        float movement=player.GetComponent<PlayerController>().MovementSpeed;int basis=player.EffectiveDefense;
        Assert.That(player.Equipment.TryEquip(1),Is.True);int equipped=player.EffectiveDefense;
        Assert.That(equipped,Is.GreaterThan(basis));Assert.That(player.Evasion,Is.GreaterThan(.1f));
        player.RefreshDerivedStats();Assert.That(player.EffectiveDefense,Is.EqualTo(equipped));Assert.That(player.GetComponent<PlayerController>().MovementSpeed,Is.EqualTo(movement));
        Assert.That(player.Equipment.TryUnequip(EquipmentSlot.Chest),Is.True);Assert.That(player.EffectiveDefense,Is.EqualTo(basis));
        yield return null;
    }
}

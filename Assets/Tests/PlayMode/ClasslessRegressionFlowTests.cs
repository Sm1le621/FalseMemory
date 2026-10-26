using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator MagicStaffBasicAttackUsesMagicDefenseAndWisdomHolyAtOneContact()
    {
        var player=Player();var enemy=Enemy();var item=GameObject.Find("Pickup_Scaling_MagicStaff").GetComponent<WorldItemPickup>().item;
        player.Attributes.SetBase(CharacterAttribute.MagicPower,20);player.Attributes.SetBase(CharacterAttribute.Intelligence,20);player.Attributes.SetBase(CharacterAttribute.Wisdom,30);
        player.Inventory.TryAdd(item,1);Assert.That(player.Equipment.TryEquip(1),Is.True);Assert.That(player.MagicalWeaponAttack,Is.True);
        int power=player.BasicAttackPower;Assert.That(power,Is.EqualTo(30));
        yield return BeginBattle(player,enemy);var manager=Manager();Assert.That(manager.TryPlayerAction(BattleAction.Attack),Is.True);
        yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(player.LastDamageDealt.BaseDamage,Is.EqualTo(27));Assert.That(player.LastDamageDealt.HolyDamage,Is.EqualTo(3));Assert.That(enemy.Health.currentHealth,Is.EqualTo(30));
    }
    [UnityTest] public IEnumerator SacredWeaponScalingResistanceAndCategoriesDoNotDuplicateHolyOrAlterWalking()
    {
        var player=Player();var enemy=Enemy();var item=GameObject.Find("Pickup_Scaling_SacredBlade").GetComponent<WorldItemPickup>().item;
        player.Attributes.SetBase(CharacterAttribute.Strength,20);player.Attributes.SetBase(CharacterAttribute.Wisdom,30);
        player.Inventory.TryAdd(item,1);Assert.That(player.Equipment.TryEquip(1),Is.True);
        int basis=BattleCalculator.PhysicalDamage(player.BasicAttackPower,enemy.EffectiveDefense,false,rules);Assert.That(basis,Is.EqualTo(42));
        enemy.Health.maxHealth=enemy.Health.currentHealth=999;enemy.creatureCategories=CreatureCategory.Undead;enemy.holyResistance=.5f;
        var result=player.DealDamage(enemy,basis,player.IntrinsicWeaponHoly);Assert.That(result.HolyDamage,Is.EqualTo(7));Assert.That(enemy.Health.currentHealth,Is.EqualTo(950));
        enemy.creatureCategories=CreatureCategory.Beast;result=player.DealDamage(enemy,basis,player.IntrinsicWeaponHoly);Assert.That(result.HolyDamage,Is.Zero);Assert.That(enemy.Health.currentHealth,Is.EqualTo(908));
        player.Attributes.SetBase(CharacterAttribute.Agility,99);Assert.That(player.GetComponent<PlayerController>().MovementSpeed,Is.EqualTo(4));yield return null;
        var window=player.GetComponent<CharacterWindow>();window.Open();window.SelectTab(1);window.combatWeapon.ForceMeshUpdate();
        Assert.That(window.combatWeapon.text,Does.Contain("Additional Holy Damage: 5 + 4 = 9"));Assert.That(window.combatWeapon.isTextOverflowing,Is.False);
        window.SelectTab(0);window.ShowAttribute(CharacterAttribute.Wisdom,Vector2.zero,true);
        player.Attributes.SetTemporaryBonuses(new CharacterAttributeValues{wisdom=1});Assert.That(window.tooltipText.text,Does.Contain("Effective: 31"));window.Close();
    }
}

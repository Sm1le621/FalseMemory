using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// Тести вигляду інвентаря і смужки мани у світі (частина класу BattleFlowTests)
public sealed partial class BattleFlowTests
{
    // Іконка і рамка одягненого предмета працюють після зняття
    [UnityTest]
    public IEnumerator EquippedIconAndBorderSurviveEmptying()
    {
        BattleUnit player = Player();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        InventoryActions equipment = player.GetComponent<InventoryActions>();
        ItemData sword = Sword(player);

        window.Open();
        equipment.TryEquip(0);
        yield return null;

        EquipmentSlotView view = window.equipmentButtons[(int)EquipmentSlot.Weapon].GetComponent<EquipmentSlotView>();
        Assert.That(view.icon.sprite, Is.SameAs(sword.icon));
        Assert.That(view.border, Is.Not.Null);

        // Зняли меч - бонус зник
        equipment.TryUnequip(EquipmentSlot.Weapon);
        Assert.That(player.EffectiveAttack, Is.EqualTo(15));

        window.Close();
    }

    // Лічильники сумки і ресурсів відповідають моделі
    [UnityTest]
    public IEnumerator LiveBagAndResourceCountsMatchModel()
    {
        BattleUnit player = Player();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        window.Open();
        yield return null;

        Assert.That(window.bagLabel.text, Does.Contain("3 / 30"));

        player.Health.TakeDamage(10);
        player.TrySpendMP(5);
        window.Refresh();
        Assert.That(window.resourcesLabel.text, Does.Contain("90 / 100").And.Contain("25 / 30"));
        Assert.That(window.equipmentButtons.Length, Is.EqualTo(2));

        window.Close();
    }

    // Смужка мани у світі показує спільну ману і сама її не змінює
    [UnityTest]
    public IEnumerator WorldManaBarFollowsSharedManaWithoutChangingIt()
    {
        BattleUnit player = Player();
        ManaHUD hud = Object.FindAnyObjectByType<ManaHUD>();
        Assert.That(hud, Is.Not.Null);
        Assert.That(hud.unit, Is.SameAs(player));
        Assert.That(hud.mpSlider, Is.Not.Null);
        Assert.That(hud.mpText, Is.Not.Null);
        yield return null;

        Assert.That(hud.mpSlider.maxValue, Is.EqualTo(30));
        Assert.That(hud.mpSlider.value, Is.EqualTo(30));
        Assert.That(hud.mpText.text, Does.Contain("30 / 30"));

        player.TrySpendMP(12);
        yield return null;
        Assert.That(hud.mpSlider.value, Is.EqualTo(18));
        Assert.That(hud.mpText.text, Does.Contain("18 / 30"));
        Assert.That(player.CurrentMP, Is.EqualTo(18));

        player.RestoreMP(99);
        yield return null;
        Assert.That(hud.mpSlider.value, Is.EqualTo(30));
        Assert.That(hud.mpText.text, Does.Contain("30 / 30"));
    }
}

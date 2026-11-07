using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

// Тести підказок інвентаря і смужки мани у світі (частина класу BattleFlowTests)
public sealed partial class BattleFlowTests
{
    // Наведення мишки показує підказку, а порожня клітинка, перетягування і закриття її ховають
    [UnityTest]
    public IEnumerator RealMouseHoverShowsItemAndHidesOnEmptyDragAndClose()
    {
        InventoryWindow window = Player().GetComponent<InventoryWindow>();
        Mouse mouse = InputSystem.AddDevice<Mouse>();
        try
        {
            window.Open();
            yield return null;
            Canvas.ForceUpdateCanvases();

            // Навели на зілля (слот 2)
            Vector2 position = SlotScreenCenter(window.slots[2]);
            MouseState overPotion = new MouseState();
            overPotion.position = position;
            InputSystem.QueueStateEvent(mouse, overPotion);
            yield return null;
            yield return null;
            Assert.That(window.tooltip.IsVisible, Is.True);
            Assert.That(window.tooltip.content.text, Does.Contain("Potion").And.Contain("Consumable").And.Contain("30 HP"));

            // Навели на порожню клітинку (слот 9)
            position = SlotScreenCenter(window.slots[9]);
            MouseState overEmpty = new MouseState();
            overEmpty.position = position;
            InputSystem.QueueStateEvent(mouse, overEmpty);
            yield return null;
            yield return null;
            Assert.That(window.tooltip.IsVisible, Is.False);

            // Знову на зілля і почали тягнути
            position = SlotScreenCenter(window.slots[2]);
            MouseState backOverPotion = new MouseState();
            backOverPotion.position = position;
            InputSystem.QueueStateEvent(mouse, backOverPotion);
            yield return null;
            yield return null;
            window.BeginDrag(2, position);
            Assert.That(window.tooltip.IsVisible, Is.False);
            window.EndDrag();

            // Закриття вікна ховає підказку
            window.tooltip.Show(window.slots[2], position);
            window.Close();
            Assert.That(window.tooltip.IsVisible, Is.False);
            Assert.That(Player().Inventory.Count(Potion(Player())), Is.EqualTo(3));
        }
        finally
        {
            InputSystem.RemoveDevice(mouse);
            window.Close();
        }
    }

    // Підказка одягненого предмета і рамка працюють після заміни та зняття
    [UnityTest]
    public IEnumerator EquippedHoverAndBorderSurviveReplacementAndEmptying()
    {
        BattleUnit player = Player();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        EquipmentController equipment = player.GetComponent<EquipmentController>();
        ItemData sword = Sword(player);

        window.Open();
        equipment.TryEquip(0);
        yield return null;

        EquipmentSlotView view = window.equipmentButtons[(int)EquipmentSlot.Weapon].GetComponent<EquipmentSlotView>();
        window.tooltip.Show(view, Vector2.zero);
        Assert.That(window.tooltip.IsVisible, Is.True);
        Assert.That(window.tooltip.content.text, Does.Contain("Iron Sword").And.Contain("ATK +5"));
        Assert.That(view.icon.sprite, Is.SameAs(sword.icon));
        Assert.That(view.border, Is.Not.Null);

        // Зняли меч - підказка зникла
        equipment.TryUnequip(EquipmentSlot.Weapon);
        Assert.That(window.tooltip.IsVisible, Is.False);
        Assert.That(player.EffectiveAttack, Is.EqualTo(15));

        // Підказка зілля лишається, навіть коли зілля стало менше
        window.tooltip.Show(window.slots[2], Vector2.zero);
        Assert.That(window.tooltip.IsVisible, Is.True);
        player.Inventory.State.TryRemove(Potion(player), 1);
        Assert.That(window.tooltip.IsVisible, Is.True);

        window.Close();
    }

    // Підказка не вилазить за краї екрана, а лічильники сумки і ресурсів правильні
    [UnityTest]
    public IEnumerator TooltipClampsToAllScreenCornersAndLiveBagResourceCountsMatchModel()
    {
        BattleUnit player = Player();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        window.Open();
        yield return null;
        Canvas.ForceUpdateCanvases();

        // 4 кути екрана
        Vector2[] corners = new Vector2[]
        {
            Vector2.zero,
            new Vector2(Screen.width, 0),
            new Vector2(0, Screen.height),
            new Vector2(Screen.width, Screen.height)
        };

        foreach (Vector2 pointer in corners)
        {
            window.tooltip.Show(window.slots[2], pointer);
            window.tooltip.Position(pointer);
            Canvas.ForceUpdateCanvases();

            // Всі 4 кути підказки мають бути в межах екрана
            Vector3[] world = new Vector3[4];
            window.tooltip.panel.GetWorldCorners(world);
            foreach (Vector3 p in world)
            {
                Vector2 screen = RectTransformUtility.WorldToScreenPoint(null, p);
                Assert.That(screen.x, Is.InRange(-.5f, Screen.width + .5f));
                Assert.That(screen.y, Is.InRange(-.5f, Screen.height + .5f));
            }
        }

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

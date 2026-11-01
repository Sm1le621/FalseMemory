using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator RealMouseHoverShowsItemAndHidesOnEmptyDragAndClose()
    {
        var window=Player().GetComponent<InventoryWindow>();var mouse=InputSystem.AddDevice<Mouse>();
        try
        {
            window.Open();yield return null;Canvas.ForceUpdateCanvases();
            var position=SlotScreenCenter(window.slots[2]);InputSystem.QueueStateEvent(mouse,new MouseState{position=position});yield return null;yield return null;
            Assert.That(window.tooltip.IsVisible,Is.True);Assert.That(window.tooltip.content.text,Does.Contain("Potion").And.Contain("Consumable").And.Contain("30 HP"));
            position=SlotScreenCenter(window.slots[9]);InputSystem.QueueStateEvent(mouse,new MouseState{position=position});yield return null;yield return null;
            Assert.That(window.tooltip.IsVisible,Is.False);
            position=SlotScreenCenter(window.slots[2]);InputSystem.QueueStateEvent(mouse,new MouseState{position=position});yield return null;yield return null;
            window.BeginDrag(2,position);Assert.That(window.tooltip.IsVisible,Is.False);window.EndDrag();
            window.tooltip.Show(window.slots[2],position);window.Close();Assert.That(window.tooltip.IsVisible,Is.False);
            Assert.That(Player().Inventory.Count(Potion(Player())),Is.EqualTo(3));
        }
        finally {InputSystem.RemoveDevice(mouse);window.Close();}
    }
    [UnityTest] public IEnumerator EquippedHoverAndBorderSurviveReplacementAndEmptying()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var equipment=player.GetComponent<EquipmentController>();
        var sword=Sword(player);
        window.Open();equipment.TryEquip(0);yield return null;
        var view=window.equipmentButtons[(int)EquipmentSlot.Weapon].GetComponent<EquipmentSlotView>();
        window.tooltip.Show(view,Vector2.zero);
        Assert.That(window.tooltip.IsVisible,Is.True);Assert.That(window.tooltip.content.text,Does.Contain("Iron Sword").And.Contain("ATK +5"));
        Assert.That(view.icon.sprite,Is.SameAs(sword.icon));Assert.That(view.border,Is.Not.Null);
        equipment.TryUnequip(EquipmentSlot.Weapon);Assert.That(window.tooltip.IsVisible,Is.False);Assert.That(player.EffectiveAttack,Is.EqualTo(15));
        window.tooltip.Show(window.slots[2],Vector2.zero);Assert.That(window.tooltip.IsVisible,Is.True);
        player.Inventory.State.TryRemove(Potion(player),1);Assert.That(window.tooltip.IsVisible,Is.True);
        window.Close();
    }
    [UnityTest] public IEnumerator TooltipClampsToAllScreenCornersAndLiveBagResourceCountsMatchModel()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();window.Open();yield return null;Canvas.ForceUpdateCanvases();
        var corners=new[]{Vector2.zero,new Vector2(Screen.width,0),new Vector2(0,Screen.height),new Vector2(Screen.width,Screen.height)};
        foreach(var pointer in corners)
        {
            window.tooltip.Show(window.slots[2],pointer);window.tooltip.Position(pointer);Canvas.ForceUpdateCanvases();
            var world=new Vector3[4];window.tooltip.panel.GetWorldCorners(world);
            foreach(var p in world){var screen=RectTransformUtility.WorldToScreenPoint(null,p);Assert.That(screen.x,Is.InRange(-.5f,Screen.width+.5f));Assert.That(screen.y,Is.InRange(-.5f,Screen.height+.5f));}
        }
        Assert.That(window.bagLabel.text,Does.Contain("3 / 30"));
        player.Health.TakeDamage(10);player.TrySpendMP(5);window.Refresh();Assert.That(window.resourcesLabel.text,Does.Contain("90 / 100").And.Contain("25 / 30"));
        Assert.That(window.equipmentButtons.Length,Is.EqualTo(2));window.Close();
    }
    [UnityTest] public IEnumerator WorldManaBarFollowsSharedManaWithoutChangingIt()
    {
        var player=Player();var hud=Object.FindAnyObjectByType<ManaHUD>();
        Assert.That(hud,Is.Not.Null);Assert.That(hud.unit,Is.SameAs(player));
        Assert.That(hud.mpSlider,Is.Not.Null);Assert.That(hud.mpText,Is.Not.Null);
        yield return null;
        Assert.That(hud.mpSlider.maxValue,Is.EqualTo(30));Assert.That(hud.mpSlider.value,Is.EqualTo(30));
        Assert.That(hud.mpText.text,Does.Contain("30 / 30"));
        player.TrySpendMP(12);yield return null;
        Assert.That(hud.mpSlider.value,Is.EqualTo(18));Assert.That(hud.mpText.text,Does.Contain("18 / 30"));
        Assert.That(player.CurrentMP,Is.EqualTo(18));
        player.RestoreMP(99);yield return null;
        Assert.That(hud.mpSlider.value,Is.EqualTo(30));Assert.That(hud.mpText.text,Does.Contain("30 / 30"));
    }
}

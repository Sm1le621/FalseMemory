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
            var position=SlotScreenCenter(window.slots[0]);InputSystem.QueueStateEvent(mouse,new MouseState{position=position});yield return null;yield return null;
            Assert.That(window.tooltip.IsVisible,Is.True);Assert.That(window.tooltip.content.text,Does.Contain("Potion").And.Contain("Common").And.Contain("30 HP"));
            position=SlotScreenCenter(window.slots[1]);InputSystem.QueueStateEvent(mouse,new MouseState{position=position});yield return null;yield return null;
            Assert.That(window.tooltip.IsVisible,Is.False);
            position=SlotScreenCenter(window.slots[0]);InputSystem.QueueStateEvent(mouse,new MouseState{position=position});yield return null;yield return null;
            window.BeginDrag(0,position);Assert.That(window.tooltip.IsVisible,Is.False);window.EndDrag();
            window.tooltip.Show(window.slots[0],position);window.Close();Assert.That(window.tooltip.IsVisible,Is.False);
            Assert.That(Player().Inventory.Count(Potion(Player())),Is.EqualTo(3));
        }
        finally {InputSystem.RemoveDevice(mouse);window.Close();}
    }
    [UnityTest] public IEnumerator EquippedHoverAndRarityRemainCorrectAfterReplacementAndEmptying()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var equipment=player.GetComponent<EquipmentController>();
        var sword=GameObject.Find("Pickup_IronSword").GetComponent<WorldItemPickup>().item;
        player.Inventory.TryAdd(sword,1);window.Open();equipment.TryEquip(1);yield return null;
        var view=window.equipmentButtons[(int)EquipmentSlot.Weapon].GetComponent<EquipmentSlotView>();
        window.tooltip.Show(view,Vector2.zero);
        Assert.That(window.tooltip.IsVisible,Is.True);Assert.That(window.tooltip.content.text,Does.Contain("Iron Sword").And.Contain("Rare").And.Contain("ATK +5"));
        Assert.That(view.icon.sprite,Is.SameAs(sword.icon));Assert.That(view.rarityBorder.effectColor,Is.EqualTo(ItemPresentation.RarityColor(ItemRarity.Rare)));
        equipment.TryUnequip(EquipmentSlot.Weapon);Assert.That(window.tooltip.IsVisible,Is.False);Assert.That(player.EffectiveAttack,Is.EqualTo(15));
        window.tooltip.Show(window.slots[1],Vector2.zero);Assert.That(window.tooltip.IsVisible,Is.True);
        player.Inventory.State.TryRemove(sword,1);Assert.That(window.tooltip.IsVisible,Is.False);
        window.Close();
    }
    [UnityTest] public IEnumerator TooltipClampsToAllScreenCornersAndLiveBagResourceCountsMatchModel()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();window.Open();yield return null;Canvas.ForceUpdateCanvases();
        var corners=new[]{Vector2.zero,new Vector2(Screen.width,0),new Vector2(0,Screen.height),new Vector2(Screen.width,Screen.height)};
        foreach(var pointer in corners)
        {
            window.tooltip.Show(window.slots[0],pointer);window.tooltip.Position(pointer);Canvas.ForceUpdateCanvases();
            var world=new Vector3[4];window.tooltip.panel.GetWorldCorners(world);
            foreach(var p in world){var screen=RectTransformUtility.WorldToScreenPoint(null,p);Assert.That(screen.x,Is.InRange(-.5f,Screen.width+.5f));Assert.That(screen.y,Is.InRange(-.5f,Screen.height+.5f));}
        }
        Assert.That(window.bagLabel.text,Does.Contain("1 / 30"));
        player.Inventory.State.TrySplit(0,1,1);Assert.That(window.bagLabel.text,Does.Contain("2 / 30"));
        player.Health.TakeDamage(10);player.TrySpendMP(5);window.Refresh();Assert.That(window.resourcesLabel.text,Does.Contain("90 / 100").And.Contain("25 / 30"));
        Assert.That(window.characterPortrait.sprite,Is.SameAs(player.battleSprite));Assert.That(window.equipmentButtons.Length,Is.EqualTo(8));window.Close();
    }
}

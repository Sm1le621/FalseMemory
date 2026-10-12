using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator RealMouseEquipsFromBagAndReturnsGearToChosenBagCell()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var sword=GameObject.Find("Pickup_IronSword").GetComponent<WorldItemPickup>().item;
        player.Inventory.TryAdd(sword,1);window.Open();yield return null;Canvas.ForceUpdateCanvases();
        var view=window.equipmentButtons[(int)EquipmentSlot.Weapon].GetComponent<EquipmentSlotView>();var mouse=InputSystem.AddDevice<Mouse>();
        try
        {
            Assert.That(view.icon.sprite,Is.SameAs(view.emptySprite));Vector2 source=SlotScreenCenter(window.slots[1]);
            var rect=view.GetComponent<RectTransform>();Vector2 target=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));
            InputSystem.QueueStateEvent(mouse,new MouseState{position=source});yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=source,buttons=1});yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=target,delta=target-source,buttons=1});yield return null;yield return null;
            Assert.That(view.DropAllowed,Is.True);Assert.That(window.equipmentButtons[(int)EquipmentSlot.Head].GetComponent<EquipmentSlotView>().DropAllowed,Is.False);
            InputSystem.QueueStateEvent(mouse,new MouseState{position=target});yield return null;yield return null;
            Assert.That(player.Equipment.State.GetSlot(EquipmentSlot.Weapon),Is.SameAs(sword));Assert.That(player.Inventory.Count(sword),Is.Zero);Assert.That(view.icon.sprite,Is.SameAs(sword.icon));Assert.That(player.EffectiveAttack,Is.EqualTo(20));
            source=target;target=SlotScreenCenter(window.slots[12]);
            InputSystem.QueueStateEvent(mouse,new MouseState{position=source,buttons=1});yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=target,delta=target-source,buttons=1});yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=target});yield return null;yield return null;
            Assert.That(player.Equipment.State.GetSlot(EquipmentSlot.Weapon),Is.Null);Assert.That(player.Inventory.State.GetSlot(12).Item,Is.SameAs(sword));Assert.That(player.Inventory.Count(sword),Is.EqualTo(1));
            Assert.That(view.icon.sprite,Is.SameAs(view.emptySprite));Assert.That(view.icon.color.a,Is.LessThan(1));Assert.That(view.DropAllowed,Is.Null);Assert.That(window.dragIcon.gameObject.activeSelf,Is.False);
        }
        finally {InputSystem.RemoveDevice(mouse);window.Close();}
    }
    [UnityTest] public IEnumerator InvalidDropCancelFullBagAndDoubleDropPreserveItemCounts()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var sword=GameObject.Find("Pickup_IronSword").GetComponent<WorldItemPickup>().item;
        player.Inventory.TryAdd(sword,1);window.Open();window.BeginDrag(1,Vector2.zero);
        Assert.That(window.DropOnEquipment(EquipmentSlot.Gloves),Is.False);Assert.That(player.Inventory.Count(sword),Is.EqualTo(1));
        window.BeginDrag(1,Vector2.zero);window.EndDrag();Assert.That(player.Inventory.Count(sword),Is.EqualTo(1));
        window.BeginDrag(1,Vector2.zero);Assert.That(window.DropOnEquipment(EquipmentSlot.Weapon),Is.True);Assert.That(window.DropOnEquipment(EquipmentSlot.Weapon),Is.False);
        var filler=ScriptableObject.CreateInstance<ItemData>();filler.id="drag_full_filler";filler.stackable=false;
        try
        {
            Assert.That(player.Inventory.TryAdd(filler,29),Is.True);window.BeginEquipmentDrag(EquipmentSlot.Weapon,Vector2.zero);
            Assert.That(window.DropOn(1),Is.False);Assert.That(window.message.text,Is.EqualTo("Inventory is full."));
            Assert.That(player.Equipment.State.GetSlot(EquipmentSlot.Weapon),Is.SameAs(sword));Assert.That(player.Inventory.Count(sword),Is.Zero);Assert.That(player.Inventory.Count(filler),Is.EqualTo(29));
            player.Inventory.State.TryRemove(filler,1);window.BeginEquipmentDrag(EquipmentSlot.Weapon,Vector2.zero);Assert.That(window.DropOn(1),Is.True);
            Assert.That(player.Inventory.Count(sword),Is.EqualTo(1));Assert.That(player.Equipment.State.GetSlot(EquipmentSlot.Weapon),Is.Null);
        }
        finally {Object.Destroy(filler);window.Close();}
        yield return null;
    }
    [UnityTest] public IEnumerator ShieldAccessoryMovesAndEmptySilhouettesUseSameEquipmentState()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var ring=ScriptableObject.CreateInstance<ItemData>();var shield=ScriptableObject.CreateInstance<ItemData>();
        ring.id="drag_ring";ring.category=ItemCategory.Accessory;ring.bonuses=new ItemStatBonus{attack=2};
        shield.id="drag_shield";shield.category=ItemCategory.Armor;shield.equipmentSlot=EquipmentSlot.OffHand;shield.bonuses=new ItemStatBonus{defense=3};
        try
        {
            player.Inventory.TryAdd(ring,1);player.Inventory.TryAdd(shield,1);window.Open();window.BeginDrag(1,Vector2.zero);Assert.That(window.DropOnEquipment(EquipmentSlot.Accessory1),Is.True);
            window.BeginEquipmentDrag(EquipmentSlot.Accessory1,Vector2.zero);Assert.That(window.DropOnEquipment(EquipmentSlot.Accessory2),Is.True);
            Assert.That(player.Equipment.State.GetSlot(EquipmentSlot.Accessory1),Is.Null);Assert.That(player.Equipment.State.GetSlot(EquipmentSlot.Accessory2),Is.SameAs(ring));Assert.That(player.EffectiveAttack,Is.EqualTo(17));
            window.BeginDrag(2,Vector2.zero);Assert.That(window.DropOnEquipment(EquipmentSlot.OffHand),Is.True);Assert.That(player.EffectiveDefense,Is.EqualTo(11));
            for(int i=0;i<window.equipmentButtons.Length;i++){var view=window.equipmentButtons[i].GetComponent<EquipmentSlotView>();Assert.That(view.emptySprite,Is.Not.Null);Assert.That(view.emptySprite.texture.width,Is.EqualTo(64));Assert.That(view.emptySprite.texture.filterMode,Is.EqualTo(FilterMode.Point));}
        }
        finally {Object.Destroy(ring);Object.Destroy(shield);window.Close();}
        yield return null;
    }
}

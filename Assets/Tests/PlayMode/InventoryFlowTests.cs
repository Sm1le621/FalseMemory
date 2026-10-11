using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator EquipmentButtonsApplyAndRemoveBonusesWithoutChangingBaseStatsOrResources()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var equipment=player.GetComponent<EquipmentController>();
        var sword=GameObject.Find("Pickup_IronSword").GetComponent<WorldItemPickup>().item;
        var armor=GameObject.Find("Pickup_LeatherArmor").GetComponent<WorldItemPickup>().item;
        var helmet=GameObject.Find("Pickup_IronHelmet").GetComponent<WorldItemPickup>().item;
        player.Inventory.TryAdd(sword,1);player.Inventory.TryAdd(armor,1);player.Inventory.TryAdd(helmet,1);
        Assert.That(window.Open(),Is.True);
        for(int i=0;i<5;i++)
        {
            window.Select(1);window.equipButton.onClick.Invoke();Assert.That(player.EffectiveAttack,Is.EqualTo(20));
            Assert.That(player.Inventory.Count(sword),Is.Zero);
            window.equipmentButtons[0].onClick.Invoke();Assert.That(player.EffectiveAttack,Is.EqualTo(15));
            Assert.That(player.Inventory.Count(sword),Is.EqualTo(1));
        }
        window.Select(1);window.equipButton.onClick.Invoke();window.Select(2);window.equipButton.onClick.Invoke();window.Select(3);window.equipButton.onClick.Invoke();
        Assert.That(player.EffectiveAttack,Is.EqualTo(20));Assert.That(player.EffectiveDefense,Is.EqualTo(14));
        Assert.That(player.attack,Is.EqualTo(15));Assert.That(player.defense,Is.EqualTo(8));
        Assert.That(player.Health.currentHealth,Is.EqualTo(100));Assert.That(player.CurrentMP,Is.EqualTo(30));
        Assert.That(window.equipmentStats.text,Does.Contain("DEF 14"));
        Assert.That(equipment.State.GetSlot(EquipmentSlot.Head),Is.SameAs(helmet));window.Close();yield return null;
    }
    [UnityTest] public IEnumerator InventoryKeysBlockMovementAndEscapeRestoresIt()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var body=player.GetComponent<Rigidbody2D>();
        var keyboard=InputSystem.AddDevice<Keyboard>();Vector2 position=body.position;
        try
        {
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.I,Key.D));yield return null;
            yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
            Assert.That(window.IsOpen,Is.True);Assert.That(player.GetComponent<WorldInputGate>().IsBlocked,Is.True);
            Assert.That(body.position,Is.EqualTo(position));Assert.That(window.slots.Length,Is.EqualTo(30));
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape,Key.D));yield return null;
            yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
            Assert.That(window.IsOpen,Is.False);Assert.That(player.GetComponent<WorldInputGate>().IsBlocked,Is.False);
            Assert.That(body.position.x,Is.GreaterThan(position.x));
        }
        finally { InputSystem.RemoveDevice(keyboard);window.Close(); }
    }
    [UnityTest] public IEnumerator InventoryPointerHandlersMoveSwapMergeAndCancelWithoutDuplication()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var inventory=player.Inventory.State;var potion=Potion(player);
        var material=ScriptableObject.CreateInstance<ItemData>();material.id="test.material";material.category=ItemCategory.Material;material.maxStack=10;
        try
        {
            Assert.That(window.Open(),Is.True);window.Select(0);window.SplitSelected();
            Assert.That(inventory.GetSlot(0).Count,Is.EqualTo(2));Assert.That(inventory.GetSlot(1).Count,Is.EqualTo(1));
            DragSlot(window,0,2);inventory.TryAdd(material,3);DragSlot(window,0,2);
            Assert.That(inventory.GetSlot(2).Item,Is.SameAs(material));Assert.That(inventory.GetSlot(0).Item,Is.SameAs(potion));
            DragSlot(window,0,1);Assert.That(inventory.GetSlot(1).Count,Is.EqualTo(3));
            Assert.That(inventory.GetSlot(0).IsEmpty,Is.True);Assert.That(inventory.Count(potion),Is.EqualTo(3));
            window.BeginDrag(1,Vector2.zero);window.EndDrag();
            Assert.That(inventory.Count(potion),Is.EqualTo(3));Assert.That(window.dragIcon.gameObject.activeSelf,Is.False);
            Assert.That(window.slots[1].quantity.text,Is.EqualTo("3"));yield return null;
        }
        finally { window.Close();Object.Destroy(material); }
    }
    private static void DragSlot(InventoryWindow window,int source,int target)
    {
        var data=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,pointerDrag=window.slots[source].gameObject,position=Vector2.zero};
        ExecuteEvents.Execute(window.slots[source].gameObject,data,ExecuteEvents.beginDragHandler);
        ExecuteEvents.Execute(window.slots[target].gameObject,data,ExecuteEvents.dropHandler);
        ExecuteEvents.Execute(window.slots[source].gameObject,data,ExecuteEvents.endDragHandler);
    }

    [UnityTest] public IEnumerator PickupERejectsFullInventoryThenCollectsExactlyOnceAndRespectsModalInput()
    {
        var player=Player();var inventory=player.Inventory.State;var window=player.GetComponent<InventoryWindow>();
        var filler=ScriptableObject.CreateInstance<ItemData>();filler.id="test.filler";filler.stackable=false;
        var loot=ScriptableObject.CreateInstance<ItemData>();loot.id="test.loot";
        var obj=new GameObject("Test pickup",typeof(CircleCollider2D),typeof(WorldItemPickup));
        var pickup=obj.GetComponent<WorldItemPickup>();obj.GetComponent<CircleCollider2D>().isTrigger=true;
        pickup.item=loot;pickup.quantity=2;obj.transform.position=player.transform.position+Vector3.right*0.2f;Physics2D.SyncTransforms();
        var keyboard=InputSystem.AddDevice<Keyboard>();
        try
        {
            Assert.That(inventory.TryAdd(filler,29),Is.True);window.Open();
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.E));yield return null;
            Assert.That(pickup.Collected,Is.False);window.Close();
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSecondsRealtime(0.2f);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.E));yield return null;yield return null;
            Assert.That(pickup.Collected,Is.False);Assert.That(obj.activeSelf,Is.True);Assert.That(inventory.Count(loot),Is.Zero);
            Assert.That(player.GetComponent<PlayerInteractor>().notification.text,Does.Contain("Нет места"));
            inventory.TryRemove(filler,1);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSecondsRealtime(0.2f);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.E));yield return null;yield return null;
            Assert.That(pickup.Collected,Is.True);Assert.That(obj.activeSelf,Is.False);Assert.That(inventory.Count(loot),Is.EqualTo(2));
            for(int i=0;i<10;i++)Assert.That(pickup.TryInteract(player,out _),Is.False);
            Assert.That(inventory.Count(loot),Is.EqualTo(2));
        }
        finally {InputSystem.RemoveDevice(keyboard);window.Close();Object.Destroy(obj);Object.Destroy(filler);Object.Destroy(loot);}
    }
}

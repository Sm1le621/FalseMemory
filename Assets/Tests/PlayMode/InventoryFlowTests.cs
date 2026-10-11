using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
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
}

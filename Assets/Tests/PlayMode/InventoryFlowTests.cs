using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
    [UnityTest] public IEnumerator RealMouseRaycastDragsAndCancelsThroughInputSystem()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var potion=Potion(player);
        var mouse=InputSystem.AddDevice<Mouse>();
        try
        {
            Assert.That(window.Open(),Is.True);yield return null;Canvas.ForceUpdateCanvases();
            Vector2 source=SlotScreenCenter(window.slots[2]);Vector2 target=SlotScreenCenter(window.slots[9]);
            InputSystem.QueueStateEvent(mouse,new MouseState{position=source});yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=source,buttons=1});yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=target,delta=target-source,buttons=1});yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=target});yield return null;yield return null;
            Assert.That(player.Inventory.State.GetSlot(2).IsEmpty,Is.True);
            Assert.That(player.Inventory.State.GetSlot(9).Count,Is.EqualTo(3));Assert.That(player.Inventory.Count(potion),Is.EqualTo(3));
            InputSystem.QueueStateEvent(mouse,new MouseState{position=target,buttons=1});yield return null;yield return null;
            Vector2 outside=new Vector2(Screen.width+50,Screen.height+50);
            InputSystem.QueueStateEvent(mouse,new MouseState{position=outside,delta=outside-target,buttons=1});yield return null;yield return null;
            InputSystem.QueueStateEvent(mouse,new MouseState{position=outside});yield return null;yield return null;
            Assert.That(player.Inventory.State.GetSlot(9).Count,Is.EqualTo(3));Assert.That(window.dragIcon.gameObject.activeSelf,Is.False);
        }
        finally {InputSystem.RemoveDevice(mouse);window.Close();}
    }
    private static Vector2 SlotScreenCenter(InventorySlotView slot)
    {
        var rect=slot.GetComponent<RectTransform>();return RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));
    }
    [UnityTest] public IEnumerator SharedInventoryEquipmentAndManaSurviveBattleVictoryAndWorldReturn()
    {
        var player=Player();var enemy=Enemy();var window=player.GetComponent<InventoryWindow>();
        var use=player.GetComponent<WorldItemUseController>();var equipment=player.GetComponent<EquipmentController>();
        var potion=Potion(player);var sword=Sword(player);var armor=Armor(player);
        player.Health.TakeDamage(50);window.Open();window.Select(2);window.useButton.onClick.Invoke();
        Assert.That(player.Health.currentHealth,Is.EqualTo(80));Assert.That(player.Inventory.Count(potion),Is.EqualTo(2));
        Assert.That(use.TryUse(2),Is.False,"A duplicate callback in the same frame is rejected");
        yield return null;
        equipment.TryEquip(0);equipment.TryEquip(1);window.Close();
        Assert.That(player.EffectiveAttack,Is.EqualTo(20));Assert.That(player.EffectiveDefense,Is.EqualTo(12));
        var shared=player.Inventory.State;var home=player.transform.position;
        yield return BeginBattle(player,enemy);var manager=Manager();
        Assert.That(manager.Player.Inventory.State,Is.SameAs(shared));Assert.That(equipment.TryUnequip(EquipmentSlot.Weapon),Is.False);
        manager.TryPlayerAction(BattleAction.Attack);yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth,Is.EqualTo(45));Assert.That(player.Health.currentHealth,Is.EqualTo(79));
        manager.TryPlayerSpell(Fire(player));yield return WaitUntil(()=>manager.State==BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP,Is.EqualTo(22));Assert.That(enemy.Health.currentHealth,Is.EqualTo(26));
        enemy.Health.TakeDamage(26);yield return WaitUntil(()=>manager.State==BattleState.Victory);
        Assert.That(BattleTransition.Instance.ReturnAfterVictory(),Is.True);yield return WaitUntil(()=>!BattleTransition.Instance.EncounterActive);
        Assert.That(player.Inventory.State,Is.SameAs(shared));Assert.That(player.transform.position,Is.EqualTo(home));
        Assert.That(player.EffectiveAttack,Is.EqualTo(20));Assert.That(player.EffectiveDefense,Is.EqualTo(12));
        Assert.That(equipment.State.GetSlot(EquipmentSlot.Weapon),Is.SameAs(sword));
        Assert.That(equipment.State.GetSlot(EquipmentSlot.Chest),Is.SameAs(armor));
        Assert.That(shared.Count(sword),Is.Zero);Assert.That(shared.Count(armor),Is.Zero);
        Assert.That(player.Health.currentHealth,Is.EqualTo(78));Assert.That(player.CurrentMP,Is.EqualTo(22));
        Assert.That(player.Health.hpText.text,Does.Contain("78"));
    }
    [UnityTest] public IEnumerator WorldUseRestrictionsDisableButtonsWithoutConsumingItems()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var use=player.GetComponent<WorldItemUseController>();
        var item=ScriptableObject.CreateInstance<BattleConsumable>();item.id="test.restricted";item.useLocations=ItemUseLocation.Battle;item.healing=10;
        try
        {
            player.Inventory.TryAdd(item,1);player.Health.TakeDamage(50);window.Open();window.Select(3);
            Assert.That(window.useButton.interactable,Is.False);Assert.That(use.TryUse(3),Is.False);
            item.useLocations=ItemUseLocation.Anywhere;window.Refresh();
            Assert.That(window.useButton.interactable,Is.True);
            Assert.That(use.TryUse(3),Is.True);
            Assert.That(player.Inventory.Count(item),Is.Zero);Assert.That(player.Health.currentHealth,Is.EqualTo(60));yield return null;
        }
        finally {window.Close();Object.Destroy(item);}
    }
    [UnityTest] public IEnumerator EquipmentButtonsApplyAndRemoveBonusesWithoutChangingBaseStatsOrResources()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var equipment=player.GetComponent<EquipmentController>();
        var sword=Sword(player);var armor=Armor(player);
        Assert.That(window.Open(),Is.True);
        for(int i=0;i<5;i++)
        {
            window.Select(0);window.equipButton.onClick.Invoke();Assert.That(player.EffectiveAttack,Is.EqualTo(20));
            Assert.That(player.Inventory.Count(sword),Is.Zero);
            window.equipmentButtons[0].onClick.Invoke();Assert.That(player.EffectiveAttack,Is.EqualTo(15));
            Assert.That(player.Inventory.Count(sword),Is.EqualTo(1));
        }
        window.Select(1);window.equipButton.onClick.Invoke();
        Assert.That(player.EffectiveAttack,Is.EqualTo(15));Assert.That(player.EffectiveDefense,Is.EqualTo(12));
        Assert.That(player.attack,Is.EqualTo(15));Assert.That(player.defense,Is.EqualTo(8));
        Assert.That(player.Health.currentHealth,Is.EqualTo(100));Assert.That(player.CurrentMP,Is.EqualTo(30));
        Assert.That(window.equipmentStats.text,Does.Contain("DEF 12"));
        Assert.That(equipment.State.GetSlot(EquipmentSlot.Chest),Is.SameAs(armor));window.Close();yield return null;
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
            Assert.That(window.modalBackdrop.gameObject.activeSelf,Is.True);
            Assert.That(body.position,Is.EqualTo(position));Assert.That(window.slots.Length,Is.EqualTo(30));
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape,Key.D));yield return null;
            yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
            Assert.That(window.IsOpen,Is.False);Assert.That(player.GetComponent<WorldInputGate>().IsBlocked,Is.False);
            Assert.That(window.modalBackdrop.gameObject.activeSelf,Is.False);
            Assert.That(body.position.x,Is.GreaterThan(position.x));
        }
        finally { InputSystem.RemoveDevice(keyboard);window.Close(); }
    }
    [UnityTest] public IEnumerator InventoryPointerHandlersMoveSwapMergeAndCancelWithoutDuplication()
    {
        var player=Player();var window=player.GetComponent<InventoryWindow>();var inventory=player.Inventory.State;var potion=Potion(player);
        var material=ScriptableObject.CreateInstance<ItemData>();material.id="test.material";material.maxStack=10;
        try
        {
            Assert.That(window.Open(),Is.True);
            DragSlot(window,2,9);
            Assert.That(inventory.GetSlot(9).Item,Is.SameAs(potion));Assert.That(inventory.GetSlot(2).IsEmpty,Is.True);
            inventory.TryAdd(material,3);
            DragSlot(window,2,3);
            Assert.That(inventory.GetSlot(3).Item,Is.SameAs(material));
            DragSlot(window,3,9);
            Assert.That(inventory.GetSlot(9).Item,Is.SameAs(material));Assert.That(inventory.GetSlot(3).Item,Is.SameAs(potion));
            Assert.That(inventory.Count(potion),Is.EqualTo(3));
            window.BeginDrag(9,Vector2.zero);window.EndDrag();
            Assert.That(inventory.Count(material),Is.EqualTo(3));Assert.That(window.dragIcon.gameObject.activeSelf,Is.False);
            yield return null;
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

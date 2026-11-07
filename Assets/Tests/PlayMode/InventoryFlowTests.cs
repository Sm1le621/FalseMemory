using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

// Тести вікна інвентаря і екіпіровки у світі (частина класу BattleFlowTests)
public sealed partial class BattleFlowTests
{
    // Справжня мишка через Input System перетягує предмет і скасовує перетягування
    [UnityTest]
    public IEnumerator RealMouseRaycastDragsAndCancelsThroughInputSystem()
    {
        BattleUnit player = Player();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        BattleConsumable potion = Potion(player);
        Mouse mouse = InputSystem.AddDevice<Mouse>();
        try
        {
            Assert.That(window.Open(), Is.True);
            yield return null;
            Canvas.ForceUpdateCanvases();

            Vector2 source = SlotScreenCenter(window.slots[2]);
            Vector2 target = SlotScreenCenter(window.slots[9]);

            // Навели на слот 2, натиснули, потягли на слот 9, відпустили
            MouseState hoverSource = new MouseState();
            hoverSource.position = source;
            InputSystem.QueueStateEvent(mouse, hoverSource);
            yield return null;
            yield return null;

            MouseState pressSource = new MouseState();
            pressSource.position = source;
            pressSource.buttons = 1;
            InputSystem.QueueStateEvent(mouse, pressSource);
            yield return null;
            yield return null;

            MouseState dragToTarget = new MouseState();
            dragToTarget.position = target;
            dragToTarget.delta = target - source;
            dragToTarget.buttons = 1;
            InputSystem.QueueStateEvent(mouse, dragToTarget);
            yield return null;
            yield return null;

            MouseState releaseTarget = new MouseState();
            releaseTarget.position = target;
            InputSystem.QueueStateEvent(mouse, releaseTarget);
            yield return null;
            yield return null;

            Assert.That(player.Inventory.State.GetSlot(2).IsEmpty, Is.True);
            Assert.That(player.Inventory.State.GetSlot(9).Count, Is.EqualTo(3));
            Assert.That(player.Inventory.Count(potion), Is.EqualTo(3));

            // Тепер тягнемо за межі екрана і відпускаємо - нічого не має змінитись
            MouseState pressTarget = new MouseState();
            pressTarget.position = target;
            pressTarget.buttons = 1;
            InputSystem.QueueStateEvent(mouse, pressTarget);
            yield return null;
            yield return null;

            Vector2 outside = new Vector2(Screen.width + 50, Screen.height + 50);
            MouseState dragOutside = new MouseState();
            dragOutside.position = outside;
            dragOutside.delta = outside - target;
            dragOutside.buttons = 1;
            InputSystem.QueueStateEvent(mouse, dragOutside);
            yield return null;
            yield return null;

            MouseState releaseOutside = new MouseState();
            releaseOutside.position = outside;
            InputSystem.QueueStateEvent(mouse, releaseOutside);
            yield return null;
            yield return null;

            Assert.That(player.Inventory.State.GetSlot(9).Count, Is.EqualTo(3));
            Assert.That(window.dragIcon.gameObject.activeSelf, Is.False);
        }
        finally
        {
            InputSystem.RemoveDevice(mouse);
            window.Close();
        }
    }

    // Центр клітинки в екранних координатах
    private static Vector2 SlotScreenCenter(InventorySlotView slot)
    {
        RectTransform rect = slot.GetComponent<RectTransform>();
        Vector3 worldCenter = rect.TransformPoint(rect.rect.center);
        return RectTransformUtility.WorldToScreenPoint(null, worldCenter);
    }

    // Інвентар, екіпіровка і мана - спільні для світу і бою, і переживають перемогу
    [UnityTest]
    public IEnumerator SharedInventoryEquipmentAndManaSurviveBattleVictoryAndWorldReturn()
    {
        BattleUnit player = Player();
        BattleUnit enemy = Enemy();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        WorldItemUseController use = player.GetComponent<WorldItemUseController>();
        EquipmentController equipment = player.GetComponent<EquipmentController>();
        BattleConsumable potion = Potion(player);
        ItemData sword = Sword(player);
        ItemData armor = Armor(player);

        // П'ємо зілля у світі через кнопку Use
        player.Health.TakeDamage(50);
        window.Open();
        window.Select(2);
        window.useButton.onClick.Invoke();
        Assert.That(player.Health.currentHealth, Is.EqualTo(80));
        Assert.That(player.Inventory.Count(potion), Is.EqualTo(2));
        Assert.That(use.TryUse(2), Is.False, "A duplicate callback in the same frame is rejected");
        yield return null;

        // Одягаємо меч і броню
        equipment.TryEquip(0);
        equipment.TryEquip(1);
        window.Close();
        Assert.That(player.EffectiveAttack, Is.EqualTo(20));
        Assert.That(player.EffectiveDefense, Is.EqualTo(12));

        InventoryState shared = player.Inventory.State;
        Vector3 home = player.transform.position;

        yield return BeginBattle(player, enemy);
        BattleManager manager = Manager();
        // В бою той самий інвентар, і екіпіровку міняти не можна
        Assert.That(manager.Player.Inventory.State, Is.SameAs(shared));
        Assert.That(equipment.TryUnequip(EquipmentSlot.Weapon), Is.False);

        manager.TryPlayerAction(BattleAction.Attack);
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(45));
        Assert.That(player.Health.currentHealth, Is.EqualTo(79));

        manager.TryPlayerSpell(Fire(player));
        yield return WaitUntil(() => manager.State == BattleState.AwaitingPlayer);
        Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(enemy.Health.currentHealth, Is.EqualTo(26));

        enemy.Health.TakeDamage(26);
        yield return WaitUntil(() => manager.State == BattleState.Victory);
        Assert.That(BattleTransition.Instance.ReturnAfterVictory(), Is.True);
        yield return WaitUntil(() => !BattleTransition.Instance.EncounterActive);

        // Після повернення все те саме
        Assert.That(player.Inventory.State, Is.SameAs(shared));
        Assert.That(player.transform.position, Is.EqualTo(home));
        Assert.That(player.EffectiveAttack, Is.EqualTo(20));
        Assert.That(player.EffectiveDefense, Is.EqualTo(12));
        Assert.That(equipment.State.GetSlot(EquipmentSlot.Weapon), Is.SameAs(sword));
        Assert.That(equipment.State.GetSlot(EquipmentSlot.Chest), Is.SameAs(armor));
        Assert.That(shared.Count(sword), Is.Zero);
        Assert.That(shared.Count(armor), Is.Zero);
        Assert.That(player.Health.currentHealth, Is.EqualTo(78));
        Assert.That(player.CurrentMP, Is.EqualTo(22));
        Assert.That(player.Health.hpText.text, Does.Contain("78"));
    }

    // Предмет "тільки для бою" у світі не використовується, а кнопка Use вимкнена
    [UnityTest]
    public IEnumerator WorldUseRestrictionsDisableButtonsWithoutConsumingItems()
    {
        BattleUnit player = Player();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        WorldItemUseController use = player.GetComponent<WorldItemUseController>();

        BattleConsumable item = ScriptableObject.CreateInstance<BattleConsumable>();
        item.id = "test.restricted";
        item.useLocations = ItemUseLocation.Battle;
        item.healing = 10;
        try
        {
            player.Inventory.TryAdd(item, 1);
            player.Health.TakeDamage(50);
            window.Open();
            window.Select(3);
            Assert.That(window.useButton.interactable, Is.False);
            Assert.That(use.TryUse(3), Is.False);

            // Дозволяємо всюди - тепер можна
            item.useLocations = ItemUseLocation.Anywhere;
            window.Refresh();
            Assert.That(window.useButton.interactable, Is.True);
            Assert.That(use.TryUse(3), Is.True);
            Assert.That(player.Inventory.Count(item), Is.Zero);
            Assert.That(player.Health.currentHealth, Is.EqualTo(60));
            yield return null;
        }
        finally
        {
            window.Close();
            Object.Destroy(item);
        }
    }

    // Кнопки екіпіровки додають і забирають бонуси, базові характеристики не змінюються
    [UnityTest]
    public IEnumerator EquipmentButtonsApplyAndRemoveBonusesWithoutChangingBaseStatsOrResources()
    {
        BattleUnit player = Player();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        EquipmentController equipment = player.GetComponent<EquipmentController>();
        ItemData sword = Sword(player);
        ItemData armor = Armor(player);

        Assert.That(window.Open(), Is.True);

        // 5 разів одягаємо і знімаємо меч
        for (int i = 0; i < 5; i++)
        {
            window.Select(0);
            window.equipButton.onClick.Invoke();
            Assert.That(player.EffectiveAttack, Is.EqualTo(20));
            Assert.That(player.Inventory.Count(sword), Is.Zero);

            window.equipmentButtons[0].onClick.Invoke();
            Assert.That(player.EffectiveAttack, Is.EqualTo(15));
            Assert.That(player.Inventory.Count(sword), Is.EqualTo(1));
        }

        // Одягаємо броню
        window.Select(1);
        window.equipButton.onClick.Invoke();
        Assert.That(player.EffectiveAttack, Is.EqualTo(15));
        Assert.That(player.EffectiveDefense, Is.EqualTo(12));
        Assert.That(player.attack, Is.EqualTo(15));
        Assert.That(player.defense, Is.EqualTo(8));
        Assert.That(player.Health.currentHealth, Is.EqualTo(100));
        Assert.That(player.CurrentMP, Is.EqualTo(30));
        Assert.That(window.equipmentStats.text, Does.Contain("DEF 12"));
        Assert.That(equipment.State.GetSlot(EquipmentSlot.Chest), Is.SameAs(armor));

        window.Close();
        yield return null;
    }

    // Клавіша I відкриває інвентар і блокує рух, Esc закриває і повертає рух
    [UnityTest]
    public IEnumerator InventoryKeysBlockMovementAndEscapeRestoresIt()
    {
        BattleUnit player = Player();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        Vector2 position = body.position;
        try
        {
            // I + D: вікно відкривається, і гравець НЕ йде
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.I, Key.D));
            yield return null;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.That(window.IsOpen, Is.True);
            Assert.That(player.GetComponent<WorldInputGate>().IsBlocked, Is.True);
            Assert.That(window.modalBackdrop.gameObject.activeSelf, Is.True);
            Assert.That(body.position, Is.EqualTo(position));
            Assert.That(window.slots.Length, Is.EqualTo(30));

            // Esc + D: вікно закривається, і гравець іде
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Escape, Key.D));
            yield return null;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.That(window.IsOpen, Is.False);
            Assert.That(player.GetComponent<WorldInputGate>().IsBlocked, Is.False);
            Assert.That(window.modalBackdrop.gameObject.activeSelf, Is.False);
            Assert.That(body.position.x, Is.GreaterThan(position.x));
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard);
            window.Close();
        }
    }

    // Перетягування: переміщення, обмін і скасування без дублювання предметів
    [UnityTest]
    public IEnumerator InventoryPointerHandlersMoveSwapMergeAndCancelWithoutDuplication()
    {
        BattleUnit player = Player();
        InventoryWindow window = player.GetComponent<InventoryWindow>();
        InventoryState inventory = player.Inventory.State;
        BattleConsumable potion = Potion(player);

        ItemData material = ScriptableObject.CreateInstance<ItemData>();
        material.id = "test.material";
        material.maxStack = 10;
        try
        {
            Assert.That(window.Open(), Is.True);

            // Зілля: слот 2 -> слот 9
            DragSlot(window, 2, 9);
            Assert.That(inventory.GetSlot(9).Item, Is.SameAs(potion));
            Assert.That(inventory.GetSlot(2).IsEmpty, Is.True);

            // Матеріал лягає в слот 2, переносимо в 3
            inventory.TryAdd(material, 3);
            DragSlot(window, 2, 3);
            Assert.That(inventory.GetSlot(3).Item, Is.SameAs(material));

            // Міняємо місцями матеріал і зілля
            DragSlot(window, 3, 9);
            Assert.That(inventory.GetSlot(9).Item, Is.SameAs(material));
            Assert.That(inventory.GetSlot(3).Item, Is.SameAs(potion));
            Assert.That(inventory.Count(potion), Is.EqualTo(3));

            // Почали і одразу скасували перетягування
            window.BeginDrag(9, Vector2.zero);
            window.EndDrag();
            Assert.That(inventory.Count(material), Is.EqualTo(3));
            Assert.That(window.dragIcon.gameObject.activeSelf, Is.False);
            yield return null;
        }
        finally
        {
            window.Close();
            Object.Destroy(material);
        }
    }

    // Імітація перетягування з клітинки source на клітинку target
    private static void DragSlot(InventoryWindow window, int source, int target)
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.button = PointerEventData.InputButton.Left;
        data.pointerDrag = window.slots[source].gameObject;
        data.position = Vector2.zero;

        ExecuteEvents.Execute(window.slots[source].gameObject, data, ExecuteEvents.beginDragHandler);
        ExecuteEvents.Execute(window.slots[target].gameObject, data, ExecuteEvents.dropHandler);
        ExecuteEvents.Execute(window.slots[source].gameObject, data, ExecuteEvents.endDragHandler);
    }
}

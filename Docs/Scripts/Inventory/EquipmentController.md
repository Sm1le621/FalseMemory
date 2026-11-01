# EquipmentController

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/EquipmentController.cs`.

Связывает `EquipmentState` с теми же объектами Player, на которых находятся `BattleUnit`, `BattleInventory` и `InventoryWindow`. Компонент принимает команды окна, проверяет контекст и не создаёт второго персонажа.

## 2. Как работает

В `Awake` компонент получает `BattleUnit`, `BattleInventory` и `InventoryWindow`. В `OnEnable` он подписывается на запросы экипировки/снятия и на изменение `State`, а в `OnDisable` снимает эти подписки. Само содержимое `EquipmentState` при скрытии объекта не сбрасывается.

Смена экипировки разрешена только активному и включённому компоненту живого персонажа вне активной встречи. Успешная команда меняет владение через `EquipmentState`, показывает английское сообщение и обновляет окно. Отказ не меняет инвентарь и экипировку.

## 3. Методы и функции

- `Awake()` — Unity callback; кеширует `unit`, `inventory`, `window`.
- `OnEnable()` — подписывает `window.EquipRequested` на `HandleEquip`, `window.UnequipRequested` на `HandleUnequip`, `State.Changed` на `window.Refresh`.
- `OnDisable()` — снимает те же подписки. Если окна нет, ничего не делает.
- `CanChange` — private bool: компонент активен, `unit` существует, персонаж жив и встреча не активна.
- `TryEquip(int index, EquipmentSlot? target = null)` — проверяет `CanChange`, индекс `0..29`, наличие предмета. Без явной цели оружие идёт в `EquipmentSlot.Weapon`, остальное — в `item.equipmentSlot`; затем вызывает `State.TryEquip`.
- `TryUnequip(EquipmentSlot slot)` — проверяет `CanChange` и вызывает `State.TryUnequip`.
- `HandleEquip(int index, EquipmentSlot? target)` — выполняет `TryEquip`, показывает `"Item equipped."` либо `"Cannot equip this item."`, обновляет окно.
- `HandleUnequip(EquipmentSlot slot)` — выполняет `TryUnequip`, показывает `"Item unequipped."` либо `"Cannot unequip: inventory is full or the slot is empty."`, обновляет окно.

## 4. Переменные и Inspector

- `State` — readonly свойство, создаёт один `EquipmentState` на компонент.
- `unit`, `inventory`, `window` — private ссылки, заполняются в `Awake`.
- Inspector-полей у компонента нет; данные предметов находятся в `ItemData`.

`EquipmentController` не хранит отдельные копии предметов и не владеет HP/MP.

## 5. Связи

Требуются `BattleUnit` и `BattleInventory`; `InventoryWindow` не обязателен, но без него нет команд и обновления UI. `EquipmentState` использует `InventoryState` для атомарного обмена и возврата предметов. `BattleUnit` читает `State.Bonuses` через `EffectiveAttack` и `EffectiveDefense`.

## 6. Пример

В стартовом инвентаре есть Iron Sword в слоте 0. Выбор меча и нажатие Equip вызывает `TryEquip(0, null)`: меч уходит из сумки в слот Weapon, `EffectiveAttack` увеличивается с 15 до 20. Повторное нажатие на слот Weapon снимает меч и возвращает его в свободную ячейку.

## 7. Настройка в Unity

Компонент находится на Player вместе с `BattleUnit`, `BattleInventory` и `InventoryWindow`. В сцене должны быть назначены две кнопки экипировки (`Weapon`, `Chest`) и ссылки окна. Дополнительных ассетов или singleton-менеджеров не требуется.

## 8. Ограничения и расширение

В бою и при мёртвом персонаже экипировка запрещена. Поддержаны только два слота и только бонусы `attack`/`defense`. Нет уровней, классов, редкости, магических бонусов и дискового сохранения. Новый слот потребует изменения enum, `EquipmentState.SlotCount`, UI и правил `ItemData.Fits`.

## 9. История изменений

- 2026-10-31 — документ приведён к текущему коду: два слота, только ATK/DEF, английские сообщения; удалены утверждения о восьми слотах, пяти характеристиках, уровнях и GameText.
- 2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена. *(Историческая запись: GameText и StringTables удалены.)*
- 2026-10-12 — 4.4: интеграция компонентов/ограничение фазы/команды окна. *(Историческая запись: описывает прежнюю модель экипировки.)*

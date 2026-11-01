# InventoryWindow

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/InventoryWindow.cs` (частичный класс, остальные части в этой же папке).

Окно инвентаря в исследовании: 30 слотов, выбор, drag & drop, использование зелий, экипировка Weapon/Chest, золото, HP/MP и подсказки. Окно не создаёт второго персонажа и работает с теми же `BattleInventory`, `BattleUnit`, `EquipmentController`, что и бой.

## 2. Как работает

`Awake` получает `BattleInventory`, `WorldInputGate`, `BattleUnit`, привязывает слоты и кнопки, затем закрывает окно. `OnEnable` подписывается на `InventoryState.Changed`; `OnDisable` отписывается и закрывает окно. В `Update` клавиша `I` открывает/закрывает инвентарь, Escape закрывает, стрелки перемещают выбор. Открытие запрещено во время активной встречи и для мёртвого персонажа.

`Open` включает панель и захватывает ввод через `WorldInputGate`, `Close` отпускает его. `Refresh` рисует слоты, описание выбранного предмета, золото, занятость сумки, доступность Use/Equip и состояние двух слотов экипировки. Перетаскивание вызывает `InventoryState.TryMoveOrMerge`; при неудаче показывается сообщение. Модели персонажа, редкости и разделения предметов по категориям в окне нет.

## 3. Методы и функции

- `Awake()` — Unity callback; кеширует компоненты, привязывает слоты/кнопки, закрывает окно.
- `OnEnable()` / `OnDisable()` / `OnDestroy()` — управляют подпиской на события и listeners.
- `Update()` — читает `I`, Escape и стрелки; обновляет ресурсы при открытом окне.
- `Open()` → `bool`; включает панель, захватывает ввод, обновляет содержимое. Возвращает `false` при недоступном контексте.
- `Close()` — скрывает подсказку, завершает drag, выключает панель и отпускает ввод.
- `Select(int index)` — выбирает слот 0…29 и обновляет окно.
- `Refresh()` — отрисовывает слоты/описание/золото/сумку, доступность кнопок и экипировку.
- `RefreshResources()` — обновляет строку `HP x / y    MP x / y` только при изменении значений.
- `RequestUse()` / `RequestEquip()` — поднимают события `UseRequested`/`EquipRequested`.
- `ClickEquipmentSlot(EquipmentSlot slot)` — экипирует выбранный подходящий предмет или снимает предмет из слота.
- `ShowMessage(string text)` — выводит сообщение окна.
- `BeginDrag`, `MoveDrag`, `DropOn`, `EndDrag` — drag & drop и объединение стаков.
- `UseRequested`, `CanUseSelection`, `EquipRequested`, `UnequipRequested` — события/делегаты для контроллеров.
- `IsOpen`, `SelectedIndex`, `Inventory` — публичное состояние.

## 4. Переменные и Inspector

- `panel` — корень окна; `slots` — 30 `InventorySlotView`; `details` — описание; `goldLabel`, `bagLabel`, `message`, `resourcesLabel` — тексты.
- `useButton`, `closeButton`, `equipButton` — кнопки действий.
- `equipmentButtons` — две кнопки слотов Weapon/Chest; `equipmentStats` — строка `ATK/DEF`.
- `dragIcon` — изображение перетаскиваемого предмета; `modalBackdrop` — затемнение; `tooltip` — подсказка.
- `gate` — `WorldInputGate`; `unit` — `BattleUnit`; `dragging`, `equipmentClicks`, `shownHP/MP` — внутреннее состояние.

## 5. Связи

Требуются `BattleInventory`, `WorldInputGate`, `BattleUnit` и `InventoryWindow` на Player. `EquipmentController` обрабатывает экипировку, `WorldItemUseController` — использование, `InventoryTooltip` — подсказки, `ItemPresentation` — текст. `InventoryState` владеет слотами и золотом. Окно не работает во время `BattleTransition.EncounterActive`.

## 6. Пример

Нажатие `I` открывает окно. Выбор Potion и `Use` вызывает `WorldItemUseController`, HP восстанавливается, сообщение показывает `Potion: +30 HP`. Выбор Iron Sword и `Equip` отправляет предмет в слот Weapon, `ATK` в строке экипировки увеличивается. Перетаскивание зелья на другой слот объединяет стаки, если позволяет `maxStack`.

## 7. Настройка в Unity

Компонент находится на Player. Назначь панель, 30 слотов, две кнопки экипировки, тексты, кнопки Use/Close/Equip, drag icon, backdrop и tooltip. В `InventoryWindow.equipmentButtons` должно быть ровно два элемента; `EquipmentSlotView.slot` — Weapon/Chest. Проверь, что `WorldInputGate` есть на Player.

## 8. Возможные проблемы

Без `WorldInputGate` окно не сможет корректно блокировать движение. Если массив слотов короче 30, часть сумки не отобразится. Открытие во время боя запрещено. При полном инвентаре снятие экипировки отклоняется. Drag & drop не работает между разными типами предметов. Нет модели персонажа, редкости и отдельных вкладок — это ожидаемое упрощение.

## 9. История изменений

- 2026-10-31 — окно упрощено: 30 слотов, только Weapon/Chest, без модели, редкости, вкладок и подбора; тексты английские.
- 2026-10-12 — базовая версия окна инвентаря.

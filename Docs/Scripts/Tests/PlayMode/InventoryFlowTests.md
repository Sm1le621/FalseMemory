# InventoryFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/InventoryFlowTests.cs`.

Partial `BattleFlowTests`: настоящий ввод мышью/клавиатурой, drag & drop, использование предметов, экипировка и сохранение инвентаря после боя.

## 2. Как работает

Использует общий `Setup`/`Cleanup` и реальную `SampleScene`. Создаёт синтетические Mouse/Keyboard через Input System, вызывает UI-события и проверяет модель/компоненты. Временные устройства и предметы удаляются.

## 3. Методы и функции

- `RealMouseRaycastDragsAndCancelsThroughInputSystem()` — настоящий MouseState: перенос Potion 2→9, отмена за экраном без потери 3 предметов.
- `SharedInventoryEquipmentAndManaSurviveBattleVictoryAndWorldReturn()` — Potion, Sword, Armor; Attack/Fire; победа и возврат с теми же `InventoryState`, экипировкой, HP 78/MP 22/позицией.
- `WorldUseRestrictionsDisableButtonsWithoutConsumingItems()` — предмет с `useLocations = Battle` недоступен в мире; после Anywhere используется.
- `EquipmentButtonsApplyAndRemoveBonusesWithoutChangingBaseStatsOrResources()` — пять циклов Equip/Unequip меча, затем броня; ATK 20/DEF 12, базы 15/8, HP/MP не меняются.
- `InventoryKeysBlockMovementAndEscapeRestoresIt()` — `I` открывает окно и блокирует тело, Escape закрывает и возобновляет движение; 30 слотов/backdrop.
- `InventoryPointerHandlersMoveSwapMergeAndCancelWithoutDuplication()` — handlers begin/drop/end: перенос, обмен, слияние, отмена drag.

## 4. Переменные и Inspector

Локальные `player`, `window`, `keyboard`, `mouse`, временный `material`. Inspector отсутствует; реальные ассеты не сохраняются.

## 5. Связи

`BattleFlowTests`, `InventoryWindow`, `InventoryState`, `WorldItemUseController`, `EquipmentController`, `WorldInputGate`, `BattleTransition`, Input System/EventSystem.

## 6. Пример

Открытие окна: `I` + D — тело не двигается. Перенос зелья на слот 9 сохраняет количество 3. После победы меч остаётся надетым, ATK 20.

## 7. Настройка в Unity

Test Runner → PlayMode; нужны `SampleScene` с Player и настроенным инвентарём. Синтетические устройства создаются тестом.

## 8. Ограничения и проверки

Проверяет реальный UI-ввод, но не человеческое удобство. Не тестирует подбор предметов — его больше нет. 2026-10-31: PlayMode 36/36.

## 9. История изменений

- 2026-10-31 — удалены Ether/Helmet/EXP и сценарий подбора; ожидания HP/MP приведены к текущим.
- 2026-10-12 — базовая документация.

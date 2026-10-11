# InventoryFlowTests

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/InventoryFlowTests.cs.

Дополнительная часть BattleFlowTests для окна/мыши/ввода.

## 2. Как работает

Использует прежние Setup/Cleanup реальной SampleScene. Создаёт/удаляет синтетическую клавиатуру и только тестовые данные.

## 3. Методы и функции

`InventoryKeysBlockMovementAndEscapeRestoresIt` UnityTest IEnumerator: I+D открывает/останавливает тело, Escape+D закрывает/возобновляет;30 slots/gate. `InventoryPointerHandlersMoveSwapMergeAndCancelWithoutDuplication` проверяет split/drag/swap/merge/cancel/метку количества с исходным Potion. `DragSlot(window,source,target)` private void вызывает настоящие EventSystem handlers begin/drop/end через ExecuteEvents. Не полная симуляция мышиного raycast.

## 4. Переменные и Inspector

Локальные player/window/keyboard/material; временный material удаляется finally, устройство тоже. Inspector отсутствует.

## 5. Связи

FalseMemory.Battle.PlayModeTests/Runtime/InputSystem/EventSystem и общие fixtures BattleFlowTests.

## 6. Пример

Сценарий переносит Potion и материал, итоговые количества сохраняются.

## 7. Настройка в Unity

Не прикреплять; PlayMode/Test Runner или MCP с именем сборки. Проверить итоговое total>0.

## 8. Ограничения и расширение

Проверки handlers не доказывают физический путь курсора; реальный drag проверить вручную. Подбор/экипировка/использование ещё не входят.

## 9. История изменений

2026-10-12 — 4.2: два сценария окна/ввода/handlers.

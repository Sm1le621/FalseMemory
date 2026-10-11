# InventoryTests

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/InventoryTests.cs.

Семь граничных Edit Mode сценариев модели.

## 2. Как работает

Setup создаёт два временных ItemData с разными ID/лимитом10, Cleanup уничтожает тестовые SO. Assert проверяет итог/отказы.

## 3. Методы и функции

`Setup`/`Cleanup` — NUnit lifecycle. `ThirtySlotsFillAndRejectWithoutPartialAddition` —299+2 отказ/+1 успех/полный запас. `NonStackableAndEquipmentUseOneSlotEach` —30 оружий по1. `MoveSwapMergeAndSplitPreserveQuantities` — перенос/обмен/полная стопка/деление/частичное слияние. `RejectedTransactionsDoNotRaiseEventsOrMutate` — неверные количества/индексы/события. `RemovalAcrossStacksIsAtomicAndFreesSlots` —16 из15 отказ/удаление12. `ConflictingStableIdIsRejected` — одинаковый ID двух ссылок. `GoldHasOneBalanceAndRejectsUnderflowAndOverflow` — нехватка/отрицательный/предел int. Методы void, Unity callbacks нет.

## 4. Переменные и Inspector

item/other/inventory — временные данные, лимит10,30 слотов; Inspector отсутствует.

## 5. Связи

NUnit, FalseMemory.Battle.EditModeTests, Runtime и InventoryState/ItemData.

## 6. Пример

Run All проверяет невозможность частичного добавления при недостатке места.

## 7. Настройка в Unity

Не прикреплять. MCP run_tests EditMode с указанной сборкой/Test Runner; проверить итоговое total>0.

## 8. Ограничения и расширение

Не доказывает работу курсора/окна/подбора/экипировки. Предыдущий Passed не подтверждает новую копию.

## 9. История изменений

2026-10-12 — 4.1: семь сценариев.

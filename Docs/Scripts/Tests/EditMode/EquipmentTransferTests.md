# EquipmentTransferTests

Исходник: `Assets/Tests/EditMode/EquipmentTransferTests.cs`.

## 1. Название и назначение

Четыре Edit Mode проверки новых переносов/Off-Hand/двуручного ограничения и безопасности старого восьмиэлементного массива.

## 2. Как работает

Setup создаёт временные определения и чистые InventoryState/EquipmentState. Tests вызывают атомарные операции; TearDown уничтожает только созданные тестом SO. Игровые ассеты не сохраняются.

## 3. Методы и функции

`Setup`/`Cleanup` NUnit создают/освобождают Sword/Other/Shield/Filler. `OffHandIsAppendedAndTwoHandedConflictsPreserveAllItems` проверяетGloves7/OffHand8, корректныйshield, отказдвуручного приshield иобратныйотказ. `EquipmentReturnsToExactEmptyCellOrSwapsCompatibleSingleWithoutDuplication` проверяет обмен вoccupied compatible cell иreturn в27, повторнеудваивает. `AccessoryTransferAndSwapAreAtomicAndRejectReentry` проверяет дваaccessory/bonus7/reentryfalse/неверныйGloves. `FullBagAndLegacyEightSlotDataCannotLoseGearOrBreakOffHand` проверяет30filler/full refusal ирасширение старого JSON массива8. Все Test void; Unity callbacks нет.

## 4. Переменные и Inspector

Поля four test ItemData; id transfer.*, categoryWeapon/Armor, OffHand8, filler nonstack. НастроекInspector нет; slot27/29 выбраны для проверкиадресного возврата.

## 5. Связи

EquipmentState/InventoryState/ItemData/JsonUtility/NUnit. Не использует сцены/мышь/игровой SaveSystem.

## 6. Пример

Sworddrag изWeapon наbag1 заменяетсяOther: Sword ровно1 вbag1, Otherнадет. Затем Otherровно1 возвращается вbag27.

## 7. Настройка в Unity

К GameObject не прикрепляется. Test Runner → EditMode → FalseMemory.Battle.EditModeTests → EquipmentTransferTests.

## 8. Возможные проблемы

JSON тестирует только форму старого массива иnull slots; не доказывает сохранениеItemData references междузапусками. Для дисковыхсохранений нужны stableitem IDs иstage9. UI/mouse проверяются отдельнымиPlaytests.

## 9. История изменений

- 2026-10-12 — Player rework5: Off-Hand, атомарные переносы и временные native64 силуэты; дисковые сохранения ещё отсутствуют.

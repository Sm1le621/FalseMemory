# EquipmentTests

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/EquipmentTests.cs.

12 EditMode случаев модели экипировки: восемь слотов и четыре граничных сценария.

## 2. Как работает

Setup создаёт InventoryState/EquipmentState и три временных определения. Cleanup уничтожает только тестовые SO.

## 3. Методы и функции

Player rework5: параметризованныйEachEquipmentSlot... включаетновыйOffHand8 сArmorкатегорией. Прежниеcasesиguardsсохранены, проверкаположительна идля9-гоslot.

Gloves проверяется как Armor, а аксессуары определяются явными Accessory1/2, независимо от положения новых значений enum.

`Setup`/`Cleanup` NUnit lifecycle. `EachEquipmentSlotAcceptsOnlyMatchingCategoryAndReturnsItem(slot)` восемь TestCase: подходящая категория/слот, переход владения и обратный возврат/бонус0. `FullInventorySwapsWeaponButCannotUnequipUntilSpaceIsFree` полная замена/отказ снятия/успех после освобождения. `WrongSlotLevelAndIndicesNeverLoseItemsOrRaiseEvents` неверный уровень/слот/индекс без мутации. `TwoAccessoriesAddOnceAndRepeatedReadsNeverAccumulate` две копии,20 чтений/снятие. `InventoryNotificationCannotReenterEquipmentTransaction` реальный Changed пытается снять во время надевания и получает отказ. Методы void, Unity callbacks нет.

## 4. Переменные и Inspector

Временные sword/staff/filler, бонусы ATK5/MATK4,30 слотов; Inspector отсутствует.

## 5. Связи

FalseMemory.Battle.EditModeTests, NUnit, EquipmentState/InventoryState/ItemData.

## 6. Пример

Полная сумка безопасно меняет оружие, но не теряет надетое при невозможности снятия.

## 7. Настройка в Unity

Не прикреплять, EditMode Test Runner/MCP с assembly_names, проверять итог>0.

## 8. Ограничения и расширение

ДополнительныеEquipmentTransferTests проверяютtwohand/адресныйbag/swap/reentry/legacyarray; этотфайлнепроверяетмышь.

Не проверяет настоящий курсор/боевой урон; UI отдельно PlayMode, формулы4.5.

## 9. История изменений

- 2026-10-12 — Player rework5: девятыйOffHand, bidirectionaldrag/atomictransfers/совместимость/силуэты; sourceIDsсохранены.

2026-10-12 — Gloves и проверка категории при сохранении старых числовых значений.

2026-10-12 — 4.4:11 случаев.

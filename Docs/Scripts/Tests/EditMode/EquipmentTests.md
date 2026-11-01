# EquipmentTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/EquipmentTests.cs`.

Проверки модели экипировки: только Weapon/Chest, совместимость, возврат предметов, полный инвентарь, откат транзакций и отсутствие накопления бонусов.

## 2. Как работает

`SetUp` создаёт `InventoryState`, `EquipmentState` и временные `ItemData` (меч ATK +5, броня DEF +4, filler). `TearDown` уничтожает ScriptableObject. Сцена и реальные ассеты не используются.

## 3. Методы и функции

- `EachEquipmentSlotAcceptsOnlyMatchingItemAndReturnsIt(slot)` — два TestCase (Weapon/Chest): надевание, владение, снятие, обнуление бонусов.
- `WeaponCannotFillChestAndChestCannotFillWeapon()` — несовместимые слоты отклоняются без потери предметов.
- `FullInventorySwapsWeaponButCannotUnequipUntilSpaceIsFree()` — замена в полном инвентаре работает, снятие без места отклоняется, после освобождения — успех.
- `InvalidSlotsAndIndicesNeverLoseItemsOrRaiseEvents()` — неверный слот/индекс не меняет инвентарь и не вызывает событий.
- `WeaponAndChestBonusesStackOnceAndRepeatedReadsNeverAccumulate()` — ATK 5 + DEF 4, 20 чтений не накапливают бонусы.
- `InventoryNotificationCannotReenterEquipmentTransaction()` — повторный вход через `InventoryState.Changed` отклоняется.

## 4. Переменные и Inspector

`inventory`, `equipment`, `sword`, `armor`, `filler` — тестовые объекты. Inspector отсутствует.

## 5. Связи

NUnit, `EquipmentState`, `InventoryState`, `ItemData`, `ItemStatBonus`, `EquipmentSlot`.

## 6. Пример

Меч ATK +5 и броня DEF +4 дают `Bonuses` 5/4; снятие меча оставляет DEF 4, снятие брони обнуляет DEF.

## 7. Настройка в Unity

Компонент не прикрепляется; Test Runner → EditMode.

## 8. Ограничения и проверки

Проверяется модель, не кнопки окна. Поддержаны только два слота и ATK/DEF. 2026-10-31: тесты прошли в составе EditMode 59/59.

## 9. История изменений

- 2026-10-31 — удалены проверки восьми слотов, редкости и уровня; остались Weapon/Chest.
- 2026-10-12 — базовая документация.

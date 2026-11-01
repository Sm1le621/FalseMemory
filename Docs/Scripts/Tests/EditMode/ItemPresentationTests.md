# ItemPresentationTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/ItemPresentationTests.cs`.

Три EditMode-проверки текста и совместимости предметов: только ATK/DEF, экранирование TMP-разметки, эффекты зелий, цены и слоты.

## 2. Как работает

Каждый тест создаёт временный `ItemData` или `BattleConsumable` и уничтожает его в finally. Реальные ассеты и сцена не меняются.

## 3. Методы и функции

- `WeaponAndArmorDescribeOnlyAttackAndDefenseBonuses()` — оружие показывает `Weapon`/`ATK +5` без DEF; броня — `Chest`/`DEF +4` без ATK; `StackLimit` оружия 1.
- `TooltipHidesEmptyFieldsEscapesMarkupAndReportsRealConsumableEffect()` — количество ×3, `Restores up to 30 HP`, экранирование `<size=...>`, отсутствие пустых цен; затем MP 20, Buy 50/Sell 25.
- `ArmorFitsOnlyItsConfiguredChestSlot()` — броня подходит только к Chest, оружие — только к Weapon; `Description(null)` пуст.

## 4. Переменные и Inspector

Временные `ItemData`/`BattleConsumable`; Inspector отсутствует. Проверяются реальные методы `ItemPresentation` и `ItemData.Fits`.

## 5. Связи

NUnit, `ItemPresentation`, `ItemData`, `BattleConsumable`, `EquipmentSlot`, `ItemCategory`.

## 6. Пример

`Description` для Potion ×3 содержит `×3` и `Restores up to 30 HP`, но не содержит `Buy:` при нулевой цене. Для MP-зелья выводится `Restores up to 20 MP`.

## 7. Настройка в Unity

Компонент не прикрепляется; Test Runner → EditMode.

## 8. Ограничения и проверки

Проверяется текст/совместимость, не реальный tooltip и не вёрстка. Редкости и Gloves больше нет. 2026-10-31: тесты прошли в составе EditMode 59/59.

## 9. История изменений

- 2026-10-31 — удалены проверки шести редкостей и Gloves; остались ATK/DEF и два слота.
- 2026-10-12 — базовая документация.

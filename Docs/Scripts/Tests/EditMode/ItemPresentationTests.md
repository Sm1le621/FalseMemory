# ItemPresentationTests

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/ItemPresentationTests.cs.

Восемь Edit Mode случаев оформления и совместимости: шесть редкостей, текст эффектов, стабильные индексы/Gloves.

## 2. Как работает

Каждый тест создаёт временный ItemData либо BattleConsumable и гарантированно уничтожает его finally. Сцену и реальные ассеты не меняет.

## 3. Методы и функции

2026-10-12 — актуальная локализация: Текстовые ожидания приведены к английскому UI (New Game/Main Menu/Back/No items available/Inventory full/DEFEAT/описания HPMP/цен). Проверки состояния, количества, урона, ресурсов и поведения сохранены.

`RarityIsNamedAndColoredWithoutChangingGameplay(rarity)` шесть TestCase: имя редкости/ATK+5/непрозрачный цвет/StackLimit1/неизменный бонус. `TooltipHidesEmptyFieldsEscapesMarkupAndReportsRealConsumableEffect()` void проверяет количество3, HP30, защиту от TMP markup, отсутствие пустых цен/бонусов; затем MP20/цены50/25 без HP. `AppendingGlovesPreservesOriginalSerializedSlotsAndDoesNotAcceptAccessories()` void проверяет5/6, Armor Gloves, отказ Boots/Accessory Gloves и null Description. Unity callbacks нет.

## 4. Переменные и Inspector

Временные SO/rarity, реальные исходники ItemPresentation/ItemData; Inspector отсутствует.

## 5. Связи

NUnit и FalseMemory.Battle.EditModeTests; EquipmentTests отдельно проверяет транзакции восьми слотов.

## 6. Пример

При случайном изменении enum Accessory1 или выводе пустой цены соответствующая проверка падает.

## 7. Настройка в Unity

Не прикрепляется к объекту. Test Runner → EditMode → FalseMemory.Battle.EditModeTests.

## 8. Ограничения и расширение

Не доказывает художественную читаемость цветов и реальный hover; это покрыто другими сценариями/снимками и ручной приёмкой.

## 9. История изменений

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

2026-10-12 — добавлены восемь случаев для переработки инвентаря.

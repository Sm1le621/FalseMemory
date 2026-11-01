# InventoryTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/InventoryTests.cs`.

Восемь EditMode-сценариев модели инвентаря: 30 слотов, нестакуемые предметы, перенос/обмен/слияние, откат отклонённых операций, удаление из стопок и золото.

## 2. Как работает

`SetUp` создаёт `InventoryState` и два `ItemData` с разными `id` и `maxStack = 10`; `TearDown` уничтожает ScriptableObject. Все операции выполняются в памяти.

## 3. Методы и функции

- `ThirtySlotsFillAndRejectWithoutPartialAddition()` — 299 + 2 отклоняется без частичного добавления, затем +1 успешно, полный запас 300.
- `NonStackableAndEquipmentUseOneSlotEach()` — 30 оружий занимают 30 отдельных слотов.
- `MoveAndSwapPreserveQuantities()` — перенос, обмен, слияние, пустые слоты и `from == to`.
- `MergeTransfersOnlyWhatFitsIntoTheTargetStack()` — переносится только свободное место стопки.
- `RejectedTransactionsDoNotRaiseEventsOrMutate()` — `null`, отрицательные количества и неверные индексы не меняют состояние и не вызывают `Changed`.
- `RemovalAcrossStacksIsAtomicAndFreesSlots()` — удаление 16 из 15 отклоняется; 12 удаляется атомарно, слоты освобождаются.
- `ConflictingStableIdIsRejected()` — два разных ассета с одинаковым непустым `id` не сосуществуют.
- `GoldHasOneBalanceAndRejectsUnderflowAndOverflow()` — отрицательное/избыточное золото отклоняется.

## 4. Переменные и Inspector

`item`, `other`, `inventory` — временные данные; лимит стопки 10, ёмкость 30. Inspector отсутствует.

## 5. Связи

NUnit, `InventoryState`, `InventorySlot`, `ItemData`. Окно/экипировка/использование проверяются отдельными тестами.

## 6. Пример

Добавление 15 предметов создаёт две стопки (10 и 5); перенос на слот 8 заполняет его до 10, а остаток остаётся в исходном слоте.

## 7. Настройка в Unity

Компонент не прикрепляется; Test Runner → EditMode.

## 8. Ограничения и проверки

Проверяется модель, не drag & drop и не UI. Операции деления стопки больше нет. 2026-10-31: тесты прошли в составе EditMode 59/59.

## 9. История изменений

- 2026-10-31 — удалены проверки split/редкости; документ сверен с текущим `InventoryState`.
- 2026-10-12 — базовая документация.

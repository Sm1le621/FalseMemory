# InventoryPresentationFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/InventoryPresentationFlowTests.cs`.

Partial `BattleFlowTests`: hover/tooltip, рамка экипировки, границы экрана, счётчики сумки и мировая MP-полоса.

## 2. Как работает

Использует общий `Setup`/`Cleanup` и реальную `SampleScene`. Отправляет MouseState в Input System, работает с Canvas/TMP и существующими компонентами; временные устройства удаляются.

## 3. Методы и функции

- `RealMouseHoverShowsItemAndHidesOnEmptyDragAndClose()` — наведение на Potion показывает `Potion`/`Consumable`/`30 HP`; пустой слот, drag и закрытие скрывают подсказку; количество не меняется.
- `EquippedHoverAndBorderSurviveReplacementAndEmptying()` — надетый Iron Sword показывает `ATK +5`, `border` назначен; снятие скрывает tooltip и возвращает ATK 15.
- `TooltipClampsToAllScreenCornersAndLiveBagResourceCountsMatchModel()` — четыре угла экрана не выходят за границы; `3 / 30`, HP/MP-строка, две кнопки экипировки.
- `WorldManaBarFollowsSharedManaWithoutChangingIt()` — `ManaHUD` показывает 30/30, после трат 18/30, после восстановления 30/30; сам MP не меняет.

## 4. Переменные и Inspector

Синтетическая Mouse, экранные координаты, существующие Potion/Sword/Player. Inspector отсутствует; сцена/ассеты не сохраняются.

## 5. Связи

`BattleFlowTests`, `InventoryWindow`, `InventoryTooltip`, `EquipmentController`, `ManaHUD`, `BattleUnit`, Input System, Canvas/TMP.

## 6. Пример

При наведении на Potion подсказка содержит `Potion`, `Consumable`, `Restores up to 30 HP`; при переносе в другой слот количество остаётся 3, а занятость сумки не меняется.

## 7. Настройка в Unity

Test Runner → PlayMode; требуется `SampleScene` с назначенными `ManaHUD.mpSlider/mpText/unit` и `InventoryTooltip`.

## 8. Ограничения и проверки

Проверяется поведение UI, не художественный вид. Tooltip позиционируется по курсору; ручная оценка удобства — за владельцем. 2026-10-31: PlayMode 36/36.

## 9. История изменений

- 2026-10-31 — удалены проверки редкости; добавлен тест мировой MP-полосы.
- 2026-10-12 — базовая документация.

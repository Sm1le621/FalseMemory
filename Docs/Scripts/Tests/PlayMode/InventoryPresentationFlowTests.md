# InventoryPresentationFlowTests

## Текущее изменение5.3 —2026-10-26

Проверки hover/редкости/закрытия/краёв Tooltip сохранены; после расхода5 из40 шапка должна содержать35 / 40. Остальные численные проверки прежние.

Фактические проверки и ограничения: [ClasslessStage53](../../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/InventoryPresentationFlowTests.cs.

Три Play Mode сценария hover, экипировки, экранных границ и связей нового интерфейса.

## 2. Как работает

Часть существующего BattleFlowTests; общий Setup загружает реальную SampleScene и подключает Input System. Общий Cleanup выгружает тестовые сцены и восстанавливает временные настройки.

## 3. Методы и функции

Player rework5: TooltipClamps... теперьтребуетequipmentButtons.Length9 вместо8, посколькуOffHand8добавленбезсменыстарыхID. Прежниеmousehover/rarity/ресурсы/границысохранены.

`RealMouseHoverShowsItemAndHidesOnEmptyDragAndClose()` IEnumerator создаёт тестовую Mouse, отправляет MouseState в реальные screen-центры ячеек, ждёт EventSystem, проверяет Potion/Common/HP, уход на пустую ячейку, скрытие при drag/close и неизменность3 предметов; finally удаляет только тестовую мышь.

`EquippedHoverAndRarityRemainCorrectAfterReplacementAndEmptying()` IEnumerator надевает Sword, проверяет tooltip Rare/ATK5, icon/рамку; снимает, проверяет скрытие/базу15, удаляет тестово добавленный Sword из сумки и проверяет обновление hover. `TooltipClampsToAllScreenCornersAndLiveBagResourceCountsMatchModel()` IEnumerator показывает tooltip в4 углах, проверяет worldcorners в Screen; деление обновляет занятость1→2, HP/MP читаются реально, портрет/8 кнопок назначены. Не все Show здесь через мышь — первый сценарий отдельно подтверждает настоящий raycast.

## 4. Переменные и Inspector

2026-10-13: TooltipClampsToAllScreenCornersAndLiveBagResourceCountsMatchModel проверяет назначенный InventoryWindow.portraitSprite, либо прежний battleSprite fallback при null. Проверки Tooltip/границ/30 слотов/9 gear slots/HP90/MP25 сохранены; графическая ссылка не копирует ресурсы.

Временная Mouse/экранные координаты, существующие Potion/Sword/Player, публичные UI-ссылки. Inspector отсутствует. Тесты не сохраняют scene/SO.

## 5. Связи

BattleFlowTests lifecycle, InventoryWindow/Tooltip, InputSystem/MouseState, EquipmentController, TMP/Canvas, реальные данные SampleScene.

## 6. Пример

После снятия надетого меча открытая подсказка немедленно исчезает, меч возвращается в сумку, ATK снова15.

## 7. Настройка в Unity

Не прикрепляется. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests. Тестовый EditorInputBehavior позволяет событиям работать без фокуса.

## 8. Ограничения и расширение

Новыйmousegear-transfer проверяетEquipmentDragFlowTests; художественныеiconsиstandaloneвэтомтестенеоцениваются.

Проверка углов выполняется в текущем размере Game View; четыре разных разрешения проверяются отдельно через MCP/измерения/снимки. Художественное впечатление, standalone и физическая мышь владельца требуют ручной проверки.

## 9. История изменений

- 2026-10-12 — Player rework5: девятыйOffHand, bidirectionaldrag/atomictransfers/совместимость/силуэты; sourceIDsсохранены.

2026-10-12 — добавлены три сценария нового RPG-инвентаря, сохраняющие исходные боевые тесты.

- 2026-10-13 — ожидание портрета обновлено под отдельную UI-ссылку; остальные проверки сохранены.

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

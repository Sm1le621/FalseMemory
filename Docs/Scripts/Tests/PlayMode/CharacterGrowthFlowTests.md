# CharacterGrowthFlowTests / BattleFlowTests partial

## Текущее изменение5.3 —2026-10-26

EXP-кривая/несколько level-up/однократность/сохранение в бою прежние. Вместо автоматическогоHP/ATK роста проверяютсяHP100/Attack15 и3 свободных points за уровни2…4; текущие повреждённые ресурсы не восстанавливаются. После расхода10 из40 остаётся30.

Фактические проверки и ограничения: [ClasslessStage53](../../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

## 1. Название и назначение

Исходник: Assets/Tests/PlayMode/CharacterGrowthFlowTests.cs.

Сценарный тест level-up через настоящую награду и сохранение состояния при возврате.

## 2. Как работает

Общий fixture загружает существующие сцены. Участники/InventoryState/EquipmentState сохраняются ссылками до скрытия карты. После победы ждёт полное завершение Transition, затем открывает окно.

## 3. Методы и функции

`VictoryLevelsOnceAndPreservesProgressResourcesEquipmentAndPosition()` готовит90EXP/80HP/22MP/EnemyHP1, выполняет настоящую Attack/Victory→110EXP/lvl2, проверяет новую вместимость/HPHUD/одно начисление/позицию/те же Inventory и Equipment/English Next. `SeveralLevelsRefreshInventoryWithoutStackingOrRestoringResources()` проверяет450→lvl4, сохранение повреждения/MP, сообщение и отсутствие повторного бонуса. UnitySetUp/TearDown у общего BattleFlowTests.

## 4. Переменные и Inspector

Inspector-параметров нет; данные подготовки runtime и не сохраняются в сцену/ассет.

## 5. Связи

CharacterGrowthRules/Unit/Health, Transition/Manager/BattleUI, InventoryWindow/GameText; реальные DefaultCharacterGrowth/StringTable.

## 6. Пример

После победы HP80/105, MP22/32, ATK17, EXP110, Next140. Награда повторно не выдаётся; позиция и исходные state-объекты сохранены.

## 7. Настройка в Unity

Test Runner → Play Mode → FalseMemory.Battle.PlayModeTests; не прикрепляется.

## 8. Возможные проблемы

Первый прогон41/42 открывал Inventory до окончания Transition. Исправлено ожидание EncounterActive/IsTransitioning и проверка результата Open. Дисковое сохранение/классы не проверяются.

## 9. История изменений

2026-10-12 — создан в подэтапе5.1. [Проверки](../../../ProgressionStage5.md).

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

# AbilityLearningFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/AbilityLearningFlowTests.cs`. Интеграция изучения, реального боя, MCR и возврата на карту.

## 2. Как работает

Интеграция изучения, реального боя, MCR и возврата на карту. Расширяет существующие BattleSpell/BattleAbilities, классовой системы нет.

## 3. Методы и функции

LearnedPhysicalAbilityEntersRealBattleMenuPaysManaOnceAndSurvivesWorldReturn проверяет STR requirement, обучение/повтор, MCR99/MP5, меню4 entries, однократную атаку31 и сохранение LP/known после победы. NewlyLearnedHealAndSacredStrikeUseRequirementsAndOnlyDamageUndeadWithHoly проверяет требования GreaterHeal/SacredStrike, лечение60 без урона цели и отдельный Holy≥8. UnityTest возвращаютIEnumerator, используют общие Setup/Cleanup; Play-данные не сохраняются в сцену.

## 4. Переменные и Inspector

Ссылки получаются из настоящего Player.catalog по стабильнымID. ConstantBattleRandom исключает случайный crit/miss. MP/HP тестовые значения относятся только к текущему fixture.

## 5. Связи

BattleUnit предоставляет уровень, эффективные атрибуты и MP; AbilityLearning хранит только LP/known IDs. BattleSpell содержит данные, BattleAbilities выполняет эффект, BattleManager строит меню, GameText локализует. Для tests используются NUnit/Unity Test Framework, для Editor — AssetDatabase и EditorSceneManager.

## 6. Пример

PowerStrike при STR20 наносит31 противDEF5 и платит5 MP приMCR99; двойная команда отклоняется.

## 7. Настройка в Unity

Не прикреплять. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests; требуются сохранённые сцены и AbilityLearningSetup.

## 8. Возможные проблемы

ID должен быть непустым и стабильным. Не расходовать attribute points на обучение. Уже известные Fire/Heal сохраняются; требования применяются при новом изучении. LP3+1/level — тестовая экономика без учителя/NPC. Постоянного save нет. Проверки — [ClasslessStage55](../../../ClasslessStage55.md).

## 9. История изменений

- 2026-10-26 — реализация и первичная документация подэтапа5.5.

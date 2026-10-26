# AbilityLearningTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/AbilityLearningTests.cs`. Контракты требований, совместимости и атомарного изучения.

## 2. Как работает

Контракты требований, совместимости и атомарного изучения. Расширяет существующие BattleSpell/BattleAbilities, классовой системы нет.

## 3. Методы и функции

LearningUsesIndependentPointsRequirementsStableIdsAndRejectsReentry проверяет STR20, списание LP, неизменность attribute points, ID, повтор/Changed-reentry и сохранение известности после снижения STR. LegacyKnownFireIgnoresNewLearningRequirementsAndInsufficientCurrencyCannotLearn проверяет известный Fire и отказ при цене100 LP. Оба NUnit Test возвращаютvoid; finally уничтожает только временные объекты. Runtime callbacks нет.

## 4. Переменные и Inspector

Локальные GameObject/правила/способности — тестовые данные, не сцена и не Inspector. Default запас LP3 проверяется явно.

## 5. Связи

BattleUnit предоставляет уровень, эффективные атрибуты и MP; AbilityLearning хранит только LP/known IDs. BattleSpell содержит данные, BattleAbilities выполняет эффект, BattleManager строит меню, GameText локализует. Для tests используются NUnit/Unity Test Framework, для Editor — AssetDatabase и EditorSceneManager.

## 6. Пример

После единственного успешного изучения LP3→2, повторный callback не списывает второй LP.

## 7. Настройка в Unity

Не прикреплять. Test Runner → EditMode → FalseMemory.Battle.EditModeTests.

## 8. Возможные проблемы

ID должен быть непустым и стабильным. Не расходовать attribute points на обучение. Уже известные Fire/Heal сохраняются; требования применяются при новом изучении. LP3+1/level — тестовая экономика без учителя/NPC. Постоянного save нет. Проверки — [ClasslessStage55](../../../ClasslessStage55.md).

## 9. История изменений

- 2026-10-26 — реализация и первичная документация подэтапа5.5.

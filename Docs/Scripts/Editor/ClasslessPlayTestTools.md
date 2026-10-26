# ClasslessPlayTestTools

## 1. Название и назначение

Исходник: `Assets/Editor/ClasslessPlayTestTools.cs`. Безопасная подготовка текущей Play-сессии к ручной приёмке распределения очков.

## 2. Как работает

Безопасная подготовка текущей Play-сессии к ручной приёмке распределения очков. Использует текущие правила и состояние, не создаёт вторую систему персонажа.

## 3. Методы и функции

GrantTestLevels() — MenuItem FalseMemory/Testing/Grant 60 Levels (Play Mode Only): проверяет CanGrant, вычисляет целевой level=min(current+60,MaximumLevel), получает требуемый totalEXP и вызывает прежний AddExperience только на разницу. Не присваивает points/HP/MP напрямую, не сохраняет сцену или ассеты. CanGrant() — validation MenuItem, разрешает команду только живому Player с classless/growth rules вне encounter в Play. Возврат void/bool, runtime Unity callbacks нет.

## 4. Переменные и Inspector

Константа 60 относится только к тестовому меню; реальные attributePointsPerLevel/LP/EXP остаются в существующих профилях. Параметров Inspector и нового постоянного состояния нет.

## 5. Связи

BattleUnit/CharacterAttributes/AbilityLearning, ItemData/BattleSpell/ArmorScalingData, CharacterAttributeRules/BattleRules/BattleCalculator/BattleDamage. Для тестов NUnit/UnityTest, для меню UnityEditor. Данные кампании/сохранений не создаются.

## 6. Пример

Новая сессия: level1→61,60 attribute points/63 LP, текущее повреждённое HP80/MP32 остаётся 80/32. После остановки Play весь тестовый прогресс сбрасывается.

## 7. Настройка в Unity

Editor-only статический класс не прикрепляется. Открыть SampleScene →Play → верхнее меню FalseMemory →Testing →Grant 60 Levels (Play Mode Only). Не использовать как реальную награду/экономику игры.

## 8. Возможные проблемы

Симуляции зависят от стратегии/fixture-профиля и не заменяют ручную оценку. Тестовое меню работает только в Editor/Play и не сохраняет прогресс. Метрики и ограничения — [ClasslessStage57](../../ClasslessStage57.md). Этап 6 закрыт до приёмки владельца.

## 9. История изменений

- 2026-10-26 — реализация проверок/инструмента ручной приёмки подэтапа 5.7.

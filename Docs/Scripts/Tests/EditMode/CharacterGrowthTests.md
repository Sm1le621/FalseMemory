# CharacterGrowthTests

## 1. Название и назначение

Исходник: Assets/Tests/EditMode/CharacterGrowthTests.cs.

Edit Mode проверяет пороги/границы, несколько уровней, безопасный рост и единую вместимость HP.

## 2. Как работает

Создаёт временные профиль/объекты; уничтожает finally. Реальный общий BattleUnit.AddExperience вызывается на живом Health.

## 3. Методы и функции

`CurveSupportsBoundariesMultipleLevelsAndUnreachableLargeThresholds()` проверяет100/250/450, отрицательные данные, cap/long-пороги и отсутствие понижения. `GrowthIsDerivedAndExperienceNotificationsRejectReentryWithoutHealing()` проверяет450→level4, неизменную базу attack, HP80/115, MP capacity36, событие один раз и отклонение вложенной награды. `ExtremeBonusesAndCapacityNeverOverflowOrResurrect()` проверяет int.Max/отрицательный growth и SetMaximumHealth без воскрешения/с выходным clamp. Unity callbacks нет.

## 4. Переменные и Inspector

Параметров Inspector нет; rules/obj/unit/calls/nested временные данные каждого теста.

## 5. Связи

CharacterGrowthRules, BattleUnit, Health, NUnit. Отдельный сценарный файл проверяет существующие сцены.

## 6. Пример

Повторный AddExperience(1) после450 не добавляет новые ATK/HP поверх уже вычисленного роста.

## 7. Настройка в Unity

Test Runner → Edit Mode → CharacterGrowthTests; не прикрепляется.

## 8. Возможные проблемы

Не оценивает баланс/локализацию UI/переходы сцен; это Play Mode и Game View.

## 9. История изменений

2026-10-12 — создан в подэтапе5.1. [Проверки](../../../ProgressionStage5.md).

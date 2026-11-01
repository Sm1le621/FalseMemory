# BattleFleeTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/BattleFleeTests.cs`.

Проверка формулы вероятности побега и защиты от некорректных настроек `BattleRules`.

## 2. Как работает

Каждый тест создаёт временные `BattleRules` в try/finally и вызывает `BattleCalculator.FleeChance`. Значения сравниваются с допуском 0.0001.

## 3. Методы и функции

- `SpeedAffectsEscapeWithReasonableLimits(playerSpeed, enemySpeed, expected)` — четыре TestCase: (10,7)→0.74, (7,7)→0.65, (0,1000)→0.05, (1000,0)→0.95.
- `InvalidConfiguredBoundsCannotProduceProbabilitiesOutsideZeroToOne()` — база/границы -1…2 и 5/-5 всё равно дают 0…1.

## 4. Переменные и Inspector

Параметры задаются атрибутами `TestCase`; правила создаются в памяти. Inspector отсутствует.

## 5. Связи

NUnit, `BattleCalculator.FleeChance`, `BattleRules`. Возврат после побега проверяется PlayMode-тестами.

## 6. Пример

При равной скорости 7/7 шанс побега 65%; при огромном превосходстве врага он ограничивается 5%.

## 7. Настройка в Unity

Компонент не прикрепляется; Test Runner → EditMode.

## 8. Ограничения и проверки

Проверяется только формула, не фактический побег и не защита после него. 2026-10-31: тесты прошли в составе EditMode 59/59.

## 9. История изменений

- 2026-10-31 — документ сверен с текущим кодом.
- 2026-10-11 — базовая документация.

# BattleRandomTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/BattleRandomTests.cs`.

Проверки крита, промаха, границ вероятности и воспроизводимости seeded-генератора.

## 2. Как работает

Тесты создают `BattleRules` в try/finally и используют `FixedRandom` с заданным `sample`. `SeededBattleRandom(42)` сравнивается с таким же экземпляром.

## 3. Методы и функции

- `CriticalMultipliesDamageAndDefendAppliesAfterIt()` — обычный удар 10, крит 15, крит при Defend 8.
- `MissDoesZeroDamageAndNeverRollsCritical()` — промах даёт 0, `Missed = true`, `Critical = false`, random вызывается один раз.
- `ChanceBoundsIncludeGuaranteedSuccessAndFailure(chance, sample, expected)` — четыре крайних TestCase, включая chance 1/sample 1.
- `SameSeedProducesSameStream()` — 20 значений seed 42 совпадают.

## 4. Переменные и Inspector

`FixedRandom.sample`/`calls` — тестовые параметры; seed 42 используется только в тесте. Inspector отсутствует.

## 5. Связи

NUnit, `BattleCalculator.ResolvePhysical`, `ChanceSucceeds`, `SeededBattleRandom`, `IBattleRandom`, `BattleRules`.

## 6. Пример

При `criticalChance = 1`, `missChance = 0` и sample 0.5 удар 15/5 даёт крит 15; при Defend — 8.

## 7. Настройка в Unity

Компонент не прикрепляется; Test Runner → EditMode.

## 8. Ограничения и проверки

Проверяется математика и генератор, не боевой UI. 2026-10-31: тесты прошли в составе EditMode 59/59.

## 9. История изменений

- 2026-10-31 — документ сверен с текущим кодом.
- 2026-10-11 — базовая документация.

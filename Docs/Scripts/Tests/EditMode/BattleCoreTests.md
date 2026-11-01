# BattleCoreTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/BattleCoreTests.cs`.

EditMode-проверки базового расчёта урона, очереди по скорости, защиты и одноразового gate команд. Не зависит от сцен и UI.

## 2. Как работает

`SetUp` создаёт `BattleRules`, `TearDown` уничтожает их. Для очереди используется вложенный `Participant` с `Speed`/`IsAlive`. В тесте защиты создаётся реальный GameObject с `BattleUnit` и `Health`.

## 3. Методы и функции

- `PhysicalDamageIsNonNegativeAndDefenseIsApplied(...)` — шесть TestCase, включая отрицательные значения и минимальный урон 1.
- `DamageMultipliersAreConfigurable()` — изменённые множители дают 26.
- `RoundUsesSpeedAndStableTiesWithOneTurnPerUnit()` — порядок по скорости, стабильные равные значения, дубликаты пропускаются.
- `KilledUnitsAreSkippedAndSpeedReordersOnlyAtNextRound()` — мёртвый пропускается, новая скорость действует со следующего раунда.
- `DuplicateCommandsAreRejectedUntilNextTurn()` — gate принимает команду один раз.
- `DefendPersistsUntilOwnTurnAndHealthIsShared()` — `BattleUnit.Health` — тот же `Health`, Defend уменьшает урон и снимается в `BeginTurn`.

## 4. Переменные и Inspector

`rules` — временные правила; `Participant` — тестовая реализация `IBattleParticipant`. Inspector отсутствует.

## 5. Связи

NUnit, `BattleCalculator`, `BattleTurnOrder`, `BattleCommandGate`, `BattleUnit`, `Health`, `BattleRules`.

## 6. Пример

Атака 12 против защиты 8 при Defend даёт 2 урона; после `BeginTurn` защита снимается. Очередь [a(7), b(10), c(10), b] выдаёт b, c, a.

## 7. Настройка в Unity

Компонент не прикрепляется; Test Runner → EditMode. Сцена не требуется.

## 8. Ограничения и проверки

Проверяется чистая логика, не визуал и не ввод. Формулы побега/крита проверяются в отдельных файлах. 2026-10-31: тесты прошли в составе EditMode 59/59.

## 9. История изменений

- 2026-10-31 — документ сверен с кодом; механики EXP в файле нет.
- 2026-10-11 — базовая документация.

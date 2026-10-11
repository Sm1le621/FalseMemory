# BattleCoreTests

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/BattleCoreTests.cs.

Edit Mode проверки базового урона, очереди, защиты и запрета двойной команды.

## 2. Как работает

Test Runner вызывает Setup, отдельную проверку и Cleanup. Временные правила и GameObject уничтожаются. Игрового компонента нет.

## 3. Методы и функции

- `Setup()` [SetUp] создаёт BattleRules; `Cleanup()` [TearDown] DestroyImmediate правил; void.
- `PhysicalDamageIsNonNegativeAndDefenseIsApplied(int attack,int defense,bool guarding,int expected)` — шесть наборов TestCase сверяют расчёт с expected.
- `DamageMultipliersAreConfigurable()` — проверяет изменённые множители, результат 26.
- `RoundUsesSpeedAndStableTiesWithOneTurnPerUnit()` — проверяет Speed, равные скорости и дубликаты.
- `KilledUnitsAreSkippedAndSpeedReordersOnlyAtNextRound()` — смерть/пересортировка нового раунда.
- `DuplicateCommandsAreRejectedUntilNextTurn()` — gate принимает один раз.
- `DefendPersistsUntilOwnTurnAndHealthIsShared()` — реальный BattleUnit/Health, урон 2 и сброс защиты. Все проверки void; Assert сообщает провал.
- Вложенный Participant предоставляет Speed/IsAlive очереди; Unity callbacks нет.

## 4. Переменные и параметры Inspector

Inspector нет. rules — временные настройки; входные TestCase задают крайние/обычные значения. Participant — искусственный участник, не GameObject сцены.

## 5. Связи с другими системами

NUnit, BattleCalculator, BattleTurnOrder, CommandGate, BattleUnit, Health; assembly FalseMemory.Battle.EditModeTests.

## 6. Пример работы

ATK12 DEF8 guarding=true → урон 2; тест требует ровно 2.

## 7. Возможные проблемы и проверки

Чистые расчёты не доказывают поведение всего UI. 11 случаев этого файла входят в 40 Edit Mode текущего успешного запуска. Запуск: Test Runner → EditMode → Run All.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

# BattleFlowTests

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/BattleFlowTests.cs.

Общая Play Mode fixture и четыре исходных сценария боя на настоящих сценах.

## 2. Как работает

Test Runner загружает SampleScene дополнительно, ускоряет паузы и делает исходы детерминированными. Cleanup выгружает сцены и восстанавливает Input System/правила. Partial-класс содержит ещё 13 тестов в BattleStage2FlowTests.

## 3. Методы и функции

- `Setup()` [UnitySetUp] → IEnumerator: сохраняет настройки ввода/правил, включает синтетический ввод без фокуса, загружает мир, задаёт задержку 0.1/фейд 0/Defend0 и crit/miss0 на тестовых участниках.
- `Cleanup()` [UnityTearDown] → IEnumerator: восстанавливает правила/ввод, возвращает предыдущую сцену либо временный anchor, выгружает мир/бой.
- `WaitUntil(Func<bool> predicate)` → IEnumerator ожидания; через 15 секунд Assert timeout.
- `Player()` / `Enemy()` → BattleUnit по имени GameObject; `Manager()` → найденный BattleManager.
- `ContactStartsOneBattleThenVictoryPreservesWorldAndHealth()` — реальная физика запускает бой один раз, Attack/Defend, победа и прежний Health/позиция; синтетический D проверяет движение после возврата.
- `DefeatStopsTurnsAndOffersRestartAndMenu()` — HP1, поражение без следующих атак, новая игра.
- `HeldEnterConsumesOneActionAndDoesNotAttackOnNextTurn()` — Enter удерживается, второго хода нет.
- `DefeatCanReturnToMainMenuAndStartNewGame()` — поражение→меню→OnStart и свежие HP. Тестовые методы возвращают IEnumerator, а не игровое значение.

## 4. Переменные и параметры Inspector

Inspector нет; previousScene/rules/previousDelay/previousFade/previousDefendChance и сохранённые режимы InputSettings — состояние fixture. Штатные ассеты не сохраняются тестом.

## 5. Связи с другими системами

NUnit/Unity Test Framework/Input System, реальные SampleScene/BattleScene/MainMenu. Зависит от имён Player/Enemy и сцен сборки.

## 6. Пример работы

Collider2D контакт → ожидается EncounterActive → Attack → Enemy50 HP, Player96 HP → Defend → Player94 HP.

## 7. Возможные проблемы и проверки

Запуск в Editor, не отдельной сборке. Временные настройки восстанавливаются; таймаут может означать сломанные ссылки/сцену или проблемы ввода. Все 17 Play Mode текущего запуска прошли. Запуск: Test Runner → PlayMode → Run All.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

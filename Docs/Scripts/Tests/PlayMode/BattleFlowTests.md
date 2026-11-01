# BattleFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/BattleFlowTests.cs`.

Базовая PlayMode-fixture боя на реальных сценах `SampleScene`/`BattleScene` и четыре сценария: вход по контакту, победа, поражение и удержание Enter. Остальные сценарии находятся в partial-файлах `BattleStage2FlowTests`, `BattlePresentationFlowTests`, `InventoryFlowTests`, `InventoryPresentationFlowTests` и `PixelVisualFlowTests`.

## 2. Как работает

`[UnitySetUp] Setup` сохраняет настройки Input System и правила, загружает `SampleScene` аддитивно, делает её активной, ускоряет `actionDelay` до 0.1, отключает fade и случайные криты/промахи. `[UnityTearDown] Cleanup` восстанавливает правила/ввод, выгружает бой и мир, при необходимости создаёт якорную сцену. `WaitUntil` ждёт условие не дольше 15 секунд.

## 3. Методы и функции

- `Setup()` / `Cleanup()` — PlayMode lifecycle.
- `WaitUntil(Func<bool>)` → IEnumerator; ожидание с таймаутом-assert.
- `Player()`, `Enemy()`, `Manager()` — поиск реальных объектов сцены.
- `ContactStartsOneBattleThenVictoryPreservesWorldAndHealth()` — физический контакт запускает бой один раз; Attack/Defend, победа, прежние HP/позиция, выгрузка боя и возобновление движения.
- `DefeatStopsTurnsAndOffersRestartAndMenu()` — HP 1, поражение, блокировка действий, New Game/Main Menu, сброс сцены.
- `HeldEnterConsumesOneActionAndDoesNotAttackOnNextTurn()` — удержание Enter не даёт второго хода.
- `DefeatCanReturnToMainMenuAndStartNewGame()` — поражение → меню → `OnStart` → свежие HP 100.

## 4. Переменные и Inspector

`previousScene`, `rules`, `previousDelay`, `previousFade`, `previousDefendChance`, сохранённые настройки Input System. Inspector отсутствует; тесты меняют только runtime-копии правил и восстанавливают их.

## 5. Связи

NUnit, Unity Test Framework, Input System, `BattleTransition`, `BattleManager`, `BattleUI`, `PlayerController`, `Health`, `MainMenuController`. Реальные сцены и объекты не сохраняются.

## 6. Пример

Игрок ставится в позицию врага; физика запускает встречу. После победы `ReturnAfterVictory` выгружает `BattleScene`, Enemy выключается, Player остаётся на прежней позиции с тем же HP.

## 7. Настройка в Unity

Запуск через Test Runner → PlayMode. Сцены `SampleScene`, `BattleScene`, `MainMenu` должны быть доступны; тесты сами управляют загрузкой.

## 8. Ограничения и проверки

Проверяется поведение в PlayMode, но не художественный вид и не standalone-сборка. Синтетический ввод требует настроек Input System, которые восстанавливаются. 2026-10-31: PlayMode 36/36, 0 failed.

## 9. История изменений

- 2026-10-31 — удалены проверки EXP/Items/Heal; ожидания HP/MP приведены к текущей модели.
- 2026-10-11 — базовая документация.

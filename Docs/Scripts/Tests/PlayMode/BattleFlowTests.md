# BattleFlowTests

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/BattleFlowTests.cs.

Общая Play Mode fixture и четыре исходных сценария боя на настоящих сценах.

## 2. Как работает

Setup временно сохраняет Application.runInBackground и включает его только на время сценария, чтобы потеря фокуса редактора не останавливала обработку физики/UI. Cleanup восстанавливает исходный bool вместе с Input System. Runtime проект/Player Settings этим тестовым решением не сохраняются.

Test Runner загружает SampleScene дополнительно, ускоряет паузы и делает исходы детерминированными. Cleanup выгружает сцены и восстанавливает Input System/правила. Partial-класс содержит ещё 13 тестов в BattleStage2FlowTests.

## 3. Методы и функции

2026-10-12 — актуальная локализация: Текстовые ожидания приведены к английскому UI (New Game/Main Menu/Back/No items available/Inventory full/DEFEAT/описания HPMP/цен). Проверки состояния, количества, урона, ресурсов и поведения сохранены.

- `Setup()` [UnitySetUp] → IEnumerator: сохраняет настройки ввода/правил, включает синтетический ввод без фокуса, загружает мир, задаёт задержку 0.1/фейд 0/Defend0 и crit/miss0 на тестовых участниках.
- `Cleanup()` [UnityTearDown] → IEnumerator: восстанавливает правила/ввод, возвращает предыдущую сцену либо временный anchor, выгружает мир/бой.
- `WaitUntil(Func<bool> predicate)` → IEnumerator ожидания; через 15 секунд Assert timeout.
- `Player()` / `Enemy()` → BattleUnit по имени GameObject; `Manager()` → найденный BattleManager.
- `ContactStartsOneBattleThenVictoryPreservesWorldAndHealth()` — реальная физика запускает бой один раз, Attack/Defend, победа и прежний Health/позиция; синтетический D проверяет движение после возврата.
- `DefeatStopsTurnsAndOffersRestartAndMenu()` — HP1, поражение без следующих атак, новая игра.
- `HeldEnterConsumesOneActionAndDoesNotAttackOnNextTurn()` — Enter удерживается, второго хода нет.
- `DefeatCanReturnToMainMenuAndStartNewGame()` — поражение→меню→OnStart и свежие HP. Тестовые методы возвращают IEnumerator, а не игровое значение.

## 4. Переменные и параметры Inspector

savedRunInBackground — private bool test fixture, исходный runtime background flag, не Inspector. Нужен только для воспроизводимого запуска без активного Game View.

Inspector нет; previousScene/rules/previousDelay/previousFade/previousDefendChance и сохранённые режимы InputSettings — состояние fixture. Штатные ассеты не сохраняются тестом.

## 5. Связи с другими системами

NUnit/Unity Test Framework/Input System, реальные SampleScene/BattleScene/MainMenu. Зависит от имён Player/Enemy и сцен сборки.

## 6. Пример работы

Collider2D контакт → ожидается EncounterActive → Attack → Enemy50 HP, Player96 HP → Defend → Player94 HP.

## 7. Настройка в Unity

Это тестовый файл, не компонент сцены: не прикрепляй его к GameObject. Открой Window → General → Test Runner, выбери PlayMode и нужную сборку/тест либо Run All. Ссылки и временные объекты создаёт fixture. Для PlayMode должны быть доступны существующие SampleScene, BattleScene и MainMenu; тест не требует ручного изменения их Inspector.

## 8. Возможные проблемы и проверки

Запуск в Editor, не отдельной сборке. Временные настройки восстанавливаются; таймаут может означать сломанные ссылки/сцену или проблемы ввода. Все 17 Play Mode текущего запуска прошли. Запуск: Test Runner → PlayMode → Run All.

## 9. История изменений

- 2026-10-13 — background execution включается временно внутри test fixture и восстанавливается; устранение зависимости сценариев UI от фокуса, gameplay код не менялся.

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

- 2026-10-11 — 3.7: устаревший FindFirstObjectByType заменён FindAnyObjectByType для единственных Manager/MainMenu; сценарии не менялись.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-11 — изменения этапа 2 сохранены в локальном Git после повторной проверки: 40 тестов Edit Mode и 17 тестов Play Mode прошли. Поведение этого файла описано выше; в задаче главного промпта его код повторно не переписывался.

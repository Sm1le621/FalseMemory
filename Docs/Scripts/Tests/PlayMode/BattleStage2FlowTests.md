# BattleStage2FlowTests

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/BattleStage2FlowTests.cs.

13 интеграционных Play Mode сценариев этапа 2 в partial BattleFlowTests.

## 2. Как работает

Использует Setup/Cleanup предыдущего файла; BeginBattle запускает встречу и ждёт меню. Часть ввода идёт через Input System, кнопки проверяются прямыми onClick.Invoke. Данные в этих сценариях временные.

## 3. Методы и функции

- `Fire(BattleUnit player)` / `Heal(BattleUnit player)` → первое заклинание нужного эффекта; `Potion(BattleUnit player)` → первое определение предмета.
- `ConstantBattleRandom(float value)` сохраняет выборку, `Next01()` возвращает её.
- `BeginBattle(BattleUnit player,BattleUnit enemy)` → IEnumerator старта и ожидания готового UI.
- `FailedFleeConsumesOneTurnAndDoesNotSpendResources()` — неудача, враг действует, ресурсов не списывает.
- `SuccessfulFleePreservesResourcesAndEnemyThenProtectsAndAllowsAnotherBattle()` — побег, прежние HP/MP/Items/EXP, живой враг, свободная позиция, защита/повторный бой.
- `VictoryAwardsExperienceOnceAndPreservesAllSessionState()` — полный Fire/Potion/Attack, итог 88 HP/6 MP/2 Potion/25 EXP/level1, результаты, HUD, выключенный враг, защита награды.
- `TargetDeathBeforeQueuedSpellCancelsManaSpendAndFurtherActions()` — погибшая до применения цель отменяет расход MP.
- `HoldingEnterAcrossMagicMenuDoesNotCastUntilASeparatePress()` — вход в Magic удержанием не кастует Fire.
- `ExpandedSpellListScrollsAndBackDoesNotSpendResources()` —6 временных SO+Назад, скролл/возврат без хода.
- `DeathWhileSelectingCancelsPendingTurnAndRejectsActions()` — смерть ожидающего игрока, Game Over, команды отклонены.
- `CriticalAndMissHaveCorrectDamageMessagesAndEachSpendOneTurn()` —15 крит,0 промах, сообщения и очередность.
- `PotionConsumesOneItemAndOneTurnDespiteDoubleSubmission()` — двойной onClick, один предмет/ход, зелёное +30.
- `EmptyItemsAndCancelDoNotConsumeTurn()` — ноль Potion, блокировка/возврат.
- `FireConsumesMPOnceAndReturnsWithSameHealthAndMana()` — двойная команда/неприменимость crit-miss к Fire/MP при возврате.
- `HealCapsHPAndInsufficientManaDoesNotConsumeTurn()` — зелёное +5, лимит/недостаток MP.
- `MagicEscapeAndDisabledSpellDoNotSpendTurnOrDoubleUseMana()` — Escape через клавиатуру, недоступный Fire, двойной Heal. Все 13 проверок возвращают IEnumerator; Assert фиксирует результат.

## 4. Переменные и параметры Inspector

Inspector нет; value фиксированного источника 0/0.5/1, HP/MP/стопки меняются только у тестовых объектов. extraSpells уничтожаются в finally.

## 5. Связи с другими системами

Общая fixture BattleFlowTests, реальные игровые компоненты/сцены, NUnit и Input System. Имя класса в Test Runner — BattleFlowTests, не имя файла.

## 6. Пример работы

Полный сценарий с исходными 80 HP/5 EXP → победа 88 HP/6 MP/2 Potion/25 EXP → мир с теми же значениями.

## 7. Настройка в Unity

Это тестовый файл, не компонент сцены: не прикрепляй его к GameObject. Открой Window → General → Test Runner, выбери PlayMode и нужную сборку/тест либо Run All. Ссылки и временные объекты создаёт fixture. Для PlayMode должны быть доступны существующие SampleScene, BattleScene и MainMenu; тест не требует ручного изменения их Inspector.

## 8. Возможные проблемы и проверки

Прямой onClick проверяет обработчик, не raycast реального курсора. Частичные тесты могут уменьшать HP врага ради короткого завершения, полный сценарий отдельно проходит весь бой. Все 13 входят в 17 успешно выполненных Play Mode; новых тестов в задаче документации не добавлялось.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-11 — изменения этапа 2 сохранены в локальном Git после повторной проверки: 40 тестов Edit Mode и 17 тестов Play Mode прошли. Поведение этого файла описано выше; в задаче главного промпта его код повторно не переписывался.

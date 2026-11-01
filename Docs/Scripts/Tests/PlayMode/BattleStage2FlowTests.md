# BattleStage2FlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/BattleStage2FlowTests.cs`.

Partial `BattleFlowTests`: интеграционные сценарии побега, победы, магии, ввода и смерти. Также содержит helper `BeginBattle` и поиск ассетов Fire/Potion/Sword/Armor.

## 2. Как работает

Использует общий `Setup`/`Cleanup`. `BeginBattle` вызывает `BattleTransition.TryBegin` и ждёт `AwaitingPlayer`. `ConstantBattleRandom` фиксирует случайные исходы. Все изменения — runtime, сцены/ассеты не сохраняются.

## 3. Методы и функции

- `Fire`, `Potion`, `Sword`, `Armor` — поиск первых подходящих определений в инвентаре/заклинаниях.
- `FailedFleeConsumesOneTurnAndDoesNotSpendResources()` — неудачный побег тратит ход, но не HP/MP/предметы.
- `SuccessfulFleePreservesResourcesAndEnemyThenProtectsAndAllowsAnotherBattle()` — побег сохраняет HP/MP/предметы, враг жив, игрок в безопасной точке, защита блокирует повторный вход.
- `VictoryPreservesAllSessionStateAndShowsResults()` — победа не содержит EXP, сохраняет MP 14/предметы/HP, выключает врага.
- `TargetDeathBeforeQueuedSpellCancelsManaSpendAndFurtherActions()` — смерть цели до применения отменяет расход MP.
- `HoldingEnterAcrossMagicMenuDoesNotCastUntilASeparatePress()` — удержание Enter не кастует Fire.
- `ExpandedSpellListScrollsAndBackDoesNotSpendResources()` — 6 временных заклинаний + Back, прокрутка без хода.
- `DeathWhileSelectingCancelsPendingTurnAndRejectsActions()` — смерть в меню блокирует действия и не тратит ресурсы.
- `CriticalAndMissHaveCorrectDamageMessagesAndEachSpendOneTurn()` — крит 15 и MISS 0, сообщения и один ход на действие.
- `FireConsumesMPOnceAndReturnsWithSameHealthAndMana()` — Fire 8 MP, возврат на карту с теми же HP/MP.
- `InsufficientManaDoesNotConsumeTurnOrSpendMP()` — при 6 MP Fire недоступен, ход не тратится.
- `MagicEscapeAndDisabledSpellDoNotSpendTurnOrDoubleUseMana()` — Escape возвращает меню, недоступный Fire не применяется.

## 4. Переменные и Inspector

`ConstantBattleRandom`, `originalSpells`, временные `extraSpells`, локальные HP/MP. Inspector отсутствует; временные SO уничтожаются.

## 5. Связи

`BattleFlowTests`, `BattleManager`, `BattleTransition`, `BattleUnit`, `BattleAbilities`, `BattleUI`, `BattleRules`, Input System.

## 6. Пример

Победа: Fire ×2 (MP 30→14), затем атаки до HP 0. Результат содержит VICTORY без EXP; возврат сохраняет 14 MP и 3 Potion.

## 7. Настройка в Unity

Test Runner → PlayMode; реальные `SampleScene`/`BattleScene`. Без `BattleTransition` сценарии не запустятся.

## 8. Ограничения и проверки

Проверяет боевые сценарии, не баланс и не визуал. 2026-10-31: PlayMode 36/36.

## 9. История изменений

- 2026-10-31 — удалены EXP/Heal/Items-сценарии; ожидания приведены к MP 22/6, HP 96/94/92.
- 2026-10-12 — базовая документация.

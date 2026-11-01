# BattlePresentationFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/BattlePresentationFlowTests.cs`.

Partial `BattleFlowTests`: проверяет визуальную постановку, звук, VFX и HUD поверх настоящего боя, не меняя ресурсы.

## 2. Как работает

Использует общий `Setup`/`Cleanup` и `BeginBattle` из `BattleStage2FlowTests`. Работает с реальными `BattleScene`, компонентами `BattlePresentation`, `BattleVFXController`, `BattleAudioController` и `BattleHUD`.

## 3. Методы и функции

- `SpaceSpeedsPresentationWithoutChoosingOrDuplicatingActions()` — Space даёт `VisualDelta ×3`, не выбирает команду; MP/инвентарь/Time.timeScale не меняются.
- `MissShowsTextWithoutHitReactionOrDamage()` — MISS без glow/отдачи, HP врага неизменен; успешный ответ врага даёт одно попадание.
- `AssignedClipUsesMixerAndRespectsExistingVolume()` — временный клип идёт через `PlayOneShot`, Mixer и `AudioListener.volume`.
- `PresentationEntranceAndOutcomesKeepTransitionLockedAndShowResults()` — повторные `TryBegin` отклоняются, при поражении показывается DEFEAT и блокируются действия.
- `MissingAudioClipsAreSafeAndListenerVolumeIsRespected()` — все звуковые события без клипов возвращают false и не ломают бой.
- `MagicVFXFinishesAndMissingVFXNeverBlocksResourceRules()` — Fire запускает один эффект, частицы очищаются; без VFX Defend работает.
- `TargetDeathDuringWindupCancelsDamageAndManaSpend()` — смерть цели во время замаха отменяет урон и расход MP.
- `MissingPresentationDoesNotBlockOrDuplicatePhysicalAction()` — без `BattlePresentation` проходит один удар и один ход.
- `HUDAnimatesReadOnlyBarsWithoutChangingResources()` — HP/MP-полосы неинтерактивны, анимируются, `LOW HP` и `HP 20 / 100` корректны.

## 4. Переменные и Inspector

Используются runtime-настройки `attackDuration`, `allowFastForward`, временный `BattleAudioProfile`/клип. Inspector отсутствует; исходные ассеты не сохраняются.

## 5. Связи

`BattleFlowTests`, `BattlePresentation`, `BattleVFXController`, `BattleAudioController`, `BattleHUD`, `BattleUI`, `BattleManager`, Input System.

## 6. Пример

Fire по скелету: MP 30→22, эффект проигрывается один раз, частицы исчезают, затем враг отвечает. При отключённом VFX урон и MP остаются прежними.

## 7. Настройка в Unity

Test Runner → PlayMode; нужны реальные `BattleScene` и `SampleScene`. Звуковые тесты используют временные клипы и не требуют назначенных художественных звуков.

## 8. Ограничения и проверки

Не подтверждает слышимость/красоту клипов и эффектов. Проверяет отсутствие побочного изменения ресурсов. 2026-10-31: PlayMode 36/36.

## 9. История изменений

- 2026-10-31 — удалены Heal/EXP-ожидания; добавлены проверки MP-полосы и статичных кадров.
- 2026-10-12 — базовая документация.

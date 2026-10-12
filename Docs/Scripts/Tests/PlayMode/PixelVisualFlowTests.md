# PixelVisualFlowTests / BattleFlowTests partial

## 1. Название и назначение

Исходник: Assets/Tests/PlayMode/PixelVisualFlowTests.cs.

Сценарные Play Mode проверки кадров скелета и pixel VFX при настоящем боевом применении.

## 2. Как работает

Использует общий Setup/Cleanup BattleFlowTests, реальные SampleScene/BattleScene, сохранённые ссылки на участников. Карту Transition скрывает, поэтому поиск Player после входа заменён использованием заранее сохранённой ссылки.

## 3. Методы и функции

`WorldSkeletonMovesAndBattleUsesAttackHurtAndDeathFrames()` проверяет Move/масштаб/два коллайдера, физическую Attack→Hurt/Attack, единственный урон, последний Death-кадр и завершение. `AnimatedFireHealAndDisabledVfxDoNotDuplicateEffectsOrResources()` проверяет повторную команду, Fire19/8MP, Heal/6MP, отсутствие прямоугольного magic glow, очистку при disable и продолжение хода. Unity lifecycle тестов задаёт общий fixture.

WorldSkeletonMovesAndBattleUsesAttackHurtAndDeathFrames теперь проверяет текущий SkeletonDetailed/Point/боевой холст384 вместо жёсткого64. Остальные assertions сохранены: мировое Move, attack/hurt/death, HP50/96, конечная Death[3]/alpha0 и отсутствие зависания. Старые PNG64 отдельно защищены Edit Mode PixelArtTests.

## 4. Переменные и Inspector

Параметров Inspector нет; attackDuration0.8 изменяется только на runtime-компоненте теста для наблюдения кадров. Scene-профили/правила восстанавливаются общим Cleanup.

## 5. Связи

BattleFlowTests helpers, BattleManager/Presentation/VFXController, EnemySpriteAnimation, SpriteAnimationSet, Health/MP.

## 6. Пример

После Fire targetHP41 и MP22; повторная команда отклонена. Отключение VFX при Heal скрывает частицы/кадр, но не блокирует бой.

## 7. Настройка в Unity

Test Runner → Play Mode → FalseMemory.Battle.PlayModeTests. К GameObject не прикрепляется.

## 8. Возможные проблемы

Первый прогон39/40 выявил ошибку поиска скрытого Player в новом тесте; исправлена ссылка, игровая система не переписывалась. Оценка красоты, standalone и четырёх направлений не входят в сценарии.

## 9. История изменений

2026-10-12 — создан в задаче native 64px Skeleton/Fire/Heal/Hit и интеграции кадров; текущие проверки — [PixelVisuals](../../../PixelVisuals.md).

- 2026-10-13 — сценарий использует подробный скелет переменного разрешения без ослабления проверки боевого урона/VFX/смерти.

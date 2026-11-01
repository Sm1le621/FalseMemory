# PixelVisualFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/PixelVisualFlowTests.cs`.

Partial `BattleFlowTests`: кадры скелета в мире/бою и пиксельный Fire VFX при настоящем боевом применении.

## 2. Как работает

Использует общий `Setup`/`Cleanup`, реальные `SampleScene`/`BattleScene` и сохранённые ссылки на Enemy/Player. Проверяет `SpriteRenderer`, `BattleUI.enemyPortrait`, `SpriteAnimationSet` и `BattleVFXController`.

## 3. Методы и функции

- `WorldSkeletonMovesAndBattleUsesAttackHurtAndDeathFrames()` — Move при смещении, масштаб (2,2,1), два коллайдера, Attack→Hurt→Death, последний кадр смерти и alpha 0.
- `AnimatedFireAndDisabledVfxDoNotDuplicateEffectsOrResources()` — повторная команда отклоняется, Fire 19/8 MP, кадры 0…7, частицы очищаются, отключённый VFX не блокирует Defend.

## 4. Переменные и Inspector

`attackDuration = 0.8` задаётся только на runtime-компоненте. Inspector отсутствует; сцена/профили не сохраняются.

## 5. Связи

`BattleFlowTests`, `EnemySpriteAnimation`, `SpriteAnimationSet`, `BattlePresentation`, `BattleVFXController`, `BattleUI`, `Health`/`BattleUnit`.

## 6. Пример

Скелет смещается вправо → кадр Move. В бою после удара показывается Hurt, затем Attack врага. Fire проигрывает 8 кадров, MP 30→22, HP врага 60→41.

## 7. Настройка в Unity

Test Runner → PlayMode; нужны реальные `Skeleton64.asset`, `FireVisual.asset` и настроенные портреты/VFX в `BattleScene`.

## 8. Ограничения и проверки

Проверяет наличие/смену кадров и отсутствие побочных эффектов, не художественное качество. 2026-10-31: PlayMode 36/36.

## 9. История изменений

- 2026-10-31 — удалены проверки Heal/Idle; Fire остаётся единственным боевым VFX.
- 2026-10-12 — базовая документация.

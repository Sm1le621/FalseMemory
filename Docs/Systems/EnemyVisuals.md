# Визуал врагов

## 1. Назначение

Внешний вид скелета в мире и бою: нативные PNG 64×64, кадры движения/атаки/урона/смерти и отражение по направлению. IDLE-кадров нет.

## 2. Общая логика

`EnemySpriteAnimation` читает позицию и `Health`, выбирает `Move` при движении, `Hurt` после урона, `Death` при HP 0 и `battleSprite` в покое. `BattlePresentation` проигрывает те же кадры на портрете. Визуальные компоненты не меняют AI, физику и боевые ресурсы.

## 3. Основные компоненты

- `SpriteAnimationSet` (`Skeleton64.asset`) — Move 4, Attack 3, Hurt 2, Death 4.
- `EnemySpriteAnimation` — мир.
- `BattlePresentation` — бой.
- `PixelBattleAssetBuilder` — генерация исходников.

## 4. Взаимодействие скриптов

`BattleUnit.visualAnimations` ссылается на набор, `battleSprite` — fallback. `EnemySpriteAnimation` работает со `SpriteRenderer`; `BattlePresentation` — с `BattleUI.enemyPortrait`. `BattleTransition` скрывает врага вместе с миром на время боя.

## 5. Последовательность событий

Движение → Move; попадание → Hurt 0.16 с; HP 0 → Death; победа → объект выключается. В бою Attack/Hurt/Death показываются на портрете, в мире — на SpriteRenderer.

## 6. Настраиваемые параметры

Кадры 64×64, PPU 100, Point, без mipmaps и компрессии. `framesPerSecond = 8`. `movementThreshold = 0.001`, `hurtDuration = 0.16`, `deathDuration = 0.4`, `faceMovement = true`. Enemy сохраняет масштаб (2,2,1) и два коллайдера.

## 7. Использование в Unity

Кадры находятся в `Assets/Art/Enemies/Skeleton64`, набор — в `Assets/Settings/Battle/Visuals/Skeleton64.asset`. Назначь `BattleUnit.battleSprite`/`visualAnimations` и `SpriteRenderer`. Для нового врага создай отдельный набор, не перезаписывай существующие PNG.

## 8. Расширение

Новые кадры генерируются `PixelBattleAssetBuilder` или рисуются вручную на сетке 64×64. Полноценные четыре направления отсутствуют; при необходимости потребуется новый набор и логика выбора направления. Не добавляй Idle-клипы.

## 9. Ограничения и проверки

Один фронтальный набор с отражением; смерть не создаёт труп/лут. Арт оценивается владельцем. EditMode проверяет 32 PNG, размеры, импорт и отсутствие Idle; PlayMode — Move/Attack/Hurt/Death, коллизии и сохранение ресурсов.

## История

- 2026-10-31 — удалены IDLE-кадры и упоминания Idle; набор сокращён до Move/Attack/Hurt/Death.
- 2026-10-12 — базовый пиксельный визуал скелета.

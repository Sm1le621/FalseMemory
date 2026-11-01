# Анимации боя и мира

## 1. Назначение

Показ поз персонажей в мире и на боевых портретах без изменения боевых ресурсов. IDLE-анимации нет: в покое используется статичный `battleSprite`.

## 2. Общая логика

`SpriteAnimationSet` хранит кадры `Move/Attack/Hurt/Death`. `EnemySpriteAnimation` в мире выбирает позу по смещению, HP и смерти. `BattlePresentation` проигрывает те же кадры на портретах `BattleUI`: выпад Attack, Hurt при попадании, Death при исходе, Move при побеге. `BattleManager` закрывает gate до Windup, повторно проверяет жизнь и применяет эффект один раз.

## 3. Основные компоненты

- `SpriteAnimationSet` — ScriptableObject кадров.
- `EnemySpriteAnimation` — мир.
- `BattlePresentation` — боевые портреты.
- `BattleUI` — всплывающие числа и портреты.
- `BattleManager` — порядок и применение действий.

## 4. Взаимодействие скриптов

`BattleUnit.visualAnimations` даёт кадры, `battleSprite` — fallback. `EnemySpriteAnimation` читает `Health` и позицию. `BattlePresentation.Bind` получает участников от `BattleManager`/`BattleUI`. Анимация не вызывает `TakeDamage`, не тратит MP и не меняет очередь.

## 5. Последовательность событий

Движение в мире → Move; урон → Hurt; HP 0 → Death. В бою: Windup → расчёт → Recover/Hurt → Finish при победе/поражении/побеге. MISS не вызывает отдачи; при смерти во время замаха действие отменяется.

## 6. Настраиваемые параметры

`attackDuration` 0.4, `hitDuration` 0.18, `outcomeDuration` 0.35, `messageDuration` 0.75, `enemyThinkDuration` 0.5, `lungeDistance` 42, `reactionDistance` 8. `framesPerSecond` в наборе — 8. Space ускоряет визуал ×3, не меняя `Time.timeScale`.

## 7. Использование в Unity

Кадры лежат в `Assets/Art/Enemies/Skeleton64` и импортированы как Sprite 2D 64×64, Point, без mipmaps, PPU 100. Набор — `Assets/Settings/Battle/Visuals/Skeleton64.asset`; он назначен в `BattleUnit.visualAnimations`. Портреты и щиты сохранены в `BattleScene`.

## 8. Расширение

Новый набор создаётся через `Create → FalseMemory → Sprite Animation Set`; новые кадры генерируются `PixelBattleAssetBuilder`. Новые позы потребуют расширения enum и всех потребителей. Не добавляй Idle-клипы.

## 9. Ограничения и проверки

Один фронтальный набор с отражением; нет четырёх направлений и Animator Controller. Анимации не содержат игровых событий. PlayMode проверяет Move/Attack/Hurt/Death, отсутствие IDLE, ускорение, MISS и смерть во время замаха. Художественная оценка — за владельцем.

## История

- 2026-10-31 — удалена IDLE-анимация, позы сокращены до Move/Attack/Hurt/Death.
- 2026-10-12 — базовые пиксельные анимации.

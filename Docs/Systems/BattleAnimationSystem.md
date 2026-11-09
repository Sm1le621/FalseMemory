# Анимации боя и мира

## 1. Назначение

Показ поз персонажей в мире и на боевых портретах без изменения боевых ресурсов. Кадры хранятся в `.anim`-клипах, которые проигрывает Animator. IDLE-анимации нет: в покое используется статичный `battleSprite`.

## 2. Общая логика

`SpriteAnimationSet` хранит два готовых Animator-контролера: `worldController` (клипы `Move/Hurt/Death` для `SpriteRenderer`) и `portraitController` (клипы `Attack/Hurt/Death` для UI `Image`). `EnemySpriteAnimation` в мире выбирает позу по смещению, HP и смерти. `BattlePresentation` проигрывает клипы на портретах `BattleUI`: выпад Attack, Hurt при попадании, Death при исходе, Move при побеге. `SpriteClipPlayer` добавляет Animator в рантайме и выставляет состояние/прогресс. `BattleManager` закрывает gate до Windup, повторно проверяет жизнь и применяет эффект один раз.

## 3. Основные компоненты

- `SpriteAnimationSet` — ScriptableObject со ссылками на контролеры.
- `SpriteClipPlayer` — общий помощник показа клипов.
- `EnemySpriteAnimation` — мир.
- `BattlePresentation` — боевые портреты.
- `BattleUI` — всплывающие числа и портреты.
- `BattleManager` — порядок и применение действий.
- `SpriteAnimationAssetBuilder` — редакторская сборка клипов/контролеров.

## 4. Взаимодействие скриптов

`BattleUnit.visualAnimations` даёт контролеры, `battleSprite` — fallback. `EnemySpriteAnimation` читает `Health` и позицию и просит `SpriteClipPlayer` показать `Move/Hurt/Death`. `BattlePresentation` кеширует Animator портретов и показывает `Attack/Hurt/Death`. `BattlePresentation.Bind` получает участников от `BattleManager`/`BattleUI`. Анимация не вызывает `TakeDamage`, не тратит MP и не меняет очередь.

## 5. Последовательность событий

Движение в мире → клип Move; урон → Hurt; HP 0 → Death. В бою: Windup → расчёт → Recover/Hurt → Finish при победе/поражении/побеге. MISS не вызывает отдачи; при смерти во время замаха действие отменяется.

## 6. Настраиваемые параметры

`attackDuration` 0.4, `hitDuration` 0.18, `outcomeDuration` 0.35, `messageDuration` 0.75, `enemyThinkDuration` 0.5, `lungeDistance` 42, `reactionDistance` 8. Частота клипов — 8 fps (задаётся в `SpriteAnimationAssetBuilder`). Space ускоряет визуал ×3, не меняя `Time.timeScale`.

## 7. Использование в Unity

Кадры лежат в `Assets/Art/Enemies/Skeleton64` и импортированы как Sprite 2D 64×64, Point, без mipmaps, PPU 100. Клипы и контролеры — в `Assets/Animations/Enemies/Skeleton/{World,Portrait}`, набор — `Assets/Settings/Battle/Visuals/Skeleton64.asset`; он назначен в `BattleUnit.visualAnimations`. Animator на Enemy и портреты не добавляется в сцене: его создаёт `SpriteClipPlayer`. Портреты и щиты сохранены в `BattleScene`.

## 8. Расширение

Новый набор создаётся через `Create → FalseMemory → Sprite Animation Set`, кадры — через `PixelBattleAssetBuilder`, клипы и контролеры — через `FalseMemory → Animation → Rebuild sprite animations`. Новые позы потребуют расширения enum, клипов и всех потребителей. Не добавляй Idle-клипы.

## 9. Ограничения и проверки

Один фронтальный набор с отражением; нет четырёх направлений. Анимации не содержат игровых событий. PlayMode проверяет Move/Attack/Hurt/Death, отсутствие IDLE, ускорение, MISS и смерть во время замаха. EditMode проверяет наличие состояний в контролерах. Художественная оценка — за владельцем.

## История

- 2026-11-10 — анимации переведены на `.anim`-клипы и Animator; добавлен `SpriteClipPlayer`, набор хранит контролеры.
- 2026-10-31 — удалена IDLE-анимация, позы сокращены до Move/Attack/Hurt/Death.
- 2026-10-12 — базовые пиксельные анимации.

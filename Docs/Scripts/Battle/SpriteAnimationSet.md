# SpriteAnimationSet

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/SpriteAnimationSet.cs`.

ScriptableObject со ссылками на готовые Animator-контролеры персонажа. Сами кадры лежат в `.anim` AnimationClip; ассет больше не хранит массивы `Sprite`. Позы `Idle` по-прежнему нет.

## 2. Как работает

В ассете два поля: `worldController` для анимации в мире (кадры применяются к `SpriteRenderer`) и `portraitController` для боевого портрета (кадры применяются к UI `Image`). `EnemySpriteAnimation` и `BattlePresentation` берут нужный контролер и просят `SpriteClipPlayer` показать состояние `Move`, `Attack`, `Hurt` или `Death` на нужном прогрессе.

## 3. Методы и функции

Собственных методов нет. Перечисление `SpritePose` (`Move`, `Attack`, `Hurt`, `Death`) остаётся общим списком имён состояний. Имена состояний в контролерах совпадают с именами значений `SpritePose`.

## 4. Переменные и Inspector

- `worldController` — Animator-контролер для мира; содержит клипы `Move` (цикл), `Hurt`, `Death`.
- `portraitController` — Animator-контролер для портрета в бою; содержит клипы `Attack`, `Hurt`, `Death`.
- В `Skeleton64.asset` назначены `SkeletonWorld.controller` и `SkeletonPortrait.controller`.

Старые поля `move`, `attack`, `hurt`, `death` и `framesPerSecond` в коде отсутствуют. В YAML-файле ассета могут остаться старые строки — Unity игнорирует их и удалит при следующем сохранении.

## 5. Связи

Используется `EnemySpriteAnimation` и `BattlePresentation`. Ссылка хранится в `BattleUnit.visualAnimations`. `battleSprite` остаётся статичным fallback, если контролер не назначен. Кліпы и контролеры собирает редакторский `SpriteAnimationAssetBuilder`, кадры рисует `PixelBattleAssetBuilder`.

## 6. Пример

У скелета `Move` — 4 кадра, `Attack` — 3, `Hurt` — 2, `Death` — 4. В мире `SpriteClipPlayer.Loop(..., "Move")` крутит ходьбу, а в бою `Sample(..., "Hurt", progress)` показывает нужный кадр по прогрессу фазы.

## 7. Настройка в Unity

Создай ассет через `Assets → Create → FalseMemory → Sprite Animation Set` и назначь оба контролера. Для нового врага сделай отдельный набор и отдельные контролеры через `FalseMemory → Animation → Rebuild sprite animations`. Кадры должны быть 64×64, Point filtering, без mipmaps.

## 8. Возможные проблемы

Если контролер не назначен, используется статичный `battleSprite`. Имена состояний должны совпадать с `SpritePose`, иначе `SpriteClipPlayer` не найдёт состояние. Animator добавляется в игре автоматически, поэтому не нужно вручную вешать его на Enemy или портрет. Не пытайся запросить `SpritePose.Idle` — такого значения нет.

## 9. История изменений

- 2026-11-10 — ассет переведён на `.anim`-клипы и Animator-контролеры; массивы кадров удалены.
- 2026-10-31 — удалена поза `Idle`; остались Move/Attack/Hurt/Death.
- 2026-10-12 — базовый набор анимаций.

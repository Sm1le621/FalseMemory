# EnemySpriteAnimation

## 1. Название и назначение

Исходник: Assets/Scripts/Battle/EnemySpriteAnimation.cs.

Компонент мирового Enemy: показывает кадры и отражение по фактическому перемещению, читая здоровье. Не двигает объект и не наносит урон.

## 2. Как работает

Отслеживает изменение позиции между кадрами и HP; живой объект показывает Move/Idle, после уменьшения HP кратко Hurt, погибший — конечную Death. Карта скрывается существующим Transition, поэтому её анимация также прекращается.

## 3. Методы и функции

`Awake()` получает SpriteRenderer/BattleUnit. `OnEnable()` сбрасывает previousPosition/previousHP/clock и таймеры. `LateUpdate()` читает displacement/HP, обновляет clock, выбирает pose/кадр; обновляет последнее направление и выбирает directional Walk/Idle; у старых профилей остаётся отражение по X. Возвращаемых значений нет; изменяются только Sprite/flipX и внутренние таймеры.

LateUpdate обновляет movingUntil при реальном displacement выше movementThreshold. Между физическими шагами Walk сохраняется на movementHoldDuration, чтобы не мигать Idle. После задержки Idle выбирается по lastDirection. Health/Hurt/Death сохраняют приоритет. OnEnable сбрасывает movingUntil и таймеры, но сохраняет последнее направление при возврате из боя. При faceMovement=false направление не меняется; legacy flip по прежним правилам.

## 4. Переменные и Inspector

movementThreshold=0.001 (≥0); hurtDuration=0.16 (Inspector0.05…0.5), deathDuration=0.4 (0.1…1), faceMovement=true. Эти значения назначены по умолчанию на Enemy. visual/unit/previousPosition/previousHP/clock/hurtUntil/deathStarted — внутреннее состояние.

movementHoldDuration=.08 по умолчанию и в сцене (допустимо.01… .2 секунды); влияет только на задержку выбора Idle, не на скорость/положение врага. lastDirection=Down при первом создании и movingUntil — внутреннее состояние, не Inspector.

## 5. Связи

Требует SpriteRenderer и BattleUnit; последний требует Health. Читает Unit.visualAnimations/battleSprite; EnemyAI и Rigidbody2D управляют движением независимо.

## 6. Пример

EnemyAI переместил скелета влево → SkeletonDetailed выбирает Left Walk без дополнительного flipX. Старый Skeleton64 сохраняет Move/flipX=true. При остановке — Idle; позиция/скорость не изменяются этим скриптом.

## 7. Настройка в Unity

Прикреплён через MCP к существующему Enemy SampleScene. Назначить профиль в BattleUnit. Сохранять Collider2D/Rigidbody2D/EnemyAI и масштаб исходного объекта.

## 8. Возможные проблемы

У SkeletonDetailed есть четыре направления; отдельный Animator Controller не требуется. Движение определяется displacement, краткие паузы между физическими шагами могут переключать pose; художественную плавность принимает владелец. Скрипт не возрождает и не удаляет объект.

## 9. История изменений

2026-10-12 — создан в задаче native 64px Skeleton/Fire/Heal/Hit и интеграции кадров; текущие проверки — [PixelVisuals](../../PixelVisuals.md).

- 2026-10-13 — направления Down/Up/Right/Left и удержание Walk между физическими шагами; AI/Rigidbody/Collider/ресурсы не изменены.

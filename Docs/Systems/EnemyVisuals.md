# Визуал врагов FalseMemory

## Назначение и данные

2026-10-12 — существующий Enemy SampleScene использует Skeleton64: original native PNG64×64, тёмные кости/ржавая броня/бордовая ткань/меч/щит. Старый Assets/skeleton-topdown-64.png и его импорт сохранены. Источник новых рисунков — Assets/Editor/PixelBattleAssetBuilder.cs, напрямую заполняющий RGBA на сетке64; увеличенный AI-референс не является игровым исходником.

## Последовательность и компоненты

EnemyAI/Rigidbody2D определяют реальную позицию. EnemySpriteAnimation читает displacement и Health, выбирает Idle/Move/Hurt/Death и flipX. BattleUnit.visualAnimations ссылается на SpriteAnimationSet; battleSprite — Idle-00 fallback. Transition прячет исходную карту во время боя. BattlePresentation проигрывает этот же набор на UI.enemyPortrait по конечному progress; применение действий остаётся в Manager. Ни один новый визуальный компонент не пишет ресурсы/скорость/коллайдеры.

## Пути и настройка

Assets/Art/Enemies/Skeleton64: Idle2, Move4, Attack3, Hurt2, Death4, отдельные PNG64×64. Assets/Settings/Battle/Visuals/Skeleton64.asset — массивы и FPS8/Idle2. Importer: Sprite Single, Point, Uncompressed, no mipmaps, PPU100, центрированный pivot. Enemy сохраняет scale(2,2,1), прежнюю позицию/Rigidbody/два CircleCollider2D (тело radius0.32). Ссылки SpriteRenderer/Unit назначены на существующем объекте через MCP; BattleUI.previewEnemySprite также обновлён и сцены фактически сохранены.

## Как заменить позже

Открыть PNG в pixel-art редакторе и сохранять размер64×64/pivot; .meta/GUID не удалять. Для нового набора создать отдельные ресурсы и SpriteAnimationSet, заполнить5 массивов и назначить Unit.visualAnimations/battleSprite и SpriteRenderer. Действующие шаблоны Generate/GenerateImpact отвергают существующие пути; не удалять ассеты ради повторной генерации. Новый вариант программного рецепта должен использовать новые пути. В инспекторе менять FPS, не урон. UI-кадры не требуют Animator Controller/Animation Events.

## Расширение и ограничения

Сейчас один фронтальный набор с отражением; полноценные четыре направления отсутствуют. Герой сохраняет исходные кадры/анимации мира. В бою применяются кадры врага и прежние движения RectTransform; новой боевой логики нет. World Heal/Fire не добавлялись. Death после победы не создаёт новый труп/лут — старый Transition отключает Enemy. Нативный PNG является основным форматом, превью1920×1080 его не заменяет. Художественную читаемость/стиль и ощущение темпа принимает владелец, standalone ещё не проверен.

## Проверки

[PixelVisuals](../PixelVisuals.md): фактические размеры/палитра/альфа32 PNG, фильтры/ссылки Edit Mode, Move/Attack/Hurt/Death/коллизия/ресурсы Play Mode, реальные кадры Game View. Индивидуальные скрипты: [SpriteAnimationSet](../Scripts/Battle/SpriteAnimationSet.md), [EnemySpriteAnimation](../Scripts/Battle/EnemySpriteAnimation.md), [BattlePresentation](../Scripts/Battle/BattlePresentation.md).

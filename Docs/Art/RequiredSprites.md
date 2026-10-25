# Недостающие спрайты

Список составлен 2026-10-25 после переноса UI/эффектов в сцены. Все новые пиксельные ассеты — Dark Fantasy Pixel Art, целые пиксели, без сглаживания; импорт Sprite 2D, Point, без mipmaps и размывающей компрессии.

## 1. Игрок (64×64, 15 кадров)

Сейчас у `Player` в бою только один `battleSprite`; `visualAnimations` пуст.

| Поза | Кадров | Имена (предложение) |
|---|---:|---|
| Idle | 2 | `Player-Idle-00…01` |
| Move | 4 | `Player-Move-00…03` |
| Attack | 3 | `Player-Attack-00…02` |
| Hurt | 2 | `Player-Hurt-00…01` |
| Death | 4 | `Player-Death-00…03` |

После рисования: собрать `SpriteAnimationSet` `Assets/Settings/Battle/Visuals/Player64.asset` и назначить в `Player.visualAnimations`. Размер и pivot — как у Skeleton64 (64×64, низ по центру), чтобы не менять масштаб/коллайдеры.

## 2. Частицы VFX (8×8 или 16×16)

`FireVisual.particleSprite` и `HealVisual.particleSprite` пусты, поэтому сейчас рисуются цветные квадраты.

| Спрайт | Размер | Назначение |
|---|---|---|
| `vfx-ember-00…03` | 8×8 | огонь, Rising (оранжевый/жёлтый) |
| `vfx-heal-mote-00…03` | 8×8 | лечение, Orbit (зелёный/белый) |
| `vfx-hit-spark-00…02` | 8×8 | физическое попадание |
| `vfx-impact-ring` | 16×16 | вспышка попадания (опционально) |

## 3. Hit / Slash (16×16 или 32×32)

`Assets/Art/VFX/Hit64/Impact.png` уже есть (64×64), используется как маска попадания. Для читаемости удара желательны:

- `vfx-slash-00…02` — 32×32, диагональный разрез;
- `vfx-crit-burst` — 32×32, усиленная вспышка для крита.

## 4. Иконки команд (16×16, 5 штук)

`BattleHUD.commandIcons` пуст; сейчас `CommandIcon` — белый квадрат.

- `icon-attack` — меч;
- `icon-defend` — щит;
- `icon-ability` — руна/посох;
- `icon-items` — сумка/флакон;
- `icon-flee` — сапог/стрелка.

Подписи кнопок остаются текстовыми; иконки необязательны и скрываются, если массив пуст.

## 5. Рамки и панели 9-slice (16×16)

Сейчас все панели/кнопки — белый пиксель `Assets/Art/UI/WhitePixel.png`, растянутый Image.

- `ui-button-frame` — 16×16, border 4px;
- `ui-panel-frame` — 16×16, border 4px;
- `ui-slot-frame` — 16×16, border 4px (обычный/выбранный/редкий варианты);
- `ui-outline` — 16×16 для `Outline`-эффекта (опционально).

## 6. Фон и символы защиты

- `menu-background` — 320×180 или 640×360, тёмный лес/склеп; сейчас плоский цвет (18,12,28).
- `guard-shield` — 24×24 или 32×32; сейчас `PlayerGuard`/`EnemyGuard` — повёрнутый на 45° цветной квадрат.

## 7. Порядок внедрения

1. Игрок 15 кадров + `Player64.asset` (самый заметный пробел).
2. Частицы огня/лечения (убирают квадраты).
3. Иконки команд.
4. Рамки/фон.
5. Slash/крит и щит.

Ссылки: [SceneComposition](../Systems/SceneComposition.md), [BattleVFXSystem](../Systems/BattleVFXSystem.md), [EnemyVisuals](../Systems/EnemyVisuals.md), [PixelArtTests](../../Assets/Tests/EditMode/PixelArtTests.cs).

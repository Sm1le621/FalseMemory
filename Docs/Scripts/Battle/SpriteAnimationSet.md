# SpriteAnimationSet

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/SpriteAnimationSet.cs`.

ScriptableObject с кадрами анимации для мира и боя: движение, атака, получение урона и смерть. Позы `Idle` больше нет.

## 2. Как работает

Ассет хранит четыре массива `Sprite`. `Frames(pose)` возвращает нужный массив. `At(pose, progress, fallback)` выбирает кадр по прогрессу 0…1 (последний кадр для 1), `Loop(pose, seconds, fallback)` циклически выбирает кадр по времени и `framesPerSecond`. Если массив пуст или кадр равен `null`, возвращается fallback — обычно `BattleUnit.battleSprite`.

## 3. Методы и функции

- `Frames(SpritePose pose)` → `Sprite[]`; выбирает массив по позе.
- `At(SpritePose pose, float progress, Sprite fallback = null)` → `Sprite`; кадр по прогрессу, без зацикливания.
- `Loop(SpritePose pose, float seconds, Sprite fallback = null)` → `Sprite`; циклический кадр по времени.

Перечисление `SpritePose`: `Move`, `Attack`, `Hurt`, `Death`.

## 4. Переменные и Inspector

- `move`, `attack`, `hurt`, `death` — массивы кадров; могут быть пустыми.
- `framesPerSecond` — скорость цикла, 1…24, по умолчанию 8.
- В `Skeleton64.asset` заполнены Move/Attack/Hurt/Death; IDLE-кадры удалены.

## 5. Связи

Используется `EnemySpriteAnimation` и `BattlePresentation`. Ссылка хранится в `BattleUnit.visualAnimations`. `battleSprite` остаётся статичным fallback. Ассеты кадров создаются `PixelBattleAssetBuilder`.

## 6. Пример

У скелета 4 кадра `Move`. `Loop(Move, 0.5f)` при 8 fps выбирает кадр 0; `At(Hurt, 0.5f)` выбирает средний кадр из двух. Если массив `Hurt` пуст, показывается `battleSprite`.

## 7. Настройка в Unity

Создай ассет через `Assets → Create → FalseMemory → Sprite Animation Set`, заполни массивы и назначь в `BattleUnit.visualAnimations`. Кадры должны быть 64×64, Point filtering, без mipmaps. Для нового врага можно использовать отдельный ассет.

## 8. Возможные проблемы

Пустые массивы не ошибка — используется fallback. `At` при `progress = 1` берёт последний кадр, `Loop` никогда не выходит за границы. Отрицательное время в `Loop` трактуется как 0. Набор не содержит IDLE, поэтому не пытайся запросить `SpritePose.Idle` — такого значения нет.

## 9. История изменений

- 2026-10-31 — удалена поза `Idle`; остались Move/Attack/Hurt/Death.
- 2026-10-12 — базовый набор анимаций.

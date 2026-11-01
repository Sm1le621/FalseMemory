# PixelArtTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/PixelArtTests.cs`.

Проверки реальных PNG и импортёров: 64×64, Point, без mipmaps/компрессии, полные наборы кадров и отсутствие Idle.

## 2. Как работает

Читает `AssetDatabase` по путям `Assets/Art/Enemies/Skeleton64` и `Assets/Art/VFX`, проверяет 32 текстуры и `Skeleton64.asset`. Для проверки fallback создаёт временную копию набора и уничтожает её.

## 3. Методы и функции

- `EveryNewFrameIsNative64PointUncompressedAndWithoutMipmaps()` — 32 PNG: размер 64×64, rect 64×64, PPU 100, Point, без mipmaps, Uncompressed, Single.
- `SkeletonHasNoIdlePoseAndFireHasCompleteFrames()` — все позы набора непустые, `SpritePose` не содержит `Idle`, Fire имеет 8 кадров и duration 0.1…1.
- `FiniteFramesClampAndLoopIsStableWithMissingFrames()` — `At` ограничивает прогресс, `Loop` стабилен, пустые/null кадры дают fallback.

## 4. Переменные и Inspector

Пути и число 32 — контракт текущего набора. `copy` — временная копия, исходный ассет не меняется. Inspector отсутствует.

## 5. Связи

UnityEditor, `SpriteAnimationSet`, `BattleVFXProfile`, `PixelBattleAssetBuilder`. PlayMode-проверки кадров — в `PixelVisualFlowTests`.

## 6. Пример

`At(Death, -1)` возвращает первый кадр смерти, `At(Death, 2)` — последний; `Loop(Move, 0.125)` при 8 fps — второй кадр.

## 7. Настройка в Unity

Компонент не прикрепляется; Test Runner → EditMode. Требуется открытый проект с импортированными ассетами.

## 8. Ограничения и проверки

Проверяются размеры/импорт/ссылки, не художественное качество. Набор содержит Move/Attack/Hurt/Death без Idle. 2026-10-31: тесты прошли в составе EditMode 59/59.

## 9. История изменений

- 2026-10-31 — добавлена проверка отсутствия Idle; число кадров/поз приведено к текущему набору.
- 2026-10-12 — базовая документация.

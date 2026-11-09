# PixelArtTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/PixelArtTests.cs`.

Проверки реальных PNG и импортёров: 64×64, Point, без mipmaps/компрессии, полные наборы кадров, контролеры анимаций и отсутствие Idle.

## 2. Как работает

Читает `AssetDatabase` по путям `Assets/Art/Enemies/Skeleton64`, `Assets/Art/Enemies/BossSkeleton64` и `Assets/Art/VFX`, проверяет 25 текстур и `Skeleton64.asset`. Затем проверяет, что в Animator-контролерах есть нужные состояния, а у `FireVisual` заполнены `effectController` и `frameCount`.

## 3. Методы и функции

- `EveryNewFrameIsNative64PointUncompressedAndWithoutMipmaps()` — 25 PNG (14 скелета, 1 босс, 8 огня, 1 удар, 1 запасной Idle-кадр скелета): размер 64×64, rect 64×64, PPU 100, Point, без mipmaps, Uncompressed, Single.
- `SkeletonHasNoIdlePoseAndFireHasCompleteFrames()` — `SpritePose` не содержит `Idle`; `worldController` содержит `Move`/`Hurt`/`Death`, `portraitController` — `Attack`/`Hurt`/`Death`; Fire имеет `frameCount = 8`, контролер с состоянием `Effect` и duration 0.1…1.
- `AssertStateExists(RuntimeAnimatorController controller, string stateName)` — private helper; ищет состояние по имени в первом слое контролера.

## 4. Переменные и Inspector

Пути и число 25 — контракт текущего набора. Inspector отсутствует.

## 5. Связи

UnityEditor, `UnityEditor.Animations`, `SpriteAnimationSet`, `BattleVFXProfile`, `PixelBattleAssetBuilder`, `SpriteAnimationAssetBuilder`. PlayMode-проверки кадров — в `PixelVisualFlowTests`.

## 6. Пример

Если добавить новый PNG в `Assets/Art/VFX`, тест числа 25 упадёт — число нужно обновить вместе с набором. Если переименовать состояние контролера, упадёт проверка `AssertStateExists`.

## 7. Настройка в Unity

Компонент не прикрепляется; Test Runner → EditMode. Требуется открытый проект с импортированными ассетами и собранными контролерами.

## 8. Ограничения и проверки

Проверяются размеры/импорт/ссылки/состояния, не художественное качество. 2026-11-10: EditMode 57/57, включая этот файл.

## 9. История изменений

- 2026-11-10 — проверки переведены на контролеры и `.anim`-клипы; число PNG стало 25 после удаления Heal и добавления босса.
- 2026-10-31 — добавлена проверка отсутствия Idle; число кадров/поз приведено к текущему набору.
- 2026-10-12 — базовая документация.

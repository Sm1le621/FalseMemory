# PixelArtTests

## 1. Название и назначение

Исходник: Assets/Tests/EditMode/PixelArtTests.cs.

Edit Mode проверки фактических новых спрайтов/импортёров, полных ссылок и выбора кадров.

## 2. Как работает

Читает реальные AssetDatabase-ресурсы; проверяет32 PNG и наборы скелета/магии. Для пустых массивов создаёт временную копию профиля и уничтожает её.

## 3. Методы и функции

`EveryNewFrameIsNative64PointUncompressedAndWithoutMipmaps()` проверяет размер, PPU, Point, Single, компрессию и mipmaps. `SkeletonAndSpellsHaveCompleteAnimationReferences()` проверяет все5 pose и8 кадров Fire/Heal. `FiniteFramesClampAndLoopIsStableWithMissingFrames()` проверяет границы/циклическую скорость/пустые и null кадры. Unity callbacks отсутствуют.

## 4. Переменные и Inspector

Пути ассетов и ожидаемое количество32 являются контрактом текущего набора. Inspector-параметров нет. copy — временная инстанция, не изменяет ассет.

## 5. Связи

FalseMemory.Battle.EditModeTests, UnityEditor/Runtime, SpriteAnimationSet и BattleVFXProfile.

## 6. Пример

Удаление ссылки Death-03 нарушает тест полноты; включение Bilinear нарушает pixel-art проверку.

## 7. Настройка в Unity

Test Runner → Edit Mode → PixelArtTests. К объекту не прикрепляется.

## 8. Возможные проблемы

Не оценивает художественный стиль/ощущение движения; Play Mode проверяет поведение. При расширении набора согласовать контракт количества.

## 9. История изменений

2026-10-12 — создан в задаче native 64px Skeleton/Fire/Heal/Hit и интеграции кадров; текущие проверки — [PixelVisuals](../../../PixelVisuals.md).

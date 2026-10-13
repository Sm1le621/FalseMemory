# ApprovedHeroAssetTests

## 1. Название и назначение

`Assets/Tests/EditMode/ApprovedHeroAssetTests.cs` — проверяет импорт кадров и ссылки существующих Walk-клипов.

## 2. Как работает

Три NUnit-теста читают ассеты через AssetDatabase; сцены/настройки не меняются. Проверяются восемь направлений отдельно.

## 3. Методы и функции

WalkClipsUseEightFramesForTheirDirectionWithoutTransformAnimation также проверяет AnimationClipSettings.stopTime=⅔с, чтобы конечный повтор первого Sprite не создавал дополнительную задержку петли.

`SheetsKeepNativeResolutionAndConsistentGroundPivots()` проверяет четыре Sprite, реальные размеры texture/rect, PPU/pivot, Point, mipmaps/compression/NPOT. `WalkClipsUseEightFramesForTheirDirectionWithoutTransformAnimation()` проверяет девять ключей, восемь разных Sprite правильного направления, замыкание⅔с, Loop и отсутствие Transform-кривых. Возврат void; ошибки — NUnit assertions. Unity callbacks отсутствуют.

SheetsKeepNativeResolutionAndConsistentGroundPivots сохраняет проверку прежних резервных Multiple-листов. Тест Walk-клипов теперь проверяет текущие8 Single PNG/направление, замыкание на⅔с и отсутствие Transform-кривых. Резервные4-кадровые ресурсы не выдаются за актуальную ходьбу.

## 4. Переменные и Inspector

Root/Directions — пути/ожидаемые направления теста, не Inspector. Down512×640/1100PPU, прочие384×384/640PPU — контракт этого набора, не универсальное ограничение графики.

## 5. Связи

PNG ApprovedHero, Player64-клипы, SpriteRenderer, TextureImporter/AnimationUtility. Тест не ссылается на Editor-only ApprovedHeroImporter.

## 6. Пример

Max Size1024 уменьшит Down-лист2048×640 и нарушит контракт ширины. Назначение Down-кадров в Up-клип выявляется проверкой имён.

## 7. Настройка в Unity

К объекту не прикреплять. Test Runner → EditMode → FalseMemory.Battle.EditModeTests → ApprovedHeroAssetTests после завершённого импорта.

## 8. Возможные проблемы

Ссылки/настройки не доказывают естественность походки и идентичность деталей дизайна. Смену кадров проверяет PlayerDirectionFlowTests в Play Mode; художественную оценку — владелец.

WalkClipsUseEightFramesForTheirDirectionWithoutTransformAnimation теперь отдельно проверяет frameRate12 и завершающий ключ8/12=⅔с; проверки восьми уникальных ссылок, направления, loop и отсутствия Transform-кривых сохранены.

IdleDownHasThirtyNativeDistinctFramesAndSeparateTwoSecondLoop(): новый NUnit/Test void; читает Idle_Down Sprite-кривую,31 ключ/30 разных ссылок/30 разных SHA256 PNG,15FPS/2с и отдельность от Walk. Для каждого Single Sprite проверяет320×320/pivot160,16/PPU477/Point/no mipmaps/Uncompressed; последний ключ повторяет первый, stopTime2. Учитывает новый набор, не доказывая художественную уникальность каждой фазы.

## 9. История изменений

- 2026-10-13 — две проверки переменного разрешения и Sprite-ключей.

- 2026-10-13 — контракт активных Walk-клипов обновлён на8 кадров; проверки прежних источников сохранены.

- 2026-10-13 — актуализированы темп/физическое воспроизведение и проверки в задаче Animation Rework.

- 2026-10-13 — проверка нового30-frame Idle_Down без ослабления прежних направлений/движения.

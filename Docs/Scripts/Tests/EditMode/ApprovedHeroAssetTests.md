# ApprovedHeroAssetTests

## 1. Название и назначение

`Assets/Tests/EditMode/ApprovedHeroAssetTests.cs` — проверяет импорт кадров и ссылки существующих Walk-клипов.

## 2. Как работает

Два NUnit-теста читают ассеты через AssetDatabase; сцены/настройки не меняются. Проверяются восемь направлений отдельно.

## 3. Методы и функции

`SheetsKeepNativeResolutionAndConsistentGroundPivots()` проверяет четыре Sprite, реальные размеры texture/rect, PPU/pivot, Point, mipmaps/compression/NPOT. `WalkClipsUseEightFramesForTheirDirectionWithoutTransformAnimation()` проверяет девять ключей, восемь разных Sprite правильного направления, замыкание .5, Loop и отсутствие Transform-кривых. Возврат void; ошибки — NUnit assertions. Unity callbacks отсутствуют.

SheetsKeepNativeResolutionAndConsistentGroundPivots сохраняет проверку прежних резервных Multiple-листов. Тест Walk-клипов теперь проверяет текущие8 Single PNG/направление, замыкание на.5 и отсутствие Transform-кривых. Резервные4-кадровые ресурсы не выдаются за актуальную ходьбу.

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

## 9. История изменений

- 2026-10-13 — две проверки переменного разрешения и Sprite-ключей.

- 2026-10-13 — контракт активных Walk-клипов обновлён на8 кадров; проверки прежних источников сохранены.

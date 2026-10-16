# ApprovedHeroAssetTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/ApprovedHeroAssetTests.cs`. Проверяет реальные ассеты героя, Sprite-ссылки, импорт и сетку времени 16 FPS.

## 2. Как работает

Четыре NUnit-теста читают AssetDatabase/AnimationUtility; не меняют сцены и настройки. Ссылки, времена и параметры импорта проверяются отдельно от художественного качества.

## 3. Методы и функции

- `AllHeroClipsIncludingBackupsUseTheSixteenFpsTimeGrid()` — Test/void: все20 существующих клипов (16 directional и4 явно перечисленных резервных), Samples16, каждый Sprite-key.time×16 и stopTime×16 целые. Ловит изменение только метаданных без ретайминга ключей.
- `IdleDownHasThirtyNativeDistinctFramesAtSixteenFramesPerSecond()` — Test/void:31 ключ/30 уникальных ссылок и SHA256 PNG, FPS16/1,875 с, последний повторяет первый, stopTime1,875; каждый Sprite320×320/pivot160,16/PPU477/Point/no mipmaps/Uncompressed, набор отдельно от Walk.
- `SheetsKeepNativeResolutionAndConsistentGroundPivots()` — Test/void: резервные Multiple-листы каждого направления имеют4 Sprite/ожидаемые нативные размеры/PPU/pivot/Point/NPOT/no mipmaps/compression.
- `WalkClipsUseEightFramesForTheirDirectionWithoutTransformAnimation()` — Test/void: каждый текущий Walk имеет9 ключей,8 разных Sprite правильного направления, замыкание.5 с,16 FPS/Loop/stopTime.5, нет Transform-кривых.

Unity callbacks отсутствуют; результаты — NUnit assertions.

## 4. Переменные и Inspector

Root/Directions/paths/hash-наборы — тестовые данные, не Inspector. Размеры старых листов и Idle относятся к текущим ассетам, не универсальному ограничению графики.

## 5. Связи

AssetDatabase/TextureImporter/AnimationUtility, Player64 и четыре резервных clip, подготовленные PNG. Тест не зависит от Editor-класса импортёра напрямую.

## 6. Пример

Samples16 с ключами по1/12 с не пройдёт проверку целых ticks и длительности. Назначение Down-кадров в Up нарушит проверку направления.

## 7. Настройка в Unity

Не прикреплять к GameObject. Test Runner → EditMode → FalseMemory.Battle.EditModeTests после завершения импорта.

## 8. Возможные проблемы

Разные ссылки/хеши не доказывают последовательное движение. Частоту фактического воспроизведения/ввод проверяют Play Mode сценарии, художественное качество — просмотр кадров и владельцем.

## 9. История изменений


- 2026-10-13 — две проверки переменного разрешения и Sprite-ключей.

- 2026-10-13 — контракт активных Walk-клипов обновлён на8 кадров; проверки прежних источников сохранены.

- 2026-10-13 — актуализированы темп/физическое воспроизведение и проверки в задаче Animation Rework.

- 2026-10-13 — проверка нового30-frame Idle_Down без ослабления прежних направлений/движения.

- 2026-10-16 — актуализированы реальная сетка16 FPS, opt-in тайминг/проверки и ограничения текущего технического подэтапа.

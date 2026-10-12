# ApprovedHeroImporter

## 1. Название и назначение

`Assets/Editor/ApprovedHeroImporter.cs` — инструмент редактора для импорта утверждённых листов героя в существующие клипы и Player. Игровым компонентом не является.

## 2. Как работает

Вне Play Mode импортирует восемь листов по четыре кадра, сохраняет spriteID при повторном вызове, обновляет шестнадцать существующих Idle/Walk-клипов. Исходные PNG сохраняются как Default. Отдельный вызов подключает Player и сохраняет только SampleScene.

## 3. Методы и функции

- `ImportAnimations()` — MenuItem/MCP, без аргументов; готовит все листы/клипы и источники.
- `ImportSheet(direction)` — английское направление; Multiple, PPU, Full Rect через TextureImporterSettings, SpriteRect и stable spriteID через SpriteDataProviderFactories/ISpriteNameFileIdDataProvider. Apply/SaveAndReimport сохраняют конкретный importer.
- `ConfigureTexture(importer)` — изменяет только переданный importer: Point, no mipmaps/compression, Max Size4096, NPOT None, PNG alpha, Clamp.
- `Frames(direction)` — возвращает массив Sprite одного листа, отсортированный по имени.
- `WriteClip(motion,direction,frames)` — обновляет только существующий AnimationClip: SpriteRenderer.m_Sprite,8FPS, повтор первого ключа, Loop Time; SaveAssetIfDirty.
- `WireActivePlayer()` — требует SampleScene/Player; назначает SpriteRenderer.sprite/color, BattleUnit.battleSprite/spriteColor и InventoryWindow.characterPortrait.sprite/color; проверяет SaveScene. BattleScene preview назначен отдельно через MCP.

Unity Awake/Start/Update/FixedUpdate/столкновения отсутствуют: статический класс вызывается при авторинге.

## 4. Переменные и Inspector

Root — постоянный путь, Directions — восемь имён, не Inspector. Down512×640, PPU1100, pivot(256,40); прочие384×384, PPU640, pivot(192,20). Walk4 кадра/.5 секунды, Idle1 кадр. Значения совпадают с назначенными импортерами; moveSpeed не меняется.

## 5. Связи

UnityEditor Sprite data providers/AssetDatabase/AnimationUtility/EditorSceneManager; SpriteRenderer/BattleUnit/InventoryWindow исходного Player и прежние16 Player64-клипов. Подготовленные PNG не перерисовываются скриптом.

## 6. Пример

Вызвать ImportAnimations через MCP, затем WireActivePlayer в SampleScene. Walk_Down получает четыре утверждённых изображения; Idle_Down первый кадр. Физика/HP/MP/экипировка сохраняются.

## 7. Настройка в Unity

Не прикреплять к GameObject. FalseMemory → Art → Import approved hero animation sheets вне Play Mode, когда PNG уже существуют. Повторный вызов обновляет перечисленные клипы. WireActivePlayer — только существующая SampleScene.

## 8. Возможные проблемы

Отсутствующие PNG/clip/Player вызывают исключение; некорректная нарезка не исправляется автоматически. Не менять имена кадров ради перевода. Глобальный SaveAssets не используется. Idle статический, левые виды зеркальные; качество шага требует художественной проверки.

## 9. История изменений

- 2026-10-13 — адресный импорт крупного героя без уменьшения; Full Rect настроен через TextureImporterSettings для Unity6.

- 2026-10-13 — белый tint назначается также общему BattleUnit/портрету, чтобы старое затемнение не меняло утверждённую палитру.

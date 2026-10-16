# ApprovedHeroImporter

## 1. Название и назначение

Исходник: `Assets/Editor/ApprovedHeroImporter.cs`. Статический редакторский инструмент импорта существующей графики героя и настройки его существующих клипов. Не игровой компонент.

## 2. Как работает

Вне Play Mode импортирует manifest Idle_Down, резервные листы и исходники, записывает 16 направленных Idle/Walk клипов на сетке 16 FPS, переводит четыре прежних резервных клипа на эту же сетку и сохраняет существующую настройку WalkSpeed. GUID и рисунки сохраняются. Отдельный WireActivePlayer подключает прежние ссылки в SampleScene; в задаче 2026-10-16 он не вызывался.

## 3. Методы и функции

- `ImportAnimations()` — MenuItem/MCP, без аргументов, void. Проверяет Play Mode, вызывает ImportPreparedFrames, ImportSheet для восьми направлений, Frames/IdleFrames и WriteClip, ConfigureTexture исходников, RetimeLegacyClips, ConfigureController.
- `ImportPreparedFrames()` — читает PlayerAnimation30Manifest в DTO PreparedManifest/PreparedFrame, проверяет начало пути ThirtyFrameRoot, положительные размеры/PPU и существование PNG; адресно импортирует Single Sprite и исходники Default. Нет manifest → ранний выход.
- `PreparedFrames(motion,direction)` — возвращает отсортированные Sprite из Animation30/{motion}/{direction}; нет папки → пустой массив, неполные/неимпортированные 30 кадров → исключение. Ограничение 30 относится к этому прежнему набору, не ко всем будущим анимациям.
- `Frames(direction)` — подготовленный Walk30, если есть, иначе восемь AnimationV2/World PNG, иначе резервный Multiple-лист. Возвращает Sprite[].
- `IdleFrames(direction)` — подготовленный Idle30, иначе один первый кадр соответствующего Walk. Down использует собственные дыхательные изображения.
- `ImportSheet(direction)` — сохраняет Multiple/SpriteRect и spriteID старых четырёхкадровых листов; размеры, PPU и ground pivot прежние.
- `ConfigureTexture(importer)` — Point, без mipmaps, Uncompressed, Max4096, NPOT None, PNG alpha, Clamp; меняет только переданный importer.
- `WriteClip(motion,direction,frames)` — требует существующий clip/кадры, пишет SpriteRenderer.m_Sprite с шагом 1/16 с и повтором первого на frameCount/16; stopTime ровно frameCount/16, Loop, адресный SaveAssetIfDirty. Все motion используют 16 FPS независимо от количества кадров.
- `RetimeLegacyClips()` — четыре явно перечисленных резервных Player_* клипа: при прежнем FPS масштабирует времена Sprite-ключей, Animation Events и границы цикла oldFPS/16, затем Samples=16. Уже16 → пропуск; отсутствующий ресурс/невалидный FPS/неожиданные float-кривые → исключение для ручного анализа. Ссылки и порядок Sprite сохраняются; void.
- `ConfigureController()` — существующий Player.controller, WalkSpeed(default1), Player_DirectionalWalk.speedParameterActive; адресное сохранение без второго контроллера.
- `WireActivePlayer()` — только SampleScene вне Play: назначает первый Idle Down, прежний HeroDetailed и отдельный Inventory portrait, белый tint; проверяет фактический SaveScene. Физику/HP/MP не меняет.

Awake/Start/Update/коллизий нет.

## 4. Переменные и Inspector

Root/ThirtyFrameRoot/Directions — технические пути/направления. WalkFramesPerSecond=16 и сохранённый по совместимости ThirtyFrameIdleFramesPerSecond=16 — const, не Inspector. DTO хранит path/canvasWidth/canvasHeight/pivotX/pivotY/ppu. Фактический Idle_Down320×320/pivot160,16/PPU477. Walk8/16=0,5 с; Idle_Down30/16=1,875 с; остальные Idle1/16=0,0625 с, визуально статичны.

## 5. Связи

AssetDatabase/AnimationUtility/TextureImporter/Sprite data providers/EditorSceneManager; прежние Player64 clips, Player.controller, SpriteRenderer/BattleUnit/InventoryWindow. PlayerController меняет WalkSpeed по фактической скорости относительно4; Idle не ускоряется.

## 6. Пример

ImportAnimations → Walk_Down с прежними восемью Sprite и длительностью0,5 с → S запускает ходьбу → отпускание возвращает отдельный Idle_Down1,875 с. Это изменение времени, а не создание новых поз.

## 7. Настройка в Unity

К объекту не прикрепляется. FalseMemory → Art → Import approved hero animation sheets вне Play. Исходники и старые листы сохранять. Новый набор другого размера требует расширения импорта: не заполнять папку30 копиями ради прохождения проверки.

## 8. Возможные проблемы

Изменение только Samples без времени ключей не меняет реальную частоту. Новый импорт ретаймит ключи и границы. Резервные клипы не используются активными состояниями; их ритм меняется с60/8 на16 без изменения кадров. Новые рисунки Walk 2026-10-16 отклонены, активные Sprite прежние. Художественная плавность не доказана тестом ссылок. Глобальный SaveAssets не вызывается.

## 9. История изменений


- 2026-10-13 — адресный импорт крупного героя без уменьшения; Full Rect настроен через TextureImporterSettings для Unity6.

- 2026-10-13 — белый tint назначается также общему BattleUnit/портрету, чтобы старое затемнение не меняло утверждённую палитру.

- 2026-10-13 — расширен существующий импортёр для AnimationV2/16FPS и HeroDetailed без второго controller.

- 2026-10-13 — сохранён прежний крупный портрет Inventory независимо от Battle Idle.

- 2026-10-13 — актуализированы темп/физическое воспроизведение и проверки в задаче Animation Rework.

- 2026-10-13 — отдельные IdleFrames, подготовленный30-frame manifest/import и Idle_Down15FPS/2с; прежние игровые ресурсы сохранены.

- 2026-10-16 — актуализированы реальная сетка16 FPS, opt-in тайминг/проверки и ограничения текущего технического подэтапа.

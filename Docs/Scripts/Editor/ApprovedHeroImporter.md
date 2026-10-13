# ApprovedHeroImporter

## 1. Название и назначение

`Assets/Editor/ApprovedHeroImporter.cs` — редакторский импортёр существующих направленных клипов героя и подготовленных30-кадровых ресурсов. Не игровой компонент.

## 2. Как работает

Вне Play сначала читает новый PlayerAnimation30Manifest при наличии, настраивает30 Single PNG Idle_Down. Далее сохраняет прежний резервный Multiple-импорт восьми старых листов/GUID, выбирает текущие кадры для каждого направления и обновляет только существующие16 Idle/Walk клипов. Down Idle теперь30 кадров/15FPS/2с; остальные Idle пока статические. Все Walk остаются8 кадров/12FPS/⅔с. ConfigureController сохраняет прежний WalkSpeed на том же Walk-состоянии. Новые Walk drafts из Docs не импортируются. WireActivePlayer отдельно сохраняет SampleScene; при текущей задаче через MCP изменён только world preview Sprite.

## 3. Методы и функции

- `ImportAnimations()` — MenuItem/MCP, без аргументов: ImportPreparedFrames, ImportSheet, Frames/IdleFrames, WriteClip, настройка прежних source PNG как Default, ConfigureController; void.
- `ImportPreparedFrames()` — читает JSON в DTO PreparedManifest/PreparedFrame, проверяет существование файла/положительные размеры и PPU/начало пути ThirtyFrameRoot; импортирует конкретные Single Sprite, Full Rect/Custom pivot, Point/no mipmaps/compression, затем source PNG как Default. При отсутствии manifest безопасно сохраняет прежний путь; void.
- `PreparedFrames(motion,direction)` — читает отсортированные PNG в Animation30/{motion}/{direction}; нет папки → пустой массив; есть → требует ровно30 ненулевых Sprite или исключение. Возвращает Sprite[]. Частичный набор не объявляется готовым.
- `Frames(direction)` — сначала подготовленный30-frame Walk (сейчас ни одного), затем AnimationV2/World восемь PNG, иначе старый Multiple-лист; Sprite[] в порядке индексов.
- `IdleFrames(direction)` — отдельный30-frame Idle при наличии; иначе один первый кадр Frames. Не подменяет новый Idle ходьбой Down.
- `ImportSheet(direction)` — прежний Multiple Sprite import, stable spriteID, SpriteDataProviderFactories/SpriteRect; размеры/PPU/ground pivot резервных листов сохранены.
- `ConfigureTexture(importer)` — изменяет только переданный importer: Point/no mipmaps/Uncompressed/Max4096/NPOT None/PNG alpha/Clamp; void.
- `WriteClip(motion,direction,frames)` — пишет только SpriteRenderer.m_Sprite, замыкающий первый ключ, loop; startTime0/stopTime=frameCount/FPS без лишнего интервала. Текущий Walk8→12FPS, Idle30→15FPS, static Idle→8FPS. Предусмотрено Walk30→45FPS/⅔с для будущего готового рисунка; такой Walk ещё не создан/не проверен. SaveAssetIfDirty конкретного clip, GUID прежний.
- `ConfigureController()` — получает существующий Player.controller/Player_DirectionalWalk; создаёт float WalkSpeed(default1) только если отсутствует, включает speedParameterActive, адресно сохраняет. Других states/переходов не создаёт.
- `WireActivePlayer()` — требует SampleScene/Player вне Play; назначает первый IdleFrames Down в SpriteRenderer, прежний HeroDetailed/BattleUnit/white tint и отдельный Inventory portrait fallback; проверяет SaveScene. Не меняет физику/HP/MP.

Unity Awake/Start/Update/FixedUpdate/коллизии отсутствуют; статический класс только для авторинга.

## 4. Переменные и Inspector

Root — прежние ApprovedHero backup sheets; Directions — восемь английских направлений. WalkFramesPerSecond12, ThirtyFrameRoot=Animation30, ThirtyFrameIdleFramesPerSecond15 — const, не Inspector. PreparedManifest.frames и Frame.path/canvasWidth/canvasHeight/pivotX/pivotY/ppu — DTO JSON. Idle_Down PNG320×320, pivot(160,16), PPU477 назначены по manifest. Никакого отдельного health/ввода/движения не хранит.

## 5. Связи

AssetDatabase/TextureImporterSettings/AnimationUtility/Sprite data providers/EditorSceneManager; существующие Player.controller/Player64 clips, SpriteRenderer/BattleUnit/InventoryWindow. PlayerController уже разделяет Idle и Walk и ускоряет только Walk по фактическому перемещению; его runtime код не менялся.

## 6. Пример

Готовые30 Idle_Down PNG и manifest → ImportAnimations через MCP → Idle_Down получает30 Sprite и2-секундную петлю, Walk_Down остаётся прежним. На карте неподвижный герой проигрывает Idle, S включает Walk, отпускание возвращает Idle последнего взгляда.

## 7. Настройка в Unity

К GameObject не прикрепляется. Запускать FalseMemory → Art → Import approved hero animation sheets вне Play после подготовки файлов. Для нового направления/motion нужны ровно30 PNG, согласованные canvas/PPU/pivot и записи manifest; source PNG сохранять отдельно. Отдельный native вызов подключения только для правильной SampleScene. Не назначать отклонённые sheets вместо готовых Sprites.

## 8. Возможные проблемы и проверки

Нет clip/source/Player или неполные30 Sprite → явное исключение. Проверка количества/хешей не подтверждает30 анатомически правильных фаз; ноги/дыхание/лицо требуют просмотра. Авторинг обновляет существующие clip IDs и отдельные ресурсы, глобальный SaveAssets отсутствует. Другие семь Idle остаются статическими, Walk ещё8 кадров. Стороны оружия/зеркальные старые виды не исправлены этим импортом. Генератор может давать jitter/вариации деталей; новая художественная приёмка обязательна.

## 9. История изменений

- 2026-10-13 — адресный импорт крупного героя без уменьшения; Full Rect настроен через TextureImporterSettings для Unity6.

- 2026-10-13 — белый tint назначается также общему BattleUnit/портрету, чтобы старое затемнение не меняло утверждённую палитру.

- 2026-10-13 — расширен существующий импортёр для AnimationV2/16FPS и HeroDetailed без второго controller.

- 2026-10-13 — сохранён прежний крупный портрет Inventory независимо от Battle Idle.

- 2026-10-13 — актуализированы темп/физическое воспроизведение и проверки в задаче Animation Rework.

- 2026-10-13 — отдельные IdleFrames, подготовленный30-frame manifest/import и Idle_Down15FPS/2с; прежние игровые ресурсы сохранены.

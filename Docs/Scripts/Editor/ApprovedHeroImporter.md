# ApprovedHeroImporter

## 1. Название и назначение

Исходник: `Assets/Editor/ApprovedHeroImporter.cs`. Редакторский импортёр существующих ресурсов и клипов героя. Новая приоритетная ветка обрабатывает40 подготовленных Cardinal4 PNG (16 Idle, 16 архивных Walk и 8 шагов), не создаёт второй Animator.

## 2. Как работает

Вне Play при наличии PlayerCardinal4Manifest читает40 подготовленных PNG, настраивает импорт, записывает восемь прежних cardinal-клипов с четырьмя фазами и обновляет два существующих Blend Tree на четыре child motion. Возвращается без перезаписи архивных диагоналей/Animation30. При отсутствии нового manifest остаётся прежняя ветка восьми направлений. WireActivePlayer отдельно подключает новую графику и useCardinalAnimations в SampleScene, проверяет SaveScene. ImportAnimations и WireActivePlayer выполнены через MCP; существующая SampleScene сохранена, dirty=false.

## 3. Методы и функции

- `ImportAnimations()` — MenuItem/MCP, void, требует Play выключен. Новая ветка: ImportPreparedFrames(CardinalManifest,CardinalRoot), CardinalFrames для восьми motion/direction, WriteClip с hold8/добавленным замыканием для Idle и hold2/appendLoopKey=false для Walk, ConfigureController(true), return. Старая ветка: прежний manifest/Multiple sheets/16 клипов, RetimeLegacyClips/ConfigureController.
- `ImportPreparedFrames()` — void, обёртка прежнего Animation30 manifest. Перегрузка `ImportPreparedFrames(manifestPath,assetRoot)` читает DTO PreparedManifest/PreparedFrame, проверяет path под переданным корнем/существование/положительные размеры/PPU; адресно импортирует Single Sprite Full Rect/Custom pivot и Source как Default. Нет manifest → выход.
- `CardinalFrames(motion,direction)` → Sprite[]: для Idle сортирует PNG в папке направления, требует четыре ненулевых Sprite. Для Walk читает единственную запись направления в manifest.walkSequences, требует четыре пути, одинаковую стойку в позициях 0/3 и два различных шага, отличных от стойки. Возвращает упорядоченный Sprite[], ошибка данных вызывает исключение.
- `Frames(direction)` → Sprite[]: для текущих четырёх направлений новый Cardinal Walk; иначе прежний подготовленный Walk30/AnimationV2/старый Multiple.
- `IdleFrames(direction)` → Sprite[]: новый Cardinal Idle; иначе прежний30-frame Idle или один соответствующий Walk Sprite.
- `PreparedFrames(motion,direction)` → прежний Animation30 Sprite[]: отсутствующая папка → пусто, неполный набор30 → исключение. Это резервная ветка, не требование нового задания.
- `ImportSheet(direction)` — сохраняет старые Multiple/SpriteRect/spriteID, PPU и pivot; новая cardinal-ветка его не вызывает.
- `ConfigureTexture(importer)` — Point/no mipmaps/Uncompressed/4096/NPOT None/PNG alpha/Clamp только у переданного ресурса.
- `WriteClip(motion,direction,frames,holdSamples=1,appendLoopKey=true)` — сохраняет существующий clip/GUID и Samples16. Для Idle пишет равномерные ключи i×holdSamples/16 и замыкающий первый Sprite. Для четырёхпозиционного Walk appendLoopKey=false: ровно четыре ключа на ticks 0/1/4/7, без пятого ключа. start=0, stopTime=frameCount×hold/16, Loop=true; адресный SaveAssetIfDirty. Walk длиной .5 с: стойка .0625 с, правый шаг .1875 с, левый .1875 с, стойка .0625 с. Этот режим предназначен только для четырёх переданных поз.
- `ConfigureController(cardinal=false)` — прежний Player.controller/WalkSpeed на Player_DirectionalWalk. При true назначает существующим Idle/Walk Blend Tree четыре child Down/Left/Right/Up с единичным timeScale; старые диагональные клипы не удаляет. Адресное сохранение.
- `RetimeLegacyClips()` — резервные четыре Player_*: масштабирование Sprite-ключей/событий/границ oldFPS/16, уже16 → пропуск, неизвестные float-кривые/отсутствующий clip → исключение. В новой ветке повторно не выполняется.
- `WireActivePlayer()` — только SampleScene вне Play: первый Idle Down/white tint, useCardinalAnimations=true через SerializedObject при новом manifest; сохраняет существующие HeroDetailed/BattleUnit и отдельный Inventory portrait, проверяет SaveScene. Физику/HP/MP не меняет.

Unity callbacks Awake/Start/Update/коллизий нет.

## 4. Переменные и Inspector

Root/ThirtyFrameRoot/CardinalRoot/CardinalManifest/Directions/CardinalDirections — технические пути/порядки. WalkFramesPerSecond=16 — сетка Samples, не фактическая частота новых рисунков. hold Idle8 даёт смену двух рисунков/с и цикл2 с; Walk2 задаёт цикл .5 с с неравными удержаниями фаз, а не равномерные8 FPS. PreparedManifest.frames содержит40 записей, walkSequences — четыре PreparedWalkSequence(direction,paths[4]); первые/последние paths совпадают. PreparedFrame: path/canvasWidth/canvasHeight/pivotX/pivotY/ppu; Cardinal canvas512×512/pivot256,20; PPU Down602/Left567/Right569/Up576, одинаковый для Idle/Walk. Не параметры Inspector компонента.

## 5. Связи

AssetDatabase/AnimationUtility/TextureImporter/Sprite providers/EditorSceneManager, Player.controller/Player64 clips, PlayerController.useCardinalAnimations/SpriteRenderer/BattleUnit/InventoryWindow. Выбор взгляда меняет только визуальные параметры, фактическая диагональ остаётся у PlayerController.

## 6. Пример

ImportAnimations → четыре Idle2 с/Walk.5 с → WireActivePlayer → на S новый Walk Down, отпускание — четыре кадра спокойного Idle Down, W+D движет по диагонали с визуальным Up.

## 7. Настройка в Unity

Не прикреплять к объекту. FalseMemory → Art → Import approved hero animation sheets вне Play, затем WireActivePlayer только для существующей SampleScene. Не назначать Source Texture как игровой кадр и не удалять прежние ассеты.

## 8. Возможные проблемы

Неполные4 Sprite/нет клипа/неверная сцена → исключение, не создание альтернативы. Samples16 не означает16 новых изображений/с: новый Idle удерживает4 разных рисунка, Walk —4 позиции с повторной стойкой. Высокий кадр создаётся прозрачным padding без resampling. Художественное качество проверяется отдельно; результаты подключения/тестов — [PlayerWalkOrder](../../PlayerWalkOrder.md).

## 9. История изменений


- 2026-10-13 — адресный импорт крупного героя без уменьшения; Full Rect настроен через TextureImporterSettings для Unity6.

- 2026-10-13 — белый tint назначается также общему BattleUnit/портрету, чтобы старое затемнение не меняло утверждённую палитру.

- 2026-10-13 — расширен существующий импортёр для AnimationV2/16FPS и HeroDetailed без второго controller.

- 2026-10-13 — сохранён прежний крупный портрет Inventory независимо от Battle Idle.

- 2026-10-13 — актуализированы темп/физическое воспроизведение и проверки в задаче Animation Rework.

- 2026-10-13 — отдельные IdleFrames, подготовленный30-frame manifest/import и Idle_Down15FPS/2с; прежние игровые ресурсы сохранены.

- 2026-10-16 — актуализированы реальная сетка16 FPS, opt-in тайминг/проверки и ограничения текущего технического подэтапа.

- 2026-10-26 — актуализирован новый ограниченный набор4×4 Idle/Walk, выбор cardinal-взгляда/импорт и соответствующие проверки. Фактический статус выполнения — в PlayerCardinal4.

- 2026-10-26 — через Unity MCP подключён четырёхпозиционный Walk: нейтральная стойка → RightStep → LeftStep → та же стойка; обновлены контракты и реальные Play-проверки. Подробности текущей проверки — в PlayerWalkOrder.

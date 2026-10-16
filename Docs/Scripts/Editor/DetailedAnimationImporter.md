# DetailedAnimationImporter

## 1. Название и назначение

`Assets/Editor/DetailedAnimationImporter.cs` — инструмент Unity Editor для подготовленных кадров героя/скелета, профилей и адресного подключения существующих сцен. Не игровая система и не компонент.

## 2. Как работает

Читает AnimationV2Manifest.json, импортирует126 PNG по размерам/PPU/pivot из подготовки. Создаёт либо обновляет два SpriteAnimationSet, затем обновляет существующие клипы через ApprovedHeroImporter. Отдельные методы сохраняют SampleScene/BattleScene после подключения ссылок. Исходные листы остаются отдельными PNG Default.

## 3. Методы и функции

- `ImportAll()` — MenuItem/MCP вне Play Mode; читает DTO Manifest/Frame через JsonUtility, настраивает Single Sprite и сохраняет каждый importer, вызывает SaveProfile и ApprovedHeroImporter.ImportAnimations. Возврат void, существующие клипы/GUID сохраняются.
- `Configure(importer)` — Point, no mipmaps/compression, Max Size4096, NPOT None, alpha из PNG, Clamp; изменяет только переданный importer.
- `Frames(folder,prefix)` — сортирует подходящие PNG и возвращает массив Sprite; используются устойчивые имена с индексом00…07.
- `SaveProfile(path,root,name,hasDirections)` — сохраняет конкретный SpriteAnimationSet. Hero: Idle4/Attack8,16FPS/Idle16FPS/contact.5/useFrameTiming=true. Skeleton: прежние12FPS/Idle4FPS/useFrameTiming=false, дополнительно Hurt2/Death4 и четыре worldDirections с8 Walk/одним Idle. Hero использует прежнюю flash/fade реакцию без новых Hurt/Death кадров. Разделённые ставки не позволяют импорту героя менять темп скелета.
- `WireWorld()` — требует активную SampleScene; ApprovedHeroImporter подключает Player, затем существующий Enemy получает профиль/спрайт/white tint и movementHoldDuration.08. SaveScene проверяется.
- `WireBattlePreview()` — требует BattleScene, получает единственный BattleUI через FindAnyObjectByType, задаёт preview Sprite/white tint и расширяет только RectTransform портретов внутри прежнего battlefield; проверяет SaveScene.

Unity Awake/Start/Update/FixedUpdate и callbacks столкновения отсутствуют. Ошибочные Play Mode/сцена/файлы не подменяются созданием альтернативной сцены.

## 4. Переменные и Inspector

HeroRoot/SkeletonRoot и HeroProfile/SkeletonProfile — постоянные пути. Manifest.frames и Frame.path/canvasWidth/canvasHeight/pivotX/pivotY/ppu — DTO исходных данных, не Inspector. Profile worldDirections — настраиваемые данные, но выбор текущего движения остаётся у EnemySpriteAnimation. Battle портреты: Player anchors(.65,.46)..(.90,.80), Enemy(.10,.46)..(.35,.80), zero offsets; остальные UI не меняются.

## 5. Связи

AssetDatabase/TextureImporterSettings/EditorSceneManager/JsonUtility, ApprovedHeroImporter, SpriteAnimationSet, BattleUnit, EnemySpriteAnimation, InventoryWindow/BattleUI. PNG подготовлены отдельно с сохранением исходников и без resampling; инструмент не рисует и не влияет на HP/MP/AI/скорость.

## 6. Пример

Через MCP вызвать ImportAll, в SampleScene WireWorld, затем в BattleScene WireBattlePreview. На карте Enemy выбирает Down/Up/Right/Left; в бою оба участника используют собственные idle/attack. Старый Skeleton64 остаётся резервом.

## 7. Настройка в Unity

К GameObject не прикреплять. FalseMemory → Art → Import detailed character animations вне Play. Manifest и PNG должны существовать. WireWorld/WireBattlePreview вызываются отдельно для правильной активной сцены. Глобальный SaveAssets не используется.

## 8. Возможные проблемы

Нет источников/кадров — импорт не считается готовым; проверить Console и manifest. Повторный импорт сохраняет существующие профили/PNG .meta. Высокое разрешение увеличивает память; потенциальная оптимизация SpriteAtlas выполняется отдельной задачей, без уменьшения утверждённого дизайна. Художественную естественность не доказывает число кадров.

SaveProfile использует ApprovedHeroImporter.WalkFramesPerSecond=16 только для героя; его конечная атака теперь вычисляется из8 кадров/FPS16, а не прежних.4 секунды. Skeleton сохраняет12/Idle4 и старую длительность. Существующий BattleManager по-прежнему владеет уроном. Пробные листы текущей переработки не подменяют126 рабочих кадров. Повторный ImportAll действительно вызван через MCP2026-10-16; WireWorld/WireBattlePreview в этой задаче не вызывались, сцены сохранены прежними.

## 9. История изменений

- 2026-10-16 — повторный импорт сохраняет Hero16/Idle16/useFrameTiming и отдельные Skeleton12/Idle4/старый тайминг; изменение героя не ускоряет врага.

- 2026-10-13 — добавлен адресный импорт подробных battle/world кадров и профилей; используется FindAnyObjectByType для Unity6 без нового предупреждения об устаревшем API.

- 2026-10-13 — актуализированы темп/физическое воспроизведение и проверки в задаче Animation Rework.

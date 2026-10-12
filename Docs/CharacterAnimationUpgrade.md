# Новые анимации героя и скелета

## Реализовано2026-10-13

Расширена существующая визуальная система: герой и скелет получили отдельные анимированные Battle Idle и Attack; ходьба героя теперь8 кадров на направление вместо4, при16FPS и прежней длительности.5 секунды. Скорость игры/физика/Health/MP/порядок ходов не переписывались. Старые PNG/ASE/клипы/профили сохранены.

## Где находятся ресурсы

- `Assets/Art/Characters/Player/AnimationV2/Battle`: Hero Idle4/Attack8, холст512×512, pivot(192,24), PPU620.
- `Assets/Art/Characters/Player/AnimationV2/World`:64 кадра,8 направлений×8, холст640×512/pivot(320,24), PPU Down860/Up860/Right и Left830/UpRight и UpLeft850/DownRight и DownLeft790.
- `Assets/Art/Enemies/SkeletonDetailed/Battle`: Idle4/Attack8/Hurt2/Death4, холст384×384, pivot(176,24), PPU430.
- `Assets/Art/Enemies/SkeletonDetailed/World`:32 кадра, Down/Up/Right/Left×8, холст640×512/pivot(320,24), PPU880/770/700/700.
- `Source` рядом с каждым набором: десять оригинальных листов, байтовые копии результатов built-in imagegen. Source PNG не используются как игровые Sprite.
- `Assets/Settings/Battle/Visuals/HeroDetailed.asset` и `SkeletonDetailed.asset`: профили той же существующей SpriteAnimationSet.
- `Assets/Art/AnimationV2Manifest.json`: реальные crop-прямоугольники, размеры, pivot/PPU и контрольные хеши126 кадров.

Сгенерированы пять hero walk-ракурсов и три skeleton walk-ракурса; недостающие левые версии механически отражены из соответствующих правых. Задние виды имеют самостоятельные изображения. Разрешение листов гибкое: Hero Battle1295×1214, Skeleton Battle1159×1358, Hero Walk1586×992 (DownRight1774×887), Skeleton Walk1536×1024. Листы не уменьшались и не растягивались.

## Генерация и подготовка

Навык imagegen, встроенный image_gen, без CLI/API. Главный герой основан на утверждённом ApprovedHeroSource и ранее полученных ракурсах, скелет — на прежнем Skeleton64 и детализации утверждённого героя. Точные промпты всех10 вызовов: [CharacterAnimationPrompts](Verification/CharacterAnimationPrompts.json). Главные требования: прежний black armor/dark hair/burgundy cape, совместимый detailed pixel style, прозрачность,4 breathing Idle/8 sword phases,8 articulated walk phases/направление, естественное движение ног/рук/плаща, без blur/background/VFX в рисунке.

Некоторые удары выходят за условную ячейку листа. Поэтому подготовка выделяет связные alpha-компоненты персонажей и присоединяет близкие мелкие детали, затем выравнивает кадры на одинаковом холсте. Пиксели не ресэмплируются; alpha≤16 и далёкий изолированный шум исключены только из производных изображений, исходники сохранены целиком. Для walk ось головы/тела и нижняя линия стоп едины; battle использует опорную область стоп, corpse — центр лежащего силуэта. SpriteRect одинаков внутри каждого цикла. Широкий холст сохраняет плащ/меч, не заставляя расширять Collider.

Технический [PNG audit](Verification/CharacterAnimationArt.json):126 разных кадров в18 группах, совпадающие размеры/pivot/PPU внутри группы, общая линия земли, нет обрезания видимых краёв,10 byte-exact источников. Разные пиксели не доказывают полностью естественную походку.

## Как подключено

Через Unity MCP и[DetailedAnimationImporter](Scripts/Editor/DetailedAnimationImporter.md) импортированы Single Sprite: Point, Uncompressed, no mipmaps, NPOT None, Max Size4096/Custom ground pivot/Full Rect. Source Default. Изменены ссылки существующих Player/Enemy→BattleUnit.visualAnimations/battleSprite и SpriteRenderer; белый tint сохраняет цвета. SampleScene и BattleScene фактически сохранены с проверкой SaveScene.

У Player остаётся прежний Player.controller с двумя directional Blend Tree и16 клипами Player64. GUID/MoveX/Y/LastMoveX/Y/isMoving и PlayerController сохранены; клипы теперь8 Walk/16FPS и1 directional Idle. Old ApprovedHeroImporter предпочитает новые PNG и не откатывает их к4 кадрам. Существующий Enemy не является Prefab instance, поэтому изменён только его сохранённый объект в SampleScene; три существующих Prefab не менялись. Enemy Animator не добавлялся: EnemySpriteAnimation уже выполняет data-driven playback.

BattleUI использует новые preview-кадры, портретам выделено больше места для клинка внутри прежнего battlefield; другие панели/кнопки/тексты не перестраивались. InventoryWindow.portraitSprite сохраняет прежний утверждённый фронтальный портрет512×640, чтобы широкая боевая поза не уменьшала персонажа в существующем окне. При null работает прежний fallback battleSprite; предметы/ресурсы/drag не меняются.

## Атака и ожидание

BattlePresentation удерживает текущие Attack/Hurt/Death отдельно для участников. Ожидающий продолжает Idle4FPS, не затирая чужую атаку. SpriteAnimationSet.attackContactProgress=.5: Windup проходит подготовку0→.5, прежний BattleManager один раз применяет действие, Recover проходит завершение.5→1 вперёд, затем Idle. Анимация не наносит урон и не расходует MP; Animation Events не добавлены. Старое обратное проигрывание Attack убрано. Guard/Magic/Items/Flee/Space×3 и остановка при смерти сохранены.

EnemySpriteAnimation читает displacement, хранит lastDirection, выбирает ближайший из4 cardinal-ракурсов. Диагонали используют ближайший cardinal, не объявляются отдельными анимациями. Walk удерживается.08 секунды между физическими шагами вместо миграции Walk→Idle на каждом пустом Update. Это задержка визуального переключения, не изменение скорости ИИ. После остановки — Idle последнего направления. Hurt/Death имеют прежний приоритет.

## Скрипты и документация

Изменены SpriteAnimationSet, BattlePresentation, EnemySpriteAnimation, ApprovedHeroImporter и InventoryWindow. Новый DetailedAnimationImporter — только Editor. Обновлены ApprovedHeroAssetTests/PlayerDirectionFlowTests/PixelVisualFlowTests, добавлены DetailedAnimationTests/DetailedAnimationFlowTests. InventoryPresentationFlowTests проверяет optional portrait/fallback вместо устаревшей обязательной связи с battleSprite. Для каждого источника обновлён отдельный9-раздельный документ Docs/Scripts с сохранением истории; обновлены AnimationSystem/BattleAnimationSystem/EnemyVisuals, Architecture/README/Roadmap/Changelog и инструкции импорта/тестирования.

## Реально выполненные проверки

Первый прогон: **95/95 Edit Mode**, job `9f32af5046fb478fab9e787e2dd92d99`; **52/52 Play Mode**, job `47bd5da6fd2440f28165f42f0876fa8e`,0 failed/skipped. Проверено воспроизведение4 idle-кадров каждого участника, обе атаки с минимум6 наблюдаемыми кадрами, однократные HP60→50/100→96, MP30, возврат обоих в Idle/следующий ход. Проверены направления скелета и удержание Walk между физическими кадрами; герой реально показывает8 Down-кадров,8 направлений/Idle/gate. Старые регрессионные сценарии проверяют контакт/бой/VFX/победу/поражение/инвентарь/экипировку/характеристики.

Обычный Play Mode: герой/скелет одновременно на карте, Inventory, физический переход в BattleScene. Для наглядного снимка враг временно поставлен рядом и AI отключён только в runtime; сохранённая сцена/ИИ не менялись. Battle1920×1080 и3440×1440: нет UI за экраном, overflow TMP, русского текста и missing glyphs; [метрики](Verification/CharacterAnimationLayout.json). Карта/Inventory1920 проверены аналогично. Снимки анализировались визуально. Найден и исправлен слишком маленький inventory portrait после замены battleSprite. Повторный прогон после этой коррекции:95 Edit прошли,51/52 Play прошёл; единственный провал — устаревший assert SameAs(battleSprite) в InventoryPresentationFlowTests. Ожидание исправлено на назначенный portraitSprite/fallback; проверки Tooltip/предметов/HP/MP/gear сохранены. Финальный прогон после обновления теста указан ниже.

Новый CS0618 FindFirstObjectByType в импортёре исправлен на FindAnyObjectByType; компиляция успешна. До тестов наблюдалось прежнее служебное предупреждение Unity Account API30 секунд, ошибок игровых скриптов не было. Итоговая Console и восстановление редактора фиксируются после окончательной проверки.

## Финальные результаты

После исправления портрета и ожидания теста: **95/95 Edit Mode**, job `98f7c61f3e9845be9a173a4c0fa8591e`; **52/52 Play Mode**, job `a6a85cfb4fe34604b31c23d3a44557a6`,0 failed/skipped. Компиляция завершена. После тестов Console0 ошибок/0 предупреждений; последнее чтение после восстановления редактора:0 ошибок и2 служебных предупреждения: MCP WebSocket is not initialised и Account API30 секунд. MCP продолжает отвечать. Ранее отдельно наблюдалось Account API warning.

SampleScene validate0issues/Missing Script/Broken Prefab, auto_repair=false. Сравнение YAML сцен с HEAD: SampleScene981/BattleScene230 объектов, без новых/удалённых/повторных ID. Все87 собственных C# имеют соответствующую документацию с9 разделами; .meta/централизованные literal GameText-ключи не потеряны.

Обычный финальный Play: исправленный портрет проверен [кадром Inventory](Images/AnimationV2-inventory-1920.png), обе модели —[картой](Images/AnimationV2-world-1920.png), боевой idle —[Full HD](Images/AnimationV2-battle-1920.png)/[wide](Images/AnimationV2-battle-3440.png), реальный windup героя —[Attack](Images/AnimationV2-hero-attack-1920.png). В обычном ходе Attack был принят: Skeleton HP60→50, затем вновь AwaitingPlayer/оба Idle/IsAnimating=false; скелет выбрал защиту, поэтому его удар на этом снимке не заявляется. Его attack подтверждён автоматическим сценарным тестом. [Границы/язык UI](Verification/CharacterAnimationLayout.json) после исправления портрета чистые.

Play Mode остановлен; активна сохранённая чистая SampleScene, Game View index0/7 presets и runInBackground=false восстановлены. Удалён только созданный для этой проверки временный wide preset. Runtime-позиции/выключение AI и временные Capture callbacks не сохранены в проект.

## Ограничения и ручная проверка

Техническое воспроизведение проверено, художественная полировка не объявляется завершённой. В некоторых сгенерированных ракурсах глубина шага/чередование ног может требовать правки художником; больше кадров само по себе не гарантирует естественность. Левые изображения зеркальные, поэтому стороны оружия/щита отражаются. World Idle статический; новые Hero Hurt/Death не рисовались, сохранены flash/fade с battle idle fallback. Hero Attack универсальный меч и не подменяется по экипированному Staff/оружию. Отдельные8 ракурсов скелета, оружейные наборы/статусы/новые способности — не реализованы.

Высокое разрешение увеличивает память:126 отдельных Sprite без потерь занимают приблизительно150 МБ GPU RGBA без mipmaps, помимо исходников редактора. Это оценка размеров, не профилирование сборки; SpriteAtlas/оптимизация загрузки и standalone-проверка — отдельные задачи. Разные PPU сохраняют рост персонажа; крупный холст не меняет скорость/коллайдеры. Старое окружение этим заданием не перерисовывалось.

В SampleScene включить Play:

1. Пройти WASD/стрелками в8 направлениях; оценить переходы, шаг/руки/плащ, отсутствие рывков корпуса и сохранение взгляда после остановки. Посмотреть подходящего скелета: видны ли ноги, не мерцает ли Idle между шагами, согласованы ли размеры?
2. Нажать I: портрет должен сохранить прежний размер/дизайн, здоровье и экипировка прежние.
3. Коснуться врага: оба idle живые; Attack показывает подготовку/удар/возврат, затем ответ скелета и следующий ход. Проверить Fire/Heal/Defend, ускорение Space, победу/возврат. При отдельном запуске проверить поражение.
4. Оценить Full HD и широкий экран, особенно читаемость меча/щита, общий стиль и темп.

## Roadmap и Git

Следующий общий подэтап —5.3 Warrior/Mage/Rogue;5.1/5.2/инвентарь не повторяются. Перед ним новая художественная приёмка ходьбы/удара: задача технично Tested, художественно In Progress. Код классов/способностей/дискового сохранения не выдаётся за реализованный.

Ветка `stas_megamosk_build`. Локальный коммит сохраняет только эту интеграцию/кадры/профили/сцены/тесты/Docs; push не выполняется. Посторонний LiberationSans SDF - Fallback.asset остаётся вне коммита. Исходные Player/Enemy Transform/Rigidbody/Collider и остальные Prefab/пакеты/настройки сохранены; временные параметры проверки восстанавливаются.

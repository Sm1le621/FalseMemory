# Утверждённый герой: импорт и анимации

## Результат2026-10-13

Владелец утвердил ранее созданный лист1983×793 и подтвердил прежние изменения Player/Inventory/Equipment. Жёсткое правило64×64 отменено в AGENTS.md. Предоставленные четыре фронтальных изображения действительно используются в существующем Player и Walk_Down; качество исходника сохранено без уменьшения. Остальные системы не переписывались.

## Источники и подготовка

`Assets/Art/Characters/Player/ApprovedHero/ApprovedHeroSource.png` — байтовая копия ранее созданного утверждённого PNG. `DirectionalSource.png` — новый результат built-in imagegen1122×1402. Старые ASE/ASEPRITE/анимации сохранены; исходники остаются отдельно от производных листов.

Фронт: четыре512×640 кадра, исходные интервалы X по496, Y83..723, сдвиг X+8; никаких resampling/stretch. Исходный alpha уже прозрачный. Убраны только почти невидимые остатки alpha≤16 в производных кадрах; чёрная броня не вырезалась. PPU1100/pivot(256,40) дают прежнюю видимую высоту около1.04 единицы при Scale Y2.

Дополнительный инструмент создал реальные изображения Up/Right/UpRight/DownRight. Разделение рядов по фактическим промежуткам Y0/347/697/1041/1402, а не равной сетке. Кадры384×384 выровнены по оси головы/тела и линии стоп, без изменения размеров пикселей; PPU640/pivot(192,20). Left/UpLeft/DownLeft механически зеркальны: сторона оружия отражается. Это не отдельная генерация левых ракурсов. Up имеет самостоятельный вид спины.

[Механическая подготовка](Verification/ApprovedHeroPreparation.json) сохраняет прямоугольники/смещения; [PNG audit](Verification/ApprovedHeroArt.json) подтверждает байтовое сохранение источников, неизменность исходных фронтальных пикселей кроме невидимого alpha,32 разных RGBA-кадра, общую линию стоп, отсутствие обрезания видимых краёв. Разные пиксели не доказывают естественный шаг: дополнительные четыре направления являются художественными черновиками, особенно боковая постановка ног требует оценки.

## Генерация и воспроизводимость

Применён навык imagegen, встроенный image_gen, не CLI/API. Источник: ранее утверждённый PNG `exec-b9063f99-7b33-44e2-b42b-d6b15b104351.png`; новый файл `exec-4e04007d-621c-4b7e-8ce6-d71e785dd839.png` в каталоге `.codex/generated_images/01a12829-b4db-7f73-8433-82a4503052aa`. Оба скопированы в проект и оригиналы сохранены.

Prompt: identity-preserve detailed Dark Fantasy Pixel Art hero; preserve black detailed armor/dark hair/serious face/burgundy cape/proportions/lighting/pixel density; transparent four-row/four-column sheet; row1 genuine rear Up, row2 right profile, row3 rear-right, row4 front-right; four alternating walk poses per row, shared feet/center, no background/text/blur/cropping/redesign. Requested approximate2048×2560; actual1122×1402 accepted technically because size no longer fixed. Качество/точность дополнительных ракурсов не объявляется одобренной владельцем.

## Изменения Unity и кода

- Существующие16 Player64 Idle/Walk-клипов обновлены с сохранением GUID и прежнего Animator Controller/параметров. Walk8FPS/.5s/Loop/4 Sprite, Idle один кадр каждого направления. Название папки Player64 историческое.
- SampleScene: ссылки SpriteRenderer, BattleUnit.battleSprite и InventoryWindow.characterPortrait заменены на ApprovedHero_Down_00. Tint мира/общего BattleUnit/портрета белый, чтобы сохранить утверждённые цвета.
- BattleScene: previewPlayerSprite/Image.sprite и preview tint обновлены. Обе сцены фактически сохранены через EditorSceneManager/MCP.
- Sprite import Multiple/Point/Uncompressed/no mipmaps/NPOT None/Max Size4096/Full Rect/ground pivot. Отдельные источники Default, в клипы не включены.
- Новый Editor-only [ApprovedHeroImporter](Scripts/Editor/ApprovedHeroImporter.md) выполняет адресный импорт/привязку; игровые PlayerController/Health/BattleManager/Inventory-код не менялся.
- Новый [ApprovedHeroAssetTests](Scripts/Tests/EditMode/ApprovedHeroAssetTests.md) проверяет ресурсы/клипы. Изменён [PlayerDirectionFlowTests](Scripts/Tests/PlayMode/PlayerDirectionFlowTests.md): реальный directional Idle и смена всех четырёх Down-кадров вместо статического fallback.

Player Transform(0,0,0), Scale(2,2,1), CapsuleCollider2D size(.43966949,.64329064)/offset(-.0101878,.2912171), камера ortho5 и MovementSpeed4 сохранены. Collider по-прежнему охватывает больше, чем видимый узкий торс: это существующий игровой объём, не точная маска брони/плаща. Если контакт ощущается ранним, нужна отдельная согласованная настройка физики после ручной проверки.

## Проверки

Первый полный прогон:92/92 Edit Mode, job `78e60883097e417ca2fedde7fab498fb`;50/50 Play Mode, job `82357ff8b4eb45dd93359bec47e11fad`, без failed/skipped. Проверены импорт,8 нормализованных направлений/Idle/WorldInputGate, фактическая смена четырёх Down-поз, неизменная скорость, боевой контакт/переход/команды/VFX/возврат, инвентарь/экипировка/характеристики.

Обычный Play Mode: карта и Inventory с новым героем; физический контакт подготовлен размещением героя рядом с текущим врагом, вызвал BattleScene. На игровых экранах1920×1080 и3440×1440 у Inventory/Battle нет UI вне экрана, переполнений TMP, русского текста и отсутствующих glyphs. На карте1920×1080 те же проверки чистые. Это геометрическая проверка и просмотр кадров, не полная ручная приёмка. Fire принят через обычный BattleManager: MP30→22, Skeleton HP60→41; случайные боевые исходы не подменяют детерминированные тесты.

Во время импорта исправлено обращение к Full Rect через отсутствующий TextureImporter.spriteMeshType: используется TextureImporterSettings. После исправления компиляция успешна. При обычном Play наблюдалось служебное предупреждение Unity Account API30 секунд; оно не является ошибкой игровых скриптов.

Финальный прогон после белого tint: **92/92 Edit Mode**, job `a5522309a7b946e98feeab836388f9d5`; **50/50 Play Mode**, job `2affdcda9dc94180b42bd9b9ba0a1db4`,0 failed/skipped. После тестов Console была чистой. Последнее чтение после восстановления редактора:0 ошибок,2 служебных предупреждения — Account API30 секунд и MCP WebSocket is not initialised. MCP продолжает отвечать; игровые тесты прошли. SampleScene validate0issues; сравнение сериализованных сцен с HEAD: SampleScene981 объектов/BattleScene230, без добавленных/удалённых/повторных ID. Все84 собственных C# имеют отдельные9-раздельные документы; отсутствующих .meta и literal GameText-ключей нет.

После финального tint повторены геометрические проверки Inventory/Battle×2 разрешения: [результаты](Verification/ApprovedHeroLayout.json), вне экрана/overflow/русский/пропавшие glyphs —0. Просмотрены сохранённые кадры [карты](Images/ApprovedHero-world-1920.png), [инвентаря](Images/ApprovedHero-inventory-1920.png), [боя Full HD](Images/ApprovedHero-battle-1920.png) и [широкого боя](Images/ApprovedHero-battle-3440.png). Play Mode остановлен, активна чистая SampleScene; исходные Game View index0/7 presets и runInBackground=false восстановлены. Удалён только созданный для проверки временный preset3440×1440.

## Ручная граница и следующий этап

Откройте SampleScene и включите Play:

1. Пройдите WASD/стрелками во всех8 направлениях и по диагонали. Оцените шаг, плащ, отсутствие заметного сдвига тела/скачка роста и сохранение взгляда после остановки. Боковые/диагональные циклы пока черновые: есть ли ощущение скольжения вместо переступания?
2. Посмотрите размер относительно дороги/скелета, момент столкновения и камеру. В инвентаре I оцените цвета/границы портрета.
3. Коснитесь скелета: проверьте внешний вид героя в бою, Attack/Fire/Heal, возвращение на карту. По возможности повторите в Full HD и широком Game View.

Idle пока статический, левые виды зеркальны, новые Attack/Hurt/Death-кадры игрока не создавались. Standalone-сборка/полная художественная приёмка/дисковые сохранения не проверены. Новые окружение/NPC/VFX этим заданием не создавались: для будущей графики действуют новые правила.

По Roadmap следующий программный подэтап5.3 — классы Warrior/Mage/Rogue. Прежняя ручная граница пройдена по сообщению владельца; перед5.3 остановка относится именно к новой походке/ракурсам, а не к повторному тестированию старого инвентаря.

## Git и документация

Ветка `stas_megamosk_build`; завершённый технический подэтап сохраняется отдельным локальным коммитом `feat: integrate approved player walking sprites`. Push запрещён без отдельной команды. Посторонний LiberationSans SDF - Fallback.asset остаётся вне коммита. Обновлены AGENTS, PixelArtStyleGuide, AnimationSystem, Architecture, README, Roadmap, Changelog, PlayerRework, инструкции Unity/проверок и документы всех трёх затронутых C#.

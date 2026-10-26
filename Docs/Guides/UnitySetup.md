# Настройка существующего проекта в Unity

## Проверки 5.7 —2026-10-26

Дляприёмкибез C#:SampleScene→Play→FalseMemory/Testing/Grant 60 Levels (Play Mode Only)→C. Командадаётреальный EXP тольковтекущейсессии,неизменяетсцену/ассеты/HP/MP напрямую. После StopPlay всёсбросится. Подробныйсценарий —[5.7](../ClasslessStage57.md).

## Character5.6 —2026-10-26

В SampleScene Player.CharacterWindow связан сновыми CharacterPanel/Backdrop/Tooltip под InventoryCanvas. C открывает/закрывает,Escзакрывает,IпереключаетвInventory. Подробности ссылок —[CharacterUISystem](../Systems/CharacterUISystem.md).

## Изучение способностей5.5 —2026-10-26

На Player добавлен AbilityLearning, catalog содержит Fire/Heal/PowerStrike/QuickStrike/GreaterHeal/SacredStrike. Условия и экономика — [AbilityLearningSystem](../Systems/AbilityLearningSystem.md). Изучение пока через API; окно Character подключается следующим подэтапом.

## Подэтап5.4 —2026-10-26

SampleScene содержит6 новыхPickup_Scaling_*Armor подTestLoot, Enemy категорииUndead, PlayerHuman. DefaultBattleRules useDiminishingDefense/K20; DefaultCharacterAttributesholycurve. Настройкаипримеры —[ClasslessStage54](../ClasslessStage54.md). НезапускатьEditorsetupвPlay; старыйконтентнезаменён.

## Подэтап5.3 —бесклассовая прокачка

Существующий Player.CharacterAttributes использует обновлённый DefaultCharacterAttributes: classless=true, points1/level, MP40, soft caps, без AGI movement/автороста10 stats. Новые6 weapon assets/pickups созданы адресно через MCP/ClasslessProgressionSetup.ApplyStage53 вне Play на SampleScene; прежние ItemData и объекты сохранены. Для нового оружия обязателен useWeaponScaling=true, одного вложенного поля weapon недостаточно. Команда настройки повторно применяет баланс своих тестовых предметов; не запускать автоматически при каждом Refresh. [Подключение/ограничения](../ClasslessStage53.md).

## Подключение Cardinal4 —2026-10-26

В существующей SampleScene уже сохранены Player preview Idle_Down_00 и Use Cardinal Animations=true. Player использует прежний Player.controller: два состояния/Blend Tree,4 child Down/Left/Right/Up, WalkSpeed только у Walk. Для повторного импорта вне Play: FalseMemory → Art → Import approved hero animation sheets; затем ApprovedHeroImporter.WireActivePlayer только на SampleScene. Не назначать Source PNG как игровой кадр, не менять Rigidbody/scale/скорость4. Активный Idle выбирается из PlayerCardinal4Manifest.frames по motion/direction/index0…3; DownStanding заменяет старый Idle/Down без удаления его файлов. Порядок Walk читается из PlayerCardinal4Manifest.walkSequences; ручная сортировка папки архивного Walk его не заменяет. [Полные параметры/проверки](../PlayerWalkOrder.md). Ниже исторические настройки.

## Действующие настройки героя2026-10-16

Использовать те же SampleScene/Player/Player.controller и16 Player64 Idle/Walk клипов. ApprovedHeroImporter.ImportAnimations вне Play воспроизводит16 FPS/ключи; DetailedAnimationImporter.ImportAll сохраняет HeroDetailed16/Idle16/useFrameTiming=true и отдельные SkeletonDetailed12/Idle4/useFrameTiming=false. Никаких новых сцен/контроллеров. World PNG/PPU/pivot/scale2/скорость4/Interpolate/камера5 сохранены. Отклонённые sheets из Docs не назначать Sprite-клипам.

В BattleScene прежний BattlePresentation читает длительности opt-in HeroDetailed:8 Attack=.5 с, контакт4=.25 с; прежний attackDuration остаётся для остальных действий/профилей. Глобальный Animator.speed и timeScale детей1. Обычный WalkSpeed масштабируется фактической скоростью/reference4. [Проверки/ручная граница](../PlayerAnimation16.md). Ниже прежние настройки для истории.

## Idle_Down30 —2026-10-13

Существующий Player SampleScene/Player.controller/Idle_Down подключён к Animation30/Idle/Down; SpriteRenderer preview использует первый Idle. Импорт: подготовить ровно30 PNG и PlayerAnimation30Manifest, вызвать ApprovedHeroImporter.ImportAnimations вне Play. Canvas/pivot/PPU берутся из manifest; source PNG Default. Неполный набор вызывает исключение, отклонённые Walk sheets из Docs не назначать. Runtime/коллайдеры/камера/скорость4 не изменялись. [Ручной сценарий](../PlayerAnimation30.md). Ниже прежняя настройка.

## Animation Rework2026-10-13

В сохранённой SampleScene Player/Enemy Rigidbody2D: Interpolate/Continuous, gravity0 и FreezeRotation. Player Move Speed4/Acceleration48/Deceleration80/Animation Reference Speed4; Enemy Move Speed3/Acceleration48 и EnemySpriteAnimation reference3. Player.controller дополнен только float WalkSpeed(default1) на существующем Walk-состоянии. ApprovedHeroImporter обновляет12FPS-клипы/этот параметр без нового controller; DetailedAnimationImporter согласован с12FPS. BattleTransition сам приостанавливает simulation; вручную выключать его навсегда не нужно. Новые Draft в Docs не назначать вместо Sprite. [Инструкция проверки](../AnimationRework.md). Ниже более ранняя настройка.

## Подключение CharacterAnimationUpgrade

Использовать существующие SampleScene/BattleScene и[DetailedAnimationImporter](../Scripts/Editor/DetailedAnimationImporter.md). Player/Enemy→BattleUnit.visualAnimations назначены HeroDetailed/SkeletonDetailed. EnemySpriteAnimation уже присутствовал; нового Animator врага не требуется. InventoryWindow.portraitSprite сохраняет прежний утверждённый фронтальный портрет. Не менять moveSpeed/Collider/Scale ради текстур.

## Утверждённый герой —2026-10-13

Открыть существующую SampleScene: Player использует прежний Animator и новые16 клипов с ApprovedHero. Настройки импортёра и PPU —[PixelArtStyleGuide](PixelArtStyleGuide.md), адресный инструмент —[ApprovedHeroImporter](../Scripts/Editor/ApprovedHeroImporter.md). Source PNG/старыеASE сохранены; не менять Scale/moveSpeed ради разрешения. BattleScene сохранена с тем же Down-портретом.

## Актуально2026-10-13

SampleScene содержит9equipmentButtons сemptySprite ипрежний30bag. ВTestLoot добавленPickup_IronShield (−3.4,−3.5), его ItemData —Assets/Settings/Inventory/IronShield.asset. Слоты54×54/шаг80 избегаютпересеченияподписей. НаProfile/Enemy/Camera/Rigidbody другихизмененийнет. [Настройкапереносов](../Systems/EquipmentDragSystem.md).

## Player rework4 — актуальная настройка

На исходном Player SampleScene назначены CharacterAttributes/DefaultCharacterAttributes и прежний DefaultCharacterGrowth. Не складывать legacy growth с primary: BattleUnit сам выбирает источник. InventoryWindow содержит10 AttributeRowView, общий прежний tooltip и компактную сводку. Scene сохранена через MCP. Enemy без primary, исходные коллайдеры/камера сохранены. [Система](../Systems/CharacterStatsSystem.md).

5.1: Player.BattleUnit.growthRules=Assets/Settings/Progression/DefaultCharacterGrowth.asset; Enemy без профиля. Кривая/рост настраиваются в этом ассете вне Play. Health.maxHealth/Unit.maxMP — стартовая база, CurrentExperience — накопленный EXP. [ProgressionSystem](../Systems/ProgressionSystem.md).

Native pixel art: [EnemyVisuals](../Systems/EnemyVisuals.md). Enemy→BattleUnit.visualAnimations=Skeleton64, SpriteRenderer/battleSprite=Idle-00; EnemySpriteAnimation рядом с существующими AI/Collider/Rigidbody. BattleUI.impactSprite=Hit64/Impact; FireVisual/HealVisual.frames имеют8 кадров. Новые текстуры64×64, Point, Uncompressed, без mipmaps, PPU100. Старый skeleton-topdown-64 сохранён.

Открывай корень FalseMemory, содержащий Assets, Packages и ProjectSettings. Текущая проверенная версия Editor —6000.6.2f1. Новую сцену для проверки боя создавать не нужно.

## Сцены

В Build Profiles → Scene List должны быть включены MainMenu, SampleScene и BattleScene. MainMenu запускает новую игру; SampleScene содержит мир и участников; BattleScene загружается дополнительно координатором BattleTransition. Прямой Play в BattleScene без встречи показывает вспомогательное меню, а не полноценный бой.

## Player в SampleScene

- PlayerController: Rigidbody2D, физический Collider2D, SpriteRenderer; Animator необязателен, bool isMoving нужен для ходьбы.
- Тег Player; Health с Max Health100 и ссылками мирового Slider/HPText. Slider не интерактивен.
- BattleUnit: Level1, EXP0, Max MP30, MATK12/MDEF8, Fire и Heal в Spells; crit0.1/miss0.05.
- BattleInventory: одна стопка Definition=Potion, Count3. Количество хранится здесь, не в общем SO.

Current Health и Current MP до первого запуска не задают сохранённый прогресс: Start/Awake заполняют их максимумами. Для временных тестов меняй текущие значения только в Play Mode.

## Enemy и переход

Enemy содержит Rigidbody2D, триггер обнаружения для EnemyAI и физический Collider2D тела для BattleEncounter, Health60, BattleUnit ATK12/DEF5/SPD7/MDEF3/Reward20. Старый EnemyAttack выключен; наличие Encounter также исключает его контактный урон.

Единственный BattleTransition имеет Rules=DefaultBattleRules и имена BattleScene/MainMenu. Он сохраняет исходные объекты мира; копировать Player/Enemy в BattleScene не нужно. CameraFollow мировой камеры ссылается на Player.

## Боевой UI

BattleManager.ui ссылается на BattleUI. В UI назначены сообщения, характеристики, раунд, портреты, пять кнопок/пять подписей, AbilityGlow/AbilityAmount и BattleResults/VictoryResultText. BattleHUD, BattlePresentation, BattleVFXController и BattleAudioController подключены к прежнему BattleManager. HUD содержит три неинтерактивные полосы, группы проявления и пять commandIcons. Портреты имеют pivot=(0.5,0.5), масштаб отражает спрайт без перемещения. Canvas Scale With Screen Size960×540, Match Height=1, Pixel Perfect=true; центральный BattleContent шириной960. Команды используют BattlePixelFont с fallback для кириллицы. Сохранённое оформление уже построено: не вызывай BuildLayout повторно.

Для Fire/Heal назначены FireVisual/HealVisual в поле visualEffect заклинаний; длительность0.45, частицы12. В AudioController назначен DefaultBattleAudio и существующий AudioSource: Spatial Blend0, Ignore Listener Volume=false. BattleMixer/Master подключён, все AudioClip пока пусты — назначайте подходящие звуки в профиль. Общая громкость меню сохраняет прежнее управление AudioListener.volume.
Presentation: Attack0.4, Hit0.18, Outcome0.35, Message0.75, Enemy Think0.5, Allow Fast Forward=true, множитель3. Space ускоряет движения/паузы, не игровой урон; отключить можно в Inspector. DefaultBattleRules.actionDelay остаётся0.8, fadeDuration назначено0.5; стоимость заклинаний и параметры участников сохраняются.
## Инвентарь мира и подбор
На Player общий BattleInventory, InventoryWindow, WorldInputGate и PlayerInteractor. InventoryCanvas содержит30 ячеек/описания и две мировые подсказки; I/Escape закрывают окно. Количество менять через State, стартовые seed только до инициализации. TestLoot слева/ниже старта содержит семь предметов из WorldItemPickup prefab; триггеры/иконки/количества/уникальные worldId назначены MCP. Подойти и нажать E; при нехватке места вещь остаётся. Для новых размещений задайте уникальный ID, не перемещайте существующие кусты.
## Экипировка
На Player EquipmentController, семь слотов окна и итоговые Stats. Базовые ATK/DEF остаются15/8; вещи добавляют бонусы через Effective*. Выбрать → Экипировать, либо подходящий правый слот. Клик надетого слота при пустом/несовместимом выборе снимает вещь; полная сумка безопасно отклоняет снятие. ItemData задаёт Fits/requiredLevel/bonuses.
## Использование предметов
WorldItemUseController уже на Player. Выбрать Potion/Ether → Использовать; общий эффект с боем, ограничения ItemData.useLocations/requiredLevel. Potion до30HP, Ether до20MP, расход1; даже полный ресурс допускает расход с amount0 как прежний Potion. В бою Items/Ether — один ход, голубой профиль EtherVisual. Изменяемое количество в State, не SO.
## Оформление и отладка полного запаса
InventoryWindow resourcesLabel/modalBackdrop назначены. Шапка читает HPMP; фон закрывает старый HUD только при открытии и скрывается при закрытии. PlayerInteractor очищает уведомления при gate. Pickup_BoneStressTest по умолчанию inactive, только отдельный ручной Play-тест;2871 Bone заполняют29 стопок99 плюс стартовый Potion3. Уникальный worldId задан, не включать его в сохранённой сцене для обычной игры.
## Добавление данных

Create → FalseMemory → Battle Spell создаёт заклинание с поддерживаемым эффектом; назначь его в Spells игрока. Battle Consumable создаёт лечебный предмет; добавь его определение в стопку Inventory. Battle Rules — общие коэффициенты. Новые типы эффектов требуют программирования; изменение чисел существующего эффекта — настройки данных.

## Unity MCP

Выбирай открытый FalseMemory по списку instances, читай editor/state и Console. Если Editor компилирует, дождись завершения перед назначением новых компонентов. Ошибка соединения означает недоступность инструмента, а не успешное выполнение команды. При нескольких Editor явно выбирай экземпляр. Сцены после необходимых изменений сохраняются через MCP; чтение сцены не равно сохранению.

Подробнее: [архитектура](../Architecture.md), [документация компонентов](../README.md#описания-скриптов), [тесты](TestingGuide.md).

## Переработанный инвентарь

InventoryWindow.BuildFantasyLayout через Unity MCP переиспользует сохранённые объекты SampleScene. InventoryCanvas: reference1200×780/Expand/PixelPerfect/sorting60; InventoryPanel1140×720; слева CharacterPanel/PortraitFrame/CharacterPortrait/CharacterName, вокруг восемь Equipment-кнопок с EquipmentSlotView; справа BagPanel/BagTitle/прежние30 Slot. Tooltip-компонент на том же Canvas, отдельный child InventoryTooltip с TMP Content (raycast=false). Все ссылки сохранены, окно и tooltip закрыты вне игры. Исходный герой/импорт не менялись.

ItemData → rarity определяет только цвет; Gloves=Armor с equipmentSlot Gloves, maxStack автоматически1. В наборе пока нет такой вещи, слот пустой. Не менять enum-индексы прежних аксессуаров. Настройки/ручная проверка в [InventoryRedesign](../InventoryRedesign.md).


## English и редактирование текста

Игровой интерфейс English en-US. Unity → FalseMemory → Localization → English Text: поиск ключа, правка Value, Save table; новый ключ через Add entry. Таблица Resources/Localization/GameText_en-US и SharedData рядом,136 записей. Static TMP получает LocalizedLabel/key; динамические поля управляются прежними контроллерами через GameText. ItemData id+.name/.description; Unit/Spell localizationId назначены в сцене/ассетах. PixelUi.extraPadding=false для bitmap-атласа, исходный PressStart2P не меняется. Полная инструкция [LocalizationSystem](../Systems/LocalizationSystem.md).

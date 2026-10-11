# Настройка существующего проекта в Unity

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

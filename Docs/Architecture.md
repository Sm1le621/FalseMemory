# Архитектура FalseMemory

Состояние прочитанного кода и проверенных ссылок на 2026-10-11. Существующая реализация этапа 2 повторно проверена и сохраняется в локальном Git вместе со сценами, ассетами, тестами и документацией. Новые игровые системы в задаче главного промпта повторно не создавались.

## Структура проекта

- `Assets/Scripts/` — движение игрока, камера, здоровье и поведение мира.
- `Assets/Scripts/Battle/` — менеджер и интерфейс боя, очередь/расчёты, переходы, данные магии/предметов, ресурсы участника.
- `Assets/Scripts/UI/` — главное меню, настройки и строительные функции UI.
- `Assets/Scenes/` — MainMenu, SampleScene, BattleScene.
- `Assets/Settings/Battle/` — DefaultBattleRules, Spells/Fire и Heal, Items/Potion.
- `Assets/Prefabs/` — существующие заготовки карты/кустов; текущие Player/Enemy проверяются как объекты SampleScene. Prefabs в задаче документации не изменялись.
- `Assets/Resources/Fonts/` — источник шрифта главного меню; `Assets/TextMesh Pro/` — ресурсы TMP.
- `Assets/Tests/EditMode/` и `PlayMode/` — расчёты и интеграционные проверки.
- `Assets/Welcome/` — служебный шаблон Unity 2D.
- `Packages/` и `ProjectSettings/` — зависимости и настройки Unity, сохраняются Git.
- `Docs/` — документация; `AGENTS.md` — постоянные правила работы.
- `Docs/Guides/` — инструкции настройки Unity, локальных коммитов и тестирования.
- `Library/`, `Temp/`, `Obj/`, `Logs/`, `Builds/`, `.vs/` — генерируемые/локальные данные, исключаются `.gitignore`.

## Сцены и ответственность

**MainMenu:** MainMenuBootstrap создаёт UI во время Awake; MainMenuController запускает SampleScene, открывает SettingsMenu или завершает приложение. PixelUi создаёт элементы. SettingsMenu хранит громкость/fullscreen в PlayerPrefs.

**SampleScene:** Player имеет PlayerController, Health, BattleUnit, BattleInventory и физические/визуальные компоненты. Enemy имеет EnemyAI, Health, BattleUnit, BattleEncounter; старый EnemyAttack выключен и дополнительно защищён от контактного урона наличием Encounter. Main Camera использует CameraFollow. Grid/Borders/Bushs — окружение. Мировой HUD находится под root `Health` в текущем прочитанном Hierarchy; его ссылки HP Slider/HPText назначены Player.Health. BattleTransition — отдельный координатор.

**BattleScene:** собственные камера/освещение/EventSystem, BattleManager и BattleUI с сохранённым Canvas. Это отображение и управление существующими участниками мира, без создания копии здоровья игрока. BattleHUD отдельно оформляет компактные характеристики, полосы и меню; не меняет ресурсы. Canvas боя использует Scale With Screen Size, 960×540, match 1; BattleContent шириной 960 центрирован на широком экране.

В текущем списке сборки доступны MainMenu, SampleScene и BattleScene. Физическая встреча запускает бой; прямой запуск BattleScene без активной встречи показывает вспомогательное меню исследования.

## Владельцы данных
- EquipmentController.State — семь надетых ItemData, атомарный обмен с InventoryState. BattleUnit.Effective* читает базу+сумму заново, базовые поля не изменяются/ресурсы не восстанавливаются. InventoryWindow даёт команды; это не LevelUp/классы/HPMP-бонусы.

- PlayerInteractor/IWorldInteractable — один вход E/поиск ближайшей физической цели. WorldItemPickup резервирует выдачу в общий State и выключает только себя; prefab/семь тестовых instance TestLoot сохраняются в SampleScene. Это не SaveSystem/NPC.

- InventoryWindow/InventorySlotView —30 readonly views, событийное обновление, pointer drag. InventoryCanvas сохранён в SampleScene, не отдельная система данных.
- WorldInputGate — владельцы модального ввода; PlayerController проверяет gate, время и enabled не меняются.

- ItemData — только статические определения; прежний BattleConsumable наследует их без замены Potion/GUID.
- BattleInventory.State — один InventoryState30 слотов и Gold на исходном Player. Старый items/новый startingItems только seed; модель создаётся один раз. UI/бой не копируют количество. Код: Assets/Scripts/Inventory, данные Assets/Settings/Inventory. Сохранение/внешняя десериализация ещё не подключены.


- **BattleScreenFade:** временный Canvas, единственная часть перехода, переживающая Single-загрузку до проявления; не хранит RPG-состояние. HUD/Presentation отображают вход/исход, Transition управляет сценами.

- **BattleAudioProfile/Controller:** ссылки клипов и единственный AudioSource боя, output BattleMixer/Master. Общая громкость остаётся в SettingsMenu/AudioListener.

- **BattleVFXProfile/Controller:** данные и воспроизведение эффектов в UI; ссылки Spell.visualEffect не участвуют в игровой формуле. Пул освобождается при выгрузке.

- **BattlePresentation:** только позы/цвета существующих Image и символы защиты; Windup/Recover не записывают ресурсы. Manager остаётся владельцем единственного применения действия.

- **Health:** maxHealth/currentHealth и обновление привязанного мирового HUD. BattleUnit и UI используют тот же компонент.
- **BattleUnit:** физические/магические характеристики, текущие MP/EXP, Level, флаг Defend и внешний вид боя. Awake заполняет MP один раз. Level не растёт автоматически.
- **BattleInventory:** количества расходуемых предметов на объекте игрока; дефиниции предметов находятся в SO.
- **ScriptableObject:** BattleRules — коэффициенты, BattleSpell — цена/эффект/цель/мощность, BattleConsumable — лечебный эффект/цель. Это общие определения, не индивидуальный расходуемый запас.
- **BattleManager:** фаза и очередь текущего боя, выбранное действие и блокировка ввода.
- **BattleTransition:** ссылки активной встречи, состояние перехода, сохранённая активность мировых объектов, отметка выдачи награды и защитный таймер.

## Последовательность боя

1. EnemyAI преследует Player, обнаруженный триггером.
2. BattleEncounter получает физическое касание тел и вызывает BattleTransition.TryBegin.
3. Transition синхронно блокирует повторный старт, останавливает физические скорости и выключает контроллеры. Скрывает мир и загружает BattleScene в Additive.
4. Manager получает те же Player/Enemy, строит каждый раунд по Speed. Игрок выбирает, враг вызывает EnemyBattleAI только в свой ход.
5. Calculator возвращает урон/исход, Abilities списывает MP/предмет и обращается к Health; UI отображает результат. Gate защищает от повторной команды.
6. После победы Transition выдаёт EXP один раз; UI показывает результат. Возврат выгружает BattleScene и отключает побеждённого врага.
7. После побега возврат автоматический: враг остаётся жив, игрок по возможности перемещается на свободное место, защита от встречи действует 3 секунды.
8. После поражения новая игра/меню загружаются Single и намеренно сбрасывают текущую сессию.

## Сборки и зависимости

`FalseMemory.Runtime.asmdef` объединяет Assets/Scripts и ссылается на Unity.InputSystem, Unity.TextMeshPro, UnityEngine.UI. Edit Mode и Play Mode имеют отдельные тестовые asmdef со ссылкой на Runtime; Edit Mode ограничен платформой Editor. Welcome имеет собственную сборку Unity.2D.Welcome.

Установлены Unity Input System, TMP/uGUI, URP, 2D-инструменты, Unity Test Framework и Unity MCP. Точные версии фиксируются manifest/package lock проекта; новые пакеты здесь не добавлялись. Элементы UnityEditor в игровых файлах ограждены UNITY_EDITOR, но отдельная сборка в этой задаче не проверялась.

## Проверенное и известные границы

Историческая проверка этапа2: 40 Edit Mode и 17 Play Mode. Актуальная полировка этапа3 описана в BattleStage3.md: дополнительные визуальные компоненты читают прежние ресурсы, Manager остаётся единственным местом применения действия. Space меняет только VisualDelta/паузу представления. Ссылки и отсутствие Missing Script проверяются через MCP. Тесты используют настоящий SceneManager, физический контакт и синтетический Input System. Отдельные нажатия кнопок тестируют onClick напрямую; это не полная симуляция курсора.

Механики боёв проверены сценариями, но не весь мир RPG. Враг не обходит стены; камера без ограничений карты; Health напрямую зависит от UI; клавиатура не переназначается и gamepad не добавлен. Бой один на один, только два типа магических эффектов и лечебные предметы. Экипировка этапа4 имеет семь слотов и пять бонусов. Нет партий, уровней развития, статусов, заданий, диалогов и сохранений RPG на диск. Данные сессии сохраняются, пока исходная карта загружена.

Этап 3.6: новая проверка пяти состояний на четырёх разрешениях, включая 3440×1440; отчёт и измерения в BattleStage3.md и Verification/BattleStage3Layout.json. BattlePixelFont использует существующий PressStart2P для латинских команд и Liberation Sans как fallback для русского текста. Пять собственных пиксельных значков хранятся в Assets/Settings/Battle/UI; импорт исходных персонажей и мировая камера не менялись. Громкость/fullscreen/Exit и самостоятельная сборка здесь не проверялись.

Подробности: [системы и скрипты](README.md), [проверки](Testing.md), [отчёт этапа 2](BattleStage2.md).

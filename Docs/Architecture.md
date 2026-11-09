# Архитектура FalseMemory

Актуально на **2026-11-09**. Документ описывает текущую рабочую копию: упрощённый инвентарь без золота и цен, бой без EXP и вкладки Items, английский текст прямо в сценах и сценовую композицию UI. C#-код пишется простым стилем «уровня 3 курса» (см. `AGENTS.md`, Code Style Rules).

## Структура проекта

- `Assets/Scripts/` — движение игрока, камера, здоровье, поведение мира, вход в бой.
- `Assets/Scripts/Battle/` — менеджер и интерфейс боя, очередь и расчёты, переходы, магия, предметы, ресурсы участника, визуальные профили.
- `Assets/Scripts/Inventory/` — общая модель инвентаря, предметы, экипировка, окно, подсказка и использование предметов.
- `Assets/Scripts/UI/` — главное меню, настройки и мировой HUD маны.
- `Assets/Scenes/` — `MainMenu`, `SampleScene`, `BattleScene`.
- `Assets/Settings/Battle/` — `DefaultBattleRules`, `Fire`, `Potion`, визуальные профили и кадры.
- `Assets/Settings/Inventory/` — `IronSword`, `LeatherArmor`.
- `Assets/sword.ase`, `Assets/armor.ase`, `Assets/potion.ase` — исходники иконок предметов (32×32, Aseprite importer, Point, PPU 100).
- `Assets/Tests/EditMode/` и `Assets/Tests/PlayMode/` — расчёты и интеграционные проверки.
- `Assets/Editor/` — Editor-only инструмент сборки пиксельных боевых ассетов.
- `Docs/` — документация; `AGENTS.md` — постоянные правила.
- `Library/`, `Temp/`, `Obj/`, `Logs/`, `Builds/`, `.vs/` — генерируемые данные, исключены `.gitignore`.

## Сцены и ответственность

**MainMenu** содержит сохранённые Canvas, фон, три кнопки, панель настроек и EventSystem. `MainMenuController` запускает `SampleScene`, открывает настройки или завершает приложение. `SettingsMenu` хранит громкость и fullscreen в `PlayerPrefs`.

**SampleScene** — мир. `Player` имеет `PlayerController`, `Health`, `BattleUnit`, `BattleInventory`, `InventoryWindow`, `EquipmentController`, `WorldItemUseController`, `WorldInputGate`, физические и визуальные компоненты. Animator Player использует `Assets/Player.controller`; в текущей рабочей копии контроллер содержит `Player_Idle`/`Player_Walk`/`Player_Run` и bool `isMoving`, а `PlayerController` выставляет этот параметр при движении. `Enemy` имеет `EnemyAI`, `Health`, `BattleUnit`, `BattleEncounter`; старый `EnemyAttack` выключен. Камера использует `CameraFollow`. Мировой HUD (`Health`) содержит HP-полосу и добавленную MP-полосу `ManaHUD`. `BattleTransition` — отдельный координатор встречи.

**BattleScene** содержит собственную камеру, освещение, EventSystem, `BattleManager` и `BattleUI` с сохранённым Canvas. Бой отображает тех же `Player`/`Enemy` из мира и не копирует здоровье игрока. `BattleHUD` оформляет характеристики, полосы и меню, не изменяя ресурсы. Canvas использует Scale With Screen Size 960×540, match 1.

Все три сцены включены в Build Profiles. Физическая встреча запускает бой; прямой запуск `BattleScene` без встречи показывает вспомогательное меню.

## Владельцы данных

- **Health** — единственный владелец HP (`maxHealth`, `currentHealth`) и обновления мирового HP-текста/полосы. `BattleUnit` и UI читают тот же компонент.
- **BattleUnit** — физические и магические характеристики, текущие MP, флаг защиты и внешний вид боя. `EffectiveAttack`/`EffectiveDefense` добавляют бонусы экипировки к базовым полям, не изменяя их и не восстанавливая ресурсы. Уровней, EXP и growth нет.
- **BattleInventory** — сессионный запас на объекте игрока. `InventoryState` — один массив на 30 слотов; `startingItems` служит стартовым seed и создаёт модель один раз. UI и бой не копируют количества.
- **ItemData** — статическое определение предмета: `id`, имя, описание, иконка, категория, стопка, контекст использования, слот экипировки и бонусы `attack`/`defense`. Иконки берутся из `sword.ase`/`armor.ase`/`potion.ase`. `BattleConsumable` добавляет лечение и восстановление MP. Цен, валюты и редкости нет.
- **EquipmentState** — два надетых предмета (`Weapon`, `Chest`), атомарный обмен с `InventoryState`. `EquipmentController` связывает модель с окном; `BattleUnit.Effective*` читает сумму бонусов.
- **InventoryWindow / InventorySlotView / EquipmentSlotView / InventoryTooltip** — 30 readonly-ячеек, выбор, drag, описание и два слота экипировки. Это представление, а не второй запас.
- **WorldItemUseController** — применение расходуемого из инвентаря вне боя через общую `BattleAbilities`; повторный callback в том же кадре отклоняется.
- **WorldInputGate** — блокировка движения, пока открыто модальное окно. `PlayerController` проверяет gate.
- **BattleManager** — фаза и очередь текущего боя, выбранное действие и защита от повторной команды.
- **BattleTransition** — активная встреча, сохранение активности мировых объектов, загрузка/выгрузка `BattleScene`, возврат после победы/побега/поражения.
- **BattleScreenFade** — временный Canvas, переживающий Single-загрузку; RPG-состояние не хранит.
- **BattleAudioProfile/Controller** — клипы и единственный AudioSource боя; общая громкость остаётся в `SettingsMenu`/`AudioListener`.
- **BattleVFXProfile/Controller** — данные и воспроизведение эффектов на существующих Image; ссылки заклинаний не участвуют в формулах.
- **BattlePresentation** — только позы и цвета портретов; урон/MP применяет `BattleManager` один раз.

## Последовательность боя

1. `EnemyAI` преследует игрока, обнаруженного триггером.
2. `BattleEncounter` получает физическое касание и вызывает `BattleTransition.TryBegin`.
3. Transition блокирует повторный старт, останавливает физические скорости, скрывает мир и загружает `BattleScene` аддитивно.
4. `BattleManager` получает тех же участников и строит очередь по Speed. Игрок выбирает Attack, Magic, Defend или Flee; враг действует через `EnemyBattleAI`.
5. `BattleCalculator` считает урон, `BattleAbilities` списывает MP/предмет и вызывает `Health`; UI показывает результат.
6. После победы результат показывается без EXP; возврат выгружает `BattleScene` и отключает побеждённого врага.
7. После побега враг остаётся жив, игрок по возможности перемещается на свободное место, встреча защищена таймером.
8. После поражения новая игра или меню загружаются Single и сбрасывают сессию.

## Сборки и зависимости

`FalseMemory.Runtime.asmdef` объединяет `Assets/Scripts` и ссылается на `Unity.InputSystem`, `Unity.TextMeshPro`, `UnityEngine.UI`. EditMode и PlayMode имеют отдельные тестовые asmdef со ссылкой на Runtime; EditMode ограничен платформой Editor. Пакеты Unity Localization и Addressables удалены.

Установлены Input System, TMP/uGUI, URP, 2D-инструменты, Test Framework и Unity MCP. Точные версии фиксируются `Packages/manifest.json` и lock-файлом.

## Композиция сцен

UI, текст, эффекты и затемнение заранее сохранены в сценах. Скрипты читают сериализованные ссылки и меняют состояние, но не создают GameObject в рантайме. `MainMenuBootstrap`, `PixelUi`, `InventoryWindow.Layout` и Editor-методы `Build*` удалены. Подробная схема — [SceneComposition](Systems/SceneComposition.md).

## Проверенное и известные границы

На 2026-11-02 прошли **58/58 EditMode** и **37/37 PlayMode**; Console без ошибок. Тесты используют настоящий `SceneManager`, физический контакт и синтетический Input System; часть кнопок вызывается через `onClick.Invoke`, что не заменяет ручную проверку курсора.

Известные ограничения: бой один на один; враг не обходит стены; камера без ограничений карты; Health напрямую связан с UI; клавиатура не переназначается; gamepad не добавлен; нет партий, классов, статусов, квестов, диалогов, торговли и дисковых сохранений. Данные сессии живут, пока исходная карта загружена. Сборка вне Editor и художественная оценка владельцем остаются отдельными проверками.

# 8. Справочник скриптов

Тип: **MB** — `MonoBehaviour` (компонент на объекте), **SO** — `ScriptableObject` (файл-ассет), **C#** — обычный класс/struct, **static** — статический класс.

## Корень `Assets/Scripts/`

| Скрипт | Тип | Где висит | Назначение |
|---|---|---|---|
| `PlayerController` | MB | Player | Движение WASD/стрелками через `Rigidbody2D`, анимация ходьбы, разворот спрайта |
| `EnemyAI` | MB | Скелет | Преследует игрока, пока тот в зоне-триггере |
| `Health` | MB | Player, скелет | Хранит HP; `TakeDamage`, `Heal`; обновляет полоску HP |
| `CameraFollow` | MB | Main Camera | Плавно следует за целью в `LateUpdate` |

## `Battle/` — бой

| Скрипт | Тип | Назначение |
|---|---|---|
| `BattleTransition` | MB, singleton | Начинает бой (прячет мир, грузит `BattleScene` Additive) и возвращает в мир |
| `BattleEncounter` | MB | На скелете: при касании вызывает `BattleTransition.TryBegin` |
| `BattleManager` | MB | Режиссёр боя: корутина с раундами и ходами, применяет урон/MP, решает исход, выдаёт лут после победы |
| `BattleUnit` | MB | Характеристики участника: ATK/DEF/MATK/MDEF, MP, заклинания, защита |
| `BattleCalculator` | static | Все правила боя: физический урон, промах, крит, магический урон, выбор действия врага (`ChooseEnemyAction`: 20% Defend, иначе Attack), заклинания (`CanCast`/`TryCast`), зелья (`CanUseItem`/`TryUseItem`). Тот же файл: `enum BattleAction`, `enum BattleState` |
| `BattleRandom` | C# | Случайные числа: `BattleRandom.Unity` (в игре, `UnityEngine.Random`) или `new BattleRandom(seed)` (в тестах, предсказуемо). Тот же файл: struct `PhysicalAttackResult` |
| `BattleRules` | SO | Числа баланса: множители, мин. урон, крит, защита, паузы, защита после побега |
| `BattleSpell` | SO | Заклинание: стоимость MP, сила, цвет, визуальный эффект. Всегда наносит магический урон противнику |
| `BattleConsumable` | SO | Наследник `ItemData`: лечение HP или восстановление MP (пьётся из инвентаря на себя) |
| `BattleInventory` | MB | Владелец инвентаря на Player: хранит `InventoryState` и стартовые предметы из Inspector. Через него инвентарь достают бой, окно, экипировка и лут |
| `BattleUI` | MB | Универсальное меню (подписи + делегаты), сообщения, всплывающий урон, экран результата |
| `BattleHUD` | MB | Полоски HP/MP с анимацией, оформление команд, появление экрана |
| `BattlePresentation` | MB | Анимации портретов: замах, рывок, реакция на удар, финальные позы |
| `BattleVFXController` | MB | Проигрывает эффект заклинания (кадры + частицы) |
| `BattleVFXProfile` | SO | Настройки эффекта: цвет, частицы, длительность, Animator-контроллер |
| `BattleAudioController` | MB | Проигрывает звук по `enum BattleSound` |
| `BattleAudioProfile` | SO | Набор звуков: клип на каждое `BattleSound` |
| `BattleScreenFade` | MB | Чёрный экран, который переживает загрузку сцены и плавно исчезает |
| `SpriteClipPlayer` | static | Помощник: добавить `Animator` на объект и показать кадр клипа (нужен анимации огня) |

## `Inventory/` — инвентарь

| Скрипт | Тип | Назначение |
|---|---|---|
| `ItemData` | SO | Данные предмета: имя, иконка, категория, стопка, слот, бонусы. Тот же файл: `enum ItemCategory`, `EquipmentSlot`, struct `ItemStatBonus` |
| `InventoryState` | C# | Модель: 30 слотов, добавить/убрать/переместить, событие `Changed`. Тот же файл: `InventorySlot` |
| `EquipmentState` | C# | Модель: 2 слота экипировки, обмен с инвентарём, сумма бонусов, событие `Changed` |
| `InventoryActions` | MB | Слушает события окна и выполняет действия: надеть/снять (`EquipmentState`), выпить зелье. В бою ничего не разрешает |
| `WorldInputGate` | MB | «Замок» движения: пока есть владельцы — игрок стоит |
| `InventoryWindow` | MB | Окно инвентаря: открытие по **I**, выбор, drag & drop, кнопки, события `UseRequested`/`EquipRequested`/`UnequipRequested` |
| `InventorySlotView` | MB | Одна ячейка: рисует предмет, обрабатывает клик/наведение/перетаскивание |
| `EquipmentSlotView` | MB | Ячейка экипировки (оружие / броня) |
| `ItemPresentation` | static | Цвет фона ячейки и название слота экипировки |
| `LootDrop` | MB | Висит на враге (сейчас на боссе): какой предмет выпадает. После победы `BattleManager` вызывает `GiveTo(player)` — предмет кладётся в инвентарь |

## `Dangeon/` — переходы между сценами

| Скрипт | Тип | Назначение |
|---|---|---|
| `SceneTransition` | MB | Триггер-вход: при касании игрока просит менеджер сменить сцену |
| `SceneTransitionManager` | MB, singleton | Переносит игрока и HUD в новую сцену через `DontDestroyOnLoad`, удаляет дубликаты, настраивает камеру |

## `UI/`

| Скрипт | Тип | Назначение |
|---|---|---|
| `MainMenuController` | MB | Кнопки New Game / Settings / Exit и панель настроек: громкость и полноэкранный режим, сохранение в `PlayerPrefs` |
| `PauseMenu` | MB | Пауза в мире по Esc: `Time.timeScale = 0`, панель с кнопками Resume / Main Menu |
| `ManaHUD` | MB | Полоса маны в мире |

## `Assets/Tests/` — автотесты

Запуск: **Window → General → Test Runner**.

- **EditMode** (8 файлов) — проверяют чистую логику без запуска игры: формулы, инвентарь, экипировку, заклинания, рандом с seed.
- **PlayMode** (8 файлов) — запускают сцены и проверяют сценарии целиком: бой, инвентарь, движение, переход в подземелье.

То, что `InventoryState` и `BattleCalculator` — обычные классы, а в бой можно подставить `BattleRandom` с seed (`SetRandomSource`), и делает их удобными для тестов.

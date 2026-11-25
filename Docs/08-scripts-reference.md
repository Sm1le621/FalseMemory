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
| `BattleUnit` | MB | Характеристики участника: ATK/DEF/MATK/MDEF, MP, заклинания, защита. Реализует `IBattleParticipant` |
| `BattleTurnOrder<T>` | C# (generic) | Очередь ходов в раунде, пропускает мёртвых. Тот же файл: `enum BattleAction`, `enum BattleState`, класс `BattleCommandGate` (одна команда за ход) |
| `BattleCalculator` | static | Формулы: физический урон, промах, крит, магический урон |
| `BattleAbilities` | static | Проверка и применение заклинаний (`CanCast`/`TryCast`) и предметов (`CanUseItem`/`TryUseItem`) |
| `EnemyBattleAI` | static | Выбор действия врага: 20% Defend, иначе Attack |
| `BattleRandom` | C# | Интерфейс `IBattleRandom` + `UnityBattleRandom` (игра) + `SeededBattleRandom` (тесты) + struct `PhysicalAttackResult` |
| `BattleRules` | SO | Числа баланса: множители, мин. урон, крит, защита, паузы, защита после побега |
| `BattleSpell` | SO | Заклинание: стоимость MP, сила, цель, визуальный эффект. Тот же файл: `enum BattleEffect`, `enum BattleTarget` |
| `BattleConsumable` | SO | Наследник `ItemData`: лечение HP или восстановление MP |
| `BattleInventory` | MB | Владелец инвентаря на Player: хранит `InventoryState` и стартовые предметы из Inspector. Через него инвентарь достают бой, окно, экипировка и лут |
| `BattleUI` | MB | Универсальное меню (подписи + делегаты), сообщения, всплывающий урон, экран результата |
| `BattleHUD` | MB | Полоски HP/MP с анимацией, оформление команд, появление экрана |
| `BattlePresentation` | MB | Анимации портретов: замах, рывок, реакция на удар, финальные позы |
| `BattleVFXController` | MB | Проигрывает эффект заклинания (кадры + частицы) |
| `BattleVFXProfile` | SO | Настройки эффекта: форма, цвет, длительность, Animator-контроллер |
| `BattleAudioController` | MB | Проигрывает звук по `enum BattleSound` |
| `BattleAudioProfile` | SO | Набор звуков: клип на каждое `BattleSound` |
| `BattleScreenFade` | MB | Чёрный экран, который переживает загрузку сцены и плавно исчезает |
| `SpriteAnimationSet` | SO | Пара Animator-контроллеров (мир/портрет). Тот же файл: `enum SpritePose` |
| `SpriteClipPlayer` | static | Помощник: добавить `Animator` на объект и проиграть клип |
| `EnemySpriteAnimation` | MB | Анимация врага в мире: движение, получение урона, смерть |

## `Inventory/` — инвентарь

| Скрипт | Тип | Назначение |
|---|---|---|
| `ItemData` | SO | Описание предмета: имя, иконка, категория, стопка, слот, бонусы. Тот же файл: `enum ItemCategory`, `EquipmentSlot`, `ItemUseLocation`, struct `ItemStatBonus` |
| `InventoryState` | C# | Модель: 30 слотов, добавить/убрать/переместить, событие `Changed`. Тот же файл: `InventorySlot` |
| `EquipmentState` | C# | Модель: 2 слота экипировки, обмен с инвентарём, сумма бонусов, событие `Changed` |
| `EquipmentController` | MB | Связывает `EquipmentState` с окном; запрещает менять экипировку в бою |
| `WorldItemUseController` | MB | Использование зелий вне боя по событию окна |
| `WorldInputGate` | MB | «Замок» движения: пока есть владельцы — игрок стоит |
| `InventoryWindow` | MB | Окно инвентаря: открытие по **I**, выбор, drag & drop, кнопки, события `UseRequested`/`EquipRequested`/`UnequipRequested` |
| `InventorySlotView` | MB | Одна ячейка: рисует предмет, обрабатывает клик/наведение/перетаскивание |
| `EquipmentSlotView` | MB | Ячейка экипировки (оружие / броня) |
| `InventoryTooltip` | MB | Всплывающее описание предмета у курсора |
| `ItemPresentation` | static | Тексты и цвета для UI: название типа, строка бонусов, описание |
| `LootDrop` | MB | Висит на враге (сейчас на боссе): какой предмет выпадает. После победы `BattleManager` вызывает `GiveTo(player)` — предмет кладётся в инвентарь |

## `Dangeon/` — переходы между сценами

| Скрипт | Тип | Назначение |
|---|---|---|
| `SceneTransition` | MB | Триггер-вход: при касании игрока просит менеджер сменить сцену |
| `SceneTransitionManager` | MB, singleton | Переносит игрока и HUD в новую сцену через `DontDestroyOnLoad`, удаляет дубликаты, настраивает камеру |

## `UI/`

| Скрипт | Тип | Назначение |
|---|---|---|
| `MainMenuController` | MB | Кнопки New Game / Settings / Exit |
| `PauseMenu` | MB | Пауза в мире по Esc: `Time.timeScale = 0`, панель с кнопками Resume / Main Menu |
| `SettingsMenu` | MB | Громкость и полноэкранный режим, сохранение в `PlayerPrefs` |
| `ManaHUD` | MB | Полоса маны в мире |

## `Assets/Editor/` — инструменты редактора

Работают только в редакторе Unity (пункты меню), в игру не попадают.

| Скрипт | Назначение |
|---|---|
| `PixelBattleAssetBuilder` | Рисует кодом пиксельные PNG 64×64: скелет, босс, огонь, удар |
| `SpriteAnimationAssetBuilder` | Собирает из PNG `.anim`-клипы и Animator-контроллеры, заполняет `SpriteAnimationSet` и `BattleVFXProfile` |

## `Assets/Tests/` — автотесты

Запуск: **Window → General → Test Runner**.

- **EditMode** (8 файлов) — проверяют чистую логику без запуска игры: формулы, инвентарь, экипировку, заклинания, рандом с seed.
- **PlayMode** (8 файлов) — запускают сцены и проверяют сценарии целиком: бой, инвентарь, движение, переход в подземелье.

То, что `InventoryState` и `BattleCalculator` — обычные классы, а рандом подставляется через интерфейс, и делает их удобными для тестов.

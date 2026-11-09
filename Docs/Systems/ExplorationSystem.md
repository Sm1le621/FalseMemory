# Исследование мира

## 1. Назначение

Перемещение Player по карте, открытие инвентаря, контакт с врагом, переход в пошаговый бой и переход в другую сцену через вход в пещеру/дверь. Подбора предметов, NPC и диалогов нет.

## 2. Общая логика

`PlayerController` читает WASD/стрелки и двигает `Rigidbody2D`; в покое показывает статичный `idleSprite` и выключает Animator. При движении он включает Animator, выставляет bool `isMoving` и переводит `Player.controller` в `Player_Walk`/`Player_Run`. `WorldInputGate` блокирует движение и выключает анимацию ходьбы, пока открыт инвентарь. `BattleEncounter` на враге запускает `BattleTransition.TryBegin`; на время боя мир скрывается, после победы/побега возвращается.

`SceneTransition` на входе в пещеру запускает загрузку `Dungeon` через `SceneTransitionManager`. Если идёт бой или другой переход, вход игнорируется.

## 3. Основные компоненты

- `PlayerController`, `Rigidbody2D`, `Collider2D`, `SpriteRenderer`, `Animator`.
- `WorldInputGate`, `InventoryWindow`.
- `EnemyAI`, `BattleEncounter`, `EnemySpriteAnimation`.
- `BattleTransition`, `CameraFollow`.
- `SceneTransition`, `SceneTransitionManager`.

## 4. Взаимодействие скриптов

`InventoryWindow.Open` захватывает `WorldInputGate`; `Close` отпускает. `BattleEncounter` вызывает `BattleTransition`; тот выключает `PlayerController` и `EnemyAI`, а после боя восстанавливает. `EnemySpriteAnimation` читает позицию и HP и показывает клипы, но не управляет AI и боем. `SceneTransition` проверяет состояние `BattleTransition` и передаёт переход менеджеру.

## 5. Последовательность событий

Старт `SampleScene` → Player в покое (`Idle1.ase`, Animator выключен) → движение (`isMoving = true`, `Player_Walk`/`Player_Run`) → `I` открывает инвентарь и блокирует ввод → `Escape` закрывает → касание скелета → бой → возврат на прежнюю позицию с текущими HP/MP/инвентарём → вход в пещеру → `Dungeon` с тем же Player, HP/MP-полосами и инвентарём.

## 6. Настраиваемые параметры

`PlayerController.moveSpeed` (4), `defaultFacingRight` (true), `idleSprite` (`Idle1.ase`). `EnemyAI` — скорость/дистанция преследования. `BattleEncounter` — параметры встречи. `BattleTransition.rules` — тайминги и защита после побега. `SceneTransition.targetSceneName` (`Dungeon`), `useSpawnPosition` (true), `spawnPosition` ((0,0)); `CameraFollow.smoothSpeed` — скорость камеры после переноса.

## 7. Использование в Unity

Player и Enemy находятся в `SampleScene`; бой — в `BattleScene`; целевая сцена перехода — `Dungeon`. Для нового врага добавь `BattleUnit`, `Health`, `BattleEncounter`, `EnemyAI` и `EnemySpriteAnimation`. Не создавай второй `BattleTransition` и не копируй Player в боевую сцену. Для входа в пещеру добавь `SceneTransition` на объект с коллайдером-триггером и включи `Dungeon` в Build Settings.

## 8. Расширение

NPC/сундуки/диалоги отсутствуют; для них потребуется новый контроллер взаимодействия, а не удалённый `PlayerInteractor`. Новые зоны/враги используют существующие компоненты. Новые переходы делаются вторым объектом `SceneTransition` с другим `targetSceneName`/`spawnPosition`. Подбор предметов с земли не планируется в текущей упрощённой модели.

## 9. Ограничения и проверки

Нет геймпада, NPC, квестов, диалогов, подбора и сохранения мира. Побеждённый скелет остаётся выключенным до перезагрузки сцены. При переходе в `Dungeon` старая сцена выгружается, поэтому враги и мир `SampleScene` создаются заново при возврате. PlayMode проверяет движение, блокировку инвентарём, вход в бой, победу/побег и возврат; переход в `Dungeon` проверен ручной компиляцией, но не PlayMode; визуальная оценка карты — за владельцем.

## История

- 2026-11-10 — удалён устаревший `EnemyAttack`; визуал врага работает через `.anim`-клипы.
- 2026-11-09 — добавлен переход в `Dungeon` через `SceneTransition`/`SceneTransitionManager`; Player и HP/MP HUD переносятся без копирования.
- 2026-11-02 — параметр `isMoving` теперь выставляется при движении; в рабочей копии Player.controller использует `Player_Idle`/`Player_Walk`/`Player_Run`, в покое по-прежнему показывается статичный `Idle1.ase`.
- 2026-10-31 — удалены подбор предметов, `PlayerInteractor`, `WorldItemPickup` и IDLE-анимация; инвентарь стал единственным источником предметов.
- 2026-10-11 — базовая документация исследования.

# Исследование мира

## 1. Назначение

Перемещение Player по карте, открытие инвентаря, контакт с врагом и переход в пошаговый бой. Подбора предметов, NPC и диалогов нет.

## 2. Общая логика

`PlayerController` читает WASD/стрелки и двигает `Rigidbody2D`; в покое показывает статичный `idleSprite`, при движении включает Animator с клипом ходьбы. `WorldInputGate` блокирует движение, пока открыт инвентарь. `BattleEncounter` на враге запускает `BattleTransition.TryBegin`; на время боя мир скрывается, после победы/побега возвращается.

## 3. Основные компоненты

- `PlayerController`, `Rigidbody2D`, `Collider2D`, `SpriteRenderer`, `Animator`.
- `WorldInputGate`, `InventoryWindow`.
- `EnemyAI`, `EnemyAttack`, `BattleEncounter`, `EnemySpriteAnimation`.
- `BattleTransition`, `CameraFollow`.

## 4. Взаимодействие скриптов

`InventoryWindow.Open` захватывает `WorldInputGate`; `Close` отпускает. `BattleEncounter` вызывает `BattleTransition`; тот выключает `PlayerController`, `EnemyAI` и `EnemyAttack`, а после боя восстанавливает. `EnemySpriteAnimation` читает позицию и HP, но не управляет AI и боем.

## 5. Последовательность событий

Старт `SampleScene` → Player в покое (`Idle1.ase`) → движение (`Player_Walk`) → `I` открывает инвентарь и блокирует ввод → `Escape` закрывает → касание скелета → бой → возврат на прежнюю позицию с текущими HP/MP/инвентарём.

## 6. Настраиваемые параметры

`PlayerController.moveSpeed` (4), `defaultFacingRight` (true), `idleSprite` (`Idle1.ase`). `EnemyAI` — скорость/дистанция преследования. `BattleEncounter` — параметры встречи. `BattleTransition.rules` — тайминги и защита после побега.

## 7. Использование в Unity

Player и Enemy находятся в `SampleScene`; бой — в `BattleScene`. Для нового врага добавь `BattleUnit`, `Health`, `BattleEncounter`, `EnemyAI` и `EnemySpriteAnimation`. Не создавай второй `BattleTransition` и не копируй Player в боевую сцену.

## 8. Расширение

NPC/сундуки/диалоги отсутствуют; для них потребуется новый контроллер взаимодействия, а не удалённый `PlayerInteractor`. Новые зоны/враги используют существующие компоненты. Подбор предметов с земли не планируется в текущей упрощённой модели.

## 9. Ограничения и проверки

Нет геймпада, NPC, квестов, диалогов, подбора и сохранения мира. Побеждённый скелет остаётся выключенным до перезагрузки сцены. PlayMode проверяет движение, блокировку инвентарём, вход в бой, победу/побег и возврат; визуальная оценка карты — за владельцем.

## История

- 2026-10-31 — удалены подбор предметов, `PlayerInteractor`, `WorldItemPickup` и IDLE-анимация; инвентарь стал единственным источником предметов.
- 2026-10-11 — базовая документация исследования.

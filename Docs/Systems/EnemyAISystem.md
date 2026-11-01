# Поведение противника

## 1. Назначение

Преследование игрока в мире и выбор действия в бою. Это два разных режима: `EnemyAI` двигает врага, `EnemyBattleAI` выбирает Attack/Defend.

## 2. Общая логика

`EnemyAI` реагирует на игрока в радиусе обнаружения и двигается к нему. `BattleEncounter` запускает бой физическим касанием. На время боя `BattleTransition` выключает `EnemyAI` и `EnemyAttack`; после побега возвращает управление с временной защитой. `EnemyBattleAI` с шансом `enemyDefendChance` выбирает Defend, иначе Attack.

## 3. Основные компоненты

- `EnemyAI` — преследование.
- `BattleEncounter` — запуск встречи.
- `EnemyAttack` — устаревший контактный урон (пропускается при `BattleEncounter`).
- `EnemyBattleAI` — боевое решение.
- `BattleTransition`, `BattleRules`.

## 4. Взаимодействие скриптов

`BattleTransition.TryBegin` выключает `EnemyAI`/`EnemyAttack` и останавливает тело. `BattleManager` вызывает `EnemyBattleAI.Choose(rules, random)` в ход врага. `EnemySpriteAnimation` отражает движение и показывает Hurt/Death, не влияя на AI.

## 5. Последовательность событий

Обнаружение → преследование → контакт → бой → Attack/Defend по очереди. Победа выключает врага; побег оставляет HP/позицию и временно отключает AI.

## 6. Настраиваемые параметры

`EnemyAI.moveSpeed` (код 3), радиус/дистанция — в компоненте. `BattleRules.enemyDefendChance` — 0.2. Скелет: HP 60, ATK 12, DEF 5, SPD 7, MDEF 3. Защита после побега — `fleeProtectionSeconds` (3 с).

## 7. Использование в Unity

Enemy требует `Rigidbody2D`, коллайдер обнаружения и физический `Collider2D`, `Health`, `BattleUnit`, `BattleEncounter`. Тег Player должен совпадать. `EnemyAttack` в текущей сцене выключен.

## 8. Расширение

Новый враг настраивается через `BattleUnit`/`Health`/`BattleEncounter`; тактика меняется в `EnemyBattleAI`. Поиск пути вокруг препятствий, магия AI и несколько атак — будущие задачи.

## 9. Ограничения и проверки

Прямое преследование без pathfinding; `MovePosition` вызывается в `Update`. Нет магии и выбора цели у AI. Контактный запуск, победа, поражение и побег покрыты PlayMode. Качество движения по всей карте вручную не оценивалось.

## История

- 2026-10-31 — убраны упоминания награды EXP; AI по-прежнему выбирает только Attack/Defend.
- 2026-10-11 — базовая документация AI.

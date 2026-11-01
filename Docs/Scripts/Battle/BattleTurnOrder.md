# BattleTurnOrder

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleTurnOrder.cs`.

Очередь ходов по скорости, одноразовый gate команд и общие перечисления действий/состояний боя. Классы не зависят от Unity lifecycle и тестируются отдельно.

## 2. Как работает

`BattleTurnOrder<T>.BeginRound` очищает очередь и вставляет живых уникальных участников по убыванию `Speed`. При равной скорости сохраняется исходный порядок. `TryNext` последовательно выдаёт живых участников; погибший пропускается. `BattleCommandGate` открывается на ход игрока и позволяет потребить команду только один раз, защищая от одновременного клика и Enter.

## 3. Методы и функции

- `BeginRound(IEnumerable<T> participants)` — строит очередь раунда; `null`, мёртвые и дубликаты пропускаются.
- `TryNext(out T participant)` → `bool`; выдаёт следующего живого участника или `false` в конце раунда.
- `BattleCommandGate.Open()` / `Close()` — открывают/закрывают приём команды.
- `BattleCommandGate.TryConsume()` → `bool`; при открытом gate закрывает его и возвращает `true`, иначе `false`.
- `BattleAction` — `Attack`, `Defend`, `Magic`, `Flee`.
- `BattleState` — `Starting`, `Resolving`, `AwaitingPlayer`, `Victory`, `Defeat`, `Escaped`.

## 4. Переменные и Inspector

- `order` — внутренний список участников раунда.
- `cursor` — индекс следующего участника.
- `IsOpen` — публичное свойство gate только для чтения.

Inspector-полей нет: это обычные C#-классы, не MonoBehaviour.

## 5. Связи

`BattleManager` создаёт очередь и gate. Участники реализуют `IBattleParticipant` (`Speed`, `IsAlive`); в игре это `BattleUnit`. `EnemyBattleAI`, `BattleCalculator` и тесты используют `BattleAction`/`BattleState`.

## 6. Пример

Player имеет Speed 10, Skeleton — 7. `BeginRound` ставит Player первым. После его атаки `TryNext` возвращает Skeleton; если скелет погиб до своего хода, он пропускается и раунд заканчивается.

## 7. Настройка в Unity

Компонент не прикрепляется. Порядок зависит от `BattleUnit.speed`; для изменения очерёдности меняй скорость участников, а не код очереди.

## 8. Возможные проблемы

Очередь строится заново каждый раунд, поэтому изменение скорости во время боя влияет со следующего раунда. `TryNext` не проверяет `null` после `BeginRound`, но `BeginRound` уже отфильтровал такие элементы. Gate не является потокобезопасным — предполагается один поток Unity.

## 9. История изменений

- 2026-10-31 — `BattleAction` сокращён до Attack/Defend/Magic/Flee; документ приведён к текущему коду.
- 2026-10-11 — базовая очередь и gate.

# BattleTurnOrder

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleTurnOrder.cs.

Очередь участников по Speed, перечисления команд/состояний и защита от повторного принятия команды.

## 2. Как работает

BeginRound сортирует живых уникальных участников по убыванию Speed; равенство сохраняет исходный порядок. TryNext пропускает погибших. Gate принимает команду один раз до следующего Open.

## 3. Методы и функции

- `BattleTurnOrder<T>.BeginRound(IEnumerable<T> participants)` — очищает очередь/курсор, добавляет живых участников без дублей; void.
- `TryNext(out T participant)` → bool; выдаёт следующего живого, в конце false и null.
- `BattleCommandGate.Open()` / `Close()` — меняют IsOpen; void.
- `TryConsume()` → bool; если открыта, закрывает и возвращает true, иначе false. Unity callbacks нет.

## 4. Переменные и параметры Inspector

Inspector нет. T реализует IBattleParticipant.Speed/IsAlive. order/cursor — внутренние. BattleAction: Attack, Defend, Magic, Items, Flee. BattleState: Starting, Resolving, AwaitingPlayer, Victory, Defeat, Escaped.

## 5. Связи с другими системами

BattleManager содержит очередь BattleUnit и gate. Класс очереди поддерживает N участников, но текущий менеджер формирует только Player/Enemy.

## 6. Пример работы

Speed10 игрока и 7 скелета → игрок первым. Два вызова TryConsume после Open дают true, затем false.

## 7. Настройка в Unity

Класс не прикрепляется к GameObject и не имеет Inspector. Его вызывает BattleManager/BattleAbilities с данными BattleUnit/Rules. Назначь существующие Rules на BattleTransition; параметры передаются методам, а не через отдельный компонент.

## 8. Возможные проблемы и проверки

Speed пересортируется только в новом раунде. null список участников не поддерживается. Generic очередь не означает готовую систему партии. Порядок, смерть, равенство и двойная команда проверены Edit Mode.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

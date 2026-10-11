# BattleEncounter

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleEncounter.cs.

Запуск боя при физическом касании игроком врага.

## 2. Как работает

Enter/Stay collision передают контакт TryBegin. Метод ищет BattleUnit у Player и передаёт пару в единственный BattleTransition.

## 3. Методы и функции

- `OnCollisionEnter2D(Collision2D collision)` — Unity при начале физического контакта, вызывает TryBegin; void.
- `OnCollisionStay2D(Collision2D collision)` — продолжающийся контакт, также TryBegin.
- `TryBegin(Collision2D collision)` — внутренний метод: тег Player и BattleUnit → transition.TryBegin(player, собственный BattleUnit); void.

## 4. Переменные и параметры Inspector

Сериализованных полей Inspector нет. RequireComponent гарантирует BattleUnit, тот гарантирует Health.

## 5. Связи с другими системами

На Enemy. Нужны физические Collider2D/Rigidbody2D и Player tag. Триггер агрессии EnemyAI не запускает бой. Transition блокирует повторные старты.

## 6. Пример работы

Радиус агрессии → преследование → контакт тел → BattleTransition → BattleScene.

## 7. Возможные проблемы и проверки

BattleUnit ищется на объекте collision.gameObject, а не родителе; перенос коллайдера на дочерний объект требует адаптации. Без Transition/Player tag бой не начнётся. Физический контакт и один запуск проверены Play Mode.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

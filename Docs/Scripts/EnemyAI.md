# EnemyAI

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/EnemyAI.cs.

Преследование Player скелетом на карте.

## 2. Как работает

Start получает тело. Вход Player в триггер включает агрессию, Update двигает к цели, выход прекращает движение.

## 3. Методы и функции

- `Start()` — Unity получает Rigidbody2D; void.
- `Update()` — при isAggro и цели вызывает MovePosition по направлению, moveSpeed и deltaTime.
- `OnTriggerEnter2D(Collider2D collision)` — callback входа: тег Player → сохраняет Transform, isAggro=true.
- `OnTriggerExit2D(Collider2D collision)` — выход Player → isAggro=false, linearVelocity=0.

## 4. Переменные и параметры Inspector

moveSpeed — код 3, задавать ≥0; Min нет. playerTransform/isAggro/rb — внутренние ссылки и состояние.

## 5. Связи с другими системами

Enemy требует Rigidbody2D и триггер; Player — тег Player. BattleEncounter отвечает за касание; BattleTransition приостанавливает AI в бою и после побега.

## 6. Пример работы

Player входит в радиус → скелет подходит → касание начинает бой.

## 7. Настройка в Unity

На Enemy в SampleScene с Rigidbody2D и Collider2D-триггером обнаружения. Player имеет тег Player. Отдельный нетриггерный коллайдер тела запускает Encounter. Настрой Move Speed, не добавляй второй AI; движение должно быть выключено Transition во время боя.

## 8. Возможные проблемы и проверки

Rigidbody2D не гарантирован. MovePosition в Update, не FixedUpdate; нет пути вокруг стен. Несколько коллайдеров игрока могут преждевременно сбрасывать агрессию. Повторный бой проверен тестом; преследование на всей карте не проверено.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

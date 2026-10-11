# PlayerController

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/PlayerController.cs.

Движение Player по карте, анимация ходьбы и направление спрайта.

## 2. Как работает

Start получает компоненты. Update читает WASD/стрелки и нормализует направление; FixedUpdate перемещает тело. Диагональ не ускоряет героя.

## 3. Методы и функции

- `Start()` — Unity до первого Update получает rb/animator/spriteRenderer; void.
- `Update()` — каждый кадр читает Keyboard.current, записывает moveInput и Animator.isMoving; вызывает HandleSpriteFlipping.
- `FixedUpdate()` — шаг физики: MovePosition по moveInput × moveSpeed × fixedDeltaTime.
- `HandleSpriteFlipping()` — из Update; горизонтальный ввод определяет аргумент SetFacingDirection.
- `SetFacingDirection(bool lookRight)` — меняет flipX с учётом defaultFacingRight; void.

## 4. Переменные и параметры Inspector

moveSpeed — сериализованная скорость, код 4, использовать ≥0 (Min нет). defaultFacingRight — bool исходного взгляда, код true. rb/animator/spriteRenderer/moveInput — внутреннее состояние.

## 5. Связи с другими системами

Player требует Rigidbody2D, Collider2D для столкновений и SpriteRenderer (только последний гарантирует RequireComponent). Animator необязателен, нужен bool isMoving. BattleTransition выключает компонент в бою и восстанавливает после.

## 6. Пример работы

D → ввод (1,0) → физическое перемещение вправо → касание скелета приостанавливает движение до окончания боя.

## 7. Настройка в Unity

На существующем Player в SampleScene. Проверь Rigidbody2D, Collider2D, SpriteRenderer; Animator при наличии должен содержать bool isMoving. В Inspector настраивай Move Speed и Default Facing Right. Добавлять второй контроллер не нужно. Проверка: Play → фокус Game View → WASD/стрелки.

## 8. Возможные проблемы и проверки

Без Rigidbody2D ошибка. Нужен фокус Game View; Gamepad/переназначения нет. Возобновление движения после победы покрыто Play Mode тестом; анимации отдельно в этой задаче не проверялись.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

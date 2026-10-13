# PlayerController

## 1. Название и назначение

`Assets/Scripts/PlayerController.cs` — единственный существующий контроллер движения героя на карте, ввода и directional Animator.

## 2. Как работает

Start получает компоненты. Update читает WASD/стрелки и нормализует диагонали. FixedUpdate измеряет завершённое физическое перемещение, плавно меняет модуль скорости и вызывает MovePosition. Направление меняется сразу, поэтому боковой инерции нет. LateUpdate передаёт фактическую скорость в Animator; Walk продолжает общую фазу при повороте. Inventory/gate и отключение компонента останавливают движение немедленно. Health/MP/баланс характеристик прежние.

## 3. Методы и функции

- `Start()` — Unity перед первым Update кеширует Rigidbody2D, Animator, SpriteRenderer, WorldInputGate, BattleUnit, начальную физическую позицию; определяет directional/WalkSpeed параметры и обновляет позу.
- `Update()` — каждый render-кадр читает Keyboard.current; сохраняет moveInput и последний ненулевой lastDirection. Gate вызывает StopMovement и UpdateAnimation. Legacy Animator получает HandleSpriteFlipping.
- `FixedUpdate()` — каждый шаг физики измеряет ActualMovementSpeed по rb.position предыдущего шага и fixedDeltaTime; currentSpeed приближается к MovementSpeed либо0 через acceleration/deceleration; MovePosition двигает вдоль lastDirection. При gate/несимулируемом теле StopMovement.
- `LateUpdate()` — после Update вызывает UpdateAnimation с результатами физики.
- `UpdateAnimation()` — isMoving включён при actual/current speed выше.03; WalkSpeed=actual/reference, ограничено0…3. Четыре directional float получают последнее направление; flipX=false. Без Animator безопасный выход.
- `StopMovement()` — из gate/OnDisable/FixedUpdate; обнуляет модуль скорости, измеренную скорость и rb.linearVelocity, обновляет previousPhysicsPosition; позицию не переносит.
- `OnDisable()` — очищает ввод, вызывает StopMovement и UpdateAnimation, сохраняет взгляд.
- `HasFloat(parameter)` — ищет float-параметр Animator, возвращает bool, ничего не создаёт.
- `HandleSpriteFlipping()`/`SetFacingDirection(lookRight)` — legacy-ветка отражает SpriteRenderer по горизонтальному вводу с учётом defaultFacingRight.
- Readonly `MovementInput`, `LastDirection`, `MovementSpeed`, `ActualMovementSpeed`, `CurrentMovementSpeed` — нормализованный ввод, взгляд, целевая скорость с атрибутами/gear, измеренная скорость и внутренний модуль разгона. Это диагностика, не Inspector.

## 4. Переменные и Inspector

moveSpeed: код/сцена4, ≥0; acceleration:48 world units/s², deceleration:80, оба≥.01; animationReferenceSpeed:4 world units/s, ≥.01; defaultFacingRight=true только legacy. До4 разгон≈.083с, остановка≈.05с, дискретизированы физическим шагом. MovementSpeed добавляет BattleUnit.MovementAdjustment и ограничивает максимумом CharacterAttributeRules (назначено12). Кеши, previousPhysicsPosition/currentSpeed/ActualMovementSpeed и lastDirection — внутреннее состояние. Inspector не хранит отдельные HP/MP.

## 5. Связи

Rigidbody2D/SpriteRenderer/Collider2D исходного Player; WorldInputGate и BattleTransition блокируют карту; BattleUnit даёт бонус скорости. Player.controller содержит isMoving, MoveX/Y, LastMoveX/Y и новый WalkSpeed только для Walk-состояния. World Idle остаётся статическим отдельным ракурсом.

## 6. Пример

W+D → нормализованное направление, за короткий разгон скорость достигает4 без диагонального ускорения; WalkSpeed≈1. У стены фактическая скорость падает, анимация переходит Idle. Отпускание тормозит вдоль последнего направления, I останавливает сразу.

## 7. Настройка в Unity

На существующем Player SampleScene. Rigidbody2D: Dynamic, Gravity0, FreezeRotation, Interpolate, Continuous. Коллайдер/scale не менялись. Настраивать базовую скорость/ускорение/торможение/reference; reference соответствует темпу готового клипа. Не ставить второй контроллер и не двигать Transform параллельно MovePosition.

## 8. Возможные проблемы и проверки

Первое измерение после внешней телепортации может дать краткий завышенный WalkSpeed (ограничено3); штатный переход боя сохраняет позицию. Переназначение Animator после Start не обновляет кеш. Gamepad/настройка клавиш отсутствуют. Interpolate добавляет примерно один шаг визуальной задержки. Естественность ног/плаща и выбранные12FPS требуют ручного просмотра; тесты проверяют физику/кадры, а не художественное качество.

## 9. История изменений

- 2026-10-12 — Player rework4: primary attributes подключены к исходным ресурсам/бою/UI; прежние идентификаторы и игровая сессия сохранены.
- 2026-10-12 — Player rework2: четыре directional-параметра, сохранение последнего направления, legacy-совместимость, readonly diagnostics и сброс движения при OnDisable. Подготовлен Animator; настоящие Walk-кадры отсутствуют.
- 2026-10-12 — 4.2: WorldInputGate останавливает ввод/движение при инвентаре; скорость/направление сохранены.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-13 — короткий разгон/торможение, мгновенная модальная остановка, actual-speed WalkSpeed и безопасный сброс; базовая скорость4 сохранена.

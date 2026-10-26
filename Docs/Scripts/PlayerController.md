# PlayerController

## 1. Название и назначение

Исходник: `Assets/Scripts/PlayerController.cs`. Существующий единственный контроллер перемещения героя, клавиатуры и параметров Animator.

## 2. Как работает

Update читает нормализованный WASD/стрелки; FixedUpdate измеряет фактическое перемещение и вызывает MovePosition с прежним разгоном/торможением. LateUpdate передаёт состояние в Animator. Новый переключатель useCardinalAnimations округляет только визуальный взгляд до четырёх ракурсов, физическое направление/диагонали/скорость не меняет. Gate и OnDisable немедленно останавливают движение.

## 3. Методы и функции

- `Start()` — Unity перед Update получает Rigidbody2D/Animator/SpriteRenderer/WorldInputGate/BattleUnit, исходную физическую позицию, проверяет float-параметры, обновляет позу.
- `Update()` — каждый render-кадр читает Keyboard.current, нормализует moveInput, сохраняет ненулевой lastDirection; при gate StopMovement/UpdateAnimation. Legacy-ветка использует flip.
- `FixedUpdate()` — измеряет ActualMovementSpeed по завершённому шагу, currentSpeed приближается к MovementSpeed/0 через acceleration/deceleration; MovePosition вдоль настоящего lastDirection. Несимулируемое тело/gate → StopMovement.
- `LateUpdate()` — вызывает UpdateAnimation после Update.
- `UpdateAnimation()` — isMoving при actual/current>.03; WalkSpeed=actual/reference clamp0…3. При useCardinalAnimations получает CardinalFacing, иначе исходный lastDirection; передаёт этот взгляд в MoveX/Y и LastMoveX/Y, flipX=false. Без Animator безопасный выход.
- `CardinalFacing(direction)` → Vector2, приватный static: при absX>absY возвращает горизонтальную ось со знаком X, иначе вертикальную со знаком Y (при равенстве приоритет вертикали). Не записывает moveInput/lastDirection/позицию.
- `StopMovement()` — очищает current/actual speed, rb.linearVelocity, сбрасывает previousPhysicsPosition; позицию не переносит.
- `OnDisable()` — нулевой ввод/StopMovement/обновление позы, взгляд сохранён.
- `HasFloat(parameter)` → bool наличия float-параметра Animator.
- `HandleSpriteFlipping()`/`SetFacingDirection(lookRight)` — прежняя legacy-совместимость по горизонтальному вводу/defaultFacingRight.
- Readonly MovementInput/LastDirection/MovementSpeed/ActualMovementSpeed/CurrentMovementSpeed — диагностические значения; MovementSpeed учитывает BattleUnit.MovementAdjustment и прежний лимит правил.

## 4. Переменные и Inspector

moveSpeed код/сцена4, acceleration48, deceleration80, animationReferenceSpeed4 (≥.01). useCardinalAnimations по умолчанию false для совместимости; существующему Player в SampleScene через MCP назначено true, сцена сохранена. defaultFacingRighttrue только legacy. moveInput/lastDirection/currentSpeed/previousPhysicsPosition и ссылки — внутренние состояния. Диагональный LastDirection остаётся нормализованным диагональным вектором, даже когда визуальный взгляд cardinal.

## 5. Связи

Прежний Player/SpriteRenderer/Rigidbody2D/Collider2D, WorldInputGate/Inventory/BattleTransition, BattleUnit/attributes. Player.controller содержит прежние параметры и два Blend Tree; новая графика — четыре ракурса: четыре разных Idle Sprite и четыре позиции Walk с тремя уникальными Sprite (нейтральный повторяется по заданию). Health/MP/очередь не принадлежат контроллеру.

## 6. Пример

W+D → реальный вектор(.707,.707), обычная скорость4; визуально Up, без смешения Sprite двух ракурсов. Отпускание → Idle Up, тело тормозит прежним способом; Inventory прекращает движение немедленно.

## 7. Настройка в Unity

Существующий Player SampleScene. Для нового controller4 children включить Use Cardinal Animations, не устанавливать второй контроллер. Сохранить Dynamic/Gravity0/FreezeRotation/Interpolate/Continuous/scale и коллайдеры. WireActivePlayer выполнен через MCP: флаг и Sprite назначены, SaveScene успешен, dirty=false. Остальные параметры сцены сохранены.

## 8. Возможные проблемы

Для старого8-direction controller можно оставить false. Если новый4-direction controller используется с false, диагональ может смешивать/переключать Sprite — поэтому оба изменения проверяются вместе. При внешнем teleport первое измерение скорости кратко завышено (clamp3), настоящая система движения не переписана. Художественная естественность/темп не доказываются одними числами; [проверки](../PlayerWalkOrder.md).

## 9. История изменений

- 2026-10-12 — Player rework4: primary attributes подключены к исходным ресурсам/бою/UI; прежние идентификаторы и игровая сессия сохранены.
- 2026-10-12 — Player rework2: четыре directional-параметра, сохранение последнего направления, legacy-совместимость, readonly diagnostics и сброс движения при OnDisable. Подготовлен Animator; настоящие Walk-кадры отсутствуют.
- 2026-10-12 — 4.2: WorldInputGate останавливает ввод/движение при инвентаре; скорость/направление сохранены.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-13 — короткий разгон/торможение, мгновенная модальная остановка, actual-speed WalkSpeed и безопасный сброс; базовая скорость4 сохранена.

- 2026-10-26 — актуализирован новый ограниченный набор4×4 Idle/Walk, выбор cardinal-взгляда/импорт и соответствующие проверки. Фактический статус выполнения — в PlayerCardinal4.

- 2026-10-26 — через Unity MCP подключён четырёхпозиционный Walk: нейтральная стойка → RightStep → LeftStep → та же стойка; обновлены контракты и реальные Play-проверки. Подробности текущей проверки — в PlayerWalkOrder.

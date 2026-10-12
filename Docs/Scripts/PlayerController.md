# PlayerController

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/PlayerController.cs.

Движение Player по карте, анимация ходьбы и направление спрайта.

## 2. Как работает

Start получает компоненты, включая необязательный WorldInputGate, и распознаёт directional Animator по четырём float-параметрам. Update читает WASD/стрелки, нормализует направление и сохраняет последнее ненулевое направление. При блокировке окна ввод/анимация движения обнуляются без изменения последнего направления; FixedUpdate останавливает скорость. Закрытие окна не меняет enabled контроллера. Диагональ не ускоряет героя. Directional Animator выбирает Idle/Walk через isMoving и два2D Blend Tree. Сейчас все клипы используют один исходный ракурс как временную позу — уникальной восьминаправленной графики ещё нет.

## 3. Методы и функции

- `Start()` — Unity до первого Update получает rb/animator/spriteRenderer/inputGate; через HasFloat определяет поддержку направления и вызывает UpdateAnimation; void.
- `Update()` — каждый кадр читает Keyboard.current, записывает moveInput/lastDirection; вызывает UpdateAnimation. При WorldInputGate оставляет последнее направление и обнуляет движение. HandleSpriteFlipping вызывается только для прежнего Animator без directional-параметров.
- `HasFloat(string parameter)` — при Start ищет параметр Animator указанного имени и типа Float; возвращает bool, параметры не создаёт.
- `UpdateAnimation()` — из Start/Update/OnDisable; устанавливает isMoving и, при directional Animator, MoveX/MoveY/LastMoveX/LastMoveY. Отключает flipX, чтобы будущие отдельные ракурсы не отражались повторно. Без Animator безопасно возвращает управление; void.
- `OnDisable()` — Unity при скрытии/выключении Player сбрасывает moveInput/linearVelocity и состояние движения Animator; lastDirection сохраняется. Не восстанавливает HP/MP и не меняет позицию.
- `MovementInput`, `LastDirection` — публичные readonly getters внутреннего нормализованного ввода/последнего направления; используются проверками, не параметры Inspector.
- `FixedUpdate()` — шаг физики: MovePosition по moveInput × moveSpeed × fixedDeltaTime.
- `HandleSpriteFlipping()` — из Update; горизонтальный ввод определяет аргумент SetFacingDirection.
- `SetFacingDirection(bool lookRight)` — меняет flipX с учётом defaultFacingRight; void.

inputGate — private cached WorldInputGate, без нового регулятора движения; изменение относится только к модальному вводу. Start/Update/FixedUpdate учитывают этот компонент.
## 4. Переменные и параметры Inspector

moveSpeed — сериализованная скорость, код4, использовать≥0 (Min нет). defaultFacingRight — bool исходного взгляда, кодtrue, действует только в legacy-ветке. rb/animator/spriteRenderer/moveInput/inputGate/directionalAnimator/lastDirection — внутреннее состояние, не Inspector. Начальное последнее направление Vector2.down. Новых serialized-полей и изменения сценовых скорости/физики нет.

## 5. Связи с другими системами

Player требует Rigidbody2D, Collider2D для столкновений и SpriteRenderer (только последний гарантирует RequireComponent). Animator необязателен; при наличии нужен bool isMoving. Directional-вариант дополнительно содержит четыре Float: MoveX, MoveY, LastMoveX, LastMoveY. BattleTransition выключает компонент в бою и восстанавливает после. [Схема анимаций](../Systems/AnimationSystem.md).

## 6. Пример работы

W+A → нормализованный ввод(−0.707,0.707) → Walk_UpLeft. После отпускания MoveX/MoveY=0, LastMoveX/LastMoveY сохраняются и выбирается Idle_UpLeft. Пока художественных кадров нет, обе позы показывают оригинальный myproject1idle. Касание скелета останавливает движение до окончания боя.

## 7. Настройка в Unity

На существующем Player в SampleScene, Animator использует прежний Assets/Player.controller. Проверь Rigidbody2D, Collider2D, SpriteRenderer и перечисленные параметры Animator. В Inspector настраивай Move Speed; Default Facing Right нужен только legacy-контроллерам. Добавлять второй контроллер движения не нужно. Проверка: Play → фокус Game View → WASD/стрелки, диагонали и отпускание клавиш. Кадры подключаются в существующие Assets/Animations/Player64/*.anim; скрипт менять не требуется.

## 8. Возможные проблемы и проверки

Без Rigidbody2D ошибка. Нужен фокус Game View; Gamepad/переназначения нет. Смена runtimeAnimatorController после Start не перенастраивает кеш — контроллер назначается до запуска. Наличие параметров не означает наличие качественных кадров; оригинальный single-angle fallback не может визуально смотреть во все восемь сторон. Физический контакт/возврат проверяют существующие сценарии, направление/смену Idle-Walk проверяет PlayerDirectionFlowTests. Художественную плавность/ракурсы нужно проверить после предоставления настоящих кадров.

## 9. История изменений
- 2026-10-12 — Player rework2: четыре directional-параметра, сохранение последнего направления, legacy-совместимость, readonly diagnostics и сброс движения при OnDisable. Подготовлен Animator; настоящие Walk-кадры отсутствуют.
- 2026-10-12 — 4.2: WorldInputGate останавливает ввод/движение при инвентаре; скорость/направление сохранены.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

# PlayerController

## 1. Название и назначение

Исходник: `Assets/Scripts/PlayerController.cs`.

Движение Player по карте, направление спрайта и переключение между статичным кадром покоя и анимацией ходьбы. В покое Animator выключен и показывается `idleSprite`; IDLE-клипа нет.

## 2. Как работает

`Start` получает `Rigidbody2D`, `Animator`, `SpriteRenderer` и необязательный `WorldInputGate`, затем показывает статичный кадр. `Update` при заблокированном вводе обнуляет движение и показывает покой. Иначе читает WASD/стрелки, нормализует вектор, включает Animator только при движении и обновляет отражение спрайта. `FixedUpdate` перемещает тело через `MovePosition`; при блокировке останавливает скорость. Диагональ не ускоряет персонажа.

## 3. Методы и функции

- `Start()` — Unity callback; кеширует компоненты и вызывает `ShowIdle`.
- `Update()` — каждый кадр читает ввод, вызывает `SetWalking` и `HandleSpriteFlipping`.
- `FixedUpdate()` — шаг физики; двигает `Rigidbody2D` или обнуляет скорость при блокировке.
- `SetWalking(bool walking)` — включает/выключает Animator; при остановке показывает `idleSprite`.
- `ShowIdle()` — назначает статичный спрайт покоя.
- `HandleSpriteFlipping()` — по горизонтальному вводу вызывает `SetFacingDirection`.
- `SetFacingDirection(bool lookRight)` — учитывает `defaultFacingRight` и меняет `flipX`.

## 4. Переменные и Inspector

- `moveSpeed` — скорость движения, по умолчанию 4; ограничения `Min` нет.
- `defaultFacingRight` — исходное направление, по умолчанию true.
- `idleSprite` — статичный спрайт покоя; в `SampleScene` назначен `Idle1.ase`.
- `rb`, `animator`, `spriteRenderer`, `moveInput`, `inputGate` — внутреннее состояние.

## 5. Связи

Требуется `SpriteRenderer`; для движения нужен `Rigidbody2D`, для столкновений — `Collider2D`. `WorldInputGate` блокирует ввод при открытом инвентаре. `BattleTransition` выключает контроллер на время боя и восстанавливает после. `Animator` необязателен: без него персонаж двигается, но анимация ходьбы не проигрывается.

## 6. Пример

Зажата D: `moveInput = (1,0)`, Animator включается, проигрывается `Player_Walk`, спрайт смотрит вправо. Клавиша отпущена — Animator выключается и показывается `Idle1.ase`. При открытом инвентаре ввод игнорируется, скорость тела равна нулю.

## 7. Настройка в Unity

На Player в `SampleScene` должны быть `Rigidbody2D`, `Collider2D`, `SpriteRenderer` и, при необходимости, `Animator` с клипом ходьбы. Назначь `idleSprite`. В `Player.controller` оставлен только `Player_Walk`; клипы `Player_Idle`/`Player_Run` удалены. Проверка: Play → фокус Game View → WASD/стрелки.

## 8. Возможные проблемы

Без `Rigidbody2D` движение не сработает. Нужен фокус Game View; геймпад и переназначение клавиш не поддерживаются. Если `idleSprite` не назначен, после остановки останется последний кадр Animator. Не добавляй IDLE-клип обратно: контроллер намеренно выключает Animator в покое.

## 9. История изменений

- 2026-10-31 — Animator включается только при движении, в покое показывается `Idle1.ase`; из контроллера удалены Idle/Run.
- 2026-10-12 — добавлена блокировка ввода через `WorldInputGate`.
- 2026-10-11 — базовая документация движения.

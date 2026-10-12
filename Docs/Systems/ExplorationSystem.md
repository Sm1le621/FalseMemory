# Исследование мира и камера

## 1. Назначение

Клавиатурное движение героя по 2D-карте и следящая камера.

## 2. Общая логика

Input System даёт WASD/стрелки; Update нормализует ввод и поворачивает спрайт, FixedUpdate двигает тело. Камера следует в LateUpdate.

## 3. Основные компоненты

PlayerController, CameraFollow, Player Rigidbody2D/Collider2D/SpriteRenderer/Animator, Tilemap/Grid/Borders и EnemyAI.

## 4. Взаимодействие скриптов

PlayerController обновляет только движение/анимацию; камера читает Transform. Transition приостанавливает систему во время боя.

## 5. Последовательность событий

Клавиша → moveInput → MovePosition → коллизии карты → LateUpdate камеры → возможная встреча.

## 6. Настраиваемые параметры

moveSpeed код 4; defaultFacingRight кодtrue. CameraFollow smoothSpeed код 5, offset(0,0,−10), target Player. Фактические значения Inspector могут отличаться от дефолтов кода.

## 7. Использование в Unity

Открыть SampleScene, Play, кликнуть Game View. WASD/стрелки двигают героя; проверить Animator.isMoving и Rigidbody2D. Для камеры назначить target.

## 8. Расширение

Для Gamepad/переназначения нужна отдельная обработка Input Actions; для границ камеры — дополнительная логика. Сохранять движение через физику.

## 9. Ограничения и проверки

2026-10-12, Player rework1: вместо Idle1 подключён исходный myproject1idle.ase (native canvas64, Point, PPU100); физика/PlayerController сохранены. Новый временный статический Idle-клип назначен всем прежним состояниям Animator; это не цикл ходьбы. Свежие81 Edit/42 Play и карта/бой1920×1080 — [PlayerRework](../PlayerRework.md).

Нет gamepad, границ камеры, переходов между картами и взаимодействий RPG. Движение после победы подтверждено тестом; все коллизии/анимации/плавность здесь не проверялись.

Документ описывает текущую рабочую копию на 2026-10-11. Отдельные методы и поля см. в [указателе скриптов](../README.md#описания-скриптов).

# PlayerDirectionFlowTests

## 1. Название и назначение

Исходник `Assets/Tests/PlayMode/PlayerDirectionFlowTests.cs`. Дополнительная часть существующего BattleFlowTests: проверяет directional-ввод/Animator на реальном Player в SampleScene.

## 2. Как работает

Общий UnitySetUp BattleFlowTests загружает SampleScene и временно настраивает Input System. Тест создаёт Keyboard, последовательно посылает восемь сочетаний клавиш и отпускание; после проверяет блокировку ввода открытым Inventory. Общий TearDown возвращает настройки и выгружает тестовые сцены.

## 3. Методы и функции

`EightDirectionsNormalizeInputSelectMotionAndRetainLastFacing()` — UnityTest/IEnumerator: W,S,D,A,WD,WA,SD,SA; проверяет норму1, MoveX/Y, состояние Walk и отсутствие flipX. Отпускание проверяет нулевой ввод, сохранённый LastMoveX/Y и Idle. Проверяет сохранение оригинального временного Sprite, а не наличие восьми уникальных изображений. Открытый Inventory блокирует новый ввод без изменения предыдущего направления. Finally удаляет только синтетическую клавиатуру и закрывает окно; игровой контент не удаляется. Unity Awake/Update у самого теста отсутствуют; общие callbacks описаны в [BattleFlowTests](BattleFlowTests.md).

## 4. Переменные и Inspector

Локальные player/control/animator/keyboard, восемь массивов Key и нормализованных Vector2. Значения не настраиваются в Inspector. Погрешность сравнения float0.0001. Каждое событие ждёт два кадра.

## 5. Связи

PlayerController, Animator/Player.controller, SpriteRenderer, InventoryWindow/WorldInputGate, Input System и Unity Test Framework. Использует Player() и общую изоляцию fixture BattleFlowTests.

## 6. Пример

W+A устанавливает MoveX≈−0.707/MoveY≈0.707, выбирается Player_DirectionalWalk. Отпускание выбирает Player_DirectionalIdle и сохраняет последний вектор. Открытие I не позволяет другим клавишам изменить этот вектор.

## 7. Настройка в Unity

К GameObject не прикрепляется. Window → General → Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests → этот метод. Нужна текущая SampleScene с directional Animator и существующим UI.

## 8. Возможные проблемы

Несохранённый/старый Animator даёт несоответствие параметров/состояний. Синтетический Keyboard возвращается в finally. Тест не доказывает качество walk-графики, восемь уникальных ракурсов, скорость всех возможных столкновений и standalone-сборку. Существующие сценарии отдельно проверяют контакт и возврат из боя.

## 9. История изменений

- 2026-10-12 — добавлена автоматическая проверка восьми направлений и последнего взгляда; статический fallback явно отделён от полноценной анимации.

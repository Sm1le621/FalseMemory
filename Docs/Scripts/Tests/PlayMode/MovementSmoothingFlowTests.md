# MovementSmoothingFlowTests

## 1. Название и назначение

`Assets/Tests/PlayMode/MovementSmoothingFlowTests.cs` — сценарии реальной физики/ввода внутри существующего partial BattleFlowTests, не компонент.

## 2. Как работает

Общий Setup загружает исходную SampleScene и временно разрешает синтетическую клавиатуру без фокуса. Тесты используют реальные Rigidbody2D, Collider2D, Input System и Animator; общий Cleanup восстанавливает сцены/настройки.

## 3. Методы и функции

- `WalkRateCandidatesAdvanceAndTurningKeepsCyclePhase()` — UnityTest/IEnumerator: временно изменяет только runtime animationReferenceSpeed через reflection, чтобы проверить8/10/12/15FPS на том же клипе12FPS; измеряет нормализованную фазу за два цикла, проверяет все8 Sprite и продолжение фазы при D→W. Finally восстанавливает поле и удаляет синтетический Keyboard. Не выбирает художественно лучший FPS и не сохраняет тестовые значения.

- `MovementRampsMaintainsDiagonalSpeedAndModalStopsImmediately()` — IEnumerator/UnityTest: движение D, измерение расстояния/скорости/WalkSpeed, диагональ WD, открытие Inventory, проверка полной остановки. Finally удаляет только тестовый Keyboard/закрывает окно.
- `CollisionStopsActualGaitAndBrakingKeepsFacing()` — создаёт временную тонкую стену только в тестовой сцене, проверяет непреодоление, Idle у стены, уход влево и ограниченный тормозной путь/сохранённый взгляд; finally убирает стену/Keyboard.
- `SkeletonChasesWithPhysicsAndStopsAfterTriggerExit()` — ставит реальные тела в радиус, измеряет подход и смену Sprite, переносит Player за радиус, проверяет остановку и Up Idle.
У тестового файла нет Awake/Start/Update; shared UnitySetUp/TearDown описаны в BattleFlowTests. Методы возвращают IEnumerator, результаты — assertions, а не игровые данные.

## 4. Переменные и Inspector

Локальные body/control/keyboard/wall/start/frames и временные позиции вне карты. Ожидания.1/.12/.4/.5с и допуски расстояния учитывают дискретный fixed step. Настройки Inspector отсутствуют.

## 5. Связи

PlayerController/EnemyAI/EnemySpriteAnimation, реальные тела/коллайдеры, InventoryWindow, WorldInputGate, Animator, Input System/Test Framework; общая fixture BattleFlowTests.

## 6. Пример

Герой упирается в стену толщиной.1: не проходит её и прекращает перебирать ноги. Открытие Inventory посреди диагональной ходьбы полностью останавливает тело.

## 7. Настройка в Unity

Не прикреплять к GameObject. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests; требуются сохранённые SampleScene/Player.controller. Временные объекты теста не сохраняются в проект.

## 8. Возможные проблемы и проверки

Тесты не доказывают художественную естественность, оптимальность12FPS, все возможные препятствия или standalone. Изменение стартового баланса скорости требует осмысленного обновления ожиданий, а не удаления проверок.

## 9. История изменений



- 2026-10-13 — добавлены три проверки перемещения, коллизии/торможения/gate и физического преследования.

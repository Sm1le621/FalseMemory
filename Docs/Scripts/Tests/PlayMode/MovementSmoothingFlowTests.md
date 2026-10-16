# MovementSmoothingFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/MovementSmoothingFlowTests.cs`. Часть BattleFlowTests: реальное перемещение, столкновения, сохранение фазы и связь скорости с Animator.

## 2. Как работает

Общий Setup загружает SampleScene и разрешает синтетическую клавиатуру без фокуса; Cleanup восстанавливает сцены/настройки. Используются настоящие Rigidbody2D/Collider2D/Input System/Animator.

## 3. Методы и функции

- `WalkRateCandidatesAdvanceAndTurningKeepsCyclePhase()` → IEnumerator/UnityTest: временно меняет runtime animationReferenceSpeed через reflection (64/fps для исходных16 FPS при скорости4), последовательно проверяет8/10/12/15/16 эффективных FPS. За два цикла измеряет normalizedTime и наличие8 Sprite; D→W сохраняет фазу. Finally восстанавливает поле/удаляет тестовый Keyboard. Обычная сцена использует reference4 и базовые16, тестовые темпы не сохраняются.
- `MovementRampsMaintainsDiagonalSpeedAndModalStopsImmediately()` → IEnumerator/UnityTest: D, расстояние/ActualSpeed4/WalkSpeed1, WD с той же длиной пути, Inventory полностью останавливает тело. Finally удаляет тестовую клавиатуру/закрывает окно.
- `CollisionStopsActualGaitAndBrakingKeepsFacing()` → IEnumerator/UnityTest: временная стена.1 в тестовой сцене, проверка непреодоления/Idle у стены/ухода влево/тормозного пути до.16 и последнего взгляда; finally уничтожает только тестовую стену/Keyboard.
- `SkeletonChasesWithPhysicsAndStopsAfterTriggerExit()` → IEnumerator/UnityTest: ставит тела в радиус, измеряет подход/смену Sprite/скорость3, переносит Player наружу, проверяет остановку и Up Idle.

Собственных Awake/Start/Update нет; shared UnitySetUp/TearDown описаны в BattleFlowTests. Результаты — assertions.

## 4. Переменные и Inspector

body/control/keyboard/wall/start/frames, временные позиции вне карты, интервалы ожидания и допуски — локальные тестовые значения, не Inspector.

## 5. Связи

PlayerController/EnemyAI/EnemySpriteAnimation, реальные тела/коллайдеры, InventoryWindow, WorldInputGate, Animator, Input System/Test Framework и общая fixture.

## 6. Пример

Клавиша D при обычной скорости4 → WalkSpeed1 →8 кадров за.5 с. Поворот W не начинает цикл с нуля; стена прекращает перебор ног, Inventory останавливает тело.

## 7. Настройка в Unity

Не прикреплять к объекту. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests; требуются исходные сцены/контроллер. Временные объекты не сохраняются.

## 8. Возможные проблемы

Тесты не подтверждают художественную естественность, все типы препятствий или standalone. Базовые16 могут масштабироваться фактической скоростью экипировки/разгона — это намеренная синхронизация дистанции, а не FPS в метаданных.

## 9. История изменений




- 2026-10-13 — добавлены три проверки перемещения, коллизии/торможения/gate и физического преследования.

- 2026-10-16 — актуализированы реальная сетка16 FPS, opt-in тайминг/проверки и ограничения текущего технического подэтапа.

# MovementSmoothingFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/MovementSmoothingFlowTests.cs`. Часть BattleFlowTests: реальная физика, коллизии, темп новых четырёхкадровых Walk и фаза поворота.

## 2. Как работает

Общая fixture использует исходную SampleScene, настоящие Rigidbody/коллайдеры/Input System/Animator; временные значения и устройства восстанавливаются. Новые базовые Walk имеют4 позиции/3 уникальных Sprite/цикл .5с; времена смен неравномерны, на шве две нейтральные позиции объединяются в одну видимую выдержку.

## 3. Методы и функции

- `WalkRateCandidatesAdvanceAndTurningKeepsCyclePhase()` → IEnumerator/UnityTest: временный runtime animationReferenceSpeed=32/fps при скорости4, проверка4/8/12/16 условных позиций в секунду на четырёхпозиционном клипе; за два цикла все3 уникальных Sprite и ожидаемая normalizedTime, затем D→W сохраняет фазу. Finally возвращает reference/удаляет Keyboard. Не сохраняет тестовые темпы.
- `MovementRampsMaintainsDiagonalSpeedAndModalStopsImmediately()` → IEnumerator/UnityTest: разгон/расстояние/скорость4/WalkSpeed1, равная длина пути WD, Inventory полностью останавливает тело.
- `CollisionStopsActualGaitAndBrakingKeepsFacing()` → IEnumerator/UnityTest: временная тонкая стена.1, непреодоление/Idle у стены, уход влево, тормозной путь≤.16/взгляд. Finally уничтожает только тестовую стену/Keyboard.
- `SkeletonChasesWithPhysicsAndStopsAfterTriggerExit()` → IEnumerator/UnityTest: реальный trigger/подход/смена Sprite/скорость3, выход Player за радиус и остановка/Up Idle.

Собственных Awake/Start/Update нет; общие Setup/Cleanup — BattleFlowTests.

## 4. Переменные и Inspector

Ссылки/временные позиции/ожидания/допуски и reference reflection — тестовые, не Inspector. Формула темпа обновлена из-за нового числа/удержания рисунков; физические допуски не ослаблены.

## 5. Связи

PlayerController/EnemyAI/EnemySpriteAnimation, Rigidbody2D/Collider2D, Inventory/WorldInputGate, Animator/Input System/Test Framework.

## 6. Пример

Обычная скорость4/reference4 →WalkSpeed1 →4 кадра за.5с. Более медленный физический шаг замедляет Walk, поворот не начинает петлю заново, стена прекращает перебор ног.

## 7. Настройка в Unity

Не компонент. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests, нужны сохранённые новые clips/controller/Player flag.

## 8. Возможные проблемы

Численная проверка не доказывает естественность шага или всех столкновений. Samples16 — сетка времени, обычный Walk имеет2 цикла в секунду с неравными выдержками. Тест проверяет темп normalizedTime, а не равномерность смены рисунков. [Результаты](../../../PlayerWalkOrder.md).

## 9. История изменений




- 2026-10-13 — добавлены три проверки перемещения, коллизии/торможения/gate и физического преследования.

- 2026-10-16 — актуализированы реальная сетка16 FPS, opt-in тайминг/проверки и ограничения текущего технического подэтапа.

- 2026-10-26 — актуализирован новый ограниченный набор4×4 Idle/Walk, выбор cardinal-взгляда/импорт и соответствующие проверки. Фактический статус выполнения — в PlayerCardinal4.

- 2026-10-26 — через Unity MCP подключён четырёхпозиционный Walk: нейтральная стойка → RightStep → LeftStep → та же стойка; обновлены контракты и реальные Play-проверки. Подробности текущей проверки — в PlayerWalkOrder.

# PlayerDirectionFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/PlayerDirectionFlowTests.cs`. Часть BattleFlowTests: нормализованное движение, переключение и последний cardinal-взгляд.

## 2. Как работает

Общая fixture загружает исходную SampleScene и разрешает синтетическую клавиатуру; finally удаляет тестовое устройство/закрывает Inventory. Физическое движение всё ещё проверяется по восьми сочетаниям клавиш, визуальных направлений только четыре.

## 3. Методы и функции

- `EightDirectionsNormalizeInputSelectMotionAndRetainLastFacing()` → IEnumerator/UnityTest: W/S/D/A и4 диагонали, ожидание.12с, MovementInput нормализован/равен ожидаемому; Animator Move/LastMove получает cardinal (диагональ →Up/Down), Walk/Idle состояния/flipfalse. LastDirection остаётся истинным нормализованным вектором. Idle Sprite имеет правильный prefix всех четырёх направлений; Inventory блокирует ввод и сохраняет последнее направление.
- `ApprovedWalkChangesFramesAndStoppingKeepsDirectionalIdle()` → IEnumerator/UnityTest: перенос в свободное место, S, ожидание реального Walk, сбор.85с — ровно3 уникальных Sprite (стойка и два шага, 4 позиции)/без flicker в Idle; отпускание возвращает Idle Down, MovementSpeed остаётся4.

Собственных Awake/Update нет; shared Setup/Cleanup описаны в BattleFlowTests.

## 4. Переменные и Inspector

Keyboard/keys/directions/facing/names/shown — временные данные, не Inspector. Physics-направления нормализованы, cardinal-параметры целочисленные ±1/0.

## 5. Связи

PlayerController/useCardinalAnimations, Input System, Animator/Player.controller, SpriteRenderer, InventoryWindow/WorldInputGate, общая BattleFlowTests fixture.

## 6. Пример

S+A движет вниз-влево без ускорения; герой показывает Walk Down, после отпускания Idle Down, при открытом Inventory не начинает шаг.

## 7. Настройка в Unity

Не компонент. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests. Нужны реально сохранённые4-direction controller/Player flag.

## 8. Возможные проблемы

Низкая частота рендера может пропускать Sprite. Тест не подтверждает естественную анатомию/плащ. [Текущие результаты](../../../PlayerWalkOrder.md); прежние8-frame ожидания заменены новым запросом, физические проверки сохранены.

## 9. История изменений

- 2026-10-12 — добавлена автоматическая проверка восьми направлений и последнего взгляда; статический fallback явно отделён от полноценной анимации.

- 2026-10-13 — проверяется настоящая directional-графика ApprovedHero; добавлена проверка смены восьми кадров и неизменной скорости.

- 2026-10-13 — актуальная проверка восьми walk-кадров вместо четырёх.

- 2026-10-13 — актуализированы темп/физическое воспроизведение и проверки в задаче Animation Rework.

- 2026-10-13 — проверка нового30-frame Idle_Down без ослабления прежних направлений/движения.

- 2026-10-26 — актуализирован новый ограниченный набор4×4 Idle/Walk, выбор cardinal-взгляда/импорт и соответствующие проверки. Фактический статус выполнения — в PlayerCardinal4.

- 2026-10-26 — через Unity MCP подключён четырёхпозиционный Walk: нейтральная стойка → RightStep → LeftStep → та же стойка; обновлены контракты и реальные Play-проверки. Подробности текущей проверки — в PlayerWalkOrder.

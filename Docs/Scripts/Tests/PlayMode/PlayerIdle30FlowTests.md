# PlayerIdle30FlowTests — текущие cardinal-циклы

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/PlayerIdle30FlowTests.cs`. Историческое имя файла сохранено; теперь проверяет четыре разных Idle Sprite и четыре позиции Walk для каждого из четырёх направлений по новому заданию. Часть partial BattleFlowTests.

## 2. Как работает

Общая fixture загружает SampleScene; тест переносит Player в свободную область, последовательно нажимает S/A/D/W, наблюдает реальную смену Sprite и отпускает клавиши. Сохранённая сцена не перезаписывается.

## 3. Методы и функции

`FourCardinalIdleAndWalkCyclesShowAllFramesWithoutIdlePhysicalSteps()` → IEnumerator/UnityTest: для каждого направления ждёт Walk, за1.1с собирает3 правильных уникальных Sprite и список изменений; требует в списке непрерывный порядок neutral|RightStep|LeftStep|neutral; отпускает и ждёт.12с торможения, затем2.2с собирает4 Idle Sprite с правильным prefix. При каждом Idle кадре Rigidbody неподвижен с допуском.0001, после цикла сохранены LastDirection/Idle state/WalkSpeed0. Finally удаляет только тестовый Keyboard. Собственных Awake/Update нет, общие UnitySetUp/TearDown в BattleFlowTests.

## 4. Переменные и Inspector

player/controller/animator/body/visual/keyboard, keys S/A/D/W, names/directions, HashSet и таймеры — локальные, не Inspector. В каждом направлении наблюдаются4 разных Idle и3 уникальных Walk Sprite; совпадение первой/четвёртой позиции Walk разрешено владельцем. changes/previous сохраняют реальный порядок смен, а не только множество ссылок.

## 5. Связи

PlayerController, реальные Rigidbody2D/Animator/SpriteRenderer, Input System и shared fixture. Новые Player64 cardinal-клипы/флаг должны быть назначены перед запуском.

## 6. Пример

A →стойка/RightStep/LeftStep/стойка в Walk Left →отпускание→4 Idle Left без шагов физического корня; следующий D даёт Right, прежние другие ракурсы не смешиваются.

## 7. Настройка в Unity

Не прикреплять к GameObject. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests.

## 8. Возможные проблемы

Папка/имя файла30 не означает требование30 кадров: оно отменено владельцем. Низкий render FPS может пропустить фазы. Неподвижный root не доказывает художественную неподвижность ботинок; это проверяется просмотром. [Результаты](../../../PlayerWalkOrder.md).

## 9. История изменений

- 2026-10-13 — добавлена текущая проверка30-frame Idle_Down, отдельности от Walk и физической неподвижности.

- 2026-10-26 — актуализирован новый ограниченный набор4×4 Idle/Walk, выбор cardinal-взгляда/импорт и соответствующие проверки. Фактический статус выполнения — в PlayerCardinal4.

- 2026-10-26 — через Unity MCP подключён четырёхпозиционный Walk: нейтральная стойка → RightStep → LeftStep → та же стойка; обновлены контракты и реальные Play-проверки. Подробности текущей проверки — в PlayerWalkOrder.

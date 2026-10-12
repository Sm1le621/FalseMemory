# DetailedAnimationFlowTests

## 1. Название и назначение

`Assets/Tests/PlayMode/DetailedAnimationFlowTests.cs` — дополнительная часть BattleFlowTests, проверяет реальное воспроизведение в существующих сценах.

## 2. Как работает

Общий Setup загружает SampleScene; первый сценарий запускает BattleScene и наблюдает UI Sprite, второй перемещает существующего врага в тестовой копии сцены. Общий Cleanup восстанавливает временные настройки и выгружает тестовый мир.

## 3. Методы и функции

`WaitingActorsAnimateIdleAndBothAttacksReturnToIdleWithSingleDamage()` — UnityTest/IEnumerator: сначала открывает Inventory и проверяет отдельный portraitSprite/высокий rect, затем запускает бой; при ожидании за1.1 секунды собирает4 Idle героя/врага; принимает одну Attack/отклоняет повтор, наблюдает минимум6 attack-кадров каждого за5 секунд, ждёт новый ход игрока и проверяет HP50/96, MP30, возврат обоих в Idle и завершение IsAnimating. Ускоренные правила временно заменены штатным темпом. `SkeletonSelectsCardinalViewsRetainsFacingAndBridgesPhysicsGaps()` — UnityTest/IEnumerator: выключает AI/симуляцию Rigidbody только в fixture, перемещает по четырём направлениям, проверяет Walk/no double flip/удержание Walk между кадрами, затем Idle последнего направления и наличие двух коллайдеров. Unity Awake/Update у тестов отсутствуют; callbacks Setup/Cleanup описаны в BattleFlowTests.

## 4. Переменные и Inspector

Локальные HashSet Sprite, ссылки manager/player/enemy, timeout5 секунд и временные actionDelay.8/шанс защиты0/crit/miss0. Не параметры Inspector. Арт/скорость/коллайдеры сохранённой сцены тест не переписывает.

## 5. Связи

BattleFlowTests fixture/BeginBattle, BattleManager/Presentation/UI, SpriteAnimationSet, EnemySpriteAnimation/EnemyAI/Rigidbody2D/Health.

## 6. Пример

Attack → hero frames → единственный урон10 → Skeleton Attack → единственный урон4 → оба Idle и следующий ход. Блокировка повторной команды исключает дублирование урона независимо от кадров.

## 7. Настройка в Unity

Не компонент. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests. Требуются сохранённые SampleScene/BattleScene с новыми профилями.

## 8. Возможные проблемы

Слишком низкая частота кадров может пропустить промежуточные позы; timeout/минимум6 отделяет исполнение от художественного качества. Визуальная естественность, standalone и дисковые сохранения этим тестом не подтверждаются.

## 9. История изменений

- 2026-10-13 — два сценария battle idle/attack/возврата/ресурсов и четырёх направлений/физических интервалов.

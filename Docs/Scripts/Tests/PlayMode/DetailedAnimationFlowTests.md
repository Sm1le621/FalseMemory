# DetailedAnimationFlowTests

## Текущее изменение5.3 —2026-10-26

Проверка8 Attack/16FPS/контакт4/одного урона не менялась; отсутствие расходаMP теперь означает40 вместо30. Другие анимационные ожидания сохранены.

Фактические проверки и ограничения: [ClasslessStage53](../../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/DetailedAnimationFlowTests.cs`. Часть partial BattleFlowTests: реальное воспроизведение боевых Image, урон/ресурсы и направление скелета.

## 2. Как работает

Общая fixture загружает SampleScene/BattleScene, включает разрешённый синтетический ввод и восстанавливает временные значения/сцены в Cleanup. Три сценария наблюдают настоящие Sprite/HP.

## 3. Методы и функции

- `HeroAttackUsesSixteenFpsAndAppliesDamageAtContactOnce()` → IEnumerator/UnityTest: запускает бой при actionDelay.8/crit/miss0, отключает Space и временно задаёт старый attackDuration.9. Проверяет useFrameTiming/16 FPS, наблюдает все8 Sprite в конце кадров, измеряет интервалы1/16 с и весь цикл.5 с с допуском рендер-шага, проверяет единственное изменение HP, контактный индекс4, HP50/MP30. Доказывает, что legacy duration не растягивает новую атаку. Изменяется только тестовый компонент сцены.
- `WaitingActorsAnimateIdleAndBothAttacksReturnToIdleWithSingleDamage()` → IEnumerator/UnityTest: проверяет сохранённый высокий портрет Inventory,4 Idle героя/врага за1.1 с; одну принятую Attack/отклонение повторной; минимум6 видимых Attack-кадров каждого, следующий ход, HP50/96, MP30, оба Idle/IsAnimatingfalse.
- `SkeletonSelectsCardinalViewsRetainsFacingAndBridgesPhysicsGaps()` → IEnumerator/UnityTest: временно отключает AI/simulation, перемещает Enemy по четырём направлениям, проверяет Walk/no flip/отсутствие flicker между физическими шагами, затем Idle последнего взгляда и два коллайдера.

Awake/Update отсутствуют; shared UnitySetUp/TearDown — в BattleFlowTests.

## 4. Переменные и Inspector

Локальные HashSet/Dictionary Sprite→время, текущий/предыдущий HP, счётчик изменений, deadline3/5 с. Допуск нового теста max(.035, renderDelta+.01) для интервалов, вдвое для цикла. Не Inspector; сохранённый профиль/сцена тестами не перезаписываются.

## 5. Связи

Общая BattleFlowTests fixture/BeginBattle, BattleManager/Presentation/UI, SpriteAnimationSet, Health/MP, EnemySpriteAnimation/EnemyAI/Rigidbody2D.

## 6. Пример

При legacy attackDuration.9 герой всё равно показывает8 кадров за.5 с и наносит10 HP ровно на кадре4. Ответ скелета и возврат проверяются отдельным полным сценарием.

## 7. Настройка в Unity

Не компонент. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests; сохранённые SampleScene/BattleScene обязательны.

## 8. Возможные проблемы

RenderFPS ниже16 может не показать все8 кадров и привести к честному провалу проверки. Интервалы включают дискретность рендера; это не тест естественности поз, standalone или сохранений на диск.

## 9. История изменений


- 2026-10-13 — два сценария battle idle/attack/возврата/ресурсов и четырёх направлений/физических интервалов.

- 2026-10-16 — актуализированы реальная сетка16 FPS, opt-in тайминг/проверки и ограничения текущего технического подэтапа.

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

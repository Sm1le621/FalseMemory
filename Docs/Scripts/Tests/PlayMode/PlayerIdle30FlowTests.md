# PlayerIdle30FlowTests

## 1. Название и назначение

`Assets/Tests/PlayMode/PlayerIdle30FlowTests.cs` — проверяет отдельный Idle_Down на реальном Player; часть существующей partial fixture BattleFlowTests, не компонент.

## 2. Как работает

Shared Setup загружает SampleScene, временно разрешает синтетический Input System. Без клавиш наблюдает4.2с (больше двух Idle циклов), затем включает S и отпускает. Shared TearDown возвращает сцены/настройки.

## 3. Методы и функции

IdleDownPlaysThirtyFramesWithoutPhysicalStepsAndResumesAfterWalking() — UnityTest/IEnumerator: переносит тело в свободную тестовую область, наблюдает SpriteRenderer и Idle-state, собирает все30 разных имён, проверяет неподвижность Rigidbody и WalkSpeed0 при продолжающемся Idle. S на.2с должно реально двигать тело/показывать прежний Walk; отпускание.12с возвращает Down Idle и взгляд. Finally удаляет только тестовый Keyboard. У файла нет Awake/Update/физических callbacks; UnitySetUp/TearDown в BattleFlowTests.

## 4. Переменные и Inspector

player/controller/animator/body/visual/keyboard/start/shown/until — локальные тестовые значения; Inspector отсутствует. Допуск неподвижности.0001 world units,4.2с наблюдения/30 имён. Runtime настройки не сохраняются.

## 5. Связи

PlayerController, Player.controller/Idle_Down/Walk_Down, Rigidbody2D/SpriteRenderer, Input System/Test Framework, общая BattleFlowTests fixture. Ресурсы HP/MP не меняет.

## 6. Пример

Стоящий герой перебирает30 дыхательных Sprite, сохраняя одну физическую позицию. После короткой ходьбы вниз возвращается именно в новую Idle-анимацию, не в один walk-кадр.

## 7. Настройка в Unity

Не прикреплять. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests → метод. Нужны сохранённый новый Idle_Down и SampleScene; клавиатура тестовая, реальный фокус не обязателен благодаря fixture.

## 8. Возможные проблемы и проверки

Низкий render FPS может пропускать Sprite между наблюдениями; тест ждёт два полных цикла. Количество кадров/неподвижный root не доказывают спокойное дыхание, неизменность лица/броневых деталей или отсутствие визуального jitter. Эти критерии оцениваются отдельно в Play/preview владельцем. Standalone не проверяется.

## 9. История изменений

- 2026-10-13 — добавлена текущая проверка30-frame Idle_Down, отдельности от Walk и физической неподвижности.

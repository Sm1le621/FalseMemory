# SceneTransitionFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/SceneTransitionFlowTests.cs`.

PlayMode-проверка перехода `SampleScene` → `Dungeon` → `SampleScene` через настоящие триггеры входа в пещеру и выхода `CaveExit`.

## 2. Как работает

`Setup` загружает `SampleScene` в режиме Single. Тест ставит Player в центр триггера входа, ждёт загрузки `Dungeon`, проверяет Player и выход, ждёт окончания защиты после прибытия, ставит Player в триггер выхода и ждёт возврата в `SampleScene`. `Cleanup` уничтожает `SceneTransitionManager`, создаёт пустую сцену-якорь и выгружает остальные сцены, чтобы другие тесты начинали с чистого состояния.

## 3. Методы и функции

- `Setup()` — `[UnitySetUp]`, Single-загрузка `SampleScene`.
- `Cleanup()` — `[UnityTearDown]`, удаление менеджера и выгрузка сцен.
- `CaveEntranceAndExitCarrySamePlayerBothWays()` — `[UnityTest]`: HP 100→90 сохраняется в обе стороны; в каждой сцене ровно один `PlayerController` и это тот же объект; в `Dungeon` Player в `(0,0)`; выход ведёт в `SampleScene`, стоит выше точки появления, его триггер не пересекается с коллайдером Player, спрайт выхода — `Cave`; после возврата одна `EventSystem`, `BattleTransition.Instance` активен, Player под входом и вне его триггера, через 0,3 с сцена всё ещё `SampleScene`. Любая ошибка в Console (например, два Global Light 2D) проваливает тест.
- `FindTransition(string targetScene)` — находит `SceneTransition` по `TargetSceneName`.
- `MovePlayerInto(PlayerController, SceneTransition)` — ставит Rigidbody2D Player так, чтобы центр его коллайдера совпал с центром триггера.
- `CountPlayers()` — количество активных `PlayerController`.
- `WaitForScene(string sceneName)` — ждёт активную сцену и конец `IsTransitioning` (тайм-аут 10 с), затем ещё кадр.

## 4. Переменные и Inspector

`Timeout = 10` секунд. Inspector нет.

## 5. Связи

`SceneTransition`, `SceneTransitionManager`, `PlayerController`, `Health`, `BattleTransition`, `EventSystem`, сцены `SampleScene` и `Dungeon` из Build Profiles.

## 6. Пример

Player с 90 HP входит в пещеру → `Dungeon`, `(0,0)` → через 0,6 с входит в `CaveExit` → `SampleScene`, `(7, 3.4)`, 90 HP.

## 7. Настройка в Unity

Компонент не прикрепляется. Test Runner → PlayMode. Нужны сохранённые `Cave/DoorTriger` в `SampleScene` и `CaveExit/DoorTriger` в `Dungeon`.

## 8. Возможные проблемы

Тест использует Single-загрузку, поэтому сам выгружает сцены в `Cleanup`. Если переставить вход/выход или точки появления так, что Player появится в триггере, тест провалится. Визуальную картинку и плавность камеры тест не оценивает.

## 9. История изменений

- 2026-11-20 — тест создан; PlayMode 38/38. Во время первого запуска выявил ошибку URP «More than one global light» при возврате в `SampleScene`, исправлено в `SceneTransitionManager`.

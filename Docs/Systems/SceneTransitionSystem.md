# Система переходов между сценами

Актуально на **2026-11-20**. Документ описывает перенос Player и его интерфейса из `SampleScene` в `Dungeon` и обратно.

## 1. Назначение

`SceneTransition` превращает объект-триггер (вход в пещеру или дверь) в точку перехода. `SceneTransitionManager` загружает целевую сцену и переносит в неё существующего Player с текущими HP/MP, инвентарём и экипировкой. Копии Player и HUD не создаются, поэтому здоровье и мана не сбрасываются.

## 2. Общая логика

1. `SceneTransition.OnTriggerEnter2D` проверяет, что вошёл объект с тегом `Player`, и что сейчас нет боя или другого перехода.
2. `SceneTransition` вызывает `SceneTransitionManager.GetOrCreate().ChangeScene(...)`.
3. Менеджер собирает корневые объекты Player и интерфейса и временно помещает их в `DontDestroyOnLoad`.
4. `EventSystem`, `PlayerController` и перенесённый Global Light 2D выключаются, чтобы во время загрузки не было второго ввода и движения.
5. Сцена загружается в режиме `LoadSceneMode.Single`.
6. После `sceneLoaded` менеджер удаляет из новой сцены её собственные копии Player, HUD, Canvas, EventSystem, `BattleTransition` и Global Light 2D.
7. Перенесённые объекты возвращаются в новую сцену через `SceneManager.MoveGameObjectToScene`.
8. Player ставится в заданную точку появления, камера новой сцены привязывается через `CameraFollow`.
9. Управление, EventSystem и свет включаются обратно. Следующие 0,5 секунды новые переходы игнорируются (`IsArrivalProtected`).

HP и MP сохраняются потому, что это те же самые компоненты `Health` и `BattleUnit`. `Health.Start` и `BattleUnit.Awake` у уже созданных объектов повторно не вызываются.

## 3. Основные компоненты

- `SceneTransition` (`Assets/Scripts/Dangeon/SceneTransition.cs`) — триггер входа и выхода.
- `SceneTransitionManager` (`Assets/Scripts/Dangeon/SceneTransitionManager.cs`) — загрузка сцены и перенос объектов.
- `Player`, `Health`, `ManaHUD`, `InventoryCanvas`, `EventSystem`, `BattleTransition`, Global Light 2D — переносимые корневые объекты.
- `CameraFollow` — получает перенесённого Player в целевой сцене.
- Build Settings — целевая сцена должна быть в списке сцен.

## 4. Взаимодействие скриптов

`SceneTransition` не загружает сцену сам. Он только проверяет условия и передаёт работу `SceneTransitionManager`. Менеджер живёт в `DontDestroyOnLoad` и создаётся автоматически при первом переходе, поэтому добавлять его в сцену вручную не нужно.

`BattleTransition` остаётся владельцем боя: если идёт встреча или затемнение, `SceneTransition` не начинает переход. Во время обычного перехода `BattleTransition` тоже переносится в новую сцену, чтобы враги работали и там.

## 5. Последовательность событий

Player касается триггера пещеры → `SceneTransition` проверяет тег и состояние боя → `SceneTransitionManager.ChangeScene` → загрузка `Dungeon` → удаление дубликатов → перенос Player/HUD/инвентаря/EventSystem/`BattleTransition`/света → размещение Player в `spawnPosition` → камера следует за Player → управление возвращается.

Обратно: Player касается `CaveExit` в `Dungeon` (через 0,5 с после прибытия) → загрузка `SampleScene` → копии Player/HUD/EventSystem/`BattleTransition`/света из `SampleScene` удаляются → тот же Player появляется в `(7, 3.4)` под входом в пещеру.

## 6. Настраиваемые параметры

- `SceneTransition.targetSceneName` — имя сцены; по умолчанию `Dungeon`.
- `SceneTransition.useSpawnPosition` — ставить ли Player в точку появления; по умолчанию включено.
- `SceneTransition.spawnPosition` — координаты появления; по умолчанию `(0, 0)`.
- `CameraFollow.smoothSpeed` — скорость догоняния камеры после перехода.

## 7. Использование в Unity

1. Откройте `SampleScene` и найдите объект входа в пещеру.
2. Добавьте на него `BoxCollider2D` с включённым `Is Trigger`.
3. Добавьте компонент `SceneTransition`.
4. В `Target Scene Name` укажите `Dungeon`.
5. Проверьте `Use Spawn Position` и при необходимости задайте `Spawn Position`.
6. Убедитесь, что Player имеет тег `Player`, `Rigidbody2D` и `Collider2D`.
7. Добавьте `Dungeon` в Build Profiles → Scene List.
8. Сохраните сцену.

В `Dungeon` ничего добавлять не нужно: менеджер создаётся в рантайме. В сцене должна быть камера; если у неё нет `CameraFollow`, менеджер добавит его. Окружение `Dungeon` (`Grid`, `Rocks`) остаётся на месте: менеджер удаляет только дубликаты Player, HUD, Canvas, EventSystem, `BattleTransition` и Global Light 2D.

## 8. Расширение

- Обратный переход уже есть: `CaveExit` в `Dungeon` (тот же спрайт `Cave`, точка `(0,2)`, `Target Scene Name = SampleScene`, `Spawn Position = (7, 3.4)`).
- Несколько входов: создайте несколько триггеров с разными `spawnPosition`.
- Плавное затемнение: можно подключить существующий `BattleScreenFade` или отдельный overlay. Сейчас переход выполняется без fade.

## 9. Ограничения и проверки

- При `LoadSceneMode.Single` старая сцена выгружается. Враги, побеждённые ранее, и объекты мира в `SampleScene` при возврате создаются заново; дискового сохранения мира нет.
- Переносятся только корневые объекты, которые менеджер распознаёт как Player или интерфейс. Новые корневые объекты другого типа останутся в старой сцене.
- Если в целевой сцене уже есть собственный Player, HUD, Canvas, EventSystem, `BattleTransition` или Global Light 2D, они будут удалены как дубликаты.
- 2026-11-20: переход туда и обратно через настоящие триггеры проверен PlayMode-тестом `SceneTransitionFlowTests` (один Player, HP сохраняется, одна EventSystem, `BattleTransition` активен, нет мгновенного возврата, Console без ошибок). Визуальная проверка остаётся за владельцем.

## История

- 2026-11-20 — добавлен выход `CaveExit` из `Dungeon` в `SampleScene`, защита после прибытия и выключение перенесённого света на время загрузки.

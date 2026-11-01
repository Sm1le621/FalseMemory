# BattleTransition

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleTransition.cs`.

Единственный координатор перехода «карта → бой → карта». Сохраняет ссылки на исходных `Player`/`Enemy`, приостанавливает исследование, аддитивно загружает `BattleScene`, а после победы/побега возвращает мир без создания копий персонажа и без снимков HP.

## 2. Как работает

`Awake` устанавливает singleton `Instance` (повторный экземпляр отключается), `OnDestroy` очищает ссылку. `TryBegin` проверяет, что игра запущена, встреча ещё не активна, сцена боя доступна в Build Settings, участники живы и находятся в одной сцене с координатором. Затем синхронно ставит `EncounterActive`, сбрасывает защиту участников, выключает `PlayerController`, `EnemyAI`, `EnemyAttack`, останавливает тела и запускает `EnterBattle`.

`EnterBattle` затемняет экран, скрывает корневые объекты мира, аддитивно загружает бой, делает его активной сценой и проявляет экран. При победе `ReturnAfterVictory` выгружает бой, скрывает побеждённого врага и возвращает мир. При побеге `ReturnAfterEscape` дополнительно даёт защиту от повторной встречи, выключает AI врага на время `fleeProtectionSeconds` и ищет безопасную точку для игрока. `RestartGame`/`ReturnToMainMenu` загружают сцену заново через `BattleScreenFade`.

## 3. Методы и функции

- `TryBegin(BattleUnit player, BattleUnit enemy)` → `bool`; запускает встречу, если все проверки пройдены. Повторный вызов во время боя возвращает `false`.
- `EnterBattle()` — coroutine загрузки боевой сцены; при ошибке загрузки восстанавливает мир и снимает флаги.
- `ReturnAfterVictory()` → `bool`; разрешён только при победе (враг мёртв, игрок жив).
- `ReturnAfterEscape()` → `bool`; разрешён, когда оба участника живы.
- `ReturnToWorld(bool victory)` — coroutine выгрузки боя и восстановления мира; при побеге размещает игрока и включает защиту.
- `PlacePlayerSafely()` — ищет свободную точку вокруг врага с учётом коллайдера игрока; если не находит, оставляет исходную позицию.
- `IsFree(...)` — проверяет пересечение с нетриггерными коллайдерами.
- `RestoreWorld(bool victory)` — выключает побеждённого врага, включает скрытые корни, возвращает `enabled` приостановленным компонентам и сбрасывает защиту.
- `RestartGame()` / `ReturnToMainMenu()` — запускают `LoadFresh` через `BattleScreenFade`.
- `FadeTo(float alpha)` — coroutine затемнения/проявления.
- `ReleaseProtectionIfExpired()` — в `LateUpdate` возвращает AI врага после истечения защиты.
- `ResetStatics()` — `RuntimeInitializeOnLoadMethod`, очищает singleton при входе в Play Mode.
- `IsEncounterProtected` — bool; защита от повторного контакта после побега.

## 4. Переменные и Inspector

- `rules` — обязательные правила боя (`BattleRules`).
- `battleSceneName` — имя боевой сцены, по умолчанию `BattleScene`.
- `mainMenuSceneName` — сцена главного меню, по умолчанию `MainMenu`.
- `fade` — `Image` затемнения; обязательна для переходов.
- `Player`, `Enemy`, `EncounterActive`, `IsTransitioning` — публичные свойства только для чтения.
- `worldScene`, `hiddenRoots`, `pausedBehaviours`, `protectedUntil`, `protectedEnemyAI`, `protectedEnemyAIWasEnabled` — внутреннее состояние.
- Собственных числовых параметров нет: длительности и дистанции берутся из `BattleRules`.

## 5. Связи

Компонент находится в `SampleScene` на отдельном объекте-координаторе. `BattleEncounter` вызывает `TryBegin`. `BattleManager` использует `Player`, `Enemy`, `rules`, `ReturnAfterVictory`, `ReturnAfterEscape`, `RestartGame`, `ReturnToMainMenu`. Требуются `BattleScene` и `MainMenu` в Build Settings, `BattleScreenFade`, а также компоненты Player/Enemy.

## 6. Пример

Игрок касается скелета. `BattleEncounter` вызывает `TryBegin`; мир скрывается, загружается `BattleScene`. После победы кнопка `Return to Map` вызывает `ReturnAfterVictory`: бой выгружается, скелет выключается, игрок остаётся на прежнем месте с текущими HP/MP. После побега игрок оказывается в безопасной точке, а скелет ненадолго не преследует его.

## 7. Настройка в Unity

Объект с компонентом должен находиться в той же сцене, что Player и Enemy. Назначь `rules` и `fade`. Добавь `BattleScene` и `MainMenu` в Build Settings. Не создавай второй координатор и не копируй Player в боевую сцену.

## 8. Возможные проблемы

Если `BattleScene` отсутствует в Build Settings, `TryBegin` пишет ошибку и бой не начинается. Без `fade` переход не выполнится. Участники из разных сцен не принимаются. При побеге нужно свободное место; иначе игрок остаётся на исходной позиции, но защита от врага всё равно действует. После победы враг выключается и не возрождается до перезагрузки сцены.

## 9. История изменений

- 2026-10-31 — документ приведён к текущему коду; удалены упоминания опыта и роста, добавлены `PlacePlayerSafely` и защита после побега.
- 2026-10-11 — аддитивная загрузка боя и сохранение объектов мира.

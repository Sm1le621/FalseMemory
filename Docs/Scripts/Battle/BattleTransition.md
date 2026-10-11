# BattleTransition

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleTransition.cs.

Единственный координатор перехода мир↔бой, сохранения объектов сессии и защиты после побега.

## 2. Как работает

TryBegin синхронно блокирует повторную встречу, выключает движение/AI и загружает BattleScene дополнительно. Мир скрывается, но не выгружается. После победы враг выключается; после побега остаётся жив и включается временная защита. Поражение может полностью загрузить новую игру/меню.

## 3. Методы и функции

- `ResetStatics()` — RuntimeInitializeOnLoadMethod SubsystemRegistration очищает Instance при запуске; void.
- `Awake()` — Unity назначает Instance; лишний экземпляр выключает себя. `OnDestroy()` очищает Instance, если это он.
- `LateUpdate()` — каждый кадр вызывает ReleaseProtectionIfExpired; тот восстанавливает прежний enabled EnemyAI после таймера.
- `TryBegin(BattleUnit player,BattleUnit enemy)` → bool; проверяет Play, правила, живых, общую сцену, доступность BattleScene, отсутствие встречи/защиты; сохраняет участников, Pause/StopBody, StartCoroutine EnterBattle.
- `Pause(Behaviour behaviour)` — запоминает enabled и выключает при наличии; `StopBody(BattleUnit unit)` — обнуляет линейную/угловую скорость тела; void.
- `EnterBattle()` — coroutine FadeTo, скрытие активных roots кроме собственного, LoadSceneAsync Additive, SetActiveScene, затемнение назад; при неудаче загрузки RestoreWorld.
- `ReturnAfterVictory()` → bool; живой Player, мёртвый Enemy, активная встреча → coroutine ReturnToWorld(true).
- `TryGrantVictoryExperience(out int gained)` → bool; проверяет победу/однократность, помечает награду выданной и AddExperience; gained — фактически добавленное.
- `ReturnAfterEscape()` → bool; живые оба и активная встреча → ReturnToWorld(false).
- `ReturnToWorld(bool victory)` — coroutine выгрузки BattleScene, RestoreWorld, при побеге таймер/пауза AI/PlacePlayerSafely, завершение перехода и очистка ссылок.
- `PlacePlayerSafely()` — void: проверяет bounds коллайдера и ищет свободный пункт в восьми направлениях на двух радиусах, меняет позицию только Player.
- `IsFree(Vector2 center,Vector2 size,int mask)` → bool по OverlapBoxAll; игнорирует trigger и собственные коллайдеры.
- `RestoreWorld(bool victory)` — активирует сохранённые roots, возвращает enabled Behaviour, снимает Defend; победный враг остаётся отключённым.
- `RestartGame()` / `ReturnToMainMenu()` — вызывают LeaveFor с миром/меню; void.
- `LeaveFor(string scene)` — проверяет доступность/переход, запускает LoadFresh; `LoadFresh(string scene)` — coroutine FadeTo и загрузки Single новой сессии.
- `FadeTo(float alpha)` — coroutine: создаёт экран BattleFade при необходимости и меняет прозрачность за fadeDuration; alpha использовать 0…1.

## 4. Переменные и параметры Inspector

rules — обязательный BattleRules; battleSceneName/mainMenuSceneName — строки BattleScene/MainMenu, сцены должны быть в сборке. Player/Enemy/EncounterActive/IsTransitioning/IsEncounterProtected — читаемое состояние. hiddenRoots/pausedBehaviours/worldScene/protectedUntil/protectedEnemyAI/victoryRewardClaimed/fade — внутренние.

## 5. Связи с другими системами

Объект BattleTransition в SampleScene; BattleEncounter вызывает старт, Manager — исход. Сохраняет Health/MP/Inventory/EXP на настоящем Player. Работает с PlayerController, EnemyAI/EnemyAttack, Collider2D и правилами.

## 6. Пример работы

Flee успешен → BattleScene выгружена → Player отнесён в свободное место → скелет жив с прежними HP →3 секунды без повторной встречи → вновь может начать бой.

## 7. Возможные проблемы и проверки

Защита блокирует все встречи этого координатора, не только прежнего врага. Без свободного места оставляет Player на месте с таймером. Сохранения на диск нет, Single/перезагрузка мира сбрасывает сессию. Один координатор на сцену; дубликат не удаляется, только выключается. Победа/побег/повторный бой и сохранение покрыты Play Mode.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

# SceneTransitionManager

## 1. Название и назначение

**Исходный файл:** `Assets/Scripts/Dangeon/SceneTransitionManager.cs`.

Единый менеджер перехода между сценами. Он загружает новую сцену и переносит в неё существующего Player вместе с HP/MP-полосами, инвентарём, окном инвентаря, EventSystem, `BattleTransition` и Global Light 2D. Новые копии Player/HUD в целевой сцене не создаются.

## 2. Как работает

Менеджер создаётся автоматически при первом вызове `SceneTransitionManager.GetOrCreate()` и живёт в `DontDestroyOnLoad`.

Последовательность перехода:

1. `ChangeScene` проверяет имя сцены, наличие её в Build Settings и блокирует повторный запуск.
2. `CollectCarriedRoots` собирает корневые объекты Player и интерфейса.
3. Найденные объекты временно переводятся в `DontDestroyOnLoad`.
4. `PrepareForLoad` выключает EventSystem и `PlayerController`, чтобы не было двух систем ввода и движения во время загрузки.
5. `SceneManager.LoadSceneAsync(..., Single)` загружает новую сцену.
6. В `sceneLoaded` менеджер удаляет из новой сцены копии Player, HUD, Canvas, EventSystem, `BattleTransition` и Global Light.
7. После загрузки перенесённые объекты возвращаются из `DontDestroyOnLoad` в активную сцену через `SceneManager.MoveGameObjectToScene`.
8. При необходимости игрок ставится в `spawnPosition`.
9. Камера новой сцены получает `CameraFollow` и цель — перенесённого Player.
10. EventSystem и `PlayerController` включаются обратно.

HP, MP, инвентарь и экипировка сохраняются, потому что это те же самые компоненты и объекты, а не новые копии. `Health.Start` и `BattleUnit.Awake` повторно не вызываются.

## 3. Методы и функции

- `ResetStatics()` — `[RuntimeInitializeOnLoadMethod]` сбрасывает `Instance` перед запуском Play Mode, чтобы статическая ссылка не осталась от предыдущего запуска.
- `Awake()` — если уже есть другой экземпляр, уничтожает лишний объект; иначе сохраняет `Instance`.
- `OnEnable()` / `OnDisable()` — подписываются на `SceneManager.sceneLoaded` и отписываются.
- `GetOrCreate()` — возвращает существующий менеджер или создаёт новый GameObject `SceneTransitionManager` в `DontDestroyOnLoad`.
- `ChangeScene(string sceneName, bool useSpawnPosition, Vector2 spawnPosition)` — публичная точка входа. Проверяет имя сцены, Build Settings, повторный запуск; запускает корутину и возвращает `true`, если переход начался.
- `LoadScene(...)` — корутина всего перехода: сбор объектов, загрузка, перенос, камера, восстановление управления.
- `CollectCarriedRoots()` — очищает список и собирает корневые объекты текущей сцены, которые являются Player или его интерфейсом.
- `IsGameplayRoot(GameObject root)` — проверяет наличие `PlayerController`, `ManaHUD`, `Canvas`, `EventSystem`, `BattleTransition` или `Light2D`. Такие объекты переносятся, а копии в новой сцене удаляются.
- `HandleSceneLoaded(Scene scene, LoadSceneMode mode)` — Unity-событие после загрузки сцены; при активном переходе вызывает удаление копий.
- `RemoveDuplicates(Scene scene)` — выключает и уничтожает корневые объекты новой сцены, которые совпадают по роли с переносимыми.
- `PrepareForLoad()` — запоминает и выключает `EventSystem`, выключает `PlayerController`.
- `RestoreAfterLoad()` — включает EventSystem и `PlayerController` обратно.
- `RestoreAfterFailedLoad()` — возвращает перенесённые объекты в текущую сцену, если загрузка не удалась.
- `PlacePlayer(Vector2 spawnPosition)` — ставит Transform и `Rigidbody2D` Player в точку появления и обнуляет скорость.
- `SetupCamera(Scene scene)` — находит камеру сцены, добавляет `CameraFollow`, если его нет, назначает цель и сразу ставит камеру на Player.
- `FindCamera(Scene scene)` — ищет `Camera` среди корневых объектов сцены, затем использует `Camera.main`.

## 4. Переменные и Inspector

Компонент не добавляется в сцену вручную, поэтому Inspector обычно не используется.

- `Instance` — статическая ссылка на единственный менеджер.
- `IsTransitioning` — `true`, пока идёт загрузка и перенос.
- `carriedRoots` — список переносимых корневых объектов.
- `playerRoot` — корень Player; нужен для позиции и камеры.
- `eventSystemRoot` — корень EventSystem; выключается на время загрузки.
- `eventSystemWasActive` — было ли EventSystem включено до перехода.
- `playerControllerWasEnabled` — был ли `PlayerController` включён до перехода.

## 5. Связи с другими системами

- `SceneTransition` — вызывает `ChangeScene`.
- `PlayerController`, `Health`, `ManaHUD`, `BattleUnit`, `BattleInventory`, `InventoryWindow`, `EquipmentController` — живут на переносимом Player и сохраняют состояние.
- `Health` (Canvas) — HP-полоса, MP-полоса `ManaSlider` и тексты переносятся вместе с Player.
- `InventoryCanvas` — окно инвентаря продолжает работать в новой сцене.
- `EventSystem` — нужен для кнопок и мыши в инвентаре.
- `BattleTransition` — переносится, чтобы встречи с врагами работали и после смены сцены.
- `Light2D` Global Light 2D — переносится, чтобы спрайты в пустой сцене не стали чёрными.
- `CameraFollow` — получает нового Player и сразу выравнивается.
- `SceneManager` и Build Settings — загрузка сцены по имени.

## 6. Пример работы

Игрок в `SampleScene` входит в триггер пещеры. `SceneTransition` вызывает `ChangeScene("Dungeon", true, (0,0))`. Менеджер переносит Player, HP/MP HUD, инвентарь, EventSystem, `BattleTransition` и Global Light в `Dungeon`, удаляет из `Dungeon` только его `Main Camera`-дубликаты интерфейса, ставит Player в `(0,0)` и привязывает камеру `Dungeon` к Player. HP и MP остаются такими же, какими были до входа.

## 7. Настройка в Unity

- Ничего не добавляйте в `Dungeon`: менеджер создаётся в рантайме.
- В `SampleScene` нужен объект с `SceneTransition` и коллайдером-триггером.
- В Build Settings должна быть сцена `Dungeon`.
- В `Dungeon` должна быть камера; если у неё нет `CameraFollow`, менеджер добавит компонент в рантайме.
- Player должен иметь тег `Player`, `Rigidbody2D` и `Collider2D`.
- Не добавляйте второго Player или второй HUD в `Dungeon` — менеджер удалит такие копии.

## 8. Возможные проблемы

- **Сцена не загружается.** Проверьте имя и Build Settings; ошибка пишется в Console.
- **Player не найден.** Проверьте наличие `PlayerController` на корневом объекте Player в текущей сцене.
- **В новой сцене нет камеры.** Менеджер напишет предупреждение; игрок будет существовать, но камера не будет за ним следовать.
- **Пропали объекты мира.** При `LoadSceneMode.Single` старая сцена выгружается; враги и подобранные объекты перезагружаются заново. Дискового сохранения мира нет.
- **Дубликаты HUD.** Если в целевой сцене есть собственный Canvas/Player, менеджер удалит их как копии. Это сделано намеренно, чтобы не было двух HP/MP-полос.
- **Несколько переходов подряд.** `IsTransitioning` блокирует повторный запуск; дождитесь окончания загрузки.
- **Возврат в `SampleScene` пока не настроен.** В `Dungeon` нет обратного триггера, поэтому проверить обратный перенос можно только после добавления такого объекта владельцем.
- **Нет плавного затемнения.** Переход выполняется без fade; при необходимости используйте существующий `BattleScreenFade` или отдельный overlay.

## 9. История изменений

- 2026-11-09 — менеджер создан для переноса Player и HP/MP-интерфейса из `SampleScene` в `Dungeon` без копирования объектов. Добавлены очистка дубликатов, привязка камеры и точка появления.

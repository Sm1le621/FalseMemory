# 7. Сцены и меню

Скрипты: `SceneTransition`, `SceneTransitionManager`, `PauseMenu`, `MainMenuController`.

## 7.1 Проблема

При обычной загрузке сцены (`LoadScene`) **все объекты старой сцены уничтожаются**. А игроку нужно перейти в подземелье со своим HP, маной, инвентарём и экипировкой.

Решение проекта: **не пересоздавать игрока, а перенести тот же объект** в новую сцену.

## 7.2 Триггер — `SceneTransition`

Висит на объекте-входе (`BoxCollider2D` с **Is Trigger**). В Inspector заданы имя сцены и точка появления.

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Player")) return;
    if (BattleTransition.Instance != null && BattleTransition.Instance.EncounterActive) return; // идёт бой

    SceneTransitionManager.GetOrCreate().ChangeScene(targetSceneName, useSpawnPosition, spawnPosition);
}
```

Триггер сам сцену **не грузит** — он передаёт задачу менеджеру.

## 7.3 Менеджер — `SceneTransitionManager`

Синглтон, который **создаётся кодом**, а не лежит на сцене:

```csharp
public static SceneTransitionManager GetOrCreate()
{
    if (Instance != null) return Instance;
    GameObject obj = new GameObject("SceneTransitionManager");
    DontDestroyOnLoad(obj);                                   // переживёт загрузку сцен
    Instance = obj.AddComponent<SceneTransitionManager>();    // так создают компонент кодом
    return Instance;
}
```

Корутина `LoadScene`:

1. **Собрать, что переносим**: корневые объекты, на которых есть `PlayerController`, `ManaHUD`, `Canvas`, `EventSystem`, `BattleTransition` или `Light2D`.
2. Пометить их `DontDestroyOnLoad` — Unity их не уничтожит.
3. `LoadSceneAsync(name, LoadSceneMode.Single)` и `yield return` — ждать загрузки.
4. Если в новой сцене есть такие же объекты (второй игрок, второй Canvas) — удалить дубликаты (подписка на `SceneManager.sceneLoaded`).
5. Переместить перенесённые объекты в новую сцену, поставить игрока в `spawnPosition`.
6. Найти камеру новой сцены и привязать к игроку: `CameraFollow.SetTarget(player)` + `SnapToTarget()`.

Вход в пещеру ведёт в `Dungeon`, выход `CaveExit` в `Dungeon` — обратно в `SampleScene`.

## 7.4 Схема

```mermaid
sequenceDiagram
    participant T as SceneTransition (вход)
    participant M as SceneTransitionManager
    participant U as Unity SceneManager
    participant C as CameraFollow

    T->>M: GetOrCreate().ChangeScene("Dungeon", ...)
    M->>M: собрать Player, HUD, Canvas → DontDestroyOnLoad
    M->>U: LoadSceneAsync(Single)
    U-->>M: sceneLoaded → удалить дубликаты
    M->>M: перенести объекты, поставить игрока в spawn
    M->>C: SetTarget(player), SnapToTarget()
```

## 7.5 Меню паузы — `PauseMenu`

Работает в мире (`SampleScene` и `Dungeon`). В бою пауза не открывается: там `Esc` — «назад» в меню команд.

**Объекты** (сделаны в сцене, а не кодом):

```
PauseCanvas            Canvas (sortingOrder 500 — поверх HUD и инвентаря) + PauseMenu
 └─ PausePanel         Image, чёрный с прозрачностью 60% на весь экран (выключен)
     └─ PauseWindow    Image + Outline — окно в стиле инвентаря
         ├─ Title          TMP "PAUSED"
         ├─ ResumeButton   Button → OnClick: PauseMenu.Resume
         └─ MainMenuButton Button → OnClick: PauseMenu.OpenMainMenu
```

Кнопки подключены в Inspector (раздел `OnClick()` у `Button`). Поля `PauseMenu` тоже заданы в Inspector: `panel` → `PausePanel`, `inventory` → `InventoryWindow` игрока, `inputGate` → `WorldInputGate` игрока.

**Как работает:**

| Действие | Что делает `PauseMenu` |
|---|---|
| `Esc` (инвентарь закрыт, нет боя и перехода сцены) | `Pause()`: показать `PausePanel`, `Time.timeScale = 0`, выключить `InventoryWindow`, закрыть замок `WorldInputGate` |
| `Esc` на паузе или кнопка Resume | `Resume()`: всё обратно, `Time.timeScale = 1` |
| Кнопка Main Menu | `OpenMainMenu()`: `Time.timeScale = 1`, `SceneManager.LoadScene("MainMenu")` |

`Time.timeScale = 0` останавливает физику, `FixedUpdate`, анимации и всё, что считает время через `Time.deltaTime`, — враги замирают, бой начаться не может.

Тонкость с `Esc`: им же закрывается инвентарь. Над классом стоит `[DefaultExecutionOrder(-100)]` — `PauseMenu.Update` выполняется **раньше** `InventoryWindow.Update`. Поэтому при открытом инвентаре пауза видит `inventory.IsOpen == true` и ничего не делает, а инвентарь потом закрывается сам.

`PauseCanvas` — корневой объект с `Canvas`, поэтому `SceneTransitionManager` переносит его в `Dungeon` вместе с игроком, и пауза работает и там.

## 7.6 Главное меню — `MainMenuController`

Кнопки связаны с методами **в Inspector** (раздел `OnClick()` у компонента `Button`), а не кодом:

| Кнопка | Метод | Действие |
|---|---|---|
| New Game | `MainMenuController.OnStart()` | `SceneManager.LoadScene("SampleScene")` |
| Settings | `MainMenuController.OnSettings()` | показать панель `settingsRoot` |
| Exit | `MainMenuController.OnExit()` | `Application.Quit()` (в редакторе — остановить Play Mode) |
| Слайдер громкости | `MainMenuController.OnVolumeChanged(float)` | `AudioListener.volume = value` |
| Fullscreen | `MainMenuController.ToggleFullscreen()` | `Screen.fullScreen = !...` |
| Back | `MainMenuController.HideSettings()` | спрятать панель |

Все кнопки ведут в **один** компонент `MainMenuController` на объекте `Menu`. Поля панели настроек (`settingsRoot`, `volumeSlider`, `fullscreenLabel`) заданы в Inspector.

**`PlayerPrefs`** — встроенное в Unity маленькое хранилище «ключ → значение» на диске. `MainMenuController` сохраняет туда громкость и полноэкранный режим и загружает их в `Start`. Это единственное, что игра сохраняет между запусками.

`OnExit` использует **директиву препроцессора**: `#if UNITY_EDITOR` — код только для редактора (остановить игру), `#else` — для собранной игры (закрыть приложение).

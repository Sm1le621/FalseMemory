# MainMenuController

## 1. Название и назначение

**Исходный файл:** `Assets/Scripts/UI/MainMenuController.cs`.

Обработчики кнопок главного меню. Компонент живёт на объекте `Menu` в сцене `MainMenu`; UI и все ссылки заранее сохранены в сцене.

## 2. Как работает

При загрузке `MainMenu` объект `Menu` активен, компонент уже содержит имя игровой сцены и ссылку на `SettingsMenu`. Кнопки `StartButton`/`SettingsButton`/`ExitButton` вызывают его методы через persistent `onClick` в сцене. Кода создания UI и `Awake` у компонента нет.

## 3. Методы и функции

- `OnStart()` — проверяет, что `gameSceneName` не пустое; при пустом значении пишет `Debug.LogError`, иначе `SceneManager.LoadScene(gameSceneName)` с Single-загрузкой. Возвращает void.
- `OnSettings()` — при назначенном `settingsMenu` вызывает `Show()`; без ссылки пишет `Debug.LogError`.
- `OnExit()` — в редакторе останавливает Play (`UnityEditor.EditorApplication.isPlaying = false`), в сборке вызывает `Application.Quit()`.

## 4. Переменные и параметры Inspector

- `gameSceneName` — сериализованная строка, значение сцены `SampleScene`; сцена должна быть в Build Scenes.
- `settingsMenu` — сериализованная ссылка на компонент `SettingsMenu` объекта `Menu`.

Оба поля приватные `[SerializeField]`; назначаются в сцене, а не кодом.

## 5. Связи с другими системами

`SettingsMenu.Show()` открывает панель настроек. `SceneManager` загружает `SampleScene`. `BattleTransition.ReturnToMainMenu()` возвращает игрока в эту сцену после поражения; `MainMenuController` при этом уже находится в сохранённой сцене.

## 6. Пример работы

Поражение в бою → `MainMenu` → кнопка `StartButton` (persistent `OnStart`) → свежая `SampleScene`. Кнопка `SettingsButton` открывает `SettingsDim`, `ExitButton` завершает приложение.

## 7. Настройка в Unity

Компонент находится на объекте `Menu` сцены `MainMenu`. Проверьте в Inspector: `Game Scene Name = SampleScene`, `Settings Menu` указывает на компонент `SettingsMenu` того же объекта. У кнопок в `Button.onClick` должны быть persistent-вызовы `MainMenuController.OnStart`, `OnSettings`, `OnExit`. Отдельно прикреплять компонент не нужно.

## 8. Возможные проблемы и проверки

- Пустое `Game Scene Name` даёт ошибку в Console и не грузит сцену.
- `Settings Menu` не назначен — кнопка Settings пишет ошибку.
- `SampleScene` отсутствует в Build Scenes — `LoadScene` завершится ошибкой.
- Кнопки должны иметь именно persistent-события в сцене: тест вызывает `onClick.Invoke()`.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии.
- 2026-10-11 — добавлена инструкция настройки в Unity.
- 2026-10-25 — меню перенесено в сцену: удалён `Initialize`, поля назначаются в сцене; `MainMenuBootstrap` удалён. Проверено EditMode/PlayMode-тестами и чтением YAML сцены.

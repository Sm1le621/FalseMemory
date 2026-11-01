# MainMenuController

## 1. Название и назначение

Исходник: `Assets/Scripts/UI/MainMenuController.cs`.

Кнопки главного меню: начать игру, открыть настройки, выйти. Текст интерфейса задан в сцене на английском; локализации и `GameText` нет.

## 2. Как работает

`OnStart` проверяет имя игровой сцены и загружает её через `SceneManager.LoadScene`. Пустое имя даёт ошибку в Console. `OnSettings` показывает назначенную панель `SettingsMenu`. `OnExit` в редакторе останавливает Play Mode, в сборке вызывает `Application.Quit`.

## 3. Методы и функции

- `OnStart()` — загружает `gameSceneName`; void.
- `OnSettings()` — вызывает `settingsMenu.Show()`; void.
- `OnExit()` — завершает игру/Play Mode; void.

## 4. Переменные и Inspector

- `gameSceneName` — имя сцены, по умолчанию `SampleScene`; должно быть в Build Settings.
- `settingsMenu` — ссылка на компонент панели настроек.

## 5. Связи

Компонент находится в `MainMenu` и вызывается `Button.onClick`. `SettingsMenu` управляет громкостью и полноэкранным режимом. Загрузка игры переходит в `SampleScene`.

## 6. Пример

Нажатие `New Game` загружает `SampleScene`. Нажатие `Settings` открывает панель, где можно изменить громкость и полноэкранный режим. `Exit` в редакторе останавливает игру.

## 7. Настройка в Unity

Добавь компонент на объект меню, назначь `settingsMenu` и проверь `gameSceneName`. Сцена должна быть добавлена в Build Settings. Кнопки привязываются к методам через `onClick` в Inspector.

## 8. Возможные проблемы

Если сцена не добавлена в Build Settings, `LoadScene` выдаст ошибку. Без `settingsMenu` кнопка настроек пишет ошибку. В редакторе `OnExit` не закрывает редактор, а останавливает Play Mode — это ожидаемо.

## 9. История изменений

- 2026-10-31 — удалены `LocalizedLabel`/`GameText`; тексты меню хранятся в сцене.
- 2026-10-11 — базовая документация меню.

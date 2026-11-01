# Главное меню и настройки

## 1. Назначение

Сцена `MainMenu` с английскими `New Game`/`Settings`/`Exit` и панелью громкости/полноэкранного режима. Весь UI — сохранённые объекты сцены; код не строит интерфейс.

## 2. Общая логика

`SettingsMenu.Start` скрывает панель и применяет `PlayerPrefs`. `MainMenuController` загружает `SampleScene`, открывает настройки или завершает игру. Тексты — обычные английские TMP-строки в сцене; `LocalizedLabel`/`GameText` удалены.

## 3. Основные компоненты

- `MainMenuCanvas` — Canvas (Screen Space Overlay), Scaler 640×360, Match Height, GraphicRaycaster.
- `MenuRoot` — Title, StartButton, SettingsButton, ExitButton.
- `SettingsDim` — панель с заголовком, VolumeSlider, FullscreenButton, BackButton.
- `Menu` — `MainMenuController` + `SettingsMenu`.
- `EventSystem` — EventSystem + InputSystemUIInputModule.
- Шрифт `Assets/Settings/UI/PixelFont.asset`.

## 4. Взаимодействие скриптов

`OnStart` вызывает `SceneManager.LoadScene(gameSceneName)`. `OnSettings` вызывает `SettingsMenu.Show`. `OnExit` останавливает Play Mode или вызывает `Application.Quit`. `SettingsMenu` читает/пишет `PlayerPrefs` и обновляет подпись `Fullscreen: On/Off`. `BattleTransition.ReturnToMainMenu` просто загружает эту сцену; прогресс не восстанавливается.

## 5. Последовательность событий

MainMenu → `SettingsMenu.Start` скрывает панель → New Game загружает `SampleScene`; Settings показывает панель; Back скрывает; Exit завершает приложение.

## 6. Настраиваемые параметры

- `MainMenuController.gameSceneName = SampleScene`; `settingsMenu` — компонент на `Menu`.
- `SettingsMenu.root = SettingsDim`; `volumeSlider`; `fullscreenLabel`.
- Громкость 0…1 (`FalseMemory.Volume`), fullscreen bool (`FalseMemory.Fullscreen`).
- Цвета/размеры/текст — в компонентах сцены.

## 7. Использование в Unity

Открой `Assets/Scenes/MainMenu.unity` и запусти Play. Для новой кнопки добавь объект в `MenuRoot`, назначь `Button.onClick` на метод контроллера и впиши английский текст прямо в TMP. Не добавляй второй Canvas/EventSystem.

## 8. Расширение

Новые команды — методы `MainMenuController` + событие кнопки. Новые настройки — поля `SettingsMenu` и элементы панели. Палитра/шрифт меняются в сцене. Для других языков потребуется отдельная система локализации, которой сейчас нет.

## 9. Ограничения и проверки

Настройки — локальные `PlayerPrefs`, не сохранение HP/MP. Смена громкости/fullscreen и Exit в standalone вручную не проверялись. PlayMode проверяет меню, настройки, возврат из поражения и запуск новой игры; EditMode 59/59 и PlayMode 36/36 пройдены 2026-10-31. Визуальная оценка — за владельцем.

См. также [SceneComposition](SceneComposition.md), [MainMenuController](../Scripts/UI/MainMenuController.md), [SettingsMenu](../Scripts/UI/SettingsMenu.md).

## История

- 2026-10-31 — удалены `LocalizedLabel`/`GameText`, английский текст хранится в сцене.
- 2026-10-25 — UI меню сохранён в сцене, Build-код удалён.

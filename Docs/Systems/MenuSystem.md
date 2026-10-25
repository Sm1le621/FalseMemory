# Главное меню и настройки

## 1. Назначение

Сцена `MainMenu` с английскими New Game/Settings/Exit и панелью настроек громкости/fullscreen. С 2026-10-25 весь UI и текст меню — сохранённые объекты сцены; код UI не строит.

## 2. Общая логика

При загрузке сцены `LocalizedLabel` применяет английские ключи StringTable. Кнопки вызывают `MainMenuController` через persistent-события. `SettingsMenu.Start` скрывает `SettingsDim` и применяет `PlayerPrefs`. New Game грузит `SampleScene` в Single-режиме; Settings открывает панель; Exit останавливает Play Mode или завершает приложение.

## 3. Основные компоненты

- `MainMenuCanvas` — Canvas (Screen Space Overlay), CanvasScaler (Scale With Screen Size 640×360, Match Height), GraphicRaycaster.
- `MenuRoot` — VerticalLayoutGroup с `Title`, `StartButton`, `SettingsButton`, `ExitButton`.
- `SettingsDim` — скрытая панель с `SettingsTitle`, `VolumeLabel`, `VolumeSlider`, `FullscreenButton`, `BackButton`.
- `Menu` — объект с `MainMenuController` и `SettingsMenu`.
- `EventSystem` — EventSystem + InputSystemUIInputModule.
- `MainMenuController`, `SettingsMenu`, `LocalizedLabel`, `GameText` — логика и текст.
- Шрифт `Assets/Settings/UI/PixelFont.asset`; белый спрайт `Assets/Art/UI/WhitePixel.png`.

## 4. Взаимодействие скриптов

`MainMenuController.OnStart` проверяет `gameSceneName` и вызывает `SceneManager.LoadScene`. `OnSettings` вызывает `SettingsMenu.Show`. `OnExit` завершает приложение. `SettingsMenu` читает/пишет `PlayerPrefs` и обновляет подпись через `GameText`. `LocalizedLabel` обновляет статические подписи при смене языка. `BattleTransition.ReturnToMainMenu` просто загружает эту сцену; меню не восстанавливает прогресс.

## 5. Последовательность событий

MainMenu → LocalizedLabel применяет ключи → SettingsMenu.Start скрывает панель → New Game: Single-загрузка `SampleScene`; Settings: `SettingsDim.SetActive(true)` + перечитывание настроек; Back: скрытие; Exit: остановка/Quit.

## 6. Настраиваемые параметры

- `MainMenuController.Game Scene Name` = `SampleScene`; `Settings Menu` = компонент на `Menu`.
- `SettingsMenu.Root` = `SettingsDim`; `Volume Slider`; `Fullscreen Label`.
- Громкость 0…1 (`FalseMemory.Volume`), fullscreen bool (`FalseMemory.Fullscreen`).
- Цвета/размеры — в компонентах сцены; текст — в StringTable.

## 7. Использование в Unity

Откройте `Assets/Scenes/MainMenu.unity` и запустите Play. Ничего создавать вручную не нужно: Canvas, кнопки, панель и EventSystem уже в сцене. Для новой кнопки добавьте объект в `MenuRoot`, назначьте `Button.onClick` на метод контроллера и добавьте `LocalizedLabel` с ключом. Не добавляйте второй Canvas/EventSystem.

## 8. Расширение

Новые команды — методы `MainMenuController` + persistent-событие кнопки. Новые подписи — запись в StringTable и ключ в `LocalizedLabel`. Новые настройки — поля `SettingsMenu` + элементы панели. Палитра/шрифт меняются в компонентах сцены, а не в удалённой `PixelUi`.

## 9. Ограничения и проверки

- Настройки — локальные `PlayerPrefs`, не сохранение HP/MP/EXP.
- Смена громкости/fullscreen и Exit в standalone-сборке вручную не проверялись.
- PlayMode `LocalizationFlowTests` проверяет английские подписи, открытие/закрытие Settings, Back/New Game; EditMode 82/82 и PlayMode 42/42 пройдены 2026-10-25.
- `MainMenuBootstrap` и `PixelUi` удалены; история остаётся в Git.

См. также [SceneComposition](SceneComposition.md), [MainMenuController](../Scripts/UI/MainMenuController.md), [SettingsMenu](../Scripts/UI/SettingsMenu.md), [LocalizationSystem](LocalizationSystem.md).

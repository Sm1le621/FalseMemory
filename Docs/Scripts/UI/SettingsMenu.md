# SettingsMenu

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/UI/SettingsMenu.cs.

Громкость и полноэкранность с PlayerPrefs; не сохранение игрового прогресса.

## 2. Как работает

Bind соединяет UI и применяет настройки. Show открывает/перечитывает, Hide закрывает. Slider и Fullscreen записывают настройки.

## 3. Методы и функции

2026-10-12 — актуальная локализация: RefreshFullscreenLabel читает settings.fullscreen_on/off. OnEnable подписывает этот метод на GameText.LanguageChanged и обновляет текст; OnDisable отписывает. Настройки громкости/fullscreen и ключи PlayerPrefs сохранены.

- `Bind(GameObject panelRoot,Slider slider,TextMeshProUGUI fullscreenValueLabel)` — записывает ссылки, Hide, LoadAndApply; void.
- `Show()` — root включён, LoadAndApply; `Hide()` — root выключен.
- `OnVolumeChanged(float value)` — Clamp01 → AudioListener.volume → SetFloat/Save.
- `ToggleFullscreen()` — меняет bool, ApplyFullscreen, SetInt/Save, RefreshFullscreenLabel.
- `LoadAndApply()` — читает PlayerPrefs, SetValueWithoutNotify, применяет fullscreen/подпись.
- `ApplyFullscreen()` — Screen.fullScreen=fullscreen.
- `RefreshFullscreenLabel()` — пишет Fullscreen: On/Off из GameText. Методы без перечисленных параметров — void и без аргументов.

## 4. Переменные и параметры Inspector

root/volumeSlider/fullscreenLabel — сериализованные ссылки из Bind. fullscreen — внутренний bool. VolumePrefKey=FalseMemory.Volume, 0…1; FullscreenPrefKey=FalseMemory.Fullscreen, 0/1.

## 5. Связи с другими системами

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

MainMenuController вызывает Show, Bootstrap создаёт элементы, PixelUi оформляет. Screen/AudioListener/PlayerPrefs независимы от BattleUnit.

## 6. Пример работы

Volume=0.5 → громкость 50% → сохранена при следующем открытии.

## 7. Настройка в Unity

В штатном меню компонент создаётся на SettingsDim, ссылки назначает Bootstrap через Bind. Для ручного UI нужен root панели, Volume Slider0…1 и TMP Fullscreen Label; события на OnVolumeChanged, ToggleFullscreen и Hide. Не менять PlayerPrefs во время аудита без необходимости.

## 8. Возможные проблемы и проверки

Bind применяет настройки при закрытой панели. Fullscreen в Editor отличается от сборки. В задаче локализации проверены английские подписи, открытие/закрытие Settings и границы UI; смена громкости, fullscreen и сохранение PlayerPrefs заново не проверялись поведением.

## 9. История изменений

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

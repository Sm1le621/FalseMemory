# SettingsMenu

## 1. Название и назначение

**Исходный файл:** `Assets/Scripts/UI/SettingsMenu.cs`.

Открывает/закрывает панель настроек и хранит громкость и полноэкранность в `PlayerPrefs`; не является системой сохранения игрового прогресса.

## 2. Как работает

Компонент находится на объекте `Menu` рядом с `MainMenuController`. Ссылки на корень панели `SettingsDim`, слайдер громкости и TMP-подпись fullscreen сохранены в сцене. `Start` скрывает панель и применяет сохранённые настройки. `Show` открывает панель и перечитывает настройки; `Hide` закрывает. Слайдер и кнопка fullscreen пишут значения в `PlayerPrefs`.

## 3. Методы и функции

- `Start()` — `Hide()` + `LoadAndApply()`; вызывается Unity один раз при загрузке сцены, когда объект `Menu` активен.
- `Show()` — `root.SetActive(true)`, затем `LoadAndApply()`.
- `Hide()` — `root.SetActive(false)`.
- `OnVolumeChanged(float value)` — `AudioListener.volume = Clamp01(value)`, сохраняет `FalseMemory.Volume`.
- `ToggleFullscreen()` — инвертирует `fullscreen`, применяет `Screen.fullScreen`, сохраняет `FalseMemory.Fullscreen`, обновляет подпись.
- `LoadAndApply()` — читает `PlayerPrefs`, применяет громкость и fullscreen, выставляет слайдер без события и обновляет подпись.
- `ApplyFullscreen()` — `Screen.fullScreen = fullscreen`.
- `RefreshFullscreenLabel()` — берёт `settings.fullscreen_on`/`settings.fullscreen_off` через `GameText`.
- `OnEnable()`/`OnDisable()` — подписка/отписка на `GameText.LanguageChanged` и обновление подписи.

## 4. Переменные и параметры Inspector

- `root` — GameObject `SettingsDim`; при старте выключается.
- `volumeSlider` — `Slider` 0…1 в панели настроек.
- `fullscreenLabel` — TMP-подпись кнопки Fullscreen.
- `fullscreen` — внутреннее состояние, не редактируется вручную.
- Ключи `FalseMemory.Volume` (float 0…1) и `FalseMemory.Fullscreen` (int 0/1).

## 5. Связи с другими системами

`MainMenuController.OnSettings()` вызывает `Show()`. Кнопка `BackButton` вызывает `Hide()`, `FullscreenButton` — `ToggleFullscreen()`, слайдер — `OnVolumeChanged`. `GameText`/`LocalizedLabel` отвечают за английские подписи.

## 6. Пример работы

MainMenu → Settings → панель `SettingsDim` включается, слайдер показывает сохранённую громкость → перетаскивание меняет `AudioListener.volume` → Back выключает панель. Fullscreen переключает режим и подпись On/Off.

## 7. Настройка в Unity

Компонент размещён на объекте `Menu` (активном), а не на самой скрытой панели. В Inspector назначьте `Root = SettingsDim`, `Volume Slider = VolumeSlider`, `Fullscreen Label = Label` кнопки `FullscreenButton`. На кнопках должны быть persistent-вызовы `Hide`, `ToggleFullscreen`; на слайдере — `OnVolumeChanged`. Панель `SettingsDim` в сцене выключена.

## 8. Возможные проблемы и проверки

- Если компонент положить на выключенный `SettingsDim`, `Start` не выполнится, но `Show` продолжит работать; правильное место — активный объект `Menu`.
- Без `root` панель не откроется.
- Смена fullscreen в редакторе может отличаться от сборки.
- Громкость/fullscreen в standalone-сборке не проверялись; меню и настройки покрыты PlayMode-тестами.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный код.
- 2026-10-25 — удалён `Bind`; добавлен `Start`, ссылки назначаются в сцене. Панель и все элементы сохранены объектами `MainMenu`.

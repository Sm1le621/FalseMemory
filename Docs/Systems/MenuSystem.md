# Главное меню и настройки

## 1. Назначение

MainMenu с английскими New Game/Settings/Exit и настройками громкости/fullscreen.

## 2. Общая логика

Bootstrap во время Awake создаёт UI через PixelUi. Controller реагирует на кнопки; SettingsMenu читает/сохраняет PlayerPrefs.

## 3. Основные компоненты

MainMenuBootstrap, MainMenuController, SettingsMenu, PixelUi, EventSystem и InputSystemUIInputModule.

## 4. Взаимодействие скриптов

Bootstrap назначает Controller.Initialize, Settings.Bind и события. Start полностью загружает SampleScene; не восстанавливает RPG-прогресс.

## 5. Последовательность событий

MainMenu → создание Canvas/кнопок → New Game: свежая игра; Settings: панель/настройки; Exit: остановка Play или Quit.

## 6. Настраиваемые параметры

gameTitle, gameSceneName; громкость 0…1, fullscreen bool. PlayerPrefs ключи FalseMemory.Volume/Fullscreen. PixelUi масштаб max(1,floor(Screen.height/360)).

## 7. Использование в Unity

Запускать MainMenu. UI появляется в Play, не нужно вручную создавать вторую копию. Нужен Bootstrap, доступная SampleScene, TMP/Input System и шрифт Resources.

## 8. Расширение

Новые команды назначать контроллеру и Bootstrap. Их подписи добавлять в английскую StringTable и получать через GameText; статические TMP-подписи связывать LocalizedLabel. Изменение палитры/строительных функций — PixelUi. Систему сохранений прогресса проектировать отдельно.

## 9. Ограничения и проверки

Настройки — существующее локальное сохранение предпочтений, не HP/MP/EXP. Меню не пересчитывает масштаб при resize. Start из поражения покрыт тестом; звук/fullscreen/Exit в сборке не проверены.

Первичная документация — 2026-10-11, актуализация — 2026-10-12. Отдельные методы и поля см. в [указателе скриптов](../README.md#описания-скриптов).

2026-10-12 — меню и Settings читают GameText/LocalizedLabel; Fullscreen: On/Off берутся из таблицы. В PixelUi отключён extraPadding bitmap-шрифта, чтобы не появлялись фрагменты соседних букв. Проверены открытие Settings/Back/New Game и геометрия/символы в четырёх разрешениях. Переключение fullscreen, изменение громкости и Exit в standalone не проверялись заново. Результаты и снимки: [EnglishLocalization](../EnglishLocalization.md), устройство текста: [LocalizationSystem](LocalizationSystem.md).

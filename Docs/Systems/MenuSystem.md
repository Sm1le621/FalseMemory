# Главное меню и настройки

## 1. Назначение

MainMenu со Start/Setting/Exit и настройками громкости/fullscreen.

## 2. Общая логика

Bootstrap во время Awake создаёт UI через PixelUi. Controller реагирует на кнопки; SettingsMenu читает/сохраняет PlayerPrefs.

## 3. Основные компоненты

MainMenuBootstrap, MainMenuController, SettingsMenu, PixelUi, EventSystem и InputSystemUIInputModule.

## 4. Взаимодействие скриптов

Bootstrap назначает Controller.Initialize, Settings.Bind и события. Start полностью загружает SampleScene; не восстанавливает RPG-прогресс.

## 5. Последовательность событий

MainMenu → создание Canvas/кнопок → Start: свежая игра; Setting: панель/настройки; Exit: остановка Play или Quit.

## 6. Настраиваемые параметры

gameTitle, gameSceneName; громкость 0…1, fullscreen bool. PlayerPrefs ключи FalseMemory.Volume/Fullscreen. PixelUi масштаб max(1,floor(Screen.height/360)).

## 7. Использование в Unity

Запускать MainMenu. UI появляется в Play, не нужно вручную создавать вторую копию. Нужен Bootstrap, доступная SampleScene, TMP/Input System и шрифт Resources.

## 8. Расширение

Новые команды назначать контроллеру и Bootstrap. Изменение палитры/строительных функций — PixelUi. Систему сохранений прогресса проектировать отдельно.

## 9. Ограничения и проверки

Настройки — существующее локальное сохранение предпочтений, не HP/MP/EXP. Меню не пересчитывает масштаб при resize. Start из поражения покрыт тестом; звук/fullscreen/Exit в сборке не проверены.

Документ описывает текущую рабочую копию на 2026-10-11. Отдельные методы и поля см. в [указателе скриптов](../README.md#описания-скриптов).

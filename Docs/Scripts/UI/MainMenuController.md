# MainMenuController

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/UI/MainMenuController.cs.

Обработчики кнопок главного меню.

## 2. Как работает

Bootstrap создаёт компонент, вызывает Initialize и связывает методы с событиями кнопок; Unity callbacks нет.

## 3. Методы и функции

- `Initialize(string sceneName,SettingsMenu menu)` — записывает имя сцены и настройки; void.
- `OnStart()` — START: проверяет непустое имя, LoadScene; пустое даёт Error.
- `OnSettings()` — SETTING: settingsMenu.Show, при отсутствии ссылки Error.
- `OnExit()` — EXIT: останавливает Play в Editor, Application.Quit в сборке.

## 4. Переменные и параметры Inspector

gameSceneName — сериализованная строка, код SampleScene, должна быть в сборке. settingsMenu — ссылка, задаётся Initialize.

## 5. Связи с другими системами

Создаётся MainMenuBootstrap. Использует SettingsMenu/SceneManager. BattleTransition возвращает сюда после поражения.

## 6. Пример работы

Game Over → MainMenu → START → свежая SampleScene.

## 7. Настройка в Unity

Текущий Bootstrap добавляет компонент сам и вызывает Initialize. Для штатного меню ничего отдельно не прикрепляй. При ручном UI назначь игровую сцену и SettingsMenu, затем события Button.onClick на OnStart/OnSettings/OnExit.

## 8. Возможные проблемы и проверки

Проверяет пустое имя, не доступность сцены. Новая игра сбрасывает сессию. Start после поражения покрыт тестом; Settings/Exit в сборке не проверены.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

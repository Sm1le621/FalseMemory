# MainMenuBootstrap

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/UI/MainMenuBootstrap.cs.

Создание главного меню и настроек при запуске MainMenu.

## 2. Как работает

Awake обеспечивает EventSystem и строит Canvas/кнопки/SettingsMenu, затем связывает действия.

## 3. Методы и функции

- `Awake()` — Unity: EnsureEventSystem, BuildUi; void.
- `EnsureEventSystem()` — создаёт EventSystem+InputSystemUIInputModule при отсутствии.
- `BuildUi()` — PixelUi создаёт элементы и MainMenuController, подключает события.
- `BuildSettingsPanel(Transform canvas)` — создаёт панель и настройки, вызывает Bind, возвращает SettingsMenu.
- `CreateCenteredColumn(Transform parent,string objectName,Vector2 size)` — возвращает созданный центральный RectTransform. Остальные методы void.

## 4. Переменные и параметры Inspector

gameTitle — сериализованный заголовок FALSE MEMORY; gameSceneName — SampleScene. Размеры 360×280/380×280 и подписи прописаны в методах.

## 5. Связи с другими системами

Компонент на MenuBootstrap MainMenu; использует PixelUi/MainMenuController/SettingsMenu, TMP/uGUI/Input System.

## 6. Пример работы

MainMenu → Awake строит MainMenuCanvas → START начинает игру.

## 7. Возможные проблемы и проверки

Два Bootstrap создадут дубликаты; resize не пересчитывает масштаб PixelUi. Создание меню и Start проверены Play Mode; все разрешения не проверены.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

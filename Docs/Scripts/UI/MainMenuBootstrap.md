# MainMenuBootstrap

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/UI/MainMenuBootstrap.cs.

Создание главного меню и настроек при запуске MainMenu.

## 2. Как работает

Awake обеспечивает EventSystem и строит Canvas/кнопки/SettingsMenu, затем связывает действия.

## 3. Методы и функции

2026-10-12 — актуальная локализация: BuildUi/BuildSettingsPanel получают New Game/Settings/Exit/Volume/Back через GameText.Get; LocalizedLabel.Bind подписывает статические подписи на смену языка. Размеры/события кнопок сохранены.

- `Awake()` — Unity: EnsureEventSystem, BuildUi; void.
- `EnsureEventSystem()` — создаёт EventSystem+InputSystemUIInputModule при отсутствии.
- `BuildUi()` — PixelUi создаёт элементы и MainMenuController, подключает события.
- `BuildSettingsPanel(Transform canvas)` — создаёт панель и настройки, вызывает Bind, возвращает SettingsMenu.
- `CreateCenteredColumn(Transform parent,string objectName,Vector2 size)` — возвращает созданный центральный RectTransform. Остальные методы void.

## 4. Переменные и параметры Inspector

gameTitle — сохранённое legacy-поле FALSE MEMORY, используемое при создании TMP; после Bind отображаемый заголовок берётся из ключа game.title. Менять пользовательский заголовок нужно в таблице. gameSceneName — SampleScene. Размеры 360×280/380×280 прописаны в методах; подписи — в английской StringTable.

## 5. Связи с другими системами

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

Компонент на MenuBootstrap MainMenu; использует PixelUi/MainMenuController/SettingsMenu, TMP/uGUI/Input System.

## 6. Пример работы

MainMenu → Awake строит MainMenuCanvas → New Game начинает игру.

## 7. Настройка в Unity

На единственном MenuBootstrap в MainMenu. Настрой Game Title и Game Scene Name=SampleScene. Canvas/кнопки/контроллер и настройки создаются Awake; вручную дублировать их не требуется. Сцены включены в Build Profiles Scene List.

## 8. Возможные проблемы и проверки

Два Bootstrap создадут дубликаты; resize не пересчитывает масштаб PixelUi. Создание меню и Start проверены Play Mode; все разрешения не проверены.

## 9. История изменений

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

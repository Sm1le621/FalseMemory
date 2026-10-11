# PixelUi

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/UI/PixelUi.cs.

Статические строительные функции пиксельного UI главного меню.

## 2. Как работает

Bootstrap вызывает функции. Font/WhiteSprite создают ленивые кеши; компоненты возвращаются для назначения событий.

## 3. Методы и функции

- `Font` — свойство TMP_FontAsset: Resources-шрифт или TMP default, ApplyPointFilter.
- `WhiteSprite` — Sprite из whiteTexture.
- `CreateCanvas(string objectName)` → экранный Canvas, max(1,floor(Screen.height/360)).
- `CreateLabel(Transform parent,string objectName,string content,int fontSize,TextAlignmentOptions alignment)` → TMP-текст.
- `CreatePanel(Transform parent,string objectName,Color color)` → Image.
- `CreateButton(Transform parent,string objectName,string caption,Vector2 size)` → Button с рамкой/текстом без событий.
- `CreateSlider(Transform parent,string objectName,Vector2 size)` → Slider 0…1.
- `Stretch(RectTransform rect,float left=0,float right=0,float top=0,float bottom=0)` — растягивает с отступами, void.
- `ApplyPointFilter(TMP_FontAsset fontAsset)` — фильтр Point, void. Unity callbacks нет.

## 4. Переменные и параметры Inspector

Inspector отсутствует. FontResourcePath=Fonts/PressStart2P-Regular. Background/Panel/ButtonNormal/ButtonHover/ButtonPressed/Outline/Text/TextMuted — статическая палитра; cachedFont/whiteSprite — кеши. Размеры/fontSize положительные.

## 5. Связи с другими системами

MainMenuBootstrap, uGUI, TMP, Assets/Resources/Fonts. BattleUI имеет отдельный конструктор и TMP default.

## 6. Пример работы

CreateButton START → фон/рамка/Label → Bootstrap связывает OnStart.

## 7. Настройка в Unity

Статический класс не прикрепляется к GameObject и не имеет Inspector. MainMenuBootstrap вызывает его методы. Проверь ресурс Fonts/PressStart2P-Regular и настройки TMP; палитра/конструкторы настраиваются через код, а не компонент.

## 8. Возможные проблемы и проверки

Fallback-шрифт тоже получает Point-фильтр; динамические атласы могут менять кеш. Масштаб выбирается один раз; освобождения кеша нет. Меню создавалось тестами, его внешний вид отдельно здесь не проверялся.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

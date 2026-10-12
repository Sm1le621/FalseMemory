# AttributeRowView

## 1. Название и назначение

`Assets/Scripts/Inventory/AttributeRowView.cs`. Одна строка primary attribute в существующем Inventory, значение/gear bonus и источник общей hover-панели.

## 2. Как работает

InventoryWindow.Refresh передаёт итоговые primary и gear. Строка формирует централизованный английский текст. Hover отправляет эту строку в прежний InventoryTooltip, выход закрывает только её tooltip. Копии характеристик/ресурсов не хранит.

## 3. Методы и функции

`Key` readonly — стабильный attribute.<enum lowercase> prefix. `Render(values,gear)` void формирует abbreviation/name/value и gear suffix; positive/negative/zero разделены. `Description()` возвращает английские имя/описание и base/level/equipment/class/temporary источники, читает исходный BattleUnit/Attributes. Без CharacterAttributes возвращает пустой текст. `OnPointerEnter(PointerEventData)` Unity EventSystem показывает tooltip по position; `OnPointerExit` закрывает только этот источник. Awake/Update/Start/физических callbacks нет.

## 4. Переменные и Inspector

owner — прежний InventoryWindow; attribute — один из10 enum0…9; label — TMP этой строки. Key/read-only значения не Inspector. Текущие сценовые строки создаёт BuildAttributeLayout, два столбца по5, размер192×18, font10…13.

## 5. Связи

InventoryWindow, InventoryTooltip, CharacterAttributes/Rules/Values, BattleUnit и GameText StringTable. Родитель Image имеет raycastTarget=true, TMP=false, чтобы pointer попал в строку.

## 6. Пример

STR base10 +gear3 → `STR Strength: 13 (+3)`. Tooltip раскрывает Base10/Level0/Equipment3/Class0/Temporary0; цифра13 не записывается в базу.

## 7. Настройка в Unity

Использовать десять уже созданных Attribute* под InventoryPanel в SampleScene. owner/attribute/label назначаются через BuildAttributeLayout/MCP. Не добавлять второй CharacterAttributes или tooltip. Новые descriptions менять в английской StringTable через GameTextEditor.

## 8. Возможные проблемы

Нет owner/label или неправильный enum — неверная настройка строки. Для hover нужен EventSystem/GraphicRaycaster, окно открыто и drag выключен. Tooltip CHA/подготовленных score явно говорит об отсутствующих механиках; наличие строки не означает готовые классы/торговлю/стамину.

## 9. История изменений

- 2026-10-12 — добавлены10 primary rows и источник общей hover-панели; без собственного состояния ресурсов.

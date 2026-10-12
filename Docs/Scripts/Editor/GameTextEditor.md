# GameTextEditor

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Editor/GameTextEditor.cs. Редактор существующей английской Unity StringTable: поиск, изменение значения и добавление стабильного ключа без программирования.

## 2. Как работает

Окно читает одну таблицу из проекта, показывает ключи по алфавиту и английские значения. Изменение записывается с Undo и dirty; Save table сохраняет только таблицу/SharedData. Игровую сцену, количество предметов и характеристики не меняет.

## 3. Методы и функции

`Open()` static void MenuItem FalseMemory/Localization/English Text открывает EditorWindow. `OnGUI()` Unity Editor callback загружает Native StringTable, рисует поиск/scroll/поля. Значения фильтруются по ключу или тексту без учёта регистра. Изменение значения с кириллицей отклоняется, допустимое записывается с Undo.RecordObject. Add entry доступна при новом уникальном ключе разрешённого формата и непустом английском значении; Undo.RecordObjects включает SharedData, native AddEntry создаёт устойчивый ID. Save table вызывает SaveAssetIfDirty(table/SharedData), других ассетов не касается.

## 4. Переменные и Inspector

TablePath const Assets/Resources/Localization/GameText_en-US.asset. scroll Vector2, search/newKey/newValue string — состояние окна, не данные игры/Inspector. Ключи: строчные буквы/цифры/точки/подчёркивание/дефис. Существующие ключи и ID окно не переименовывает/не удаляет.

## 5. Связи

UnityEditor, Unity Localization StringTable/SharedTableData, GameText. Инструкция [LocalizationSystem](../../Systems/LocalizationSystem.md).

## 6. Пример

Открыть окно → поиск inventory.use → изменить английское значение → Save table. Для нового сообщения указать inventory.example и English value, Add entry, Save; затем использовать GameText.Get или LocalizedLabel.key.

## 7. Настройка в Unity

Не прикрепляется к GameObject. Скомпилирован только Editor через Assets/Editor. Открывать через верхнее меню FalseMemory → Localization → English Text; кнопка Add неактивна для дубликата/неверного ключа/пустого текста.

## 8. Возможные проблемы

Полноту английского смысла автоматическая проверка не гарантирует, кириллица лишь одна защита от смешения языков. {0}/{1} должны соответствовать аргументам кода, буквальные скобки писать {{/}}. Изменения существующих значений могут быть пустыми до сохранения: проверяйте Test Runner и итоговый UI. Окно пока только английское; другие языки требуют отдельной задачи/таблиц. Не редактировать native numeric ID вручную.

## 9. История изменений

2026-10-12 — поиск и редактирование английского текста, добавление ключей/Undo/сохранение только таблиц.

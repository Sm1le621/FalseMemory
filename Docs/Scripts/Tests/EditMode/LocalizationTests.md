# LocalizationTests

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/LocalizationTests.cs.

Четыре Edit Mode сценария английской таблицы, ключей/форматов и безопасных отказов.

## 2. Как работает

Читает настоящий Resources StringTable, не создаёт переводы/файлы и не сохраняет SO. Locale переключается только на существующий en-US.

## 3. Методы и функции

`EnglishCatalogHasUniqueKeysNonemptyEnglishValuesAndValidFormats()` void перебирает все записи: en-US,>100, уникальность/непустые значения/без кириллицы, string.Format с16 аргументами и доступ черезGameText. `DynamicResourcesAndExperienceUseEnglishTemplates()` void проверяет HPMP/Level/EXP/Fire реальными значениями. `MissingKeysBadArgumentsAndUnsupportedLanguagesAreSafe()` void проверяет неизвестный ключ/недостающие и отсутствующие аргументы/fallback/отказ uk-UA/null/успех en-US. `EveryRarityAndEquipmentSlotHasACentralizedEnglishLabel()` void проверяет все enum значений редкости и8 слотов. Unity callbacks нет.

## 4. Переменные и Inspector

Временные keys HashSet/args; реальные Native Table/SharedData. Inspector отсутствует.

## 5. Связи

NUnit, FalseMemory.Runtime и Unity.Localization в EditMode asmdef.

## 6. Пример

Отсутствие equipment.slot.gloves или неверный {4} в шаблоне вызывает падение конкретной проверки.

## 7. Настройка в Unity

Не прикрепляется. Test Runner/EditMode/FalseMemory.Battle.EditModeTests.

## 8. Возможные проблемы

Не доказывает смысл английского текста и изображение букв; отдельные Play/визуальные проверки. Уникальность общих ID сохраняется SharedData; не менять ID вручную.

## 9. История изменений

2026-10-12 — английские таблицы/динамические значения/состояние языка/ошибочные аргументы.

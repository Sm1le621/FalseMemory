# CharacterAttributeTests

**Исходник:** `Assets/Tests/EditMode/CharacterAttributeTests.cs`.

## 1. Название и назначение

Edit Mode проверки primary formulas, нейтрального баланса, разных ролей и безопасного восстановления базовых значений.

## 2. Как работает

Каждый Test создаёт временный rules; при необходимости GameObject с CharacterAttributes. Finally освобождает только тестовые экземпляры. Данные игровых ассетов/сцен не сохраняются.

## 3. Методы и функции

`ReferenceAttributesPreserveExistingBalanceAndGrowthMatchesLegacyRates` проверяет нулевые поправки при10, ростATK2/HP5/MP2 и Fire8. `DistinctAttributesAffectTheirIntendedFormulasAndManaCostsStaySafe` проверяетSTR/DEX/AGI/MCR отдельно, MP7/4/0/−1. `PrimaryRestoreAndModifiersNeverAccumulateDerivedGrowth` проверяетsigned modifiers, JSON restore базы, рост без накопления и saturation. Методы void/[Test], Unity callbacks отсутствуют.

## 4. Переменные и Inspector

Локальные rules/go/attributes/values/equipment; level4, equipmentSTR3, temporarySTR−2; float tolerance.0001. Inspector настроек нет.

## 5. Связи

CharacterAttributeValues/Rules/Attributes, NUnit и JsonUtility. Это тесты данных; игровой ресурс/переходы/скорость проверяются отдельно в следующей интеграции.

## 6. Пример

Json restore базыSTR10 сохраняет результат14 на уровне4; не записывает14 как новую базу.

## 7. Настройка в Unity

К GameObject не прикрепляется. Window → General → Test Runner → EditMode → FalseMemory.Battle.EditModeTests → CharacterAttributeTests.

## 8. Возможные проблемы

Не доказывает, что primary attributes уже подключены к Player/бою/UI. JSON roundtrip структуры не является работающим сохранением/загрузкой игры. Для интеграции нужны отдельные сценарии ресурсов/действий.

## 9. История изменений

- 2026-10-12 — добавлены десять primary attributes, настраиваемые формулы и проверки данных; игровая интеграция — отдельный следующий подэтап.

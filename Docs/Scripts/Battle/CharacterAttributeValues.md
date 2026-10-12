# CharacterAttributeValues

**Исходник:** `Assets/Scripts/Battle/CharacterAttributeValues.cs`.

## 1. Название и назначение

Serializable набор десяти primary attributes; стабильные enum CharacterAttribute и DerivedAttributeStat для формул. Это данные, не второй владелец HP/MP/Level.

## 2. Как работает

Хранит десять int; Get/Set обращаются по enum, сложение/Scale используют long и ограничивают int, NonNegative возвращает копию без отрицательных итогов. Для временных дебаффов отдельные значения могут быть отрицательными.

## 3. Методы и функции

Актуально Player rework4: NonNegative/operator+/Scale обходят0…Count−1 без создания массивов Enum.GetValues, чтобы частые производные getters не выделяли эти массивы.

`Uniform(int)` создаёт все десять одинаковыми; `Get(CharacterAttribute)` читает int, `Set(CharacterAttribute,int)` меняет поле; неверный enum вызывает ArgumentOutOfRangeException. `NonNegative()` возвращает копию≥0. `Clamp(long)` ограничивает int.MinValue…MaxValue. `operator +` суммирует соответствующие поля с насыщением; `Scale(int)` умножает на неотрицательное количество. Unity callbacks отсутствуют.

## 4. Переменные и Inspector

Count=10 — const для stable enum и UI, не Inspector. Struct добавлен в ItemStatBonus.attributes, не переименовывая прежние serialized-поля.

strength/agility/dexterity/endurance/vitality/intelligence/wisdom/charisma/magicPower/magicControl — int, по умолчанию struct0. Uniform10 для базы; Uniform1 для growth. Signed bonuses допускают отрицательные значения. Enum значения0…9 сохраняют порядок для будущих файлов сохранения; производные Attack/Defense/MagicAttack/MagicDefense/Speed/MaximumHealth/MaximumMana также стабильны.

## 5. Связи

CharacterAttributes хранит базу/class/temporary; CharacterAttributeRules читает значения; будущая экипировка добавляет отдельный набор. Не содержит Unity Object ссылок, пригоден JsonUtility.

## 6. Пример

БазаSTR10 +growth3 +предмет3 −temporary2 =STR14; baseValues остаётся10.

## 7. Настройка в Unity

Не прикрепляется к GameObject, не ScriptableObject. Редактируется как вложенная Serializable структура в CharacterAttributes/профилях; технические имена не переводить.

## 8. Возможные проблемы

Не является дисковым SaveSystem. Сложение насыщается на каждом шаге; экстремальные signed значения у границ int могут терять точность, обычные RPG значения не затрагиваются. NonNegative применять к итогу, а не к дебаффу.

## 9. История изменений

- 2026-10-12 — Player rework4: primary attributes подключены к исходным ресурсам/бою/UI; прежние идентификаторы и игровая сессия сохранены.

- 2026-10-12 — добавлены десять primary attributes, настраиваемые формулы и проверки данных; игровая интеграция — отдельный следующий подэтап.

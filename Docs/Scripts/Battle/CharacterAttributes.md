# CharacterAttributes

**Исходник:** `Assets/Scripts/Battle/CharacterAttributes.cs`.

## 1. Название и назначение

Один компонент первичных значений, разделяет base/class/temporary и вычисляемый growth. Level/EXP/MP по-прежнему BattleUnit, HP — Health. Не дублирует ресурсы.

## 2. Как работает

Calculate получает текущий level и бонусы экипировки, складывает независимые источники и ограничивает итог≥0. Growth вычисляется каждый раз из уровня; не записывается в базу. Методы изменения отправляют Changed. OnValidate ставит флаг; Update отправляет событие на основном потоке вместо игровых действий из callback импортера.

## 3. Методы и функции

`BaseValues` возвращает nonnegative копию; `ClassBonuses`/`TemporaryBonuses` readonly getters. `Growth(int level)` — growthPerLevel×(max(1,level)−1),0 без rules. `Calculate(level,equipment)` — итог всех источников. `SetBase(attribute,value)` ограничивает≥0 и уведомляет только при изменении. `SetClassBonuses`/`SetTemporaryBonuses` заменяют отдельный набор и уведомляют. `RestoreBaseValues(values)` восстанавливает только базовые Serializable данные, не добавляет growth; событие Changed. `OnValidate()` Unity при изменениях Inspector ставит inspectorDirty; `Update()` Unity каждый кадр проверяет только dirty-флаг и уведомляет при необходимости. Не пересчитывает здоровье из OnValidate; Awake/Start/столкновений нет.

## 4. Переменные и Inspector

rules — ссылка на CharacterAttributeRules; baseValues — private SerializeField, исходные все10; classBonuses/temporaryBonuses — private SerializeField, исходные0. Можно редактировать Inspector, отрицательные modifiers допустимы. inspectorDirty внутренний флаг, Changed событие, readonly getters не Inspector. DisallowMultipleComponent исключает второй экземпляр.

## 5. Связи

CharacterAttributeRules/Values. Не требует BattleUnit для изолированных тестов данных. При интеграции размещается на исходном Player; BattleUnit подписывается и пересчитывает производные/ресурсы. ClassBonuses — подготовленный источник, готовой системы выбора класса нет.

## 6. Пример

БазаSTR10 на уровне4 с equipmentSTR3/temporarySTR−2 даёт14. Capture JsonUtility.ToJson(BaseValues) и RestoreBaseValues восстанавливают10, новый Calculate снова14 без накопления.

## 7. Настройка в Unity

На существующий Player, назначить DefaultCharacterAttributes.rules через MCP в подэтапе интеграции. На момент data-подэтапа не подключён к сцене намеренно. Использовать mutators для немедленного Changed, Inspector изменения обрабатываются очередным Update.

## 8. Возможные проблемы

Не реализует распределение очков/классы/таймеры баффов/дисковые сохранения. При изменении нескольких источников по очереди события приходят отдельно. Restore восстанавливает primary base, а не HP/MP/Level или экипировку; будущий SaveSystem обязан восстановить их отдельно.

## 9. История изменений

- 2026-10-12 — добавлены десять primary attributes, настраиваемые формулы и проверки данных; игровая интеграция — отдельный следующий подэтап.

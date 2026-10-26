# ArmorScalingData

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/ArmorScalingData.cs`. Serializable-данные защиты одного ItemData.

## 2. Как работает

Serializable-данные защиты одного ItemData. Расширяет прежние Equipment/BattleUnit/Health; отдельного владельцаHP нет.

## 3. Методы и функции

MeetsRequirements(values) сравнивает все10 атрибутов с неотрицательным минимумом, возвращает bool. Bonuses(values,rules) возвращает новый ItemStatBonus: отдельный Scale для physical/magic/evasion, без изменения исходника. Evasion считается с точностью1/100000 и ограничивается0…1; NaN/Infinity отклоняются. Unity callbacks нет.

## 4. Переменные и Inspector

physicalDefense/magicDefense — неотрицательные базовые рейтинги; evasion — доля0…1. requirements —10 минимальных статов. Три массива Scaling содержат attribute/grade, допускают пустые значения. Inspector находится внутри ItemData. Default-поля0, у6 тестовых предметов назначены конкретные рейтинги.

## 5. Связи

ArmorScalingData → WeaponScalingData.Scale/CharacterAttributeRules; EquipmentState/BattleUnit → BattleDamage/Health; Editor использует AssetDatabase/Localization/PrefabUtility; tests используют NUnit/UnityTest и существующие fixtures.

## 6. Пример

END30/STR10 усиливают HeavyPlate; при снятии никакой бонус не остаётся.

## 7. Настройка в Unity

Не прикреплять к GameObject. На ItemData категории Armor включить useArmorScaling, выбрать Chest/иной допустимый слот, настроить базы/grades/requirements. Назначение в предмет не создаёт дубликат Equipment.

## 8. Возможные проблемы

Сохранять явный opt-in useArmorScaling, категории задавать данными, не именем. ЧистыйHoly не наносит урон Human/Beast. Новые6 armor — тестовый контент с прежними иконками. Дискового save пока нет. Проверки и ограничения — [ClasslessStage54](../../ClasslessStage54.md).

## 9. История изменений

- 2026-10-26 — реализация и первичная документация подэтапа5.4.

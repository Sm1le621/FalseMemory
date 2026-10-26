# ArmorHolyTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/ArmorHolyTests.cs`. Чистые контракты защиты/брони/Holy.

## 2. Как работает

Чистые контракты защиты/брони/Holy. Расширяет прежние Equipment/BattleUnit/Health; отдельного владельцаHP нет.

## 3. Методы и функции

Setup/Cleanup создают/освобождают временные rules. Параметризованные проверки WIS10…99, Human/Beast/Undead/Unholy, сопротивления/чистогоHoly, K20/минимума/Defend, armoraggregation/unequip/repeat подтверждают расчёты. Каждый Test/TestCase возвращаетvoid и использует Assert; сцена не меняется. Детальные имена методов находятся в исходнике.

## 4. Переменные и Inspector

Временные правила/вещи/EquipmentState —локальные тестовые данные, не Inspector. Не меняют проектные ассеты.

## 5. Связи

ArmorScalingData → WeaponScalingData.Scale/CharacterAttributeRules; EquipmentState/BattleUnit → BattleDamage/Health; Editor использует AssetDatabase/Localization/PrefabUtility; tests используют NUnit/UnityTest и существующие fixtures.

## 6. Пример

Defense20 снижает100до50, Defense60до25; повторный ScaledBonuses даёт тот же результат.

## 7. Настройка в Unity

Не прикреплять. Test Runner EditMode → FalseMemory.Battle.EditModeTests. Математика не доказывает финальный баланс.

## 8. Возможные проблемы

Сохранять явный opt-in useArmorScaling, категории задавать данными, не именем. ЧистыйHoly не наносит урон Human/Beast. Новые6 armor — тестовый контент с прежними иконками. Дискового save пока нет. Проверки и ограничения — [ClasslessStage54](../../../ClasslessStage54.md).

## 9. История изменений

- 2026-10-26 — реализация и первичная документация подэтапа5.4.

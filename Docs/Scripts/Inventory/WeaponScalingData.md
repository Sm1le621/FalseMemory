# WeaponScalingData

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/WeaponScalingData.cs`. Данные Weapon Scaling в существующем ItemData. ScalingGrade None/E/D/C/B/A/S, WeaponDamageType Physical/Magic и AttributeScalingEntry сохраняют английские идентификаторы.

## 2. Как работает

Данные Weapon Scaling в существующем ItemData. ScalingGrade None/E/D/C/B/A/S, WeaponDamageType Physical/Magic и AttributeScalingEntry сохраняют английские идентификаторы. Использует прежние10 характеристик и одного владельца ресурсов/экипировки; отдельная система классов/сохранений не создаётся.

## 3. Методы и функции

MeetsRequirements(values) проверяет все10 минимальных требований и возвращаетbool. Damage(values,rules) суммирует нормированные ScalingScore×GradeCoefficient, каждую характеристику учитывает не более одного раза; ограничивает общий бонус, умножает baseDamage, один раз округляет AwayFromZero/ограничивает int. Нулевые/неизвестные grades не усиливают урон, без rules остаётся база; Unity callbacks отсутствуют.

## 4. Переменные и Inspector

weaponType — ключ типа оружия, damageType — канал, baseDamage≥0, requirements —10 минимальных primary, scaling — список stat/grade. Сам флаг включения находится в ItemData.useWeaponScaling; поле weapon само по себе его не включает. Конфигурация рангов/soft caps принадлежит CharacterAttributeRules.

## 5. Связи

CharacterAttributeRules/CharacterAttributes/BattleUnit/ItemData/EquipmentState, GameText и существующая InventoryWindow. Для tests используются NUnit/Unity Test Framework; для Editor AssetDatabase/EditorSceneManager. Назначение конкретных связей раскрыто выше.

## 6. Пример

Greatsword40, STR25/DEX10, S/E: normalized scores .45/.2, сумма .47, итог59. Это сила Attack до защиты цели, а не гарантированное снятие59HP.

## 7. Настройка в Unity

Заполнять на ItemData, не прикреплять к сцене. Required values проверять без primary-бонусов кандидата/заменяемого предмета. UI и бой должны читать те же методы. Holy и Armor Scaling относятся к5.4, здесь отсутствуют.

## 8. Возможные проблемы

Заполнять на ItemData, не прикреплять к сцене. Required values проверять без primary-бонусов кандидата/заменяемого предмета. UI и бой должны читать те же методы. Holy и Armor Scaling относятся к5.4, здесь отсутствуют. Результаты запусков/ограничения — [ClasslessStage53](../../ClasslessStage53.md). Дискового RPG-save нет, данные сохраняются в текущей игровой сессии.

## 9. История изменений

- 2026-10-26 — создана реализация подэтапа5.3; первичная документация текущего кода. Отдельный акт художественной/балансной приёмки не подразумевается.

# BattleSpell

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleSpell.cs.

ScriptableObject определения заклинания и перечисления его эффекта/цели.

## 2. Как работает

Не содержит выполнения: меню читает подпись/стоимость, BattleAbilities проверяет и применяет данные.

## 3. Методы и функции

Методов/Unity callbacks нет. CreateAssetMenu FalseMemory/Battle Spell. BattleEffect: MagicDamage, RestoreHealth; BattleTarget: Opponent, Self.

## 4. Переменные и параметры Inspector

displayName — строка, код Fire. mpCost≥0, код 8. effect — код MagicDamage; target — код Opponent. power≥0, код 10. effectColor — Color, код оранжевый. Fire.asset: эти значения; Heal.asset: цена 6, RestoreHealth, Self, power25, зелёный.

## 5. Связи с другими системами

Assets/Settings/Battle/Spells/Fire.asset и Heal.asset назначены Player.BattleUnit.spells. BattleManager показывает, BattleAbilities применяет, UI отображает effectColor.

## 6. Пример работы

Копия Heal с power50 и mpCost12 → более сильное лечение при добавлении в Spells без правки BattleManager.

## 7. Возможные проблемы и проверки

Новый тип эффекта требует расширения BattleAbilities; несколько союзников пока не поддержаны. Неверное сочетание цели/эффекта не является отдельной проверкой дизайна. Fire/Heal проверены тестами; произвольные новые заклинания не реализованы.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

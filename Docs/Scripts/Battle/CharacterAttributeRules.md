# CharacterAttributeRules

**Исходник:** `Assets/Scripts/Battle/CharacterAttributeRules.cs`.

## 1. Название и назначение

ScriptableObject коэффициентов primary→derived; сохраняет исходный баланс при referenceValue10. Профиль Assets/Settings/Progression/DefaultCharacterAttributes.asset.

## 2. Как работает

Актуально Player rework4: профиль подключён к исходному Player. Его derived growth заменяет legacy CharacterGrowthRules increments для этого Player; EXP-кривая остаётся старой. Формулы применяются к реальным ресурсам/действиям.

Формулы используют отклонение значения от referenceValue. При всех10 итоговая поправка0; Uniform1 growth за уровень даётATK2/DEF1/MATK1/MDEF1/SPD1/HP5/MP2, как предыдущий рост. Эти поправки впоследствии заменяют legacy growth для участников с primary profile, не добавляются второй раз.

## 3. Методы и функции

`Delta(values,attribute)` возвращает double отклонения; `Coefficient(float)` отвергает NaN/Infinity/отрицательные коэффициенты. `Adjustment(values,stat)` возвращает округлённый signed int для семи derived; неизвестный enum исключение. `MovementAdjustment`, `AccuracyBonus`, `CriticalBonus` возвращают float поправки. `Evasion` ограничивает шанс0…maximumEvasion. `ManaCost(baseCost,values)` возвращает−1 для неверной стоимости,0 для бесплатного, ceil сниженного расхода минимум1; reduction ограничен maximumManaReduction. `HealingBonus` возвращает signed MAG-поправку. `PreparedScore` возвращает nonnegative double для будущих capacity/stamina/resistance/knowledge/perception/persuasion/spell precision. Unity callbacks отсутствуют.

## 4. Переменные и Inspector

maximumMovementSpeed — новый Min0 Inspector параметр, код/ассет12; PlayerController ограничивает скорость0…этого значения. Другие coefficients/reference/growth сохранены. Неподключённые future scores остаются подготовленными.

Код/новый ассет: referenceValue10, growthPerLevel все1. strengthAttack/dexterityAttack/enduranceDefense/magicPowerAttack/wisdomDefense/agilitySpeed/intelligenceMana/wisdomMana/magicPowerHealing=1; vitalityHealth5; agilityMovement.03; dexterityAccuracy.005; dexterityCritical.002; agilityCritical.001; agilityEvasion.005; magicControlEfficiency.02; maxManaReduction.5/maxCritical.95/maxEvasion.5. Prepared коэффициенты strengthCapacity2/enduranceStamina5/enduranceRecovery.1/остальные1. END recovery поле подготовлено, пока не используется действующей механикой.

## 5. Связи

BattleUnit.Derived/PrimaryValues/SpellCost/SpellHealing/Evasion и PlayerController теперь читают этот профиль.

CharacterAttributeValues и CharacterAttributes; интеграция BattleUnit/боем выполняется отдельным следующим подэтапом. Существующий CharacterGrowthRules остаётся владельцем EXP-кривой; второй таблицы опыта нет.

## 6. Пример

STR13/DEX12 даютATK-поправку5; AGI14 даётMove+.12/Evasion.02; MCR20 снижает Fire8MP до7. На этом data-подэтапе это проверка формулы, не применённое игровое действие.

## 7. Настройка в Unity

Create → FalseMemory → Character Attribute Rules. Созданный через MCP профиль назначается CharacterAttributes.rules при последующей интеграции. Не прикреплять SO как компонент. Коэффициенты менять в ассете; это общий баланс всех его пользователей.

## 8. Возможные проблемы

Числа primary/class/temporary меняют derived; capacity/stamina/perception/persuasion/disease/spell-precision системы по-прежнему отсутствуют.

Нельзя складывать этот рост с прежним derived growth: будет двойной бонус. PreparedScore не означает наличие усталости, торговых/диалоговых проверок или болезней. MCRPrecision пока только интерфейс данных, магических промахов ещё нет. Коэффициенты/rounding описаны здесь; standalone не проверен.

## 9. История изменений

- 2026-10-12 — Player rework4: primary attributes подключены к исходным ресурсам/бою/UI; прежние идентификаторы и игровая сессия сохранены.

- 2026-10-12 — добавлены десять primary attributes, настраиваемые формулы и проверки данных; игровая интеграция — отдельный следующий подэтап.

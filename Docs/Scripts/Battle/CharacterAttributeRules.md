# CharacterAttributeRules

## Изменение5.4 —2026-10-26

holyBonusCurve — отдельная настраиваемая WIS-кривая: до30 .005/point,30…60 .0025,после60 .001; HolyBonus вычитает значение reference10 и ограничивает минимумом0. WIS10/20/30/40/50/60/80/99 дают0/5/10/12.5/15/17.5/19.5/21.4%. WIS не даёт MP. Это коэффициент дополнительного канала только против отмеченных целей. Исторические результаты ниже не являются результатами текущего запуска; см. [ClasslessStage54](../../ClasslessStage54.md).

## Текущее изменение5.3 —2026-10-26

Новый classlessProgression по умолчанию false для совместимости, в DefaultCharacterAttributes true. attributePointsPerLevel1/referenceMana40/scalingReferencePoints50/maximumWeaponScalingBonus1.5; scalingGrades None/E/D/C/B/A/S =0/.1/.2/.4/.6/.8/1. SoftCaps содержит10 AttributeSoftCapCurve. Curve возвращает профиль/безопасный default, GradeCoefficient проверяет enum/NaN, ScalingScore нормирует интеграл/первую marginal gain/50. Delta при classless использует интеграл относительно reference10. HP — VIT-кривая8/4/1, MP — только INT4/2/.5, одно AwayFromZero округление; legacy-линейные HP5/MP INT+WIS сохранены только при false. MovementAdjustment всегда0, default максимум экономииMP=.3. В назначенном профиле growthPerLevel0/agilityMovement0: автоматических primary-бонусов за уровень больше нет. Все приведённые ниже старые линейные примеры относятся к legacy-режиму.

Фактические проверки и ограничения: [ClasslessStage53](../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

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

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

- 2026-10-26 — броня/Holy/рейтинговая защита подэтапа5.4; описание изменений приведено в начале документа.

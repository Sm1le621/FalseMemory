# Primary attributes персонажа

## Назначение и текущая граница

2026-10-12, Player rework3: реализованы **данные и формулы** десяти primary attributes, без второй копии HP/MP/Level. Интеграция с Player/BattleUnit/экипировкой/UI — следующий подэтап4; на этой контрольной точке наличие формулы не выдаётся за действующее игровое влияние.

## Компоненты и источники

CharacterAttributeValues — Serializable десять int/стабильный enum. CharacterAttributes — один MonoBehaviour: база, отдельные class/temporary modifiers, вычисляемый growth, Changed. CharacterAttributeRules — настраиваемый SO, созданный через MCP DefaultCharacterAttributes в Assets/Settings/Progression. Существующий CharacterGrowthRules сохраняет EXP-кривую; Health/BattleUnit остаются владельцами ресурсов. Нет выбора класса/распределения очков/таймеров баффов/нового SaveManager.

## Роли

- STR Strength — physical Attack; capacity score подготовлен.
- AGI Agility — Movement/Speed/Evasion и небольшой Crit.
- DEX Dexterity — weapon contribution кAttack, Accuracy и Crit.
- END Endurance — physical Defense; stamina score/recovery data подготовлены, системы усталости нет.
- VIT Vitality — MaxHP; сопротивления ранениям/яду/болезням только score.
- INT Intelligence — MaxMP; knowledge score, изучения магии пока нет.
- WIS Wisdom — MaxMP/MagicDefense; perception score, ментальные статусы ещё не реализованы.
- CHA Charisma — данные persuasion score; NPC/торговли/диалогов пока нет, игрового влияния пока не заявляем.
- MAG Magic Power — MagicAttack и сила healing.
- MCR Magic Control — снижение MP-cost; precision score подготовлен, механики магического промаха нет.

## Стартовые формулы

`ΔX = итоговое X − referenceValue10`. Base/class/growth/equipment/temporary хранятся отдельно, итог primary≥0. Поправки округляются AwayFromZero и насыщаются int. К вычисленному базовому derived добавляется поправка; итог derived≥0, MaxHP≥1.

- Attack:ΔSTR+ΔDEX.
- Defense:ΔEND.
- MagicAttack:ΔMAG; MagicDefense:ΔWIS.
- Speed:ΔAGI; Movement:ΔAGI×0.03.
- MaxHP:ΔVIT×5; MaxMP:ΔINT+ΔWIS.
- Accuracy bonus:ΔDEX×0.005; Crit bonus:ΔDEX×0.002+ΔAGI×0.001, maximumCrit0.95.
- Evasion:clamp(ΔAGI×0.005,0,0.5).
- MP-cost:ceil(baseCost×(1−clamp(ΔMCR×0.02,0,0.5))), минимум1 для платного,0 для бесплатного; отрицательная стоимость отклоняется.
- Healing bonus:ΔMAG.

База всех10 даёт нулевые поправки, сохраняет исходный level1 баланс. Growth всехprimary+1/level воспроизводит прошлые derived incrementsATK2/DEF1/MATK1/MDEF1/SPD1/HP5/MP2. **Этот рост нельзя складывать с прежним legacy growth**, иначе бонусы удвоятся. Следующая интеграция заменяет источник derived growth только для участников с primary profile; остальные остаются на прежних формулах. Ростaccuracy/crit/evasion/MP-efficiency/movement/healing — новые эффекты, их последствия будут проверены отдельно.

## Последовательность и Unity

Сейчас setters/Inspector дают Changed, Calculate получает level/gear и каждый раз считает новый результат без записи бонусов в base. OnValidate ставит dirty; событие отправляется в Update на главном потоке. RestoreBaseValues восстанавливает только base; JsonUtility roundtrip проверяет данные, **не сохранение игры на диск**. В интеграции BattleUnit подписывается, обновляет maxHP/MP без лечения и сообщает UI.

Профиль создаётся Create → FalseMemory → Character Attribute Rules; coefficients/reference/growth редактируются Inspector. CharacterAttributes затем назначается существующему Player с этим профилем. Распределение очков безопаснее добавить отдельным подэтапом после классов: отдельный unspentPoints и атомарная покупка+1 кbase; сейчас его не реализуем, чтобы не дублировать будущие class/level rules.

## Проверки и ограничения

CharacterAttributeTests проверяет нейтральный баланс, роли/MP-cost, modifiers без накопления и JSON base restore. Сценовые/боевые/UI проверки должны следовать при подключении. Наличие класса modifier не означает готовые Warrior/Mage/Rogue. Stamina, carry limits, dialogue/trade/knowledge/resistance/spell precision scores пока не действуют в геймплее. [Value](../Scripts/Battle/CharacterAttributeValues.md), [Rules](../Scripts/Battle/CharacterAttributeRules.md), [Component](../Scripts/Battle/CharacterAttributes.md), [Tests](../Scripts/Tests/EditMode/CharacterAttributeTests.md).

# Прокачка FalseMemory — основа5.1

## Дополнение2026-10-12

Player rework4/5.2: исходныйPlayer получает10 primary attributes и growth всех+1/level. Derived increments совпадают с прежними5.1, legacy growth не добавляется второй раз. EXP-кривая/награда прежние. Дополнительно растутaccuracy/crit/evasion/movement/MAG-healing/MCR efficiency. Классы/распределениеочков не реализованы.

## Назначение и текущая граница

Подключены накопительный EXP, достижимый level, настраиваемая кривая, несколько level-up за награду и производный рост. Primary Strength/Vitality/Intelligence/Mind/Agility/Luck, классы и новые способности/Character C ещё относятся к5.2–5.5. Нет дискового сохранения; карта остаётся загруженной в бою, поэтому растёт исходный Player.

## Компоненты и последовательность

Transition защищает однократную победную награду и вызывает тот же BattleUnit.AddExperience. Unit насыщает cumulative EXP до int.MaxValue, вычисляет новый уровень, LevelBonuses и новую вместимость HP. Событие ProgressChanged вызывает обновление открытого Inventory, BattleUI после награды читает Level Up/Next. Повторное обращение через событие отклоняется до окончания награды; failed/zero/dead award не повышает уровень. Health остаётся единственным владельцем HP/maxHP, currentMP — в Unit, Equipment — отдельными бонусами.

## Параметры и формулы

Assets/Settings/Progression/DefaultCharacterGrowth.asset: cap99; первый переход100EXP, каждый последующий на50 дороже. Пороги cumulative: lvl1=0,2=100,3=250,4=450,5=700. TotalForLevel использует long; недостижимые пороги выше int.MaxValue не выдаются. Изменение кривой не понижает ранее полученный уровень.

Рост за уровень после первого: HP5, MP2, ATK2, DEF1, MATK1, MDEF1, SPD1. Effective = прежнее base-поле + growth(level−1) + EquipmentBonuses; базы не увеличиваются и повторный пересчёт не копит бонусы. Стартовый maxHP запоминается Unit при Awake/первой награде; Health.SetMaximumHealth получает baseHP+growth. MaxMP вычисляется из base maxMP+growth. Текущие ресурсы сохраняются, а не лечатся при Level Up.

## Использование и расширение

Create → FalseMemory → Character Growth Rules, назначить Player.BattleUnit.growthRules. На Enemy профиль не назначен; его прежние level/60HP/награда20 не менялись. Настраивать баланс до Play; при runtime-изменении профиля maxHP обновляется очередным ApplyHealthGrowth, остальные getters читают данные напрямую. Прямая запись level/EXP в Inspector не заменяет AddExperience, поэтому ручную подготовку делать ниже ближайшего порога и затем получить настоящую награду.

При добавлении классов/primary attributes расширять данные/центральный расчёт, сохраняя level/totalEXP, базовые поля/serialized ID и отдельную экипировку. Новые формулы/ограничения должны иметь сценарные тесты. Не создавать второй Health/копию EXP/UI-персонажа.

## Проверки и ограничения

[ProgressionStage5](../ProgressionStage5.md):81 Edit/42 Play, уровни/пороги/реентерабельность/награда/HPHUD/общие объекты/позиция/текст, снимки8 сочетаний. Standalone и сохранение на диск отсутствуют; художественную оценку новой графики/темпа и баланс роста принимает владелец. Один скелет даёт20EXP — без подготовки90EXP обычная первая победа ещё не повышает уровень.

[Данные роста](../Scripts/Battle/CharacterGrowthRules.md), [участник](../Scripts/Battle/BattleUnit.md), [здоровье](../Scripts/Health.md).

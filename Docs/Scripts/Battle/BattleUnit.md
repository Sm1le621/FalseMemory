# BattleUnit

## Текущее изменение5.3 —2026-10-26

EquippedWeapon возвращает только ItemData.HasWeaponScaling; UsesWeaponScaling дополнительно проверяет classless profile и требования без primary-бонусов самого оружия. BasicAttackPower выбирает physical/magic канал. Оружейный Damage заменяет базовый Attack соответствующего канала и глобальную STR/DEX/MAG-поправку, direct flat gear остаётся отдельным бонусом. MaxMP в classless использует referenceMana40 вместо serialized maxMP30; HP baseline100 прежний. RefreshDerivedStats/Awake вызывают SynchronizeLevel; изменения вместимости не лечат. Старый рост действует только в legacy-ветке. MovementAdjustment от AGI всегда0.

Фактические проверки и ограничения: [ClasslessStage53](../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

## 1. Название и назначение

Исходник Assets/Scripts/Battle/BattleUnit.cs. Один участник мира/боя: Level/EXP/текущийMP, производные характеристики и Defend. HP принадлежит прежнему Health, предметы/gear прежним Inventory/Equipment. IBattleParticipant предоставляет Speed/IsAlive очереди.

## 2. Как работает

Awake запоминает исходный maxHP, рассчитывает вместимости и заполняет MP один раз. Подписывается на Attributes.Changed/Equipment.State.Changed на весь lifetime, включая скрытый мир во время боя. При primary profile считает base+primary-derived+direct gear; legacy level bonuses не добавляет вторично. Без profile использует предыдущий growth. Изменение modifier/gear/level пересчитывает maxHP/MP и ограничивает текущие значения, не лечит. Level/EXP остаются на том же Player между сценами.

## 3. Методы и функции

- LocalizedName — GameText.Content по localizationId, английский fallback displayName; имя не меняет ID.
- Health/Inventory/Equipment/Attributes — getters компонентов того же объекта. UsesPrimaryAttributes true только при CharacterAttributes с rules. EquipmentBonuses/EquipmentAttributes — один агрегат State. LevelBonuses — legacy growth только без primary profile.
- PrimaryValues — Calculate с level, ограниченным growthRules.MaximumLevel при наличии, и gear primary. Derived(baseValue,growth,gear,stat) — signed primary поправка +неотрицательная база/legacy/direct gear через long, итог0…intMax.
- EffectiveAttack/Defense/MagicAttack/MagicDefense/Speed — соответствующие Derived getters, не запись в base-поля. Finite(float) отбрасывает NaN/Infinity.
- MovementAdjustment — AGI-поправка и gear movementSpeed. EffectiveCriticalChance — base crit+gear+primary, cap rules.maximumCritical; EffectiveMissChance — base miss−DEX accuracy, clamp01. Evasion — gear+AGI, cap rules.maximumEvasion. Без primary профиля сохраняет legacy chance, с нулевыми gear предыдущий баланс.
- SpellCost(BattleSpell) — effective MCR MP-cost; null→−1, без primary прежняя mpCost. SpellHealing(BattleSpell) — nonnegative spell.power+MAG signed bonus, saturating; null0.
- IsAlive — Health.currentHealth>0; IsDefending — внутренний флаг. MaxMP — база+primary или legacy growth+gear maximumMana; CurrentMP clamp по вместимости.
- CurrentExperience — cumulative≥0; ExperienceToNextLevel/AtMaximumLevel читают growthRules.
- AddExperience(int) — живому положительная насыщенная награда, возвращает фактически полученное. Защищает от reentry/finally; growthRules вычисляетlevel/LevelsGainedLastAward, вызывает RefreshDerivedStats и ProgressChanged. Без rules толькоEXP, безlevel-up.
- EnsureGrowthBaseline() — один раз запоминает исходный maxHP. ApplyHealthGrowth() — устанавливает вычисленный maxHP≥1 через Health.SetMaximumHealth, без лечения/воскрешения.
- RefreshDerivedStats() — baseline/maxHP, сохраняет CurrentMP с clamp, затем StatsChanged. Вызывается при level/modifier/gear и может вызываться явно после Inspector измененияlevel/rules; void.
- Awake() Unity — baseline/maxHP/full startingMP и lifetime subscriptions. OnDestroy() Unity — отписка от тех же cached sources. OnDisable не отписывает: скрытый Player продолжает использоваться BattleScene. Update/FixedUpdate/коллизий нет.
- TrySpendMP(cost) — bool, только alive/валидная цена/достаточныйCurrentMP; списывает единожды. RestoreMP(amount) — фактически восстановленное доMaxMP, только живому иpositive.
- BeginTurn/Defend/ResetBattleState — снятие/установка/снятиеDefend; не мутируютресурсы.

## 4. Переменные и Inspector

Публичные serialized: displayName/localizationId, level≥1 (код1), attack15/defense8/speed10≥0, maxMP/magicAttack/magicDefense≥0 (код0), spells, criticalChance.1/missChance.05 Range0…1, experienceReward0, battleSprite/visualAnimations/spriteColor/flipInBattle/growthRules. Player SampleScene: исходныеbattle base15/8/10, maxMP30, MAG attack12, growth DefaultCharacterGrowth, CharacterAttributes profile DefaultCharacterAttributes, battleSprite myproject1idle; Enemy безprimary, Skeleton64visuals. Scene values и defaults различаются.
Private serialized currentMP/currentExperience — состояние, не рекомендуемые seed-параметры; Awake заполняетMP. BaseMaximumHealth/awardingExperience/boundAttributes/boundEquipment — внутреннее. ProgressChanged/StatsChanged — события; остальныеreadonly getters неInspector. LevelGrowth/profile coefficients редактировать в SO, primary/class/temporary в CharacterAttributes через его API/Inspector.

## 5. Связи

Health (RequireComponent), необязательные BattleInventory/EquipmentController/CharacterAttributes, CharacterGrowthRules EXP-кривая, CharacterAttributeRules derived coefficients. Manager/Abilities читают эффективные свойства; InventoryWindow подписан на ProgressChanged/StatsChanged, прежний HUD обновляется Health. EnemySpriteAnimation/BattlePresentation читают visualAnimations, Unit не проигрывает кадры.

## 6. Пример

База всехprimary10 сохраняет Player15ATK/100HPmax/30MPmax. Level2 primary11 даёт17/105/32, текущие80HP/22MP сохраняются. GearSTR3+DEX7/directATK2 наlevel1 даёт27ATK; повторныйRefresh не увеличивает базу. MCR20:Fire7MP;MAG14:Fire23/Heal29 при нынешнихспеллах/Enemy.

## 7. Настройка в Unity

На существующих Player/Enemy SampleScene. Не добавлять второйHealth/Unit. ДляPlayer назначен CharacterAttributes.rules DefaultCharacterAttributes; growthRules остаётся DefaultCharacterGrowth. Enemy не получает primary автоматически. На Balance SO редактировать coefficients/growth, наItemData gear bonuses. Изменение level/rules во времяPlay черезполя требует RefreshDerivedStats; primary setters/gear автоматически уведомляют. UIrefs Health сохраняются.

## 8. Возможные проблемы

Не складывать legacy growth и primary growth. Initial HP baseline фиксируется Awake; смена Health.maxHealth вручную послеAwake не изменяет baseline. Снижение capacity ограничивает текущий ресурс; возвращение capacity не восстанавливает потерянное. Restore primary базы не восстанавливаетMP/HP/Level/Inventory. Нетклассов/таймеров buffs/стамины/дисковыхсохранений; CHA и preparedscores не объявляются работающими NPC проверками. Художественныйhero Walk покаfallback; standalone/статистическийбаланс не подтверждены.

## 9. История изменений

- 2026-10-12 — Player rework4: primary profile/производные/chances/MCR/MAG, capacities без лечения, StatsChanged/lifetime subscriptions и старый legacy fallback.
2026-10-12 —5.1: кривая EXP/производный рост/Level Up при прежнем владельце HP. [Проверки](../../ProgressionStage5.md).

2026-10-12 — кадры Skeleton64/Fire/Heal/Hit подключены к существующему представлению; игровые ресурсы не перенесены в анимации. [Текущие проверки](../../PixelVisuals.md).

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.
- 2026-10-12 — 4.4: readonly итоговые характеристики экипировки, без накопления/лечения. С4.5 менеджер/магия читают эти свойства; цены/формулы сохранены.

- 2026-10-11 — изменения этапа 2 сохранены в локальном Git после повторной проверки: 40 тестов Edit Mode и 17 тестов Play Mode прошли. Поведение этого файла описано выше; в задаче главного промпта его код повторно не переписывался.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

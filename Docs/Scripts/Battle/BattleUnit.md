# BattleUnit

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleUnit.cs.

Характеристики участника боя и состояние MP/EXP. HP принадлежит отдельному Health; IBattleParticipant позволяет очереди читать Speed/IsAlive.

## 2. Как работает

Awake заполняет MP единожды. Менеджер начинает ход и сбрасывает Defend, действия расходуют MP или дают опыт. Исходный объект мира используется в бою.

## 3. Методы и функции

AddExperience теперь сохраняет насыщенный cumulative EXP, вычисляет достижимый уровень, LevelsGainedLastAward, ApplyHealthGrowth и событие ProgressChanged. awardingExperience отклоняет рекурсивные награды и сбрасывается finally. EnsureGrowthBaseline запоминает стартовый maxHP; ApplyHealthGrowth передаёт Health вычисленный максимум, сохраняя текущий HP. Awake готовит baseline/максимум/полный стартовый MP. Effective*/Speed = прежняя база + LevelBonuses + EquipmentBonuses; публичные base-поля не увеличиваются.

2026-10-12 — native pixel visuals: EnemySpriteAnimation и BattlePresentation используют одни и те же данные кадров; Unit сам их не проигрывает.

2026-10-12 — актуальная локализация: localizationId — ключ unit.player/unit.skeleton, LocalizedName → GameText.Content(localizationId+".name",displayName). Это только имя; уровни/EXP/характеристики/Health не меняются.
`Equipment` получает EquipmentController того же объекта; private `EquipmentBonuses` читает его State либо нулевые. `EffectiveAttack/EffectiveDefense/EffectiveMagicAttack/EffectiveMagicDefense` → int: неотрицательная база+соответствующий бонус с насыщением int.MaxValue. `Speed` учитывает speed-бонус. Без Equipment прежние значения. Базовые поля/HP/MP не меняются.

- `Awake()` — Unity: currentMP=MaxMP; void, не повторяется при скрытии мира.
- `TrySpendMP(int cost)` — при живом участнике и доступной стоимости ≥0 списывает MP, возвращает bool успеха.
- `RestoreMP(int amount)` — живому восстанавливает до MaxMP, возвращает фактически добавленное; отрицательное/0 даёт 0.
- `AddExperience(int amount)` — живому добавляет неотрицательный опыт до int.MaxValue; возвращает добавленное, не повышает level.
- `BeginTurn()` — очередь начинает ход: снимает Defend. `Defend()` — включает защиту. `ResetBattleState()` — снимает защиту при переходах; void.
- Свойства `Health`/`Inventory` получают компоненты, `Speed`/`MaxMP`/`CurrentMP`/`CurrentExperience` ограничивают читаемые значения, `IsAlive` проверяет HP>0, `IsDefending` хранит защиту.

## 4. Переменные и параметры Inspector

growthRules — необязательный CharacterGrowthRules; на Player назначен DefaultCharacterGrowth, на Enemy null. baseMaximumHealth/awardingExperience внутренние; LevelBonuses getter. ProgressChanged событие; LevelsGainedLastAward/ExperienceToNextLevel/AtMaximumLevel readonly. CurrentExperience по-прежнему общий накопленный EXP, maxMP — база; MaxMP теперь включает рост.

visualAnimations — новый необязательный SpriteAnimationSet; в Enemy назначен Skeleton64, в Player отсутствует. battleSprite остаётся исходным/fallback Sprite; боевые характеристики/идентификаторы не менялись.

localizationId — новое serialized string-поле: spell.fire/spell.heal у заклинаний, unit.player/unit.skeleton у участников сцены. LocalizedName readonly читает таблицу, displayName остаётся английским fallback. Ни ID, ни боевые параметры это поле не меняет.

displayName — подпись; level≥1; attack/defense/speed≥0 (код 15/8/10). maxMP/currentMP/magicAttack/magicDefense≥0; currentMP сериализован, Awake перезаписывает. spells — список BattleSpell. criticalChance/missChance 0…1 (0.1/0.05). currentExperience/experienceReward≥0; battleSprite/spriteColor/flipInBattle — внешний вид. Сцена Player: MP30, MATK12, MDEF8; Enemy: ATK12 DEF5 SPD7 MDEF3 Reward20.

## 5. Связи с другими системами

Health.SetMaximumHealth обновляет тот же HUD; InventoryWindow подписывается на ProgressChanged, BattleUI показывает Level Up после награды.

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

RequireComponent Health. Inventory необязателен, но нужен для предметов. Player/Enemy в SampleScene; BattleManager, Transition, UI, Abilities и очередь читают его.

## 6. Пример работы

Fire → TrySpendMP(8): 30→22. Возврат в мир не запускает Awake вновь, остаётся 22 MP.

## 7. Настройка в Unity

На существующих Player/Enemy с Health. Настрой характеристики, battleSprite/цвет/flip, Max MP, Spells, шансы и Reward. Player использует Fire/Heal и Max MP30; Enemy Reward20. Current MP задаётся Awake из Max MP, поэтому исходную ману меняй через максимум. Количество предметов задаётся отдельным BattleInventory.

## 8. Возможные проблемы и проверки

Новый экземпляр/перезагрузка карты сбрасывает MP. Прямая запись сериализованных полей может обойти ограничения; Inspector-атрибуты не полная защита. Нет level-up. MP/EXP/защита и сохранение сессии проверены тестами.

## 9. История изменений

2026-10-12 —5.1: кривая EXP/производный рост/Level Up при прежнем владельце HP. [Проверки](../../ProgressionStage5.md).

2026-10-12 — кадры Skeleton64/Fire/Heal/Hit подключены к существующему представлению; игровые ресурсы не перенесены в анимации. [Текущие проверки](../../PixelVisuals.md).

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.
- 2026-10-12 — 4.4: readonly итоговые характеристики экипировки, без накопления/лечения. С4.5 менеджер/магия читают эти свойства; цены/формулы сохранены.

- 2026-10-11 — изменения этапа 2 сохранены в локальном Git после повторной проверки: 40 тестов Edit Mode и 17 тестов Play Mode прошли. Поведение этого файла описано выше; в задаче главного промпта его код повторно не переписывался.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

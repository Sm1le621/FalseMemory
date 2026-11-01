# BattleUnit

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleUnit.cs`.

Данные участника боя: имя, физические и магические характеристики, MP, заклинания, внешний вид и состояние защиты. HP остаётся в отдельном `Health`; `IBattleParticipant` позволяет очереди читать `Speed` и `IsAlive`. Уровней, опыта и роста характеристик нет.

## 2. Как работает

`Awake` один раз заполняет `currentMP` значением `maxMP`. В начале своего хода `BattleManager` вызывает `BeginTurn`, который снимает защиту; команда `Defend` включает её до следующего хода. Магия проверяет и списывает MP через `TrySpendMP`; зелья восстановления вызывают `RestoreMP`. `EffectiveAttack`/`EffectiveDefense` складывают базовые значения с бонусами надетой экипировки, магические характеристики пока равны базовым. При переходе в бой и обратно вызывается `ResetBattleState`, который только снимает защиту и не сбрасывает MP.

## 3. Методы и функции

- `Awake()` — Unity callback; `currentMP = maxMP`. При повторном включении объекта не вызывается.
- `TrySpendMP(int cost)` → `bool`; живой участник с достаточным MP списывает стоимость. Отрицательная стоимость отклоняется.
- `RestoreMP(int amount)` → `int`; восстанавливает MP до максимума и возвращает фактически добавленное количество.
- `BeginTurn()` — снимает `IsDefending` в начале хода.
- `Defend()` — включает `IsDefending` до следующего `BeginTurn`.
- `ResetBattleState()` — снимает защиту при входе/выходе из боя; HP/MP/инвентарь не трогает.
- `Health` → `Health`; `Inventory` → `BattleInventory`; `Equipment` → `EquipmentController`.
- `EffectiveAttack`/`EffectiveDefense` — база плюс бонусы экипировки с насыщением `int.MaxValue`.
- `EffectiveMagicAttack`/`EffectiveMagicDefense` — текущие базовые `magicAttack`/`magicDefense`.
- `Speed` → `speed`; `IsAlive` → `Health.currentHealth > 0`.
- `MaxMP` → `maxMP`; `CurrentMP` → `currentMP`, ограниченный диапазоном `0..MaxMP`.

## 4. Переменные и Inspector

- `displayName` — подпись в сообщениях и характеристиках.
- `attack`, `defense`, `speed` — базовые физические характеристики, `≥ 0`.
- `maxMP` — максимум маны; `currentMP` — сериализованное текущее значение, при старте заполняется максимумом.
- `magicAttack`, `magicDefense` — магические характеристики, `≥ 0`.
- `spells` — массив `BattleSpell`; сейчас у игрока Fire, у скелета пусто.
- `criticalChance`, `missChance` — шансы 0…1.
- `battleSprite`, `visualAnimations`, `spriteColor`, `flipInBattle` — внешний вид в бою и мире.
- В текущей сцене у игрока MP 30, MATK 12, MDEF 8; у скелета ATK 12, DEF 5, SPD 7, MDEF 3.

## 5. Связи

Требуется `Health` (через `RequireComponent`). `BattleInventory` и `EquipmentController` необязательны, но нужны для предметов и экипировки. `BattleManager`, `BattleAbilities`, `BattleCalculator`, `BattleUI`, `BattleHUD`, `EnemySpriteAnimation` и `BattlePresentation` читают данные компонента. `ManaHUD` и `InventoryWindow` показывают MP.

## 6. Пример

Fire стоит 8 MP. При старте у игрока 30 MP; после `TrySpendMP(8)` остаётся 22. Возврат на карту не запускает `Awake` заново, поэтому 22 MP сохраняются до конца сессии. Зелье восстановления MP возвращает фактически добавленное значение и не превышает максимум.

## 7. Настройка в Unity

Компонент находится на Player и Enemy рядом с `Health`. Настрой базовые характеристики, максимум MP, заклинания, шансы и внешний вид. Стартовый MP задаётся через `maxMP`: `currentMP` перезаписывается в `Awake`. Количество предметов задаётся в `BattleInventory`, экипировка — в слотах `EquipmentController`.

## 8. Возможные проблемы

Новый экземпляр объекта или перезагрузка сцены сбрасывает MP к максимуму. Прямая запись сериализованных полей обходит ограничения Inspector. Без `Health` компонент не добавится. `spells` может быть `null` в старых сценах — менеджер это проверяет. Нет сохранения между запусками игры, повышения уровня и магических бонусов от экипировки.

## 9. История изменений

- 2026-10-31 — удалены EXP, уровни, `CharacterGrowthRules` и `localizationId`; магические характеристики больше не растут от уровня; добавлено пояснение о MP и защите.
- 2026-10-12 — базовая версия с ростом и строками; запись историческая.

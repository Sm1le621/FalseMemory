# ItemData

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/ItemData.cs.

Статическое определение предмета: ID, данные, категории и бонусы. Не количество/владение.

## 2. Как работает

Player rework5: OffHand добавленвконецenum, значение8; прежниеWeapon0…Gloves7сохранены. Двуручность/можноOffHand —данныепредмета, ownershipconstraintsвEquipmentState.

Актуально Player rework4: ItemStatBonus сохраняет прежние пять derived bonus и добавляет primary attributes/HPMP capacities/float movement/crit/evasion; данные предмета не становятся владельцем текущего ресурса.

ItemData один для мира/боя. BattleConsumable наследует поля. Экипировка всегда по1 в слоте; остальные по StackLimit.

## 3. Методы и функции

Fits(slot): WeaponвсегдаMainWeapon; вOffHand толькоcanUseOffHand && !twoHanded. ArmorприequipmentSlot==OffHand такжеallowed. Accessoryпо-прежнемуобаслота. twoHanded блокируетзанятыйOffHand черезState, неклонitem.

ItemStatBonus.operator+ суммирует primary через CharacterAttributeValues.+, capacities через Sum, float modifiers обычным сложением. BattleUnit отвергает NaN/Infinity результирующих float и ограничивает gameplay chances. Остальные getters/Fits/StackLimit/CanSell не изменены.

2026-10-12 — актуальная локализация: LocalizedName/LocalizedDescription → GameText.Content(id+".name"/".description", исходное английское поле). Существующие id/GUID/поля сохранены; редкость и бонусы не меняются.

`IsEquipment` → bool по категории; `StackLimit` →1 для экипировки/нестакаемых, иначе maxStack≥1; `CanSell` → bool, QuestItem запрещён; `Fits(slot)` → bool: Weapon, Armor Head/Chest/Legs/Boots/Gloves, Accessory1/2. ItemStatBonus `operator +` и `Sum(a,b)` складывают неотрицательные бонусы с насыщением int.MaxValue. Unity callbacks нет.

## 4. Переменные и Inspector

twoHanded/canUseOffHand — new public bool/defaultfalse. ИсходныеSword/Staffнеперенастроены. СозданIronShield:Armor/OffHand8/DEF3/Common/level1/nonstack/buy100/sell50/nativegrayShield icon; английскиеid/name/descriptionцентрализованы.

Новые fields ItemStatBonus: attributes (десять signed int, default0), maximumHealth/maximumMana (Min0/default0), movementSpeed/criticalChance/evasion (float/default0). Предметные primary modifiers могут быть отрицательными. Direct capacities положительные. Исходные предметы не перенастраивались.

LocalizedName/LocalizedDescription — readonly свойства через id+.name/.description; исходные английские поля остаются fallback. В Inspector новый запас не появляется, id/GUID/параметры сохранены.

rarity — ItemRarity: Common (по умолчанию и для прежних ассетов без поля), Uncommon, Rare, Epic, Legendary, Mythic. Меняет только представление. Sword/Ether назначены Rare, Staff/LeatherArmor Uncommon, Helmet/Bone/Potion Common. EquipmentSlot.Gloves добавлен со значением7; Accessory1/2 сохраняют5/6.

id — уникальный стабильный string; displayName/description/icon; category Consumable/Weapon/Armor/Accessory/Material/QuestItem; stackable=true,maxStack99≥1; buyPrice/sellPrice≥0; requiredLevel≥1; useLocations None/Exploration/Battle/Anywhere; equipmentSlot для Armor; bonuses Attack/Defense/MagicAttack/MagicDefense/Speed. Количество не хранится.

## 5. Связи

EquipmentState.Bonuses возвращает один агрегат; BattleUnit пересчитывает derived и capacities. Не записывать бонус в базовые Player.attack/Health.maxHealth вручную.

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

InventoryState хранит ссылки/количества, BattleConsumable добавляет эффект. Значки временные, исходная графика не менялась.

## 6. Пример

В двух слотах Potion10 и5 — один SO, два runtime количества.

## 7. Настройка в Unity

Create → FalseMemory → Item; для расходуемого Battle Consumable. Назначить уникальный ID. К GameObject не прикрепляется. Подготовлены Potion, Ether, Iron Sword, Wooden Staff, Leather Armor, Iron Helmet, Bone.

## 8. Ограничения и расширение

Нетновойdual-wield атакующеймеханики: OffHand weapon readiness толькослот/bonus. Двуручнаясовместимостьготова, productiontwoHanded оружия не создано.

Enum EquipmentSlot пока8 слотов; Off-Hand/двуручное правило — следующий подэтап. Поддержка Serializable bonus не означает рабочий SaveSystem.

Пустой ID разрешён для временных тестовых объектов; проектные данные проверяются отдельно. Конфликтующий ID другой ссылки State отвергает. Нет классов/роста/SaveSystem. Бонусы/ограничения подключены к миру и бою. Редкость пока не влияет на баланс.

## 9. История изменений

- 2026-10-12 — Player rework5: девятыйOffHand, bidirectionaldrag/atomictransfers/совместимость/силуэты; sourceIDsсохранены.

- 2026-10-12 — Player rework4: primary attributes подключены к исходным ресурсам/бою/UI; прежние идентификаторы и игровая сессия сохранены.

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

2026-10-12 — переработка UI: шесть редкостей и Gloves без перенумерации прежних слотов и изменения баланса.

2026-10-12 — 4.1: общие определения и совместимость Potion.

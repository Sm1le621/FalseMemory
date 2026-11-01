# ItemData

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/ItemData.cs`.

ScriptableObject-определение предмета: идентификатор, имя, описание, иконка, категория, стаки, цены, места использования, слот экипировки и бонусы ATK/DEF. Редкости и модели персонажа нет.

## 2. Как работает

Ассет создаётся через `FalseMemory/Item`. Категория определяет поведение: `Consumable` можно использовать, `Weapon`/`Armor` — экипировать. `StackLimit` равен 1 для экипировки и нестакуемых предметов, иначе `maxStack`. `Fits(slot)` проверяет совместимость: оружие — только Weapon, броня — указанный слот (в текущей игре Chest). `ItemStatBonus` хранит только `attack`/`defense` и умеет безопасно складываться.

## 3. Методы и функции

- `ItemStatBonus.operator +` — покомпонентное сложение бонусов.
- `ItemStatBonus.Sum(int a, int b)` — неотрицательная сумма с насыщением `int.MaxValue`.
- `IsEquipment` — true для Weapon/Armor.
- `StackLimit` — 1 для экипировки/нестакуемых, иначе `maxStack`.
- `CanSell` — `sellPrice > 0`.
- `Fits(EquipmentSlot slot)` — совместимость предмета со слотом.

Перечисления: `ItemCategory { Consumable, Weapon, Armor }`, `EquipmentSlot { Weapon, Chest }`, `ItemUseLocation { None, Exploration, Battle, Anywhere }`.

## 4. Переменные и Inspector

- `id` — строковый идентификатор для проверки конфликтов (не локализационный ключ).
- `displayName`, `description`, `icon` — отображение.
- `category` — категория; по умолчанию Consumable.
- `stackable`, `maxStack` — стаки; `maxStack ≥ 1`.
- `buyPrice`, `sellPrice` — цены, `≥ 0`.
- `useLocations` — маска мест использования.
- `equipmentSlot` — слот для брони; для оружия всегда Weapon.
- `bonuses` — только `attack`/`defense`.

## 5. Связи

Наследник `BattleConsumable` добавляет лечение/восстановление MP и цель. `InventoryState` хранит ссылки на ассеты, `EquipmentState` — надетые предметы. `InventoryWindow`, `ItemPresentation`, `InventoryTooltip` и `BattleAbilities` читают поля. `BattleInventory` хранит количество.

## 6. Пример

Iron Sword: `category = Weapon`, `bonuses.attack = 5`, `stackable = false`. `StackLimit` равен 1, `Fits(Weapon)` — true, `Fits(Chest)` — false. Leather Armor: `category = Armor`, `equipmentSlot = Chest`, `bonuses.defense = 3`.

## 7. Настройка в Unity

Создай ассет через `Assets → Create → FalseMemory → Item` (или `Battle Consumable` для зелья). Заполни `id`, имя, описание, иконку и параметры. Для экипировки выбери Weapon/Armor и слот. Не добавляй поля редкости: их больше нет.

## 8. Возможные проблемы

Два разных ассета с одинаковым непустым `id` конфликтуют при добавлении в инвентарь. `maxStack` меньше 1 нежелателен. Оружие игнорирует `equipmentSlot` и всегда идёт в Weapon. Нет редкости, веса и ограничений по классу. Изменение ассета влияет на все ссылки сразу.

## 9. История изменений

- 2026-10-31 — удалены редкость, модель персонажа и лишние характеристики; остались ATK/DEF и слоты Weapon/Chest.
- 2026-10-12 — базовая модель предметов.

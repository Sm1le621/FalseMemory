# ItemPresentation

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/ItemPresentation.cs.

Статический помощник оформления: приглушённые цвета шести редкостей и текст описания предмета. Не меняет боевые параметры, цену или владение.

## 2. Как работает

Актуально Player rework4: BonusText показывает primary equipment modifiers и прямые HP/MP capacities поверх прежних bonuses, из централизованных keys.

View запрашивает фон/рамку; Tooltip запрашивает описание. Используются реальные ItemData и BattleConsumable. Строки с отсутствующими бонусами/нулевой ценой не добавляются.

## 3. Методы и функции

BonusText обход Count10: nonzero primary читает attribute.<enum>.abbr и item.bonus/character.signed_bonus; добавляет maximumHealth/maximumMana через stat.hp/stat.mp. Description автоматически включает эти строки в прежний tooltip. Редкость/цены/правила не менялись.

2026-10-12 — актуальная локализация: SlotName/TypeName/BonusText/Description и Add читают GameText по ключам слотов/категорий/редкостей/положительных эффектов/цен/уровня. Name/Description читают локализованные свойства ItemData. Палитра/защита TMP markup/правила пустых полей сохранены.

`RarityColor(rarity)` → Color: Common серый, Uncommon зелёный, Rare голубой, Epic фиолетовый, Legendary оранжевый, Mythic красный; неизвестное значение серое. `CellColor(item,highlighted)` → приглушённая смесь тёмного фона и редкости (12%, либо36% при выборе/hover); null использует нейтральный цвет.

`SlotName(slot)` → английское имя восьми слотов из GameText; `TypeName(item)` → категория, для Armor имя конкретного слота. `BonusText(item)` → непустые ATK/DEF/MATK/MDEF/SPD и HP/MP эффект расходника. `Add(parts,name,value)` добавляет положительный бонус в список. `Description(item,count)` → локализованное имя/количество>1/редкость/тип/бонусы/описание/уровень/ненулевые цены; null → пустая строка. `Escape(text)` заменяет угловые скобки в контенте, чтобы данные не меняли TMP-разметку. Unity callbacks нет.

## 4. Переменные и Inspector

Все параметры — аргументы статических методов. Палитра и насыщенность заданы кодом; состояние не хранится. Редкость по умолчанию Common определена ItemData.

## 5. Связи

CharacterAttributeValues.Count/Get, ItemStatBonus.attributes/capacities и English StringTable.

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

InventorySlotView, EquipmentSlotView, InventoryTooltip, InventoryWindow; ItemData/подкласс BattleConsumable — источник сведений.

## 6. Пример

Iron Sword Rare → голубая рамка, ATK +5, реальная цена100/50. Potion → до30HP, без пустых MATK/DEF.

## 7. Настройка в Unity

Не прикрепляется к GameObject. Изменить rarity в Inspector предметного ассета; UI читает его без копирования модели.

## 8. Ограничения и расширение

Новые float movement/critical/evasion modifiers участвуют в игре, их числовые строки в item tooltip пока не добавлены; primary/HPMP бонусы показаны. Rarity не меняет balance.

Ancient и влияние редкости на награды не добавлены. Названия шести существующих редкостей берутся из английской таблицы. TypeName/BonusText требуют ненулевой ItemData; вызывающий проверяет null. Торговля не реализована, цены информационные.

## 9. История изменений

- 2026-10-12 — Player rework4: primary attributes подключены к исходным ресурсам/бою/UI; прежние идентификаторы и игровая сессия сохранены.

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

2026-10-12 — создан для переработанного RPG-инвентаря; восемь новых Edit Mode случаев проверяют данные/представление.

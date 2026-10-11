# EquipmentState

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/EquipmentState.cs.

Сериализуемое владение семью надетыми предметами отдельно от статических определений.

## 2. Как работает

Надевание обменивает один предмет инвентаря с надетым в том же исходном слоте, поэтому полная сумка не мешает замене. Снятие сначала проверяет место; отказ оставляет обе стороны неизменными. changing запрещает повторный вход из событий.

## 3. Методы и функции

`GetSlot(slot)` → ItemData/null, неверный enum бросает исключение. `Bonuses` → новая сумма ItemStatBonus всех7, не накапливает историю. `TryEquip(inventory,index,slot,level)` bool проверяет индекс/содержимое1/Fits/уровень; выставляет надетый предмет перед Inventory notification, вызывает TryExchangeSingle, при отказе восстанавливает прежнее и не даёт Changed. `TryUnequip(inventory,slot)` bool проверяет CanAdd, снимает/добавляет предмет целиком, откатывает отказ. `Valid(slot)` private bool0..6. changing снимается finally, Changed после успеха. Unity callbacks нет.

## 4. Переменные и Inspector

slots ItemData[7] private сериализуемые, пусты сначала; changing private transient; Changed Action. Предмет в equipped больше не лежит в inventory, старый при замене возвращается ровно1.

## 5. Связи

ItemData.Fits/bonuses/requiredLevel, InventoryState.TryExchangeSingle/CanAdd/TryAdd; EquipmentController владеет моделью.

## 6. Пример

Меч ATK+5 меняется на Staff MATK+4 в полной сумке: меч становится предметом прежней ячейки, Staff надет, ATK-бонус0/MATK4. Снять Staff без места нельзя.

## 7. Настройка в Unity

Не компонент; создаётся EquipmentController.State. Для новых типов нужен Slot enum/размер/правила/UI; текущих7 достаточно.

## 8. Ограничения и расширение

SaveSystem ещё не подключён. Нельзя править private slots или общий SO вместо операции. Подписчики должны быть корректными; модель запрещает рекурсивные Equip/Unequip во время уведомления.

## 9. История изменений

2026-10-12 — 4.4: семь слотов, атомарный обмен/снятие/бонусы/защита событий.

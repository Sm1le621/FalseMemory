# Система экипировки

## 1. Назначение

Два слота экипировки — Weapon и Chest — и бонусы только к ATK/DEF. Экипировка меняет эффективные характеристики, но не базовые поля `BattleUnit` и не создаёт копию персонажа.

## 2. Общая логика

`EquipmentState` хранит надетые `ItemData` и вычисляет `Bonuses` из двух слотов. `EquipmentController` принимает команды окна, проверяет контекст (живой персонаж, не в бою) и передаёт владение между `InventoryState` и `EquipmentState`. При снятии предмет возвращается в свободный слот; при полном инвентаре снятие отклоняется. Замена в занятом слоте атомарно возвращает старый предмет в освободившийся слот.

## 3. Основные компоненты

- `EquipmentState` — два слота, бонусы, события.
- `EquipmentController` — команды Equip/Unequip и сообщения.
- `EquipmentSlotView` — отображение слота (иконка, имя, рамка).
- `ItemData`/`ItemStatBonus` — совместимость и бонусы.
- `InventoryState`/`EquipmentController` — обмен предметами.
- `BattleUnit` — `EffectiveAttack`/`EffectiveDefense`.

## 4. Взаимодействие скриптов

`InventoryWindow` поднимает `EquipRequested`/`UnequipRequested`; `EquipmentController` вызывает `EquipmentState.TryEquip/TryUnequip`. `BattleUnit` читает `State.Bonuses` при каждом обращении к эффективным характеристикам. `InventoryWindow.Refresh` обновляет две кнопки слотов и строку `ATK/DEF`.

## 5. Последовательность событий

Выбор Iron Sword → Equip → проверка `Fits(Weapon)` → предмет уходит из сумки в слот Weapon → `EffectiveAttack` растёт на 5 → окно обновляется. Повторный клик по слоту Weapon снимает меч и возвращает его в первый свободный слот.

## 6. Настраиваемые параметры

`EquipmentSlot { Weapon, Chest }`. Iron Sword: ATK +5. Leather Armor: DEF +4. Совместимость задаётся категорией и `equipmentSlot` предмета. Бонусы суммируются один раз; повторное чтение не накапливает значения.

## 7. Использование в Unity

`EquipmentController` находится на Player. В `SampleScene` две кнопки экипировки: `EquipmentWeapon` (slot Weapon) и `EquipmentChest` (slot Chest); у обеих назначены `border`, `icon`, `background`, `slotLabel`, `owner`. Массив `InventoryWindow.equipmentButtons` содержит ровно два элемента.

## 8. Расширение

Новый слот требует изменения enum, `EquipmentState.SlotCount`, UI и правил `ItemData.Fits`. Новые бонусы (например, магия) потребуют расширения `ItemStatBonus` и `BattleUnit`. Классы, уровни и требования к экипировке не поддерживаются.

## 9. Ограничения и проверки

В бою экипировка запрещена. Поддержаны только Weapon/Chest и ATK/DEF. Нет редкости, прочности и требований по уровню. EditMode проверяет совместимость, замену при полном инвентаре, откат транзакций и отсутствие накопления бонусов; PlayMode — кнопки слотов, сохранение после боя и отсутствие изменения базовых характеристик.

## История

- 2026-10-31 — упрощено до двух слотов и ATK/DEF; `rarityBorder` заменён на `border`, ссылки слотов восстановлены.
- 2026-10-12 — базовая система экипировки.

# EquipmentSlotView

## 1. Название и назначение

Assets/Scripts/Inventory/EquipmentSlotView.cs. Одна gear ячейка: item или серый silhouette, подпись/rarity/hover и обработчики drag/drop.

## 2. Как работает

Render читает переданный worn item. Для пустого показывает emptySprite полупрозрачно; для занятого только item.icon с full alpha. Hover использует прежний tooltip. DropAllowed окрашивает фон совместимости отдельно от rarityBorder. Движение вещи делегируется одному InventoryWindow.

## 3. Методы и функции

Item readonly читает текущую State того же owner. Render(worn) сохраняет только view cache, меняет sprite/alpha/active/label и RefreshColors; не владеет предметом. SetDropAllowed(bool?) задаёт green/red/neutral состояние и вызывает RefreshColors; rarity всегда по item.
RefreshColors() меняет background и отдельно rarityBorder. OnDisable() Unity очищает hover/tooltip, ownership не меняет. OnPointerEnter/Exit(EventData) меняют hover и Show/Hide только этого источника.
OnBeginDrag(e) для LMB вызывает BeginEquipmentDrag(slot,position); OnDrag перемещает общийcursor; OnEndDrag завершает. OnDrop принимает pointerDrag InventorySlotView или EquipmentSlotView только того жеowner, затем DropOnEquipment(slot). Unity Awake/Update/физических callbacks нет.

## 4. Переменные и Inspector

owner InventoryWindow, slot enum0…8, icon/background Image, rarityBorder Outline, slotLabel TMP, emptySprite Sprite — Inspector links. item/hovered private view cache; DropAllowed readonly nullable bool, не Inspector. Empty alpha.4/color.65; worn white/alpha1. Green(40,76,48), red(91,39,43); rarityBorder цвета отдельно.

## 5. Связи

Button прежней команды, InventoryWindow/EquipmentController.State, ItemPresentation.SlotName/RarityColor, InventoryTooltip. Icon/label raycast=false, background/button остаются drop target.

## 6. Пример

Пустой Weapon показывает muted Sword. При перетаскивании Iron Sword фонWeapon green/Head red, Rare border не становится зелёным. Drop надеваетSword; возвратвbag снова показываетsilhouette.

## 7. Настройка в Unity

BuildFantasyLayout назначил view на9 кнопок; MCP назначил восемь Native64 sprites (общийRing длядвухAccessory) и сохранилSampleScene. Самостоятельно не добавлять второйвладелецgear или второйcursor. emptySprite менять накачественнуюграфикуприготовности.

## 8. Возможные проблемы

Empty не имеет item-tooltip. Временные icons требуютхудожественнойприёмки; productionGloves/Legs/Boots/Accessory items покаотсутствуют, UI иrulesпроверены временнымиitems. Вещьбезitem.icon имеетзанятоесостояние, ноизображениепусто; назначитьicon. СнятиепоClick прежнее, полныйbagотказывает.

## 9. История изменений

- 2026-10-12 — Player rework5: OffHand8/atomic transfers или view silhouette/drag/compatibility; старые IDs сохранены.
2026-10-12 — создано отображение экипировки/редкости/hover для текущих кнопок.

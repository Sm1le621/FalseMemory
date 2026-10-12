# Перетаскивание и силуэты экипировки

## Назначение

Расширение прежнего Inventory/Equipment, без второго запаса или новых сцен. Сумка30, девять gear slots и один cursor Image. OffHand8 добавлен после Gloves7, прежние ID0…7 сохранены.

## Компоненты и события

InventorySlotView и EquipmentSlotView получают настоящие Pointer events. InventoryWindow фиксирует source index или gear slot и ItemData, показывает cursor и допустимость целей. Drop перепроверяет тот же источник, затем вызывает один EquipmentController/State либо прежний InventoryState. EndDrag/Close отменяют только представление, предмет до Drop остаётся в исходном состоянии.

Допустимы bag→gear, gear→точнаяbag ячейка, gear→compatiblegear, прежниеbag move/swap/merge/split. Занятый bag target обменивается только на compatible singleton equipment; material/consumable/stack отказывают. TwoHanded Weapon и occupied OffHand несовместимы в обе стороны; автоматического снятия/потери вещей нет. canUseOffHand разрешаетweapon вOffHand только еслинеtwoHanded; второй отдельной атаки не создаёт.

Перед мутацией проверяются индексы, тип, requiredLevel и руки. State.changing блокирует reentry из Changed. Gear меняется перед синхронным Inventory уведомлением, обе стороны уже согласованы; при отказе возвращается прежняя ссылка. Успешная операция даёт Changed/StatsChanged, бонусы пересчитываются от базы, без лечения или накопления. Быстрый второй Drop отвергается, captured source уже очищен.

## Визуальное представление

Две колонки54×54 вокруг исходного героя, шаг80, английские подписи Weapon/Off-Hand/Head/Chest/Gloves/Legs/Boots/Accessory1/2. Пустые slots используют восемь оригинальных native64 PNG: Sword/Shield/Helmet/Chest/Gloves/Legs/Boots/Ring (Ring общийдлядвухslots). Point/Uncompressed/noMipmaps/centerPivot/PPU100. Серые маски182 сUIalpha.4; занятому показывается только item.icon/fullalpha. После снятия маска возвращается.

EquipmentPlaceholderBuilder создал аккуратные временные маски через пиксельные графические инструменты Unity; это не финальные художественные illustrations. Герой не перерисован программно, image_gen дляHero отклонён какненативный/инаявнешность. Существующиеitemicons сохранены. IronShield использует пока ту жеgrayShield маску каквременнуюиконку, Empty отличаетсяпрозрачностью.

DropAllowed окрашивает фон зелёным/красным отдельно от rarityBorder. Cursor следует Mouse.position, отпускание вне цели сохраняет исходное состояние. Hover использует общий InventoryTooltip, скрытый при переносе; все primary/HPMP/float bonuses и требование двуручного оружия показаны централизованными English keys. Inventory is full. используется для click unequip и drag.

## Unity и параметры

SampleScene сохранена через MCP;9equipmentButtons/views/emptySprite links и30bag slots существующегоInventory. Assets/Art/UI/Equipment64 — masks, Assets/Settings/Inventory/IronShield.asset — Armor/OffHand/DEF3/Common/level1/buy100/sell50. НовыйPickup_IronShield вTestLoot `(−3.4,−3.5,0)`, worldId sample.iron_shield, quantity1, исходныйтипpickup/Collider сохранён клонированием. Бонус/цены/редкостьредактируютсявItemData, маска —EquipmentSlotView.emptySprite. Не добавлять второйEquipmentController.

## Сохранность и расширение

EquipmentState.Serializable сохраняет форму9 slots иумеет расширять старыйarray8 безсменыиндексов. Это **не дисковый SaveSystem**: нынешняясессияпереживаетadditive BattleScene, NewGameпересоздаётмир. Stage9 должен сохранятьstable item IDs/version/quantity, анеUnityinstanceIDs. JSON тестoldarray только проверяетмиграциюформы; загрузкаигрывтрёхслотах ещёне реализована.

Удобство реального drag/читаемость/временные icons принимаютсявладельцем вручную. Автоматическаямышьпроверила bag↔gear; другиеtransfers/отказы/reentry/полныйbag проверенысценариями. Нет productionAccessory/Gloves/Boots/Legs items, существующиеrules проверены temporary test items. Дляfinal art заменитьemptySprite/icon, сохранивGUIDсылки наостальнойконтент. [Отчёт](../PlayerRework.md), [EquipmentState](../Scripts/Inventory/EquipmentState.md).

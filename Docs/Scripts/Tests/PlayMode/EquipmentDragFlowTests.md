# EquipmentDragFlowTests

Исходник: `Assets/Tests/PlayMode/EquipmentDragFlowTests.cs`.

## 1. Название и назначение

Три сценария новых gear переносов в существующей SampleScene; один реально посылает событияMouse/InputSystem/UI-raycast.

## 2. Как работает

Общий BattleFlowTests fixture загружаетисходныйPlayer. РеальнаяMouse тянетIronSword изbag1→Weapon→bag12; другиесценарии проверяют отказы/полнуюсумку/повторныйdrop/Accessory/OffHand. Temporary items удаляются толькоfinally.

## 3. Методы и функции

`RealMouseEquipsFromBagAndReturnsGearToChosenBagCell` IEnumerator sendshover/press/move/release, проверяетзелёнуюсовместимость/redHead, экипировку/ATK20/ровно1item иreturnв12/силуэт/alpha/clearhighlight. `InvalidDropCancelFullBagAndDoubleDropPreserveItemCounts` проверяетGlovesrefusal/EndDragcancel/doubleDrop/fullfiller29 сPotion→30cells/messageInventoryisfull, затемосвобождениеместа. `ShieldAccessoryMovesAndEmptySilhouettesUseSameEquipmentState` переноситringмеждудвумяslot, надеваетshieldDEF+3, проверяет9emptySprite64/Point. ОбщиеSetup/TearDown описанывBattleFlowTests; своихUnitycallbacksнет.

## 4. Переменные и Inspector

ЛокальныеActor/window/Mouse/positions, productionSword, temporaryRing/Shield/Filler. finally RemoveDevice толькосинтетическуюмышь. НастроекInspectorнет.

## 5. Связи

InventoryWindow/Slots/EquipmentController/State/ItemData/PlayerController/GameText, реальныйEventSystem/GraphicRaycaster/InputSystem.

## 6. Пример

Послеmouse-drop SwordвWeapon bagcount0/gear1; послеобратногоdrop bag12count1/gear0. Серый силуэт возвращается, rarityborder не заменяется цветомсовместимости.

## 7. Настройка в Unity

Не компонент. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests → соответствующийметод; актуальнаяSampleScene9slots/8silhouetteassetsrequired.

## 8. Возможные проблемы

Первый сценарий подтверждаетMouse raycast; остальныеиспользуютпубличныеdrag API напрямую. Нетпроверкивсехмышей/всех разрешенийвнутриэтогофайла/standalone/дисковогосохранения. Художественнаяоценкасерыхвременныхмасокза владельцем.

## 9. История изменений

- 2026-10-12 — Player rework5: Off-Hand, атомарные переносы и временные native64 силуэты; дисковые сохранения ещё отсутствуют.

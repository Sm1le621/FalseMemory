# EquipmentController

## Текущее изменение5.3 —2026-10-26

Awake дополнительно привязывает State.BindRequirements: для HasWeaponScaling проверяет weapon.MeetsRequirements(Attributes.Calculate(level,gearБезПроверяемогоПредмета)). Все Can/Try drag/bag-swap используют этот контракт State. HandleEquip при отказе показывает конкретные requirements/current через RequirementsText; предмет не расходуется. Старое ItemData без явного useWeaponScaling не получает нового Scaling.

Фактические проверки и ограничения: [ClasslessStage53](../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/EquipmentController.cs.

Связывает EquipmentState с существующим Player/инвентарём/окном, без второго персонажа.

## 2. Как работает

Player rework5: прежний контроллер делегирует все переносы одной State. CanChange по-прежнему проверяет alive/enabled/отсутствие боя; highlight queries используют те же правила, что фактическая операция.

Awake получает Unit/Inventory/Window. OnEnable подключает команды и обновление, OnDisable отписывает без сброса State. CanChange допускает живого активного героя вне встречи.

## 3. Методы и функции

2026-10-13 — HandleUnequip при полном bag также использует централизованное Inventory is full., как и drag-to-bag. Успех и остальные ошибки сохраняют прежние сообщения.

CanEquip(index,slot) bool вызывает State.CanEquip с текущимlevel. CanMove(from,to) bool проверяет compatibility/level/hands. TryMove(from,to) bool атомарно перемещает или обмениваетgear slots. TryUnequipTo(from,destination) bool возвращает или обменивает вещь с конкретнойbag ячейкой. Все четыре ограниченыCanChange. ПрежниеTryEquip/TryUnequip/handlers/событияUI сохранены.

2026-10-12 — актуальная локализация: HandleEquip/HandleUnequip показывают equipment.equipped/equip_failed/unequipped/unequip_failed; транзакции/слоты/уровень не меняются.

Unity `Awake` ссылки; `OnEnable` подписывает EquipRequested/UnequipRequested/State.Changed, `OnDisable` снимает те же подписки. `CanChange` bool проверки. `TryEquip(index,target=null)` bool определяет Weapon/Armor-слот либо первый свободный Accessory1/2 (иначе1) и делегирует State.TryEquip с level. `TryUnequip(slot)` bool делегирует при CanChange. `HandleEquip`/`HandleUnequip` void — ответы окну/Refresh после результата.

## 4. Переменные и Inspector

State теперь9слотов; новыйOffHand8, прежние0…7непереименованы. Новыхпубличныхserializedполейвконтроллере нет.

State readonly создан один раз, private unit/inventory/window. Inspector дополнительных настроек нет, данные в ItemData. Target enumnullable позволяет явно выбрать слот.

## 5. Связи

InventoryWindow drag API иEquipmentSlotView/InventorySlotView; отдельногоtransfer inventory не создаётся.

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

BattleUnit читает новые EffectiveAttack/Defense/MagicAttack/MagicDefense/Speed из State, базовые поля не меняются; формула боя будет подключена4.5. InventoryWindow даёт команды/семь кнопок.

## 6. Пример

Пять надеваний/снятий меча дают20/15 ATK, а не20/25/30; HP/MP не восстанавливаются.

## 7. Настройка в Unity

Уже на Player, InventoryWindow назначен того же объекта. Require Unit/Inventory. Нет singleton; не дублировать компонент.

## 8. Ограничения и расширение

Отказнечастичный: невернаяячейка/уровень/полныйbag/двуручныйконфликт сохраняютвладение. UI сообщаетInventory is full. ДисковыйSaveSystem ещёотсутствует.

В бою/при скрытом Player/смерти смена экипировки запрещена. Нет классов/HPMP-бонусов, только пять поддержанных характеристик. Новая игра создаёт пустой State, hide/show его сохраняет.

## 9. История изменений

- 2026-10-12 — Player rework5: девятыйOffHand, bidirectionaldrag/atomictransfers/совместимость/силуэты; sourceIDsсохранены.

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

2026-10-12 — 4.4: интеграция компонентов/ограничение фазы/команды окна.

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

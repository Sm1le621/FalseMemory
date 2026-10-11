# EquipmentController

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/EquipmentController.cs.

Связывает EquipmentState с существующим Player/инвентарём/окном, без второго персонажа.

## 2. Как работает

Awake получает Unit/Inventory/Window. OnEnable подключает команды и обновление, OnDisable отписывает без сброса State. CanChange допускает живого активного героя вне встречи.

## 3. Методы и функции

Unity `Awake` ссылки; `OnEnable` подписывает EquipRequested/UnequipRequested/State.Changed, `OnDisable` снимает те же подписки. `CanChange` bool проверки. `TryEquip(index,target=null)` bool определяет Weapon/Armor-слот либо первый свободный Accessory1/2 (иначе1) и делегирует State.TryEquip с level. `TryUnequip(slot)` bool делегирует при CanChange. `HandleEquip`/`HandleUnequip` void — ответы окну/Refresh после результата.

## 4. Переменные и Inspector

State readonly создан один раз, private unit/inventory/window. Inspector дополнительных настроек нет, данные в ItemData. Target enumnullable позволяет явно выбрать слот.

## 5. Связи

BattleUnit читает новые EffectiveAttack/Defense/MagicAttack/MagicDefense/Speed из State, базовые поля не меняются; формула боя будет подключена4.5. InventoryWindow даёт команды/семь кнопок.

## 6. Пример

Пять надеваний/снятий меча дают20/15 ATK, а не20/25/30; HP/MP не восстанавливаются.

## 7. Настройка в Unity

Уже на Player, InventoryWindow назначен того же объекта. Require Unit/Inventory. Нет singleton; не дублировать компонент.

## 8. Ограничения и расширение

В бою/при скрытом Player/смерти смена экипировки запрещена. Нет классов/HPMP-бонусов, только пять поддержанных характеристик. Новая игра создаёт пустой State, hide/show его сохраняет.

## 9. История изменений

2026-10-12 — 4.4: интеграция компонентов/ограничение фазы/команды окна.

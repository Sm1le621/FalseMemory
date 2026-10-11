# InventoryWindow

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/InventoryWindow.cs.

Существующий Player управляет окном общего инвентаря6×5.

## 2. Как работает

Открытие I, закрытие I/Escape; стрелки выбирают. Событие State.Changed обновляет30 views только после изменения/выбора. UI не хранит количества; использование делегируется UseRequested следующего подэтапа.

## 3. Методы и функции
4.4: `RequestEquip()` вызывает EquipRequested(index,null); `ClickEquipmentSlot(slot)` вызывает EquipRequested при выбранной совместимой вещи, иначе UnequipRequested для снятия. `Refresh` также читает семь надетых вещей/итоговые характеристики/доступность. `BuildEquipmentLayout()` Editor добавляет только недостающую панель7 кнопок/Stats/Equip, перенастраивает три существующие кнопки/message; повторный вызов запрещён. Awake связывает7 сохранённых UnityAction, OnDestroy снимает те же listeners.

Unity `Awake` получает Inventory/gate/unit, Bind30 ячеек и button listeners, закрывает окно. `OnEnable` подписывает Refresh, `OnDisable` отписывает и закрывает/освобождает ввод, `OnDestroy` удаляет listeners. `Update` читает I/Escape/стрелки без изменения Input Actions.

`Open()` bool проверяет готовую panel/живого unit/исследование, открывает/блокирует ввод; `Close()` void скрывает/освобождает/EndDrag. `Select(index)` принимает0..29 и обновляет; `Refresh()` читает слот/Gold/описание/доступность. `SplitSelected()` делит пополам в первое пустое место, иначе сообщение. `RequestUse()` вызывает UseRequested(index), если открыто; `ShowMessage(text)` обновляет строку.

`BeginDrag(index,position)` запоминает только индекс/ghost, `MoveDrag(screenPosition)` переводит координаты в Canvas; `DropOn(destination)` bool вызывает TryMoveOrMerge ровно раз/EndDrag/Select; `EndDrag()` очищает ghost, не мутирует модель.

Editor `BuildLayout()` создаёт Canvas/панель/30 views/подписи/кнопки/ghost и ссылки, существующая panel запрещает повторную перестройку. Helpers `Panel(parent,name,color)` RectTransform с Image/Outline; `Place(rect,x,y,w,h)` устанавливает позицию/размер; `Label(...)` TMP с fallback/авторазмером; `Button(...)` Button без keyboard navigation. Сохранение сцены выполняет MCP.

## 4. Переменные и Inspector
equipButton/equipmentButtons[7]/equipmentStats — UI-ссылки. EquipRequested Action<int,EquipmentSlot?> и UnequipRequested Action<EquipmentSlot> — команды без владения. equipmentClicks — private runtime массив callback для правильной отписки.

panel, slots[30], details/goldLabel/message, split/use/close Button, dragIcon обязательны; SelectedIndex/Inventory/IsOpen readonly; dragging приватный transient. UseRequested Action<int> — будущий обработчик применения; пока без подписчика кнопка disabled.

## 5. Связи

BattleInventory.State, BattleUnit, WorldInputGate, InventorySlotView, существующий EventSystem. Canvas Scale With Screen Size960×540/MatchHeight1, PixelPerfect, sorting60.

## 6. Пример

Potion3 → Разделить½ →2/1; drag в пустой/на другой/на Potion переносит/обменивает/сливает, итог3.

## 7. Настройка в Unity

Уже добавлен на Player; InventoryCanvas отдельный root SampleScene. Не BuildLayout повторно. Ссылки созданы/сохранены MCP. Экипировка подключена4.4; использование расходуемых подключается4.5.

## 8. Ограничения и расширение

Не работает в BattleScene: мир скрыт, Open отвергает активную встречу. Не паузит время/врагов. Callback-подписчик обязан проверять реальные условия использования. Курсор/ощущения окончательно проверить владельцу.

## 9. История изменений
2026-10-12 — 4.4: семь слотов/Stats/Equip/Unequip и корректные listeners.

2026-10-12 — 4.2: окно, ввод, drag-and-drop и события.

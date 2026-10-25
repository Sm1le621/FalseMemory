# InventoryWindow

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/InventoryWindow.cs.

Существующий Player управляет окном общего инвентаря6×5.

## 2. Как работает

Открытие I, закрытие I/Escape; стрелки выбирают. Событие State.Changed обновляет30 views только после изменения/выбора. UI не хранит количества; использование делегируется UseRequested/WorldItemUseController.

## 3. Методы и функции

OnEnable/OnDisable дополнительно подписываются/отписываются ProgressChanged. OnProgressChanged при открытом окне вызывает Refresh и показывает character.level_up, если получены уровни. Update использует character.header_progress: имя/уровень/total EXP/Next или Maximum Level; без профиля сохраняет прежний заголовок.

2026-10-12 — актуальная локализация: Update/Refresh/RefreshResources формируют подписи имени/уровня/EXP/HPMP/сумки/Gold/выбора/характеристик через GameText.Get с аргументами. SplitSelected/DropOn/первоначальные editor builders читают централизованные ключи. State и события не меняются.

`RefreshResources()` читает HP/maxHP/MP/maxMP и меняет строку только при изменении четырёх чисел. Update вызывает его в открытом окне и показывает имя/уровень/EXP. Refresh дополнительно обновляет занятость сумки, портрет и EquipmentSlotView, затем текущую подсказку. Close/BeginDrag скрывают tooltip. details показывает краткие сведения выбранного предмета; полное описание находится в hover.

Редакторные методы `Build*` и файл `InventoryWindow.Layout.cs` удалены: все объекты инвентаря сохранены в `SampleScene/InventoryCanvas`, повторная перестройка кодом не требуется.
4.6: Refresh также читает реальные HP/MP в resourcesLabel. Open/Close показывают/скрывают собственный `modalBackdrop`; настройки мирового HUD не меняются. `Resources` и `InventoryBackdrop` — сохранённые объекты сцены, ссылки назначены в Inspector.
`RequestEquip()` вызывает EquipRequested(index,null); `ClickEquipmentSlot(slot)` вызывает EquipRequested при выбранной совместимой вещи, иначе UnequipRequested для снятия. `Refresh` читает восемь надетых вещей/итоговые характеристики/доступность. Семь кнопок экипировки, `EquipmentStats`, `Equip` и `message` сохранены в сцене. Awake связывает столько сохранённых UnityAction, сколько назначено кнопок (сейчас 8); OnDestroy снимает те же listeners.

Unity `Awake` получает Inventory/gate/unit, Bind30 ячеек и button listeners, закрывает окно. `OnEnable` подписывает Refresh, `OnDisable` отписывает и закрывает/освобождает ввод, `OnDestroy` удаляет listeners. `Update` читает I/Escape/стрелки без изменения Input Actions.

`Open()` bool проверяет готовую panel/живого unit/исследование, открывает/блокирует ввод; `Close()` void скрывает/освобождает/EndDrag. `Select(index)` принимает0..29 и обновляет; `Refresh()` читает слот/Gold/описание/доступность. `SplitSelected()` делит пополам в первое пустое место, иначе сообщение. `RequestUse()` вызывает UseRequested(index), если открыто; `ShowMessage(text)` обновляет строку.

`BeginDrag(index,position)` запоминает только индекс/ghost, `MoveDrag(screenPosition)` переводит координаты в Canvas; `DropOn(destination)` bool вызывает TryMoveOrMerge ровно раз/EndDrag/Select; `EndDrag()` очищает ghost, не мутирует модель.

Canvas, панель, 30 `InventorySlotView`, подписи, кнопки, `DragIcon` и все ссылки сохранены в `SampleScene`; код построения удалён вместе с `InventoryWindow.Layout.cs`.

## 4. Переменные и Inspector

Новых Inspector-полей нет. Три строки в прежнем characterLabel, параметры/геометрия не заменены.

tooltip — InventoryTooltip на существующем Canvas; characterPortrait — Image существующего спрайта героя; characterLabel — имя/уровень/EXP; bagLabel — занятость30 ячеек. shownHP/shownMaxHP/shownMP/shownMaxMP — приватный кеш отображения, старт−1. Не владеет ресурсами.
resourcesLabel — readonly отображение Health/BattleUnit, modalBackdrop — необязательный полноэкранный Image. Оба назначены MCP, закрытый фон inactive. Не владеют ресурсами.
CanUseSelection Func<int,bool> — необязательный runtime predicate; Refresh учитывает его для кнопки использования. Статический запас/Inspector-количество не создаются.
equipButton/equipmentButtons[8]/equipmentStats — UI-ссылки. EquipRequested Action<int,EquipmentSlot?> и UnequipRequested Action<EquipmentSlot> — команды без владения. equipmentClicks — private runtime массив callback для правильной отписки.

panel, slots[30], details/goldLabel/message, split/use/close Button, dragIcon обязательны; SelectedIndex/Inventory/IsOpen readonly; dragging приватный transient. UseRequested Action<int> — обработчик применения; без подписчика кнопка disabled; с4.5 подключён WorldItemUseController.

## 5. Связи

BattleUnit.ProgressChanged; CharacterGrowthRules косвенно через readonly свойства Unit. Модальное открытие всё ещё запрещено до завершения Transition.

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

BattleInventory.State, BattleUnit, WorldInputGate, InventorySlotView, существующий EventSystem. Canvas Scale With Screen Size1200×780/Expand, PixelPerfect, sorting60.

## 6. Пример

Potion3 → Разделить½ →2/1; drag в пустой/на другой/на Potion переносит/обменивает/сливает, итог3.

## 7. Настройка в Unity

Уже добавлен на Player; `InventoryCanvas` — отдельный root `SampleScene`. Все ссылки сохранены в сцене. Экипировка подключена 4.4; использование расходуемых подключено 4.5 через общую Abilities.

## 8. Ограничения и расширение

Не работает в BattleScene: мир скрыт, Open отвергает активную встречу. Не паузит время/врагов. Callback-подписчик обязан проверять реальные условия использования. Курсор/ощущения окончательно проверить владельцу.

## 9. История изменений

2026-10-12 —5.1: кривая EXP/производный рост/Level Up при прежнем владельце HP. [Проверки](../../ProgressionStage5.md).

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

2026-10-12 — RPG-панель персонажа, восемь слотов, сумка и hover; живое чтение HP/MP, прежние события/модель сохранены.
2026-10-12 — 4.6: HPMP шапка/фон открытого окна, сохранённые ссылки/доступность.
2026-10-12 — 4.5: predicate использования для разрешённого контекста/уровня.
2026-10-12 — 4.4: семь слотов/Stats/Equip/Unequip и корректные listeners.

2026-10-12 — 4.2: окно, ввод, drag-and-drop и события.

- 2026-10-25 — удалены Editor-методы `Build*` и файл `InventoryWindow.Layout.cs`; инвентарь остаётся объектами `SampleScene`.

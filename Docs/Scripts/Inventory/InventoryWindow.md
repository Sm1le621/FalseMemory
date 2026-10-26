# InventoryWindow

## Текущее изменение5.3 —2026-10-26

Добавлено readonly Unit => unit для общего Tooltip; отдельная копия персонажа не создаётся. I/Escape, slots30/gear9/drag/gate и layout не менялись. MP-текст теперь показывает фактическую вместимость40 на базовом classless-профиле.

Фактические проверки и ограничения: [ClasslessStage53](../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

Актуальное изменение2026-10-13: optional portraitSprite задаёт отдельное изображение для существующего characterPortrait, чтобы боевой холст с мечом не уменьшал героя в инвентаре. При null сохраняется прежний fallback BattleUnit.battleSprite; все предметы/ресурсы/drag остаются прежними.

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/InventoryWindow.cs.

Существующий Player управляет окном общего инвентаря6×5.

## 2. Как работает

Player rework5: одинdrag содержитлибоbag index, либоgear slot иcapturedItem. Begin/Move/Drop/End сохраняютодинcursor Image. Приdrop пересчитываютсяте же совместимость/level/hands; выбраннаяячейка/количество не копируютсявUI.

Актуально Player rework4: прежний Inventory дополнен10 строками primary; Refresh получает один итоговый набор/gear и вызывает AttributeRowView.Render. StatsChanged обновляет открытое окно при modifier/gear/resource-capacity изменении.

Открытие I, закрытие I/Escape; стрелки выбирают. Событие State.Changed обновляет30 views только после изменения/выбора. UI не хранит количества; использование делегируется UseRequested/WorldItemUseController.

## 3. Методы и функции

BeginDrag(index,pos) теперьвалидируетиндекс, завершаетстарыйdrag ификсируетItem. BeginEquipmentDrag(slot,pos) получаетвещьизgear. DragSourceValid() bool перепроверяетcapturedItem втомжеслоте. HighlightEquipmentTargets() задаётnullablecompatibility каждойView, неменяетrarityBorder. MoveDrag поддерживаетобаисточника. DropOn(destination) выбираетTryMoveOrMerge илиTryUnequipTo, затемEnd/Refresh; fullbag даётinventory.full. DropOnEquipment(slot) выбираетTryEquip илиTryMove исообщаетрезультат. EndDrag очищаетобаисточника/capturedItem/cursor/highlight. Close/OnDisable вызываютEndDrag. Перемещениевнецелевойзонынезабираетвладение.

OnEnable/OnDisable теперь дополнительно подписываются/отписываются от BattleUnit.StatsChanged→Refresh. Refresh после derived summary обновляет attributeRows; существующие операции/drag/выбор30 ячеек не менялись.

OnEnable/OnDisable дополнительно подписываются/отписываются ProgressChanged. OnProgressChanged при открытом окне вызывает Refresh и показывает character.level_up, если получены уровни. Update использует character.header_progress: имя/уровень/total EXP/Next или Maximum Level; без профиля сохраняет прежний заголовок.

2026-10-12 — актуальная локализация: Update/Refresh/RefreshResources формируют подписи имени/уровня/EXP/HPMP/сумки/Gold/выбора/характеристик через GameText.Get с аргументами. SplitSelected/DropOn/первоначальные editor builders читают централизованные ключи. State и события не меняются.

`RefreshResources()` читает HP/maxHP/MP/maxMP и меняет строку только при изменении четырёх чисел. Update вызывает его в открытом окне и показывает имя/уровень/EXP. Refresh дополнительно обновляет занятость сумки, портрет и EquipmentSlotView, затем текущую подсказку. Close/BeginDrag скрывают tooltip. details показывает краткие сведения выбранного предмета; полное описание находится в hover.

`BuildFantasyLayout()` — повторяемая редакторная перестройка существующих объектов без удаления, реализована в [InventoryWindow.Layout](InventoryWindow.Layout.md). Старые первоначальные Build* сохранены; в уже оформленной сцене их повторять не нужно.
4.6: Refresh также читает реальные HP/MP в resourcesLabel. Open/Close показывают/скрывают собственный modalBackdrop; настройки мирового HUD не меняются. Editor `BuildResourceHeader()` создаёт/находит Resources и сужает Title; `BuildModalBackdrop()` создаёт/находит Image под панелью, растягивает по всему Canvas, помещает первым sibling и сохраняет ссылку. Повторное применение не создаёт дублей.
`RequestEquip()` вызывает EquipRequested(index,null); `ClickEquipmentSlot(slot)` вызывает EquipRequested при выбранной совместимой вещи, иначе UnequipRequested для снятия. `Refresh` читает восемь надетых вещей/итоговые характеристики/доступность. Исторический `BuildEquipmentLayout()` Editor создаёт первоначальные7 кнопок/Stats/Equip и перенастраивает три существующие кнопки/message; повторный вызов запрещён. Awake связывает столько сохранённых UnityAction, сколько назначено кнопок (сейчас8); OnDestroy снимает те же listeners.

Unity `Awake` получает Inventory/gate/unit, Bind30 ячеек и button listeners, закрывает окно. `OnEnable` подписывает Refresh, `OnDisable` отписывает и закрывает/освобождает ввод, `OnDestroy` удаляет listeners. `Update` читает I/Escape/стрелки без изменения Input Actions.

`Open()` bool проверяет готовую panel/живого unit/исследование, открывает/блокирует ввод; `Close()` void скрывает/освобождает/EndDrag. `Select(index)` принимает0..29 и обновляет; `Refresh()` читает слот/Gold/описание/доступность. `SplitSelected()` делит пополам в первое пустое место, иначе сообщение. `RequestUse()` вызывает UseRequested(index), если открыто; `ShowMessage(text)` обновляет строку.

`BeginDrag(index,position)` запоминает только индекс/ghost, `MoveDrag(screenPosition)` переводит координаты в Canvas; `DropOn(destination)` bool вызывает TryMoveOrMerge ровно раз/EndDrag/Select; `EndDrag()` очищает ghost, не мутирует модель.

Editor `BuildLayout()` создаёт Canvas/панель/30 views/подписи/кнопки/ghost и ссылки, существующая panel запрещает повторную перестройку. Helpers `Panel(parent,name,color)` RectTransform с Image/Outline; `Place(rect,x,y,w,h)` устанавливает позицию/размер; `Label(...)` TMP с fallback/авторазмером; `Button(...)` Button без keyboard navigation. Сохранение сцены выполняет MCP.

Refresh выбирает portraitSprite при наличии, иначе BattleUnit.battleSprite; выставляет прежний цвет участника и включённость Image по наличию Sprite. Никакие состояния инвентаря/здоровья этим выбором не меняются.

## 4. Переменные и Inspector

draggingEquipment nullableprivate EquipmentSlot иdraggedItem private ItemData — transient, неInspector/save. equipmentButtons содержит9кнопок, первоначальныеиндексысохранены.

attributeRows — новый массив10 AttributeRowView в Inspector, назначен сохранённой SampleScene; это views, не отдельные характеристики.

Новых Inspector-полей нет. Три строки в прежнем characterLabel, параметры/геометрия не заменены.

tooltip — InventoryTooltip на существующем Canvas; characterPortrait — Image существующего спрайта героя; characterLabel — имя/уровень/EXP; bagLabel — занятость30 ячеек. shownHP/shownMaxHP/shownMP/shownMaxMP — приватный кеш отображения, старт−1. Не владеет ресурсами.
resourcesLabel — readonly отображение Health/BattleUnit, modalBackdrop — необязательный полноэкранный Image. Оба назначены MCP, закрытый фон inactive. Не владеют ресурсами.
CanUseSelection Func<int,bool> — необязательный runtime predicate; Refresh учитывает его для кнопки использования. Статический запас/Inspector-количество не создаются.
equipButton/equipmentButtons[8]/equipmentStats — UI-ссылки. EquipRequested Action<int,EquipmentSlot?> и UnequipRequested Action<EquipmentSlot> — команды без владения. equipmentClicks — private runtime массив callback для правильной отписки.

panel, slots[30], details/goldLabel/message, split/use/close Button, dragIcon обязательны; SelectedIndex/Inventory/IsOpen readonly; dragging приватный transient. UseRequested Action<int> — обработчик применения; без подписчика кнопка disabled; с4.5 подключён WorldItemUseController.

portraitSprite — optional Sprite в Inspector, по умолчанию null; в SampleScene назначен прежний ApprovedHero_Down_00 из четырёхкадрового листа512×640. characterPortrait по-прежнему ссылка на существующий Image.

## 5. Связи

EquipmentController/State выполняютатомарнуюлогику; Window не пишетgear массив/bonuses сам. Pointerhandlers находятсявSlots.

CharacterAttributes/Rules через BattleUnit, AttributeRowView и прежний InventoryTooltip. Старые ресурсы/InventoryState остаются общими.

BattleUnit.ProgressChanged; CharacterGrowthRules косвенно через readonly свойства Unit. Модальное открытие всё ещё запрещено до завершения Transition.

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

BattleInventory.State, BattleUnit, WorldInputGate, InventorySlotView, существующий EventSystem. Canvas Scale With Screen Size1200×780/Expand, PixelPerfect, sorting60.

## 6. Пример

Potion3 → Разделить½ →2/1; drag в пустой/на другой/на Potion переносит/обменивает/сливает, итог3.

## 7. Настройка в Unity

Уже добавлен на Player; InventoryCanvas отдельный root SampleScene. Не BuildLayout повторно. Ссылки созданы/сохранены MCP. Экипировка подключена4.4; использование расходуемых подключено4.5 через общую Abilities.

## 8. Ограничения и расширение

Сумка30сохранена. Приdrop вoccupied bag можнообменятьтолькосовместимуюодиночнуюgear; consumable/material/stack несовместимы. Полныйbagнеудаляетнадетое. Покаэтоcurrent session, не дисковоеsave/load.

Окно показывает primary и equipment suffix отдельно; источники class/temporary/growth в tooltip. Class modifier не означает готовый выбор класса. Дисковых сохранений нет.

Не работает в BattleScene: мир скрыт, Open отвергает активную встречу. Не паузит время/врагов. Callback-подписчик обязан проверять реальные условия использования. Курсор/ощущения окончательно проверить владельцу.

## 9. История изменений

- 2026-10-12 — Player rework5: девятыйOffHand, bidirectionaldrag/atomictransfers/совместимость/силуэты; sourceIDsсохранены.

- 2026-10-12 — Player rework4: primary attributes подключены к исходным ресурсам/бою/UI; прежние идентификаторы и игровая сессия сохранены.

2026-10-12 —5.1: кривая EXP/производный рост/Level Up при прежнем владельце HP. [Проверки](../../ProgressionStage5.md).

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

2026-10-12 — RPG-панель персонажа, восемь слотов, сумка и hover; живое чтение HP/MP, прежние события/модель сохранены.
2026-10-12 — 4.6: HPMP шапка/фон открытого окна, сохранённые ссылки/доступность.
2026-10-12 — 4.5: predicate использования для разрешённого контекста/уровня.
2026-10-12 — 4.4: семь слотов/Stats/Equip/Unequip и корректные listeners.

2026-10-12 — 4.2: окно, ввод, drag-and-drop и события.

- 2026-10-13 — портрет отделён от боевого рисунка единственной optional ссылкой без изменения предметов/ресурсов.

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

# EquipmentSlotView

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/EquipmentSlotView.cs`.

Отображает один надетый предмет в кнопке экипировки и показывает hover-подсказку. Не меняет владение: операции выполняет `EquipmentController` по команде `InventoryWindow`.

## 2. Как работает

`InventoryWindow.Refresh` передаёт в `Render` текущий надетый `ItemData`. Компонент показывает иконку, название слота и цвета фона/рамки. При наведении мыши он просит общий `InventoryTooltip` показать сведения, при уходе или выключении — скрыть их.

Свойство `Item` каждый раз читает актуальное значение из `EquipmentController.State`, а не хранит копию владения.

## 3. Методы и функции

- `Item` — readonly `ItemData`: надетый предмет из `owner.GetComponent<EquipmentController>().State.GetSlot(slot)` либо `null`, если владельца или контроллера нет.
- `Render(ItemData worn)` — запоминает отображаемую ссылку, обновляет `icon`, видимость иконки, `slotLabel` через `ItemPresentation.SlotName` и цвета.
- `RefreshColors()` — задаёт цвет фона через `ItemPresentation.CellColor`; рамка получает цвет `(150,133,101)` при предмете и `(109,95,74)` при пустом слоте.
- `OnPointerEnter(PointerEventData e)` — Unity EventSystem callback; включает hover и вызывает `tooltip.Show(this, e.position)`.
- `OnPointerExit(PointerEventData e)` — выключает hover и скрывает подсказку этого слота.
- `OnDisable()` — сбрасывает hover, обновляет цвета и скрывает подсказку; экипировка не меняется.

## 4. Переменные и Inspector

- `owner` — `InventoryWindow` на том же объекте Player.
- `slot` — `EquipmentSlot.Weapon` или `EquipmentSlot.Chest`.
- `icon`, `background` — `Image`; `border` — `Outline`; `slotLabel` — `TextMeshProUGUI`.
- `item`, `hovered` — private состояние отображения.

В сцене используются две кнопки экипировки. Inspector-настройки ограничены ссылками на графику и слотом.

## 5. Связи

Компонент стоит на кнопках `InventoryWindow.equipmentButtons`, которые уже имеют `Button.onClick` для команд окна. `InventoryWindow.Refresh` вызывает `Render`; `InventoryTooltip` читает `Item`; `EquipmentController` предоставляет данные; `ItemPresentation` задаёт подпись слота и цвета.

## 6. Пример

Iron Sword надет в слот Weapon. Кнопка показывает иконку меча и подпись `Weapon`; при наведении tooltip выводит имя, тип, `ATK +5` и описание. После снятия меча `Render(null)` скрывает иконку и возвращает нейтральную рамку.

## 7. Настройка в Unity

Компонент добавляется на кнопку слота в `SampleScene/InventoryCanvas`. Нужно назначить `owner`, `slot`, `icon`, `background`, `border`, `slotLabel`. Ссылки на кнопки должны совпадать с массивом `InventoryWindow.equipmentButtons`.

## 8. Ограничения и расширение

Подсказка появляется только для надетого предмета; пустой слот ничего не показывает. Нет редкости, уровней и дополнительных слотов. При добавлении слота нужно расширить enum, массив кнопок и сцену.

## 9. История изменений

- 2026-10-31 — документ приведён к текущему коду: две кнопки, нейтральные цвета, без редкости и восьми слотов.
- 2026-10-12 — создано отображение экипировки/редкости/hover для текущих кнопок. *(Историческая запись: редкость удалена.)*
- 2026-10-25 — оформление экипировки остаётся объектами сцены; код построения удалён.

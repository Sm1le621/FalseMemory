# InventorySlotView

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/InventorySlotView.cs`.

Одна ячейка окна инвентаря: показывает иконку, количество, выделение и hover-подсказку. Не владеет количеством и не меняет инвентарь напрямую — передаёт действия `InventoryWindow`.

## 2. Как работает

`InventoryWindow.Awake` вызывает `Bind` для каждой из 30 ячеек. `Render` читает readonly `InventorySlot` и обновляет графику. Клик выбирает ячейку, drag начинает перенос, drop передаёт индексы окну; только `InventoryWindow.DropOn` вызывает изменение модели.

## 3. Методы и функции

- `Bind(InventoryWindow owner, int index)` — сохраняет владельца и индекс.
- `Render(InventorySlot slot, bool selected)` — запоминает выбранность и предмет, обновляет иконку, её видимость и текст количества; пустая ячейка показывает пустую строку.
- `RefreshColors()` — фон через `ItemPresentation.CellColor`, рамка выбранной ячейки толще (`2,-2`) и светлее, обычной — `(1,-1)`.
- `OnDisable()` — сбрасывает hover и скрывает подсказку этого слота.
- `OnPointerEnter(PointerEventData e)` / `OnPointerExit(PointerEventData e)` — показывают/скрывают общий tooltip.
- `OnPointerClick(PointerEventData e)` — левый клик вызывает `Owner.Select(Index)`.
- `OnBeginDrag`, `OnDrag`, `OnEndDrag` — запускают, двигают и завершают ghost-иконку через окно.
- `OnDrop(PointerEventData e)` — принимает drop только от ячейки того же `Owner` и вызывает `Owner.DropOn(Index)`.

Unity EventSystem callbacks не применяют предметы и не меняют количества сами.

## 4. Переменные и Inspector

- `background`, `icon` — `Image`; `quantity` — `TextMeshProUGUI`; `border` — `Outline`.
- `Owner` — `InventoryWindow`, `Index` — `0..29`; выставляются через `Bind`.
- `hovered`, `selected`, `item` — private состояние отображения.

В сцене созданы 30 экземпляров; все графические ссылки назначены в `SampleScene/InventoryCanvas`.

## 5. Связи

`InventoryWindow`, `InventoryState`, `InventoryTooltip`, `ItemPresentation`, EventSystem и Input System. Иконка и текст не должны перехватывать raycast, иначе drag/hover работают нестабильно.

## 6. Пример

Potion лежит в ячейке 2. Наведение показывает подсказку, левый клик выбирает ячейку, drag в ячейку 9 переносит всю стопку. Если отпустить кнопку вне окна, `EndDrag` убирает ghost, а исходная стопка остаётся на месте.

## 7. Настройка в Unity

Компонент находится на каждой ячейке `InventoryCanvas`. Нужно назначить `background`, `icon`, `quantity`, `border`; `Owner` и `Index` выставляет окно в `Awake`. Добавлять или удалять ячейки без изменения массива `InventoryWindow.slots` нельзя.

## 8. Ограничения и расширение

Нет разделения стопки, контекстного меню и переноса между разными окнами. Неверные ссылки или перехват raycast ломают отображение. Выбор ограничен индексами `0..29`.

## 9. История изменений

- 2026-10-31 — документ приведён к текущему коду: 30 ячеек, без редкости и без разделения стопки.
- 2026-10-12 — редкости, hover, рамка выбора и сброс состояния при закрытии. *(Историческая запись: редкость удалена.)*
- 2026-10-12 — 4.2: обработчики pointer и readonly отображение.

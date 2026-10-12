# InventorySlotView

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/InventorySlotView.cs.

Ячейка окна: визуал/мышь, не владелец количества.

## 2. Как работает

Window.Bind задаёт Owner/Index; Render читает readonly слот. Pointer-события делегируются Window, модель меняется только на Drop.

## 3. Методы и функции

`RefreshColors()` обновляет приглушённый фон/рамку по ItemPresentation; выбор и hover усиливают фон, выбранная рамка толще. `OnPointerEnter(e)` включает hover и вызывает Tooltip.Show(this,e.position); `OnPointerExit(e)` выключает его и скрывает только собственную подсказку. `OnDisable()` сбрасывает hover и скрывает подсказку при закрытии. Методы void, количество не меняют.

`Bind(owner,index)` void назначает ссылки. `Render(slot,selected)` обновляет фон/иконку/число. `OnPointerClick(e)` выбирает левым кликом; `OnBeginDrag(e)` начинает drag левой кнопкой; `OnDrag(e)` передвигает ghost; `OnEndDrag(e)` отменяет/заканчивает его. `OnDrop(e)` принимает только source из того же Owner и вызывает DropOn. Эти Unity EventSystem callbacks не применяют предметы сами.

## 4. Переменные и Inspector

rarityBorder Outline — рамка; item/selected/hovered — приватные текущие значения отображения, не данные владения. Render сохраняет item/selected перед обновлением цвета.

background/icon Image, quantity TMP обязательны; Owner/Index runtime readonly, индекс0..29.

## 5. Связи

InventoryWindow, EventSystem/GraphicRaycaster/Input System, InventoryState.

## 6. Пример

Drag Potion в пустую ячейку → одно перемещение; drop вне окна → исходный слот сохраняется.

## 7. Настройка в Unity

Уже создано30 views в InventoryCanvas через MCP. Bind вызывается Awake владельца, в Inspector назначены графика/текст.

## 8. Ограничения и расширение

Правильные ссылки обязательны. Иконки/количество не перехватывают raycast. Нельзя перетаскивать между разными игроками/окнами.

## 9. История изменений

2026-10-12 — редкости, hover, рамка выбора и сброс состояния при закрытии.

2026-10-12 — 4.2: обработчики pointer и readonly отображение.

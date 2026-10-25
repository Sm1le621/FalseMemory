# EquipmentSlotView

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/EquipmentSlotView.cs.

Отображение одного надетого предмета и hover-подсказки рядом с персонажем; обработка экипировки остаётся у InventoryWindow/EquipmentController.

## 2. Как работает

Refresh окна передаёт надетый ItemData в Render. Иконка показывается при наличии Sprite, рамка соответствует редкости. При наведении Tooltip читает актуальную модель, при уходе скрывается.

## 3. Методы и функции

`Item` → текущий ItemData из EquipmentController.State.GetSlot(slot) либо null без владельца/контроллера. `Render(worn)` void запоминает только отображаемую ссылку, обновляет icon/slotLabel/цвет. `RefreshColors()` void обновляет фон и Outline через ItemPresentation. `OnPointerEnter(e)` Unity EventSystem callback включает hover/Tooltip.Show(this,e.position); `OnPointerExit(e)` выключает hover/Hide(this). `OnDisable()` очищает hover/подсказку при закрытии; владение не меняется.

## 4. Переменные и Inspector

owner — InventoryWindow; slot — EquipmentSlot (в сцене все8); icon/background Image, rarityBorder Outline, slotLabel TMP — назначены построителем. item/hovered — private кеш представления.

## 5. Связи

На существующих equipmentButtons есть Button со старым onClick окна; данный компонент добавляет только визуальное представление. Иконка/текст не перехватывают raycast.

## 6. Пример

Выберите Iron Sword → Экипировать. Слот Оружие показывает меч и голубую рамку; наведите — Rare, ATK+5. Снятие обновляет тот же слот.

## 7. Настройка в Unity

Компоненты уже стоят на восьми кнопках `SampleScene`; ссылки назначены в сцене, редакторный `BuildFantasyLayout` удалён. Не добавлять вторые обработчики экипировки или отдельный запас.

## 8. Ограничения и расширение

Пустой слот не показывает item-tooltip. В демонстрационных данных нет Gloves/Legs/Boots/Accessory; их UI и модель существуют, назначить ItemData соответствующего типа. Кнопка при несовместимом выборе снимает надетое — прежнее поведение.

## 9. История изменений

2026-10-12 — создано отображение экипировки/редкости/hover для текущих кнопок.

2026-10-25 — оформление экипировки остаётся объектами сцены; код построения удалён.

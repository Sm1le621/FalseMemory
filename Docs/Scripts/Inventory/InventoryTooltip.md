# InventoryTooltip

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/InventoryTooltip.cs.

Одна неперехватывающая мышь hover-панель для предметов сумки и экипировки.

## 2. Как работает

Show запоминает источник, читает живой предмет/количество, строит текст и размер. LateUpdate следует за мышью. Position ограничивает панель Canvas; при пустом источнике, закрытии или drag подсказка скрывается.

## 3. Методы и функции

`Show(InventorySlotView,position)` и `Show(EquipmentSlotView,position)` void выбирают источник и вызывают RefreshContent. `Hide(caller=null)` void: без caller скрывает всё, с caller только его подсказку — поздний Exit другой ячейки не закрывает новую. `RefreshContent()` void проверяет владельца/IsOpen/dragIcon, читает State либо EquipmentSlotView.Item; null → Hide. Назначает текст/рамку, ширину≤320/высоту по GetPreferredValues с ограничением Canvas, включает/поднимает panel и вызывает Position.

`LateUpdate()` Unity callback при видимой панели читает Mouse.current.position (без мыши сохраняет последний pointer), скрывает закрытое окно, вызывает Position. `Position(screenPosition)` void переводит экранную точку в локальные координаты родителя, смещает22 вправо/вниз и ограничивает RectTransform с отступом8. `IsVisible` readonly → panel.activeSelf.

## 4. Переменные и Inspector

owner — окно; panel — InventoryTooltip RectTransform; content — TMP; rarityBorder — Outline. source — private Unity Object текущего View, pointer — последний экранный Vector2. Размеры/отступы в коде. TMP16, авторазмер минимум12, Ellipsis для чрезмерно длинного контента; ссылки назначены сценой.

## 5. Связи

ItemPresentation, InventoryWindow, InventoryState, EquipmentController, Mouse Input System. Компонент на InventoryCanvas (остаётся активным); визуальный child выключается отдельно.

## 6. Пример

Наведите Potion3: имя, Common, расходник, до30HP и цены. Переместите мышь к правому краю — панель сдвигается внутрь экрана; уход/перенос/закрытие скрывает её.

## 7. Настройка в Unity

Сохранён в `SampleScene/InventoryCanvas` (создавался один раз через удалённый ныне `BuildFantasyLayout`); panel anchor(.5,.5), pivot(0,1), дочерний text растянут с16 отступами. Все tooltip Graphics имеют raycastTarget=false.

## 8. Ограничения и расширение

Ориентирован на Screen Space Overlay существующего Canvas. Touch/gamepad и клавиатурное открытие tooltip не добавлены. Очень длинный авторский текст сокращается многоточием; для обычных описаний данные показываются целиком. Не создавать второй tooltip на каждый слот.

## 9. История изменений

2026-10-12 — hover сумки/экипировки, живое обновление и ограничение экраном; реальная мышь проверена Play Mode тестом.

2026-10-25 — уточнено: объект уже сохранён в сцене, редакторный `BuildFantasyLayout` удалён.

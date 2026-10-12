# InventoryWindow.Layout

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/InventoryWindow.Layout.cs.

Редакторная часть того же partial InventoryWindow: оформление существующего UI по RPG-референсу, без альтернативной системы/сцены.

## 2. Как работает

Актуально Player rework4: BuildAttributeLayout отдельно расширяет существующий RPG-макет, не строит второй Canvas и не удаляет старые объекты.

BuildFantasyLayout переиспользует сохранённые ячейки, кнопки и подписи; добавляет отсутствующие секции, портрет/hover. Повторный вызов на этой сцене не создаёт дублей. Только редакторный код под UNITY_EDITOR.

## 3. Методы и функции

Player rework5: BuildFantasyLayout расширяетequipmentButtons до9 идобавляетOffHand безперестановкиenumиндексов; layout порядокWeapon,OffHand,Head,Chest,Gloves,Legs,Boots,Accessory1,Accessory2. Icon emptySprite отдельноназначаетсяMCP; послеBuildFantasyLayout вызываетсяBuildAttributeLayout. Scene сохраняетсяотдельно.

BuildAttributeLayout() Editor-only void требует вне Play и готовую panel. Переиспользует EquipmentTitle/EquipmentStats/message, добавляет или находит10 Attribute* с Image/AttributeRowView/TMP; задаёт owner/enum/label и массивattributeRows. Layout два столбца по5, затем SetDirty/MarkSceneDirty; сохранение вызывается отдельно MCP. Повторный вызов не создаёт дубликаты.

2026-10-12 — актуальная локализация: BuildFantasyLayout читает inventory.title/character_equipment/bag_initial/attributes/controls, сохраняя геометрию. Saved подписи SampleScene и статические LocalizedLabel настроены отдельно через MCP.

`BuildFantasyLayout()` void запрещает Play Mode/отсутствующую panel/не30 slots; задаёт размер/Scaler, переставляет старые элементы, добавляет8-й слот Gloves без изменения индексов, назначает views/tooltip/ссылки, закрывает окно и помечает существующую сцену dirty. Сохранение отдельно через Unity MCP.

`Section(name,x,y,w,h)` → RectTransform найденной/новой панели с рамкой, raycast=false. `EnsureLabel(name,text,size,x,y,w,h)` → найденный/новый TMP и позиция. `EnsureImage(parent,name)` → найденный/новый Image без raycast. `MoveButton(button,x,y,width)` void переставляет существующую кнопку/подпись и задаёт тёмный фон. Общие helpers Panel/Place/Label/Button остаются в InventoryWindow.cs. Unity callbacks нет.

## 4. Переменные и Inspector

Две колонки gear x71/345, y552−80×row, пять строк слева/четыре справа; icons44×44/button54×54, labels112×22 с отступом−25. Это оставляет место подписи до следующей ячейки; первоначальная сетка68/78 пересекалась и исправлена до финального сохранения. Bag6×5 и10 Attributes сохранены.

Строки x40/246, y156−20×row,192×18; TMP10…13. Заголовок177, derived summary44 высота28 font10…12, message только справа494/54. Сводка имеет отдельный двухстрочный key character.derived_summary, чтобы не переполнять прежние три строки. Panel/Canvas/sumka30 сохраняются.

Ссылки хранятся в основной части InventoryWindow. Panel1140×720; Canvas reference1200×780, Expand — равномерное масштабирование по доступному экрану; PixelPerfect/sorting60 сохранены. Сумка6×5, ячейка68/шаг96 поX76 поY. Две колонки экипировки вокруг исходного Point-спрайта. Спрайт/импорт не меняются.

## 5. Связи

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

Существующий InventoryCanvas SampleScene, InventoryWindow на Player, ItemPresentation/EquipmentSlotView/InventoryTooltip, UnityEditor сохранение сцены.

## 6. Пример

Выполненный MCP вызов Player.GetComponent<InventoryWindow>().BuildFantasyLayout() преобразует старое окно, сохраняя ссылки/кнопки Use, Equip, Split, Close и30 slots.

## 7. Настройка в Unity

После BuildFantasyLayout вызывать BuildAttributeLayout и сохранять текущую SampleScene. Повторный BuildFantasyLayout переставляет summary/message, поэтому следом нужен BuildAttributeLayout.

Не отдельный MonoBehaviour для добавления в Inspector. Вызывать на существующем InventoryWindow вне Play Mode; затем сохранить ту же SampleScene. Второго Canvas или менеджера не создаёт.

## 8. Ограничения и расширение

Силуэт маскинеявляетсяitem ownership. Нельзя удалитьстарыеbuttons/slotIDs радиlayout; новый9-йButtonперепроверен черезMCP.

Проверять4 заданных разрешения и tooltip; подготовка directional art не относится к этому UI.

Предполагает существующий этап4 с Resources/Title/EquipmentTitle/Controls. Не является универсальным автоматическим мигратором произвольных сцен. Позиции — единый дизайн окна, масштабирование обеспечивает Scaler; ориентировано на ПК, не мобильную навигацию.

## 9. История изменений

- 2026-10-12 — Player rework5: девятыйOffHand, bidirectionaldrag/atomictransfers/совместимость/силуэты; sourceIDsсохранены.

- 2026-10-12 — Player rework4: primary attributes подключены к исходным ресурсам/бою/UI; прежние идентификаторы и игровая сессия сохранены.

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

2026-10-12 — повторяемая перестройка старого UI без удаления объектов, центрированный портрет/характеристики/сумка/hover.

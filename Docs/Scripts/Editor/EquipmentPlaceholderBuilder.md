# EquipmentPlaceholderBuilder

Исходник: `Assets/Editor/EquipmentPlaceholderBuilder.cs`.

## 1. Название и назначение

Editor-only авторинг восьми небольших временных силуэтов снаряжения, native64 PNG. Это аккуратные серые маски, не финальная графика предметов.

## 2. Как работает

Generate заранее проверяет отсутствие всех выходных файлов, затем создаёт оригинальные пиксельные маски/PNG и импортирует как Sprite. Существующие изображения не читаются, не редактируются, не увеличиваются и не перезаписываются.

## 3. Методы и функции

`Generate()` MenuItem/публичный static void: запрещён вPlay, проверяет файлы, создаёт64 Texture2D/PNG, уничтожает только временную Texture, импортирует Point/Uncompressed/Single. `Draw(Grid,string)` выбирает оригинальную маску Sword/Shield/Helmet/Chest/Gloves/Legs/Boots/Ring. Grid.`Set(x,y)` пишет gray182/255alpha в пределах64; `Box` заполняет прямоугольник; `Line` целочисленный Bresenham/thickness. Unity callbacks отсутствуют.

## 4. Переменные и Inspector

Root Assets/Art/UI/Equipment64; Names8; Grid.pixels4096 Color32, default transparent0. Gray182/RGBAalpha255; UI задаёт дополнительную прозрачность. Import PPU100, Point, noMipmaps, Uncompressed, maxSize64,center pivot/clamp. Inspector поля нет.

## 5. Связи

UnityEditor AssetDatabase/TextureImporter, UnityEngine Texture2D. EquipmentSlotView.emptySprite назначается MCP; Accessory1/2 используют общий Ring. Файлы не зависят от Enemy/Skeleton authoring.

## 6. Пример

Empty Weapon показывает полупрозрачный Sword.png; экипированный предмет заменяет маску настоящим item.icon, после снятия возвращает её.

## 7. Настройка в Unity

Не компонент. Menu FalseMemory → Art → Generate equipment slot silhouettes; один раз для отсутствующего набора. В задаче вызван через Unity MCP, затем сохранены ссылки SampleScene. Повторная генерация существующего набора намеренно запрещена.

## 8. Возможные проблемы

Временные простые изображения требуют художественной приёмки. Это не герой/Walk анимации; программная перерисовка myproject1idle не выполняется. При наличии любого выходного файла отказ до записи; вручную не удалять оригиналы ради повторного вызова.

## 9. История изменений

- 2026-10-12 — Player rework5: Off-Hand, атомарные переносы и временные native64 силуэты; дисковые сохранения ещё отсутствуют.

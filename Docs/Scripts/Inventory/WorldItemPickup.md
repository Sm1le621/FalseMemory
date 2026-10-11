# WorldItemPickup

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/WorldItemPickup.cs.

Предмет на земле с атомарным получением; в этом файле общий IWorldInteractable для будущих NPC/сундуков.

## 2. Как работает

Доступность учитывает active/enabled/item/quantity/Collected. TryInteract проверяет живого Player/Inventory/gate/расстояние. Резервирует Collected до события Inventory.Changed, добавляет всё количество; при отказе освобождает резерв и оставляет объект. Успех выключает собственный объект на текущую сессию.

## 3. Методы и функции

IWorldInteractable: `Prompt` string подсказка; `Available` bool; `TryInteract(player,out message)` bool+текст. WorldItemPickup реализует их: Prompt E/имя/количество, Available правильные данные/неподобранный; TryInteract проверяет дальность по PlayerInteractor либо1.2 и мировой gate, ставит резерв, вызывает общий TryAdd, откатывает отказ или выключает объект. Unity callbacks нет.

## 4. Переменные и Inspector

worldId — стабильный уникальный ID размещения; item — статический ItemData; quantity≥1 (код1), в сцене Potion2/Ether2/оружие/броня1/Bone5. Collected readonly runtime bool, начальноfalse. WorldId не хранит прогресс на диск.

## 5. Связи

PlayerInteractor/IWorldInteractable, общий BattleInventory.State, Collider2D trigger. SpriteRenderer временных значков — только оформление. Префаб WorldItemPickup сохранён MCP, семь instance имеют разные IDs.

## 6. Пример

Полный инвентарь без подходящего места → предмет остаётся. Освобождён слот → E выдаёт всю награду и выключает предмет. Повторные вызовы отказ.

## 7. Настройка в Unity

Префаб Assets/Prefabs/WorldItemPickup.prefab: Collider2D trigger, SpriteRenderer, этот компонент. Для каждого размещения задать отдельный worldId/item/quantity. Тестовые размещения под root TestLoot слева/ниже Player, позиции выбраны без физических блокеров.

## 8. Ограничения и расширение

Нельзя копировать одинаковый worldId для будущей загрузки. Нет дисковых сохранений/лос/торговли. Подбор не требует покупной цены/уровня; ограничения использования отдельно. Null/неположительные данные не активны.

## 9. История изменений

2026-10-12 — 4.3: резерв/атомарный подбор и общий контракт взаимодействия.

# PlayerInteractor

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/PlayerInteractor.cs.

Единый поиск ближайшего взаимодействия и отдельное нажатие E во время исследования.

## 2. Как работает

Периодически собирает физические Collider2D в переиспользуемый List, выбирает доступный IWorldInteractable по расстоянию. E запускает один вызов. При gate/смерти/бое взаимодействие закрыто. Не использует Find всех объектов каждый кадр.

## 3. Методы и функции

Unity `Awake` получает Unit/Gate. `Update` истекает уведомление, проверяет блокировки (очищает оба текста при модальном окне/бое/смерти), по scanInterval вызывает Scan, обновляет prompt, принимает eKey.wasPressedThisFrame и очищает текущий target после вызова. `Scan` void: Physics2D.OverlapCircle с trigger/filter/list, ближайший доступный target с центром внутри radius. `ShowNotification(text)` void: текст и срок2.5сек. Unity `OnDisable` очищает target/list/оба текста без сброса инвентаря.

## 4. Переменные и Inspector

interactionRadius1.2≥0.1; scanInterval0.15 (0.05..0.5), сохранены в сцене. prompt/notification TMP назначены в InventoryCanvas; Current readonly interface. unit/gate/nearby/nextScan/notificationUntil внутренние.

## 5. Связи

WorldInputGate, BattleUnit/Transition и любой IWorldInteractable. Открытый инвентарь блокирует E. Input Actions не менялись, используется существующий Input System.

## 6. Пример

Подход к Potion → E-подсказка → отдельное нажатие → Получено/однократный запас. Удержание E не повторяет вызов.

## 7. Настройка в Unity

Компонент добавлен MCP на прежний Player, ссылки сохранены; новые TMP-labels в Canvas. Будущие NPC реализуют тот же интерфейс вместо второго обработчика E.

## 8. Ограничения и расширение

На карте с большими количествами targets настраивать layer/filter/interval по профилю. Сейчас один player и маленький query, list переиспользуется. Видимость через стены/лос ещё не реализована; подбор ограничен близостью. Смерть/выгрузка очищают подсказки.

## 9. История изменений
2026-10-12 — 4.6: уведомление скрывается при модальном вводе, не накладывается на сообщения инвентаря.

2026-10-12 — 4.3: общий E/физический поиск/подсказка.

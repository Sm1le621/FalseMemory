# WorldInputGate

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/WorldInputGate.cs.

Блокировка мирового ввода владельцами открытых окон без изменения Time.timeScale/enabled контроллера.

## 2. Как работает

HashSet хранит уникальных владельцев. Повторный Acquire одного окна не создаёт второй замок; Release чужого владельца не снимает остальные.

## 3. Методы и функции

`Acquire(owner)` void добавляет ненулевого владельца; `Release(owner)` void удаляет только его; `IsBlocked` bool при непустом наборе. Unity `OnDisable` очищает transient владельцев при скрытии/выгрузке мира.

## 4. Переменные и Inspector

owners приватный HashSet<object>; IsBlocked readonly. Inspector параметров нет.

## 5. Связи

InventoryWindow получает/освобождает замок; PlayerController проверяет и Update, и FixedUpdate. Будущие NPC используют тот же gate.

## 6. Пример

Открыт I → ходьба/анимация остановлены → Escape освобождает владельца → движение работает.

## 7. Настройка в Unity

Компонент на существующем Player; InventoryWindow RequireComponent добавляет его. Не нужен singleton.

## 8. Ограничения и расширение

Не глобальная пауза мира. Нельзя забывать Release; окно делает это Close/OnDisable. Сохранений замков нет.

## 9. История изменений

2026-10-12 — 4.2: владение модальным вводом.

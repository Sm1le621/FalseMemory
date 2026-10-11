# BattleConsumable

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleConsumable.cs.

ScriptableObject определения лечебного расходуемого предмета.

## 2. Как работает

Хранит общий эффект, а количество лежит в BattleInventory. BattleAbilities применяет healing к разрешённой цели.

## 3. Методы и функции

Методов/Unity callbacks нет; CreateAssetMenu FalseMemory/Battle Consumable создаёт данные.

## 4. Переменные и параметры Inspector

displayName — код Potion; target — код Self; healing≥0, код 30; effectColor — зелёный Color. Эти значения назначены Potion.asset.

## 5. Связи с другими системами

Assets/Settings/Battle/Items/Potion.asset; ссылки в BattleInventory.items[].definition; BattleAbilities/Manager/UI.

## 6. Пример работы

Potion.asset с healing30 → использование одного Potion восстанавливает до 30 HP.

## 7. Настройка в Unity

Это ScriptableObject, не компонент GameObject. Create → FalseMemory → Battle Consumable создаёт лечебное определение; настрой имя, Target, Healing, цвет и назначь Definition стопки BattleInventory. Готовый Potion в Assets/Settings/Battle/Items; Count в ассете не хранится.

## 8. Возможные проблемы и проверки

Поддерживается только лечение HP: Ether, Phoenix Down и антидоты отсутствуют. Количество нельзя хранить в общем ассете. Лечение Potion проверено тестами.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-11 — изменения этапа 2 сохранены в локальном Git после повторной проверки: 40 тестов Edit Mode и 17 тестов Play Mode прошли. Поведение этого файла описано выше; в задаче главного промпта его код повторно не переписывался.

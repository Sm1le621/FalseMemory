# BattleConsumable

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleConsumable.cs.

Прежний SO расходуемого наследует ItemData. Один предмет в мире и бою.

## 2. Как работает

Статическое имя/ID/цены/иконка/лимит наследуются; target/healing/manaRestoration/effectColor определяют эффект. Runtime количество отдельно.

## 3. Методы и функции

Собственных методов/callbacks нет; наследуемые IsEquipment/StackLimit/CanSell/Fits в ItemData. Эффект вызывает BattleAbilities.

## 4. Переменные и Inspector

target Self/Enemy; healing≥0 (код30), manaRestoration≥0 (код0), effectColor зелёный. Potion прежние имя/30HP/GUID, item.potion/лимит99/цены30/15. Ether healing0/manaRestoration20, item.ether/цены50/25. С4.5 MP-эффект подключён общей логикой; visualEffect — необязательный BattleVFXProfile, EtherVisual голубой Orbit.

## 5. Связи

ItemData, InventoryState, BattleAbilities/Manager/UI. Прежняя ссылка/ассет Potion не заменены.

## 6. Пример

Определение Potion не хранит владение3/2; это делает общий State.

## 7. Настройка в Unity

Create → FalseMemory → Battle Consumable, задать имя/ID/категорию Consumable/цель/эффект. Не компонент. Ether healing0/manaRestoration20.

## 8. Ограничения и расширение

Не воскрешает. Ether20MP подключён миру/бою; поддерживается один положительный HP либо MP эффект, смешанные/нулевые отвергаются. SO не должен хранить количество.

## 9. История изменений
2026-10-12 — 4.5: Ether/ограничения подключены и поле visualEffect для профиля расходуемого.

2026-10-12 — 4.1: наследование ItemData и данные Ether, Potion сохранён.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-11 — изменения этапа 2 сохранены в локальном Git после повторной проверки: 40 тестов Edit Mode и 17 тестов Play Mode прошли. Поведение этого файла описано выше; в задаче главного промпта его код повторно не переписывался.

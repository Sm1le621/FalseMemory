# BattleAbilities

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleAbilities.cs.

Проверка и применение магии/лечебных предметов через единый Health.

## 2. Как работает

Can... проверяет участника, цель, данные и ресурсы. Try... повторяет проверку, списывает ресурс ровно один раз и применяет эффект. Списание хода контролирует Manager, не этот класс.

## 3. Методы и функции

- `CanUseItem(BattleUnit user,BattleUnit target,BattleConsumable item)` → bool: живые, подходящая Self/Opponent цель, healing≥0, Inventory и Count>0.
- `TryUseItem(...,out int amount)` → bool успеха; TryConsume, Health.Heal; amount — фактическое лечение.
- `CanCast(BattleUnit caster,BattleUnit target,BattleSpell spell)` → bool: живые, mpCost≥0, доступные MP, цель и поддерживаемый эффект.
- `TryCast(BattleUnit caster,BattleUnit target,BattleSpell spell,BattleRules rules,out int amount)` → bool: CanCast+TrySpendMP, затем Heal или MagicalDamage/TakeDamage; amount — фактическое лечение либо расчётный урон. Unity callbacks нет.

## 4. Переменные и параметры Inspector

Inspector отсутствует; аргументы — ссылки действующих участников/данных, amount начинается с 0. rules обязателен для TryCast.

## 5. Связи с другими системами

Manager дополнительно проверяет очередь и принадлежность заклинания Spells. Health хранит HP, Inventory хранит количество, Calculator считает магию.

## 6. Пример работы

Fire при MP7 → false, HP/MP не меняются. Heal при HP95 → amount5, цена 6.

## 7. Настройка в Unity

Класс не прикрепляется к GameObject и не имеет Inspector. Его вызывает BattleManager/BattleAbilities с данными BattleUnit/Rules. Назначь существующие Rules на BattleTransition; параметры передаются методам, а не через отдельный компонент.

## 8. Возможные проблемы и проверки

Самостоятельный прямой вызов не проверяет фазу боя/изученность заклинания: обращаться через Manager. Полное HP не запрещает расход. Нет воскрешения/других эффектов. Проверки ресурсов и целей покрыты Edit/Play Mode.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-11 — изменения этапа 2 сохранены в локальном Git после повторной проверки: 40 тестов Edit Mode и 17 тестов Play Mode прошли. Поведение этого файла описано выше; в задаче главного промпта его код повторно не переписывался.

# BattleAbilities

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleAbilities.cs`.

Общие проверки и применение магии и расходуемых предметов. Статический класс не прикрепляется к GameObject и не хранит состояние: он работает с `BattleUnit`, `BattleConsumable`, `BattleSpell`, `BattleRules` и `BattleCalculator`.

## 2. Как работает

Для предмета сначала проверяется контекст: предмет является расходуемым, разрешён в текущем месте (бой или исследование), имеет ровно один эффект — лечение HP **или** восстановление MP, цель соответствует пользователю и в инвентаре есть хотя бы одна копия. Только после всех проверок `TryUseItem` списывает одну единицу и применяет эффект.

Для заклинания проверяется, что кастер и цель живы, MP достаточно, цель корректна, а эффект — `MagicDamage`. `TryCast` дополнительно требует `BattleRules`, списывает MP и наносит магический урон через `BattleCalculator`.

## 3. Методы и функции

- `CanUseItem(BattleUnit user, BattleUnit target, BattleConsumable item, ItemUseLocation location = Battle)` → `bool`; проверяет категорию, маску `useLocations`, неотрицательные эффекты, ровно один ненулевой эффект, цель `Self`/`Opponent` и наличие предмета.
- `TryUseItem(...)` → `bool`; при успешной проверке вызывает `Inventory.TryConsume`, затем `RestoreMP` или `Health.Heal`; через `out int amount` возвращает фактическое восстановление.
- `CanCast(BattleUnit caster, BattleUnit target, BattleSpell spell)` → `bool`; проверяет живых участников, стоимость MP, цель и эффект `MagicDamage`.
- `TryCast(BattleUnit caster, BattleUnit target, BattleSpell spell, BattleRules rules, out int amount)` → `bool`; проверяет правила и `CanCast`, списывает MP, считает и применяет урон.

## 4. Переменные и Inspector

Собственных полей и Inspector-параметров нет. Значения приходят из `BattleConsumable` (`healing`, `manaRestoration`, `target`, `useLocations`), `BattleSpell` (`mpCost`, `target`, `effect`, `power`) и `BattleUnit`.

## 5. Связи

Используется `BattleManager`, `WorldItemUseController` и тестами. `BattleConsumable` наследует `ItemData`; `BattleSpell` — ScriptableObject; `BattleCalculator` считает магический урон; `Health` и `BattleUnit` меняют HP/MP; `BattleInventory` хранит количество.

## 6. Пример

Зелье лечения с `healing = 30`, `target = Self`, `useLocations = Anywhere`. В мире `CanUseItem(..., Exploration)` возвращает true, если HP не ноль и зелье есть. `TryUseItem` списывает одну единицу и возвращает фактически восстановленное HP (например, 12, если до максимума не хватало 12). Fire с ценой 8 MP наносит урон только при достаточном MP.

## 7. Настройка в Unity

Компонент не прикрепляется. Настрой ассеты `BattleConsumable` и `BattleSpell` в Inspector. У расходуемого предмета должен быть заполнен только один из эффектов; иначе использование будет отклонено. Список заклинаний задаётся в `BattleUnit.spells`.

## 8. Возможные проблемы

Предмет с одновременно заполненными `healing` и `manaRestoration` не используется. Предмет без эффекта тоже отклоняется. Проверка количества и последующее списание не атомарны на уровне многопоточности, но в Unity-бою вызываются в одном потоке. Лечения в боевом меню нет: зелья применяются через инвентарь вне боя. Новый эффект заклинания потребует расширения enum и `TryCast`.

## 9. История изменений

- 2026-10-31 — удалены лечебные заклинания и предметы с двумя эффектами; магия ограничена `MagicDamage`, предметы — одним эффектом HP или MP.
- 2026-10-12 — базовая версия способностей.

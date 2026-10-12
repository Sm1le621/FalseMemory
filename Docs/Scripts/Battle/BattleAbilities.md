# BattleAbilities

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleAbilities.cs.

Общие статические эффекты предметов и магии. Один расход из InventoryState/MP, один владелец ресурса.

## 2. Как работает

Актуально Player rework4: CanCast и TryCast используют одну эффективную стоимость BattleUnit.SpellCost; MCR снижает стоимость, MAG влияет на MagicAttack и SpellHealing. Первичная проверка и фактическое списание читают одни данные. Предметное лечение не менялось.

CanUse/CanCast проверяют условия до расхода. TryUseItem удаляет1 и применяет HP либо MP. TryCast списывает стоимость и лечит/наносит магический урон, читая итоговые характеристики. Возвращается фактический эффект.

## 3. Методы и функции

CanCast проверяет spell.mpCost≥0 и достаточный CurrentMP для caster.SpellCost(spell). TryCast повторно проверяет и списывает эту же цену, healing передаёт caster.SpellHealing(spell) в прежний Health.Heal; damage использует прежний BattleCalculator.MagicalDamage с новым EffectiveMagicAttack. Сигнатуры/returned amount и target/liveness guards сохранены.

`CanUseItem(user,target,item,location=Battle)` bool: живые объекты, Consumable, требуемый уровень, разрешённый Exploration/Battle, один положительный неотрицательный HP/MP-эффект, правильная цель и количество. `TryUseItem(...,out amount,location=Battle)` bool: повторно проверяет/удаляет1; вызывает RestoreMP либо Health.Heal, amount фактический. `CanCast(caster,target,spell)` bool: прежние MP/цель/эффект MagicDamage/RestoreHealth. `TryCast(...,rules,out amount)` bool: прежний расход MP; Heal через Health, урон через Calculator с EffectiveMagicAttack/Defense. Unity callbacks нет, класс static.

## 4. Переменные и Inspector

Статических запасов нет. Аргументы ссылаются на настоящие участники/данные. location optional Battle сохраняет прежние вызовы. amount0 при отказе и при полном ресурсе. Смена сцены не копирует значения.

## 5. Связи

BattleUnit primary profile/SpellCost/SpellHealing — новые зависимости, без второго MP. Без primary profile цена/лечение прежние.

BattleInventory.State, Health.Heal/TakeDamage, BattleUnit.RestoreMP/TrySpendMP/Effective*, Calculator. UI/аудио/VFX не вызывают расчёт второй раз.

## 6. Пример

Ether20MP при текущем25/30 даёт5, удаляет1. Staff MATK+4 повышает Fire19→23 без изменения8 MP цены.

## 7. Настройка в Unity

К GameObject не прикрепляется. Вызывается Manager для Battle и WorldItemUseController с Exploration. ItemData.useLocations/requiredLevel настраивают ограничения, смешанные HPMP сейчас запрещены.

## 8. Ограничения и расширение

Магическая точность пока не имеет RNG: MCR precision только prepared score. Бесплатные заклинания0MP, платные минимум1; отрицательные стоимости отвергаются. MAG не усиливает Potion/Ether.

Не воскрешает. Полный HP/MP не запрещает расход положительного эффекта — сохранено правило Potion. Нулевой/отрицательный/гибридный эффект отвергается. Нет новых типов способности/классов/MP-стоимости предметов. Игровые события подписчиков не должны менять ресурс во время транзакции.

## 9. История изменений

- 2026-10-12 — Player rework4: primary attributes подключены к исходным ресурсам/бою/UI; прежние идентификаторы и игровая сессия сохранены.

2026-10-12 — 4.5: общий контекст использования, Ether, ограничения и итоговые характеристики.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-11 — изменения этапа 2 сохранены в локальном Git после повторной проверки: 40 тестов Edit Mode и 17 тестов Play Mode прошли. Поведение этого файла описано выше; в задаче главного промпта его код повторно не переписывался.

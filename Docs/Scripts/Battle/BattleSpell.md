# BattleSpell

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleSpell.cs`.

ScriptableObject с описанием заклинания: имя, стоимость MP, эффект, цель, сила, цвет и визуальный профиль. Сейчас поддерживается только магический урон; лечебных заклинаний нет.

## 2. Как работает

Ассет создаётся через меню `FalseMemory/Battle Spell`. `BattleManager.OpenMagicMenu` читает `displayName` и `mpCost`, `BattleAbilities.CanCast` проверяет стоимость/цель/эффект, `TryCast` считает урон по `power` и `magicAttack/magicDefense`. `effectColor` и `visualEffect` используются только для отображения.

## 3. Методы и функции

Собственных методов нет. Поля читаются другими системами:

- `displayName` — подпись в меню и сообщениях.
- `mpCost` — стоимость, `≥ 0`.
- `effect` — `BattleEffect.MagicDamage`.
- `target` — `Opponent` или `Self`.
- `power` — базовая сила, `≥ 0`.
- `effectColor` — цвет всплывающего числа.
- `visualEffect` — необязательный `BattleVFXProfile`.

Перечисления `BattleEffect` и `BattleTarget` объявлены в этом же файле.

## 4. Переменные и Inspector

Все перечисленные поля редактируются в ассете. Значения по умолчанию: имя `Fire`, стоимость 8, эффект `MagicDamage`, цель `Opponent`, сила 10, оранжевый цвет. В текущем проекте используется `Assets/Settings/Battle/Spells/Fire.asset`; ассет `Heal` удалён.

## 5. Связи

`BattleUnit.spells` хранит ссылки на ассеты. `BattleAbilities` проверяет и применяет заклинание, `BattleCalculator.MagicalDamage` считает урон, `BattleUI` показывает имя/цену, `BattleVFXController` — визуал, `BattleAudioController` — звук Fire по действию.

## 6. Пример

Fire: 8 MP, сила 10. При MATK 12 и MDEF 3 расчёт даёт урон, который показывается над врагом; MP игрока уменьшается на 8. Если MP меньше 8, пункт меню недоступен.

## 7. Настройка в Unity

Создай ассет через `Assets → Create → FalseMemory → Battle Spell`, заполни поля и добавь ссылку в `BattleUnit.spells`. Для нового визуального эффекта назначь `BattleVFXProfile`. Не добавляй лечебный эффект без расширения enum и `BattleAbilities`.

## 8. Возможные проблемы

`null` в массиве заклинаний пропускается менеджером. Отрицательная стоимость или сила нежелательны, хотя Inspector ограничивает значения. `Self`-заклинание с уроном может навредить самому кастеру — проверяй цель. Ассет не хранит состояние: MP и урон меняются только у участников.

## 9. История изменений

- 2026-10-31 — удалён `RestoreHealth`; остался только `MagicDamage`, ассет Heal удалён.
- 2026-10-12 — базовая модель заклинаний.

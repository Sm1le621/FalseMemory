# Боевая система

## 1. Назначение

Пошаговый бой один на один между Player и Enemy: очередь по скорости, команды Attack/Magic/Defend/Flee, расчёт урона, магия, защита, побег и завершение встречи. Опыта, уровней и боевого меню Items нет.

## 2. Общая логика

`BattleEncounter` на враге запускает `BattleTransition.TryBegin`, который сохраняет исходные объекты мира и аддитивно загружает `BattleScene`. `BattleManager` получает тех же `Player`/`Enemy` и ведёт раунды. В начале хода снимается защита, затем игрок выбирает команду или враг решает через `EnemyBattleAI`. Перед применением действия проигрывается визуальный замах и повторно проверяется жизнь участников. Победа/поражение/побег завершают цикл; ресурсы не копируются.

## 3. Основные компоненты

- `BattleManager` — оркестратор хода.
- `BattleTransition` — вход/выход и сохранение мира.
- `BattleCalculator` — физический/магический урон, крит, промах, побег.
- `BattleAbilities` — проверка и применение магии/предметов.
- `BattleTurnOrder` + `BattleCommandGate` — очередь и защита от двойной команды.
- `BattleUnit`, `Health`, `BattleInventory`, `EquipmentController`.
- `BattleUI`, `BattleHUD`, `BattlePresentation`, `BattleVFXController`, `BattleAudioController`.
- `BattleRules` — общие коэффициенты и тайминги.

## 4. Взаимодействие скриптов

`BattleEncounter` → `BattleTransition.TryBegin` → `BattleScene`. `BattleManager` вызывает `BattleUI.ShowMenu`, `BattleCalculator`, `EnemyBattleAI` и `BattleAbilities`, а визуальные компоненты — только для отображения. `BattleUnit.EffectiveAttack/Defense` учитывают экипировку; HP остаётся в `Health`, MP — в `BattleUnit`. `BattleHUD` и `ManaHUD` читают те же значения.

## 5. Последовательность событий

1. Контакт с врагом → затемнение → аддитивная загрузка боя.
2. `ShowUnits`/`Bind` → очередь раунда по `Speed`.
3. Ход игрока: Attack/Magic/Defend/Flee; gate закрывается после первой команды.
4. Windup → проверка жизни → расход MP/расчёт урона → сообщение/эффект → Recover.
5. Ход врага: AI выбирает Attack или Defend.
6. Победа → результат → `ReturnAfterVictory`; поражение → New Game/Main Menu; побег → защита и возврат.

## 6. Настраиваемые параметры

`BattleRules`: множители атаки/защиты, шансы крита/промаха, защита, побег, задержки и переходы. `BattleUnit`: HP через `Health`, MP, ATK/DEF/SPD, MATK/MDEF, шансы. `BattleSpell`: цена MP, сила, цель, визуал. `BattleConsumable`: лечение HP или MP. Экипировка даёт только ATK/DEF.

## 7. Использование в Unity

Компоненты боя находятся в `BattleScene`; Player/Enemy — в `SampleScene`. Запуск — касанием скелета, не прямым Play боевой сцены. Для нового заклинания создай `BattleSpell` и добавь в `BattleUnit.spells`. Для нового врага — `BattleUnit`, `Health`, `BattleEncounter` и при необходимости `EnemySpriteAnimation`.

## 8. Расширение

Новый эффект заклинания требует расширения `BattleEffect` и `BattleAbilities.TryCast`. Новый предмет — `BattleConsumable` с одним эффектом. Новый враг использует те же компоненты; AI сейчас выбирает только Attack/Defend. Поддержка партии потребует изменения очереди, UI и выбора цели.

## 9. Ограничения и проверки

Только один игрок и один враг; нет статусов, воскрешения, классов, QTE и тайминговых ударов. Зелья применяются только через инвентарь вне боя. Нет дискового сохранения. EditMode 59/59 и PlayMode 36/36 пройдены 2026-10-31; Console без ошибок. Ручная оценка баланса и визуала остаётся за владельцем.

## История

- 2026-10-31 — удалены Items/Heal/EXP/уровни; команды сокращены до четырёх, магия только с уроном.
- 2026-10-11 — базовая боевая система.

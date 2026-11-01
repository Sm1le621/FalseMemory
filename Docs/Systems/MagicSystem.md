# Система магии и MP

## 1. Назначение

Заклинания с уроном и запас MP участника. Сейчас в игре одно боевое заклинание Fire; лечебных заклинаний и боевого меню предметов нет.

## 2. Общая логика

`BattleUnit.Awake` заполняет MP до максимума. В ход игрока `BattleManager.OpenMagicMenu` строит список заклинаний с ценой и доступностью. `BattleAbilities.CanCast` проверяет живых участников, цель, эффект и достаточность MP. `TryCast` списывает MP и наносит магический урон через `BattleCalculator`. Неуспешная проверка не тратит ход и MP.

## 3. Основные компоненты

- `BattleUnit` — `maxMP`, `currentMP`, `magicAttack`, `magicDefense`, `spells`.
- `BattleSpell` — ScriptableObject заклинания.
- `BattleAbilities` — проверка/применение.
- `BattleCalculator` — магический урон.
- `BattleManager`/`BattleUI` — меню и сообщения.
- `ManaHUD`, `BattleHUD`, `InventoryWindow` — отображение MP.

## 4. Взаимодействие скриптов

`BattleManager` проверяет принадлежность заклинания игроку и вызывает `BattleAbilities`. `BattleUnit.TrySpendMP` меняет MP, `Health.TakeDamage` — HP цели. `BattleVFXController` показывает визуал, `BattleAudioController` — звук. `ManaHUD` читает MP, но не тратит его.

## 5. Последовательность событий

Magic → список `Fire 8 MP` + Back → выбор → `CanCast` → закрытие gate → `TryCast` → списание 8 MP → расчёт урона → сообщение/число/VFX → ход врага. Escape/Back не тратит ход.

## 6. Настраиваемые параметры

Player: MP 30, MATK 12, MDEF 8; Enemy MDEF 3. Fire: 8 MP, power 10, цель Opponent, оранжевый цвет. `BattleRules` задаёт коэффициенты магии и минимальный урон. Формула Fire при текущих данных даёт 19 урона.

## 7. Использование в Unity

Ассеты заклинаний лежат в `Assets/Settings/Battle/Spells`; Fire назначен в `BattleUnit.spells` игрока. Новое заклинание создаётся через `Assets → Create → FalseMemory → Battle Spell` и добавляется в массив. Стартовый MP задаётся через `maxMP`.

## 8. Расширение

Новый эффект требует расширения `BattleEffect`, `BattleAbilities` и, при необходимости, `BattleCalculator`. Лечение через заклинания отсутствует; зелья восстанавливают HP/MP из инвентаря. Для MP-предмета используй `BattleConsumable.manaRestoration`.

## 9. Ограничения и проверки

Только цель Self/Opponent, нет выбора союзника, статусов, воскрешения и лечения заклинанием. Fire игнорирует физический Defend, крит и промах. MP не сохраняется на диск, но переживает переход в бой и обратно в рамках сессии. EditMode-тесты проверяют расход, нехватку MP, мёртвых участников и границы; PlayMode — меню, VFX и возврат с тем же MP.

## История

- 2026-10-31 — удалён Heal; магия ограничена `MagicDamage`.
- 2026-10-11 — базовая система магии.

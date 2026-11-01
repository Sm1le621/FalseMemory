# BattleAbilityTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/BattleAbilityTests.cs`.

Базовая fixture EditMode-тестов способностей: Fire, расход MP, мёртвые участники и цели. Класс `partial`, поэтому `BattleItemTests.cs` добавляет проверки предметов в ту же fixture.

## 2. Как работает

`SetUp` создаёт Player/Enemy с `BattleUnit`, HP 100/60, MP 30, MATK 12/MDEF 3, правила, Fire и Potion; Potion ×3 добавляется в `BattleInventory`. `TearDown` уничтожает объекты и временные ScriptableObject. Каждый тест работает только в памяти.

## 3. Методы и функции

- `Setup()` / `Cleanup()` — NUnit lifecycle.
- `FireCostsEightMPAndIgnoresPhysicalDefend()` — Fire стоит 8 MP, даёт 19 урона даже при Defend; HP врага 41, MP 22.
- `InsufficientMPRejectsFireWithoutChangingHealthOrMP()` — при 7 MP заклинание отклоняется, HP/MP не меняются.
- `DeadCasterOrTargetCannotCast()` — мёртвый кастер или цель отклоняются.
- `SelfTargetedDamageSpellCannotBeCastOnTheOpponent()` — Self-заклинание нельзя направить на врага; на себя — можно.
- `ManaNeverGoesNegativeOrAboveMaximum()` — отрицательная/избыточная стоимость и восстановление до максимума.

## 4. Переменные и Inspector

`playerObject`, `enemyObject`, `player`, `enemy`, `fire`, `rules`, `potion` — поля fixture. Inspector отсутствует; данные создаются в памяти.

## 5. Связи

NUnit, `BattleUnit`, `Health`, `BattleAbilities`, `BattleCalculator`, `BattleInventory`, `BattleSpell`, `BattleConsumable`. Partial-часть находится в `BattleItemTests.cs`.

## 6. Пример

Тест с MP 7: `FireCostsEightMP...` не запускается, а `InsufficientMP...` подтверждает, что `TryCast` вернул false и не списал последние 7 MP.

## 7. Настройка в Unity

Компонент не прикрепляется. Тесты запускаются через Test Runner → EditMode; отдельная сцена не нужна.

## 8. Ограничения и проверки

Проверяется модель, а не UI/анимация. Нет проверки Heal — лечебных заклинаний нет. Результаты 2026-10-31: EditMode 59/59, 0 failed.

## 9. История изменений

- 2026-10-31 — удалены проверки Heal и EXP; fixture приведена к Fire + Potion.
- 2026-10-12 — базовая fixture.

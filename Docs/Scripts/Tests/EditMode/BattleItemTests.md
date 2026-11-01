# BattleItemTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/BattleItemTests.cs`.

Partial-часть `BattleAbilityTests`: восстановление MP, ограничения мест использования, лечение Potion, невалидные эффекты и влияние экипировки на физическую атаку.

## 2. Как работает

Использует `SetUp`/`TearDown` из `BattleAbilityTests.cs`. Каждый тест меняет только временные HP/MP, количества и ScriptableObject; реальные ассеты и сцены не затрагиваются.

## 3. Методы и функции

- `ManaPotionRestoresActualManaWithoutHealingAndConsumesOne()` — MP-зелье возвращает 20, затем 5 (до максимума), HP не меняется, количество уменьшается на 1.
- `ItemLocationsRejectWithoutResourceChanges()` — `Exploration`/`Battle` маски соблюдаются; отказ не тратит предмет.
- `InvalidHybridAndNegativeEffectsNeverConsume()` — одновременно HP+MP, отрицательный или нулевой эффект отклоняются без расхода.
- `EquipmentAttackBonusRaisesPhysicalAttackButNotSpellPower()` — меч +5 ATK повышает `EffectiveAttack` до 20, но Fire по-прежнему наносит 19 и стоит 8 MP; база `attack` остаётся 15.
- `PotionHealsAndConsumesExactlyOneItem(initial, expected, restored)` — HP 40/95/100: лечение, фактическое значение, Potion 3→2.
- `EmptyPotionStackCannotHealOrBecomeNegative()` — пустая стопка не лечит и не уходит в минус.
- `PotionCannotReviveDeadUserOrTarget()` — мёртвого нельзя вылечить, предмет остаётся.
- `DuplicateStacksConsumeOnlyOneAndNegativeStacksAreUnavailable()` — отрицательная стопка игнорируется, из положительных списывается одна единица.

## 4. Переменные и Inspector

Собственных полей нет: `player`, `enemy`, `potion`, `fire`, `rules` берутся из partial fixture. Inspector отсутствует.

## 5. Связи

`BattleAbilityTests`, `BattleAbilities`, `BattleInventory`, `Health`, `BattleUnit`, `EquipmentController`, `ItemData`, NUnit.

## 6. Пример

Potion с `healing = 30` при HP 95/100 возвращает 5, HP становится 100, количество 3→2. При `manaRestoration = 20` и MP 5/30 возвращает 20, MP становится 25.

## 7. Настройка в Unity

Компонент не прикрепляется; запускается через Test Runner → EditMode вместе с `BattleAbilityTests`.

## 8. Ограничения и проверки

Проверяется модель и `BattleAbilities`, не окно инвентаря. Гибридные зелья намеренно запрещены. Нет тестов редкости/уровня — этих механик больше нет. 2026-10-31: все EditMode-тесты прошли.

## 9. История изменений

- 2026-10-31 — Ether заменён на MP-зелье, удалены проверки уровня и редкости; добавлено влияние меча на ATK.
- 2026-10-12 — базовая часть тестов предметов.

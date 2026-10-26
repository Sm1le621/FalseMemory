# ClasslessBalanceTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/ClasslessBalanceTests.cs`. Повторяемое сравнение 6 равнобюджетных сборок и широкая матрица Scaling/брони.

## 2. Как работает

Повторяемое сравнение 6 равнобюджетных сборок и широкая матрица Scaling/брони. Использует текущие правила и состояние, не создаёт вторую систему персонажа.

## 3. Методы и функции

Builds() задаёт 6 наборов ровно+60 к исходным 10×10: STR,DEX,MAG,WIS,STR_MAG,END_VIT. Foes() задаёт 5 чистых fixture-профилей, включая Undead/Unholy/Human/Beast и отдельный Zombie fixture; это не создание игровых врагов. Load<T> читает существующий ассет с AssertNotNull. Calculate получает stat block из действующих правил/оружия/брони, проверяет requirements; baseline15ATK/12MATK/8DEF/8MDEF/10initiative/100HP/40MP явно фиксирован для сценария. Hit использует настоящие BattleCalculator/BattleDamage и текущие ability данные. В отличие от обычного Play он не вызывает BattleManager/анимации.

EqualBudgetBuildsProduceReproducibleCombatAndResourceReport проверяет budget, проводит 30 пар×64 seeds, учитывает инициативу/crit/miss/evasion/Defend врага,MP/Heal/предпочтённую технику и fallback на Basic, раздельную magic/physical защиту и Holy. Проверяет ресурсные границы/различие специализаций, экспортирует JSON в ignoredTemp. Результат не является проверкой идеального равенства winrate. EveryArmorSupportsIndependentRemovalAndEveryGradeIsFiniteAtAllCaps проверяет 6 реальных armors/снятие/повтор ивсе grades×10stats×cap−1/cap/cap+1/99/120. NUnit методывозвращают void; Unitycallbacks нет.

## 4. Переменные и Inspector

Build/Foe/Stats — внутренние fixture данные; Metric/Report — сериализуемый результат. Samples64, seeds20261026+i, предел 100rounds. Враг 20%Defend; долямагии 0…65%по fixture. Среднийуроннаатакуотделёнотлечения; meanRounds включаетпоражения,meanWinRounds —толькопобеды. Нет Inspector/изменения игровыхассетов.

## 5. Связи

BattleUnit/CharacterAttributes/AbilityLearning, ItemData/BattleSpell/ArmorScalingData, CharacterAttributeRules/BattleRules/BattleCalculator/BattleDamage. Для тестов NUnit/UnityTest, для меню UnityEditor. Данные кампании/сохранений не создаются.

## 6. Пример

DEX с 180HP/40MP и QuickStrike сравнивается с STR100HP/40MP наодинаковых 60points. Магические fixture показываютслабость HeavyPlate к MagicDefense.

## 7. Настройка в Unity

Не прикреплять. Test Runner EditMode →FalseMemory.Battle.EditModeTests. Выход Temp/ClasslessBalance/Stage57.json; сохранённый срез —Docs/Verification/ClasslessStage57.json. Для повторенияиспользуется таже версияассетов. Этоформульнаясимуляция,неручнаякампания.

## 8. Возможные проблемы

Симуляции зависят от стратегии/fixture-профиля и не заменяют ручную оценку. Тестовое меню работает только в Editor/Play и не сохраняет прогресс. Метрики и ограничения — [ClasslessStage57](../../../ClasslessStage57.md). Этап 6 закрыт до приёмки владельца.

## 9. История изменений

- 2026-10-26 — реализация проверок/инструмента ручной приёмки подэтапа 5.7.

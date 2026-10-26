# ClasslessRegressionFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/ClasslessRegressionFlowTests.cs`. Дополнительная реальная регрессия магического оружия, Holy и Character.

## 2. Как работает

Дополнительная реальная регрессия магического оружия, Holy и Character. Использует текущие правила и состояние, не создаёт вторую систему персонажа.

## 3. Методы и функции

MagicStaffBasicAttackUsesMagicDefenseAndWisdomHolyAtOneContact экипирует настоящий MagicStaff при MAG20INT20WIS30, запускает BattleManager: basicpower30,после MagicDefense3base27+Holy3,Enemy60→30. SacredWeaponScalingResistanceAndCategoriesDoNotDuplicateHolyOrAlterWalking экипирует SacredBladeSTR20WIS30,проверяет base42,intrinsic9,50%HolyResistance→Holy7,total49,Beast без Holy,AGI99 безускорения. Дополнительно проверяет CharacterWeapontextAdditionalHoly5+4=9/отсутствие TMPoverflow иживоеобновление tooltip при temporaryWIS+1. Оба UnityTest возвращают IEnumerator,используютобщий fixture; временные HP999 не сохраняются.

## 4. Переменные и Inspector

Настоящие pickups/ItemData/BattleRules читаютсяиз SampleScene. Ссылки на Manager/Player —sharedhelpers. Тестовыебазовые/временныеатрибуты/категории/здоровье принадлежаттекущей Play-сессии,не Inspector профилю.

## 5. Связи

BattleUnit/CharacterAttributes/AbilityLearning, ItemData/BattleSpell/ArmorScalingData, CharacterAttributeRules/BattleRules/BattleCalculator/BattleDamage. Для тестов NUnit/UnityTest, для меню UnityEditor. Данные кампании/сохранений не создаются.

## 6. Пример

SacredBlade:42normal+(4.2WIS+9intrinsic)×50%=49total. Один DealDamage уменьшает 999→950,Beast получает 42 без Holy.

## 7. Настройка в Unity

Не прикреплять. Test Runner PlayMode →FalseMemory.Battle.PlayModeTests. Требуются сохранённые сцены/ассеты 5.3–5.6.

## 8. Возможные проблемы

Симуляции зависят от стратегии/fixture-профиля и не заменяют ручную оценку. Тестовое меню работает только в Editor/Play и не сохраняет прогресс. Метрики и ограничения — [ClasslessStage57](../../../ClasslessStage57.md). Этап 6 закрыт до приёмки владельца.

## 9. История изменений

- 2026-10-26 — реализация проверок/инструмента ручной приёмки подэтапа 5.7.

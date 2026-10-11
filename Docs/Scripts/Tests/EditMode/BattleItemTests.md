# BattleItemTests

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/BattleItemTests.cs.

Часть partial BattleAbilityTests для лечения и расхода Potion.

## 2. Как работает

Использует Setup/Cleanup из BattleAbilityTests.cs; меняет только временные стопки и HP.

## 3. Методы и функции

- `PotionHealsAndConsumesExactlyOneItem(int initial,int expected,int restored)` — три набора HP40/95/100: лечение, фактическое количество, Potion3→2 и отсутствие расхода MP.
- `EmptyPotionStackCannotHealOrBecomeNegative()` — Count0 не лечит и не отрицательный.
- `PotionCannotReviveDeadUserOrTarget()` — мёртвый не воскресает, Potion остаётся 3.
- `DuplicateStacksConsumeOnlyOneAndNegativeStacksAreUnavailable()` — отрицательная стопка игнорируется, одна положительная уменьшается на 1. Все void с Assert; своих Unity callbacks нет.

## 4. Переменные и параметры Inspector

Собственных Inspector/полей нет; player/potion и остальные поля из partial fixture.

## 5. Связи с другими системами

BattleAbilityTests, BattleAbilities, BattleInventory, Health, NUnit.

## 6. Пример работы

HP95 +Potion30 →100, amount5, количество 2.

## 7. Настройка в Unity

Это тестовый файл, не компонент сцены: не прикрепляй его к GameObject. Открой Window → General → Test Runner, выбери EditMode и нужную сборку/тест либо Run All. Ссылки и временные объекты создаёт fixture. Для PlayMode должны быть доступны существующие SampleScene, BattleScene и MainMenu; тест не требует ручного изменения их Inspector.

## 8. Возможные проблемы и проверки

Имя файла отличается от имени partial-класса; Test Runner показывает BattleAbilityTests. Это проверка данных, меню отдельно в PlayMode. Все случаи входят в прошедшие 40 Edit Mode.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

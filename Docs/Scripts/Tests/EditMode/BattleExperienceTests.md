# BattleExperienceTests

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/BattleExperienceTests.cs.

Часть BattleAbilityTests для накопления EXP и защиты от переполнения.

## 2. Как работает

Использует общую временную fixture; проверяет BattleUnit.AddExperience без настоящей сцены.

## 3. Методы и функции

- `ExperienceAccumulatesWithoutOverflowOrImplicitLevelUp(int initial,int reward,int expected,int added)` — три TestCase0/15/почти int.MaxValue; сверяет итог, фактически добавленное и level1.
- `NegativeRewardOrDeadPlayerCannotGainExperience()` — отрицательная награда/мёртвый участник дают 0. void с Assert; Setup/Cleanup в BattleAbilityTests.cs, Unity callbacks нет.

## 4. Переменные и параметры Inspector

Собственных полей Inspector нет; initial/reward/expected/added — тестовые числа.

## 5. Связи с другими системами

BattleUnit и общая fixture. Однократность награды встречи проверяется отдельным Play Mode тестом Transition.

## 6. Пример работы

EXP15+20 →35 без повышения level.

## 7. Настройка в Unity

Это тестовый файл, не компонент сцены: не прикрепляй его к GameObject. Открой Window → General → Test Runner, выбери EditMode и нужную сборку/тест либо Run All. Ссылки и временные объекты создаёт fixture. Для PlayMode должны быть доступны существующие SampleScene, BattleScene и MainMenu; тест не требует ручного изменения их Inspector.

## 8. Возможные проблемы и проверки

Не доказывает переход между сценами или однократность сама по себе. Все четыре случая входят в прошедшие 40 Edit Mode.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

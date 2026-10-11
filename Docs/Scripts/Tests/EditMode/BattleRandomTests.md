# BattleRandomTests

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/BattleRandomTests.cs.

Проверка крита, промаха, границ вероятности и повторяемого источника.

## 2. Как работает

Тесты создают правила в try/finally и фиксированный источник, затем сравнивают результаты. Общих lifecycle callbacks нет.

## 3. Методы и функции

- Вложенный `FixedRandom.Next01()` возвращает sample и увеличивает calls.
- `CriticalMultipliesDamageAndDefendAppliesAfterIt()` —10 обычный,15 крит,8 крит с Defend.
- `MissDoesZeroDamageAndNeverRollsCritical()` —0 урона, Missed=true, Critical=false и только 1 запрос random.
- `ChanceBoundsIncludeGuaranteedSuccessAndFailure(float chance,float sample,bool expected)` — четыре крайних TestCase, включая chance1/sample1.
- `SameSeedProducesSameStream()` — сравнивает 20 значений seed42. Тесты void, Assert; временные правила DestroyImmediate.

## 4. Переменные и параметры Inspector

Inspector нет. FixedRandom.sample — заданная выборка, calls — счётчик; seed42 используется только тестом.

## 5. Связи с другими системами

IBattleRandom, SeededBattleRandom, BattleCalculator и Rules, NUnit.

## 6. Пример работы

Принудительный промах → нет второго броска на крит.

## 7. Настройка в Unity

Это тестовый файл, не компонент сцены: не прикрепляй его к GameObject. Открой Window → General → Test Runner, выбери EditMode и нужную сборку/тест либо Run All. Ссылки и временные объекты создаёт fixture. Для PlayMode должны быть доступны существующие SampleScene, BattleScene и MainMenu; тест не требует ручного изменения их Inspector.

## 8. Возможные проблемы и проверки

Фиксированная выборка проверяет ветки, не статистическое распределение за тысячи боёв. Семь случаев входят в 40 прошедших Edit Mode.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

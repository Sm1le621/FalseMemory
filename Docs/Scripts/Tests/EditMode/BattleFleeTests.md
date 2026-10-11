# BattleFleeTests

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/BattleFleeTests.cs.

Проверка формулы и границ вероятности побега.

## 2. Как работает

Создаёт временные правила, вычисляет FleeChance, сравнивает результат и уничтожает правила в finally.

## 3. Методы и функции

- `SpeedAffectsEscapeWithReasonableLimits(int playerSpeed,int enemySpeed,float expected)` — четыре TestCase:74%,65%,5%,95%, допуск 0.0001.
- `InvalidConfiguredBoundsCannotProduceProbabilitiesOutsideZeroToOne()` — некорректные границы/база не выходят за 0…1. Методы void/Assert, Unity callbacks нет.

## 4. Переменные и параметры Inspector

Inspector нет; playerSpeed/enemySpeed/expected задаются атрибутами; правила меняются только в памяти.

## 5. Связи с другими системами

Calculator/Rules/NUnit. Возврат после Flee покрывается PlayMode отдельно.

## 6. Пример работы

Speed10 против 7 →0.74.

## 7. Возможные проблемы и проверки

Расчёт вероятности не проверяет свободное место, переход или AI. Пять случаев включены в успешные 40 Edit Mode.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

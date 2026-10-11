# BattleRandom

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleRandom.cs.

Управляемый источник случайности и неизменяемый результат попадания.

## 2. Как работает

В игре UnityBattleRandom читает UnityEngine.Random.value. SeededBattleRandom хранит отдельный System.Random; тесты могут подставить свой IBattleRandom. PhysicalAttackResult передаёт результат менеджеру.

## 3. Методы и функции

- `IBattleRandom.Next01()` — контракт float 0…1; реализации вызываются при промахе, крите, AI и побеге.
- `UnityBattleRandom.Next01()` → Random.value; Instance — общий экземпляр.
- `SeededBattleRandom(int seed)` — создаёт локальный генератор. `Next01()` → float из NextDouble, диапазон [0,1).
- `PhysicalAttackResult(int damage,bool missed,bool critical)` — конструктор; сохраняет Damage/Missed/Critical, свойства только читаемые. Unity callbacks нет.

## 4. Переменные и параметры Inspector

Inspector отсутствует. seed — целочисленная начальная настройка, одинаковые seed дают одинаковую последовательность в проверенной среде. Damage — int; Missed/Critical — bool.

## 5. Связи с другими системами

BattleCalculator и EnemyBattleAI получают IBattleRandom; BattleManager.SetRandomSource меняет источник конкретного боя.

## 6. Пример работы

Два SeededBattleRandom(42) дают совпадающие первые 20 значений в тесте; глобальный Random Unity не меняется.

## 7. Возможные проблемы и проверки

Порядок обращений к источнику влияет на последовательность; seed не гарантирует одинаковый бой при изменённом коде. Конструктор результата не запрещает вручную передать оба флага true, но калькулятор так не делает. Повторяемость и исключение крита после промаха проверены.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

# EnemyBattleAI

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/EnemyBattleAI.cs.

Выбор Attack или Defend врагом исключительно в назначенный ему ход.

## 2. Как работает

Менеджер ждёт короткую паузу, проверяет живых участников и вызывает Choose. AI сравнивает случайную выборку с enemyDefendChance.

## 3. Методы и функции

- `Choose(BattleRules rules)` → BattleAction; использует UnityBattleRandom.Instance.
- `Choose(BattleRules rules,IBattleRandom random)` → Defend при успешной ChanceSucceeds, иначе Attack. Unity callbacks нет.

## 4. Переменные и параметры Inspector

Inspector отсутствует. enemyDefendChance задаётся в BattleRules, диапазон 0…1; rules/random должны существовать.

## 5. Связи с другими системами

BattleManager вызывает, BattleCalculator сравнивает шанс. Это не EnemyAI преследования мира.

## 6. Пример работы

Выборка 0.1 при шансе 0.2 → Defend; выборка 0.5 → Attack.

## 7. Возможные проблемы и проверки

Нет магии, предметов или тактики врага. Null ссылки не обрабатываются здесь. Физические ходы проверены Play Mode; тактическое качество не оценено.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

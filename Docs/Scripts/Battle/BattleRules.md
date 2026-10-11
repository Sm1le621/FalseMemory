# BattleRules

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleRules.cs.

ScriptableObject общих коэффициентов боя; позволяет менять баланс без переписывания менеджера.

## 2. Как работает

Сам действий не выполняет; BattleTransition передаёт ссылку менеджеру, калькулятор и AI читают настройки при действиях.

## 3. Методы и функции

Методов и Unity callbacks нет. CreateAssetMenu FalseMemory/Battle Rules создаёт ассет данных; расчёты находятся в BattleCalculator.

## 4. Переменные и параметры Inspector

attackMultiplier/defenseMultiplier≥0, код 1/1; minimumDamage≥1, код 1; defendMultiplier 0…1, код 0.5; enemyDefendChance 0…1, код 0.2. actionDelay≥0.1 секунды, код 0.8; fadeDuration≥0, код 0.3. magicAttackMultiplier/magicDefenseMultiplier≥0, код 1/1; minimumMagicDamage≥1, код 1. criticalMultiplier≥1, код 1.5. fleeBaseChance 0…1, код 0.65; fleeSpeedCoefficient≥0, код 0.03; minimumFleeChance/maximumFleeChance 0…1, код 0.05/0.95 (минимум≤максимум); fleeProtectionSeconds/fleeSafeDistance≥0.1, код 3/2.

## 5. Связи с другими системами

Assets/Settings/Battle/DefaultBattleRules.asset назначен BattleTransition.rules. BattleManager, EnemyBattleAI и BattleCalculator используют общую ссылку.

## 6. Пример работы

criticalMultiplier=1.5 → обычные 10 HP превращаются в критические 15; enemyDefendChance=0.2 → AI выбирает Defend с вероятностью 20%.

## 7. Возможные проблемы и проверки

Изменение общего ассета влияет на все бои с этой ссылкой; Inspector значения Play могут менять ассет. Тесты временно меняют задержки и восстанавливают их. Не путать chance 0.65 и 65. Коэффициенты и границы проверены Edit Mode.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

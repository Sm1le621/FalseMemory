# BattleCalculator

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleCalculator.cs.

Чистые расчёты физического/магического урона и вероятностей; сам HP не меняет.

## 2. Как работает

При физическом действии проверяет промах, затем крит; вычисляет урон после Defense, умножает на крит и Defend, округляет вверх. Магия рассчитывается отдельно.

## 3. Методы и функции

- `FleeChance(int playerSpeed,int enemySpeed,BattleRules rules)` → float 0…1: базовый шанс+разница Speed×коэффициент, ограниченный настроенными границами.
- `ChanceSucceeds(float chance,float sample)` → bool; ограничивает 0…1, chance=1 всегда успешен, иначе sample<chance.
- `ResolvePhysical(int attack,int defense,bool defending,BattleRules rules,float criticalChance,float missChance,IBattleRandom random)` → PhysicalAttackResult; после промаха не делает второй случайный запрос.
- `MagicalDamage(int attack,int defense,int power,BattleRules rules)` → int: power+MATK×коэффициент−MDEF×коэффициент, минимум и потолок int.MaxValue.
- `PhysicalDamage(int attack,int defense,bool defending,BattleRules rules)` → int обычного попадания; приватная перегрузка также получает criticalMultiplier. База=max(minimumDamage,ATK×множитель−DEF×множитель), затем крит/защита; округление вверх, минимум 1. Unity callbacks нет.

## 4. Переменные и параметры Inspector

Нет Inspector/компонента. Параметры передаются методам; правила не null. attack/defense/power/speeds используются как ≥0; defending bool; шансы 0…1.

## 5. Связи с другими системами

Вызывают BattleManager, BattleAbilities, EnemyBattleAI. Результат применяет Health.TakeDamage вызывающий код, источник случайности — IBattleRandom.

## 6. Пример работы

ATK15 против DEF5 → 10; крит ×1.5 →15; крит при Defend×0.5 →ceil(7.5)=8. Fire power10+MATK12−MDEF3=19.

## 7. Настройка в Unity

Класс не прикрепляется к GameObject и не имеет Inspector. Его вызывает BattleManager/BattleAbilities с данными BattleUnit/Rules. Назначь существующие Rules на BattleTransition; параметры передаются методам, а не через отдельный компонент.

## 8. Возможные проблемы и проверки

rules/random null вызывают ArgumentNullException в расчётах, поэтому ссылки обязательны. MISS имеет 0 урона, успешное попадание минимум 1. Числа Fire показывают расчётный урон, который может превышать оставшиеся HP цели. Формулы/seed/границы проверены тестами.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

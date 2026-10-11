# BattleAbilityTests

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/EditMode/BattleAbilityTests.cs.

Общая fixture и Edit Mode проверки Fire, Heal, MP и допустимых целей.

## 2. Как работает

Setup создаёт временных участников, Health, правила, Fire/Heal/Potion и инвентарь; Cleanup уничтожает их. Partial-класс объединяется с BattleItemTests/BattleExperienceTests.

## 3. Методы и функции

- `Setup()` [SetUp] — HP100/60, MP30, MATK12/MDEF3, данные эффектов; void.
- `Cleanup()` [TearDown] — DestroyImmediate созданных объектов/ассетов; void.
- `FireCostsEightMPAndIgnoresPhysicalDefend()` — цена 8, урон 19 независимо от Defend.
- `InsufficientMPRejectsFireWithoutChangingHealthOrMP()` — MP7 отклоняет Fire.
- `HealCapsHealthAndReturnsActualAmount(int initial,int expected,int restored)` — три TestCase40/95/100 HP, проверяет итог, фактическое лечение и MP24.
- `DeadCasterOrTargetAndIncorrectTargetsCannotCast()` — мёртвые/неверная цель отклонены.
- `ManaNeverGoesNegativeOrAboveMaximum()` — отрицательная цена/недостаток/большое восстановление. Проверки void с Assert; Unity callbacks нет.

## 4. Переменные и параметры Inspector

Inspector нет. playerObject/enemyObject/player/enemy/fire/heal/rules/potion — поля тестовой fixture; данные только в памяти.

## 5. Связи с другими системами

NUnit, BattleUnit, Health, BattleAbilities, Inventory, SO; partial-класс находится ещё в двух отдельных исходных файлах.

## 6. Пример работы

Временный Player30 MP → Fire →22 MP и Enemy41 HP.

## 7. Настройка в Unity

Это тестовый файл, не компонент сцены: не прикрепляй его к GameObject. Открой Window → General → Test Runner, выбери EditMode и нужную сборку/тест либо Run All. Ссылки и временные объекты создаёт fixture. Для PlayMode должны быть доступны существующие SampleScene, BattleScene и MainMenu; тест не требует ручного изменения их Inspector.

## 8. Возможные проблемы и проверки

Это не готовые ассеты сцены и не проверка выбора меню. Вместе с item/experience partial-файлами использует общую Setup/Cleanup. Проверки включены в успешные 40 Edit Mode.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

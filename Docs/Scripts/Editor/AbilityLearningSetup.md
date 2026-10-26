# AbilityLearningSetup

## Уточнение 5.7 —2026-10-26

Баланс 5.7 после замеров: QuickStrike mpCost4/attackMultiplier1.6 вместо 5/1.15. Изменены только собственный прототип и параметры его повторного создания. Исторические параметры 5.5 ниже описывают прежний срез.

## 1. Название и назначение

Исходник: `Assets/Editor/AbilityLearningSetup.cs`. Адресное подключение данных5.5 в существующей SampleScene через MCP.

## 2. Как работает

Адресное подключение данных5.5 в существующей SampleScene через MCP. Расширяет существующие BattleSpell/BattleAbilities, классовой системы нет.

## 3. Методы и функции

ApplyStage55 требует остановленный Play и SampleScene, добавляет/настраивает AbilityLearning только Player, переиспользует unit.spells Fire/Heal, создаёт4 ассета. Make проверяет стабильный ID перед обновлением собственного пути, назначает требования/стоимость/тип/таргет/множитель. Add обновляет English StringTable. Сохраняет только изменённые ассеты и текущую сцену, не SaveAssets. Возвратvoid, Unity callbacks нет.

## 4. Переменные и Inspector

Массив catalog содержит6 умений. Цены MP: Power6, Quick5, Greater12, Sacred8. learningCost1, requiredLevel1. GreaterHeal использует прежний Heal VFX, новый pixel art не создаётся.

## 5. Связи

BattleUnit предоставляет уровень, эффективные атрибуты и MP; AbilityLearning хранит только LP/known IDs. BattleSpell содержит данные, BattleAbilities выполняет эффект, BattleManager строит меню, GameText локализует. Для tests используются NUnit/Unity Test Framework, для Editor — AssetDatabase и EditorSceneManager.

## 6. Пример

После ApplyStage55 Player знает прежние2 заклинания, а4 новые способности доступны для просмотра и изучения при выполнении требований.

## 7. Настройка в Unity

Статический Editor-класс не прикрепляется к объекту. MCP execute_code → AbilityLearningSetup.ApplyStage55(). Нельзя запускать в Play или на другой сцене. Повторный вызов обновляет только собственные4 профиля и описание Fire/Heal.

## 8. Возможные проблемы

ID должен быть непустым и стабильным. Не расходовать attribute points на обучение. Уже известные Fire/Heal сохраняются; требования применяются при новом изучении. LP3+1/level — тестовая экономика без учителя/NPC. Постоянного save нет. Проверки — [ClasslessStage55](../../ClasslessStage55.md).

## 9. История изменений

- 2026-10-26 — реализация и первичная документация подэтапа5.5.

- 2026-10-26 — баланс/регрессия 5.7; актуальное уточнение приведено выше.

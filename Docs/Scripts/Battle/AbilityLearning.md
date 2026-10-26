# AbilityLearning

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/AbilityLearning.cs`. Состояние известных способностей и независимой валюты Learning Points на игроке.

## 2. Как работает

Состояние известных способностей и независимой валюты Learning Points на игроке. Расширяет существующие BattleSpell/BattleAbilities, классовой системы нет.

## 3. Методы и функции

Knows(ability) проверяет совместимый unit.spells или стабильный learnedIds. KnownAbilities объединяет старые spells и изученные из catalog, исключает null/дубликаты ссылки. Availability возвращает Learned/RequirementsNotMet/NotEnoughPoints/Available, проверяет принадлежность каталогу, ID, уровень/эффективные характеристики и цену. TryLearn возвращает bool: только живой игрок вне боя, подходящие требования и достаточно LP; списывает LP/записывает ID перед Changed, блокирует повторный вход. SynchronizeLevel выдаёт pointsPerLevel только за ещё не учтённые уровни. Awake связывает владельца и синхронизирует level; OnEnable/OnDisable подписывают/снимают ProgressChanged.

## 4. Переменные и Inspector

catalog — BattleSpell[]; pointsPerLevel=1. serialized learningPoints=3 — стартовый тестовый запас, highestAwardLevel=1 — защита повторной выдачи, learnedIds — стабильные строки. Points/LearnedIds — только чтение. unit/learning — внутренние, не Inspector. Changed уведомляет UI; очки характеристик не расходуются.

## 5. Связи

BattleUnit предоставляет уровень, эффективные атрибуты и MP; AbilityLearning хранит только LP/known IDs. BattleSpell содержит данные, BattleAbilities выполняет эффект, BattleManager строит меню, GameText локализует. Для tests используются NUnit/Unity Test Framework, для Editor — AssetDatabase и EditorSceneManager.

## 6. Пример

STR10 не позволяет изучить Power Strike; STR20 и1 LP позволяют. Снятие STR-экипировки после изучения не забывает способность. Fire/Heal уже известны даже при новых requirements Fire.

## 7. Настройка в Unity

Прикрепить один AbilityLearning к Player с BattleUnit. Заполнить catalog существующими Fire/Heal и новыми4 ассетами. pointsPerLevel/стартовый learningPoints настраиваются в Inspector. Не добавлять компонент врагам без необходимости.

## 8. Возможные проблемы

ID должен быть непустым и стабильным. Не расходовать attribute points на обучение. Уже известные Fire/Heal сохраняются; требования применяются при новом изучении. LP3+1/level — тестовая экономика без учителя/NPC. Постоянного save нет. Проверки — [ClasslessStage55](../../ClasslessStage55.md).

## 9. История изменений

- 2026-10-26 — реализация и первичная документация подэтапа5.5.

# Документация FalseMemory

FalseMemory — 2D Pixel Art RPG в стиле Dark Fantasy на Unity 6. Документы написаны для владельца без опыта C# и описывают текущую рабочую копию на **2026-10-11**.

Начните с [архитектуры](Architecture.md), затем откройте нужную систему или скрипт. Постоянные правила находятся в [AGENTS.md](../AGENTS.md); их нужно читать перед каждой новой задачей.

## Общие документы

- [Этап 3 — полировка](BattleStage3.md) — изменения представления боя и результаты проверок.

- [Architecture — структура и связи](Architecture.md).
- [Changelog — значимые изменения](Changelog.md).
- [Testing — результаты, покрытие и ручная проверка](Testing.md).
- [Настройка Unity](Guides/UnitySetup.md).
- [Рабочий процесс Git](Guides/GitWorkflow.md).
- [Руководство тестирования](Guides/TestingGuide.md).
- [Отчёт этапа 1](BattleStage1.md) — исторический отчёт до расширения; параметры этапа 2 смотрите в актуальных системах.
- [Отчёт этапа 2](BattleStage2.md) — реализация, список файлов, ограничения и подробная инструкция Play Mode.

## Игровые системы

- [Интерфейс боя](Systems/BattleUISystem.md).

- [Пошаговые сражения](Systems/CombatSystem.md).
- [Здоровье и HUD](Systems/HealthSystem.md).
- [Магия и MP](Systems/MagicSystem.md).
- [Расходуемые предметы](Systems/InventorySystem.md).
- [Поведение противника](Systems/EnemyAISystem.md).
- [Исследование мира и камера](Systems/ExplorationSystem.md).
- [Главное меню и настройки](Systems/MenuSystem.md).
- [Опыт и состояние игровой сессии](Systems/SessionProgressSystem.md).

Полноценного SaveSystem, партийного боя, классов, экипировки и level-up пока нет; они не документируются как готовые системы. PlayerPrefs уже сохраняет настройки меню, но не RPG-прогресс.

## Описания скриптов

- [BattleHUD](Scripts/Battle/BattleHUD.md) — оформление, полосы и предупреждение HP.
- [BattlePresentationFlowTests](Scripts/Tests/PlayMode/BattlePresentationFlowTests.md) — проверки представления боя.

После подэтапа 3.1 — **34 C#-файла в Assets**: 24 игровых, 9 тестовых и 1 служебный. Частичные тестовые классы объединяют несколько файлов, но каждый исходный файл имеет отдельное описание. Скрипты внешних пакетов и Library сюда не входят.

- [BattleAbilities](Scripts/Battle/BattleAbilities.md) — `Assets/Scripts/Battle/BattleAbilities.cs`.
- [BattleCalculator](Scripts/Battle/BattleCalculator.md) — `Assets/Scripts/Battle/BattleCalculator.cs`.
- [BattleConsumable](Scripts/Battle/BattleConsumable.md) — `Assets/Scripts/Battle/BattleConsumable.cs`.
- [BattleEncounter](Scripts/Battle/BattleEncounter.md) — `Assets/Scripts/Battle/BattleEncounter.cs`.
- [BattleInventory](Scripts/Battle/BattleInventory.md) — `Assets/Scripts/Battle/BattleInventory.cs`.
- [BattleManager](Scripts/Battle/BattleManager.md) — `Assets/Scripts/Battle/BattleManager.cs`.
- [BattleRandom](Scripts/Battle/BattleRandom.md) — `Assets/Scripts/Battle/BattleRandom.cs`.
- [BattleRules](Scripts/Battle/BattleRules.md) — `Assets/Scripts/Battle/BattleRules.cs`.
- [BattleSpell](Scripts/Battle/BattleSpell.md) — `Assets/Scripts/Battle/BattleSpell.cs`.
- [BattleTransition](Scripts/Battle/BattleTransition.md) — `Assets/Scripts/Battle/BattleTransition.cs`.
- [BattleTurnOrder](Scripts/Battle/BattleTurnOrder.md) — `Assets/Scripts/Battle/BattleTurnOrder.cs`.
- [BattleUI](Scripts/Battle/BattleUI.md) — `Assets/Scripts/Battle/BattleUI.cs`.
- [BattleUnit](Scripts/Battle/BattleUnit.md) — `Assets/Scripts/Battle/BattleUnit.cs`.
- [EnemyBattleAI](Scripts/Battle/EnemyBattleAI.md) — `Assets/Scripts/Battle/EnemyBattleAI.cs`.
- [CameraFollow](Scripts/CameraFollow.md) — `Assets/Scripts/CameraFollow.cs`.
- [EnemyAI](Scripts/EnemyAI.md) — `Assets/Scripts/EnemyAI.cs`.
- [EnemyAttack](Scripts/EnemyAttack.md) — `Assets/Scripts/EnemyAttack.cs`.
- [Health](Scripts/Health.md) — `Assets/Scripts/Health.cs`.
- [PlayerController](Scripts/PlayerController.md) — `Assets/Scripts/PlayerController.cs`.
- [MainMenuBootstrap](Scripts/UI/MainMenuBootstrap.md) — `Assets/Scripts/UI/MainMenuBootstrap.cs`.
- [MainMenuController](Scripts/UI/MainMenuController.md) — `Assets/Scripts/UI/MainMenuController.cs`.
- [PixelUi](Scripts/UI/PixelUi.md) — `Assets/Scripts/UI/PixelUi.cs`.
- [SettingsMenu](Scripts/UI/SettingsMenu.md) — `Assets/Scripts/UI/SettingsMenu.cs`.
- [BattleAbilityTests](Scripts/Tests/EditMode/BattleAbilityTests.md) — `Assets/Tests/EditMode/BattleAbilityTests.cs`.
- [BattleCoreTests](Scripts/Tests/EditMode/BattleCoreTests.md) — `Assets/Tests/EditMode/BattleCoreTests.cs`.
- [BattleExperienceTests](Scripts/Tests/EditMode/BattleExperienceTests.md) — `Assets/Tests/EditMode/BattleExperienceTests.cs`.
- [BattleFleeTests](Scripts/Tests/EditMode/BattleFleeTests.md) — `Assets/Tests/EditMode/BattleFleeTests.cs`.
- [BattleItemTests](Scripts/Tests/EditMode/BattleItemTests.md) — `Assets/Tests/EditMode/BattleItemTests.cs`.
- [BattleRandomTests](Scripts/Tests/EditMode/BattleRandomTests.md) — `Assets/Tests/EditMode/BattleRandomTests.cs`.
- [BattleFlowTests](Scripts/Tests/PlayMode/BattleFlowTests.md) — `Assets/Tests/PlayMode/BattleFlowTests.cs`.
- [BattleStage2FlowTests](Scripts/Tests/PlayMode/BattleStage2FlowTests.md) — `Assets/Tests/PlayMode/BattleStage2FlowTests.cs`.
- [Welcome2DScript](Scripts/Welcome/Welcome2DScript.md) — `Assets/Welcome/Welcome2DScript.cs`.

## Как пользоваться

В документах скриптов девять разделов: назначение, работа, методы, Inspector, связи, пример, отдельная настройка в Unity, проблемы/проверки и история изменений. В системах — общая последовательность и инструкция подключения/расширения.

Не путайте значения по умолчанию в C# с назначенными значениями сцены: Inspector может их переопределять. Настройки изменяйте в Editor осознанно; временные проверки HP/MP/шансов выполняйте только в Play Mode.

## Состояние Git этой документации

Первоначальная документация сохранена коммитом `bd17fa0`, дополнения постоянных правил и руководства — `0f609a1`. Существующая реализация этапа 2, сцены, ассеты, тесты и связанная документация теперь сохраняются отдельным локальным коммитом после повторных проверок 40 Edit Mode и 17 Play Mode. Новые игровые системы в задаче главного промпта повторно не создавались. Push не выполнялся.

Локальные коммиты обязательны после успешно проверенных задач; push выполняется исключительно по явной команде владельца. Если перед задачей есть сторонние изменения, добавляются только относящиеся к текущей задаче пути.

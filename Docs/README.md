# Документация FalseMemory

После Player rework4 —79 собственных C# (53 runtime,23 test,2 Editor,1 Welcome);84 Edit/46 Play и8 UI cases. Primary подключены; CHA/prepared scores пока без NPC/стамины/сопротивлений. [AttributeRowView](Scripts/Inventory/AttributeRowView.md), [CharacterAttributeFlowTests](Scripts/Tests/PlayMode/CharacterAttributeFlowTests.md), [отчёт](PlayerRework.md).

FalseMemory — 2D Pixel Art RPG в стиле Dark Fantasy на Unity 6. Документы написаны для владельца без опыта C# и описывают текущую рабочую копию на **2026-10-12**.

Начните с [архитектуры](Architecture.md), затем откройте нужную систему или скрипт. Постоянные правила находятся в [AGENTS.md](../AGENTS.md); их нужно читать перед каждой новой задачей.

## Общие документы

- [Переработка Player](PlayerRework.md) — myproject1idle, поэтапные проверки и ограничения новой задачи.

- [Прокачка5.1](ProgressionStage5.md) — EXP/уровни/рост и текущие проверки.

- [Skeleton64 и pixel VFX](PixelVisuals.md) — реальные64px кадры, подключение, проверки и ограничения.

- [Английская локализация](EnglishLocalization.md) — восстановленная работа, текущие тесты/кадры и ручная приёмка.

- [Переработка инвентаря](InventoryRedesign.md) — персонаж/экипировка/сумка, редкости, hover, результаты и ручная проверка.
- [Roadmap](Roadmap.md) — статусы этапов4–10 и ручные границы.
- [MasterDevelopmentPlan](MasterDevelopmentPlan.md) — полный план владельца.
- [Этап4](InventoryStage4.md) — интеграция/результаты.


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

- [Primary attributes](Systems/CharacterStatsSystem.md) — данные10 характеристик/формулы; подключение к геймплею отдельным подэтапом.

- [Направления игрока](Systems/AnimationSystem.md) — два directional Blend Tree; настоящие walk-кадры ещё нужны.

- [Прокачка](Systems/ProgressionSystem.md) — первые уровни и growth.

- [Визуал врагов](Systems/EnemyVisuals.md) — общий набор кадров мира/боя.

- [Локализация](Systems/LocalizationSystem.md) — английские Unity StringTables и GameText.
- [Экипировка](Systems/EquipmentSystem.md) — восемь слотов/бонусы/безопасный обмен.
- [Взаимодействие E](Systems/InteractionSystem.md) — общий контракт и подбор.

- [Звук боя](Systems/AudioSystem.md).

- [Эффекты магии](Systems/BattleVFXSystem.md).

- [Анимации боя](Systems/BattleAnimationSystem.md).

- [Интерфейс боя](Systems/BattleUISystem.md).

- [Пошаговые сражения](Systems/CombatSystem.md).
- [Здоровье и HUD](Systems/HealthSystem.md).
- [Магия и MP](Systems/MagicSystem.md).
- [Расходуемые предметы](Systems/InventorySystem.md).
- [Поведение противника](Systems/EnemyAISystem.md).
- [Исследование мира и камера](Systems/ExplorationSystem.md).
- [Главное меню и настройки](Systems/MenuSystem.md).
- [Опыт и состояние игровой сессии](Systems/SessionProgressSystem.md).

Полноценного SaveSystem, партийного боя и классов пока нет; level-up подключён в5.1; они не документируются как готовые системы. PlayerPrefs уже сохраняет настройки меню, но не RPG-прогресс.

## Описания скриптов

После Player rework3 —77 собственных C# (52 runtime,22 test,2 Editor,1 Welcome). Fresh84 Edit Mode;43 Play — предыдущая подготовка Animator, игровая интеграция primary ещё впереди.

- [CharacterAttributeValues](Scripts/Battle/CharacterAttributeValues.md), [CharacterAttributeRules](Scripts/Battle/CharacterAttributeRules.md), [CharacterAttributes](Scripts/Battle/CharacterAttributes.md), [CharacterAttributeTests](Scripts/Tests/EditMode/CharacterAttributeTests.md).

- [PlayerDirectionFlowTests](Scripts/Tests/PlayMode/PlayerDirectionFlowTests.md) — восемь направлений/Idle/последний взгляд/modal gate.

После Player rework2 —73 собственных C# (49 runtime,21 test,2 Editor,1 Welcome); текущие81 Edit/43 Play. Восемь уникальных ракурсов этим числом не подтверждаются.

- [CharacterGrowthRules](Scripts/Battle/CharacterGrowthRules.md).
- [CharacterGrowthTests](Scripts/Tests/EditMode/CharacterGrowthTests.md).
- [CharacterGrowthFlowTests](Scripts/Tests/PlayMode/CharacterGrowthFlowTests.md).

После5.1 —72 собственных C# (49 runtime,20 test,2 Editor,1 Welcome); предыдущие числа ниже исторические.

После native pixel visual подэтапа —69 собственных C# (48 runtime,18 test,2 Editor,1 Welcome); у каждого отдельное описание. Результаты ниже о предыдущих этапах исторические.

- [SpriteAnimationSet](Scripts/Battle/SpriteAnimationSet.md).
- [EnemySpriteAnimation](Scripts/Battle/EnemySpriteAnimation.md).
- [PixelBattleAssetBuilder](Scripts/Editor/PixelBattleAssetBuilder.md).
- [PixelArtTests](Scripts/Tests/EditMode/PixelArtTests.md).
- [PixelVisualFlowTests](Scripts/Tests/PlayMode/PixelVisualFlowTests.md).

- [GameText](Scripts/Localization/GameText.md) — ключи, форматирование, fallback.
- [LocalizedLabel](Scripts/Localization/LocalizedLabel.md) — привязка статической TMP-подписи.
- [GameTextEditor](Scripts/Editor/GameTextEditor.md) — редактирование English в Unity без программирования.
- [LocalizationTests](Scripts/Tests/EditMode/LocalizationTests.md) — таблица/ключи/форматы.
- [LocalizationFlowTests](Scripts/Tests/PlayMode/LocalizationFlowTests.md) — английские меню/бой/подсказки.

- [InventoryWindow.Layout](Scripts/Inventory/InventoryWindow.Layout.md) — оформление существующей сцены.
- [InventoryTooltip](Scripts/Inventory/InventoryTooltip.md) — одна ограниченная экраном hover-панель.
- [EquipmentSlotView](Scripts/Inventory/EquipmentSlotView.md) — надетые предметы/цвета/hover.
- [ItemPresentation](Scripts/Inventory/ItemPresentation.md) — текст/палитра редкостей.
- [ItemPresentationTests](Scripts/Tests/EditMode/ItemPresentationTests.md) — редкости/совместимость/текст.
- [InventoryPresentationFlowTests](Scripts/Tests/PlayMode/InventoryPresentationFlowTests.md) — реальная мышь/hover/границы.
- [WorldItemUseController](Scripts/Inventory/WorldItemUseController.md) — общий эффект/предикат и защита кадра.
- [EquipmentState](Scripts/Inventory/EquipmentState.md) — владение/обмен/бонусы.
- [EquipmentController](Scripts/Inventory/EquipmentController.md) — связка Player/UI.
- [EquipmentTests](Scripts/Tests/EditMode/EquipmentTests.md) — слоты/полнота/границы.
- [PlayerInteractor](Scripts/Inventory/PlayerInteractor.md) — E/подсказки.
- [WorldItemPickup](Scripts/Inventory/WorldItemPickup.md) — атомарный подбор.
- [InventoryWindow](Scripts/Inventory/InventoryWindow.md) — окно6×5.
- [InventorySlotView](Scripts/Inventory/InventorySlotView.md) — pointer/drag.
- [WorldInputGate](Scripts/Inventory/WorldInputGate.md) — блокировка ввода окнами.
- [InventoryFlowTests](Scripts/Tests/PlayMode/InventoryFlowTests.md) — окно/ввод/drag.
- [ItemData](Scripts/Inventory/ItemData.md) — статические определения.
- [InventoryState](Scripts/Inventory/InventoryState.md) — общий запас30 слотов/Gold.
- [InventoryTests](Scripts/Tests/EditMode/InventoryTests.md) — граничные операции.


- [BattleScreenFade](Scripts/Battle/BattleScreenFade.md) — временный экран перехода, включая Restart/Main Menu.

- [BattleAudioProfile](Scripts/Battle/BattleAudioProfile.md) — ссылки собственных звуков.
- [BattleAudioController](Scripts/Battle/BattleAudioController.md) — безопасное воспроизведение событий.

- [BattleVFXProfile](Scripts/Battle/BattleVFXProfile.md) — данные художественного эффекта.
- [BattleVFXController](Scripts/Battle/BattleVFXController.md) — лёгкие эффекты без изменений ресурсов.

- [BattlePresentation](Scripts/Battle/BattlePresentation.md) — визуальные позы и реакция портретов.

- [BattleHUD](Scripts/Battle/BattleHUD.md) — оформление, полосы и предупреждение HP.
- [BattlePresentationFlowTests](Scripts/Tests/PlayMode/BattlePresentationFlowTests.md) — проверки представления боя.

Историческая статистика после подэтапа4.6 — **53 C#-файла в Assets**:40 игровых,12 тестовых и1 служебный. Частичные тестовые классы объединяют несколько файлов, но каждый исходный файл имеет отдельное описание. Скрипты внешних пакетов и Library сюда не входят.

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

Первоначальная документация сохранена коммитом `bd17fa0`, дополнения постоянных правил и руководства — `0f609a1`. Реализация этапа 2 сохранена коммитом 07a1a34; этап 3 развивается семью локальными коммитами. Актуальные результаты и изменения приведены в BattleStage3.md, исторические проверки этапа2 — 40 Edit Mode и 17 Play Mode. Новые игровые системы в задаче главного промпта повторно не создавались. Push не выполнялся.

Локальные коммиты обязательны после успешно проверенных задач; push выполняется исключительно по явной команде владельца. Если перед задачей есть сторонние изменения, добавляются только относящиеся к текущей задаче пути.


Актуально после локализации:64 собственных C#-файла и64 отдельных описания (46 runtime,16 тестовых,1 Editor-инструмент,1 Welcome). Игровой язык English en-US; Docs/отчёты русские. Предыдущие отчёты и кадры сохраняются как история, не подтверждение текущего интерфейса.

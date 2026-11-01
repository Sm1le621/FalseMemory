# Документация FalseMemory

FalseMemory — 2D Pixel Art RPG в жанре Dark Fantasy на Unity 6. Документация описывает текущую рабочую копию на **2026-10-31** и написана по-русски для владельца без опыта C#.

Начните с [архитектуры](Architecture.md), затем откройте нужную систему или описание скрипта. Постоянные правила проекта находятся в [AGENTS.md](../AGENTS.md).

## Текущее состояние игры

- Три сцены: `MainMenu`, `SampleScene` (мир), `BattleScene` (пошаговый бой).
- Инвентарь: 30 слотов, Gold, предметы трёх категорий — `Consumable`, `Weapon`, `Armor`.
- Экипировка: два слота — `Weapon` и `Chest`; бонусы только `attack` и `defense`.
- Стартовый набор: Iron Sword ×1, Leather Armor ×1, Potion ×3.
- Боевые команды: Attack, Magic, Defend, Flee. Отдельной вкладки Items нет; Potion применяется из инвентаря вне боя.
- Магия: Fire; заклинание Heal удалено. MP показан в бою и отдельной полосой в мире (`ManaHUD`).
- Локализация, уровни/EXP, редкость предметов и подбор предметов с земли удалены. Игровой текст в сценах — английский.
- У скелета и игрока нет idle-анимации: в покое показывается статичный кадр, Animator работает только при движении.

## Общие документы

- [Architecture — структура и связи](Architecture.md).
- [Changelog — значимые изменения](Changelog.md).
- [Testing — результаты и ограничения проверок](Testing.md).
- [Настройка Unity](Guides/UnitySetup.md).
- [Рабочий процесс Git](Guides/GitWorkflow.md).
- [Руководство тестирования](Guides/TestingGuide.md).
- [Недостающие спрайты](Art/RequiredSprites.md).

Исторические отчёты предыдущих этапов сохранены, но могут описывать уже удалённые механики: [этап 1](BattleStage1.md), [этап 2](BattleStage2.md), [этап 3](BattleStage3.md), [InventoryStage4](InventoryStage4.md), [InventoryRedesign](InventoryRedesign.md), [PixelVisuals](PixelVisuals.md), [EnglishLocalization](EnglishLocalization.md), [ProgressionStage5](ProgressionStage5.md). Актуальное поведение определяется текущим кодом и документами систем.

## Игровые системы

- [Состав сцен](Systems/SceneComposition.md) — объекты, компоненты и связи MainMenu/SampleScene/BattleScene.
- [Пошаговые сражения](Systems/CombatSystem.md).
- [Интерфейс боя](Systems/BattleUISystem.md).
- [Магия и MP](Systems/MagicSystem.md).
- [Здоровье и HUD](Systems/HealthSystem.md).
- [Инвентарь и предметы](Systems/InventorySystem.md).
- [Экипировка](Systems/EquipmentSystem.md).
- [Блокировка ввода и использование предметов](Systems/InteractionSystem.md).
- [Поведение противника](Systems/EnemyAISystem.md).
- [Визуал врагов](Systems/EnemyVisuals.md).
- [Анимации боя](Systems/BattleAnimationSystem.md).
- [Эффекты магии](Systems/BattleVFXSystem.md).
- [Звук боя](Systems/AudioSystem.md).
- [Исследование мира и камера](Systems/ExplorationSystem.md).
- [Главное меню и настройки](Systems/MenuSystem.md).
- [Состояние игровой сессии](Systems/SessionProgressSystem.md).

Полноценного SaveSystem, партийного боя, классов, квестов, диалогов и торговли пока нет. `PlayerPrefs` сохраняет только настройки меню.

## Описания скриптов

Всего в `Assets` **59 собственных C#-файлов**: 42 runtime, 15 тестовых, 1 Editor-инструмент и 1 служебный Welcome. У каждого проектного скрипта есть отдельное описание в `Docs/Scripts`; частичные тестовые классы объединяют несколько файлов.

### Бой

- [BattleAbilities](Scripts/Battle/BattleAbilities.md)
- [BattleAudioController](Scripts/Battle/BattleAudioController.md)
- [BattleAudioProfile](Scripts/Battle/BattleAudioProfile.md)
- [BattleCalculator](Scripts/Battle/BattleCalculator.md)
- [BattleConsumable](Scripts/Battle/BattleConsumable.md)
- [BattleEncounter](Scripts/Battle/BattleEncounter.md)
- [BattleHUD](Scripts/Battle/BattleHUD.md)
- [BattleInventory](Scripts/Battle/BattleInventory.md)
- [BattleManager](Scripts/Battle/BattleManager.md)
- [BattlePresentation](Scripts/Battle/BattlePresentation.md)
- [BattleRandom](Scripts/Battle/BattleRandom.md)
- [BattleRules](Scripts/Battle/BattleRules.md)
- [BattleScreenFade](Scripts/Battle/BattleScreenFade.md)
- [BattleSpell](Scripts/Battle/BattleSpell.md)
- [BattleTransition](Scripts/Battle/BattleTransition.md)
- [BattleTurnOrder](Scripts/Battle/BattleTurnOrder.md)
- [BattleUI](Scripts/Battle/BattleUI.md)
- [BattleUnit](Scripts/Battle/BattleUnit.md)
- [BattleVFXController](Scripts/Battle/BattleVFXController.md)
- [BattleVFXProfile](Scripts/Battle/BattleVFXProfile.md)
- [EnemyBattleAI](Scripts/Battle/EnemyBattleAI.md)
- [EnemySpriteAnimation](Scripts/Battle/EnemySpriteAnimation.md)
- [SpriteAnimationSet](Scripts/Battle/SpriteAnimationSet.md)

### Инвентарь и экипировка

- [EquipmentController](Scripts/Inventory/EquipmentController.md)
- [EquipmentSlotView](Scripts/Inventory/EquipmentSlotView.md)
- [EquipmentState](Scripts/Inventory/EquipmentState.md)
- [InventorySlotView](Scripts/Inventory/InventorySlotView.md)
- [InventoryState](Scripts/Inventory/InventoryState.md)
- [InventoryTooltip](Scripts/Inventory/InventoryTooltip.md)
- [InventoryWindow](Scripts/Inventory/InventoryWindow.md)
- [ItemData](Scripts/Inventory/ItemData.md)
- [ItemPresentation](Scripts/Inventory/ItemPresentation.md)
- [WorldInputGate](Scripts/Inventory/WorldInputGate.md)
- [WorldItemUseController](Scripts/Inventory/WorldItemUseController.md)

### Мир, интерфейс и служебные

- [CameraFollow](Scripts/CameraFollow.md)
- [EnemyAI](Scripts/EnemyAI.md)
- [EnemyAttack](Scripts/EnemyAttack.md)
- [Health](Scripts/Health.md)
- [PlayerController](Scripts/PlayerController.md)
- [MainMenuController](Scripts/UI/MainMenuController.md)
- [ManaHUD](Scripts/UI/ManaHUD.md)
- [SettingsMenu](Scripts/UI/SettingsMenu.md)
- [PixelBattleAssetBuilder](Scripts/Editor/PixelBattleAssetBuilder.md)
- [Welcome2DScript](Scripts/Welcome/Welcome2DScript.md)

### Тесты

- EditMode: [BattleAbilityTests](Scripts/Tests/EditMode/BattleAbilityTests.md), [BattleCoreTests](Scripts/Tests/EditMode/BattleCoreTests.md), [BattleFleeTests](Scripts/Tests/EditMode/BattleFleeTests.md), [BattleItemTests](Scripts/Tests/EditMode/BattleItemTests.md), [BattleRandomTests](Scripts/Tests/EditMode/BattleRandomTests.md), [EquipmentTests](Scripts/Tests/EditMode/EquipmentTests.md), [InventoryTests](Scripts/Tests/EditMode/InventoryTests.md), [ItemPresentationTests](Scripts/Tests/EditMode/ItemPresentationTests.md), [PixelArtTests](Scripts/Tests/EditMode/PixelArtTests.md).
- PlayMode: [BattleFlowTests](Scripts/Tests/PlayMode/BattleFlowTests.md), [BattlePresentationFlowTests](Scripts/Tests/PlayMode/BattlePresentationFlowTests.md), [BattleStage2FlowTests](Scripts/Tests/PlayMode/BattleStage2FlowTests.md), [InventoryFlowTests](Scripts/Tests/PlayMode/InventoryFlowTests.md), [InventoryPresentationFlowTests](Scripts/Tests/PlayMode/InventoryPresentationFlowTests.md), [PixelVisualFlowTests](Scripts/Tests/PlayMode/PixelVisualFlowTests.md).

## Как пользоваться

Документ скрипта содержит девять разделов: назначение, работу, методы, Inspector, связи, пример, настройку в Unity, возможные проблемы и историю. Документы систем описывают общую логику, компоненты, последовательность событий, параметры, подключение, расширение и ограничения.

Значения по умолчанию в C# могут отличаться от назначенных в сценах и ассетах. Настройки меняйте в Editor осознанно; временные проверки HP/MP/шансов выполняйте только в Play Mode и не сохраняйте в ассеты.

## Git

Локальные коммиты обязательны после успешно проверенных задач. Push выполняется только по явной команде владельца. Актуальные результаты проверок — в [Testing.md](Testing.md); история изменений — в [Changelog.md](Changelog.md).

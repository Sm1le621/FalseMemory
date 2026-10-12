# CharacterAttributeFlowTests

## 1. Название и назначение

`Assets/Tests/PlayMode/CharacterAttributeFlowTests.cs`. Три дополнительных UnityTest существующего BattleFlowTests для игровой интеграции primary.

## 2. Как работает

Общий fixture загружает SampleScene/настраивает Input/боевые задержки и после восстанавливает. Каждый тест использует исходный Player/Health/Inventory; временный тестовый ItemData не сохраняется в Assets. Проверяются события и реальные команды существующего BattleManager.

## 3. Методы и функции

`PrimaryGearGrowthAndRestoreKeepOneSourceAndNeverHealOnRecalculation()` — IEnumerator: damage/spend, экипировка temporary test sword с primary/direct bonuses, repeated refresh, level2, unequip, JSON base restore и изменениеVIT после смерти; проверяет отсутствие heal/накопления/двойного growth иAGI/accuracy/evasion. Finally удаляет только созданный тестом item.

`PrimaryRowsShowTenValuesGearSourcesAndTooltipWithoutDuplicatingStats()` — IEnumerator: classSTR+2/temporarySTR−1, открывает текущий Inventory, проверяет10 строк/STR11/MCR10 и источники tooltip, закрывает.

`MagicControlCostAndMagicPowerApplyToExistingFireAndHeal()` — IEnumerator: MCR20/MAG14, вход в BattleScene, английская ценаFire7MP, реальное применениеFire23/Heal29 и ответ врага4; проверяет MP0/1 иHP66/91. Общие Setup/TearDown/WaitUntil описаны в [BattleFlowTests](BattleFlowTests.md). У самого файла нет MonoBehaviour callbacks.

## 4. Переменные и Inspector

Локальные Player/Enemy/attributes/window/item/manager, тестовый Weapon с directATK2 и STR3/DEX7/VIT2/INT2/WIS1/MAG4/AGI6. Начальные ресурсы70HP/22MP первого теста. Общий fixture выключает стандартные крит/промах/защиту скелета; первый небоевой тест явно возвращает missChance0.05 для проверки DEX. Магический тест открывает меню публичным OpenMagicMenu, затем посылает реальное TryPlayerSpell. Параметров Inspector нет.

## 5. Связи

CharacterAttributes/Rules, BattleUnit, EquipmentController/State, InventoryWindow/Tooltip, PlayerController, BattleAbilities/Manager/UI и английская таблица. JsonUtility проверяет восстановление primary base, не SaveSystem.

## 6. Пример

Level1 gear даётATK27/HPmax110/MPmax33, текущие70/22 сохраняются. Level2 даётATK29/115/35 без лечения. Unequip возвращает17/105/32, сохраняет70/22.

## 7. Настройка в Unity

К GameObject не прикреплять. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests → методы этого файла. Нужна актуальная сохранённая SampleScene с primary component/профилем и10 строками.

## 8. Возможные проблемы

Отсутствующие сцена/профиль/StringTable/связи дают провал. Проверка цены читает TMP текущей команды; действие использует реальный Manager, не подставленный результат. Нет проверки дискового save/load, художественной анимации, статистического баланса шансов и standalone. Мутируемые временные ресурсы изолирует общий teardown.

## 9. История изменений

- 2026-10-12 — добавлены3 сценария primary/ресурсов/level/экипировки/UI/Fire/Heal.

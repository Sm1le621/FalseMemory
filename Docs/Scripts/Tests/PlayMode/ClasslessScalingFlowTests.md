# ClasslessScalingFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/ClasslessScalingFlowTests.cs`. Сценарий на настоящей SampleScene/BattleScene для очков, ресурсов и нового оружия.

## 2. Как работает

Сценарий на настоящей SampleScene/BattleScene для очков, ресурсов и нового оружия. Использует прежние10 характеристик и одного владельца ресурсов/экипировки; отдельная система классов/сохранений не создаётся.

## 3. Методы и функции

AllocatedPointsScalingAndRequirementsSurviveRealBattleAndNeverRestoreResources() — UnityTest/IEnumerator: проверяет исходные100HP/40MP, выдаёт100EXP/1point, покупаетVIT11→108maxHP без лечения; повторное списание отклоняется. Greatsword приSTR10 не надевается/не теряется, приSTR25 надевается. EffectiveAttack равен одному weapon.Damage без повторногоSTR/DEX. Настоящий бой/победа/возврат сохраняют базу/очки/gear/MP32. Tooltip показывает Physical Damage/STR:S/Requires, активныйUI английский. Использует shared Setup/Cleanup/BeginBattle/WaitUntil из BattleFlowTests; собственных Unity runtime callbacks нет.

## 4. Переменные и Inspector

Временные level/base/HP/MP меняются только в Play fixture, не сохраняются в сцену. Новый pickup должен существовать; локальные expected/snapshot/points — не Inspector.

## 5. Связи

CharacterAttributeRules/CharacterAttributes/BattleUnit/ItemData/EquipmentState, GameText и существующая InventoryWindow. Для tests используются NUnit/Unity Test Framework; для Editor AssetDatabase/EditorSceneManager. Назначение конкретных связей раскрыто выше.

## 6. Пример

Level2 даёт1 свободное очко; послеVIT+1 текущееHP80 остаётся80, новая вместимость108. НачальныйMP40 после8 остаётся32 и таким же возвращается из боя.

## 7. Настройка в Unity

Не прикреплять. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests. Требует применённый ClasslessProgressionSetup/сохранённую SampleScene и настоящие сцены сборки. Ручное ощущение силы/tooltip-дизайна — отдельная проверка.

## 8. Возможные проблемы

Не прикреплять. Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests. Требует применённый ClasslessProgressionSetup/сохранённую SampleScene и настоящие сцены сборки. Ручное ощущение силы/tooltip-дизайна — отдельная проверка. Результаты запусков/ограничения — [ClasslessStage53](../../../ClasslessStage53.md). Дискового RPG-save нет, данные сохраняются в текущей игровой сессии.

## 9. История изменений

- 2026-10-26 — создана реализация подэтапа5.3; первичная документация текущего кода. Отдельный акт художественной/балансной приёмки не подразумевается.

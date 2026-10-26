# ClasslessScalingTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/ClasslessScalingTests.cs`. NUnit-контракты новых кривых, очков, оружия и требований, отдельно от legacy-проверок.

## 2. Как работает

NUnit-контракты новых кривых, очков, оружия и требований, отдельно от legacy-проверок. Использует прежние10 характеристик и одного владельца ресурсов/экипировки; отдельная система классов/сохранений не создаётся.

## 3. Методы и функции

Setup/Cleanup создают/удаляют только временный rules с classless=true. EverySoftCapHasPositiveNonIncreasingMarginalGainOnBothSides проверяет все10 stats/каждый cap−1/cap/cap+1. VitalityUsesRequestedHealthCurve пятью TestCase подтверждает10/20/40/60/99. IntelligenceUsesManaCurveWithSingleRoundingAndWisdomDoesNotAddMana пятью TestCase подтверждает40…220MP и отсутствие WIS-вклада. AgilityNeverChangesWalkSpeedAndPaidSpellsKeepPositiveThirtyPercentFloor проверяет0…intMax/платные заклинания. PointsAwardOnceAcrossLevelsAndReentrantAllocationCannotSpendTwice проверяет3 points, повторный sync/reentry/99/class modifier. ScalingGradesAreOrderedNoneIsBaseAndPrimarySpecializationBeatsWeakSecondary сравнивает равный бюджет40, ранги/дубли статов. RequirementsExcludeTheCandidateAndReplacementItemAndRejectWithoutLosingGear проверяет self-funded/replacement и сохранность. OldSerializedItemsCannotImplicitlyEnableScaling проверяет обязательный флаг. Все тесты возвращаютvoid, результаты через Assert; игровых callbacks нет.

## 4. Переменные и Inspector

rules, временные GameObject/ItemData/InventoryState/EquipmentState — локальные тестовые объекты, не Inspector. Созданные ресурсы удаляются только внутри tests finally/TearDown; сцена/проектные PNG не удаляются.

## 5. Связи

CharacterAttributeRules/CharacterAttributes/BattleUnit/ItemData/EquipmentState, GameText и существующая InventoryWindow. Для tests используются NUnit/Unity Test Framework; для Editor AssetDatabase/EditorSceneManager. Назначение конкретных связей раскрыто выше.

## 6. Пример

Оружие, само дающее STR100 и требующееSTR25, нельзя надеть при базе10. Даже заменяемый мечSTR+20 не должен оплатить требование кандидата.

## 7. Настройка в Unity

Не прикреплять к GameObject. Test Runner → EditMode → FalseMemory.Battle.EditModeTests. Проверки математики не доказывают художественную приёмку или баланс полного RPG-эпизода.

## 8. Возможные проблемы

Не прикреплять к GameObject. Test Runner → EditMode → FalseMemory.Battle.EditModeTests. Проверки математики не доказывают художественную приёмку или баланс полного RPG-эпизода. Результаты запусков/ограничения — [ClasslessStage53](../../../ClasslessStage53.md). Дискового RPG-save нет, данные сохраняются в текущей игровой сессии.

## 9. История изменений

- 2026-10-26 — создана реализация подэтапа5.3; первичная документация текущего кода. Отдельный акт художественной/балансной приёмки не подразумевается.

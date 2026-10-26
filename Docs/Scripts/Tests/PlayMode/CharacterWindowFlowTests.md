# CharacterWindowFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/CharacterWindowFlowTests.cs`. Интеграционные проверки окна, ввода, распределения и двух разрешений.

## 2. Как работает

Интеграционные проверки окна, ввода, распределения и двух разрешений. Расширяет текущую сцену, использует существующее состояние; игровая архитектура здоровья/боя не заменяется.

## 3. Методы и функции

CharacterKeysSwitchModalsBlockMovementAndRejectBattleAndOtherOwners использует настоящий InputSystem C/I/Esc, проверяет неподвижный Rigidbody/WorldInputGate, запрет при другом modal/encounter, возвращение после победы. CharacterAllocationTooltipsLearningAndEquipmentRenderRealStateWithoutDoubleSpend проверяет реальные кнопки+/Learn дважды вframe, LP/point/HP/MP, источники/cap/next effect,gear/CombatStats иbase99. CharacterPagesAndEdgeTooltipsFitFullHdAndUltrawide временно переключает GameView1920×1080/3440×1440, проверяет4 страницы/TMPoverflow/границыпанели иtooltip с двухкраёв. finally восстанавливаетpreset и удаляет только собственный временныйpreset. AssertOnScreen проверяет4corners. UnityTest возвращаютIEnumerator, общегоfixtureSetup/Cleanup не заменяют.

## 4. Переменные и Inspector

Синтетическая Keyboard/временный GameView preset живут только в тесте. Ссылки берутся из настоящего Player. Исходный размер восстанавливается. Никакие Play-статы/gear не сохраняются в сцену.

## 5. Связи

CharacterWindow → BattleUnit/CharacterAttributes/AbilityLearning/EquipmentController/GameText. WorldInputGate блокирует PlayerController/Interactor, InventoryWindow переключает modal. UI использует TMP/EventSystem/InputSystem; Editor authoring — AssetDatabase/EditorSceneManager.

## 6. Пример

Два callback + в одномframe тратят только1point; VIT11/max108 не лечит текущие60HP. Все4 страницы помещаются в оба разрешения.

## 7. Настройка в Unity

Не прикреплять. Test Runner PlayMode → FalseMemory.Battle.PlayModeTests. Проверка FullHD/Wide требует Unity Editor; условие UNITY_EDITOR исключает reflection изbuild. Художественная оценка удобства остаётся владельцу.

## 8. Возможные проблемы

Все ссылки UI обязательны. После добавления новых catalog entries нужны дополнительные строки/прокрутка: текущий экран рассчитан на6 способностей. Equipment здесь только просмотр; менять вещи вInventory. Прогноз использует текущую экипировку и округление; дискового save нет. [Проверки и ограничения](../../../ClasslessStage56.md).

## 9. История изменений

- 2026-10-26 — реализация/документация Character UI подэтапа5.6.

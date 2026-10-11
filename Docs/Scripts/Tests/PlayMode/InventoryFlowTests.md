# InventoryFlowTests

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/InventoryFlowTests.cs.

Дополнительная часть BattleFlowTests для окна/мыши/ввода.

## 2. Как работает

Использует прежние Setup/Cleanup реальной SampleScene. Создаёт/удаляет синтетическую клавиатуру и только тестовые данные.

## 3. Методы и функции
`SharedInventoryEquipmentAndEtherSurviveBattleVictoryAndWorldReturn` UnityTest IEnumerator: окно/Potion/Ether/повтор кадра, Sword/Armor/Helmet, настоящая BattleScene, Attack15/ответ1, HPMP-предметы/Fire/победа/возврат. Итог97HP/30MP/Potion1/Ether0/EXP20 и те же InventoryState/экипировка/позиция/мировой HUD. `WorldUseRestrictionsDisableButtonsWithoutConsumingItems` UnityTest: разрешённость только Battle/нехватка уровня, disabled кнопка и отказ без qty/HP.
`EquipmentButtonsApplyAndRemoveBonusesWithoutChangingBaseStatsOrResources` UnityTest IEnumerator: реальные сценовые определения/кнопки, пять циклов меча, Armor/Helmet; ATK20/DEF14, база15/8, HP100/MP30, одиночное владение и Stats.
`PickupERejectsFullInventoryThenCollectsExactlyOnceAndRespectsModalInput` — UnityTest IEnumerator: временный trigger ближе остальных,29 нестакаемых вещей+Potion заполняют30 слотов; E при окне не действует, E при полном запасе оставляет предмет; освобождение слота даёт2 вещи, десять повторных вызовов отказ. Реальный Input System/Physics2D-поиск, временные устройства/объекты очищаются finally.

`InventoryKeysBlockMovementAndEscapeRestoresIt` UnityTest IEnumerator: I+D открывает/останавливает тело, Escape+D закрывает/возобновляет;30 slots/gate. `InventoryPointerHandlersMoveSwapMergeAndCancelWithoutDuplication` проверяет split/drag/swap/merge/cancel/метку количества с исходным Potion. `DragSlot(window,source,target)` private void вызывает настоящие EventSystem handlers begin/drop/end через ExecuteEvents. Не полная симуляция мышиного raycast.

## 4. Переменные и Inspector

Локальные player/window/keyboard/material; временный material удаляется finally, устройство тоже. Inspector отсутствует.

## 5. Связи

FalseMemory.Battle.PlayModeTests/Runtime/InputSystem/EventSystem и общие fixtures BattleFlowTests.

## 6. Пример

Сценарий переносит Potion и материал, итоговые количества сохраняются.

## 7. Настройка в Unity

Не прикреплять; PlayMode/Test Runner или MCP с именем сборки. Проверить итоговое total>0.

## 8. Ограничения и расширение

Проверки handlers не доказывают физический путь курсора; реальный drag проверить вручную. Подбор/экипировка/использование ещё не входят.

## 9. История изменений
2026-10-12 — 4.5: два сценария общей сессии/ресурсов и ограничений.
2026-10-12 — 4.4: сценарий кнопок экипировки/повторных бонусов.
2026-10-12 — 4.3: реальный E/полный запас/резерв/повторные вызовы.

2026-10-12 — 4.2: два сценария окна/ввода/handlers.

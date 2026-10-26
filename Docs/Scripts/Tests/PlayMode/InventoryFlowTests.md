# InventoryFlowTests

## Изменение5.4 —2026-10-26

Сценарные ожидания урона обновлены под текущий DefaultBattleRules K=20. Реальные атаки/Fire/Defend/крит/лечение/предметы и переходы сцен продолжают проверяться через PlayMode. Прямой TakeDamage в подготовке теста не меняет значения. Исходные проверки MP/предметов/позиции/анимаций сохранены. Исторические результаты ниже не являются результатами текущего запуска; см. [ClasslessStage54](../../../ClasslessStage54.md).

## Текущее изменение5.3 —2026-10-26

Общие Potion/Ether/gear/drag/pickup/gate остаются под строгими сценариями. Ожидаемые MP адаптированы к новой вместимости40; прежние Sword20ATK/Armor14DEF и количество вещей не изменены.

Фактические проверки и ограничения: [ClasslessStage53](../../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/InventoryFlowTests.cs.

Дополнительная часть BattleFlowTests для окна/мыши/ввода.

## 2. Как работает

Использует прежние Setup/Cleanup реальной SampleScene. Создаёт/удаляет синтетическую клавиатуру и только тестовые данные.

## 3. Методы и функции

2026-10-12 — актуальная локализация: Текстовые ожидания приведены к английскому UI (New Game/Main Menu/Back/No items available/Inventory full/DEFEAT/описания HPMP/цен). Проверки состояния, количества, урона, ресурсов и поведения сохранены.
`RealMouseRaycastDragsAndCancelsThroughInputSystem` UnityTest IEnumerator: добавляет Mouse, вычисляет screen-центры через helper `SlotScreenCenter(slot)` → Vector2, отправляет настоящие MouseState position/buttons/delta в Input System, не вызывает handlers сам. Проверяет перенос0→9 и отмену вне экрана без потери3 Potion/ghost; finally удаляет устройство/закрывает. Тест I/Escape теперь также проверяет backdrop.
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

Отдельный DragSlot вызывает handlers напрямую; новый Mouse-сценарий проходит настоящий Input System/UI-raycast. В набор также входят подбор/экипировка/общие предметы/бой/возврат. Человеческое удобство/мышь на рабочем месте/звук/standalone оцениваются отдельно.

## 9. История изменений

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.
2026-10-12 — 4.6: настоящий mouse/raycast/drop/cancel и backdrop.
2026-10-12 — 4.5: два сценария общей сессии/ресурсов и ограничений.
2026-10-12 — 4.4: сценарий кнопок экипировки/повторных бонусов.
2026-10-12 — 4.3: реальный E/полный запас/резерв/повторные вызовы.

2026-10-12 — 4.2: два сценария окна/ввода/handlers.

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

- 2026-10-26 — броня/Holy/рейтинговая защита подэтапа5.4; описание изменений приведено в начале документа.

# WorldItemUseController

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/WorldItemUseController.cs.

Применение расходуемого из окна мира через тот же BattleAbilities/InventoryState, что и бой.

## 2. Как работает

Awake получает Unit/Window; OnEnable связывает UseRequested и CanUseSelection. Попытка проверяет контекст/слот/условия и резервирует номер кадра до расхода. Повторный callback в тот же кадр отклоняется. Эффект не копируется в новый код.

## 3. Методы и функции

Unity `Awake` ссылки, `OnEnable` подписывает HandleUse/назначает CanUse, `OnDisable` снимает собственные подписку/predicate. `CanUse(index)` bool требует активного компонента/открытого окна/индекс0..29/отсутствия встречи; получает BattleConsumable и вызывает CanUseItem с Exploration. `TryUse(index)` bool проверяет lastUseFrame/CanUse, ставит кадр до TryUseItem, показывает фактический HP или MP/Refresh. `HandleUse(index)` void делегирует. Игра не ждёт coroutine.

## 4. Переменные и Inspector

unit/window private ссылки; lastUseFrame=-1 transient. Нет отдельного количества/HP/MP/Inspector-параметров. Predicate не содержит lastUseFrame, чтобы кнопка не оставалась disabled после операции.

## 5. Связи

InventoryWindow.UseRequested/CanUseSelection, BattleAbilities и прежние Health/BattleUnit.Inventory. После скрытия мира отписывается, после возврата подписывается без сброса ресурсов.

## 6. Пример

HP50/MP5 → Potion (+30HP)80, Ether (+20MP)25. Каждый уменьшает одну вещь общего запаса, повторное событие кадра отказ.

## 7. Настройка в Unity

Добавлен MCP на прежний Player, Require Unit/InventoryWindow. Нет другого менеджера/запаса. Window.Open задаёт допустимый контекст.

## 8. Ограничения и расширение

Как раньше Potion, положительный расходуемый можно потратить при полном ресурсе: фактический amount0. HPMP-гибриды/нулевые эффекты не поддержаны. Самые быстрые отдельные нажатия разных кадров — отдельные действия, не копии. Нет лечения погибшего.

## 9. История изменений

2026-10-12 — 4.5: общая логика мира/боя и защита callback кадра.

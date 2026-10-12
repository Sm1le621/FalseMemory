# InventoryState

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Inventory/InventoryState.cs.

Сериализуемая модель30 слотов и единственного Gold. InventorySlot — внешне неизменяемая пара Item/Count.

## 2. Как работает

Player rework5: внутреннийsingle exchange также принимает empty destination для адресногоunequip. Capacity30/Gold/обычныеstack/move/use операции прежние.

Операции проверяются целиком до мутации, отказ не вызывает Changed/частичный расход. Событие после успешной операции. UI не хранит запас.

## 3. Методы и функции

TryExchangeSingle(index,expected,replacement) проверяеттекущийItem==expected; еслиexpectednull, ячейкаобязательнопустая, иначеCount==1. Replacement singleton/definition conflicts проверяютсядо записи. Одинновыйreadonlyслот иChanged; falseничегонеменяет. Этообщаяточкадляbag→gear/gear→bag swaps, не второйинвентарь.

`Definitions` → уникальные занятые определения. `GetSlot(index)` → readonly слот/Empty, неверный индекс бросает исключение. `Count(item)` суммирует с насыщением int.MaxValue. `CanAdd` проверяет ID/место/положительное количество; `TryAdd` сначала дополняет стопки, затем пустые. `TryRemove` проверяет полный запас, удаляет через стопки, освобождает нулевые. Все Try возвращают bool.

`TryMoveOrMerge(from,to)` переносит/обменивает разные предметы/сливает одинаковые до лимита, остаток остаётся; полная одинаковая цель отклоняется. `TrySplit(from,to,amount)` требует пустой цели и0<amount<source.Count. `FirstEmptySlot()` → индекс/-1. Внутренний `TryExchangeSingle(index,expected,replacement)` меняет один нестакаемый предмет для будущей экипировки; `Valid` проверяет0..29; `Conflicts` проверяет ID другой ссылки. `TryAddGold`/`TrySpendGold` проверяют положительное число, предел/нехватку. Unity callbacks нет.

## 4. Переменные и Inspector

Capacity30; private slots[30], private gold начально0; Gold readonly, Changed Action. InventorySlot.Item/Count/IsEmpty readonly, конструктор внутренний. Empty статический view, в slots не записывается.

## 5. Связи

BattleInventory.State владеет данными; ItemData статичны. Подписчики Changed не должны рекурсивно менять запас.

## 6. Пример

299 предметов слимитом10 занимают30 слотов. +2 отклоняется без потери; +1 доводит до300.

## 7. Настройка в Unity

Не компонент, создаётся new InventoryState либо BattleInventory. Приватные поля не редактировать.

## 8. Ограничения и расширение

Неиспользоватьinternal exchange вместообычныхstack операций: replacement.StackLimit1. Save/load игры ещёнеподключён; JSON структуры сампонеозначает стабильныеassetID.

Внешняя десериализация/сохранения — этап9. Изменение maxStack ниже текущего количества требует отдельной миграции. Count насыщается int.MaxValue; некорректные события подписчиков не считаются валидной транзакцией. Торговля ещё не создана.

## 9. История изменений

- 2026-10-12 — Player rework5: девятыйOffHand, bidirectionaldrag/atomictransfers/совместимость/силуэты; sourceIDsсохранены.

2026-10-12 — 4.1:30 слотов, атомарные операции, стопки и Gold.

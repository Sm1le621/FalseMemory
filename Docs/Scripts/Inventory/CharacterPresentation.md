# CharacterPresentation

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/CharacterPresentation.cs`. Форматирование Character UI и чистый preview следующего очка без изменения игрока.

## 2. Как работает

Форматирование Character UI и чистый preview следующего очка без изменения игрока. Расширяет текущую сцену, использует существующее состояние; игровая архитектура здоровья/боя не заменяется.

## 3. Методы и функции

AttributeName получает English StringTable имя. AttributeDescription(unit,attribute) копирует effective values, добавляет1 только к копии, сравнивает derived adjustments, weapon damage, armor bonuses, crit/evasion/accuracy/Holy/MP costs/healing. Исключает глобальный Attack-бонус канала, заменённого Weapon Scaling. Показывает base/equipment/temporary/effective/next cap и фактическую разницу после округления; при99 — предел. Percent добавляет только ненулевые процентные изменения. CombatStats форматирует9 боевых значений. AbilityState переводит enum доступности. AbilityDescription показывает имя/описание/MP/уровень/требования с current/status/type. Все методы возвращают строки, callbacks отсутствуют.

## 4. Переменные и Inspector

Нет Inspector и хранимого игрового состояния. unit/attribute/ability передаются параметрами. preview отражает текущий набор надетых вещей и округление, а не обещание будущих ещё не реализованных механик.

## 5. Связи

CharacterWindow → BattleUnit/CharacterAttributes/AbilityLearning/EquipmentController/GameText. WorldInputGate блокирует PlayerController/Interactor, InventoryWindow переключает modal. UI использует TMP/EventSystem/InputSystem; Editor authoring — AssetDatabase/EditorSceneManager.

## 6. Пример

WIS30 показывает следующий Holy+0.25 процентного пункта; MP от WIS не увеличиваются. VIT11 показывает MaxHP+8.

## 7. Настройка в Unity

Не прикреплять к GameObject. Вызывается CharacterWindow, использует GameText и реальные правила/экипировку. Новые строки добавлять в существующую English StringTable.

## 8. Возможные проблемы

Все ссылки UI обязательны. После добавления новых catalog entries нужны дополнительные строки/прокрутка: текущий экран рассчитан на6 способностей. Equipment здесь только просмотр; менять вещи вInventory. Прогноз использует текущую экипировку и округление; дискового save нет. [Проверки и ограничения](../../ClasslessStage56.md).

## 9. История изменений

- 2026-10-26 — реализация/документация Character UI подэтапа5.6.

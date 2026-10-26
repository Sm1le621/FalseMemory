# CharacterAttributeView

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/CharacterAttributeView.cs`. Связь строки характеристики с pointer tooltip окна Character.

## 2. Как работает

Связь строки характеристики с pointer tooltip окна Character. Расширяет текущую сцену, использует существующее состояние; игровая архитектура здоровья/боя не заменяется.

## 3. Методы и функции

OnPointerEnter(PointerEventData) передаёт attribute и screen position в owner.ShowAttribute(...,true). OnPointerExit скрывает подсказку. Автоматического Update/Awake нет; label/increase заполняет и связывает CharacterWindow.

## 4. Переменные и Inspector

owner — Player.CharacterWindow; attribute — стабильный enum; label — TMP; increase — кнопка +. Назначены для10 созданных UI-строк, это не сами данные характеристики.

## 5. Связи

CharacterWindow → BattleUnit/CharacterAttributes/AbilityLearning/EquipmentController/GameText. WorldInputGate блокирует PlayerController/Interactor, InventoryWindow переключает modal. UI использует TMP/EventSystem/InputSystem; Editor authoring — AssetDatabase/EditorSceneManager.

## 6. Пример

Наведение на VIT открывает источники и прогноз, выход скрывает tooltip.

## 7. Настройка в Unity

Компонент уже находится на Attribute_0…9 под CharacterPanel/Page_attributes. Нужны Image с raycast и EventSystem/InputSystemUIInputModule существующей сцены.

## 8. Возможные проблемы

Все ссылки UI обязательны. После добавления новых catalog entries нужны дополнительные строки/прокрутка: текущий экран рассчитан на6 способностей. Equipment здесь только просмотр; менять вещи вInventory. Прогноз использует текущую экипировку и округление; дискового save нет. [Проверки и ограничения](../../ClasslessStage56.md).

## 9. История изменений

- 2026-10-26 — реализация/документация Character UI подэтапа5.6.

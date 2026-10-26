# CharacterWindowSetup

## 1. Название и назначение

Исходник: `Assets/Editor/CharacterWindowSetup.cs`. Адресное создание нового Character UI внутри существующей SampleScene через MCP.

## 2. Как работает

Адресное создание нового Character UI внутри существующей SampleScene через MCP. Расширяет текущую сцену, использует существующее состояние; игровая архитектура здоровья/боя не заменяется.

## 3. Методы и функции

ApplyStage56 требует остановленныйPlay/активнуюSampleScene, добавляет English keys и корректное описание WIS, создаёт CharacterWindow на Player, собственные backdrop/panel/pages/rows/tooltip подInventoryCanvas, назначает ссылки, скрывает окно и сохраняет только StringTables/сцену. При уже заполненном panel прерывается, не удаляет/перезаписывает пользовательский UI. Box создаёт RectTransform/Image с размером и привязкой; Label создаёт TMP с существующим font, без raycast; Button создаёт кнопку без keyboard navigation и текст; Border добавляет Outline2px; Add обновляет строку. Методы возвращают созданный компонент/void, runtime callbacks нет.

## 4. Переменные и Inspector

Размер authored panel1120×680 в координатах существующего CanvasScaler,4 pages1064×420. Стиль — тёмный фон/золотистая рамка/читаемый прежний шрифт; новых bitmap нет. font — временная ссылка Editor helper, не игровое состояние.

## 5. Связи

CharacterWindow → BattleUnit/CharacterAttributes/AbilityLearning/EquipmentController/GameText. WorldInputGate блокирует PlayerController/Interactor, InventoryWindow переключает modal. UI использует TMP/EventSystem/InputSystem; Editor authoring — AssetDatabase/EditorSceneManager.

## 6. Пример

ApplyStage56 создаёт4 вкладки без изменения старой сетки30/экипировки9/контроллера/сцен боя.

## 7. Настройка в Unity

Статический Editor-класс не прикрепляется. Вызов MCP execute_code CharacterWindowSetup.ApplyStage56(). Повторное создание поверх заполненного окна запрещено; изменять уже созданные объекты адресно через MCP.

## 8. Возможные проблемы

Все ссылки UI обязательны. После добавления новых catalog entries нужны дополнительные строки/прокрутка: текущий экран рассчитан на6 способностей. Equipment здесь только просмотр; менять вещи вInventory. Прогноз использует текущую экипировку и округление; дискового save нет. [Проверки и ограничения](../../ClasslessStage56.md).

## 9. История изменений

- 2026-10-26 — реализация/документация Character UI подэтапа5.6.

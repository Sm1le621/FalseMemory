# CharacterWindow

## Уточнение 5.7 —2026-10-26

Refresh теперь обновляет также уже открытую tooltipText после StatsChanged: временныебонусы/смена gear не оставляютстарое effective значение. Повторныйрендернеизменяет points/HP/MP. Проверено ClasslessRegressionFlowTests.

## 1. Название и назначение

Исходник: `Assets/Scripts/Inventory/CharacterWindow.cs`. Отдельное окно Character с реальными данными Player, распределением очков и изучением умений.

## 2. Как работает

Отдельное окно Character с реальными данными Player, распределением очков и изучением умений. Расширяет текущую сцену, использует существующее состояние; игровая архитектура здоровья/боя не заменяется.

## 3. Методы и функции

Awake получает BattleUnit/WorldInputGate/InventoryWindow, связывает собственные кнопки и закрывает окно. OnEnable/OnDisable подписывают/снимают StatsChanged/ProgressChanged/Learning.Changed; OnDisable также Close. OnDestroy очищает только listeners принадлежащих этому окну кнопок. Update обрабатывает C, Esc/I, закрывает окно при смерти/бое и обновляет HP/MP при изменении. LateUpdate ограничивает tooltip текущими границами экрана после смены разрешения.

Open() → bool: требует живого Player с primary rules/Learning вне encounter, закрывает Inventory, отклоняет другой modal owner, включает фон/панель и Acquire. Close() скрывает панель/фон/tooltip, Release. SelectTab(index) включает одну из4 страниц, меняет цвет, вызывает Refresh. RefreshHeader показывает имя/уровень/EXP/HP/MP/очки; Refresh строит10 строк base/gear/effective, боевые показатели, оружие,6 ability rows/status/кнопку Learn и9 equipment rows/описание из настоящего состояния. Allocate(stat) → bool: окно должно быть открыто, один запрос за frame; делегирует TryIncreaseBase, обновляет данные без самостоятельного изменения HP. SelectAbility меняет выбранный индекс. LearnSelected ограничен одним frame и делегирует TryLearn. ShowAttribute выбирает характеристику и текст, при tooltip=true сохраняет экранную точку и открывает подсказку. PositionTooltip пересчитывает размер/позицию и ограничивает parent.rect. HideTooltip скрывает её.

## 4. Переменные и Inspector

Inspector: panel/tooltipPanel/backdrop, текстовые поля, close/learn, tabs[4]/pages[4], attributeRows[10], abilityButtons[6], equipmentButtons[9]. Все ссылки назначены в SampleScene. selected* и last*Frame — внутренние состояния, не баланс. IsOpen/SelectedTab/Unit — чтение. Точки распределения/known/LP принадлежат другим компонентам.

## 5. Связи

CharacterWindow → BattleUnit/CharacterAttributes/AbilityLearning/EquipmentController/GameText. WorldInputGate блокирует PlayerController/Interactor, InventoryWindow переключает modal. UI использует TMP/EventSystem/InputSystem; Editor authoring — AssetDatabase/EditorSceneManager.

## 6. Пример

Нажать C, получить1 очко заlevel, нажать + уVIT: база10→11, MaxHP100→108, текущие60HP остаются60. I переводит к прежнему Inventory.

## 7. Настройка в Unity

Один компонент на Player рядом с BattleUnit, CharacterAttributes, AbilityLearning и WorldInputGate. Панели находятся под существующим InventoryCanvas/CanvasScaler; отдельного владельца персонажа нет. Подключение через CharacterWindowSetup и MCP.

## 8. Возможные проблемы

Все ссылки UI обязательны. После добавления новых catalog entries нужны дополнительные строки/прокрутка: текущий экран рассчитан на6 способностей. Equipment здесь только просмотр; менять вещи вInventory. Прогноз использует текущую экипировку и округление; дискового save нет. [Проверки и ограничения](../../ClasslessStage56.md).

## 9. История изменений

- 2026-10-26 — реализация/документация Character UI подэтапа5.6.

- 2026-10-26 — баланс/регрессия 5.7; актуальное уточнение приведено выше.

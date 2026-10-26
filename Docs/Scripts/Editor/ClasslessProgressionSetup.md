# ClasslessProgressionSetup

## 1. Название и назначение

Исходник: `Assets/Editor/ClasslessProgressionSetup.cs`. Адресная редакторская настройка подэтапа5.3 через Unity MCP, не runtime менеджер.

## 2. Как работает

Адресная редакторская настройка подэтапа5.3 через Unity MCP, не runtime менеджер. Использует прежние10 характеристик и одного владельца ресурсов/экипировки; отдельная система классов/сохранений не создаётся.

## 3. Методы и функции

ApplyStage53() требует Play выключен; читает существующие rules/table, включает classless/referenceMana40/points1/no automatic growth/AGI movement0/MCR cap.3; создаёт или обновляет6 своих weapon assets, GameText keys и6 pickup-instance прежнего Prefab под TestLoot. MarkSceneDirty/SaveScene проверяется. Add(table,key,value) обновляет/добавляет стабильную английскую строку. MakeWeapon(...) создаёт только свой ID, отклоняет коллизию с чужим предметом, пишет requirements/grades/тип/явный флаг и временную иконку. GameTextEditorInvalidation() запрашивает Editor loop; runtime читает ту же таблицу после нового Play. Awake/Start/Update отсутствуют.

## 4. Переменные и Inspector

Пути существующих SampleScene/DefaultCharacterAttributes/GameText_en-US/TestLoot/WorldItemPickup.prefab фиксированы для этого проекта. Greatsword40 S/E STR25; Longsword28 B/C; Rapier24 S/E DEX18; Dagger18 A/C; MagicStaff20 S/D INT20 MAG15; SacredBlade30 WIS A/STR B WIS20. ID weapon.*; worldId scaling_weapon_*. У SacredBlade сейчас physical-канал, Holy появится5.4.

## 5. Связи

CharacterAttributeRules/CharacterAttributes/BattleUnit/ItemData/EquipmentState, GameText и существующая InventoryWindow. Для tests используются NUnit/Unity Test Framework; для Editor AssetDatabase/EditorSceneManager. Назначение конкретных связей раскрыто выше.

## 6. Пример

После ApplyStage53 на карте слева6 новых pickups, можно взять Longsword/Dagger сразу. Greatsword остаётся в сумке при STR10, после повышения до25 разрешён.

## 7. Настройка в Unity

Вызывать только вне Play на существующей SampleScene через MCP execute_code. Не прикреплять компонентом. Команда меняет баланс собственных новых предметов и исходный профиль; не запускать как автоматический импорт при каждом Refresh. Не сохраняет чужой TMP-кэш через SaveAssets.

## 8. Возможные проблемы

Вызывать только вне Play на существующей SampleScene через MCP execute_code. Не прикреплять компонентом. Команда меняет баланс собственных новых предметов и исходный профиль; не запускать как автоматический импорт при каждом Refresh. Не сохраняет чужой TMP-кэш через SaveAssets. Результаты запусков/ограничения — [ClasslessStage53](../../ClasslessStage53.md). Дискового RPG-save нет, данные сохраняются в текущей игровой сессии.

## 9. История изменений

- 2026-10-26 — создана реализация подэтапа5.3; первичная документация текущего кода. Отдельный акт художественной/балансной приёмки не подразумевается.

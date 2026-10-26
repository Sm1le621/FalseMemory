# Character UI

## Баланс и регрессия 5.7 —2026-10-26

Открытый tooltip обновляетсяв Refresh приизменении temporary/gear. Дляручнойприёмки без C# добавлен Editor-only пункт FalseMemory/Testing/Grant 60 Levels (Play Mode Only),которыйвыдаётреальный EXP втекущейсессиибез лечения/сохранения. [Сценарийприёмки](../ClasslessStage57.md).

## Назначение и структура

Отдельное окно по C на Player, под существующим InventoryCanvas с его CanvasScaler. Сетка Inventory30/Equipment9 сохранена. Четыре вкладки: Attributes, Combat Stats, Abilities, Equipment. CharacterWindow читает существующие компоненты и не хранит дубликат характеристик/ресурсов.

## Ввод и события

C открывает/закрывает, Esc закрывает, I переключает на Inventory. Окно требует живого Player вне боя и отсутствия постороннего modal owner. WorldInputGate удерживается только пока окно открыто, освобождается при Close/OnDisable. StatsChanged/ProgressChanged/Learning.Changed обновляют данные; HP/MP проверяются в Update, потому что Health не предоставляет ресурсное событие. Временная деактивация мира при бое закрывает окно.

## Данные и операции

Attributes показывает10 base/gear/effective, отдельные свободные points; tooltip и правая панель раскрывают temporary sources, next cap и эффект следующего base point. Preview не меняет данные. Allocate делегирует CharacterAttributes, ограничивает повтор в одномframe, base≤99; capacity не лечит. CombatStats показывает Attack/MagicAttack/Defense/MagicDefense/crit/evasion/initiative/Holy/miss и настоящее оружие сScaling/requirements. Abilities показывает все6 entries, availability/requirements и Learn за отдельныеLP. Equipment показывает9 реальных слотов и выбранное описание; смена вещей остаётся вInventory.

## Unity и расширение

CharacterWindowSetup создал тёмную панель1120×680,золотистые рамки,использовал прежний TMPfont,кнопки+/Learn/Close,tooltip. Все English keys вGameText. В Inspector Player.CharacterWindow назначены4tabs/pages,10rows,6abilityButtons,9equipmentButtons. Не запускать повторное создание поверх готовой панели. При добавлении большого каталога расширить rows/ScrollRect; текущие6 строк являются явным ограничением.

## Проверки и ограничения

[Текущие запуски](../ClasslessStage56.md). Автоматически проверяется C/I/Esc,движение,modal owners,point/LPdoublecallbacks,99,gear,возврат изBattle иlayout1920×1080/3440×1440. Tooltip ограничивается экраном послеresize. Пользовательская оценка читаемости/внешнего вида ибаланса обязательна передэтапом6. Persistent save ещё не реализован.

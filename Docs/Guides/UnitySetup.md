# Настройка существующего проекта в Unity

Актуально на **2026-10-31**. Открывайте корень `FalseMemory`, содержащий `Assets`, `Packages` и `ProjectSettings`. Проверенная версия Editor — **6000.6.2f1**. Создавать новую сцену для проверки боя не нужно.

## Сцены

В Build Profiles → Scene List включены `MainMenu`, `SampleScene` и `BattleScene`. `MainMenu` запускает новую игру; `SampleScene` содержит мир и участников; `BattleScene` загружается аддитивно координатором `BattleTransition`. Прямой Play в `BattleScene` без встречи показывает вспомогательное меню.

## Player в SampleScene

- `PlayerController`: `Rigidbody2D`, физический `Collider2D`, `SpriteRenderer`, тег `Player`. Animator необязателен; в покое показывается статичный спрайт `Idle1.ase`, при движении включается `Player_Walk`. Idle-клипа нет.
- `Health`: Max Health 100, ссылки на мировой `Slider` и `HPText`. Полоса не интерактивна.
- `ManaHUD`: ссылки на `ManaSlider`, `MPText` и `BattleUnit` игрока. Компонент живёт на объекте `Health`, читает `CurrentMP/MaxMP` и сам ресурсы не меняет.
- `BattleUnit`: Max MP 30, MATK 12, MDEF 8, Fire в Spells, crit 0.1/miss 0.05. Полей Level/EXP/Reward больше нет.
- `BattleInventory`: `startingItems` = Iron Sword ×1, Leather Armor ×1, Potion ×3. Модель `State` создаётся один раз; количество хранится в ней, не в ScriptableObject.
- `EquipmentController`, `InventoryWindow`, `WorldInputGate`, `WorldItemUseController` — на том же Player.

Current Health/MP до запуска не являются сохранением: `Start`/`Awake` заполняют их максимумами. Для временных проверок меняйте текущие значения только в Play Mode.

## Enemy и переход

`Enemy` содержит `Rigidbody2D`, триггер обнаружения для `EnemyAI`, физический `Collider2D` для `BattleEncounter`, `Health` 60 и `BattleUnit` ATK 12/DEF 5/SPD 7/MDEF 3. `EnemyAttack` выключен; наличие `BattleEncounter` дополнительно исключает контактный урон. `BattleUnit.visualAnimations` = `Skeleton64`, idle-кадров в наборе нет.

Единственный `BattleTransition` содержит Rules = `DefaultBattleRules` и имена сцен `BattleScene`/`MainMenu`. Он сохраняет исходные объекты мира; копировать Player/Enemy в `BattleScene` не нужно. `CameraFollow` ссылается на Player.

## Боевой UI

`BattleManager.ui` ссылается на `BattleUI`. В UI назначены сообщения, характеристики, раунд, портреты, четыре кнопки команд (Attack, Magic, Defend, Flee) и Back, панель результата. `BattleHUD`, `BattlePresentation`, `BattleVFXController` и `BattleAudioController` подключены к `BattleManager`. HUD содержит HP/MP/Enemy HP, группы проявления и иконки команд; полосы не интерактивны.

Canvas боя: Scale With Screen Size 960×540, Match Height = 1, Pixel Perfect = true. Оформление сохранено объектами сцены; Editor-методов `BuildLayout` больше нет. Тексты английские и заданы прямо в TMP-компонентах сцены.

`Fire` использует `FireVisual`; `HealVisual` остаётся ассетом, но заклинание Heal удалено и профиль ни одним заклинанием не используется. В `BattleAudioController` назначен `DefaultBattleAudio` и существующий AudioSource: Spatial Blend 0, Ignore Listener Volume = false. Клипы пока пусты — назначайте свои в профиль.

`BattlePresentation`: Attack 0.4, Hit 0.18, Outcome 0.35, Message 0.75, Enemy Think 0.5, Allow Fast Forward = true, множитель 3. Space ускоряет движения и паузы, не игровой урон. `DefaultBattleRules.actionDelay` = 0.8, `fadeDuration` = 0.5.

## Инвентарь и экипировка

`InventoryCanvas` содержит 30 ячеек, описание, кнопки Use/Equip/Close, подсказку и два слота экипировки: `EquipmentWeapon` (`slot = Weapon`) и `EquipmentChest` (`slot = Chest`). У каждого `EquipmentSlotView` должны быть назначены `owner`, `icon`, `background`, `border` (компонент `Outline`) и `slotLabel`. Модели персонажа, портрета и лишних слотов больше нет.

`I`/`Escape` открывают и закрывают окно; `WorldInputGate` блокирует движение. Перетаскивание, обмен и разделение работают мышью. Экипировка передаёт владение в `EquipmentState`; базовые ATK 15/DEF 8 не меняются, `EffectiveAttack/EffectiveDefense` добавляют бонусы. Снятие при полной сумке безопасно отклоняется.

Стартовые предметы задаются в `BattleInventory.startingItems`. Не меняйте seed после создания `State` в Play Mode. Iron Sword даёт ATK +5, Leather Armor — DEF +4, Potion восстанавливает до 30 HP и расходуется по одному.

## Использование предметов

`WorldItemUseController` уже на Player. Выберите Potion → Use. Ограничения задаются `ItemData.useLocations`; Potion разрешён в мире и бою, но в бою отдельной вкладки Items нет — предмет применяется из инвентаря вне боя. Изменяемое количество хранится в `InventoryState`, не в ScriptableObject.

## Добавление данных

- `Create → FalseMemory → Item` — обычный предмет; для расходуемого используйте `Battle Consumable`.
- `Create → FalseMemory → Battle Spell` — заклинание с поддерживаемым эффектом; назначьте его в `BattleUnit.spells`.
- `Create → FalseMemory → Battle Rules` — общие коэффициенты боя.

Новые категории предметов, слоты экипировки или типы эффектов требуют изменения кода; числа существующих эффектов настраиваются в ассетах.

## Unity MCP

Выбирайте открытый экземпляр FalseMemory, читайте editor state и Console. Если Editor компилирует, дождитесь завершения перед назначением компонентов. Ошибка соединения означает недоступность инструмента, а не успешное выполнение. После изменений сцену нужно явно сохранить; чтение сцены не равно сохранению.

Подробнее: [архитектура](../Architecture.md), [документация компонентов](../README.md#описания-скриптов), [тесты](TestingGuide.md), [состав сцен](../Systems/SceneComposition.md).

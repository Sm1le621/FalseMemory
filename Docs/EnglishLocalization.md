# FalseMemory — завершение английской локализации

> **Исторический документ.** Локализация полностью удалена 2026-10-31: игровой текст хранится английскими строками в сценах. Актуальная картина — [README](README.md), [Architecture](Architecture.md), [SceneComposition](Systems/SceneComposition.md).

Дата: 2026-10-12. Статус: **Tested**, визуальная приёмка владельцем Pending. Все проверки ниже относятся к текущей рабочей копии после возобновления, если явно не указано другое.

## Восстановленная точка остановки

Прочитаны AGENTS.md, README, Roadmap, Architecture, Changelog, исходники и Git diff/log/status. Исходный HEAD — `be6f3d1 feat: redesign inventory with character equipment and rarity tooltips`, ветка `stas_megamosk_build`. После остановки остались таблицы/локализованные контроллеры/сцены/предметы/новые тесты без коммита. Они продолжены; перевод заново не выполнялся, чужие изменения не откатывались.

Этапы 1–3 реализованы; подэтапы 4.1–4.5 и автоматическая часть 4.6 проверены. Переработка инвентаря по референсу уже находилась в HEAD. Обязательная ручная приёмка этапа 4 и нового оформления не подтверждена. Этапы 5–10 не начаты; имеющееся накопление EXP ещё не означает level-up/классы.

Unity MCP подключён к FalseMemory на Unity 6000.6.2f1. После запуска ПК встроенный канал не инициализировался; использован native MCP-клиент к существующему локальному серверу. Сам сервер/настройки подключения не менялись. Действия со сценами, Console, объектами, Play Mode и тестами выполнены через MCP.

## Что завершено

- Установлен Unity Localization 1.5.13; создана native English Locale и центральная StringTable **136 ключей** с SharedTableData. Все существующие игровые сообщения/подписи переведены на en-US.
- MainMenu: New Game, Settings, Exit; Settings: Volume, Fullscreen: On/Off, Back. Инвентарь: Inventory, Character & Equipment, Bag, Attributes, восемь слотов, Use/Equip/Split/Close, характеристики/уровень/EXP/Gold и подсказки управления.
- Имена/описания семи предметов, редкости, эффекты, требования уровня и цены; подсказки E и сообщения подбора/полного запаса/использования/экипировки.
- BattleScene: Attack/Magic/Items/Defend/Flee, Fire/Heal, стоимость MP/количество предметов, статусы/раунд/урон/промахи/защита/побег, Victory/Defeat/EXP и команды выхода.
- AGENTS.md закрепляет английский игровой текст и русские технические документы/отчёты; идентификаторы и GUID сохраняются.
- Исправлена найденная при просмотре настоящих кадров проблема PixelUi: extraPadding bitmap TMP-шрифта захватывал соседние буквы атласа. Отключён только этот отступ; исходные шрифты, палитра и геометрия сохранены.

NPC, диалоги, квесты, торговля и дисковые сохранения отсутствуют. Их перевод не заявляется. Русские комментарии/Inspector-заголовки и технические сообщения Console не являются игровым интерфейсом и специально не переписывались.

## Скрипты и данные

Созданы пять собственных C#-файлов:

- `GameText` (удалён) — чтение native таблиц, форматирование параметров и английский fallback.
- `LocalizedLabel` (удалён) — статическая TMP-подпись по ключу, обновление/отписка.
- `GameTextEditor` (удалён) — Unity → FalseMemory → Localization → English Text, поиск/редактирование/добавление устойчивых ключей и Save table.
- `LocalizationTests` (удалены) — таблица, форматы, ключи, fallback, предметные подписи.
- `LocalizationFlowTests` (удалены) — меню/Settings/инвентарь/tooltip/Magic/Items/настоящая победа и английские TMP-подписи.

Изменён 21 существующий файл:

- BattleHUD, BattleInventory, BattleManager, BattleSpell, BattleUI, BattleUnit — тексты/локализованные имена и новый localizationId участника/заклинания.
- EquipmentController, InventoryWindow.Layout, InventoryWindow, ItemData, ItemPresentation, WorldItemPickup, WorldItemUseController — локализованные подписи/уведомления/описания при прежней модели данных.
- MainMenuBootstrap, SettingsMenu, PixelUi — английские подписи/привязки и исправление padding.
- ItemPresentationTests, BattleFlowTests, BattlePresentationFlowTests, BattleStage2FlowTests, InventoryFlowTests — прежние русские ожидания заменены английскими без ослабления сценариев.

Runtime/EditMode asmdef подключают Unity.Localization. Packages/manifest и lock фиксируют пакет и его зависимости. Пакет создал AddressableAssetsData/группу Locale и регистрацию Addressables в EditorBuildSettings; порядок трёх игровых сцен сохранён.

В существующих SampleScene/BattleScene через MCP переведены сохранённые TMP-подписи, добавлены LocalizedLabel и ссылки unit.player/unit.skeleton. Fire/Heal получили ключи spell.fire/spell.heal; предметные поля переведены без изменения ID, GUID, эффектов, цен и бонусов. MainMenu UI создаётся существующим Bootstrap в Play Mode; отдельная сцена/альтернативные системы не создавались. Health.cs, EnemyAI.cs и формулы/очередь боя не менялись.

Таблицы находятся в Assets/Resources/Localization; Locale — Assets/Localization/Locales. `LocalizationSystem` (удалена) объясняет изменение текста без программирования. Текущий GameText синхронно читает native StringTables из Resources; глобальные LocalizationSettings, Smart Strings, plurals и меню выбора языка не подключены. Подготовлен только en-US.

## Реально выполненные проверки

Компиляция Unity завершена; новые тесты используют актуальный FindObjectsByType без устаревшей сортировки. Последнее чтение Console после проверок: **0 ошибок, 0 предупреждений**. Ранее наблюдалось исходное служебное предупреждение Account API did not become accessible within 30 seconds; его исправление не относится к игровой локализации. Console вручную не очищалась.

- **Edit Mode: 75/75 Passed**, 0 failed/skipped, 0.7175259 секунды; job `56a3d9392d0f4d51a3de47b7f23de3f2`.
- **Play Mode: 38/38 Passed**, 0 failed/skipped, 28.1597521 секунды; job `4ca5a6f4321d4b508497a55b8267ef4a`.
- Проверены реальная таблица/136 уникальных непустых английских записей/форматы, предметные подписи, безопасные отсутствующие ключи и неподдержанные языки. Поиск литеральных GameText-ключей в runtime/Editor не нашёл отсутствующих записей.
- Сценарные тесты проверяют New Game/Settings/Back, инвентарь/tooltip, бой/Magic/Items/исходы; прежние тесты также проверяют ввод/drag/gate/ресурсы/экипировку/возврат. В новом тесте меню кнопки вызываются через onClick; это не отдельный физический клик мышью.
- До остановки один Play-прогон дал 34 pass/4 fail из-за русских строк в старых assertions. Ожидания исправлены; итоговый свежий прогон выше проходит. Прерванный запуск не считается результатом.
- Через MCP валидированы реально открытые SampleScene и BattleScene: 0 Missing Script/сломанных Prefab/проблем. Проверены ссылки Health HUD, 30 inventory slots, 8 equipment buttons/tooltip, BattleManager→BattleUI/5 команд/results/HUD и локализованные ключи.
- Сравнение YAML с исходным HEAD: SampleScene 850→858 сериализованных объектов, BattleScene 229→230; ни один прежний fileID не исчез, дублирующихся fileID нет. Для всех 64 собственных C# имеются документы с девятью разделами и необходимые .meta.

Тесты проверяют игровые сценарии в Editor, не полноценную standalone-сборку и не художественную/языковую приёмку владельцем.

## Проверка отображения

**32 сочетания**: 8 состояний × 1280×720, 1920×1080, 2560×1440, 3440×1440. Состояния: MainMenu, Settings, inventory с Wooden Staff tooltip, главные боевые команды, Magic, Items, Victory, Defeat. У активных UI проверены экранные углы Graphic, isTextOverflowing, отсутствие кириллицы и наличие фактических glyphs. Все сочетания прошли; реальные кадры просмотрены для восьми Full HD состояний и широкого Magic.

Меню запускалось заново при каждом разрешении: его существующий scaler вычисляет целый масштаб при создании Canvas. Инвентарь/бой используют сохранённый Scale With Screen Size. Изменение размера меню посреди текущего Play всё ещё не пересчитывает первоначальный масштаб.

Для кадров временно добавлены предметы/экипировка и Mouse Input System; настройки ввода/устройство возвращены после inventory. Победа получена настоящей Attack после подготовки HP врага=1: Enemy HP0, Player HP100, EXP20. Поражение получено в новом Play после подготовки Player HP=1 и ответного хода врага: Player HP0, Enemy HP50. Результаты не подменялись ручным текстом. Эти данные не сохранялись в сцены/ассеты. Полное прохождение результата/возврата дополнительно покрыто тестами.

После проверки Play Mode остановлен, активна чистая SampleScene, runInBackground=false, исходные семь Game View presets и выбранный индекс0 восстановлены; удалены только два временных размера этой проверки.

[Полные метрики и отображавшиеся тексты](Verification/EnglishLocalizationLayout.json). Все 32 исходных кадра находятся во временной Temp/EnglishResume; восемь Full HD кадров сохранены в Docs/Images:

- [Главное меню](Images/EnglishLocalization-menu-1920.png), [Settings](Images/EnglishLocalization-settings-1920.png).
- [Inventory](Images/EnglishLocalization-inventory-1920.png).
- [Команды боя](Images/EnglishLocalization-battle-main-1920.png), [Magic](Images/EnglishLocalization-battle-magic-1920.png), [Items](Images/EnglishLocalization-battle-items-1920.png).
- [Victory](Images/EnglishLocalization-battle-victory-1920.png), [Defeat](Images/EnglishLocalization-battle-defeat-1920.png).

## Документация и Git

Обновлены отдельные описания всех 26 созданных/изменённых скриптов, LocalizationSystem, Inventory/Equipment/BattleUI/Menu/Magic/Interaction, README, Architecture, Changelog, Roadmap, UnitySetup, TestingGuide и Testing. Техническая документация остаётся русской; исторические результаты отделены от текущих.

Задание сохраняется одним локальным коммитом `feat: implement English localization` на stas_megamosk_build после проверок: 140 файлов. Точный hash сообщается в итоговом отчёте и Git log. Включаются только относящиеся к заданию исходники/мета/таблицы/сцены/зависимости/Docs/AGENTS. Существовавшее до локализации изменение Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset сохранено в рабочей копии и исключается из коммита. Library/Temp/Logs и секреты не включаются. **Push не выполняется.**

## Что проверить владельцу и следующий шаг

1. Открыть MainMenu, выбрать нужное разрешение Game View до Play. Проверить New Game/Settings/Volume/Fullscreen/Back, читаемость букв.
2. На карте: E подобрать предметы; I открыть Inventory, навести мышь на Potion/оружие, проверить английские имя/описание/бонусы/цены. Перенести/разделить предметы, надеть/снять экипировку, закрыть Escape; HP/MP/управление не должны ломаться.
3. Коснуться скелета: проверить Attack/Magic/Items/Defend/Flee, Fire/Heal/стоимость и возврат Back/Esc. Победить, проверить Victory/EXP/Return to Map и сохранение текущих ресурсов.
4. В новом Play только во время игры поставить Player→Health Current Health=1; начать бой и выбрать Attack. Проверить Defeat/New Game/Main Menu.
5. Повторить визуальный просмотр хотя бы в 1920×1080 и 3440×1440; для меню перезапустить Play после смены разрешения. Если нужно изменить формулировку — английская таблица через FalseMemory/Localization/English Text. Добавление/сохранение текста через GUI этого EditorWindow отдельно интерактивно не проверено.

После успешной ручной проверки сообщить, что этап 4 и новый UI приняты. [MasterDevelopmentPlan](MasterDevelopmentPlan.md) прямо требует: «Не переходи к Этапу 5, пока обязательная ручная проверка Этапа 4 не подтверждена мной». Поэтому следующий программный подэтап **5.1: кривая EXP, level-up и рост характеристик** начинается после этой приёмки; текущая проверенная точка сохранена без повторной реализации этапов.

2026-10-25 — `PixelUi` удалён: шрифт меню теперь `Assets/Settings/UI/PixelFont.asset` (PressStart2P, dynamic, Point), подписи — `LocalizedLabel` в сцене. Историческая запись выше описывает прежнюю реализацию и сохраняется как история.

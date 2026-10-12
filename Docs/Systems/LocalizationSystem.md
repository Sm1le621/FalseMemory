# Локализация FalseMemory

## Актуально2026-10-13

Текущая английская StringTable189 keys. Добавлены Off-Hand/Inventory is full./IronShield name-description/float bonus format/Requires both hands. Все статические идинамические пользовательские текстычерезGameText.12 UIcases безрусского/пропавшихglyphs; [отчёт](../PlayerRework.md).

## Дополнение2026-10-12

Player rework4: английская StringTable179 keys (141+38), включая10 name/abbr/description, equipment suffix, source tooltip иcompact derived summary. Dynamic numbers по-прежнему через GameText. Новые CHA/stamina/resistance descriptions честно указываютplanned mechanics.

## Назначение и языковые правила

С2026-10-12 весь игровой пользовательский текст — английский en-US. Docs и отчёты владельцу — русские. Правила закреплены в AGENTS.md. Не меняются технические ID/GUID/имена сцен/serialized-поля ради перевода. Подготовлен только English; Ukrainian/Polish/Russian/German/French/Spanish не переведены.

Установлен Unity Localization1.5.13, совместимый с Unity6. [Официальная совместимость](https://docs.unity.com/en-us/engine/6000.0/manual/packages-list/packages-all/pack-safe/com-unity-localization). Используются native Unity StringTable/SharedTableData; игровое чтение через компактный GameText, а не новый глобальный MonoBehaviour-менеджер. Существующие игровые системы сохранены.

## Где находятся строки

- Assets/Resources/Localization/GameText_en-US.asset — единственная центральная английская таблица,141 запись после5.1 (136 в исходной локализации).
- GameText Shared Data.asset рядом — общие устойчивые ключи/ID. Не менять numeric ID вручную.
- Assets/Localization/Locales/English (en-US).asset — Locale Unity.
- Assets/Editor/GameTextEditor.cs — удобное окно изменения таблицы.

Resources включают таблицу и SharedData в игру. GameText синхронно загружает native таблицу и форматирует Value с CultureInfo языка через string.Format. Отсутствующий ключ → [key], неверные аргументы → [invalid: key], неизвестный язык отклоняется без смены текущего. Английский fallback применяется при неполном будущем переводе; временный ItemData без ключа использует старое английское поле. LocaleCode/LanguageChanged принадлежат GameText. Глобальные Unity LocalizationSettings/SelectedLocale не управляют этим первым вариантом; задержка инициализации Addressables не влияет на первый текст.

Пакет автоматически создал AddressableAssetsData/Localization-Locales и зарегистрировал Addressables в EditorBuildSettings; существующий список сцен не изменён. Таблицы читаются через Resources, сборка Addressables-контента для текущего текста не нужна. Стандартные LocalizeStringEvent/Smart Strings в игровых объектах не подключены; не смешивать двух владельцев выбора языка.

## Как изменить и добавить текст без программирования

1. Unity → FalseMemory → Localization → English Text.
2. Search находит ключ или значение. Измените английское значение, затем Save table. Undo работает.
3. Новый текст: New stable key, например inventory.example; English value; Add entry; Save table. Ключ уникальный, строчные буквы/цифры/точки/подчёркивание/дефис. Кириллица отклоняется, пустое новое значение запрещено.
4. Изменение текста не означает изменение ключа. Ключ остаётся постоянным во всех языках и коде.

Альтернатива — native Inspector таблицы; лучше пользоваться этим окном, чтобы не редактировать serialized ID. Пакетные файлы Library/PackageCache менять не нужно.

## Ключи и динамические значения

GameText.Get("inventory.use") возвращает Use. GameText.Get("resources.hp_mp",hp,maxHP,mp,maxMP) подставляет значения вместо {0}…{3}. Поддержаны обычные .NET шаблоны и культура языка. Аргументы обязательно соответствуют номерам в таблице. Буквальные скобки записываются {{/}}. Для отсутствующих полей tooltip строка вообще не добавляется; текст не содержит пустых значений.

Для новой механики добавьте стабильный ключ заранее, используйте GameText.Get с параметрами и проверьте английский интерфейс. Не добавляйте новые жёсткие пользовательские сообщения в контроллеры. Пробелы/числа/символы оформления могут остаться локальными; английские сообщения/заголовки должны быть в таблице.

## UI, предметы и будущие диалоги

Статический TMP объект: LocalizedLabel с key, либо LocalizedLabel.Bind(text,key) при создании. Он обновляется OnEnable/LanguageChanged и правильно отписывается. Существующие шрифты/размеры/цвета сохранены. Динамические строки HPMP/EXP/бой/количество создают прежние игровые контроллеры через GameText; не ставить статический binding на такое поле.

ItemData.LocalizedName/LocalizedDescription используют id+.name/.description, например item.iron_sword.name. Старые английские displayName/description сохранены как fallback; таблица — перевод. BattleUnit/BattleSpell имеют localizationId (unit.player/unit.skeleton, spell.fire/spell.heal) и LocalizedName. Эти новые ключи не заменяют прежние ID/serialized-поля. Spell description подготовлены как текстовые записи; отдельного окна описания магии пока нет.

NPC/диалогов/квестов/торговли/дисковых сохранений ещё нет. Когда они появятся, текст хранить в той же таблице, например npc.<stable-id>.line.<stable-id>, quest.<id>.title; отдельные ID сюжета не переименовывать. Не описывать отсутствующий контент как переведённый или проверенный.

## Как подготовить другой язык позже

Создать новую native StringTable с тем же SharedData и LocaleIdentifier, сохранить в Resources/Localization/GameText_<code>.asset, например GameText_uk-UA. Создать Locale-ассет соответствующего языка. Все существующие ключи/ID сохранить, перевести только Value; проверить строки/шрифты/размеры. GameText.TrySetLocale(code) загружает только существующую таблицу, иначе оставляетEnglish. Недостающий ключ берётся изEnglish.

Сейчас второго языка нет. Полная горячая смена уже показанных динамических боевых сообщений, выбор языка в Settings, расширенные переносы/шрифты других алфавитов и стандартная Addressables/LocalizationSettings-интеграция — отдельное расширение. Статические LocalizedLabel уже подписаны, но не следует утверждать, что все будущие языки готовые.

## Ошибки и ограничения

Проверяйте пустые значения/дубликаты/случайную кириллицу/неверный ключ/формат. Для первой версии используются обычные шаблоны, Smart Strings/plurals не поддержаны игровым фасадом. Неверный шаблон безопасно выдаёт маркер, но перед релизом его исправить. Пакет не выполняет машинный перевод. Английский смысл/стиль и финальные иконки принимает владелец. Standalone-сборка отдельно не проверена.

## Проверки

2026-10-12 —5.1 добавил5 устойчивых ключей: battle.victory_level_up_result, character.level_up/header_progress/max_level/next_experience. Значения English, параметры через GameText; старые ключи/ID сохранены. [Текущие проверки](../ProgressionStage5.md).

Свежие результаты, промежуточные падения старых русских ожиданий, визуальные размеры и ручная приёмка — [EnglishLocalization](../EnglishLocalization.md). Исторические результаты инвентаря остаются в InventoryRedesign.md. После перевода требуется ручная визуальная проверка владельцем; этап4/переход к5 не объявляются Completed автоматически.

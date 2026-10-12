# GameText

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Localization/GameText.cs.

Общий доступ к пользовательскому тексту. Читает native Unity StringTable через Resources без ожидания Addressables. Английский en-US — единственный подготовленный язык.

## 2. Как работает

Ключ ищется в выбранной таблице, затем английской. Шаблон форматируется с культурой выбранного языка. Нет копии словаря строк в C#; одна сохранённая английская таблица — источник истины.

## 3. Методы и функции

`Reset()` RuntimeInitializeOnLoadMethod SubsystemRegistration очищает кеш/подписки и возвращает en-US при новом запуске. `English` private property загружает Resources/Localization/GameText_en-US. `Find(key)` private → StringTableEntry/null: текущая таблица, затем English; пустой ключ null. `Contains(key)` → bool наличия записи.

`Get(key,params object[] args)` → string: string.Format(culture,entry.Value,args), неизвестный ключ [key], неверное форматирование [invalid: key] без исключения. Даже отсутствие аргументов для {0} выявляется. `Content(key,englishFallback)` → Get при наличии либо английский fallback/пустая строка. `TrySetLocale(code)` → bool: валидная CultureInfo+существующий Resources/GameText_code; отказ сохраняет прежний язык, успех обновляет кеш/LocaleCode и вызывает LanguageChanged.

## 4. Переменные и Inspector

EnglishLocale const en-US; LocaleCode readonly снаружи; LanguageChanged Action. private english/selected StringTable и culture CultureInfo. На GameObject не хранится и через Inspector не настраивается.

## 5. Связи

Unity Localization1.5.13, Resources native StringTable/SharedTableData, LocalizedLabel, игровые окна, ItemData/BattleUnit/BattleSpell. Главная инструкция [LocalizationSystem](../../Systems/LocalizationSystem.md).

## 6. Пример

GameText.Get("resources.hp_mp",90,100,5,30) → HP90/100 MP5/30 по шаблону. GameText.Content(item.id+".name",item.displayName) читает имя, не меняет ID.

## 7. Настройка в Unity

Статический класс, не прикрепляется. Table GameText_en-US.asset в Assets/Resources/Localization, SharedData рядом; ключи регистрозависимые. Locale-ассет подготовлен Assets/Localization/Locales. Чтобы добавить другой язык, нужна таблица с теми же shared-ключами и именем GameText_<code>.

## 8. Возможные проблемы

Первый вариант использует обычные {0}/{1} шаблоны, не Smart Strings/plurals. Unity LocalizationSettings/SelectedLocale и LocalizeStringEvent не являются владельцами выбора языка: его явно задаёт GameText. Текущий UI готов только дляEnglish; горячая смена всех динамических уже показанных сообщений потребует отдельной работы. Маркер [key] — безопасный диагностический текст, не хороший финальный контент: перед релизом устранить пропущенный ключ.

## 9. История изменений

2026-10-12 — централизованный английский текст, fallback/форматирование/событие и сброс между сессиями.

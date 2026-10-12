# LocalizationFlowTests

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/LocalizationFlowTests.cs.

Два сценария английского UI в настоящих меню, инвентаре и BattleScene.

## 2. Как работает

Partial BattleFlowTests использует общий Setup/Cleanup с настоящими сценами и исходными ресурсами. Не сохраняет сценовые и тестовые значения.

## 3. Методы и функции

MenuAndSettingsUseEnglishAndTheirButtonsStillWork также проверяет extraPadding=false у меню: растровые буквы не должны захватывать соседей атласа. Реальное качество изображения отдельно проверяется снимками, а не только этим флагом.

`AssertEnglishActiveUi()` private void перебирает активные TMP_Text и проверяет отсутствие кириллицы/основных missing-key маркеров. `MenuAndSettingsUseEnglishAndTheirButtonsStillWork()` IEnumerator загружает MainMenu Single, проверяет New Game/Settings, открывает Settings кнопкой/Volume, закрывает Back, запускает новую игру, проверяет Potion3. `InventoryTooltipMagicItemsAndBattleResultsUseEnglish()` IEnumerator открывает окно/подсказкуPotion, проверяет эффект/цену, начинает бой, открывает Magic/Items, затем реальной Attack убивает предварительно ослабленного врага, проверяет Victory/+20EXP и английские TMP. UI-button тест вызывает onClick, не имитирует физическую мышь; прежние hover/drag тесты отдельно используютInputSystem.

## 4. Переменные и Inspector

Временные Player/Enemy/Window/Manager из сцен. Новый словарь состояния боя не создаётся. Inspector отсутствует.

## 5. Связи

BattleFlowTests lifecycle, GameText/LocalizedLabel, MainMenuBootstrap, Unity SceneManager/UI/TMP и боевые компоненты.

## 6. Пример

MainMenu → Settings → Back → New Game оставляет игроку начальные3Potion и английский UI, затем другой сценарий подтверждает английские Magic/Items/Victory.

## 7. Настройка в Unity

Не прикрепляется. PlayMode/FalseMemory.Battle.PlayModeTests. Cleanup восстанавливает тестовые правила/сцены/Input System как раньше.

## 8. Возможные проблемы

Нет NPC/квестов/торговли/сохранений для проверки. Не меняет fullscreen/Volume ради текста. Художественная оценка и standalone отдельны.

## 9. История изменений

2026-10-12 — после возобновления добавлена проверка безопасного padding пиксельного меню.

2026-10-12 — поиск TMP использует актуальный FindObjectsByType без устаревшего SortMode; проверки не зависят от порядка объектов.

2026-10-12 — английские меню/подсказки/бой и новая игра.

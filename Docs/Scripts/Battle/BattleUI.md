# BattleUI

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleUI.cs`.

Интерфейс боя: сообщение, характеристики участников, номер раунда, портреты, меню команд, всплывающие числа попаданий и окно результата. Компонент только отображает данные и передаёт выбранную команду через callbacks; он не меняет HP/MP.

## 2. Как работает

`Awake` отключает у `InputSystemUIInputModule` обработку move/submit/cancel, чтобы клавиатура не срабатывала дважды, и подписывает кнопки на `Invoke`. `Update` вручную читает стрелки/WASD, Enter и Escape, пока меню активно. `ShowMenu` сохраняет подписи, действия, доступность и необязательный обработчик возврата, сбрасывает выбор и блокирует приём на один кадр.

`RenderMenu` активирует только нужные кнопки, ставит маркер `>` у выбранной строки, задаёт цвет и `interactable`. Если назначен `BattleHUD`, оформление делегируется ему. `ShowUnits` применяет портреты и сразу обновляет характеристики. `RefreshStats` выводит HP/MP, ATK/DEF/SPD, содержимое инвентаря и состояние защиты. Победа и поражение показывают отдельное окно результата.

## 3. Методы и функции

- `Awake()` — Unity callback; настраивает EventSystem и listeners кнопок.
- `Update()` — читает клавиатуру: стрелки/WASD меняют выбор, Enter вызывает, Escape выполняет `CancelMenu`.
- `Invoke(int index)` — проверяет включённость меню, диапазон, доступность и защиту от повторного кадра; проигрывает звук подтверждения и вызывает callback.
- `ShowMenu(string[] names, Action[] actions, bool[] enabledOptions = null, Action onBack = null)` — проверяет совпадение длин массивов, сохраняет меню, включает ввод, обновляет `BattleHUD`; void.
- `CancelMenu()` → `bool`; вызывает `backAction`, если он есть и ввод разрешён.
- `Select(int index)` — меняет выбранную строку, корректирует прокрутку и вызывает `RenderMenu`.
- `RenderMenu()` — обновляет видимость, текст, цвет, доступность кнопок и значки через `BattleHUD.StyleCommand`.
- `SetCommandsEnabled(bool enabled)` — включает/выключает приём команд и перерисовывает меню.
- `ShowFeedback(bool onPlayer, string text, Color color, bool showImpact = true)` — размещает вспышку и число над портретом цели; при `MISS` вспышка не показывается.
- `Feedback(Color color, bool showImpact)` — coroutine подъёма и затухания текста.
- `ShowMessage(string message)` — обновляет строку сообщения.
- `ShowVictoryResults(BattleUnit player)` / `ShowDefeatResults()` — включают панель результата; тексты английские, без опыта и уровней.
- `ShowUnits(BattleUnit player, BattleUnit enemy)` — применяет портреты, вызывает `RefreshStats` и `BattleHUD.Bind`.
- `ApplyPortrait`/`PointSprite` — назначают спрайт, цвет, отражение и при необходимости создают Point-копию текстуры через GPU.
- `OnDestroy()` — освобождает временные спрайты и текстуры.
- `RefreshStats(BattleUnit player, BattleUnit enemy, int round)` — обновляет оба блока характеристик и номер раунда.
- `Stats(BattleUnit unit)` — private static; собирает строку `HP`, `MP`, `ATK/DEF/SPD`, инвентарь и `DEFENDING`.

## 4. Переменные и Inspector

- `messageText`, `playerStats`, `enemyStats`, `roundText` — обязательные TMP-поля.
- `playerPortrait`, `enemyPortrait` — изображения участников.
- `commandButtons`, `commandLabels` — массивы одинаковой длины; в сцене пять кнопок, но одновременно используется столько строк, сколько передано в меню.
- `feedbackGlow`, `impactSprite`, `feedbackText`, `feedbackDuration` — эффект попадания; `feedbackDuration ≥ 0.1`, по умолчанию 0.8 с.
- `resultPanel`, `resultText` — окно победы/поражения.
- `hud` — необязательный `BattleHUD`; при наличии отвечает за оформление и анимации.
- `audioController` — необязательный источник звуков меню и подтверждения.
- `previewPlayerSprite`, `previewEnemySprite` — исходные спрайты для редактора/предпросмотра.
- `CommandsEnabled`, `MenuCount`, `SelectedIndex` — только чтение.

## 5. Связи

Компонент находится на `BattleManager` в `BattleScene`. Использует `BattleUnit`, `Health`, `BattleInventory`, `BattleHUD`, `BattleAudioController` и TMP/uGUI/Input System. `BattleManager` вызывает меню, сообщения и обновление; `BattlePresentation` двигает портреты, но не меняет боевые значения. `BattleHUD` читает те же HP/MP.

## 6. Пример

В ход игрока отображается меню `Attack / Magic / Defend / Flee`. Выбор `Magic` открывает список заклинаний с ценой MP и пунктом `Back`; Escape возвращает в главное меню без расхода хода. После попадания над портретом врага поднимается `−19`, а строка сообщения показывает имя атакующего и урон.

## 7. Настройка в Unity

Назначь TMP-поля, оба портрета, пять кнопок и пять подписей, эффект попадания и панель результата. Canvas должен использовать Scale With Screen Size 960×540. `BattleHUD` и `BattlePresentation` назначаются на том же объекте или отдельно, как в текущей сцене. Список команд формируется кодом, поэтому добавлять кнопку Items не нужно.

## 8. Возможные проблемы

Несовпадение длин `commandButtons` и `commandLabels` даёт ошибку. Без `messageText`, портретов или панели результата соответствующие вызовы не сработают. Клавиатура обрабатывается только при фокусе Game View; геймпад не поддерживается. Прокрутка рассчитана на число видимых кнопок. Вспышка не должна использоваться как источник урона: числа приходят из `BattleCalculator`.

## 9. История изменений

- 2026-10-31 — удалены Items и Heal, характеристики без LV/EXP; результат победы не содержит опыта; меню сокращено до четырёх команд.
- 2026-10-12 — английские строки и пиксельные эффекты; запись историческая, `GameText` удалён.
- 2026-10-11 — оформление HUD, всплывающие числа и окно результата.

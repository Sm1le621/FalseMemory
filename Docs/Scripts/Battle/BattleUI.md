# BattleUI

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleUI.cs.

Существующий интерфейс боя: сообщения, характеристики, меню, портреты, цветные эффекты и результаты.

## 2. Как работает

Awake подключает мышь и отключает дублирующий keyboard Submit модуля UI. Update вручную обрабатывает отдельные нажатия стрелок/WASD/Enter/Escape. ShowMenu назначает подписи/callbacks, прокрутка использует пять сохранённых кнопок. Manager обновляет характеристики и сообщения.

## 3. Методы и функции

- `Awake()` — Unity: на текущем EventSystem делает move/submit/cancel=null, подключает listeners кнопок к Invoke.
- `Update()` — кадр: при активном меню и после acceptAfterFrame клавиатура вызывает Select/Invoke/CancelMenu; удержание Enter не повторяет ход.
- `Invoke(int index)` — внутренний void: проверяет доступность/диапазон/кадр и вызывает callback.
- `ShowMenu(string[] names,Action[] actions,bool[] enabledOptions=null,Action onBack=null)` — проверяет равную длину, сбрасывает выбор/scroll, задерживает приём на кадр, включает меню; void.
- `CancelMenu()` → bool; при разрешённом возврате вызывает backAction, иначе false.
- `Select(int index)` — меняет выбор/firstVisible и RenderMenu; void.
- `RenderMenu()` — активирует нужные кнопки, обновляет подписи, цвета и interactable.
- `SetCommandsEnabled(bool enabled)` — состояние меню и RenderMenu.
- `ShowFeedback(bool onPlayer,string text,Color color)` — привязывает эффект к портрету, заменяет прежнюю coroutine Feedback.
- `Feedback(Color color)` — coroutine синусной вспышки/временного текста; в конце скрывает.
- `ShowMessage(string message)` — обновляет messageText; `ShowVictoryResults(int gained,BattleUnit player)` — включает окно и четыре строки результата; void.
- `ShowUnits(BattleUnit player,BattleUnit enemy)` — ApplyPortrait и RefreshStats.
- `ApplyPortrait(Image image,BattleUnit unit)` — sprite/color/flip. `PointSprite(Sprite source)` → исходный или временный Point-спрайт, копируя непрочитанную текстуру через GPU.
- `OnDestroy()` — Unity: освобождает временные текстуры/спрайты pointSprites.
- `RefreshStats(BattleUnit player,BattleUnit enemy,int round)` — тексты/раунд/тусклый портрет погибшего; `Stats(BattleUnit unit)` → строка HP/MP/характеристик/инвентаря/EXP/Defend.
- Только Editor: `BuildLayout()` создаёт базовое оформление один раз (при messageText!=null исключение); `UpgradeLayout()` расширяет сохранённые объекты/ссылки для этапа 2, помечает компонент dirty, но не сохраняет сцену.
- Editor-помощники: `Fit(RectTransform rect,Vector2 min,Vector2 max)` задаёт якоря; `Panel(Transform parent,string name,Color color,Vector2 min,Vector2 max)` → Image; `Portrait(...,Sprite sprite,...)` → Image; `Label(Transform parent,string name,string value,int size,Vector2 min,Vector2 max)` → TMP. Только создание в Editor, не игровой цикл.

## 4. Переменные и параметры Inspector

Обязательны messageText/playerStats/enemyStats/roundText, playerPortrait/enemyPortrait, равные массивы commandButtons/commandLabels (в сцене 5). feedbackGlow/feedbackText — ссылки эффектов; feedbackDuration≥0.1, код 0.45 сек. resultPanel/resultText — победа. previewPlayerSprite/previewEnemySprite — начальное оформление Editor. CommandsEnabled/MenuCount/SelectedIndex — чтение. Canvas ScaleWithScreenSize960×540, match0.5; меню и текст имеют якоря/авторазмер.

## 5. Связи с другими системами

Компонент на BattleManager в BattleScene; Manager назначает callbacks и обновления. Требуются TMP/uGUI/Input System/EventSystem. Source-спрайты не меняются; их временные копии освобождаются.

## 6. Пример работы

Magic → Fire 8 MP и Heal6 MP → Escape возвращает Attack/Magic/Items/Defend/Flee без хода. Fire выводит оранжевую вспышку и число над противником.

## 7. Настройка в Unity

На BattleManager BattleScene. Назначь TMP-поля сообщений/характеристик/раунда, Image-портреты, пять Button и соответствующие пять Label, AbilityGlow/AbilityAmount и окно BattleResults/VictoryResultText. Canvas Scale With Screen Size960×540. Сохранённое оформление уже существует: BuildLayout повторно не вызывай. UpgradeLayout — только осознанная Editor-операция с последующим сохранением сцены.

## 8. Возможные проблемы и проверки

Отсутствующие основные ссылки/несогласованные массивы дают ошибки. UI listeners button.onClick в тестах проверяют обработку команд, не весь физический путь курсора. Only Enter, не Space. Gamepad нет. Длинные списки прокручиваются, выбор цели партии отсутствует. Меню, сообщения и эффекты проверены Play Mode; визуальное размещение 1920×1080/3440×1440 проверялось в задаче этапа 2, здесь заново скриншоты не делались.

## 9. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

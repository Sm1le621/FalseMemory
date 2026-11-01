# Боевой интерфейс

## 1. Назначение

Отображение боя и приём команд: сообщения, характеристики, раунд, портреты, меню Attack/Magic/Defend/Flee, полосы HP/MP, всплывающие числа, VFX и окно результата. UI не владеет боевыми ресурсами.

## 2. Общая логика

`BattleUI` показывает меню и обрабатывает мышь/клавиатуру, `BattleHUD` рисует полосы и оформление, `BattlePresentation` двигает портреты, `BattleVFXController` проигрывает эффекты, `BattleAudioController` — звуки. Все значения читаются из `BattleUnit`, `Health` и `BattleInventory`.

## 3. Основные компоненты

- `BattleUI` — тексты, портреты, меню, feedback, результат.
- `BattleHUD` — HP/MP/HP врага, LOW HP, заголовок, появление экрана.
- `BattlePresentation` — позы и перемещения портретов.
- `BattleVFXController` — частицы/кадры магии.
- `BattleAudioController` — звуковые события.
- `BattleManager` — источник команд и сообщений.

## 4. Взаимодействие скриптов

`BattleManager` вызывает `ShowMenu`, `ShowMessage`, `RefreshStats`, `ShowVictoryResults`/`ShowDefeatResults`. `BattleUI` передаёт выбранную команду через callbacks в менеджер. `BattleHUD` получает участников через `Bind`; `BattlePresentation` — через свой `Bind`. UI только отображает результат расчётов.

## 5. Последовательность событий

Вход в бой → скрытие HUD → проявление → `Your turn` + четыре команды. Выбор Magic → список заклинаний + Back. После действия → сообщение, число/VFX, обновление полос. Победа/поражение → окно результата и кнопки возврата.

## 6. Настраиваемые параметры

`revealDuration`, `barDuration`, `menuDuration`, `lowHealthThreshold`; длительности презентации и ускорение по Space; `feedbackDuration`; массивы кнопок/подписей; иконки команд. Тексты заданы английскими строками в сцене.

## 7. Использование в Unity

Все объекты сохранены в `BattleScene`. Назначь ссылки в `BattleUI`, `BattleHUD` и `BattlePresentation`. Canvas — Scale With Screen Size 960×540. Кнопок команд пять, но меню использует четыре строки; Back показывается в подменю магии.

## 8. Расширение

Новая команда требует кнопки, callback в `BattleManager` и обновления `BattleAction`/UI. Новый VFX — `BattleVFXProfile`. Новые строки добавляй прямо в TMP-компоненты сцены; `GameText` и StringTable удалены.

## 9. Ограничения и проверки

Нет геймпада, выбора цели партии и анимации IDLE. Клавиатура работает при фокусе Game View. UI не должен менять HP/MP — это проверяют PlayMode-тесты. 2026-10-31: EditMode 59/59, PlayMode 36/36; MCP не нашёл missing scripts, меню Items/Heal/LV/EXP отсутствуют. Визуальная оценка остаётся за владельцем.

## История

- 2026-10-31 — меню сокращено до четырёх команд, удалены Items/Heal/EXP; добавлена MP-полоса в мире (не в бою).
- 2026-10-11 — базовый боевой UI.

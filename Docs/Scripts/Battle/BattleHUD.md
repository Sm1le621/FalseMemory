# BattleHUD

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleHUD.cs`.

Компактный боевой HUD: полосы HP/MP игрока и HP врага, строка ресурсов, предупреждение `LOW HP`, заголовок и анимация меню, появление экрана и окна результата, оформление кнопок команд. Компонент только читает данные участников и не меняет HP/MP.

## 2. Как работает

`BattleUI.ShowUnits` вызывает `Bind(hero, foe)`. После этого каждый `Update` HUD читает актуальные `Health` и `BattleUnit`, плавно двигает полосы к целевым значениям и обновляет строку ресурсов. Полоса HP игрока краснеет при здоровье не выше `lowHealthThreshold`. `MenuChanged` переключает заголовок `COMMANDS`/`SPELLS` и запускает короткое проявление. `HideForEntrance`/`RevealScreen` управляют появлением боевого экрана, `RevealResult` — окном исхода. Ширина содержимого ограничивается 960 условными единицами и масштабируется под узкий Canvas.

## 3. Методы и функции

- `Bind(BattleUnit hero, BattleUnit foe)` — сохраняет участников, сбрасывает `initialized` и сразу вызывает `Refresh`; void.
- `Update()` — Unity callback; обновляет HUD и масштаб `content` относительно ширины родительского Canvas.
- `Refresh()` — читает HP/MP игрока, HP врага, инвентарь и защиту; вызывает `SetBar`, управляет `LOW HP` и цветом полосы HP.
- `SetBar(Slider bar, int value, int maximum)` — переводит значение в 0…1 и плавно приближает `Slider`; при первом обновлении ставит значение сразу.
- `MenuChanged(bool submenu)` — меняет заголовок и запускает `RevealMenu`; без `CanvasGroup` меню остаётся рабочим.
- `RevealMenu()` — coroutine проявления панели от alpha 0.55 до 1.
- `HideForEntrance()` / `RevealScreen(float pace)` — скрывают и проявляют `screenGroup`; темп масштабируется от `rules.actionDelay`.
- `RevealResult()` / `ResultFade()` — проявляют `resultGroup`.
- `StyleCommand(Button button, TextMeshProUGUI label, bool selected, bool enabled)` — задаёт цвета кнопки/подписи и подбирает значок по тексту команды.
- `OnDisable()` — возвращает alpha групп к 1, чтобы выключенный HUD не оставлял экран невидимым.

## 4. Переменные и Inspector

- `hpBar`, `mpBar`, `enemyHPBar` — неинтерактивные полосы.
- `resourceText` — строка `Potion xN | DEFENDING`; `lowHealthText` — предупреждение; `menuTitle` — заголовок меню.
- `content` — RectTransform содержимого; `menuGroup`, `screenGroup`, `resultGroup` — CanvasGroup для анимаций.
- `commandIcons` — пять спрайтов в порядке Attack/Magic/(резерв)/Defend/Flee; отсутствующий значок скрывается.
- `revealDuration` (0…1, по умолчанию 0.22), `barDuration` (≥0, 0.22), `menuDuration` (≥0, 0.12), `lowHealthThreshold` (0…1, 0.25).
- `player`, `enemy`, `initialized`, `menuRoutine` — внутреннее состояние.

## 5. Связи

Компонент находится на `BattleManager` вместе с `BattleUI`/`BattlePresentation`. `BattleUI` вызывает `Bind`, `MenuChanged`, `StyleCommand`, `HideForEntrance`, `RevealScreen` и `RevealResult`. Источники значений — `Health`, `BattleUnit` и `BattleInventory`. Нужны uGUI и TMP.

## 6. Пример

Игрок получает 4 урона: текст HP сразу показывает новое значение, зелёная полоса плавно уменьшается. При HP ниже 25% появляется `LOW HP`, полоса становится красной. Смена меню на список заклинаний меняет заголовок на `SPELLS`.

## 7. Настройка в Unity

Компонент уже подключён в `BattleScene`; ссылки на полосы, TMP-поля и CanvasGroup назначены. Canvas использует Scale With Screen Size 960×540, Match Height. Все Slider должны быть неинтерактивными. `commandIcons` можно оставить частично пустым.

## 8. Возможные проблемы

Отсутствующая необязательная ссылка пропускается, но без полос/текста HUD будет неполным. Анимация полосы не является источником HP. При максимуме 0 полоса пуста. Поддерживается только один игрок и один враг. `resourceText` показывает инвентарь, но не EXP — опыта в игре нет.

## 9. История изменений

- 2026-10-31 — из строки ресурсов убраны EXP/уровни; меню и значки соответствуют четырём командам без Items.
- 2026-10-12 — английские строки, пиксельные значки и плавные полосы; запись историческая.
- 2026-10-11 — базовый HUD боя.

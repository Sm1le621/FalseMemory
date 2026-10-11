# BattleHUD

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleHUD.cs. Компактные панели боя, плавные полосы ресурсов и оформление команд; не владеет HP/MP.

## 2. Как работает

BattleUI передаёт участников через Bind. Каждый кадр HUD читает их актуальные ресурсы, плавно приближает полосы к значениям и показывает LOW HP при здоровье не выше 25%. Числа отображают реальные значения сразу. Макет шириной 960 условных единиц остаётся по центру широкого экрана.

## 3. Методы и функции

- Этап 3.5: `HideForEntrance()` скрывает только screenGroup активного HUD; `RevealScreen(pace)` → IEnumerator проявляет его за revealDuration с масштабом темпа; `RevealResult()` запускает `ResultFade()` для окна исхода. `OnDisable()` возвращает alpha=1, чтобы выключенный визуальный компонент не оставлял экран невидимым.

- `Bind(hero, foe)` — назначает BattleUnit и сразу заполняет полосы; void.
- `Update()` — Unity вызывает каждый кадр: Refresh и ограничение масштаба content для узкого Canvas.
- `Refresh()` — читает HP/MP, инвентарь/EXP/защиту; вызывает SetBar, меняет предупреждение и цвет HP. Без участников ничего не делает.
- `SetBar(bar, value, maximum)` — нормализует ресурс 0..1, ограничивает значения, анимирует Slider через SetValueWithoutNotify; maximum=0 даёт пустую полосу. Игровые данные не меняет.
- `MenuChanged(submenu)` — заголовок и перезапуск RevealMenu. Отсутствие CanvasGroup не мешает выбору.
- `RevealMenu()` — короткое проявление панели от alpha 0.55 до 1; игровое действие не ожидает анимацию.
- `StyleCommand(button, label, selected, enabled)` — цвета выделения, наведения, нажатия и недоступности.
- Editor: `BuildCommandIcons(ui)` создаёт или находит CommandIcon, назначает размер 16×16 и отступ подписи; void. Не изменяет игровые значения. `commandIcons` содержит пять необязательных Sprite: Attack/Magic/Items/Defend/Flee; StyleCommand подбирает значок по команде, отсутствующий значок скрыт.
- Editor: `BuildLayout(ui)` — переносит существующие элементы в BattleContent, назначает размер/позиции/ссылки и создаёт только недостающие панели/полосы. Вызывает Place, Frame, GetPanel, GetLabel, Format и GetBar. Помечает компоненты изменёнными; сцену сохраняет вызывающий MCP.
- Editor: `Place(rect,x,y,width,height)` — положение от нижнего левого угла; `Frame(image,color)` — фон и тонкая рамка; `GetPanel(parent,name)`/`GetLabel(parent,name)` — существующий либо новый Image/TMP; `Format(label,size)` — авторазмер и отсутствие перехвата мыши; `GetBar(parent,name,color)` — Slider без управления/ручки и с заполнением.

## 4. Переменные и Inspector

commandIcons — пять Sprite по порядку Attack/Magic/Items/Defend/Flee, необязательные, назначены в сцене.
screenGroup/resultGroup — CanvasGroup содержимого/окна, назначены MCP; revealDuration=0.22 (0..1). Не влияют на HP/MP/порядок, отсутствующий CanvasGroup пропускается.

hpBar/mpBar/enemyHPBar — полосы 0..1; resourceText — Potion/EXP/Defend; lowHealthText — предупреждение; menuTitle/menuGroup — меню; content — центральный RectTransform. barDuration=0.22, menuDuration=0.12 секунды, ноль отключает плавность; lowHealthThreshold=0.25 (0..1). player/enemy/initialized/menuRoutine — внутреннее состояние, не запас ресурсов.

## 5. Связи

BattleManager в BattleScene содержит BattleUI и BattleHUD. UI вызывает Bind/MenuChanged/StyleCommand; Health, BattleUnit и BattleInventory остаются владельцами значений. Нужны uGUI и TMP.

## 6. Пример

Получено 4 урона: число HP сразу 96/100, зелёная полоса за короткое время уменьшается. При 20 HP появляется LOW HP; текущий ход не меняется.

## 7. Настройка в Unity

Компонент уже подключён к BattleManager, ссылки назначены и сохранены через MCP. Canvas: Scale With Screen Size 960×540, Match Height=1. Все три полосы не интерактивны. BuildLayout — инструмент Editor для осознанной перестройки; не вызывается в игровом цикле. Новые панели добавлять внутри BattleContent с учётом его размеров.

## 8. Ошибки и ограничения

Отсутствующий необязательный элемент пропускается; основные ссылки UI всё ещё обязательны. Анимация полосы не должна использоваться как источник HP. Оценку стиля/баланса выполняет владелец. Полоса с нулевым максимумом пуста. Нет партийного HUD.

## 9. История изменений

- 2026-10-11 — 3.7: подсказка Space добавлена в Controls; взаимодействие с ресурсами не менялось.

- 2026-10-11 — 3.6: пиксельные значки, центральные pivot портретов против смещения при flip, компактное окно результата, пиксельный шрифт команд.

- 2026-10-11 — 3.5: проявление экрана и результатов.

- 2026-10-11 — подэтап 3.1: макет, выделение команд, плавные полосы и LOW HP.

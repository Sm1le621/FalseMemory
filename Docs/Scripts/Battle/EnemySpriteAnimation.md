# EnemySpriteAnimation

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/EnemySpriteAnimation.cs`.

Проигрывает `.anim`-клипы врага в мире: ходьба, получение урона и смерть. Физика, AI и боевые расчёты остаются у `EnemyAI`, `BattleEncounter` и `BattleUnit`. В покое показывается один статичный `battleSprite`; IDLE-анимации нет.

## 2. Как работает

`Awake` получает `SpriteRenderer` и `BattleUnit`, а также Animator через `SpriteClipPlayer.GetAnimator` (Animator добавляется в игре). `OnEnable` запоминает позицию и HP, сбрасывает таймеры. В `LateUpdate` компонент сравнивает текущую позицию с предыдущей, накапливает время и следит за HP: при уменьшении включает короткую позу `Hurt`, при нуле — `Death`, иначе при движении проигрывает циклический `Move`, а в покое выключает Animator и ставит `battleSprite`. При движении влево спрайт отражается, если `faceMovement` включён.

## 3. Методы и функции

- `Awake()` — Unity callback; кеширует `SpriteRenderer`, `BattleUnit` и Animator.
- `OnEnable()` — сбрасывает предыдущую позицию/HP и таймеры.
- `LateUpdate()` — Unity callback; определяет движение, урон, смерть и обновляет спрайт.
- `SpriteClipPlayer.Sample(...)` / `Loop(...)` / `Stop(...)` — показывают клип `Hurt`/`Death`/`Move` или возвращают статичный спрайт.

## 4. Переменные и Inspector

- `movementThreshold` — минимальное смещение для анимации ходьбы (≥0, 0.001).
- `hurtDuration` — длительность позы урона (0.05…0.5, 0.16).
- `deathDuration` — длительность проигрывания смерти (0.1…1, 0.4).
- `faceMovement` — отражать спрайт по направлению движения.
- `visual`, `unit`, `animator`, `previousPosition`, `previousHP`, `clock`, `hurtUntil`, `deathStarted` — внутреннее состояние.

## 5. Связи

Требуются `SpriteRenderer` и `BattleUnit`. `BattleUnit.visualAnimations.worldController` содержит клипы `Move/Hurt/Death`; `battleSprite` — статичный fallback. Animator добавляет `SpriteClipPlayer`. Компонент не управляет `EnemyAI` и не запускает бой.

## 6. Пример

Скелет идёт к игроку — Animator крутит клип `Move`, спрайт отражается влево. После попадания на 0.16 с показывается `Hurt`. При HP 0 проигрывается `Death`; после победы объект выключается `BattleTransition`.

## 7. Настройка в Unity

Добавь компонент на Enemy со `SpriteRenderer` и `BattleUnit`. Назначь `battleSprite` и `SpriteAnimationSet` с `worldController` в `BattleUnit`. Animator вручную добавлять не нужно — его создаёт `SpriteClipPlayer`. Убедись, что кадры импортированы как Sprite 2D с Point filtering. Не добавляй IDLE-клипы — их нет в наборе.

## 8. Возможные проблемы

Без `visualAnimations` или `worldController` используется только статичный спрайт. Смерть определяется по уменьшению HP; прямое изменение спрайта другим компонентом может перезаписаться. Animator остаётся выключенным в покое, поэтому статичный спрайт не перебивается. Порог движения слишком мал для физического дрожания — при необходимости увеличь `movementThreshold`. Компонент не анимирует бой на портретах.

## 9. История изменений

- 2026-11-10 — покадровые массивы заменены `.anim`-клипами и Animator.
- 2026-10-31 — удалена IDLE-анимация; в покое используется статичный кадр; набор поз сокращён.
- 2026-10-12 — базовая анимация врага.

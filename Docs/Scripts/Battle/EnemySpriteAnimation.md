# EnemySpriteAnimation

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/EnemySpriteAnimation.cs`.

Меняет спрайт врага в мире: ходьба, получение урона и смерть. Физика, AI и боевые расчёты остаются у `EnemyAI`, `BattleEncounter` и `BattleUnit`. В покое показывается один статичный `battleSprite`; IDLE-анимации нет.

## 2. Как работает

`Awake` получает `SpriteRenderer` и `BattleUnit`. `OnEnable` запоминает позицию и HP, сбрасывает таймеры. В `LateUpdate` компонент сравнивает текущую позицию с предыдущей, накапливает время и следит за HP: при уменьшении включает короткую позу `Hurt`, при нуле — `Death`, иначе при движении проигрывает `Move`, а в покое ставит `battleSprite`. При движении влево спрайт отражается, если `faceMovement` включён.

## 3. Методы и функции

- `Awake()` — Unity callback; кеширует `SpriteRenderer` и `BattleUnit`.
- `OnEnable()` — сбрасывает предыдущую позицию/HP и таймеры.
- `LateUpdate()` — Unity callback; определяет движение, урон, смерть и обновляет спрайт.
- `visualAnimations.At(...)` / `Loop(...)` — берут кадр позы с fallback на `battleSprite`.

## 4. Переменные и Inspector

- `movementThreshold` — минимальное смещение для анимации ходьбы (≥0, 0.001).
- `hurtDuration` — длительность позы урона (0.05…0.5, 0.16).
- `deathDuration` — длительность проигрывания смерти (0.1…1, 0.4).
- `faceMovement` — отражать спрайт по направлению движения.
- `visual`, `unit`, `previousPosition`, `previousHP`, `clock`, `hurtUntil`, `deathStarted` — внутреннее состояние.

## 5. Связи

Требуются `SpriteRenderer` и `BattleUnit`. `BattleUnit.visualAnimations` содержит кадры `Move/Attack/Hurt/Death`; `battleSprite` — статичный fallback. Компонент не управляет `EnemyAI` и не запускает бой.

## 6. Пример

Скелет идёт к игроку — проигрывается `Move` и спрайт отражается влево. После попадания на 0.16 с показывается `Hurt`. При HP 0 проигрывается `Death`; после победы объект выключается `BattleTransition`.

## 7. Настройка в Unity

Добавь компонент на Enemy со `SpriteRenderer` и `BattleUnit`. Назначь `battleSprite` и `SpriteAnimationSet` в `BattleUnit`. Убедись, что кадры импортированы как Sprite 2D с Point filtering. Не добавляй IDLE-клипы — их нет в наборе.

## 8. Возможные проблемы

Без `visualAnimations` используется только статичный спрайт. Смерть определяется по уменьшению HP; прямое изменение спрайта другим компонентом может перезаписаться. Порог движения слишком мал для физического дрожания — при необходимости увеличь `movementThreshold`. Компонент не анимирует бой на портретах.

## 9. История изменений

- 2026-10-31 — удалена IDLE-анимация; в покое используется статичный кадр; набор поз сокращён.
- 2026-10-12 — базовая анимация врага.

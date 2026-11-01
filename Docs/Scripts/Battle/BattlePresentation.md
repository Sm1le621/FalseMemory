# BattlePresentation

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattlePresentation.cs`.

Визуальная постановка боя на портретах `BattleUI`: замах, выпад, реакция на удар, защита, магия, побег и завершение боя. Компонент не меняет HP, MP, инвентарь или очередь ходов; в покое показывает статичный кадр без IDLE-анимации.

## 2. Как работает

`Bind` запоминает исходные позиции, масштабы и участников. `LateUpdate` постоянно поддерживает статичные кадры и видимость щитов защиты. `Windup` проигрывает подготовку/выпад для Attack, сжатие для Defend и подсветку для Magic. `Recover` возвращает атакующего на место, проигрывает `Hurt` цели и отбрасывание. `Finish` проявляет победу/поражение/побег. `ReadPause` задаёт длительность пауз. `ResetPoses` возвращает портреты в исходное состояние.

Все тайминги масштабируются от `rules.actionDelay`; удержание Space ускоряет визуал, но не меняет расчёт боя. Если компонент выключен или ссылки отсутствуют, менеджер продолжает бой без анимации.

## 3. Методы и функции

- `Bind(BattleUnit hero, BattleUnit foe)` — сохраняет участников и домашние позиции/масштабы портретов; void.
- `LateUpdate()` — Unity callback; при отсутствии активной анимации ставит статичные кадры и обновляет щиты защиты.
- `StaticFrame(bool onPlayer)` — показывает `battleSprite` живого участника; IDLE-кадра нет.
- `PoseFrame(bool onPlayer, SpritePose pose, float progress)` — берёт кадр из `SpriteAnimationSet` с fallback на `battleSprite`.
- `ReadPause(bool enemyThinking, float pace)` — coroutine паузы; длительность ограничена 0…2 с и масштабируется темпом.
- `Windup(bool onPlayer, BattleAction action, float pace)` — coroutine подготовки действия; двигает портрет и меняет позу/цвет.
- `Recover(bool attackerIsPlayer, bool successfulHit, bool critical, float pace)` — coroutine возврата атакующего и реакции цели; увеличивает `HitEffectsPlayed` при попадании.
- `Finish(BattleState outcome, float pace)` — coroutine завершения: побег двигает портрет, смерть/поражение проявляет `Death`.
- `ResetPoses()` — сбрасывает позиции, масштабы, цвета и флаги анимации.
- `VisualDelta` — float; кадровый шаг с ускорением по Space.
- `OnDisable()` — вызывает `ResetPoses`.

## 4. Переменные и Inspector

- `ui` — ссылка на `BattleUI` с портретами.
- `playerGuard`, `enemyGuard` — необязательные изображения щитов; показываются при `IsDefending`.
- `attackDuration` (0…1, 0.4), `hitDuration` (0…1, 0.18), `outcomeDuration` (0…1, 0.35) — длительности фаз.
- `lungeDistance` (≥0, 42), `reactionDistance` (≥0, 8) — смещения в пикселях.
- `messageDuration` (0…2, 0.75), `enemyThinkDuration` (0…2, 0.5) — паузы.
- `allowFastForward` (true), `fastForwardMultiplier` (1…6, 3) — ускорение по Space.
- `HitEffectsPlayed`, `IsAnimating`, `VisualDelta` — публичное состояние только для чтения/наблюдения.
- `player`, `enemy`, `bound`, `playerAttacking`, `enemyAttacking`, домашние позиции/масштабы — внутреннее состояние.

## 5. Связи

Компонент находится в `BattleScene` рядом с `BattleUI`/`BattleManager`. `BattleManager` вызывает `Bind`, `Windup`, `Recover`, `Finish` и `ReadPause`. `BattleUI` предоставляет портреты; `BattleUnit.visualAnimations` — кадры поз; `Health`/`BattleUnit` остаются источниками HP/MP. `BattleHUD` показывает полосы, `BattleVFXController` — эффекты магии.

## 6. Пример

Игрок выбирает Attack. `Windup` сдвигает портрет вправо и проигрывает кадры Attack. После попадания `Recover` возвращает портрет, показывает `Hurt` и отбрасывание цели. Если враг погиб, `Finish` проявляет `Death`; в покое портрет снова показывает один статичный `battleSprite`.

## 7. Настройка в Unity

Назначь `ui`, при наличии — щиты. Портреты должны быть дочерними элементами Canvas с сохранёнными исходными позициями. `BattleUnit.visualAnimations` может быть пустым: тогда используется `battleSprite`. Не добавляй IDLE-клипы в `SpriteAnimationSet` — их больше нет.

## 8. Возможные проблемы

Без `ui` или портретов визуал пропускается. Смещения заданы в пикселях Canvas и могут требовать подстройки под другое разрешение. Ускорение по Space влияет только на представление. Кадры `Hurt`/`Death` должны существовать в `SpriteAnimationSet`, иначе используется fallback. Компонент не должен менять ресурсы — это проверяется тестами.

## 9. История изменений

- 2026-10-31 — удалена IDLE-анимация: в покое показывается статичный кадр; `SpritePose` сокращён до Move/Attack/Hurt/Death; удалены ссылки на лечение.
- 2026-10-12 — базовая визуальная постановка боя.

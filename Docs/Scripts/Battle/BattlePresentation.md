# BattlePresentation

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattlePresentation.cs. Боевые кадры и движения существующих портретов; HP/MP не принадлежат этому компоненту.

## 2. Как работает

Bind запоминает исходные позиции и отражение. Manager вызывает Windup перед единственным применением эффекта и Recover после него. Защита отображается, пока установлен настоящий флаг Unit.

## 3. Методы и функции

2026-10-12 — native pixel visuals: LateUpdate дополнительно проигрывает Idle живых участников, для участников, чья поза сейчас не удерживается действием. IdleFrame(onPlayer) выбирает циклический Sprite. PoseFrame(onPlayer,pose,progress) выбирает конечный кадр. Windup показывает Attack до контакта; Recover возвращает кадры Attack только для физической атаки и Hurt только для попадания; Finish показывает Death/Move и завершает IsAnimating. ResetPoses возвращает Idle и сбрасывает флаги; игровые события остаются в Manager.

- `Finish(outcome,pace)` → IEnumerator: при Victory исчезает Enemy, Defeat — Player, Escaped — Player смещается наружу и исчезает. Длительность outcomeDuration (0.35, 0..1) и Pace; не удаляет мировой объект и не выдаёт награды.

- `VisualDelta` → float: unscaledDeltaTime, при разрешённом удержании Space умноженный на fastForwardMultiplier (1..6). Не меняет Time.timeScale. `ReadPause(enemyThinking,pace)` → IEnumerator: конечная пауза 0.5/0.75 секунды по VisualDelta и Pace; вызывается Manager между действиями.
- `Bind(hero,foe)` — ссылки участников и исходные позиции/масштабы Image; void.
- `LateUpdate()` — показывает/скрывает playerGuard/enemyGuard по IsAlive/IsDefending.
- `Windup(onPlayer,action,pace)` → IEnumerator: при Attack перемещает Image на lungeDistance к противнику, при Defend слегка меняет силуэт. Отсутствующий/выключенный компонент безопасно пропускается.
- `Recover(attackerIsPlayer,successfulHit,critical,pace)` → IEnumerator: возвращает атакующего, мигает и отодвигает цель только при успешном попадании; крит усиливает реакцию. HitEffectsPlayed увеличивается только для попаданий.
- `Pace(delay)` → float: масштаб задержек относительно прежних 0.8 секунды, ограничен 0.05..3.
- `ResetPoses()` — возвращает исходные позиции/масштаб и цвета живых/погибших; сбрасывает IsAnimating. `OnDisable()` вызывает восстановление.
- Editor: `BuildGuards()` создаёт/находит два символа через `Guard(portrait,name)` → Image и назначает ссылки; помечает dirty, сцена сохраняется вызывающим MCP.

`AttackFrame(onPlayer,progress,recovering)` выбирает кадр через SpriteAnimationSet.AttackAt: до контакта первая половина, затем продолжение вперёд до возврата. Windup удерживает позу действующего участника; ожидающий продолжает breathing Idle. Recover удерживает атакующего и при попадании Hurt цели; без попадания цель продолжает Idle. Finish удерживает исчезающего участника. LateUpdate всегда обновляет idleClock, но не затирает held Attack/Hurt/Death. ResetPoses/OnDisable сбрасывают обе блокировки и возвращают исходные RectTransform/цвета. Урон остаётся между Windup/Recover в BattleManager.

## 4. Переменные и Inspector

idleClock и playerAttacking/enemyAttacking — внутреннее визуальное состояние, не Inspector. Профиль берётся из BattleUnit.visualAnimations; существующие длительности/позы сохранены.

messageDuration=0.75, enemyThinkDuration=0.5 (0..2 секунды) и allowFastForward=true/fastForwardMultiplier=3 (1..6) в коде и сцене. При rules.actionDelay=0.8 паузы штатные; ускоренные тесты масштабируют их через Pace. Space ускоряет только Windup/Recover/Finish/ReadPause, не самостоятельные VFX, полосы, всплывающий текст и затемнение сцен.
ui, playerGuard/enemyGuard — ссылки. attackDuration=0.4, hitDuration=0.18 секунды (0..1); lungeDistance=42, reactionDistance=8 условных UI-пикселей. HitEffectsPlayed/IsAnimating — диагностическое чтение. Внутренние home/scale/player/enemy/bound не являются игровым состоянием.

holdPlayerPose/holdEnemyPose — внутренние флаги защиты конечных поз от LateUpdate, не Inspector. Профили HeroDetailed/SkeletonDetailed contact.5/4 Idle/8 Attack, idle4FPS. Штатные attackDuration.4/сообщение.75/think.5/Space×3 сохраняются; кадры конечной атаки выбираются по progress, не по framesPerSecond.

## 5. Связи

BattleManager вызывает контроллер; BattleUI показывает числа/сообщения, Health и Calculator определяют урон. Изображения уже представлены uGUI Image, поэтому мировые SpriteRenderer/Animator не меняются.

## 6. Пример

Attack → выпад → Manager наносит 10 HP один раз → вспышка/число → возврат → ход врага.

## 7. Настройка в Unity

Уже назначен на BattleManager в BattleScene. ui ссылается на прежний UI, Manager.presentation — на этот компонент. Символы сохранены через MCP. Длительности можно менять; отсутствие Animator поддерживается.

## 8. Ошибки, ограничения и расширение

Расчёты не зависят от Animator/events. Все циклы ограничены временем; отсутствие визуала не останавливает ход. При смерти во время Windup Manager повторно проверяет обоих участников и не списывает ресурсы. Для будущих Aseprite-кадров расширять визуальный контроллер, сохранив единственное применение в Manager.

## 9. История изменений

2026-10-12 — кадры Skeleton64/Fire/Heal/Hit подключены к существующему представлению; игровые ресурсы не перенесены в анимации. [Текущие проверки](../../PixelVisuals.md).

- 2026-10-11 — 3.7: конечные паузы чтения/выбора врага, безопасное ускорение Space без изменения времени игры и применения эффектов.

- 2026-10-11 — 3.6: Guard рассчитывается с учётом центрального pivot портрета; flip не смещает персонажа относительно чисел/эффектов.

- 2026-10-11 — 3.5: эффекты исходов; уничтожение/возврат мира остаются в Transition.

- 2026-10-11 — 3.3: Windup Magic слегка подсвечивает портрет кастующего; ResetPoses восстанавливает исходный цвет.

- 2026-10-11 — 3.2 — выпад, реакция/крит, защита, восстановление поз и безопасная интеграция.

- 2026-10-13 — Attack больше не проигрывается назад при Recover; ожидающий участник продолжает Idle; ресурсы и порядок ходов прежние.

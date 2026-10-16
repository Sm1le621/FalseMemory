# BattlePresentation

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattlePresentation.cs`. Воспроизводит кадры и движения существующих боевых uGUI-портретов; не владеет HP/MP/очередью.

## 2. Как работает

Bind запоминает позиции/масштабы. Manager вызывает Windup, однократно применяет эффект, затем Recover. LateUpdate проигрывает Idle только незанятым живым участникам. HeroDetailed получает время конечной атаки из кадров/FPS; остальные профили сохраняют прежнюю длительность.

## 3. Методы и функции

- `Bind(hero,foe)` — void, сохраняет участников и исходные Image RectTransform; отсутствующие UI-ссылки оставляют bound=false.
- `LateUpdate()` — прибавляет unscaledDeltaTime к idleClock, вызывает IdleFrame для не удерживаемых поз, обновляет Guard по IsAlive/IsDefending.
- `IdleFrame(onPlayer)` — void, выбирает Loop Idle живого участника; `PoseFrame(onPlayer,pose,progress)` — конечный Sprite через At; `AttackFrame(onPlayer,progress,recovering)` — фаза через AttackAt. Ресурсов не изменяют.
- `Windup(onPlayer,action,pace)` → IEnumerator: блокирует позу действующего участника, Attack проигрывает подготовку/выпад до контакта; Defend/Magic сохраняют squash/tint и прежнее время. На выходе Attack стоит на контакте.
- `Recover(attackerIsPlayer,successfulHit,critical,pace)` → IEnumerator: продолжает Attack вперёд, возвращает Image домой, при попадании показывает Hurt/flash/recoil и увеличивает HitEffectsPlayed. Затем ResetPoses.
- Приватный `AttackPhaseDuration(onPlayer,recovering)` → float: для непустого Attack профиля useFrameTiming берёт длительность соответствующей фазы; иначе прежнюю половину attackDuration (Recover минимум hitDuration). Это сохраняет тайминг скелета.
- `Finish(outcome,pace)` → IEnumerator: Victory скрывает Enemy, Defeat/побег — Player. Для непустой позы useFrameTiming длительность из массива/FPS; иначе outcomeDuration. Death/Move, fade/scale/смещение, затем IsAnimating=false.
- `VisualDelta` → float: unscaledDeltaTime, при Space и allowFastForward умноженный на fastForwardMultiplier. `ReadPause(enemyThinking,pace)` → IEnumerator паузы чтения/ИИ; `Pace(delay)` → float clamp(delay/.8,.05,3).
- `ResetPoses()` — void, сбрасывает флаги/IsAnimating, восстанавливает Idle/позиции/масштабы/цвета; `OnDisable()` вызывает его.
- Editor-only `BuildGuards()` — находит/создаёт существующие ромбы через `Guard(portrait,name)` → Image; помечает компонент dirty, сохранение — у вызывающего.

Awake/Start/Update/коллизий нет. Отсутствующие/выключенные визуальные ссылки безопасно пропускаются; Manager остаётся единственным местом применения эффекта.

## 4. Переменные и Inspector

ui/playerGuard/enemyGuard — ссылки. Значения по умолчанию и BattleScene: attackDuration.4, hitDuration.18, outcomeDuration.35, lungeDistance42, reactionDistance8, messageDuration.75, enemyThinkDuration.5, allowFastForwardtrue, multiplier3. Для физической атаки HeroDetailed длительность теперь8/16=.5 с (подготовка.25/возврат.25); attackDuration/hitDuration её не растягивают. Skeleton использует прежние.4 с. Для непустого Hero Move при побеге8/16=.5 с. Defend/Magic/Items и пустой Death сохраняют прежний fallback.

bound, home/scale, player/enemy, idleClock, holdPlayerPose/holdEnemyPose, playerAttacking/enemyAttacking — внутреннее состояние, не Inspector. HitEffectsPlayed/IsAnimating доступны только для чтения.

## 5. Связи

BattleManager вызывает фазы и применяет урон между ними. BattleUI отвечает за сообщения/числа, SpriteAnimationSet — изображения/тайминг, Health/Calculator — реальные HP/урон. Мировой Animator не используется для боевых Image.

## 6. Пример

Обычная Attack героя: кадры0…3 по1/16 с → на.25 с контакт4 и один урон10 → кадры4…7 → на.5 с Idle. Изменение Inspector attackDuration не растягивает этот opt-in профиль.

## 7. Настройка в Unity

Уже назначен на BattleManager в BattleScene; ui и manager.presentation связаны. Включение useFrameTiming в HeroDetailed выполнено через MCP, сцена не изменялась. Для старых профилей сохраняется настройка длительностей Inspector.

## 8. Возможные проблемы

Базовые16 FPS действуют при actionDelay.8 без Space. Существующие Pace/Space намеренно ускоряют конечные позы; самостоятельный Idle использует реальное unscaled-время. При недостаточном renderFPS кадры могут пропускаться. Нет новых Hero Hurt/Death/Cast рисунков: прежние flash/fade/tint сохранены. Художественная плавность не следует из успешного теста урона.

## 9. История изменений


2026-10-12 — кадры Skeleton64/Fire/Heal/Hit подключены к существующему представлению; игровые ресурсы не перенесены в анимации. [Текущие проверки](../../PixelVisuals.md).

- 2026-10-11 — 3.7: конечные паузы чтения/выбора врага, безопасное ускорение Space без изменения времени игры и применения эффектов.

- 2026-10-11 — 3.6: Guard рассчитывается с учётом центрального pivot портрета; flip не смещает персонажа относительно чисел/эффектов.

- 2026-10-11 — 3.5: эффекты исходов; уничтожение/возврат мира остаются в Transition.

- 2026-10-11 — 3.3: Windup Magic слегка подсвечивает портрет кастующего; ResetPoses восстанавливает исходный цвет.

- 2026-10-11 — 3.2 — выпад, реакция/крит, защита, восстановление поз и безопасная интеграция.

- 2026-10-13 — Attack больше не проигрывается назад при Recover; ожидающий участник продолжает Idle; ресурсы и порядок ходов прежние.

- 2026-10-16 — актуализированы реальная сетка16 FPS, opt-in тайминг/проверки и ограничения текущего технического подэтапа.

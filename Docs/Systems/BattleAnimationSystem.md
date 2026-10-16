# Анимации боя

## Актуально2026-10-16 — реальные16 FPS героя

HeroDetailed: Idle4/16=.25 с, Attack8/16=.5 с, useFrameTiming=true. SpriteAnimationSet вычисляет целый контактный индекс4/.25 с и Windup/Recover по4/16 с; BattlePresentation больше не растягивает эту атаку по attackDuration. Урон по-прежнему один раз в Manager между фазами; HP/MP/очередь не принадлежат кадрам. Непустой Hero Move при Flee8/16=.5 с; для пустых Hurt/Death/Magic Cast прежние flash/fade/tint/VFX, новых поз нет. SkeletonDetailed сохраняет12/Idle4/useFrameTiming=false и прежние длительности.

Pace относительно actionDelay.8 и Space сохраняют намеренное ускорение; базовые16 без ускорения. Новый Play-тест наблюдает все8 кадров/интервалы/контакт/единственный урон даже при старом attackDuration.9; полная регрессия58/58. Четыре обычных Play-кадра просмотрены, свежий бой HP96/50/MP30. **Графика4/8 прежняя**, её художественная переработка не завершена; короткий Idle требует ручной оценки. [Отчёт и сценарий](../PlayerAnimation16.md). Ниже исторические значения/проверки.

## Пауза физики при переходе2026-10-13

BattleTransition фиксирует текущую физическую позицию участников, временно выключает simulation и interpolation и восстанавливает режимы после включения мира. Так Interpolate не даёт дрейф позиции при возврате после победы. HP/MP/EXP, порядок ходов, контактная фаза.5, Battle Idle4/Attack8, длительность Attack.4с и единственное применение урона прежние. Полное новое художественное Battle Idle/Attack из задания владельца ещё не выполнено. [Текущий отчёт](../AnimationRework.md). Ниже история рабочей боевой графики.

## Текущие профили —2026-10-13

HeroDetailed и SkeletonDetailed добавляют каждому4 Battle Idle/8 Attack; скелет также получил совместимые Hurt2/Death4. BattleUnit сохраняет владельца Health/MP/EXP, новые данные отвечают только за Sprite. BattlePresentation проигрывает Idle4FPS для ожидающего участника даже во время Windup другого; блокировки поз не позволяют LateUpdate затирать Attack/Hurt/Death. AttackAt делит0…1 по contact.5: Windup0→.5, единственное применение в прежнем Manager, Recover.5→1 вперёд, затем Idle. Ранее обратная последовательность больше не используется.

Hero attack — универсальный меч, а не автоматическая подмена по экипированному Staff/оружию; оружейные наборы пока отсутствуют. Hero Hurt/Death сохраняют прежнюю flash/fade реакцию с fallback боевого idle, новый отдельный набор не создавался. Skeleton поддерживает Hurt/Death новым рисунком без смены дизайна. Guard/Magic/Items/Flee/Space и порядок ходов сохранены. Source PNG/старые профили остаются резервом. [Проверки/ручная оценка](../CharacterAnimationUpgrade.md). Ниже сохранена история прежнего Skeleton64.

2026-10-12 — Enemy получил общий SpriteAnimationSet Skeleton64: Idle2/Move4/Attack3/Hurt2/Death4, кадры64×64. EnemySpriteAnimation показывает состояние по позиции/Health, не меняя AI/физику. BattlePresentation проигрывает те же кадры на Image: Idle2FPS, Attack до контакта/возврат, Hurt при попадании и Death при исходе. Герой сохраняет прежние движения. Применение урона/MP остаётся в Manager, Animation Events не используются. [Кадры/проверки](../PixelVisuals.md), [EnemyVisuals](EnemyVisuals.md).

BattlePresentation отображает исходные портреты через RectTransform/Image: Attack — выпад и возврат, попадание — вспышка/реакция, крит — усиленная реакция. Defend показывает голубой ромб до следующего собственного хода. MISS не вызывает удара/отдачи. BattleUI поднимает число и плавно скрывает его за feedbackDuration; крит отличается цветом и размером.

Manager закрывает gate до Windup, повторно проверяет жизнь обоих участников после ожидания, применяет расчёт один раз и вызывает Recover. Анимации не содержат изменений HP/MP/предметов/EXP. Без компонента/Animator действие продолжает работать. Настройки attackDuration/hitDuration/lungeDistance/reactionDistance назначены в BattleScene; pace связан с существующим actionDelay, чтобы ускоренные тесты не ждали штатные задержки.

Темп: attackDuration0.4 (подготовка0.2 и возврат0.2), hitDuration0.18, outcome0.35, сообщение0.75, выбор врага0.5 секунды при actionDelay0.8. Для ускорения удерживать Space: визуальные движения/эти паузы×3, без выбора команды, повторного урона/расхода и изменения Time.timeScale. allowFastForward можно выключить на Presentation; отдельные VFX/числа/затемнения завершаются в своём темпе. Финальный Play Mode проверяет ветки MISS и ускорение.
Расширение: назначить новые согласованные PNG/Aseprite-кадры через SpriteAnimationSet, оставив игровое применение в Manager. [BattlePresentation](../Scripts/Battle/BattlePresentation.md), [BattleUI](../Scripts/Battle/BattleUI.md). Проверки смерти во время windup и отсутствующего визуала добавлены в PlayMode; художественное качество оценивается вручную.

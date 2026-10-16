# SpriteAnimationSet

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/SpriteAnimationSet.cs`. ScriptableObject с наборами Idle/Move/Attack/Hurt/Death и направленными кадрами мира. Управляет выбором изображения и длительностями, не HP/MP.

## 2. Как работает

Циклические позы читают секунды/FPS; конечные — прогресс. Для профиля useFrameTiming контакт привязан к целому кадру, длительности вычисляются из количества/FPS. Пустой набор возвращает fallback, не создаёт вымышленные изображения.

## 3. Методы и функции

- `Frames(pose)` → Sprite[] соответствующей позы.
- `At(pose,progress,fallback)` → Sprite по ограниченному прогрессу0…1, на конце последний; пустой набор/пустой элемент → fallback.
- `AttackContactFrame` → int: floor(clamp01(contactProgress)×число Attack), максимум последний индекс; пустой набор →0.
- `AttackAt(progress,recovering,fallback)` → Sprite: Windup0→контакт, Recover контакт→конец вперёд. В useFrameTiming контакт квантован до AttackContactFrame/число кадров, в старом режиме прежний непрерывный contactProgress. Урон не вызывает.
- `Duration(pose)` → float секунд: длина массива / FPS (Idle использует idleFramesPerSecond), ставка ограничена1…24; null/пустой →0.
- `AttackPhaseDuration(recovering)` → float секунд: контактный индекс/FPS для Windup; оставшееся число кадров/FPS для Recover; пустой Attack →0.
- `Loop(pose,seconds,fallback)` и приватный `LoopFrames(frames,seconds,rate,fallback)` → Sprite по времени, отрицательное время считается0, FPS1…24, без выделения массивов.
- `HasWorldDirections` → bool наличия валидного направленного Walk.
- `LoopWorld(direction,moving,seconds,fallback)` → Sprite ближайшего направления по dot product; нулевой вектор →Down, нет directional →Loop, нет Idle →первый Walk.

DirectionalSpriteFrames того же файла — сериализуемые direction/idle[]/walk[], не компонент. Unity callbacks отсутствуют.

## 4. Переменные и Inspector

idle/move/attack/hurt/death — Sprite[]. framesPerSecond по умолчанию8, idleFramesPerSecond2; оба Inspector1…24. useFrameTiming по умолчаниюfalse, сохраняет прежние профили; в HeroDetailed назначенtrue. attackContactProgress default1 (0…1), HeroDetailed/SkeletonDetailed=.5.

HeroDetailed: Idle4/Move8/Attack8, Hurt/Death пусты, FPS16/Idle16; Idle0,25 с, Attack0,5 с с контактом кадр4 на0,25 с. SkeletonDetailed сохранён: FPS12/Idle4, useFrameTimingfalse, Idle4/Attack8/Hurt2/Death4, четыре worldDirections по8 Walk. Skeleton64 — резерв8/Idle2.

## 5. Связи

BattleUnit.visualAnimations хранит профиль; EnemySpriteAnimation читает LoopWorld, BattlePresentation читает Loop/At/AttackAt и при opt-in длительности. battleSprite — прежний fallback.

## 6. Пример

HeroDetailed Attack8/FPS16/contact.5 → Windup4/16 с, контакт Sprite[4], Recover4/16 с. BattleManager применяет урон один раз между фазами.

## 7. Настройка в Unity

Create → FalseMemory → Sprite Animation Set; назначить кадры и BattleUnit.visualAnimations. Не прикреплять к GameObject. useFrameTiming включать только при согласованных кадрах/контакте; для героя уже включён через MCP.

## 8. Возможные проблемы

При низкой частоте рендера промежуточные Sprite могут не попасть на экран. Смена FPS не создаёт поз. Hero Hurt/Death и отдельный Cast отсутствуют, реакции сохраняют fallback. Четыре Battle Idle при16 дают короткий цикл: художественное обновление ещё необходимо. Pace/Space намеренно ускоряют конечные действия; 16 FPS — базовый темп.

## 9. История изменений


2026-10-12 — создан в задаче native 64px Skeleton/Fire/Heal/Hit и интеграции кадров; текущие проверки — [PixelVisuals](../../PixelVisuals.md).

- 2026-10-13 — добавлены конечные фазы атаки, данные направлений и безопасный LoopWorld; подключены новые профили без переноса HP/MP в анимации.

- 2026-10-16 — актуализированы реальная сетка16 FPS, opt-in тайминг/проверки и ограничения текущего технического подэтапа.

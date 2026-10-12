# SpriteAnimationSet

## 1. Название и назначение

Исходник: Assets/Scripts/Battle/SpriteAnimationSet.cs.

ScriptableObject с пятью наборами кадров: Idle, Move, Attack, Hurt, Death. Общие данные для мира и боевого Image; ресурсов не меняет.

## 2. Как работает

Вызывающий выбирает pose. Для конечной анимации передаёт progress 0…1, для циклической — elapsed seconds. Пустой набор/пустой кадр возвращает fallback.

## 3. Методы и функции

`Frames(pose)` возвращает соответствующий Sprite[]. `At(pose,progress,fallback)` ограничивает progress и выбирает кадр, включая последний при progress≥1. `Loop(pose,seconds,fallback)` выбирает индекс по времени, FPS и остатку деления; отрицательное время считается нулём. Unity callbacks отсутствуют.

`AttackAt(progress,recovering,fallback)` возвращает Attack Sprite: Windup идёт0→attackContactProgress, Recover от contact→1; прогресс ограничен0…1, урон не применяется. `HasWorldDirections` — readonly bool: есть ли корректный непустой directional Walk. `LoopWorld(direction,moving,seconds,fallback)` нормализует направление (нулевое заменяет Down), выбирает ближайший worldDirections по dot product, проигрывает Walk/Idle; без данных сохраняет прежний Loop. Если Idle отсутствует, использует первый Walk-кадр. `LoopFrames(frames,seconds,rate,fallback)` — общий приватный безопасный выбор циклического кадра, без выделения массивов.

`DirectionalSpriteFrames` — сериализуемый класс данных того же файла: direction, idle[],walk[]. Не компонент, не требует отдельного GameObject.

## 4. Переменные и Inspector

idle/move/attack/hurt/death — массивы спрайтов. framesPerSecond по умолчанию8, Inspector1…24; idleFramesPerSecond2, Inspector1…8. В Skeleton64 назначены2/4/3/2/4 кадра и FPS8/Idle2. Выбор Loop дополнительно ограничивает rate1…24.

Новые поля Inspector: attackContactProgress по умолчанию1 (0…1), в HeroDetailed/SkeletonDetailed назначено.5 — контактный кадр4 из8. worldDirections по умолчанию пустой, в SkeletonDetailed четыре единичных вектора Down/Up/Right/Left, по8 Walk и1 Idle. HeroDetailed:4 Idle/8 Attack/8 Move, Hurt/Death пусты, FPS16/Idle4. SkeletonDetailed:4/8/8/2/4 и FPS16/Idle4. Старый Skeleton64 остаётся резервом2/4/3/2/4, FPS8/Idle2 без directional-данных; его исходные поля не переписываются.

## 5. Связи

BattleUnit.visualAnimations хранит ссылку. EnemySpriteAnimation и BattlePresentation читают тот же набор; battleSprite остаётся fallback.

## 6. Пример

Скелет на карте чередует два Idle-кадра с частотой2FPS; во время боя Attack заканчивается третьим кадром перед применением урона.

## 7. Настройка в Unity

Create → FalseMemory → Sprite Animation Set; назначить Sprite[] и ссылку BattleUnit.visualAnimations. Не прикрепляется к GameObject. Текущий ассет Assets/Settings/Battle/Visuals/Skeleton64.asset.

## 8. Возможные проблемы

Смена профиля не изменяет AI/коллайдеры. Нужны согласованные размеры/pivot/PPU; разные холсты вызывают прыжки силуэта. HeroDetailed/SkeletonDetailed подключены; художественная плавность требует ручной проверки.

## 9. История изменений

2026-10-12 — создан в задаче native 64px Skeleton/Fire/Heal/Hit и интеграции кадров; текущие проверки — [PixelVisuals](../../PixelVisuals.md).

- 2026-10-13 — добавлены конечные фазы атаки, данные направлений и безопасный LoopWorld; подключены новые профили без переноса HP/MP в анимации.

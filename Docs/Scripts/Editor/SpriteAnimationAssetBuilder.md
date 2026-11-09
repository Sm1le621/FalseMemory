# SpriteAnimationAssetBuilder

## 1. Название и назначение

Исходник: `Assets/Editor/SpriteAnimationAssetBuilder.cs`.

Редакторский скрипт, который собирает `.anim`-клипы и Animator-контролеры из готовых 64×64 PNG. Кадры остаются исходной графикой, а клипы становятся источником анимации для мира и боевых портретов. В игре скрипт не работает.

## 2. Как работает

Меню `FalseMemory → Animation → Rebuild sprite animations` загружает `Skeleton64.asset` и `FireVisual.asset`, создаёт клипы скелета для мира (`Move`, `Hurt`, `Death` на `SpriteRenderer`), клипы портрета (`Attack`, `Hurt`, `Death` на UI `Image`) и клип огня (`Effect`). Из клипов собираются контролеры `SkeletonWorld.controller`, `SkeletonPortrait.controller` и `FireVFX.controller`, после чего ссылки записываются в ScriptableObject-ассеты.

Меню `Rebuild sprite animations and boss art` дополнительно создаёт PNG босса, если его ещё нет — это служебный пункт для одного запуска.

Кадры загружаются по именам: `Skeleton-Move-00.png` … `Skeleton-Move-03.png`, `Skeleton-Attack-00…02`, `Skeleton-Hurt-00…01`, `Skeleton-Death-00…03`, `Fire-00…07`.

## 3. Методы и функции

- `BuildAll()` — `[MenuItem]`; проверяет, что не включён Play Mode, загружает ассеты и пересобирает все клипы/контролеры.
- `BuildAllWithBoss()` — `[MenuItem]`; вызывает `BuildAll`, затем создаёт PNG босса при отсутствии файла.
- `BuildSkeletonWorld(SpriteAnimationSet set)` — создаёт клипы `Move` (цикл), `Hurt`, `Death` и `SkeletonWorld.controller`.
- `BuildSkeletonPortrait(SpriteAnimationSet set)` — создаёт клипы `Attack`, `Hurt`, `Death` и `SkeletonPortrait.controller`.
- `BuildFire(BattleVFXProfile fire)` — создаёт клип `Effect`, контролер `FireVFX.controller` и записывает `frameCount`.
- `LoadFrames(string folder, string prefix, int count)` → `Sprite[]`; загружает кадры по шаблону имени.
- `CreateClip(string path, Sprite[] frames, bool loop, System.Type targetType)` → `AnimationClip`; пишет PPtr-кривую `m_Sprite`, частоту 8 fps и настройки цикла.
- `CreateController(string path, string[] stateNames, AnimationClip[] clips)` → `AnimatorController`; создаёт состояния и назначает первый стан дефолтным.
- `SetReference(ScriptableObject asset, string fieldName, Object value)` — записывает ссылку через `SerializedObject`.
- `DeleteIfExists(string path)` / `EnsureFolder(string path)` — private helpers; удаляют старый сгенерированный файл и создают папки.

## 4. Переменные и Inspector

Inspector-полей нет. Константы задают корни: `Assets/Animations/Enemies/Skeleton/World`, `.../Portrait`, `Assets/Animations/VFX`, а также пути к `Skeleton64.asset` и `FireVisual.asset`. `FramesPerSecond = 8` и имя состояния `Effect` заданы в коде.

## 5. Связи

Читает PNG из `Assets/Art/Enemies/Skeleton64` и `Assets/Art/VFX/Fire64`. Пишет клипы и контролеры в `Assets/Animations`. Назначает `worldController`/`portraitController` в `SpriteAnimationSet`, `effectController`/`frameCount` в `BattleVFXProfile`. Связан с `PixelBattleAssetBuilder`, который создаёт исходные PNG.

## 6. Пример

После изменения кадров скелета запусти `FalseMemory → Animation → Rebuild sprite animations`. Скрипт заново создаст `Skeleton_Move.anim`, `Skeleton_Hurt.anim`, `Skeleton_Death.anim`, портретные клипы, `Fire_Cast.anim` и три контролера, затем обновит ссылки в ассетах. В рантайме `SpriteClipPlayer` уже использует новые клипы.

## 7. Настройка в Unity

Останови Play Mode. Запусти нужный пункт меню `FalseMemory → Animation`. Для повторной сборки ничего удалять вручную не нужно: старые сгенерированные клипы/контролеры заменяются. Не редактируй сгенерированные клипы вручную — при следующей сборке изменения пропадут.

## 8. Возможные проблемы

Если PNG отсутствует, сборка выбрасывает `InvalidOperationException` с путём файла. В Play Mode меню не выполняется. Скрипт перезаписывает только файлы в `Assets/Animations`, но не сами PNG. Имена кадров должны совпадать с ожидаемым шаблоном. После сборки дождись завершения импорта Unity и проверь Console.

## 9. История изменений

- 2026-11-10 — создан для перевода анимаций на `.anim`-клипы и Animator-контролеры.

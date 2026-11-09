# PixelBattleAssetBuilder

## 1. Название и назначение

Исходник: `Assets/Editor/PixelBattleAssetBuilder.cs`.

Редакторский генератор нативной пиксельной графики 64×64: кадры скелета, огня, удара и одна статичная модель скелета босса. Код создаёт PNG на реальной сетке 64×64 и сразу настраивает импорт как Sprite 2D; не является игровым MonoBehaviour.

## 2. Как работает

Меню `FalseMemory → Art → Generate native 64px battle art` перебирает все позы `SpritePose` и создаёт кадры: Move 4, Attack 3, Hurt 2, Death 4 — всего 13 кадров скелета. Дополнительно создаются 8 кадров Fire. Перед записью проверяется, что ни один файл ещё не существует: генератор никогда не перезаписывает существующую графику. Затем каждый PNG импортируется как одиночный Sprite с Point filtering, без mipmaps и компрессии, Pixels Per Unit 100.

Меню `Generate native 64px impact` создаёт `Impact.png` с той же проверкой и настройками импорта.

Меню `Generate native 64px boss skeleton` создаёт один статичный PNG `Assets/Art/Enemies/BossSkeleton64/BossSkeleton-Idle-00.png`. Он не подключается к сцене, префабу, анимациям или бою — это только модель.

## 3. Методы и функции

- `Generate()` — `[MenuItem]`; проверяет Play Mode, собирает 13 кадров скелета и 8 кадров огня, проверяет отсутствие файлов, создаёт PNG и настраивает импорт.
- `GenerateBossSkeleton()` — `[MenuItem]`; создаёт один PNG босса, если файла ещё нет; настраивает импорт.
- `GenerateImpact()` — `[MenuItem]`; создаёт один кадр удара.
- `Skeleton(SpritePose pose, int frame)` → `Grid`; рисует скелет в нужной позе (походка, замах, удар, падение).
- `BossSkeleton()` → `Grid`; рисует статичную модель босса: корона, роги, большой меч, рваная накидка.
- `Skull`, `Bone`, `Sword`, `Shield` — примитивы сборки силуэта.
- `Fire(int frame)` → `Grid`; кадры огня.
- `Grid.Px/Rect/Line/Poly/Ellipse` — низкоуровневая отрисовка в массив 64×64.
- `ApplyPixelArtSettings(string path)` / `SavePng(string path, Grid grid)` — private helpers; сохраняют PNG и включают Point-фильтр, отсутствие mipmaps/компрессии, PPU 100.
- Константы `SkeletonRoot`, `VfxRoot`, `BossRoot` — пути вывода.

## 4. Переменные и Inspector

Inspector-полей нет. `Palette` — фиксированная палитра Dark Fantasy; пути заданы константами. Все кадры имеют размер 64×64, RGBA32, прозрачный фон. Настройки импорта задаются кодом, а не вручную.

## 5. Связи

Созданные кадры используются `SpriteAnimationAssetBuilder`, который собирает из них `.anim`-клипы и Animator-контролеры для `SpriteAnimationSet` (Skeleton64) и `BattleVFXProfile` (Fire). Кадр `Impact.png` используется `BattleUI.impactSprite`. Модель босса пока ни к чему не подключена. Скрипт находится в `Assets/Editor` и не попадает в игровую сборку. Пути: `Assets/Art/Enemies/Skeleton64`, `Assets/Art/Enemies/BossSkeleton64`, `Assets/Art/VFX/Fire64`, `Hit64`.

## 6. Пример

Запуск `Generate native 64px battle art` в пустой папке создаёт 13 PNG скелета и 8 PNG огня. Если `Skeleton-Move-00.png` уже существует, генерация останавливается с `InvalidOperationException` и ничего не перезаписывает. Отдельный запуск `Generate native 64px boss skeleton` создаёт `BossSkeleton-Idle-00.png`; повторный запуск остановится, пока файл существует.

## 7. Настройка в Unity

Останови Play Mode. Выбери меню `FalseMemory → Art` и нужный генератор. Для повторной генерации сначала вручную удали/перемести старые файлы — автоматической перезаписи нет. Проверь в Inspector, что PNG импортированы как Sprite 2D, Point, без mipmaps, PPU 100. После генерации запусти `FalseMemory → Animation → Rebuild sprite animations`, чтобы обновить клипы.

## 8. Возможные проблемы

Генератор не работает в Play Mode. Он не обновляет существующие `.meta` и не создаёт атлас. Палитра и форма фиксированы; для нового врага нужен отдельный генератор или ручной ассет. Модель босса — один статичный кадр, без анимации и без префаба. Не выдавай увеличенное превью за нативный кадр — исходники всегда 64×64.

## 9. История изменений

- 2026-11-10 — удалена генерация лечения; добавлена статичная модель скелета босса 64×64.
- 2026-11-09 — добавлены private helpers `ApplyPixelArtSettings`/`SavePng`; описание сверено с кодом после упрощения.
- 2026-10-31 — из генератора/набора удалены IDLE-кадры; скелет содержит Move/Attack/Hurt/Death, проверка существующих файлов сохранена.
- 2026-10-12 — базовая генерация пиксельной графики.

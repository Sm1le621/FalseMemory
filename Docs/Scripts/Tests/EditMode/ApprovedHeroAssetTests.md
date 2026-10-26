# ApprovedHeroAssetTests

## 1. Название и назначение

Исходник: `Assets/Tests/EditMode/ApprovedHeroAssetTests.cs`. Контракты существующих клипов, 24 используемых уникальных Sprite и резервных ресурсов героя.

## 2. Как работает

Пять NUnit-тестов только читают AssetDatabase/импорт/ключи/controller. Проверки реальных четырёх кадров отличаются от архивных8/30 наборов, которые не удаляются.

## 3. Методы и функции

- `AllHeroClipsIncludingBackupsUseTheSixteenFpsTimeGrid()` — Test/void: все20 прежних clip Samples16, времена Sprite-ключей и stopTime целые ticks1/16. Сохраняет проверку и архивных диагоналей/резерва.
- `CardinalIdleKeepsFourFramesAndWalkUsesStanceRightLeftStance()` — Test/void: Idle имеет5 ключей (4 разных Sprite и замыкание), цикл2с. Walk имеет ровно4 ключа, нейтральный Sprite равен Idle_00 в позициях0/3, имена двух шагов RightStep/LeftStep, три разных ссылки/PNG-хеша, времена0/.0625/.25/.4375 и stopTime .5с. Все Sprite лежат в Cardinal4, canvas512/pivot256,20, PPU одинаков внутри направления, Point/no mipmaps/Uncompressed; всего24 уникальных используемых изображения.
- `ActiveControllerHasOnlyFourCardinalChildrenPerMotion()` — Test/void: два существующих Blend Tree имеют только4 cardinal позиции/timeScale1, диагональных child нет.
- `SheetsKeepNativeResolutionAndConsistentGroundPivots()` — Test/void: старые Multiple-листы по-прежнему имеют4 Sprite/исходные размеры/PPU/pivot/Point/NPOT/no mipmaps/compression. Они резерв, не новые кадры.
- `WalkClipsHaveOnlyFourOrderedKeysWithoutTransformAnimation()` — Test/void: четыре действующих Walk имеют ровно4 Sprite-ключа, Samples16/native clip.length .5с/Loop; float Transform-кривых нет.

Awake/Update и собственных Unity callbacks нет; результаты через assertions.

## 4. Переменные и Inspector

Root/Directions/массив cardinal-направлений/HashSet/hashes — тестовые значения, не Inspector. Число4/размер512 относятся к текущему новому набору, не ко всем будущим ассетам.

## 5. Связи

AssetDatabase/AnimationUtility/TextureImporter/AnimatorController/BlendTree, Player64, Cardinal4 PNG и старые ApprovedHero Multiple-листы.

## 6. Пример

Подмена двух разных шагов одним PNG провалит hashes; одинаковая нейтральная стойка в начале/конце обязательна; оставшиеся8 children провалят topology; Samples16 с неверными временами провалит ticks/duration.

## 7. Настройка в Unity

Не прикреплять к объекту. Test Runner → EditMode → FalseMemory.Battle.EditModeTests после фактического импорта/компиляции.

## 8. Возможные проблемы

Различия pixel SHA256/ссылок не доказывают качество походки. Play-сценарии проверяют фактическую смену24 уникальных Sprite и порядок Walk; результаты — [PlayerWalkOrder](../../../PlayerWalkOrder.md). Неподключённые файлы не считаются прошедшими тестами.

## 9. История изменений


- 2026-10-13 — две проверки переменного разрешения и Sprite-ключей.

- 2026-10-13 — контракт активных Walk-клипов обновлён на8 кадров; проверки прежних источников сохранены.

- 2026-10-13 — актуализированы темп/физическое воспроизведение и проверки в задаче Animation Rework.

- 2026-10-13 — проверка нового30-frame Idle_Down без ослабления прежних направлений/движения.

- 2026-10-16 — актуализированы реальная сетка16 FPS, opt-in тайминг/проверки и ограничения текущего технического подэтапа.

- 2026-10-26 — актуализирован новый ограниченный набор4×4 Idle/Walk, выбор cardinal-взгляда/импорт и соответствующие проверки. Фактический статус выполнения — в PlayerCardinal4.

- 2026-10-26 — через Unity MCP подключён четырёхпозиционный Walk: нейтральная стойка → RightStep → LeftStep → та же стойка; обновлены контракты и реальные Play-проверки. Подробности текущей проверки — в PlayerWalkOrder.

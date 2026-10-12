# DetailedAnimationTests

## 1. Название и назначение

`Assets/Tests/EditMode/DetailedAnimationTests.cs` — три проверки боевых профилей и directional-данных без запуска игры.

## 2. Как работает

Читает HeroDetailed/SkeletonDetailed через AssetDatabase, проверяет согласованные импортированные кадры, разделение фаз атаки и четыре направления мира. Сцены/ресурсы не меняются.

## 3. Методы и функции

`Profile(name)` возвращает профиль по техническому имени. `BattleProfilesHaveDifferentIdleAttackFramesAndSharedCanvasWithinEachActor()` проверяет4 разных Idle/8 Attack, единые rect/pivot/PPU внутри battle-набора, Point/no compression/mipmaps, Skeleton Hurt2/Death4. `AttackContactSeparatesForwardWindupAndRecoveryWithoutResourceWrites()` проверяет начало/контакт/прямое завершение по индексам0/4/6/7 и fallback старого Skeleton64 без worldDirections. `WorldDirectionsSelectFourActualViewsAndKeepIdleWhenStopped()` проверяет8 Walk/направление/неизменный Idle/согласованный импорт в каждом цикле. Тесты void/NUnit assertions; Unity callbacks отсутствуют.

## 4. Переменные и Inspector

Технические пути, имена и ожидаемые количества внутри теста, не Inspector. Это контракт текущих профилей, а не ограничение размера будущей графики.

## 5. Связи

SpriteAnimationSet/DirectionalSpriteFrames, PNG новых наборов, AssetDatabase/TextureImporter. Отдельно сохраняется контракт прежнего резервного Skeleton64.

## 6. Пример

Если Recover снова выбирает кадры в обратном порядке, AttackAt(.5,true) не совпадёт с attack[6]. Ошибочный right-кадр в Up-наборе выявляется выбором массива в directional-данных.

## 7. Настройка в Unity

Не прикрепляется. Test Runner → EditMode → FalseMemory.Battle.EditModeTests → DetailedAnimationTests после импорта и компиляции.

## 8. Возможные проблемы

Наличие разных Sprite не доказывает плавность нарисованных поз. Runtime/однократный урон проверяются Play-тестами, художественную оценку выполняет владелец.

## 9. История изменений

- 2026-10-13 — добавлены проверки новых профилей, конечных фаз атаки и направления мира.

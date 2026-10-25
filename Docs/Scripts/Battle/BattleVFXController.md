# BattleVFXController

## 1. Название и назначение

**Исходный файл:** `Assets/Scripts/Battle/BattleVFXController.cs`.

Проигрывает визуальный эффект магии через заранее созданные в сцене `Image`: покадровый `PixelSpellFrame` и пул частиц. Компонент ничего не создаёт и не уничтожает во время игры.

## 2. Как работает

`BattleManager` вызывает `Play(...)` после применения заклинания. Контроллер берёт профиль (`fireProfile`/`healProfile`), включает первые N частиц из сериализованного массива, при наличии кадров включает `PixelSpellFrame` и в корутине обновляет их позицию, размер, поворот и прозрачность. По завершении все элементы снова выключаются. Время эффекта не задерживает ход: урон/MP уже применены правилами боя.

## 3. Методы и функции

- `Play(bool onPlayer, BattleVFXProfile profile, Color fallback, bool healing)` — выбирает портрет (`playerPortrait`/`enemyPortrait`), останавливает прежний эффект, вызывает `Clear()` и запускает `Animate`; увеличивает `EffectsPlayed`.
- `Animate(Image portrait, BattleVFXProfile profile, Color fallback, bool healing)` — корутина: вычисляет count/duration/radius/size/color/shape, включает частицы (`particles[i].gameObject.SetActive(true)`), при кадрах включает `frameImage`, каждый кадр обновляет `CurrentFrame` и позицию относительно портрета, затем вызывает `Clear()`.
- `Clear()` — выключает все частицы и `frameImage`, сбрасывает `ActiveParticles = 0`, `CurrentFrame = -1`.
- `OnDisable()` — останавливает корутину и очищает эффект. `OnDestroy` отсутствует: объекты принадлежат сцене.
- `HasAnimatedFrame` — true, когда `frameImage` активен и имеет спрайт; используется тестами.
- `CurrentFrame`, `ActiveParticles`, `EffectsPlayed` — только чтение для диагностики/тестов.

## 4. Переменные и параметры Inspector

- `ui` — ссылка на `BattleUI` (портреты).
- `fireProfile`, `healProfile` — `BattleVFXProfile` с цветом, формой, кадрами и числом частиц.
- `frameImage` — `PixelSpellFrame` из `BattleScene/BattleCanvas/BattleContent/VFXLayer`.
- `particles` — массив 24 `Image` `BattleParticle0…23` из того же `VFXLayer`; лишние остаются выключенными.

Значения профилей: `particleCount` ограничен 4…24 и длиной массива; `duration` 0…2; `radius` 0…100; `particleSize` 1…24; `frameSize` 32…256.

## 5. Связи с другими системами

`BattleManager` владеет ресурсами и вызывает `Play`; `BattleUI` даёт портреты; `BattleVFXProfile` — данные эффекта. `BattlePresentation` независимо анимирует позы портретов. Объекты `VFXLayer` сохранены в `BattleScene`.

## 6. Пример работы

Игрок применяет Fire → `BattleManager` вызывает `Play(true, fireProfile, …)` → включаются 12 частиц и 8 кадров огня → через 0.6 с `Clear()` выключает всё, `ActiveParticles = 0`, ход продолжается.

## 7. Настройка в Unity

Компонент находится на `BattleManager` в `BattleScene`. В Inspector назначьте `Ui`, `Fire Profile`, `Heal Profile`, `Frame Image` и 24 элемента массива `Particles`. `VFXLayer` расположен под `BattleContent` последним sibling, его `RectTransform` — anchor (0,0), pivot (0.5,0.5), size 0, чтобы координаты совпадали с портретами. Не добавляйте второй пул.

## 8. Возможные проблемы и проверки

- Пустой массив `particles` → эффекта нет, `ActiveParticles = 0`.
- Пустой `profile.particleSprite` рисует цветной квадрат — нужен спрайт частицы (см. `Docs/Art/RequiredSprites.md`).
- Если `VFXLayer` поместить в другой RectTransform, позиции частиц разойдутся с портретами.
- `OnDisable` обязателен: тест проверяет очистку при выключенном компоненте.
- Проверено PlayMode-тестами `PixelVisualFlowTests` и `BattlePresentationFlowTests`; визуальная красота эффектов вручную не оценивалась.

## 9. История изменений

- 2026-10-12 — впервые документирован; эффект создавал `PixelSpellFrame` и пул частиц кодом.
- 2026-10-25 — пул и кадр перенесены в `BattleScene`; удалены `EnsurePool`/`OnDestroy`, массив и `frameImage` сериализованы.

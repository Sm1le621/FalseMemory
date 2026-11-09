# BattleVFXController

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleVFXController.cs`.

Проигрывает визуальный эффект заклинания на портрете цели, используя заранее созданные в сцене `Image`-частицы и кадровый клип из `BattleVFXProfile`. Компонент ничего не создаёт и не уничтожает во время боя и не меняет HP/MP.

## 2. Как работает

`Play` выбирает портрет игрока или врага, останавливает предыдущий эффект и запускает `Animate`. Если профиль не назначен, используется `fireProfile`. По профилю задаются количество частиц (4…24, ограничено длиной массива), длительность, радиус, размер, цвет и форма. Покадровая анимация показывается через `frameImage`: `SpriteClipPlayer` выбирает состояние `Effect` из `effectController` по прогрессу. Частицы движутся по орбите, разлёту или вверх. В конце `Clear` скрывает все элементы.

## 3. Методы и функции

- `Awake()` — Unity callback; заранее добавляет/находит Animator для `frameImage`.
- `Play(bool onPlayer, BattleVFXProfile profile, Color fallback)` — запускает эффект; `onPlayer` выбирает портрет-цель. Ничего не делает без `ui` или портрета.
- `Animate(Image portrait, BattleVFXProfile profile, Color fallback)` — coroutine анимации частиц/кадров.
- `Clear()` — скрывает частицы и кадр, сбрасывает счётчики.
- `OnDisable()` — останавливает coroutine и очищает эффект.
- `CurrentFrame` — текущий кадр, `-1` если анимации нет.
- `HasAnimatedFrame` — активен ли кадровый слой.
- `EffectsPlayed`, `ActiveParticles` — счётчики для тестов/наблюдения.

## 4. Переменные и Inspector

- `ui` — ссылка на `BattleUI` с портретами.
- `fireProfile` — fallback-профиль, если у заклинания нет своего.
- `frameImage` — необязательное изображение кадров.
- `particles` — массив заранее созданных Image-частиц.
- `frameAnimator`, `effect` — внутреннее состояние.
- Настройки конкретного эффекта берутся из `BattleVFXProfile`: `particleCount`, `duration`, `radius`, `particleSize`, `color`, `shape`, `effectController`, `frameCount`, `frameSize`, `particleSprite`.

## 5. Связи

Компонент находится в `BattleScene` и назначается в `BattleManager.vfx`. `BattleManager` вызывает `Play` после успешного `TryCast`. `BattleUI` даёт портреты, `BattleSpell.visualEffect`/`effectColor` — параметры. `BattleVFXProfile` — ScriptableObject с настройками. `SpriteClipPlayer` показывает клип эффекта.

## 6. Пример

Fire с профилем `FireVisual`: над портретом врага появляются 12 частиц, проигрываются 8 кадров, затем всё скрывается. HP меняет только `BattleAbilities`, VFX лишь показывает эффект.

## 7. Настройка в Unity

Назначь `ui`, `fireProfile` и массив частиц. `frameImage` и `particles` должны быть дочерними элементами Canvas. Если профиль не назначен, эффект всё равно проиграется с fallback-цветом. Для нового заклинания создай/назначь `BattleVFXProfile` и собери контролер клипа через `SpriteAnimationAssetBuilder`.

## 8. Возможные проблемы

Без `ui` или портрета эффект пропускается. Если массив частиц пуст, кадровый слой может работать, но частиц не будет. Без `effectController` показываются только частицы. Эффект не должен задерживать очередь боя. Лечебного эффекта сейчас нет: вкладка Items и команда Heal удалены из боя.

## 9. История изменений

- 2026-11-10 — кадры эффекта переведены на `.anim`-клип; параметр `healing` и `healProfile` удалены вместе с лечебным VFX.
- 2026-10-31 — документ приведён к текущему коду; отмечено, что лечебных заклинаний нет, но fallback-профиль сохранён.
- 2026-10-12 — базовый VFX-контроллер.

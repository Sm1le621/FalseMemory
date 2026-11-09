# SpriteClipPlayer

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/SpriteClipPlayer.cs`.

Небольшой статический помощник: показывает кадры из `.anim`-клипов на `SpriteRenderer` или UI `Image` через `Animator`. Animator добавляется во время игры, поэтому сцены и префабы не нужно менять вручную.

## 2. Как работает

`GetAnimator` ищет `Animator` на объекте и при отсутствии добавляет новый с отключённым root motion и `AlwaysAnimate`. `Sample` включает Animator, ставит скорость 0, выбирает состояние и нормализованное время 0…1, затем вызывает `Update(0)`, чтобы кадр применился сразу. `Loop` включает Animator, ставит скорость 1 и запускает циклическое состояние; при первом включении или смене контролера принудительно обновляет кадр. `Stop` выключает Animator, чтобы код мог поставить статичный спрайт.

## 3. Методы и функции

- `GetAnimator(GameObject target)` → `Animator`; берёт существующий компонент или добавляет новый. Для `null` возвращает `null`.
- `Sample(Animator animator, RuntimeAnimatorController controller, string stateName, float progress)` — показывает состояние в точке `progress` (0…1). Ничего не делает без Animator или контролера.
- `Loop(Animator animator, RuntimeAnimatorController controller, string stateName)` — запускает/продолжает циклический стан.
- `Stop(Animator animator)` — выключает Animator, если он есть.
- `Prepare(Animator animator, RuntimeAnimatorController controller)` → `bool`; private helper, включает Animator, меняет контролер и сообщает, был ли Animator выключен или контролер заменён.

## 4. Переменные и Inspector

Собственных полей и Inspector-параметров нет. Вся настройка приходит аргументами. `stateName` должен совпадать с именем состояния в контролере (`Move`, `Attack`, `Hurt`, `Death`, `Effect`).

## 5. Связи

Вызывается из `EnemySpriteAnimation` (мир), `BattlePresentation` (портреты) и `BattleVFXController` (эффект). Работает с контролерами из `SpriteAnimationSet` и `BattleVFXProfile`. Компонент к GameObject не прикрепляется.

## 6. Пример

`EnemySpriteAnimation` вызывает `SpriteClipPlayer.Loop(animator, set.worldController, "Move")`, пока скелет идёт. В бою `BattlePresentation` вызывает `Sample(animator, set.portraitController, "Hurt", progress)`, и портрет показывает кадр боли по прогрессу фазы.

## 7. Настройка в Unity

Ничего назначать не нужно. Убедись, что у объекта есть `SpriteRenderer` или `Image` и что контролер содержит состояние с нужным именем. Animator добавится автоматически при первом вызове.

## 8. Возможные проблемы

Если контролер или Animator отсутствуют, вызов тихо пропускается — объект останется со статичным спрайтом. `Sample` с progress 1 показывает последний кадр клипа. После `Stop` Animator не обновляет спрайт, поэтому код должен сам назначить `sprite`. Не вызывай `Loop` и `Sample` для одного Animator в одном кадре: победит последний вызов.

## 9. История изменений

- 2026-11-10 — создан для показа `.anim`-клипов через Animator.

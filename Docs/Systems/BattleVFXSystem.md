# Магические эффекты боя

## 1. Назначение

Визуальные эффекты заклинаний и физических попаданий: `.anim`-клип из `BattleVFXProfile` и частицы в `BattleScene`. Эффекты не меняют HP/MP и не задерживают очередь.

## 2. Общая логика

`BattleVFXController` использует 24 заранее сохранённых `Image` и один `PixelSpellFrame`. `Play` запускается после успешного `TryCast` и выбирает профиль заклинания либо fallback `fireProfile`. Кадры эффекта проигрывает `SpriteClipPlayer` из клипа `Effect` контролера `effectController`; частицы двигаются по форме Rising/Orbit/Burst, затем пул очищается. Отключение компонента или выгрузка сцены также очищает эффект.

## 3. Основные компоненты

- `BattleVFXController` — проигрывание/очистка.
- `BattleVFXProfile` — ScriptableObject настроек с `effectController` и `frameCount`.
- `SpriteClipPlayer` — показ клипа эффекта.
- `BattleSpell.visualEffect` — ссылка на профиль.
- `BattleUI` — портрет цели и `impactSprite`.
- `BattleManager` — момент запуска.

## 4. Взаимодействие скриптов

`BattleManager` вызывает `vfx.Play(target == Player, spell.visualEffect, spell.effectColor)` после успешного применения. `BattleUI.ShowFeedback` показывает число; для магии сплошная вспышка отключена. `BattlePresentation` не зависит от VFX. Клип и контролер эффекта собирает `SpriteAnimationAssetBuilder`.

## 5. Последовательность событий

Успешный Fire → `EffectsPlayed++` → кадры/частицы → завершение → `Clear` → `ActiveParticles = 0`. При отсутствии VFX или профиля бой продолжается, урон/MP применяются один раз.

## 6. Настраиваемые параметры

`FireVisual`: 8 кадров 64×64, `duration = 0.6`, `frameSize = 128`, оранжевые частицы, клип `Effect` в `FireVFX.controller`. Профиль задаёт `particleCount` 4…24, `duration` 0…2, `radius` 0…100, `particleSize` 1…24, цвет, форму, спрайт частицы и `frameCount`. Лечебного профиля больше нет.

## 7. Использование в Unity

Пул частиц и кадр находятся в `BattleScene/BattleCanvas/BattleContent/VFXLayer`. Назначь `BattleManager.vfx`, `BattleVFXController.ui`, `fireProfile`. Для нового заклинания создай профиль через `Create → FalseMemory → Battle VFX Profile`, собери клип через `SpriteAnimationAssetBuilder` и назначь профиль в `BattleSpell`.

## 8. Расширение

Новый эффект — новый профиль или новые кадры/клип; контроллер менять не нужно, если форма/частицы подходят. Для принципиально новой анимации потребуется расширение `BattleVFXShape`/контроллера. Не создавай GameObject эффектов в рантайме.

## 9. Ограничения и проверки

Нет 3D-частиц и отдельных префабов. Отсутствующий профиль или контролер безопасен: показываются частицы либо эффект пропускается. PlayMode проверяет один запуск, завершение, отключённый VFX и отсутствие изменения ресурсов; визуальная оценка — за владельцем.

## История

- 2026-11-10 — кадры эффекта переведены на `.anim`-клип; лечебный VFX и `HealVisual` удалены.
- 2026-10-31 — удалено лечебное заклинание; HealVisual оставлен как неиспользуемый fallback.
- 2026-10-12 — базовые пиксельные VFX.

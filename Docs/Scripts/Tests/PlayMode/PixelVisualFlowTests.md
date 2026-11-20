# PixelVisualFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/PixelVisualFlowTests.cs`.

Partial `BattleFlowTests`: статичный Idle-спрайт скелета в мире/бою и пиксельный Fire VFX при настоящем боевом применении.

## 2. Как работает

Использует общий `Setup`/`Cleanup`, реальные `SampleScene`/`BattleScene` и сохранённые ссылки на Enemy/Player. Проверяет `SpriteRenderer`, `BattleUI.enemyPortrait`, `BattleUnit.visualAnimations` и `BattleVFXController`.

## 3. Методы и функции

- `SkeletonUsesStaticIdleSpriteInWorldAndBattle()` — `visualAnimations == null`, `battleSprite` — Idle, масштаб (2,2,1), два коллайдера; при смещении в мире, ударе, ходе скелета и смерти спрайт мира/портрета всегда равен `battleSprite`; после победы портрет исчезает (alpha 0).
- `AnimatedFireAndDisabledVfxDoNotDuplicateEffectsOrResources()` — повторная команда отклоняется, Fire 19/8 MP, кадры 0…7, частицы очищаются, отключённый VFX не блокирует Defend.

## 4. Переменные и Inspector

`attackDuration = 0.8` задаётся только на runtime-компоненте. Inspector отсутствует; сцена/профили не сохраняются.

## 5. Связи

`BattleFlowTests`, `SpriteClipPlayer`, `BattlePresentation`, `BattleVFXController`, `BattleUI`, `Health`/`BattleUnit`.

## 6. Пример

Скелет смещается вправо → кадр не меняется. В бою после удара HP 60→50, портрет остаётся Idle. Fire проигрывает 8 кадров клипа Effect, MP 30→22, HP врага 60→41.

## 7. Настройка в Unity

Test Runner → PlayMode; у Enemy в `SampleScene` должен быть пустой `visualAnimations`; нужны `FireVisual.asset` с `effectController` и настроенные портреты/VFX в `BattleScene`.

## 8. Ограничения и проверки

Проверяет наличие/смену кадров и отсутствие побочных эффектов, не художественное качество. 2026-11-20: PlayMode 38/38.

## 9. История изменений

- 2026-11-20 — тест скелета заменён на `SkeletonUsesStaticIdleSpriteInWorldAndBattle`: скелет без собственной анимации.
- 2026-11-10 — проверки переведены на имена кадров из `.anim`-клипов; PlayMode 37/37.
- 2026-10-31 — удалены проверки Heal/Idle; Fire остаётся единственным боевым VFX.
- 2026-10-12 — базовая документация.

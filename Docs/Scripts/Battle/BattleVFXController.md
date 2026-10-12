# BattleVFXController

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleVFXController.cs. Лёгкие временные эффекты магии через пул uGUI Image.

## 2. Как работает

Manager вызывает Play только после успешного TryCast/TryUseItem в том же кадре применения. Квадраты поднимаются у Fire, ромбы вращаются у Heal/Potion; после эффекта скрываются.

## 3. Методы и функции

2026-10-12 — native pixel visuals: Animate создаёт PixelSpellFrame при первом наборе, выбирает индекс по progress, следует центру портрета, показывает кадры вместе с прежними частицами. Clear сбрасывает кадр/частицы; OnDisable останавливает coroutine, OnDestroy уничтожает также overlay. Null-кадр скрывается через enabled=false; ход не ждёт VFX.

- `Play(onPlayer,profile,fallback,healing)` — выбирает цель/профиль, отменяет прежнюю coroutine, Clear и Animate; void, не меняет Health/MP.
- `Animate(portrait,profile,fallback,healing)` → IEnumerator: безопасные пределы параметров, EnsurePool, положение/размер/цвет частиц по unscaledDeltaTime; в конце Clear.
- `EnsurePool(parent,count)` — создаёт недостающие Image без raycast, максимум 24.
- `Clear()` — скрывает частицы, ActiveParticles=0.
- `OnDisable()` — останавливает эффект и очищает; `OnDestroy()` — освобождает созданные дочерние объекты и пул.

## 4. Переменные и Inspector

frameImage — один переиспользуемый Image, не Inspector. CurrentFrame (-1 вне эффекта) и HasAnimatedFrame — диагностика чтения; последний учитывает enabled и ненулевой Sprite.

ui, fireProfile/healProfile — ссылки; particles/effect — внутренний пул/текущая coroutine. EffectsPlayed и ActiveParticles — диагностика чтения; не игровые события.

## 5. Связи

BattleManager, BattleUI, BattleSpell.visualEffect, BattleVFXProfile. Эффект под тем же BattleContent и Canvas, что портреты.

## 6. Пример

Fire наносит прежние 19 HP/8 MP; у цели одновременно начинается оранжевое пламя и число. Heal даёт фактические HP и зелёное кольцо.

## 7. Настройка в Unity

Компонент подключён к BattleManager через MCP; m.vfx назначен, ui/fireProfile/healProfile сохранены. particleSprite можно назначить позднее.

## 8. Ошибки, ограничения и расширение

Отсутствующий профиль использует запасной профиль либо встроенную анимацию; отсутствующий UI/выключенный контроллер безопасно пропускается. Ход не ждёт завершения VFX. Для нового визуального семейства расширять профиль/траекторию, не Abilities.

## 9. История изменений

2026-10-12 — кадры Skeleton64/Fire/Heal/Hit подключены к существующему представлению; игровые ресурсы не перенесены в анимации. [Текущие проверки](../../PixelVisuals.md).

- 2026-10-11 — 3.3 — Fire, Heal/Potion и безопасное окончание эффектов.

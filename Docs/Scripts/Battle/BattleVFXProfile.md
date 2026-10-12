# BattleVFXProfile

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleVFXProfile.cs. Расширяемые данные визуального эффекта, независимо от игрового заклинания.

## 2. Как работает

ScriptableObject выбирает Rising/Orbit/Burst, цвет, размер и необязательный спрайт частиц; VFXController только читает данные.

## 3. Методы и функции

2026-10-12 — native pixel visuals: Профиль содержит художественные данные без callbacks/методов. Контроллер дополнительно показывает конечный набор кадров; пустой массив оставляет прежние частицы.

Собственных методов нет. Create Asset Menu создаёт экземпляр средствами Unity. Enum BattleVFXShape задаёт три траектории, без новых игровых эффектов.

## 4. Переменные и Inspector

frames — необязательный Sprite[]; frameSize=128 UI-единиц, Inspector32…256. FireVisual/HealVisual:8 назначенных PNG64×64, duration0.6 вместо кодового default0.45; frameSize128. Остальные shape/color/particle* прежние.

shape; color; particleSprite — необязательный Sprite; duration 0..2 (0.45), particleCount 4..24 (12), particleSize 1..24 (8), radius 0..100 (42). На расчёт/MP не влияют.

## 5. Связи

BattleSpell.visualEffect и BattleVFXController.fireProfile/healProfile. Ассеты Assets/Settings/Battle/VFX/FireVisual и HealVisual.

## 6. Пример

Один и тот же расчёт Fire можно показать оранжевыми квадратами или позднее кадрами пламени.

## 7. Настройка в Unity

Create → FalseMemory → Battle VFX Profile; назначить профиль в Spell.visualEffect. К GameObject не прикрепляется.

## 8. Ошибки, ограничения и расширение

Нет реализации Ice/Thunder/Poison: новый профиль меняет только вид. Для новых художественных форм расширять shape/controller; игровой эффект отдельно в Abilities.

## 9. История изменений

2026-10-12 — кадры Skeleton64/Fire/Heal/Hit подключены к существующему представлению; игровые ресурсы не перенесены в анимации. [Текущие проверки](../../PixelVisuals.md).

- 2026-10-11 — 3.3 — профили Fire/Heal и подготовка новых визуальных вариантов.

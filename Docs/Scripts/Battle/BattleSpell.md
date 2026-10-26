# BattleSpell

## Подэтап5.5 —2026-10-26

BattleEffect дополнен PhysicalDamage в конце enum. StableId возвращает id, при пустом — прежний localizationId; старые ID/GUID сохранены. LocalizedDescription читает StringTable. requiredLevel/requirements/learningCost описывают изучение, damageType/scaling/attackMultiplier/accuracyBonus/holyPower — эффект. MeetsRequirements(level,values) проверяет уровень и все10 минимумов, возвращает bool. ScaledPower(unit) использует общий Scale для power, при пустых entries сохраняет базу. Fire/Heal сохраняют стоимость8/6, силу10/25 и известность. Awake/Update у ScriptableObject нет; назначать в AbilityLearning.catalog.

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleSpell.cs.

ScriptableObject определения заклинания и перечисления его эффекта/цели.

## 2. Как работает

Не содержит выполнения: меню читает подпись/стоимость, BattleAbilities проверяет и применяет данные.

## 3. Методы и функции

2026-10-12 — актуальная локализация: localizationId — стабильный ключ (назначены spell.fire/spell.heal), LocalizedName → GameText.Content(localizationId+".name",displayName). Старое serialized displayName сохранено как английский fallback, параметры/ссылки не меняются.

Методов/Unity callbacks нет. CreateAssetMenu FalseMemory/Battle Spell. BattleEffect: MagicDamage, RestoreHealth; BattleTarget: Opponent, Self.

## 4. Переменные и параметры Inspector

localizationId — новое serialized string-поле: spell.fire/spell.heal у заклинаний, unit.player/unit.skeleton у участников сцены. LocalizedName readonly читает таблицу, displayName остаётся английским fallback. Ни ID, ни боевые параметры это поле не меняет.

Этап 3.3: visualEffect — необязательный BattleVFXProfile; только оформление. Fire/Heal имеют назначенные FireVisual/HealVisual. Отсутствие ссылки не меняет CanCast/TryCast.

displayName — строка, код Fire. mpCost≥0, код 8. effect — код MagicDamage; target — код Opponent. power≥0, код 10. effectColor — Color, код оранжевый. Fire.asset: эти значения; Heal.asset: цена 6, RestoreHealth, Self, power25, зелёный.

## 5. Связи с другими системами

GameText и английская Unity StringTable; детали [LocalizationSystem](../../Systems/LocalizationSystem.md).

Assets/Settings/Battle/Spells/Fire.asset и Heal.asset назначены Player.BattleUnit.spells. BattleManager показывает, BattleAbilities применяет, UI отображает effectColor.

## 6. Пример работы

Копия Heal с power50 и mpCost12 → более сильное лечение при добавлении в Spells без правки BattleManager.

## 7. Настройка в Unity

Это ScriptableObject, не компонент GameObject. Для новой способности Create → FalseMemory → Battle Spell. Настрой имя, MP Cost, Effect, Target, Power, Effect Color; добавь ассет в Player.BattleUnit.Spells. Готовые Fire/Heal находятся в Assets/Settings/Battle/Spells.

## 8. Возможные проблемы и проверки

Новый тип эффекта требует расширения BattleAbilities; несколько союзников пока не поддержаны. Неверное сочетание цели/эффекта не является отдельной проверкой дизайна. Fire/Heal проверены тестами; произвольные новые заклинания не реализованы.

## 9. История изменений

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

- 2026-10-11 — 3.3: независимая ссылка visualEffect.

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

- 2026-10-11 — добавлена отдельная инструкция настройки в Unity по главному промпту; игровое поведение не менялось.

- 2026-10-11 — изменения этапа 2 сохранены в локальном Git после повторной проверки: 40 тестов Edit Mode и 17 тестов Play Mode прошли. Поведение этого файла описано выше; в задаче главного промпта его код повторно не переписывался.

- 2026-10-26 — интеграция бесклассового изучения и новых техник; см. актуальное описание выше.

# CharacterGrowthRules

## 1. Название и назначение

Исходник: Assets/Scripts/Battle/CharacterGrowthRules.cs.

Данные кривой EXP и роста уровня; не содержит самостоятельного персонажа/здоровья.

## 2. Как работает

Total EXP сохраняется накопительным. Порог уровня L: n×firstLevelExperience+n(n−1)/2×experienceStep, n=L−1. Рост статов определяется текущим уровнем, не прибавляется к базе повторно.

## 3. Методы и функции

`TotalForLevel(level)` → long абсолютного порога, нормализует диапазон. `LevelFor(totalExperience,currentLevel)` → int, повышает до достижимого уровня, не понижает ранее полученный при изменении кривой. `Remaining(totalExperience,level)` → int оставшегося EXP или0 на cap. `Growth(perLevel,level)` → int безопасного неотрицательного бонуса, насыщается int.MaxValue. `Bonuses(level)` → ItemStatBonus пяти характеристик. Unity callbacks нет.

## 4. Переменные и Inspector

maximumLevel99 (Inspector2…99, runtimeClamp), firstLevelExperience100 (≥1), experienceStep50 (≥0), healthPerLevel5/manaPerLevel2 (≥0), statsPerLevel ATK2 DEF1 MATK1 MDEF1 SPD1. DefaultCharacterGrowth имеет эти значения. Getter MaximumLevel нормализует поле; числовые пределы безопасны при некорректных scripted значениях.

## 5. Связи

BattleUnit.growthRules читает этот ассет; Health остаётся владельцем HP, Equipment — отдельными бонусами.

## 6. Пример

100 EXP → уровень2;250 →3;450 →4. На уровне4 ATK base15+growth6=21 до экипировки.

## 7. Настройка в Unity

Create → FalseMemory → Character Growth Rules; назначить Player.BattleUnit.growthRules. Не компонент. Assets/Settings/Progression/DefaultCharacterGrowth.asset; Enemy без growthRules.

## 8. Возможные проблемы

При live-редактировании профиля значения бонусов меняются, здоровье пересчитывается при последующей награде; баланс менять предпочтительно вне Play. Порог long может быть выше int.MaxValue накопительного опыта — недостижимый уровень не выдаётся. Классов/primary attributes пока нет.

## 9. История изменений

2026-10-12 — создан в подэтапе5.1. [Проверки](../../ProgressionStage5.md).

# BattleDamage

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleDamage.cs`. Чистая композиция обычного и Holy-канала, категории существ и результат урона.

## 2. Как работает

Чистая композиция обычного и Holy-канала, категории существ и результат урона. Расширяет прежние Equipment/BattleUnit/Health; отдельного владельцаHP нет.

## 3. Методы и функции

Compose(basis,wisdomBonus,targetCategories,holyResistance,intrinsicHoly,pureHoly) возвращает BattleDamageResult. Для обычного удара сохраняет basis, добавляет basis×WISbonus+intrinsic только Undead/Unholy. Для pureHoly весь basis становится Holy и WIS повторно не начисляется. Сопротивление применяется отдельно, одно AwayFromZero округление. Неeligible pureHoly=0; Human/Beast не получают добавку. Total насыщается int.MaxValue. Конструктор результата ограничивает компоненты снизу0. Unity callbacks/событий и записиHP нет.

## 4. Переменные и Inspector

CreatureCategory — flags None/Human/Beast/Undead/Unholy со стабильными значениями0/1/2/4/8. BattleDamageResult readonly: BaseDamage/HolyDamage/Total. Параметры приходят из BattleUnit; Inspector только у владельца, не у статического класса.

## 5. Связи

ArmorScalingData → WeaponScalingData.Scale/CharacterAttributeRules; EquipmentState/BattleUnit → BattleDamage/Health; Editor использует AssetDatabase/Localization/PrefabUtility; tests используют NUnit/UnityTest и существующие fixtures.

## 6. Пример

100 normal+WIS30 противUndead =100+10; HolyResistance50%=100+5. ПротивHuman=100+0.

## 7. Настройка в Unity

Не прикреплять. Настроить категории/сопротивление существ на BattleUnit. Вызывать только из DealDamage после основного расчёта; не подключать Compose к событию Health, чтобы избежать рекурсии.

## 8. Возможные проблемы

Сохранять явный opt-in useArmorScaling, категории задавать данными, не именем. ЧистыйHoly не наносит урон Human/Beast. Новые6 armor — тестовый контент с прежними иконками. Дискового save пока нет. Проверки и ограничения — [ClasslessStage54](../../ClasslessStage54.md).

## 9. История изменений

- 2026-10-26 — реализация и первичная документация подэтапа5.4.

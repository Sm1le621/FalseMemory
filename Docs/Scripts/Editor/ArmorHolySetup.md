# ArmorHolySetup

## 1. Название и назначение

Исходник: `Assets/Editor/ArmorHolySetup.cs`. Адресная миграция существующей SampleScene и данных5.4 через Unity MCP.

## 2. Как работает

Адресная миграция существующей SampleScene и данных5.4 через Unity MCP. Расширяет прежние Equipment/BattleUnit/Health; отдельного владельцаHP нет.

## 3. Методы и функции

ApplyStage54() возвращаетvoid, требует остановленныйPlay и активную SampleScene. Включает ratingK20, создаёт/обновляет6 своих ItemData/Englishkeys/prefabinstances, SacredBlade HolyBase5, категории игрока/скелета; сохраняет только свои ассеты и сцену. Add(table,key,value) обновляет/добавляет строку. Проверяет коллизиюID. Unity runtime callbacks нет.

## 4. Переменные и Inspector

Внутренние массивыnames/files/first/second/grades/defenses — профиль миграции, не runtime Inspector. Все6 вещей Chest; уAssassin evasion. Исходные иконки временно переиспользуются.

## 5. Связи

ArmorScalingData → WeaponScalingData.Scale/CharacterAttributeRules; EquipmentState/BattleUnit → BattleDamage/Health; Editor использует AssetDatabase/Localization/PrefabUtility; tests используют NUnit/UnityTest и существующие fixtures.

## 6. Пример

После ApplyStage54 в TestLoot появляются Pickup_Scaling_HeavyPlateArmor и ещё5, прежние объекты сохраняются.

## 7. Настройка в Unity

Не прикреплять. Вызов через MCP execute_code: ArmorHolySetup.ApplyStage54(). Не запускать вPlay/на другой сцене. Повторный запуск перезаписывает только собственные6 профилей; не является автоматическим importer.

## 8. Возможные проблемы

Сохранять явный opt-in useArmorScaling, категории задавать данными, не именем. ЧистыйHoly не наносит урон Human/Beast. Новые6 armor — тестовый контент с прежними иконками. Дискового save пока нет. Проверки и ограничения — [ClasslessStage54](../../ClasslessStage54.md).

## 9. История изменений

- 2026-10-26 — реализация и первичная документация подэтапа5.4.

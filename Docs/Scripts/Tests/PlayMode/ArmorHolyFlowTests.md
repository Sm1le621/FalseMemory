# ArmorHolyFlowTests

## 1. Название и назначение

Исходник: `Assets/Tests/PlayMode/ArmorHolyFlowTests.cs`. Интеграционные сценарии на настоящих SampleScene/BattleScene.

## 2. Как работает

Интеграционные сценарии на настоящих SampleScene/BattleScene. Расширяет прежние Equipment/BattleUnit/Health; отдельного владельцаHP нет.

## 3. Методы и функции

PhysicalAndFireDealHolyOnceOnlyToUndeadAndHealingNeverTriggersIt() проходит реальный бой: WIS30 Attack12+1, Fire20+2, Heal безурона; Human Attack12 безHoly. ArmorRequirementsDefenseAndEvasionUpdateWithoutAccumulation() проверяет отказ/экипировку Assassin приAGI30DEX30, Defense/Evasion, повторный пересчёт и снятие без измененияMovementSpeed. Оба UnityTest возвращаютIEnumerator; используют общийSetup/Cleanup/BeginBattle/WaitUntil. Других callbacks нет.

## 4. Переменные и Inspector

Меняет только временное Play-состояние. Требует сохранённые pickups/rules; начальнаяAGI10, reqAssassin15. Тестовые координаты/статы не сохраняются в сцену.

## 5. Связи

ArmorScalingData → WeaponScalingData.Scale/CharacterAttributeRules; EquipmentState/BattleUnit → BattleDamage/Health; Editor использует AssetDatabase/Localization/PrefabUtility; tests используют NUnit/UnityTest и существующие fixtures.

## 6. Пример

Enemy60→47→25; лечение оставляет25; смена категорииHuman→13 послеобычнойатаки.

## 7. Настройка в Unity

Не прикреплять. Test Runner PlayMode → FalseMemory.Battle.PlayModeTests. Ручное ощущение силы/визуальный дизайн остаются предметом приёмки.

## 8. Возможные проблемы

Сохранять явный opt-in useArmorScaling, категории задавать данными, не именем. ЧистыйHoly не наносит урон Human/Beast. Новые6 armor — тестовый контент с прежними иконками. Дискового save пока нет. Проверки и ограничения — [ClasslessStage54](../../../ClasslessStage54.md).

## 9. История изменений

- 2026-10-26 — реализация и первичная документация подэтапа5.4.

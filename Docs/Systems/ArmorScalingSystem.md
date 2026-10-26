# Armor Scaling

## Баланс и регрессия 5.7 —2026-10-26

После сравнительныхсимуляций AssassinArmor baseevasion6%вместо 3%;grades/Defense/requirements прежние. При AGI30/DEX40 итоговая itemevasion10.32%,вместес primary20.32%. Исходные матрицы EditMode ссобственным тестовым base3%остаютсявалиднымиконтрактамиформулы. [Баланс](../ClasslessStage57.md).

## Назначение и логика

Новые armor ItemData явно включают ArmorScalingData. EquipmentState сначала агрегирует flat bonuses, затем один раз добавляет scaled физическую/магическую защиту и evasion. Primary-бонусы остаются отдельными: рекурсивного пересчёта/накопления нет. Требования проверяются без самой и заменяемой вещи; повторная экипировка атомарна.

## Формулы и параметры

Scale использует ту же таблицуgrades/soft caps, что оружие: base×(1+суммаgrade×score), cap1.5. Defense — рейтинг, не проценты. DefaultBattleRules K20: входящийpower×20/(20+Defense). Затемminimum/crit/Defend/ceil. Более высокая защита полезна с убывающей отдачей, минимум1 не даёт полной неуязвимости. Magic используетMagicDefense; Defend действует только наphysical как раньше.

## Контент и Unity

6 тестовых Chest-предметов в Assets/Settings/Inventory и pickups в TestLoot. HeavyPlateEND/STR, KnightVIT/END, AssassinAGI/DEX+evasion, MageWIS/MAG, HolyWIS/VIT, BattlemageMAG/END. Настройка: ItemData Armor/useArmorScaling, bases/requirements/grades. Другие слоты и прежние предметы сохраняются; новые шлемы/ботинки добавляются теми же данными без новой системы. Иконки переиспользованы, художественная работа не выполнена.

## Проверки и расширение

Edit проверяет суммирование/повтор/снятие и mitigation. Play проверяет реальную экипировку/requirement/evasion/ходьбу. [Результаты](../ClasslessStage54.md). После потери requirements предмет не удаляется, новый scaling-вклад отключается; это ограничение первой версии. При добавлении primary-бонусов важно не финансировать собственное требование.

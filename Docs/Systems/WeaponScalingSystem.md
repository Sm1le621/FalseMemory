# Weapon Scaling —подэтап5.3

ItemData.useWeaponScaling явно включает вложенные WeaponScalingData. Старые предметы продолжают прежние flat bonuses. Новые6 ассетов/requirements/grades описаны в [ClasslessStage53](../ClasslessStage53.md).

Бой: BattleUnit проверяет пригодность надетого main weapon → Damage поэффективнымprimary/общимsoft caps → замена соответствующего базовогоAttack без повторнойSTR/DEX/MAG-поправки → BasicAttackPower и physical/magicDefense цели → прежний BattleCalculator/crit/miss/Defend → единичное применение BattleManager. Сила Tooltip до защиты отличается от реально снятыхHP.

Экипировка: EquipmentController связывает domain State с attributes владельца. До bag/gear/swap вычисляются primary без проверяемого предмета/вытесняемого источника. Отказ сохраняет владение. При последующем снижении требований предмет не уничтожается/не выбрасывается, Scaling основного оружия не используется до восстановления пригодности.

Настройка в Inspector: ItemData.category Weapon/useWeaponScaling=true, baseDamage/type/weaponType/requirements/scaling; ранги и общий предел задаются CharacterAttributeRules. Один stat в массиве не суммируется дважды. Новую вещь добавлять с уникальным ID/English StringTable keys, не копировать целые combat-классы. Тестовые pickups созданы через существующий Prefab/MCP, иконки временные.

Полная информация выводится существующим InventoryTooltip: база/бонус/итог/ранги/минимальные и текущиеstat. Обычные Short details не перегружаются. Holy/Armor Scaling появятся5.4, обучение способностям5.5, Character5.6. Реальные114 Edit/59 Play и просмотр FullHD приведены в отчёте; итоговая балансная приёмка ещё не пройдена.

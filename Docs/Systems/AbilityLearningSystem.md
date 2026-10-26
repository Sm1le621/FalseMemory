# Изучение способностей

## Назначение и состояние

Бесклассовая система расширяет BattleSpell/BattleAbilities. AbilityLearning на Player хранит стабильные learnedIds, отдельные Learning Points и максимальный учтённый level. Старый unit.spells остаётся источником известности Fire/Heal; новый каталог содержит все6 способностей. Не создаётся вторая система HP/MP или сохранений.

## Последовательность

Availability проверяет известность → каталог/ID → level/эффективные атрибуты → LP. TryLearn вне боя атомарно списывает цену и добавляет ID, затем Changed. Повторный вход/повторное изучение отклоняются. Бонусы gear могут выполнить требования обучения; последующее снятие не забывает умение. KnownAbilities соединяет совместимые и новые известные умения, BattleManager использует их в прежнем меню Magic. LP и attribute points независимы.

## Контент и формулы

Power Strike STR20,6MP, физическая сила×1.5. Quick Strike DEX18/AGI15,5MP, сила×1.15 и−20 процентных пунктов miss. Fire MAG15/INT12 для нового обучения, прежние10 power/8MP; уже известный Fire доступен при стартовых10. Heal прежние25/6MP. Greater Heal MAG20/WIS20,50 base/12MP. Sacred Strike WIS25/STR15, физическая сила×1.2, дополнительныйHoly8 толькоUndead/Unholy,8MP. У всех requiredLevel1/learningCost1; параметры Inspector.

BattleSpell поддерживает power Scaling entries, damageType, target, requirements, description. Physical использует ResolvePhysical/crit/miss/Defend, Magic — MagicDefense, Heal — существующий Health.Heal. MCR применяется один раз, максимум30%, платное действие≥1MP. Holy собирается отдельно единственным DealDamage.

## Unity и расширение

Assets/Settings/Battle/Spells содержит прежние2 и новые4 ассета; Player.AbilityLearning.catalog связан через MCP. Стартовые3 LP и1 LP/level — настраиваемый тестовый механизм до будущего учителя/предметов; финальная экономика требует настройки. Для новой способности с существующим effect достаточно ассета/English StringTable/catalog; новый effect требует кода и тестов. Character UI следующего подэтапа даст доступ к изучению.

## Проверки и ограничения

[Текущие запуски и ограничения](../ClasslessStage55.md). Состояние сохраняется между картой/боем в сессии, после полного New Game сбрасывается. Название Magic пока включает и физические техники; отдельной Skill-команды пользователь не запрашивал. Нет NPC-обучения, отдельного постоянного save или классов.

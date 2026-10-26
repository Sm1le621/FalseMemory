# Attribute Scaling —подэтап5.3

Система задаёт настраиваемую убывающую эффективность существующих10 CharacterAttributes. Назначенный DefaultCharacterAttributes включает classless; CharacterAttributes хранит одну базу/очки, BattleUnit/Health остаются владельцамиEXP/MP/HP.

Последовательность: EXP меняетlevel → SynchronizeLevel начисляет ещё не выданные points → TryIncreaseBase покупает+1 → Changed → RefreshDerivedStats → HP/MP capacity/бой/Inventory. Рост вместимости не лечит; уровень сам не повышает все характеристики. Gear/temporary складываются с базой перед вычислением кривой, class modifier в classless игнорируется.

AttributeSoftCapCurve содержит thresholds/gains и интегрирует положительный последний gain за пределами последнего порога. CharacterAttributeRules централизует кривые/grades/коэффициенты/rounding; никакого отдельного уровня/второгоPlayerStats нет. Полные10 кривых и значенияHP/MP — [ClasslessStage53](../ClasslessStage53.md).

В Unity менять существующий профиль на Player.CharacterAttributes.rules. База ограничена99, эффективные gear-бонусы могут быть выше. Для расширения конкретного эффекта допускается отдельная curve в прежнем конфиге, без записи производных значений обратно в base. Сохранение serializable-полей в текущей сессии не является дисковым RPG-save.

Edit/Play проверки выполнены и перечислены в отчёте. CharacterC/Armor/Holy/learning/полный баланс ещё следующие подэтапы. Stamina/убеждение/торговля не реализованы наличием PreparedScore.

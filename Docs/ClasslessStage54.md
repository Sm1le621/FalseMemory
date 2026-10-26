# FalseMemory — подэтап5.4: броня, защита и Holy

2026-10-26. Расширены существующие Equipment/BattleUnit/BattleAbilities. Отдельная система здоровья не создавалась; старые предметы/объекты/Prefabs/анимации сохранены.

## Реализовано

- ArmorScalingData с раздельными physical/magic/evasion bases, grades и требованиями. Явный useArmorScaling защищает старые ассеты от неявного включения вложенных данных Unity.
- 6 тестовых Chest-предметов: Heavy Plate (END A/STR B), Knight (VIT A/END B), Assassin (AGI A/DEX C и evasion), Mage Robes (WIS A/MAG C), Holy Armor (WIS S/VIT D), Battlemage (MAG B/END B). Их pickup находятся под TestLoot; иконки взяты из существующего контента. Это прототипы, не новый художественный набор.
- DefaultBattleRules включает diminishing defense: power×20/(20+Defense). Physical затем учитывает crit/Defend и один ceil; magic использует MagicDefense и прежнее отсутствие Defend. Минимум1 сохраняется. Старые независимые профили с false сохраняют прежнее вычитание.
- Новые базовые значения: Attack игрока12, ответ скелета9, ответ под Defend5; Fire20. Исторические10/4/2/19 больше не соответствуют текущему профилю.
- HolyBonus WIS10/20/30/40/50/60/80/99:0/5/10/12.5/15/17.5/19.5/21.4%. WIS не даёт MP. Добавка работает у всех damaging actions только по Undead/Unholy; категории не зависят от имени.
- BattleDamageResult хранит Base/Holy/Total. DealDamage один раз вызывает Health.TakeDamage; Heal/предметы восстановления не создают Holy. HolyResistance ограничен0…1 и применяется к отдельному компоненту. SacredBlade получил отдельную scaled HolyBase5. PureHoly не порождает повторную WIS-добавку и не действует на Human/Beast.
- Tooltip брони показывает base→final, evasion/grades/requirements; сообщения боя показывают Holy-компонент через English StringTable.

## Фактически выполненные проверки

- Компиляция через Unity завершена, isCompiling=false. Все130/130 Edit Mode прошли, job `1c6fb61bd0b0463a97d11b74df3a1d46`.
- Все61/61 Play Mode прошли, job `00ff29c2c3cf4ffc9ce1c53d5b0d6811`: настоящий бой/Attack/Fire/Holy/Heal/Human, требования Assassin/защита/evasion/снятие/повторный пересчёт, а также полная прежняя регрессия переходов, MP, предметов, экипировки и анимаций. Ожидаемые количества урона обновлены по новой формуле; прямой тестовый TakeDamage не изменён.
- Через MCP проверены активная SampleScene,6 armor pickups, сохранённая сцена без dirty, остановленный Play, WIS30=10%; профиль CharacterAttributeRules сохранён адресно.
- Console: ошибок игры/компиляции нет; присутствует предупреждение службы Unity Account API о недоступности в течение30 секунд. Оно не относится к новой боевой логике. При служебном чтении был ошибочно указан путь профиля в Battle вместо Progression; запрос исправлен, игровые данные этим запросом не менялись.
- Отдельная ручная визуальная оценка armor-tooltip и баланс владельцем не выполнялись. В этой задаче обычный Game View отдельно не снимался; автоматические Play Mode сценарии действительно запускались.

## Ограничения и следующий шаг

Все6 новых armor занимают Chest; другие типы брони могут использовать тот же формат. При последующей утрате требования вещь остаётся в слоте, её новый scaling-вклад отключается; автоматического удаления нет. Тесты категорииUndead не означают создание готового зомби. Дискового save ещё нет, состояние живёт в текущей сессии. Следующий подэтап5.5 — бесклассовое изучение способностей; этап6 остаётся закрыт до приёмки5.7.

Документы: [ArmorScalingSystem](Systems/ArmorScalingSystem.md), [HolyDamageSystem](Systems/HolyDamageSystem.md), отдельные Docs/Scripts для всех изменённых C#.

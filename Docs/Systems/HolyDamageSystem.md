# Holy Damage

## Баланс и регрессия 5.7 —2026-10-26

Добавлены реальные Play проверки MagicStaffbasic27+3 и SacredBladebase42/intrinsic9/resistance50%→Holy7;Beast бездобавки. Отдельный HolyScaling теперьвиденв Character/Inventorytooltip. Zombie fixture остаётсяданнымисимуляции,готового NPC/спрайтазомбине создано.

## Назначение и события

BattleCalculator рассчитывает основной урон, BattleUnit.DealDamage вызывает чистый BattleDamage.Compose, получает Base/Holy и один раз меняет Health. WIS-добавка работает на физические/магические повреждения только против flagsUndead/Unholy. Имя существа не используется. Heal и предметы восстановления не проходят этот путь.

## Формула и настройка

WIS10=0%,30=10%,60=17.5%,99=21.4%; CharacterAttributeRules.holyBonusCurve настраивается отдельно. Holy=(baseпослеmitigation/crit/Defend×WISbonus+scaledIntrinsicHoly)×(1−holyResistance), одно округлениеAwayFromZero. Для pureHoly весьbasisсчитаетсяHoly, WIS не запускается повторно. HolyResistance0…1 наBattleUnit, категорииflags нацели. SacredBlade имеетотдельнуюHolyBase5 иWIS/STRgrades. Human/Beast не получаютHoly; чистыйHolyпротивнихнулевой.

## UI, связи и расширение

BattleManager показывает общий урон и английское (+N Holy) черезGameText. Компоненты доступны вLastDamageDealt, безсобытияповторногоурона. Категориюзомби можно задатьUndead безнового класса, но готового зомби-контента пока нет. СопротивленияэлементовкромеHolyне добавлены. [Контракты иPlay](../ClasslessStage54.md); невыдаватьпараметризованныйтесткатегориизаигрупротивновогозомби.

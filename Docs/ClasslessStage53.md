# FalseMemory —5.3: бесклассовая прокачка и Weapon Scaling

## Текущий результат2026-10-26

Подэтап5.3 технически реализован и проверен в существующем проекте. Старый план Warrior/Mage/Rogue отменён новым запросом владельца. Использованы прежние CharacterAttributes/BattleUnit/Health/Inventory/Equipment, без второй системы персонажа и без дискового SaveSystem. Idle/Walk приняты владельцем; их рисунки/клипы не менялись.

Это не завершение всего этапа5: броня/Holy/learning/CharacterC/контрольные сборки относятся к5.4–5.7 и пока не реализованы этим подэтапом.

## Очки и совместимость

DefaultCharacterAttributes.classlessProgression=true; в коде значение по умолчанию false сохраняет старую модель для отдельно созданных legacy-профилей. Автоматический рост всех10 primary отключён только для classless. Существующая EXP-кривая100/250/450… и cap99 сохранены. Новый параметр attributePointsPerLevel=1 выдаёт одно свободное очко за каждый новый уровень. Это явное изменение экономики: прежде уровень давал+1 ко всем10 stats, теперь выбор делает игрок.

CharacterAttributes хранит unspentPoints/highestPointLevel, начисляет разницу только один раз, не выдаёт повторно при пересчёте/скрытии/возврате из боя. Для уже существующего level>1 учитываются пройденные уровни; прежние базовые значения не сбрасываются. TryIncreaseBase атомарно списывает1 и добавляет1 к выбранной базе, отклоняет reentry/недостаток очков/неверный enum/99. Базовый диапазон0…99, gear может повышать эффективное значение сверх99; кривые продолжают последний marginal gain. Старые serialized classBonuses сохраняются для совместимости, но classless их не применяет и не показывает как класс. Отдельного класса нет.

Текущие HP/MP не увеличиваются при росте вместимости. Начало новой игры по-прежнему заполняет ресурсы. UI распределения очков ещё не создан: оно будет в5.6; API уже реально проверено Play-тестом.

## Кривые и параметры

AttributeSoftCapCurve интегрирует прирост каждого очка. Пороги не запрещают дальнейшее повышение, последний прирост остаётся положительным.

- STR/DEX/MAG: пороги20/55/80, прирост1/.5/.25/.1.
- AGI/WIS/CHA/MCR:30/60, прирост1/.5/.2.
- END:30/50/60, прирост1/.5/.25/.1.
- VIT:40/60, прирост8/4/1HP за очко. База при10 —100HP;20→180,40→340,60→420,99→459.
- INT:40/60, прирост4/2/.5MP за очко. База при10 —40MP;40→160,60→200,61→201,99→220. Округление итоговой поправки один раз AwayFromZero; WIS больше не добавляетMP.

Коэффициенты/кривые находятся в прежнем CharacterAttributeRules. Остальные производные используют разницу интеграла относительно reference10 вместо линейной разницы. AGI даёт инициативу/уклонение и явно настроенный item Scaling, **не скорость ходьбы**. Прежний agilityMovement оставлен как serialized-поле совместимости, в профиле0, MovementAdjustment возвращает0. Скорость4 и анимационный WalkSpeed не переписаны. MCR уменьшает стоимость с максимумом30%; платная способность не дешевле1MP. Stamina, carry limits, торговля и убеждение остаются будущими механиками, PreparedScore не объявляется их реализацией.

## Weapon Scaling

ItemData содержит WeaponScalingData и явный useWeaponScaling=false по умолчанию. HasWeaponScaling требует этот флаг/categoryWeapon/данные. Это важно для Unity: старый вложенный объект может быть материализован десериализатором даже без записанных данных. Старые IronSword/WoodenStaff сохраняют прежние flat bonuses/ID/GUID и не получают новый Scaling автоматически.

Ранги None/E/D/C/B/A/S имеют коэффициенты0/.1/.2/.4/.6/.8/1. Для каждого stat: normalized score = integral(value) / integral(1) / scalingReferencePoints50. Sum(score×grade) ограничен maximumWeaponScalingBonus1.5. Damage = roundAway(baseDamage×(1+sum)), с ограничением диапазонаint. Дубликат одной характеристики в массиве не начисляется повторно.

Новое оружие заменяет базовый Attack соответствующего physical/magic-канала и его глобальную STR/DEX/MAG-поправку; отдельные flat bonuses остальных предметов добавляются обычным способом. Поэтому STR/DEX/MAG не удваиваются. BasicAttackPower/MagicalWeaponAttack выбирают Attack и соответствующую Defense цели. Сохраняются прежние crit/miss/Defend и единственная точка применения урона в BattleManager. Показанная сила оружия — значение до защиты/случайного исхода, не гарантированное количество снятыхHP.

Созданы шесть отдельных ItemData:

- Greatsword:40 Physical, STR S/DEX E, требуетсяSTR25, двуручный.
- Longsword:28 Physical, STR B/DEX C, дополнительных stat-требований нет.
- Rapier:24 Physical, DEX S/STR E, требуетсяDEX18.
- Dagger:18 Physical, DEX A/AGI C; AGI здесь усиливает только это оружие.
- Magic Staff:20 Magic, MAG S/INT D, требуетсяINT20/MAG15.
- Sacred Blade:30 Physical, WIS A/STR B, требуетсяWIS20. В5.3 это обычный физический канал; Holy будет в5.4.

ПриSTR25/DEX10 Greatsword имеет40+19=59 силы, не59 гарантированного урона. При базовых10/10 preview49, но экипировка запрещена требованиемSTR25. Иконки временно взяты из существующего IronSword; новый pixel art не генерировался.

## Требования и UI

EquipmentController привязывает transient callback к прежней EquipmentState. Перед надеванием/drag/swap вычисляются эффективные primary без самого проверяемого кандидата и без вытесняемой вещи. При gear↔gear проверяются оба участника будущего обмена. Флаги changing и атомарный InventoryState обмен сохраняются, отказ не расходует/не теряет предмет.

Наличие requirements не означает автоматического удаления уже надетого предмета при временном снижении stats. BattleUnit повторно проверяет пригодность основного оружия; если requirements больше не выполняются, Scaling не используется. Принудительное снятие вещей не добавлялось.

Tooltip показывает Name/Rarity/WeaponType/Base+Bonus=Total/Scaling/Requires с текущими значениями/Description. Предварительная сила использует предполагаемую экипировку после замены main weapon. Неудачная экипировка показывает конкретные требования через GameText. Полный текст не помещается в короткую нижнюю строку Inventory, поэтому отображается в существующей всплывающей подсказке. Classless-tooltip характеристик убирает Class и показывает ближайшийsoft cap.

## Изменения Unity и файлов

Через MCP выполнен ClasslessProgressionSetup.ApplyStage53: обновлён существующий DefaultCharacterAttributes, добавлены английские строки в GameText_en-US/SharedData, созданы6 weapon assets и6 экземпляров существующего WorldItemPickup Prefab под TestLoot. Координаты новой группы слева: x−5…−7, y−1.7/−2.7. Существующая SampleScene сохранена; прежние объекты не удалялись. Stable worldId scaling_weapon_* и item ID weapon.*.

Изменены CharacterAttributeRules/Attributes, BattleUnit/Manager, ItemData/Presentation, EquipmentState/Controller, InventoryTooltip/Window/AttributeRowView. В PlayerController добавлен только безопасный выход для неинициализированного Animator при domain reload, без изменения физики/кадров. Новые AttributeSoftCapCurve/WeaponScalingData/ClasslessProgressionSetup, Edit/Play тесты. У каждого созданного/изменённого C# обновлён документ Docs/Scripts с историей. Новые системные документы — [AttributeScalingSystem](Systems/AttributeScalingSystem.md), [WeaponScalingSystem](Systems/WeaponScalingSystem.md); обновлены AGENTS/Roadmap/Architecture/README/Changelog/CharacterStatsSystem/MasterDevelopmentPlan.

## Проверки именно5.3

- Компиляция Unity завершена. Финальные **114/114 Edit Mode**, job `136711df9c2448379395a2aa872d028d`: все10 stats/пороги−1/на/после, заданныеHP/MP,99, атомарные points/reentry, ранги/дубли/отсутствиеScaling/равный бюджет, self-funded требования/отказ без потери, opt-in старых предметов.
- Финальные **59/59 Play Mode**, job `620c1a8761cf475db5ce19e98138d29e`: один новый сценарий реальных level/points/VIT/оружия/боя/возврата/Tooltip, все прежние боевые/инвентарные/анимационные сценарии. Численные ожидания MP обновлены под40; специально малый запас в тестах задаётся явно, проверки расхода/очереди/предметов не ослаблены.
- Первый Play `79ffaf9ae9804f8e8aaf831792d3edae`:52/59. Выявил реальную ошибку неявного Scaling старого меча и несколько прежних ожиданий30MP. Исправлено явным флагом/согласованными ресурсными ожиданиями; полный повтор прошёл. Первоначальная недоступностьMCP устранена восстановлением подключения; во время первого domain reload наблюдались Animator warnings, добавлен безопасный guard.
- Обычный Play/1920×1080: Greatsword приSTR10 отклонён и остаётся в сумке, Tooltip содержит49 силы/STR25(current10), overflow=false; при временномSTR25 надевание успешно, EffectiveAttack59 совпадает с единственным weapon.Damage, walk4. Снимок `Temp/ClasslessStage53/requirements.png` просмотрен: текст/границы читаемы. Это временные runtime значения, в сцену не сохранены.
- После проверок Play остановлен, исходный Game View preset восстановлен, SampleScene dirty=false. Последняя прочитанная Console0 ошибок/предупреждений. Самостоятельная сборка и художественная/балансная приёмка не заявляются выполненными. 3440×1440 относится к обязательной будущей проверке Character в5.6.

## Git и следующий шаг

Исходный HEAD5dc9c06, ветка stas_megamosk_build. Посторонний LiberationSans SDF fallback сохранён и исключается из коммита. Подэтап сохраняется отдельным локальным `feat: implement classless progression and weapon scaling`; push запрещён без команды владельца. Следующий разрешённый подэтап5.4 — Armor Scaling/защита/Holy; весь этап5 ещё не завершён.

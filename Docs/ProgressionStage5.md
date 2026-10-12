# FalseMemory — подэтап5.1: EXP и уровни

2026-10-12. Статус5.1 **Tested**, этап5 In Progress. Исходный HEAD визуального подэтапа a9a9117. Предыдущая ручная граница4 уже принята владельцем, поэтому разработка продолжена после Skeleton64/VFX.

## Выполнено

Добавлены настраиваемые cumulative EXP-пороги, повышение нескольких уровней за награду, рост HP/MP/ATK/DEF/MATK/MDEF/SPD, Level Up и Next/Maximum Level в существующих Inventory/Victory. Профиль только на исходном Player; Enemy/его награда20/стартовый баланс первого уровня сохранены.

DefaultCharacterGrowth: cap99, первый переход100, шаг стоимости50; абсолютные пороги0/100/250/450/700. Growth после первого уровня: HP5, MP2, ATK2, DEF1, MATK1, MDEF1, SPD1. На level4 ATK15+6=21 до экипировки; MaxHP115/MaxMP36. Это исходные настраиваемые значения, не утверждённый окончательный баланс.

Базовые поля/EquipmentState не мутируют при вычислении бонуса. Рост уровня вычисляется от level, повторная награда1 EXP не прибавляет прежние bonus снова. HP остаётся в Health; SetMaximumHealth сохраняет повреждение, меняет максимум/старый HUD. CurrentMP тоже сохраняется. Нет автоматического лечения/воскрешения. Cumulative EXP saturates int.MaxValue; long-пороги не переполняются, отрицательные параметры нормализуются, рекурсивный AddExperience через событие отклоняется.

## Файлы и Unity

Созданы CharacterGrowthRules.cs, CharacterGrowthTests.cs и CharacterGrowthFlowTests.cs с .meta и отдельными Docs/Scripts. Изменены BattleUnit (curve/growth/event), Health (capacity API), BattleUI (Level Up result), InventoryWindow (progress event/трёхстрочный header). Существующие идентификаторы/Health/инвентарь/экипировка/переходы не заменены.

Через MCP создан Assets/Settings/Progression/DefaultCharacterGrowth.asset, назначен Player.BattleUnit.growthRules и сохранена существующая SampleScene. BattleScene не перестраивалась. Native English StringTable/SharedData получили5 устойчивых ключей, сейчас141 запись. Русская документация и английский UI сохранены. Изменений Prefab/дополнительных зависимостей нет.

[ProgressionSystem](Systems/ProgressionSystem.md) описывает формулы и настройку. Обновлены документы всех7 созданных/изменённых скриптов, Health/Inventory/Combat/Localization systems, README/Architecture/Changelog/Roadmap и руководства.

## Проверки

Компиляция завершена, финальная Console **0 ошибок/0 предупреждений**. Во время reload наблюдалось прежнее Unity Account API timeout; финальное чтение пустое. Console вручную не очищалась.

- **81/81 Edit Mode Passed**, job 58a699c2ca944942bb2f805f38673699, 0.2536109 секунды.
- **42/42 Play Mode Passed**, job adcdbd33eec84aafa24c461ec7290974, 30.9254509 секунды.
- Пороги/границы/cap/несколько уровней/long/насыщение/отрицательные данные/непонижение уровня при изменении кривой.
- Награда450→level4, одна ProgressChanged, отсутствие вложенной награды/повторного бонуса, сохранение повреждения и MP; capacity clamp не воскрешает.
- Настоящая победа после подготовки90EXP/80HP/22MP/EnemyHP1 →110EXP/level2/Next140/HP80 из105/MP22 из32/ATK17. Повторное TryGrant отклонено, позиция/HUD/исходные InventoryState и EquipmentState сохранены при возврате.
- Первый новый Play-прогон41/42 открывал Inventory до конца Transition. Исправлено ожидание EncounterActive/IsTransitioning=false и проверка Open; игровой transition не переписывался. Последующий полный42/42 проходит.
- Все прежние78 Edit/40 Play визуала/боя/ресурсов/input/Inventory/Equipment также входят в текущую регрессию.
- Через MCP проверена SampleScene:0 Missing Script/сломанных Prefab/проблем. Стартовые level1/HP100/MP30 и Enemy60HP/20EXP сохраняются.

## Game View

**8 сочетаний:** Inventory после level-up и реальный Victory после повышения ×1280×720/1920×1080/2560×1440/3440×1440. Нет выхода за экран/переполнения/кириллицы/отсутствующих glyphs. Full HD кадры просмотрены: [Inventory](Images/ProgressionStage5-inventory-1920.png), [Victory](Images/ProgressionStage5-victory-1920.png). [Метрики/тексты/jobs](Verification/ProgressionStage5Layout.json).

Для кадров в runtime подготовлено450EXP →level4, повреждение20HP/расход8MP: HP80/115, MP22/36, ATK21, Next250. Затем AddExperience249 без повышения и настоящая Attack/Victory дала20: total719, level5, Next281, HP80/120, MP22/38. Подготовка не сохранена в сцене; кадры результата показывают фактическую боевую награду, а не вручную записанный Level Up.

После проверок Play остановлен, SampleScene чистая; runInBackground=false, Game View индекс0/исходные7 presets восстановлены. Удалены только два созданных AI Growth размера. Прежняя позиция/компоненты/враги не удалялись.

## Ограничения и следующий шаг

Это5.1, не завершённый этап5: Strength/Vitality/Intelligence/Mind/Agility/Luck и единые формулы5.2, классы5.3, новые способности5.4, Character C5.5 ещё не реализованы. Нет save на диск/standalone; restart по прежнему начинает новую сессию. DefaultGrowth не является моделью всех будущих классов.

На текущей точке нужна ручная оценка нового Skeleton/Heal/Fire/темпа и роста владельцем: автоматические тесты проверяют математику/ресурсы, но не художественное впечатление/удобство баланса. Это проверка нового результата, прежний этап4 повторно не сдаётся. Следующий программный подэтап —5.2.

## Как проверить вручную

1. SampleScene → Play: понаблюдать Idle/Move нового скелета, коснуться; проверить его Attack/Hurt, Fire/Heal, числа и Space. Победить, проверить Death/Victory/возврат.
2. В новом Play до встречи выбрать Player → BattleUnit → Current Experience=90. Это только подготовка ниже порога. Победить: Level Up! Level2, total110, Next140; maxHP105/maxMP32. Остаток HP/MP сохраняется.
3. Вернуться и открыть I: проверить уровень/Next, HPHUD, экипировку и выросшие характеристики. При необходимости повторить на широком экране.
4. Для нескольких уровней в отдельном Play до боя поставить Current Experience=450, затем победить: total470, level4. Параметры кривой менять в DefaultCharacterGrowth вне Play.

## Git

Подэтап сохраняется отдельным локальным feat: implement experience curve and level growth; hash/число файлов в итоговом отчёте/Git log. Ранее изменённый LiberationSans SDF - Fallback.asset остаётся в рабочей копии вне коммита; Temp/Library/Logs/секреты исключены. **Not pushed — waiting for user command.** Визуальный подэтап ранее сохранён a9a9117,124 файла.

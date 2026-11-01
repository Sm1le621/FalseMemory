# FalseMemory — этап4: инвентарь и предметы

> **Исторический документ.** Описывает состояние до упрощения 2026-10-31: восемь слотов, редкости и подбор предметов позже удалены. Актуальное состояние — [InventorySystem](Systems/InventorySystem.md) и [EquipmentSystem](Systems/EquipmentSystem.md).

2026-10-12. Исходная точка b06c760/main, рабочая копия чистая. План сохранён в MasterDevelopmentPlan.md, статусы в Roadmap.md. Unity MCP подтверждает SampleScene/Player/BattleInventory/Potion3, сцены/боевые компоненты прежние. Console в начале без ошибок/предупреждений.

## 4.1 — общий инвентарь30 слотов

Добавлены ItemData/InventoryState, границы/стопки, атомарные add/remove, перенос/обмен/слияние/деление, один Gold. Прежний BattleInventory — владелец State; items теперь начальный seed, не текущие количества. Переход/бой используют тот же компонент. BattleConsumable наследует ItemData, прежний Potion/его GUID и три стартовых предмета сохранены; создано6 дополнительных тестовых определений. Эффекты Ether/экипировка ещё не подключены. Временные значки используют уже созданные UI-ресурсы, графика владельца не менялась.

Техническая последовательность:4.2 окно/ввод →4.3 общий подборE →4.4 экипировка →4.5 ресурсы/эффекты/урон →4.6 границы/регрессия/разрешения → ручная приёмка. До её подтверждения этап5 не начинается.

Свежий baseline PlayMode:26 passed, job05229d680d854769bbd96254d092eba2. Первый запуск job7cc97506780e43faac64b165249e940e обнаружил0 тестов и не засчитан; указание assembly_names устранило пустой запуск.

После4.1: **47 EditMode passed**,0 failed/skipped, job1894e697d55144e7a9f9fbbdbeb47315 (40 прежних+7 новых). **26 PlayMode passed**,0 failed/skipped, job182578d2039c45659fd2ed7e2e26b601. Сценарии подтверждают прежние Potion/бой/ресурсы/возврат. Компиляция успешна. При domain reload наблюдались служебные предупреждения Unity Account timeout/MCP WebSocket; игровые ошибки не обнаружены. UI/курсор/подбор/экипировка не считаются проверенными на4.1.

## Границы

Нет сохранений на диск, торговли, классов/роста/квестов. Количество хранится только в текущей сессии. Нельзя менять начальный seed, рассчитывая изменить уже созданный State. Сборка вне Editor/художественная оценка не проверены. Push не выполняется.

Текущая точка приёмки: этап4 **Tested**, обязательная ручная проверка владельца ожидается. Этапы5–10 Not Started; до подтверждения этапа4 они не запускаются.

## 4.2 — окно и управление

Созданы/сохранены MCP InventoryCanvas,30 views, контроллер на Player и WorldInputGate. I/Escape, выбор/описание/количество/Gold, перенос/обмен/слияние/деление; использование пока disabled до4.5. Сохранение SampleScene сохранило все103 прежних YAML-ID, изменены только список компонентов Player, новый root и пустой startingItems. 28 PlayMode passed,0 failed/skipped, jobc0fa536ae3004cc0a8ca2ce3c1af7ad6. Синтетическая клавиатура и EventSystem handlers проверены; настоящий raycast/drag курсором владельца ещё не проверен. MCP открыл окно в реальном Play при1484×835, снимок Temp/InventoryStage4Verification/inventory42.png просмотрен; выходов/переполнений0. Play остановлен, Console0 ошибок/предупреждений. Компиляция успешна.

## 4.3 — подбор E

После прерывания проверены AGENTS/Docs/Git/Unity:4.1 и4.2 сохранены, незавершённый код4.3 продолжен без повторного создания. Текущая ветка stas_megamosk_build, посторонних исходников в diff нет. PlayerInteractor/IWorldInteractable/WorldItemPickup, prefab и семь instance TestLoot созданы/сохранены MCP; точки выбраны Physics2D без блокеров. Подсказка/сообщение, close/gate, проверки расстояния, резерв выдачи, полный отказ и однократность.29 PlayMode passed,0 failed/skipped, job667b8aea745b4fbf8e76430172f800de. Компиляция успешна, Console0 игровых ошибок/одно служебное предупреждение Account API timeout. Оружие/использование Ether ещё не подключены; этап5 не начат.

## 4.4 — семь слотов экипировки

EquipmentState/Controller, обмен с исходной ячейкой, безопасное снятие при полном запасе, уровень/слот/защита от рекурсивных событий. BattleUnit readonly Effective* и Speed читают базу+бонус заново, HPMP/база сохраняются; Manager будет подключён4.5. Окно/семь кнопок/Stats созданы MCP, SampleScene сохранена.58 EditMode passed, job0d8e2055ac8b43178e48dc0c23ae4daa;30 PlayMode passed, jobc66085536c1148b8912a7026e53ec02a,0 failed/skipped. Проверены семь слотов, полная замена/невозможное снятие, два аксессуара,20 чтений без накопления, wronglevel/индексы/событие и кнопки. Реальный MCP Play: получены семь сценовых pickup через публичный TryInteract и экипированы Sword/Armor/Helmet через кнопки; ATK20/DEF14, HP100/MP30. Первое служебное перемещение только Rigidbody.position не обновило transform для синхронной проверки диапазона; повтор с согласованными transform/body прошёл. Это проверка API/кнопок, не новое ручное нажатие E. Снимок inventory44-equipped.png просмотрен, границы/переполнение0; Play остановлен. Компиляция успешна.

## 4.5 — общие эффекты и BattleScene

WorldItemUseController на Player/предикат UI, общий CanUse/TryUse с контекстом, Ether20MP, ограничения категории/уровня/эффекта, optional VFX-профиль EtherVisual. Manager физически читает EffectiveAttack/Defense, Abilities магически EffectiveMagicAttack/Defense; формулы/цены сохранены.62 EditMode passed, job8ab7e3118e114341a88f5dbdc5918563;32 PlayMode passed, jobbe3fa6769c2049f1be5e964dd5f4a317,0 failed/skipped. Сценарий завершился97HP/30MP/Potion1/Ether0/EXP20, прежними позицией/моделью/экипировкой/мировым HUD. Подтверждены Attack15/ответ1 соSword+Armor+Helmet и Fire23 соStaff при8MP. Новый HPMP-гибрид не поддерживается; полный положительный ресурс допускает расход0 эффекта как раньше. Дискового сохранения нет, этап5 не начат. Компиляция успешна; служебный MCP WebSocket при reload наблюдался, игровых ошибок нет.

## 4.6 — стабилизация и финальная проверка

Шапка HPMP читает прежние ресурсы; собственный полноэкранный фон скрывает частично видимый мировой HUD при I, после Close восстанавливается. Мировые уведомления при gate очищаются. Временные значки/описания сохранены; Bone и Staff пока используют условные значки других команд, это не финальная художественная графика. Выключенный BoneStressTest2871 добавлен для понятной ручной проверки30 слотов, он не включён в обычной игре.

Финальные наборы:

- EditMode **62 passed,0 failed,0 skipped**, jobb954b340253449858962f2de3ecde0fe,0.5637009сек.
- PlayMode после сохранения фона **33 passed,0 failed,0 skipped**, job19dd58522b4d4af0a8f409c6583cc80e,28.3925492сек. Всего95 случаев: исходные66 плюс29 новых (22 EditMode/7 PlayMode).
- Новый Mouse-сценарий проходит реальные InputSystem MouseState и UI-raycast, не ручной ExecuteEvents: перенос0→9 и отмена за экраном. Остальные сценарии охватывают клавиши/движение/E/полноту/события/экипировку/ограничения/общий бой/возврат.
- MCP в обычном Play проверил четыре разрешения1280×720,1920×1080,2560×1440,3440×1440 в двух состояниях: частичный запас+три надетые вещи и полный запас+отказ снятия. Все8 сочетаний:0 выходов за экран/0 TMP-переполнений, открытое окно/102 или129 активных Graphic, HP80/MP25/ATK20/DEF14. Полный сценарий использовал настоящий pickup BoneStressTest:30 слотов, FirstEmpty=-1, меч остался надет, отказ без потери.
- [Машинные измерения](Verification/InventoryStage4Layout.json), [3440×1440](Images/InventoryStage4-3440.png), [полный запас1280×720](Images/InventoryStage4-Full.png). Все8 исходных PNG имеют точные native-размеры, проверены заголовки файлов; лежат в Temp/InventoryStage4Verification. Финальные изображения просмотрены. Ранний inventory44-equipped был старым кадром Game View и не подтверждал визуальную экипировку: её цифры тогда подтвердил MCP/тесты. Финальные кадры получены после временного Application.runInBackground=true, Canvas update/GameView repaint и проверки растущего frameCount. Настройка возвращена false.
- SampleScene/BattleScene validate:0 Missing Script/повреждённых Prefab. В SampleScene проверены618 компонентов/сериализованных ссылок: повреждений0. Уникальные/непустые7 ItemData ID и8 worldId (семь обычных pickup/один inactive stress). Все103 прежних YAML-ID SampleScene на месте. Основные ссылки BattleManager/UI/Presentation/VFX/Audio/HUD назначены.
- Команда восстановления Game View сначала использовала неправильный относительный индекс RemoveCustomSize и зарегистрировала два редакторских Invalid index. Уточнён актуальный API, применён абсолютный индекс, до9→после7; удалены только AI Inventory1280/3440, исходные builtin-размеры и выбор1 сохранены. Исходные пользовательские размеры не удалялись. Эта ошибка временной MCP-команды исправлена, не ошибка C# игры. Последующий отдельный Play-smoke подтвердил Open/Close/backdrop/input/Potion3 и false background; последнее чтение Console0 ошибок/предупреждений. Ручной очистки Console не выполнялось.
- При компиляциях наблюдались служебные Unity Account timeout/MCP WebSocket reconnect, они не объявляются исправленными. Проект компилируется; Play остановлен, активна SampleScene без dirty, временные размеры удалены.
- Проверены53 отдельных описания C#, разделы1–9, ссылки Docs и полнота meta. Генерируемые файлы/кеши не включаются, кроме сохранённых отчётных PNG в Docs. Не создавались новые сцены/копия Health/копия инвентаря. Архитектура/параметры других игровых механик сохраняются.

## Изменённые исходники

Новые Runtime: ItemData (определения/ограничения/бонусы), InventoryState (30 слотов/Gold), WorldInputGate (владельцы ввода), InventoryWindow/SlotView (окно/мышь), PlayerInteractor/WorldItemPickup (E/резерв/подбор), EquipmentState/Controller (семь слотов/обмен), WorldItemUseController (общий эффект мира/защита кадра). Новые тестовые файлы InventoryTests, EquipmentTests и InventoryFlowTests.

Изменены BattleInventory (единственный общий State), BattleConsumable (наследование ItemData/MP/VFX), BattleUnit (readonly Effective*), BattleAbilities (общие эффекты/ограничения), BattleManager (итоговые боевые характеристики/HPMP сообщения), BattleUI (fallback Stats), PlayerController (gate ввода), BattleItemTests (Ether/ограничения/магические бонусы). Health/Calculator/EnemyAI/старые Prefabs/графика/анимации не переписывались.

Unity: SampleScene добавлены собственные UI/компоненты/предметы, WorldItemPickup prefab; семь ItemData и EtherVisual. BattleScene сама не пересоздавалась и её сохранённый макет не менялся. Package/ProjectSettings/Input Actions не менялись.

## Документация и Git

Обновлены README/Architecture/Changelog/Testing/Guides, соответствующие отдельные документы каждого скрипта; InventorySystem переписан вокруг фактического общего запаса, добавлены EquipmentSystem/InteractionSystem. Полный исходный план сохранён в MasterDevelopmentPlan.md, Roadmap отражает Tested/ожидание ручной приёмки.

Завершённые коммиты:4.1 92c6ea1 feat: add shared thirty-slot inventory and item definitions;4.2 e5ce150 feat: add inventory window with slot drag and modal input;4.3 929ac60 feat: add world item interaction and atomic pickup;4.4 c775def feat: add transactional equipment slots and stat bonuses;4.5 30de0db feat: share world consumables and equipment with battle. Финальный4.6: test: stabilize and verify stage four inventory (hash после создания через git log). Текущая ветка stas_megamosk_build; исходная работа начиналась на main, затем текущая ветка поменялась внешним действием. Push агентом не выполнялся.

## Обязательная ручная приёмка

[TestingGuide](Guides/TestingGuide.md#этап4): пройти E/I/мышь/стопки/экипировку/Potion/Ether, бой/возврат, отдельный полный запас с inactive BoneStressTest, четыре разрешения. Проверить удобство, внешний вид, собственную мышь/фокус; автотесты не принимают художественные решения за владельца. Standalone-сборка/дисковые сохранения/реальные аудиоклипы не проверены. После подтверждения владельца следующий этап5 — прогресс/классы/способности; сейчас он не начат.

Последнее чтение после восстановления сгенерированного TMP-кеша перед коммитом:0 ошибок,1 предупреждение NativeFormatImporter generated inconsistent result для LiberationSans SDF - Fallback.asset. Ассет загружается, atlasTextures1/material назначены. Предупреждение не объявляется исправленным; историческое чтение0/0 выше относилось к завершённому Play-smoke до импорта. Тесты/финальные кадры прошли до восстановления кеша; ручная оценка русского текста включена в приёмку.

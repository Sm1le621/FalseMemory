# Учебный маршрут по документации FalseMemory

Обновлено 2026-10-31 после упрощения проекта: удалены локализация, EXP/уровни, редкость и подбор предметов. Этот файл — не описание механик, а порядок чтения остальных документов: с чего начать, что читать дальше и как связать текст с реальными скриптами и объектами сцен. Исходная версия составлена 2026-10-27 по коммиту `2516e98` (сценовая композиция).

Документация техническая и написана по-русски; игровой интерфейс, предметы, способности и сообщения — на английском. Имена классов, методов, полей и файлов остаются английскими.

## 1. Главное правило

Не читайте `Docs/Scripts/` подряд и не начинайте с отдельного скрипта. Правильный порядок — три уровня:

1. **Карта проекта** — что это за игра, какие три сцены, кто чем владеет.
2. **Системы** — как связаны объекты и скрипты внутри одной механики (бой, инвентарь, меню).
3. **Скрипты** — что конкретно делает один класс, какие у него поля и методы.

Если начать со скриптов, легко запутаться: `BattleUI`, `BattleHUD`, `BattlePresentation` и `BattleVFXController` выглядят похоже, но отвечают за разное. Сначала прочитайте систему — она объясняет разделение ролей.

Ещё три правила:

- **Документ описывает рабочую копию, а не «игру вообще».** Смотрите дату в начале/истории и последнюю запись `Changelog`.
- **Код по умолчанию ≠ значение в игре.** Многие поля назначены в сцене через Inspector и переопределяют значения из C#.
- **Исторические отчёты — это история, а не текущая инструкция.** Если в старом отчёте упоминается удалённый класс, значит, так было раньше.

## 2. Минимальный маршрут: 20–30 минут

Этого достаточно, чтобы понимать, где что лежит, и не потеряться в дальнейшем.

| № | Документ | Зачем читать |
|---|---|---|
| 1 | [README](README.md) | Главный указатель: все системы, скрипты, руководства и дата актуальности. |
| 2 | [Architecture](Architecture.md) | Структура папок, три сцены, владельцы данных, последовательность боя, границы проекта. |
| 3 | [SceneComposition](Systems/SceneComposition.md) | Дерево объектов `MainMenu`, `SampleScene`, `BattleScene` и все важные ссылки Inspector. |
| 4 | [UnitySetup](Guides/UnitySetup.md) | Что к чему подключено в Unity и какие параметры менять. |
| 5 | [Changelog](Changelog.md), первая запись | Что изменилось последним и какие ограничения остались. |

После этого вы должны уметь ответить:

- какие три сцены есть и какая за что отвечает;
- почему UI, текст и VFX теперь находятся в сценах, а не создаются кодом;
- кто запускает бой и кто возвращает игрока в мир;
- где искать ссылку на конкретный объект сцены.

## 3. Полный маршрут по этапам

### Этап 1. Карта и правила игры

1. [README](README.md)
2. [Architecture](Architecture.md)
3. [SceneComposition](Systems/SceneComposition.md)
4. [MasterDevelopmentPlan](MasterDevelopmentPlan.md) — только для общего замысла и места текущего этапа.
5. [Roadmap](Roadmap.md) — статусы этапов; не путать план с реализованным.

**Что понять:** три сцены, переходы, разделение «данные — логика — UI», текущие границы (нет сохранений на диск, нет NPC/диалогов, бой один на один).

### Этап 2. Игровой цикл исследования

1. [ExplorationSystem](Systems/ExplorationSystem.md) — движение и камера.
2. [InteractionSystem](Systems/InteractionSystem.md) — инвентарь и встреча с врагом.
3. [EnemyAISystem](Systems/EnemyAISystem.md) — преследование и обнаружение.
4. [CombatSystem](Systems/CombatSystem.md) — вход в бой и общая последовательность.
5. [SessionProgressSystem](Systems/SessionProgressSystem.md) — что сохраняется между боями в текущей сессии.
6. [MenuSystem](Systems/MenuSystem.md) — возврат в меню и новая игра.

Скрипты в этом порядке:

1. [PlayerController](Scripts/PlayerController.md) — движение, блокировка ввода.
2. [CameraFollow](Scripts/CameraFollow.md) — слежение камеры.
3. [WorldInputGate](Scripts/Inventory/WorldInputGate.md) — кто владеет вводом при открытом окне.
4. [InventoryWindow](Scripts/Inventory/InventoryWindow.md) — открытие окна и выбор предметов.
5. [EnemyAI](Scripts/EnemyAI.md) — преследование.
6. [BattleEncounter](Scripts/Battle/BattleEncounter.md) — физический контакт → попытка боя.
7. [BattleTransition](Scripts/Battle/BattleTransition.md) — затемнение, загрузка `BattleScene`, возврат.
8. [BattleScreenFade](Scripts/Battle/BattleScreenFade.md) — экран, переживающий Single-загрузку.
9. [Health](Scripts/Health.md) — HP и мировой HUD.
10. [ManaHUD](Scripts/UI/ManaHUD.md) — мировая полоса MP.

**Что понять:** мир не «знает» о боевых правилах; встреча лишь просит `BattleTransition` начать бой. После победы/поражения/побега именно переход решает, что загрузить и что вернуть.

### Этап 3. Бой — самый большой маршрут

Читайте строго в таком порядке: сначала правила, потом отображение.

1. [CombatSystem](Systems/CombatSystem.md) — общая схема.
2. [BattleManager](Scripts/Battle/BattleManager.md) — единственный владелец хода и применения действия.
3. [BattleTurnOrder](Scripts/Battle/BattleTurnOrder.md) — очередь по Speed.
4. [BattleCalculator](Scripts/Battle/BattleCalculator.md) — урон, промах, крит, защита.
5. [BattleRules](Scripts/Battle/BattleRules.md) — коэффициенты в ScriptableObject.
6. [BattleRandom](Scripts/Battle/BattleRandom.md) — случайность и тестовый контроль.
7. [BattleUnit](Scripts/Battle/BattleUnit.md) — характеристики, MP, Effective*-значения.
8. [BattleAbilities](Scripts/Battle/BattleAbilities.md) — общее применение магии/предметов.
9. [MagicSystem](Systems/MagicSystem.md) → [BattleSpell](Scripts/Battle/BattleSpell.md) — Fire.
10. [BattleConsumable](Scripts/Battle/BattleConsumable.md), [BattleInventory](Scripts/Battle/BattleInventory.md) — зелья и общий запас.
11. [EnemyBattleAI](Scripts/Battle/EnemyBattleAI.md) — выбор врага.
12. [BattleUISystem](Systems/BattleUISystem.md) → [BattleUI](Scripts/Battle/BattleUI.md) → [BattleHUD](Scripts/Battle/BattleHUD.md) — команды, подписи, полосы.
13. [BattleAnimationSystem](Systems/BattleAnimationSystem.md) → [BattlePresentation](Scripts/Battle/BattlePresentation.md) — позы и реакции портретов.
14. [BattleVFXSystem](Systems/BattleVFXSystem.md) → [BattleVFXProfile](Scripts/Battle/BattleVFXProfile.md) → [BattleVFXController](Scripts/Battle/BattleVFXController.md) — кадры и частицы.
15. [AudioSystem](Systems/AudioSystem.md) → [BattleAudioProfile](Scripts/Battle/BattleAudioProfile.md) → [BattleAudioController](Scripts/Battle/BattleAudioController.md) — звук (сейчас без клипов).
16. [HealthSystem](Systems/HealthSystem.md) — как HP связаны с HUD.

**Что понять:** `BattleManager` и правила — единственное место, где меняются HP/MP и предметы. `BattleUI`, `BattleHUD`, `BattlePresentation`, `BattleVFXController` и аудио только показывают результат и не должны влиять на расчёт. Анимация не задерживает ход дольше, чем это задано настройками представления. Опыта и уровней нет.

### Этап 4. Инвентарь, экипировка, предметы

1. [InventorySystem](Systems/InventorySystem.md) — окно и общий запас.
2. [EquipmentSystem](Systems/EquipmentSystem.md) — слоты Weapon/Chest и бонусы.
3. [ItemData](Scripts/Inventory/ItemData.md) — статическое определение предмета.
4. [InventoryState](Scripts/Inventory/InventoryState.md) — 30 слотов и Gold.
5. [InventoryWindow](Scripts/Inventory/InventoryWindow.md) — окно и команды.
6. [InventorySlotView](Scripts/Inventory/InventorySlotView.md) — одна ячейка.
7. [ItemPresentation](Scripts/Inventory/ItemPresentation.md) — цвета и текст предмета.
8. [InventoryTooltip](Scripts/Inventory/InventoryTooltip.md) — hover-описание.
9. [EquipmentState](Scripts/Inventory/EquipmentState.md) → [EquipmentController](Scripts/Inventory/EquipmentController.md) — атомарный обмен.
10. [EquipmentSlotView](Scripts/Inventory/EquipmentSlotView.md) — надетый предмет.
11. [WorldItemUseController](Scripts/Inventory/WorldItemUseController.md) — использование вне боя.
12. [BattleInventory](Scripts/Battle/BattleInventory.md) — тот же запас в бою.

**Что понять:** `InventoryState` — единственный запас; UI не копирует количество. `EquipmentController.State` хранит надетое; `Effective*` в `BattleUnit` считается заново из базы и вещей. Подбора предметов с земли нет: стартовые вещи уже лежат в инвентаре.

### Этап 5. Меню и настройки

1. [MenuSystem](Systems/MenuSystem.md) — сцена меню и поток.
2. [MainMenuController](Scripts/UI/MainMenuController.md) — New Game/Settings/Exit.
3. [SettingsMenu](Scripts/UI/SettingsMenu.md) — громкость и fullscreen.

**Что понять:** весь UI меню — объекты `MainMenu`; `SettingsMenu.Start` скрывает панель и применяет `PlayerPrefs`. Игровой текст — английские строки прямо в TMP-компонентах сцен; StringTable и `GameText` удалены.

### Этап 6. Визуал, анимации, VFX, графика

1. [EnemyVisuals](Systems/EnemyVisuals.md) — кадры скелета в мире и бою.
2. [BattleAnimationSystem](Systems/BattleAnimationSystem.md) — позы и темп.
3. [BattleVFXSystem](Systems/BattleVFXSystem.md) — эффекты магии.
4. [SpriteAnimationSet](Scripts/Battle/SpriteAnimationSet.md) — данные кадров.
5. [EnemySpriteAnimation](Scripts/Battle/EnemySpriteAnimation.md) — выбор кадра в мире.
6. [BattlePresentation](Scripts/Battle/BattlePresentation.md) — те же данные на боевом Image.
7. [BattleVFXProfile](Scripts/Battle/BattleVFXProfile.md) → [BattleVFXController](Scripts/Battle/BattleVFXController.md) — профиль и воспроизведение.
8. [PixelBattleAssetBuilder](Scripts/Editor/PixelBattleAssetBuilder.md) — Editor-инструмент, не игровая логика.
9. [RequiredSprites](Art/RequiredSprites.md) — чего ещё нет (игрок, частицы, иконки, рамки).

**Что понять:** Animator не используется; кадры задаются `SpriteAnimationSet`. VFX — 24 сохранённые частицы и один `PixelSpellFrame` в `BattleScene`; контроллер их только включает/двигает.

### Этап 7. Сессия и ресурсы

1. [SessionProgressSystem](Systems/SessionProgressSystem.md) — что живёт в текущей сессии.
2. [HealthSystem](Systems/HealthSystem.md) — HP и его отображение.
3. [BattleUnit](Scripts/Battle/BattleUnit.md) — MP и Effective*-значения.
4. [ManaHUD](Scripts/UI/ManaHUD.md) — мировая полоса маны.

**Что понять:** HP/MP/инвентарь/экипировка принадлежат исходным объектам мира и сохраняются при переходе в бой и обратно. Уровней, EXP и роста характеристик больше нет; дискового сохранения тоже нет.

### Этап 8. Проверки

1. [TestingGuide](Guides/TestingGuide.md) — как запускать Test Runner и что проверять руками.
2. [Testing](Testing.md) — актуальные и исторические результаты.
3. Документы тестов по нужной системе:
   - EditMode: [BattleCoreTests](Scripts/Tests/EditMode/BattleCoreTests.md), [BattleAbilityTests](Scripts/Tests/EditMode/BattleAbilityTests.md), [BattleItemTests](Scripts/Tests/EditMode/BattleItemTests.md), [InventoryTests](Scripts/Tests/EditMode/InventoryTests.md), [EquipmentTests](Scripts/Tests/EditMode/EquipmentTests.md), [PixelArtTests](Scripts/Tests/EditMode/PixelArtTests.md).
   - PlayMode: [BattleFlowTests](Scripts/Tests/PlayMode/BattleFlowTests.md), [BattleStage2FlowTests](Scripts/Tests/PlayMode/BattleStage2FlowTests.md), [BattlePresentationFlowTests](Scripts/Tests/PlayMode/BattlePresentationFlowTests.md), [InventoryFlowTests](Scripts/Tests/PlayMode/InventoryFlowTests.md), [InventoryPresentationFlowTests](Scripts/Tests/PlayMode/InventoryPresentationFlowTests.md), [PixelVisualFlowTests](Scripts/Tests/PlayMode/PixelVisualFlowTests.md).

**Что понять:** EditMode проверяет расчёты и данные, PlayMode — сцены, ввод, UI и переходы. Документ теста объясняет, какие сценарии реально покрыты, а какие остаются ручными.

### Этап 9. Git и правила работы

1. [GitWorkflow](Guides/GitWorkflow.md) — локальные коммиты и разрешённый push.
2. [AGENTS.md](../AGENTS.md) — постоянные правила проекта (документация, Git, язык, пиксель-арт).

**Что понять:** документация обновляется вместе с кодом; push — только по явной команде владельца; историю не переписывают.

## 4. Как читать документ скрипта

В `Docs/Scripts/` у каждого файла девять разделов. Читайте их в таком порядке:

1. **Название и назначение** — зачем класс существует и где лежит исходник.
2. **Как работает** — короткая последовательность действий без деталей C#.
3. **Методы и функции** — кто вызывает, что меняет, что возвращает. Здесь же объясняются `Awake`, `Start`, `Update`, `OnEnable` и другие Unity-события.
4. **Переменные и Inspector** — что видно в Unity, какие значения допустимы, что задано сценой.
5. **Связи** — какие компоненты и объекты нужны.
6. **Пример** — конкретный сценарий из игры.
7. **Настройка в Unity** — к какому объекту прикреплять и что назначить.
8. **Возможные проблемы** — типичные ошибки и ограничения.
9. **История изменений** — датированные записи. Это история, а не инструкция «сделай так сейчас».

Практический приём: откройте документ и `.cs` рядом. В документе найдите имя метода, в коде — его реализацию, затем вернитесь к разделу «Связи» и найдите этот объект в `SceneComposition` или в окне Hierarchy.

## 5. Как связать документ с объектом сцены

1. Возьмите имя компонента из документа (например, `BattleVFXController`).
2. Откройте [SceneComposition](Systems/SceneComposition.md) и найдите этот компонент в дереве — там указано, на каком объекте он находится.
3. Откройте сцену в Unity и выделите объект.
4. Сверьте поля Inspector с разделом «Переменные и Inspector»: часть значений приходит из сцены, часть — из кода.
5. Если поле пустое, ищите причину в разделе «Возможные проблемы», а не сразу в C#.

Для сцен с большим числом объектов полезны разделы `SceneComposition`:

- `MainMenu` — Canvas, кнопки, панель настроек, persistent-события;
- `SampleScene` — Player, Enemy, инвентарь, затемнение перехода;
- `BattleScene` — боевой Canvas, HUD, портреты, VFXLayer.

Важно: после рефакторинга 2026-10-25 почти весь UI, текст и эффекты — **сохранённые объекты**. Если в старом документе написано «код создаёт кнопку/панель/частицу», это устарело. Актуальное состояние — в `SceneComposition` и последних записях истории скриптов.

## 6. Как читать документы систем

У системных документов структура может отличаться: у новых есть нумерованные разделы, у более старых — просто последовательные абзацы и датированные дополнения. Читайте так:

1. Первый абзац — назначение и границы.
2. Основная часть — кто владеет данными, кто вызывает действия.
3. Ссылки внизу — на скрипты и отчёты.
4. Датированные записи — история; самая свежая запись важнее старой.

Главный вопрос при чтении системы: **«Кто здесь единственный владелец состояния?»** Например:

- бой — `BattleManager` и правила;
- запас предметов — `InventoryState`;
- надетые вещи — `EquipmentState`;
- английский текст — TMP-компоненты сцен;
- сцена и переход — `BattleTransition`;
- UI — только отображение.

Если два документа утверждают разное, смотрите дату, `Changelog` и реальный код.

## 7. Как читать документы тестов

В тестовом документе важны первые два раздела и список методов. Не нужно читать каждый `Assert` подряд.

- **EditMode** — быстрые проверки формул, данных, границ. Не загружают игру целиком.
- **PlayMode** — настоящие сцены, ввод, UI, переходы, загрузка `BattleScene`.

Порядок: [TestingGuide](Guides/TestingGuide.md) → [Testing](Testing.md) → документ нужного теста. Помните: успешный тест подтверждает только те сценарии, которые в нём перечислены. Внешний вид, удобство и баланс проверяет владелец вручную.

## 8. Быстрый справочник «нужен ответ — читайте»

| Вопрос | Документы |
|---|---|
| Что это за игра и какие сцены? | [README](README.md), [Architecture](Architecture.md) |
| Какой объект на какой сцене? | [SceneComposition](Systems/SceneComposition.md) |
| Как начинается и заканчивается бой? | [CombatSystem](Systems/CombatSystem.md), [BattleTransition](Scripts/Battle/BattleTransition.md) |
| Кто считает урон? | [BattleCalculator](Scripts/Battle/BattleCalculator.md), [BattleRules](Scripts/Battle/BattleRules.md) |
| Почему UI не может изменить HP? | [BattleManager](Scripts/Battle/BattleManager.md), [BattleUISystem](Systems/BattleUISystem.md) |
| Как работает Fire? | [MagicSystem](Systems/MagicSystem.md), [BattleSpell](Scripts/Battle/BattleSpell.md) |
| Почему эффект выглядит как квадраты? | [RequiredSprites](Art/RequiredSprites.md), [BattleVFXSystem](Systems/BattleVFXSystem.md) |
| Куда добавить предмет? | [ItemData](Scripts/Inventory/ItemData.md), [InventorySystem](Systems/InventorySystem.md) |
| Куда добавить текст? | Английские строки в TMP-компонентах сцены; [MenuSystem](Systems/MenuSystem.md), [SceneComposition](Systems/SceneComposition.md) |
| Как меню собирается без кода? | [MenuSystem](Systems/MenuSystem.md), [MainMenu](Systems/SceneComposition.md) |
| Где мана и почему нет уровня? | [ManaHUD](Scripts/UI/ManaHUD.md), [BattleUnit](Scripts/Battle/BattleUnit.md), [SessionProgressSystem](Systems/SessionProgressSystem.md) |
| Как запустить тесты? | [TestingGuide](Guides/TestingGuide.md) |
| Как делать коммит? | [GitWorkflow](Guides/GitWorkflow.md), [AGENTS.md](../AGENTS.md) |

## 9. Что считать устаревшим

Эти документы — исторические отчёты и снимки прошлых этапов. Они полезны, чтобы понять, почему решение принято, но не описывают текущий код целиком:

- [BattleStage1](BattleStage1.md), [BattleStage2](BattleStage2.md), [BattleStage3](BattleStage3.md);
- [InventoryStage4](InventoryStage4.md);
- [ProgressionStage5](ProgressionStage5.md);
- [EnglishLocalization](EnglishLocalization.md);
- [InventoryRedesign](InventoryRedesign.md);
- [PixelVisuals](PixelVisuals.md).

Признаки устаревшего текста:

- упоминания `MainMenuBootstrap`, `PixelUi`, `InventoryWindow.Layout`;
- вызовы `BuildLayout`, `BuildGuards`, `BuildCommandIcons`, `BuildFantasyLayout`, `BuildModalBackdrop`;
- утверждения «код создаёт Canvas/кнопку/частицу»;
- упоминания `GameText`, `LocalizedLabel`, `PlayerInteractor`, `WorldItemPickup`, `CharacterGrowthRules`, `LocalizationSystem`, `ProgressionSystem`, `StringTable`, редкости, EXP и вкладки Items как действующих механик;
- старые числа тестов без даты рядом.

Актуальную картину дают: `README` (дата актуальности), `Architecture`, `Systems/*`, `Scripts/*`, последняя запись `Changelog`, `Testing` (свежий блок) и `SceneComposition`.

## 10. Чек-лист понимания

Прежде чем менять код или сцену, проверьте, что можете ответить:

- [ ] Назвать три сцены и их роль.
- [ ] Объяснить путь «исследование → встреча → бой → возврат».
- [ ] Показать, где хранится HP и почему UI его не дублирует.
- [ ] Объяснить, кто владеет запасом предметов и надетой экипировкой.
- [ ] Найти в `SceneComposition` объект, на котором лежит нужный компонент.
- [ ] Отличить значение из C# от значения, назначенного в Inspector.
- [ ] Найти тест, который проверяет нужную механику.
- [ ] Понимать, что после правки обновляются Docs и создаётся локальный коммит.

## 11. Рекомендуемый план на три дня

| День | Что прочитать | Результат |
|---|---|---|
| 1 | README, Architecture, SceneComposition, UnitySetup, MenuSystem | Понимаете карту проекта и сценовое меню. |
| 2 | CombatSystem, BattleUISystem, BattleAnimationSystem, BattleVFXSystem и скрипты боя по порядку | Понимаете, кто считает бой и кто только показывает. |
| 3 | InventorySystem, EquipmentSystem, SessionProgressSystem, TestingGuide | Понимаете предметы, экипировку, ресурсы и проверки. |

Не пытайтесь прочитать все 90+ документов за один раз. Маршрут выше покрывает основы; остальные документы открывайте по мере задачи — через таблицу из раздела 8.

---

См. также: [README](README.md) — полный указатель, [Architecture](Architecture.md) — фактическая архитектура, [SceneComposition](Systems/SceneComposition.md) — объекты и связи сцен.

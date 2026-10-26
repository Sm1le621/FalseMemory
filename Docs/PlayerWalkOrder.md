# FalseMemory — исправление четырёхкадрового Walk, 2026-10-26

## Результат

Через восстановленный Unity MCP в существующей SampleScene подключены Walk_Down, Walk_Left, Walk_Right и Walk_Up с порядком **стойка → RightStep → LeftStep → стойка**. В каждом Walk ровно четыре Sprite-ключа; первый и четвёртый ссылаются на один нейтральный Idle_00, как разрешено последним заданием владельца. Поэтому это четыре позиции и три уникальных изображения на направление. Отдельные четыре Idle-рисунка каждого направления сохранены и подключены.

Работа продолжила незакоммиченную Cardinal4-подготовку предыдущего задания: [история](PlayerCardinal4.md). Генерация/интеграция не начинались с нуля. Прежние8/30-кадровые и диагональные ресурсы, PNG/ASE/клипы сохранены; сейчас оба существующих Blend Tree используют только четыре ракурса.

## Какие именно кадры используются

Общий корень изображений: `Assets/Art/Characters/Player/Cardinal4/`.

- **Down:** `Idle/Down/ApprovedHero_Idle_Down_00.png` → `WalkSteps/Down/ApprovedHero_Down_RightStep.png` → `WalkSteps/Down/ApprovedHero_Down_LeftStep.png` → тот же `Idle_Down_00`.
- **Left:** `Idle/Left/ApprovedHero_Idle_Left_00.png` → `WalkSteps/Left/ApprovedHero_Left_RightStep.png` → `WalkSteps/Left/ApprovedHero_Left_LeftStep.png` → тот же `Idle_Left_00`.
- **Right:** `Idle/Right/ApprovedHero_Idle_Right_00.png` → `WalkSteps/Right/ApprovedHero_Right_RightStep.png` → `WalkSteps/Right/ApprovedHero_Right_LeftStep.png` → тот же `Idle_Right_00`.
- **Up:** `Idle/Up/ApprovedHero_Idle_Up_00.png` → `WalkSteps/Up/ApprovedHero_Up_RightStep.png` → `WalkSteps/Up/ApprovedHero_Up_LeftStep.png` → тот же `Idle_Up_00`.

Idle каждого направления использует свои прежние `ApprovedHero_Idle_DIRECTION_00…03.png`. Итого в активных Idle/Walk24 уникальных изображения:16 Idle и8 шаговых; логических позиций32. Архивная папка `Walk` с прежними16 подготовленными PNG не удалена и больше не определяет порядок активных Walk. Точный список ссылок: [FrameAudit](PlayerWalkOrderFrameAudit.json), [импортный manifest](../Assets/Art/PlayerCardinal4Manifest.json).

## Источники и подготовка графики

Использован встроенный imagegen для редактирования предоставленного Idle-листа утверждённого тёмноволосого героя: тёмная броня, бордовый плащ, меч у пояса. Новый исходник [WalkSequenceSource.png](../Assets/Art/Characters/Player/Cardinal4/Source/WalkSequenceSource.png) сохранён без изменений, реальное разрешение1254×1254. [Полный запрос и замечания](PlayerWalkOrderPrompt.json). Указанный владельцем Idle-файл прочитан по корректному Windows-пути `C:/Users/...`, без ошибочного начального `/C:/`.

Порядок колонок генератора не принят автоматически: у Down/Left/Right шаговые позы выбраны из колонок3/2 (индексация для человека с1), у Up RightStep — колонка2. Для противоположного Up LeftStep использована колонка1 прежнего [WalkSheet.png](../Assets/Art/Characters/Player/Cardinal4/Source/WalkSheet.png), где смена ноги читается лучше. Это семь рисунков нового источника и одна повторно подготовленная прежняя поза, а не восемь полностью новых рисунков. Right/Left означают стороны героя; окончательная художественная оценка контакта ног остаётся ручной.

Подготовлены8 отдельных шаговых PNG512×512 с прозрачностью; реальная высота рисунка299…320 пикселей. Нарезка/прозрачное дополнение/регистрация целого рисунка, без resampling, искусственного увеличения и перемещения отдельных частей тела. Изолированные пиксели соседей вне основной фигуры убраны только в подготовленных кадрах, исходники сохранены. Горизонтальная регистрация головы согласована с прежним Idle_00, нижняя опорная стопа на линии492. Ground pivot(256,20), общий PPU Idle/Walk направления: Down602, Left567, Right569, Up576. Импорт Single Sprite/Full Rect/Point/no mipmaps/Uncompressed/NPOT None/alpha/Clamp. Масштаб Player2 и физическая скорость4 сохранены.

## Клипы и Animator

В существующей папке `Assets/Animations/Player64` обновлены8 клипов: Idle и Walk для Down/Left/Right/Up; их GUID сохранены. Idle-рисунки не перерисованы: четыре фазы удерживаются по .5с, цикл2с; пятый технический ключ повторяет первый на границе петли.

**Walk:** Samples16 — сетка времени, цикл .5с. Четыре ключа на0/.0625/.25/.4375с, конец .5с, Loop=true. Выдержки: стойка .0625с, правый шаг .1875с, левый шаг .1875с, стойка .0625с. Нейтральные участки на шве суммарно .125с, шаги остаются хорошо видны. Нет пятого замыкающего ключа и нет Transform-кривых. Это два цикла в секунду при WalkSpeed1, а не16 разных рисунков/с.

В том же `Assets/Player.controller` состояния Player_DirectionalIdle/Walk и параметры сохранены, у каждого Blend Tree теперь четыре child motion; timeScale детей и Animator.speed равны1. Существующий WalkSpeed по фактической скорости по-прежнему масштабирует Walk; Idle от него не зависит.

В SampleScene через MCP выполнены ImportAnimations/WireActivePlayer и проверен успешный SaveScene. Сценовый diff ограничен новым preview Idle_Down_00 и `useCardinalAnimations=1`. Новых GameObjects, сцен, Prefabs и игровых систем нет. PlayerController округляет только параметры визуального направления; нормализованное диагональное движение и LastDirection остаются настоящими, при равенстве осей визуальный приоритет Up/Down. После остановки сохраняется последний взгляд.

## Изменённые C# и документация

- `Assets/Editor/ApprovedHeroImporter.cs` — manifest.walkSequences с явным порядком4 ссылок; проверка двух различных шагов/повторной стойки, импорт40 подготовленных PNG, запись четырёх Walk-ключей без дополнительного замыкания, настройка прежнего controller/Player. [Описание](Scripts/Editor/ApprovedHeroImporter.md).
- `Assets/Scripts/PlayerController.cs` — только переключатель useCardinalAnimations и округление визуального взгляда. [Описание](Scripts/PlayerController.md).
- `Assets/Tests/EditMode/ApprovedHeroAssetTests.cs` — четыре Walk-ключа/три уникальных Sprite, время/импорт/24 активных изображения/controller/сохранность резервов. [Описание](Scripts/Tests/EditMode/ApprovedHeroAssetTests.md).
- `Assets/Tests/PlayMode/PlayerDirectionFlowTests.cs` — реальная диагональная скорость, cardinal-ракурсы, остановка/взгляд и смена Sprite. [Описание](Scripts/Tests/PlayMode/PlayerDirectionFlowTests.md).
- `Assets/Tests/PlayMode/PlayerIdle30FlowTests.cs` — историческое имя сохранено; теперь наблюдает порядок четырёх позиций Walk и все четыре Idle по каждому ракурсу. [Описание](Scripts/Tests/PlayMode/PlayerIdle30FlowTests.md).
- `Assets/Tests/PlayMode/MovementSmoothingFlowTests.cs` — ожидания темпа/числа Sprite приведены к короткому циклу; физические проверки сохранены. [Описание](Scripts/Tests/PlayMode/MovementSmoothingFlowTests.md).
- `Assets/Tests/PlayMode/BattleFlowTests.cs` — после победы точная проверка физической позиции и отдельный допуск1e-5 для интерполированного отображения. Игровой код перехода/боя не менялся. [Описание](Scripts/Tests/PlayMode/BattleFlowTests.md).

Обновлены README, Roadmap, Architecture, Changelog, AnimationSystem, UnitySetup, TestingGuide, PixelArtStyleGuide и отдельные описания всех7 C#; сохранены прежние датированные записи. Источники/подготовленные ресурсы имеют Unity .meta. Новых игровых строк нет, правила English UI/русские Docs сохранены.

## Реально выполненные проверки

Unity6000.6.2f1, соединение MCP127.0.0.1:8080 восстановлено. Проверены текущая SampleScene/Player/Animator/SpriteRenderer/ссылки/Console. Первоначальная ошибка CS1503 в незавершённом тесте ApprovedHeroAssetTests устранена; успешные компиляция и выполнение тестов подтверждены ниже.

- **Edit Mode98/98:** финальный job `0742757ee17840afa2adfb116481573b`. Ключи/времена/.5с/хеши/PPU/pivot/Point/no compression/controller проверены через настоящие Unity assets.
- **Play Mode58/58:** финальный job `ee75ab6cd0004acab1d4e878bf150692`. Реальный Animator показал последовательность нейтральная/RightStep/LeftStep/нейтральная для всех4 направлений; все4 Idle наблюдались при неподвижном физическом корне. Также проверены реальные движение/диагонали/торможение/стена/gate/преследование, вход в бой через физический контакт, урон/Defend/победа/поражение/возврат/меню и остальные существующие сценарии сборки.
- Первичный Play-прогон `e22e660538a54ba5bf24082f5eb7c251`:57/58, один старый тест сравнивал интерполированный Transform побитно после победы. Для него добавлены точное сравнение Rigidbody2D.position и отдельный малый предел отображения; полный повтор прошёл. Никакие runtime-позиции ради теста не изменялись.
- Обычный Play Mode/нативный Game View1920×1080: синтетические S/A/D/W, запись фактических Sprite и несколько ScreenCapture; просмотрены кадры Down/Left/Right/Up. При первом Up герой упёрся в дерево, анимация остановилась; повтор в свободной runtime-области подтвердил обе шаговые фазы всех4 ракурсов. Скриншоты находятся в игнорируемой `Temp/WalkOrderVerification`; точное имя фазы берётся из журнала Animator, поскольку ScreenCapture сохраняется в конце render-кадра.
- Просмотр показал целый силуэт/плащ, чёткие края и сопоставимый размер. Pivot общий, анимации не двигают Transform. Естественность коротких художественных фаз и мелкая согласованность деталей не объявляются доказанными только тестами/скриншотами.
- Финальное чтение Console: **0 ошибок,0 предупреждений**. Ранее было служебное `Account API did not become accessible within30 seconds`; его появление не скрывается. Финально Play=false, compiling=false, SampleScene dirty=false, missing scripts Player0. Game View возвращён на исходный preset0, runtime/project background=false; настройки ввода восстановлены.

[Машинная запись проверок/журнал обычного Play](PlayerWalkOrderVerification.json). Standalone-сборка и широкий экран в этом задании не проверялись. Файлы боевых runtime-скриптов, BattleScene и Battle settings не изменены.

## Git и оставшаяся ручная граница

Ветка `stas_megamosk_build`, исходный HEAD `564c77e`. Изменения предыдущей Cardinal4-подготовки сохранены и включаются вместе с её завершённой интеграцией/исправлением порядка. Индекс перед работой был пуст. Чужой изменённый `LiberationSans SDF - Fallback.asset` сохранён, исключён из локального коммита. Temp/Library/Logs и временные MCP-запросы не включаются. Сообщение локального коммита: `fix: integrate four-frame cardinal walk cycles`. Push не выполняется.

Для ручной художественной оценки:

1. Открой SampleScene → Play; в свободном месте подержи S/A/D/W по2 секунды. Оцени оба шага и нейтральную позу на шве, особенно ноги под плащом в Up.
2. Остановись после каждого направления минимум на2 секунды. Idle должен спокойно дышать, сохранять взгляд и не шагать; оцени отсутствие скачков головы/масштаба/деталей брони.
3. Проверь диагональ, быстрые повороты и упор в дерево: скорость остаётся обычной, у препятствия ходьба прекращается. Открой/закрой Inventory и продолжи движение.
4. Коснись скелета, выполни Attack и вернись на карту после победы: оставшиеся HP и управление должны сохраниться.

Следующий шаг — оценка темпа/читаемости этих четырёх позиций владельцем. До неё дополнительные направления, длинные циклы и дальнейшее расширение общей RPG не запускаются.

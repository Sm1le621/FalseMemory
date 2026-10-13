# PlayerDirectionFlowTests

## 1. Название и назначение

Исходник `Assets/Tests/PlayMode/PlayerDirectionFlowTests.cs`. Дополнительная часть существующего BattleFlowTests: проверяет directional-ввод/Animator на реальном Player в SampleScene.

## 2. Как работает

Общий UnitySetUp BattleFlowTests загружает SampleScene и временно настраивает Input System. Тест создаёт Keyboard, последовательно посылает восемь сочетаний клавиш и отпускание; после проверяет блокировку ввода открытым Inventory. Общий TearDown возвращает настройки и выгружает тестовые сцены.

## 3. Методы и функции

ApprovedWalkChangesFramesAndStoppingKeepsDirectionalIdle сначала ждёт реальное состояние Walk, затем собирает ровно8 Sprite в течение.85с и проверяет, что состояние не мерцает в Idle. При отдельном новом Idle прежний счётчик ошибочно включал стартовый idle-кадр в число walk-поз; количество8/все префиксы остаются строгими.

`EightDirectionsNormalizeInputSelectMotionAndRetainLastFacing()` — UnityTest/IEnumerator: W,S,D,A,WD,WA,SD,SA; проверяет норму1, MoveX/Y, состояние Walk и отсутствие flipX. Отпускание проверяет нулевой ввод, сохранённый LastMoveX/Y и Idle. Проверяет кадр ApprovedHero выбранного направления; Down теперь отдельный анимированный Idle. Открытый Inventory блокирует новый ввод без изменения предыдущего направления. Finally удаляет только синтетическую клавиатуру и закрывает окно; игровой контент не удаляется. Unity Awake/Update у самого теста отсутствуют; общие callbacks описаны в [BattleFlowTests](BattleFlowTests.md).

`ApprovedWalkChangesFramesAndStoppingKeepsDirectionalIdle()` — второй UnityTest: синтетическая S на .85 секунды, реальные SpriteRenderer-кадры должны показать все восемь Down-поз; отпускание оставляет отдельный Idle_Down. Проверяется сохранение MovementSpeed=4 на стартовых характеристиках. Finally удаляет тестовую клавиатуру. Это проверка исполнения, не художественной естественности.

ApprovedWalkChangesFramesAndStoppingKeepsDirectionalIdle теперь должен увидеть ровно8 разных Down Sprite за.85 секунды; имена прежнего ApprovedHero направления сохранены. 12FPS/⅔s цикл проверяется поведением, отпускание оставляет правильный directional Idle, MovementSpeed остаётся4.

## 4. Переменные и Inspector

Локальные player/control/animator/keyboard, восемь массивов Key и нормализованных Vector2. Значения не настраиваются в Inspector. Погрешность сравнения float0.0001. Начало/остановка ждут.12 секунды, чтобы пройти короткий разгон/торможение независимо от render FPS.

## 5. Связи

PlayerController, Animator/Player.controller, SpriteRenderer, InventoryWindow/WorldInputGate, Input System и Unity Test Framework. Использует Player() и общую изоляцию fixture BattleFlowTests.

## 6. Пример

W+A устанавливает MoveX≈−0.707/MoveY≈0.707, выбирается Player_DirectionalWalk. Отпускание выбирает Player_DirectionalIdle и сохраняет последний вектор. Открытие I не позволяет другим клавишам изменить этот вектор.

## 7. Настройка в Unity

К GameObject не прикрепляется. Window → General → Test Runner → PlayMode → FalseMemory.Battle.PlayModeTests → BattleFlowTests → этот метод. Нужна текущая SampleScene с directional Animator и существующим UI.

## 8. Возможные проблемы

Несохранённый/старый Animator даёт несоответствие параметров/состояний. Синтетический Keyboard возвращается в finally. Тест не доказывает качество walk-графики, восемь уникальных ракурсов, скорость всех возможных столкновений и standalone-сборку. Существующие сценарии отдельно проверяют контакт и возврат из боя.

Два сценария сохраняют прежние assertions нормализации/ракурсов/gate/8 кадров/MovementSpeed4; обновлены только ожидания времени начала и остановки. Новые измерения реальной физики находятся в MovementSmoothingFlowTests.

После остановки Down проверяет префикс ApprovedHero_Idle_Down_, поскольку кадр дыхания меняется; для остальных семи направлений сохраняется точный старый static Sprite. Walk по-прежнему должен реально показать8 Down-кадров, скорость4/нормализация/gate сохранены. Все30 Idle и неподвижность физического тела проверяет PlayerIdle30FlowTests.

## 9. История изменений

- 2026-10-12 — добавлена автоматическая проверка восьми направлений и последнего взгляда; статический fallback явно отделён от полноценной анимации.

- 2026-10-13 — проверяется настоящая directional-графика ApprovedHero; добавлена проверка смены восьми кадров и неизменной скорости.

- 2026-10-13 — актуальная проверка восьми walk-кадров вместо четырёх.

- 2026-10-13 — актуализированы темп/физическое воспроизведение и проверки в задаче Animation Rework.

- 2026-10-13 — проверка нового30-frame Idle_Down без ослабления прежних направлений/движения.

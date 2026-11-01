# BattleManager

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleManager.cs`.

Оркестратор пошагового боя один на один: очередь ходов, приём команды игрока, ход врага, применение атаки/магии/защиты/побега, показ сообщений и завершение боя. Компонент не хранит HP/MP и не создаёт копии участников: он управляет теми же объектами мира, которые передал `BattleTransition`.

## 2. Как работает

`Start` — корутина. Если активной встречи нет (сцена запущена напрямую), менеджер показывает ознакомительное меню «Explore Map / Main Menu» и не начинает бой. При активной встрече он получает `Player`, `Enemy` и `rules` от `BattleTransition`, передаёт участников в `BattleUI`/`BattlePresentation`, скрывает HUD на время входа, ждёт окончания перехода и проявляет экран.

Каждый раунд начинается с `turns.BeginRound`, затем участники ходят по убыванию `Speed`. В начале хода вызывается `BeginTurn`, который снимает защиту. В ход игрока открывается `BattleCommandGate`, показывается меню `Attack / Magic / Defend / Flee`, и корутина ждёт `pendingAction`. В ход врага `EnemyBattleAI` выбирает только `Attack` или `Defend`. Перед применением действия менеджер закрывает ввод, при необходимости проигрывает `Windup`, повторно проверяет жизнь участников и только затем тратит ресурсы/наносит урон. Победа, поражение и побег завершают цикл; опыта, уровней и наград больше нет.

## 3. Методы и функции

- `Start()` — Unity coroutine; запускает цикл боя. Без `BattleTransition.EncounterActive` показывает меню предпросмотра. В цикле обновляет `Round`, очередь, UI и состояние `BattleState`.
- `SetRandomSource(IBattleRandom source)` — заменяет источник случайности (нужен тестам); `null` вызывает `ArgumentNullException`.
- `Pause(bool enemyThinking)` — coroutine; делегирует паузу `BattlePresentation.ReadPause`, иначе ждёт `rules.actionDelay` в реальном времени.
- `TryPlayerAction(BattleAction action)` → `bool`; принимает только `Attack`, `Defend`, `Flee` при `CanChoose` и открытом gate. Записывает `pendingAction`, переводит состояние в `Resolving`, блокирует кнопки. `Magic` этим методом не принимается.
- `CanChoose` — private bool: сейчас ход игрока, оба живы, состояние `AwaitingPlayer`, gate открыт.
- `ShowMainMenu()` → `bool`; при `CanChoose` показывает четыре команды: Attack, Magic, Defend, Flee. Пункта Items больше нет.
- `OpenMagicMenu()` → `bool`; строит список заклинаний игрока с ценой MP и доступностью через `BattleAbilities.CanCast`, добавляет `Back` и обработчик Escape.
- `TryPlayerSpell(BattleSpell spell)` → `bool`; проверяет, что заклинание принадлежит игроку, доступно и gate открыт; записывает `pendingSpell`, действие `Magic` и блокирует UI.
- `BattleState`/`BattleAction` определены в `BattleTurnOrder.cs`. `CurrentActor` и `Round` доступны только для чтения.

## 4. Переменные и Inspector

- `ui` — обязательная ссылка на `BattleUI` в `BattleScene`.
- `presentation`, `vfx`, `audioController` — необязательные визуальные/звуковые компоненты; при отсутствии бой продолжается без них.
- `State`, `Player`, `Enemy`, `CurrentActor`, `Round` — публичные свойства только для чтения, отражают текущее состояние.
- `rules`, `transition`, `gate`, `turns`, `pendingAction`, `pendingSpell`, `random` — внутреннее состояние; в Inspector не настраиваются.
- Собственных числовых Inspector-параметров у менеджера нет: баланс задаётся в `BattleRules`, `BattleUnit` и ассетах заклинаний.

## 5. Связи

Требуются `BattleUI` и активный `BattleTransition` в сцене мира. `BattleUI` принимает ввод, `BattleHUD`/`BattlePresentation`/`BattleVFXController` показывают бой, `BattleAudioController` проигрывает звуки. `BattleCalculator` считает урон и шансы, `BattleAbilities` проверяет и применяет магию, `BattleTurnOrder` строит очередь, `EnemyBattleAI` выбирает действие врага. HP принадлежит `Health`, MP — `BattleUnit`, предметы — `BattleInventory`.

## 6. Пример

Игрок выбирает `Magic → Fire (8 MP)`. `TryPlayerSpell` принимает команду и закрывает меню. Корутина проигрывает замах, проверяет, что враг ещё жив, списывает 8 MP и наносит магический урон. Затем враг думает и атакует или защищается. При нулевом HP скелета показывается победа и кнопка `Return to Map`.

## 7. Настройка в Unity

Компонент находится на объекте `BattleManager` в `BattleScene`. Назначь `ui`; остальные ссылки необязательны. `Player`, `Enemy` и `rules` приходят от `BattleTransition` из `SampleScene`. Запускать бой нужно касанием скелета на карте, а не прямым Play сцены `BattleScene`.

## 8. Возможные проблемы

Прямой запуск `BattleScene` не создаёт встречу и показывает только меню предпросмотра. Без `BattleUI` будут ошибки обращения к ссылкам. Двойной клик/Enter защищён `BattleCommandGate`. Если `BattleTransition` отсутствует или встреча не активна, бой не начнётся. Смерть участника во время визуального замаха отменяет действие и не тратит MP. Поддерживается только один противник; выбора цели и партии нет.

## 9. История изменений

- 2026-10-31 — документ приведён к упрощённой версии: четыре команды без Items, магия только с уроном, без EXP/уровней/наград; победа просто завершает бой.
- 2026-10-12 — английский игровой текст и строковые ключи; запись историческая, `GameText` удалён.
- 2026-10-11 — визуальные паузы, окно результата, звук и VFX; основы боя сохранены.

# FalseMemory — этап 3: полировка боя

> **Исторический документ.** Описывает состояние до упрощения 2026-10-31: EXP/уровни, локализация, редкость, подбор предметов, Heal и вкладка Items позже удалены. Актуальные механики — в [README](README.md), [Architecture](Architecture.md) и `Docs/Systems/`.

Контрольная точка перед работой: `07a1a34`, main, рабочая копия чистая. Этапы 1–2 изучены; Console в начале без ошибок/предупреждений. Подходящего боевого аудио/кадров атак нет: используются временные эффекты UI/Transform и настраиваемые звуковые ссылки без скачивания ресурсов.

## 3.1 — интерфейс

BattleHUD и прежний BattleUI: компактные панели с рамками, противник/раунд/сообщение сверху, герои в центре, HP/MP/ресурсы/команды снизу. Добавлены неинтерактивные плавные полосы и LOW HP (25%). Существующие объекты перенастроены через MCP и сохранены в BattleScene.

Play Mode: **18 passed, 0 failed, 0 skipped**, job `cf6a338d26664f24886300874e450ef1`: прежние сценарии и независимость HUD от ресурсов. В обычном Play через MCP запущен бой, получен и просмотрен Game View 1280×720 (`Temp/BattleStage3Verification/stage31.png`): панели/тексты/полосы внутри экрана, портреты без растяжения.

Промежуточная ошибка BattleHUD не найден исправлена полным импортом нового файла. После импорта компиляция успешна. Обнаружены существующие предупреждения FindFirstObjectByType в тестах и служебные Unity Account/MCP; они будут проверены при завершении.

## 3.2 — анимации действий

BattlePresentation добавлен в существующую BattleScene через MCP: выпад/возврат, вспышка и отдача при попадании, усиление крита, символ защиты. Числа поднимаются и исчезают; MISS не запускает успешный удар. Manager после windup повторно проверяет жизнь и применяет действие один раз; отсутствие контроллера не блокирует бой.

Play Mode: **20 passed, 0 failed, 0 skipped**, job `9a849b7135f54eb1b9c9cfa025a36817`, включая смерть во время подготовки Fire и отсутствие presentation. Последнее чтение Console — 0 ошибок/предупреждений. Компиляция успешна.

## 3.3 — магические эффекты

Новые BattleVFXProfile/Controller и FireVisual/HealVisual подключены через MCP в существующей сцене/заклинаниях. Fire — оранжевые поднимающиеся частицы; Heal/Potion — зелёные ромбы. Магия включает краткую подсветку кастующего. Игровые цены/эффекты сохранены, новые заклинания не добавлены.

Play Mode: **21 passed, 0 failed, 0 skipped**, job `3a92af0a30824e3ca55b77b0cea48017`, включая завершение эффекта и работу Heal без VFX. Компиляция успешна.

## 3.4 — звук

AudioProfile/Controller, один 2D AudioSource и BattleMixer/Master созданы/назначены через MCP. Подключены события UI/действий/исходов и существующая общая громкость. Клипы намеренно пустые: подходящего аудио нет, звучание не заявляется готовым.

После исправления поиска скрытого Enemy в новом тесте Play Mode: **22 passed, 0 failed, 0 skipped**, job `2e2b3b63dfb2468ba372864ff046c310`. Первый запуск `edbb63b85f8a49588a0e326ea05c05ff` имел 21 passed/1 failed из-за тестового GameObject.Find скрытого объекта; игровой код для исправления этого сбоя не менялся. Компиляция успешна.

## 3.5 — переходы и исходы

Плавное появление UI/результатов, исчезновение погибшего, Game Over и короткий уход Flee. Через MCP сохранены CanvasGroups и fadeDuration=0.5. Restart/Main Menu сохраняют только временный экран затемнения до проявления новой сцены; игровые данные новой сессии сбрасываются как прежде.

Play Mode: **23 passed, 0 failed, 0 skipped**, job `580be2ea147f4c769f4860ce33dd3e4a`. Подтверждены повторные столкновения, вход, поражение/окно, прежние победа/побег/Restart/Main Menu и ресурсы. Последнее чтение Console — 0 ошибок/предупреждений.

## 3.6 — пиксельное оформление и разрешения

Через MCP настроены Point-атлас существующего PressStart2P для команд (кириллица через fallback), пять собственных значков 16×16, центрирование портретов и окна исхода, обводка чисел. Исправлены смещение героя при отрицательном масштабе и переполнение сообщения Victory. Исходные спрайты, их импорт и мировая камера не менялись; для Overlay Canvas дополнительная Pixel Perfect Camera не нужна.

В реальном Play Mode проверены 1280×720, 1920×1080, 2560×1440 и 3440×1440: главное меню боя, Magic, Items, Victory и Game Over. Для всех 20 сочетаний MCP проверил экранные границы Graphic и TMP-переполнение: 0 выходов и 0 переполнений. [Измерения](Verification/BattleStage3Layout.json), [снимок 3440×1440](Images/BattleStage3-3440.png). Остальные снимки в Temp/BattleStage3Verification; некоторые ранние preview уменьшены, файлы *native.png сохраняют размер Game View. Ранний асинхронный снимок Game Over оказался пустым до готовности кадра; повторные снимки получены после проверки Defeat, активного результата и alpha=1, просмотрены. Снимок не засчитывался доказательством до повторной проверки.

После исправлений **23 Play Mode passed, 0 failed, 0 skipped**, job `c8eb4d1af3a642afab8ccb21d202bd7f`. Компиляция успешна; последнее чтение Console — 0 ошибок/предупреждений. Play остановлен, SampleScene активна, прежний выбор Game View восстановлен, временный размер удалён.

## 3.7 — темп и финальная проверка

Настроены attackDuration=0.4, hitDuration=0.18, outcomeDuration=0.35, messageDuration=0.75, enemyThinkDuration=0.5 секунды при штатном actionDelay=0.8. Space ускоряет движения/эти паузы ×3 и не выбирает команду; отключается allowFastForward. Time.timeScale, формулы, цены, очередь и единственное применение эффекта сохранены. Самостоятельные эффекты/полосы/числа/затемнения заканчиваются в своём темпе. Подсказка добавлена в Controls через MCP.

Финальные запуски после сохранения сцены:

- **Edit Mode: 40 passed, 0 failed, 0 skipped**, job `cdfa06a926944fcb93825e84f4a98dbd`, 0.3579563 секунды.
- **Play Mode: 26 passed, 0 failed, 0 skipped**, job `30820ca766fa40bc96cde9b1ddc780ae`, 24.2434138 секунды. Это 17 прежних и 9 новых сценариев, всего 66 тестов вместе с Edit Mode.
- Новые сценарии этапа3: независимость полос/LOW HP; отмена при смерти в windup без MP; отсутствие presentation; завершение/отсутствие VFX; пустые аудиоссылки/громкость; вход/повторные встречи/Game Over; Space и однократность; MISS без попадания; назначенный временный тихий клип через AudioSource/Mixer. Старые сценарии проверяют Attack/Defend, Fire/Heal/Potion, crit/miss, Enter, Flee, однократный EXP, возврат/ресурсы, Restart/MainMenu.
- MCP validate SampleScene/BattleScene: 0 Missing Script, 0 повреждённых Prefab. В BattleScene дополнительно проверены 167 компонентов и сериализованные ссылки: повреждённых ссылок нет, контроллеры назначены, иконок5, Pixel Perfect=true, сохранённые длительности правильные.
- Компиляция проекта завершена; устаревшие FindFirstObjectByType в двух местах теста заменены FindAnyObjectByType. Временный MCP-скан сначала использовал устаревшее objectReferenceInstanceIDValue и не скомпилировался; повторный скан через актуальное objectReferenceEntityIdValue успешно выполнен. Игровые файлы этим сканом не менялись.
- После финальных тестов Console: **0 ошибок, 1 предупреждение Account API did not become accessible within 30 seconds** из Unity AI Assistant. При доменной перезагрузке также наблюдалось служебное предупреждение MCP WebSocket is not initialised; подключение восстановилось и все проверки завершились. Эти сетевые предупреждения не объявляются исправленными.
- 40 проектных C# → 40 отдельных документов; у каждого разделы1–9. Относительные Markdown-ссылки проверены: отсутствующих целей нет.

## Изменённые исходники и данные

Новые компоненты: [BattleHUD](Scripts/Battle/BattleHUD.md) — оформление/полосы; [BattlePresentation](Scripts/Battle/BattlePresentation.md) — позы/анимации/темп; [BattleVFXController](Scripts/Battle/BattleVFXController.md) и [BattleVFXProfile](Scripts/Battle/BattleVFXProfile.md) — временные эффекты/их данные; [BattleAudioController](Scripts/Battle/BattleAudioController.md) и [BattleAudioProfile](Scripts/Battle/BattleAudioProfile.md) — звуковые события/клипы; [BattleScreenFade](Scripts/Battle/BattleScreenFade.md) — временное затемнение при Single-переходе.

Изменены [BattleUI](Scripts/Battle/BattleUI.md) — визуал/звуки/результаты; [BattleManager](Scripts/Battle/BattleManager.md) — последовательность представления и прежнее единственное действие; [BattleSpell](Scripts/Battle/BattleSpell.md) — необязательная ссылка VFX; [BattleTransition](Scripts/Battle/BattleTransition.md) — гладкие переходы. [BattleFlowTests](Scripts/Tests/PlayMode/BattleFlowTests.md) использует актуальный поиск Unity, новый partial [BattlePresentationFlowTests](Scripts/Tests/PlayMode/BattlePresentationFlowTests.md) покрывает полировку. Health, Calculator, Abilities, Inventory, Queue, RNG, EnemyAI и PlayerController не менялись.

BattleScene перенастроена и сохранена через MCP: прежние объекты UI, новые панели/полосы/значки/символы защиты/CanvasGroups, компоненты представления, один 2D AudioSource. Новых сцен не создавалось. Assets/Settings/Battle содержит новые VFX, Audio/Mixer и UI/Font/Icons; в существующих Fire/Heal назначены визуальные профили, в DefaultBattleRules fadeDuration=0.5. Новые игровые предметы/заклинания не добавлялись.

## Документация и Git

Обновлены README, Architecture, Changelog, Testing, Guides/UnitySetup и TestingGuide, этот отчёт; описания всех затронутых скриптов. Добавлены системы BattleUISystem, BattleAnimationSystem, BattleVFXSystem и AudioSystem. Снимок и машинные измерения сохранены в Docs/Images и Docs/Verification.

Ветка main. Завершённые подэтапы сохраняются отдельными непустыми локальными коммитами: 3.1 `894785e feat: improve battle ui`, 3.2 `e096d0c feat: add battle animations`, 3.3 `12e54b0 feat: implement battle visual effects`, 3.4 `86dd3f9 feat: add battle audio system`, 3.5 `0a00b72 feat: improve battle transitions`, 3.6 `5723aef fix: polish pixel battle ui across resolutions`; финальный 3.7 — `feat: polish battle pacing and verify presentation` (hash читается после создания через git log). Push не выполняется.

## Ручная оценка переходов

Начни игру из SampleScene/MainMenu, коснись скелета, кликни Game View. Проверь клавиатуру/мышь, читаемость, доступность Fire/Potion и LOW HP. Выполни Attack/Defend/Fire/Heal/Potion: посмотри выпад, реакцию, числа, защиту и частицы. Удержи и отпусти Space: темп меняется, действие самостоятельно не выбирается. На четырёх разрешениях посмотри меню и исходы. Победи/вернись и проверь HP/MP/предметы/EXP/позицию; отдельный запуск с Health=1 в Play — Game Over, Restart/Main Menu. Подробности в [TestingGuide](Guides/TestingGuide.md). Автотесты проверяют числа/сценарии, а не художественное качество/ощущения.

## Ограничения

Исходная графика, формулы и источники ресурсов сохранены. Анимации используют временные Transform/Image-эффекты, художественных кадров атак и подходящих звуков пока нет; AudioClip пустые, реальное звучание не проверено. Разрешения/Editor-кадры не подтверждают полноэкранный режим или поведение отдельной сборки. Нет дисковых сохранений, партии и новых игровых заклинаний. Сборка вне Editor не проверена. Push не выполняется.

Следующие задачи: подобрать собственные звуки/кадры атак, оценить темп владельцем, затем проверить standalone-сборку; отдельно развивать цели/нескольких участников, статусы, экипировку и сохранение RPG. Новый этап не начинается без задания владельца.

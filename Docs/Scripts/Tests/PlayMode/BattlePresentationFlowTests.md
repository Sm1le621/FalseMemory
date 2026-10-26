# BattlePresentationFlowTests

## Текущее изменение5.3 —2026-10-26

Сохранены HUD/VFX/паузы/отмена/однократность. Ожидания CurrentMP увеличены на10 вслед за новой базой40; нормализованная MP-полоса проверяется32/40 после расхода8. Расходы/урон/очередь не ослаблялись.

Фактические проверки и ограничения: [ClasslessStage53](../../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/BattlePresentationFlowTests.cs. Часть BattleFlowTests: проверяет представление поверх настоящего боя.

## 2. Как работает

Использует Setup/Cleanup и BeginBattle из других частей класса, загружает существующие сцены, проверяет HUD без изменения исходных ассетов.

## 3. Методы и функции

2026-10-12 — актуальная локализация: Текстовые ожидания приведены к английскому UI (New Game/Main Menu/Back/No items available/Inventory full/DEFEAT/описания HPMP/цен). Проверки состояния, количества, урона, ресурсов и поведения сохранены.

- `SpaceSpeedsPresentationWithoutChoosingOrDuplicatingActions()` — синтетическое удержание Space проверяет VisualDelta×3, отключение ускорения, отсутствие самостоятельной команды, единственный Attack/ответ и неизменные MP/EXP/предметы/Time.timeScale/позицию портрета. UnityTest IEnumerator, устройство удаляется finally.
- `MissShowsTextWithoutHitReactionOrDamage()` — гарантированный промах показывает MISS, не включает glow/реакцию и не уменьшает HP врага; только успешный ответ врага увеличивает счётчик реакции. UnityTest IEnumerator.
- `AssignedClipUsesMixerAndRespectsExistingVolume()` — временный профиль и тихий тестовый AudioClip проверяют PlayOneShot-путь, Mixer и громкость; данные уничтожаются finally, исходный профиль не меняется. Не подтверждает слышимость художественных клипов. UnityTest IEnumerator.
- `PresentationEntranceAndOutcomesKeepTransitionLockedAndShowResults()` — пять повторных TryBegin отклоняются, после появления alpha=1; смерть показывает Game Over/нулевую alpha героя и блокирует Attack. UnityTest IEnumerator.

- `MissingAudioClipsAreSafeAndListenerVolumeIsRespected()` — все 15 событий с пустыми клипами возвращают false; AudioListener.volume сохраняется, source 2D и ignoreListenerVolume=false; Attack/ответ дают прежние HP. UnityTest IEnumerator, не подтверждает слышимость будущего контента.

- `MagicVFXFinishesAndMissingVFXNeverBlocksResourceRules()` — Fire запускает ровно один эффект/частицы, затем они исчезают; после обнуления vfx Heal по-прежнему расходует 6 MP и сохраняет прежний цикл/HP. UnityTest IEnumerator.

- `TargetDeathDuringWindupCancelsDamageAndManaSpend()` — ждёт IsAnimating, убивает цель во время подготовки Fire, проверяет 30 MP, единственные 20 EXP и завершение анимации.
- `MissingPresentationDoesNotBlockOrDuplicatePhysicalAction()` — обнуляет необязательную ссылку, дважды отправляет Attack и проверяет один урон/один ход. Оба UnityTest IEnumerator.

`HUDAnimatesReadOnlyBarsWithoutChangingResources()` — UnityTest IEnumerator: после начала боя уменьшает HP/MP, ждёт плавности, проверяет числа, значения трёх Slider, LOW HP и возврат меню без расхода/смены хода. При несоответствии Assert завершает тест ошибкой. Общие методы описаны в BattleFlowTests/BattleStage2FlowTests.

## 4. Переменные

Локальные player/enemy/manager/hud — объекты тестовой сессии. Ожидаются 20 HP и 22 MP; ожидание 0.35 секунды больше barDuration=0.22. Inspector отсутствует.

## 5. Связи

FalseMemory.Battle.PlayModeTests, NUnit, Unity Test Framework, BattleManager/UI/HUD и настоящий Health.

## 6. Пример

Run All → ресурсы 100/30 становятся 20/22 → полосы 0.2 и 22/30 → открытие/закрытие Magic оставляет тот же ход.

## 7. Настройка в Unity

К GameObject не прикреплять. Запуск через Test Runner PlayMode или Unity MCP run_tests; дождаться итогового get_test_job.

## 8. Ограничения

Численные/сценарные проверки не заменяют оценку красоты интерфейса и реальное прослушивание звуков. Изменения HP делаются в тестовой сессии, сцена не сохраняется.

## 9. История изменений

2026-10-12 — английский игровой текст, централизованные ключи/параметры; русская техническая документация сохранена.

- 2026-10-11 — 3.7: ускорение/однократность, визуальный MISS и путь назначенного клипа.

- 2026-10-11 — 3.5: проверка входа, исхода и блокировки перехода.

- 2026-10-11 — 3.4: проверка пустых аудио-ссылок/совместимости громкости.

- 2026-10-11 — 3.3: проверка завершения/отсутствия VFX.

- 2026-10-11 — 3.2: проверки отсутствующего визуала и смерти в windup.

- 2026-10-11 — подэтап 3.1: проверка анимации полос и независимости отображения от игровых ресурсов.

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

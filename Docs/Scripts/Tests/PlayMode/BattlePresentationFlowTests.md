# BattlePresentationFlowTests

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Tests/PlayMode/BattlePresentationFlowTests.cs. Часть BattleFlowTests: проверяет представление поверх настоящего боя.

## 2. Как работает

Использует Setup/Cleanup и BeginBattle из других частей класса, загружает существующие сцены, проверяет HUD без изменения исходных ассетов.

## 3. Методы и функции

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

- 2026-10-11 — подэтап 3.1: проверка анимации полос и независимости отображения от игровых ресурсов.

# BattleScreenFade

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleScreenFade.cs. Плавный чёрный экран, в том числе через Restart/Main Menu.

## 2. Как работает

Обычный Transition использует `Image` затемнения, сохранённый в `SampleScene`. При Single-загрузке только временный Canvas затемнения отделяется от мира, переживает загрузку и удаляется после проявления новой сцены. Player/Enemy не переносятся в DontDestroyOnLoad.

## 3. Методы и функции

`Fade(image,alpha,duration)` → IEnumerator: clamp alpha 0..1, duration 0..2, SmoothStep по unscaledDeltaTime, финальное точное значение; null безопасен. `LoadAndReveal(scene,duration)` — отделяет overlay, DontDestroyOnLoad, запускает Load. `Load(scene,duration)` → IEnumerator: Single LoadSceneAsync, после завершения Fade к 0 и Destroy созданного overlay; исключение загрузки регистрируется, экран всё равно очищается.

## 4. Переменные и Inspector

panel — существующий Image чёрного overlay; отдельного игрового состояния/Singleton нет. Длительность приходит из Rules.fadeDuration (ассет 0.5).

## 5. Связи

BattleTransition.FadeTo делегирует `Fade`; LoadFresh передаёт панель уже существующему `BattleScreenFade` на `BattleFadeCanvas`, который отделяется от мира на время Single-загрузки. Это не второй координатор боя.

## 6. Пример

Game Over → Restart → экран темнеет → новая SampleScene → экран проявляется → временный Canvas исчезает.

## 7. Настройка в Unity

Вручную не добавлять на Player/Enemy. Компонент уже стоит на `SampleScene/BattleTransition/BattleFadeCanvas`; поле `panel` указывает на `Fade`. Transition только передаёт панель и вызывает `LoadAndReveal`; при Single компонент оказывается на корневом Canvas, не на объекте состояния.

## 8. Ошибки, ограничения и расширение

Сцена должна быть в Build Scene List, её доступность заранее проверяет Transition. Пустой Image пропускается. Внешняя произвольная смена сцены не является системой сохранений. Расширять только анимацию экрана, не хранение прогресса.

## 9. История изменений

- 2026-10-11 — 3.5 — общая SmoothStep-функция и проявление после Single-переходов.

- 2026-10-25 — `BattleScreenFade` и затемняющий Canvas сохранены в `SampleScene`; AddComponent в рантайме удалён.

2026-10-25 — уточнено, что `Fade` и `BattleScreenFade` сохранены в `SampleScene`; рантайм-создание Canvas/компонента удалено.

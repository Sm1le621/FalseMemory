# Звук боя

## 1. Назначение

Звуковое сопровождение боя через `BattleAudioProfile` → `BattleAudioController` → `AudioSource` (2D, PlayOneShot) → `BattleMixer/Master` → `AudioListener.volume`. Настройки меню остаются единственным регулятором громкости.

## 2. Общая логика

`BattleUI` отправляет MenuOpen/Move/Confirm/Cancel, `BattleManager` — Attack/Hit/Critical/Miss/Fire/Defend/Flee/Victory/Defeat. Если клип не назначен, `Play` возвращает `false` и бой продолжается без звука. Профиль по умолчанию `DefaultBattleAudio` содержит пустые поля, поэтому текущая сборка беззвучна.

## 3. Основные компоненты

- `BattleAudioProfile` — набор клипов и `AudioMixerGroup`.
- `BattleAudioController` — один `AudioSource`, `Play`, `LastEvent`.
- `BattleUI`/`BattleManager` — источники событий.
- `SettingsMenu` — общая громкость.

## 4. Взаимодействие скриптов

`BattleManager.audioController` необязателен. `BattleUI.audioController` вызывает звуки меню. Контроллер не влияет на расчёт боя и не создаёт второй `AudioListener`. `SettingsMenu` пишет `AudioListener.volume`, который учитывается, если `ignoreListenerVolume = false`.

## 5. Последовательность событий

Открытие меню → MenuOpen; смена строки → Move; подтверждение → Confirm; действие → Attack/Fire/Defend/Flee; попадание → Hit/Critical; промах → Miss; исход → Victory/Defeat.

## 6. Настраиваемые параметры

`Assets/Settings/Battle/Audio/DefaultBattleAudio.asset`: поля клипов и `volume` 0…1. Пустые поля допустимы. `BattleRules.actionDelay` не влияет на звук. Регулятор громкости — `SettingsMenu`.

## 7. Использование в Unity

Назначь `BattleAudioController.profile`, `BattleManager.audioController`. Для подключения звуков открой профиль и перетащи `AudioClip` в нужное поле. Mixer: `Window → Audio → Audio Mixer`; не создавай второй Master.

## 8. Расширение

Новое событие — поле в `BattleAudioProfile`, значение `BattleSound` и вызов из контроллера/менеджера. Не вызывай расчёт урона повторно из звука. Для музыки потребуется отдельный источник/контроллер.

## 9. Ограничения и проверки

Сейчас нет назначенных клипов; реальное прослушивание и баланс — ручная задача. PlayMode проверяет тишину без ошибок, путь `PlayOneShot`, Mixer/Listener и сохранение ресурсов. `Heal`/`Potion` больше не являются боевыми событиями.

## История

- 2026-10-31 — удалены боевые события Heal/Potion; документ приведён к текущим командам.
- 2026-10-12 — базовая звуковая система.

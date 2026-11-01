# BattleAudioController

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleAudioController.cs`.

Компактный проигрыватель боевых звуков через один `AudioSource`. Не участвует в расчёте боя, очереди ходов и изменении ресурсов.

## 2. Как работает

`Awake` получает `AudioSource`, отключает автозапуск и зацикливание, ставит 2D-режим и разрешает учитывать общую громкость слушателя. `Play(sound)` записывает событие для диагностики, находит клип в профиле и вызывает `PlayOneShot`. Если компонент выключен, профиль или клип отсутствуют, метод возвращает `false`, а бой продолжается без звука.

## 3. Методы и функции

- `Awake()` — Unity callback; настраивает `AudioSource`.
- `Play(BattleSound sound)` → `bool`; обновляет `LastEvent`/`EventsRequested`, назначает `outputAudioMixerGroup`, проигрывает клип с громкостью профиля. `false` означает «звук не прозвучал», а не ошибку боя.
- `LastEvent` — последнее запрошенное событие; `EventsRequested` — счётчик запросов.

## 4. Переменные и Inspector

- `profile` — `BattleAudioProfile`; без него все вызовы возвращают `false`.
- `source` — внутренний `AudioSource`, добавляется через `RequireComponent`.
- `LastEvent`, `EventsRequested` — публичное состояние только для чтения; не подтверждают слышимость при пустых клипах.

## 5. Связи

`BattleUI` вызывает `MenuOpen`/`Move`/`Confirm`/`Cancel`, `BattleManager` — `Attack`/`Hit`/`Critical`/`Miss`/`Fire`/`Defend`/`Flee`/`Victory`/`Defeat`. Громкость применяется через `AudioListener`, которым управляет `SettingsMenu`. Второй `AudioListener` не создаётся.

## 6. Пример

Подтверждение команды → `Confirm`; успешная атака → `Attack`, затем `Hit` или `Critical`; промах → `Miss`; победа → `Victory`. С пустым профилем вся последовательность остаётся беззвучной, но бой идёт обычным образом.

## 7. Настройка в Unity

Компонент находится на `BattleManager` в `BattleScene` рядом с `AudioSource`. Назначь `profile`; `BattleManager.audioController` и `BattleUI.audioController` уже ссылаются на него. Не добавляй второй `AudioSource`/`AudioListener` для тех же событий.

## 8. Возможные проблемы

Без `profile` или клипов звука нет — это ожидаемо. `PlayOneShot` не прерывает предыдущий клип. Контроллер не меняет `AudioListener.volume` и не хранит настройки. Фоновой музыки и ducking нет. Реальное прослушивание и баланс проверяются вручную владельцем.

## 9. История изменений

- 2026-10-31 — удалены вызовы и события `Heal`/`Potion`; документ сверен с текущими командами боя.
- 2026-10-11 — безопасные события, AudioSource/Mixer и совместимость громкости.

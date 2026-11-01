# BattleAudioProfile

## 1. Название и назначение

Исходник: `Assets/Scripts/Battle/BattleAudioProfile.cs`.

ScriptableObject с клипами боевых событий, общей громкостью и группой Mixer. Сам по себе ничего не проигрывает: данные читает `BattleAudioController`. Лечебных и предметных боевых событий больше нет.

## 2. Как работает

Ассет хранит по одному `AudioClip` на событие; любое поле можно оставить пустым. `GetClip(sound)` выбирает клип через `switch`. Список событий задаётся перечислением `BattleSound`: `MenuOpen`, `Move`, `Confirm`, `Cancel`, `Attack`, `Hit`, `Critical`, `Miss`, `Fire`, `Victory`, `Defeat`, `Defend`, `Flee`.

## 3. Методы и функции

- `GetClip(BattleSound sound)` → `AudioClip` или `null`; возвращает назначенный клип события. Неизвестное значение даёт `null`.
- Unity создаёт ассет через меню `FalseMemory → Battle Audio Profile`.

## 4. Переменные и Inspector

- `output` — необязательная `AudioMixerGroup`; если не назначена, играет Master.
- `volume` — относительная громкость 0…1, по умолчанию 0.8.
- `menuOpen`, `move`, `confirm`, `cancel` — звуки интерфейса.
- `attack`, `hit`, `critical`, `miss`, `fire` — боевые действия.
- `victory`, `defeat`, `defend`, `flee` — исходы и защита.
- В `Assets/Settings/Battle/Audio/DefaultBattleAudio.asset` все клипы намеренно пустые, поэтому игра сейчас беззвучна.

## 5. Связи

`BattleAudioController.profile` читает ассет. `BattleUI` передаёт события меню, `BattleManager` — боевые действия и исходы. `SettingsMenu` остаётся единственным владельцем общей `AudioListener.volume`.

## 6. Пример

Если в поле `fire` назначить `fire.wav`, при успешном заклинании Fire прозвучит этот клип с громкостью профиля. Пустое поле означает тишину без ошибки.

## 7. Настройка в Unity

Открой `Assets/Settings/Battle/Audio/DefaultBattleAudio.asset` или создай новый ассет через `Assets → Create → FalseMemory → Battle Audio Profile`. Перетащи `AudioClip` в нужные поля, задай `volume` и при необходимости `output`. Компонент к GameObject не прикрепляется.

## 8. Возможные проблемы

Пустая ссылка — не ошибка, а тишина. Не создавай второй регулятор громкости в Mixer или `PlayerPrefs`: общая громкость принадлежит `SettingsMenu`. Для нового события нужно расширить `BattleSound`, `GetClip` и место вызова. Музыка в профиль не входит.

## 9. История изменений

- 2026-10-31 — удалены неиспользуемые события `Heal` и `Potion`; осталось 13 боевых/UI-событий.
- 2026-10-11 — профиль событий без загрузки сторонних звуков.

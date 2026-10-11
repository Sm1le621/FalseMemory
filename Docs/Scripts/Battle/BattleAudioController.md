# BattleAudioController

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleAudioController.cs. Компактный проигрыватель боевых звуков.

## 2. Как работает

Один AudioSource воспроизводит PlayOneShot; отсутствующий профиль/клип/выключенный компонент просто возвращает false. Не участвует в ожидании хода.

## 3. Методы и функции

`Awake()` — назначает AudioSource, выключает playOnAwake/loop, spatialBlend=0, ignoreListenerVolume=false. `Play(sound)` → bool: записывает LastEvent/EventsRequested для диагностики, находит клип, назначает output и воспроизводит с clamp(profile.volume); при отсутствии ссылки возвращает false.

## 4. Переменные и Inspector

profile — BattleAudioProfile; source — внутренний AudioSource. LastEvent/EventsRequested только читаются, не подтверждают слышимый звук при пустых клипах.

## 5. Связи

BattleUI вызывает MenuOpen/Move/Confirm/Cancel; Manager вызывает Attack/Hit/Critical/Miss/Fire/Heal/Victory/Defeat и дополнительные Defend/Potion/Flee. Общая громкость SettingsMenu применяется AudioListener; контроллер её не меняет.

## 6. Пример

Confirm → начало Attack → Hit; после победы Victory. С текущими пустыми ссылками последовательность остаётся беззвучной.

## 7. Настройка в Unity

Уже на BattleManager, AudioSource добавлен RequireComponent/MCP, profile и ссылки UI/Manager сохранены в BattleScene. Mixer Master назначен. Не создавать второй AudioListener.

## 8. Ошибки, ограничения и расширение

Подходящие файлы отсутствуют, слышимость/баланс пока не проверяются. Нет фоновой музыки/ducking. Для будущего контента достаточно заполнить профиль; бой работает без клипов/профиля.

## 9. История изменений

- 2026-10-11 — 3.4 — безопасные события, AudioSource/Mixer и совместимость громкости.

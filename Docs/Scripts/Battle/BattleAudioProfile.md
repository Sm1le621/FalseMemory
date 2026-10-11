# BattleAudioProfile

## 1. Название и назначение

Исходник: F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Battle/BattleAudioProfile.cs. Назначение собственных клипов боевым событиям.

## 2. Как работает

Один ScriptableObject хранит по клипу на событие, громкость и группу Mixer. Клип можно оставить пустым.

## 3. Методы и функции

`GetClip(sound)` → AudioClip либо null: switch для MenuOpen/Move/Confirm/Cancel/Attack/Hit/Critical/Miss/Fire/Heal/Victory/Defeat/Defend/Potion/Flee. Unity создаёт ассет через Create Asset Menu.

## 4. Переменные и Inspector

output — AudioMixerGroup Master нового BattleMixer; volume=0.8 (0..1); 15 AudioClip-ссылок по именам событий. В текущем DefaultBattleAudio все клипы пустые намеренно.

## 5. Связи

BattleAudioController читает профиль; UI и Manager передают события. SettingsMenu остаётся владельцем общей AudioListener.volume.

## 6. Пример

Назначить собственный fire.wav в поле fire → при Fire слышен клип с общей громкостью игры.

## 7. Настройка в Unity

Create → FalseMemory → Battle Audio Profile либо открыть Assets/Settings/Battle/Audio/DefaultBattleAudio.asset. Импортировать собственные аудиофайлы обычным способом, назначить поля. К GameObject не прикрепляется.

## 8. Ошибки, ограничения и расширение

Пустая ссылка означает тишину, не ошибку. Не дублировать общий регулятор громкости в Mixer/Prefs. Для новых событий расширить enum/GetClip и вызовы; будущая музыка отдельно.

## 9. История изменений

- 2026-10-11 — 3.4 — профиль событий без загрузки сторонних звуков.

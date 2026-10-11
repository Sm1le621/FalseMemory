# Health

## 1. Название и назначение

**Исходный файл:** F:/LLM/UUU/Miri/FalseMemory/Assets/Scripts/Health.cs.

Единственный владелец HP, урон, лечение и обновление HUD.

## 2. Как работает

Start заполняет HP. TakeDamage/Heal вызывают UpdateUI. Сам Health не запускает Game Over: это делает BattleManager.

## 3. Методы и функции

- `Start()` — первый запуск перед обновлениями: currentHealth=maxHealth, UpdateUI. Повторное включение не повторяет Start.
- `TakeDamage(int damage)` — вычитает damage, ограничивает HP 0…maxHealth, UpdateUI; void.
- `Heal(int amount)` — отвергает amount≤0/мёртвого; ограничивает лечение максимумом, UpdateUI; возвращает фактически восстановленные HP.
- `UpdateUI()` — задаёт Slider.maxValue/value и TMP-текст при назначенных ссылках; void.

## 4. Переменные и параметры Inspector

maxHealth — положительный максимум, код 100; сцена Player 100, Enemy 60. currentHealth — состояние 0…maxHealth, Start его перезаписывает. hpSlider/hpText — необязательные ссылки Slider/TMP_Text.

## 5. Связи с другими системами

BattleUnit.Health возвращает тот же компонент. BattleAbilities/BattleManager/EnemyAttack используют его; BattleUI читает HP отсюда.

## 6. Пример работы

95/100 HP → Heal(25) → 100 HP, результат 5 и обновлённый HUD.

## 7. Возможные проблемы и проверки

Прямая запись currentHealth не обновляет UI. Отрицательный TakeDamage лечит — вызывающий код обязан передавать ≥0. Heal не воскрешает. Health связан с UI напрямую. Лимиты/общая ссылка покрыты тестами; Slider игрока не интерактивен.

## 8. История изменений

- 2026-10-11 — впервые документирован прочитанный текущий код рабочей копии. В задаче документации исходный C#-файл не изменялся.

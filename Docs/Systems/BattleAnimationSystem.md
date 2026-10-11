# Анимации боя

BattlePresentation отображает исходные портреты через RectTransform/Image: Attack — выпад и возврат, попадание — вспышка/реакция, крит — усиленная реакция. Defend показывает голубой ромб до следующего собственного хода. MISS не вызывает удара/отдачи. BattleUI поднимает число и плавно скрывает его за feedbackDuration; крит отличается цветом и размером.

Manager закрывает gate до Windup, повторно проверяет жизнь обоих участников после ожидания, применяет расчёт один раз и вызывает Recover. Анимации не содержат изменений HP/MP/предметов/EXP. Без компонента/Animator действие продолжает работать. Настройки attackDuration/hitDuration/lungeDistance/reactionDistance назначены в BattleScene; pace связан с существующим actionDelay, чтобы ускоренные тесты не ждали штатные задержки.

Расширение: заменить временные движения кадрами Aseprite в контроллере, оставив игровое применение в Manager. [BattlePresentation](../Scripts/Battle/BattlePresentation.md), [BattleUI](../Scripts/Battle/BattleUI.md). Проверки смерти во время windup и отсутствующего визуала добавлены в PlayMode; художественное качество оценивается вручную.

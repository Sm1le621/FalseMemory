# Магические эффекты боя

BattleVFXController использует до 24 простых Image в повторно используемом пуле. FireVisual поднимает оранжевые частицы, HealVisual вращает зелёные ромбы; Potion использует лечебный вариант. Play запускается после успешного применения ресурса в Manager, в тот же кадр, и не пишет HP/MP/предметы.

BattleSpell.visualEffect ссылается на BattleVFXProfile. Создать новый профиль через Create → FalseMemory → Battle VFX Profile: цвет, форма Rising/Orbit/Burst, duration, размер/радиус/количество и необязательный Sprite. Можно подготовить оформление Ice/Thunder/Holy/Dark, но сами новые заклинания сейчас не реализованы. Отсутствующие профили имеют fallback, отключение/выгрузка очищает пул. Эффекты не блокируют ход и не требуют Animator/частиц 3D/новых пакетов.

Детали: [профиль](../Scripts/Battle/BattleVFXProfile.md), [контроллер](../Scripts/Battle/BattleVFXController.md). Проверка Play Mode подтверждает один запуск, завершение и сохранение правил при отсутствии VFX; внешний вид оценивать в Game View.

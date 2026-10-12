# Магические эффекты боя

2026-10-12 — FireVisual/HealVisual имеют по8 native PNG64×64, duration0.6 и frameSize128; Controller переиспользует один PixelSpellFrame поверх прежних частиц. Кадр следует портрету и очищается при завершении/disable/unload; пустые массивы сохраняют fallback. Magic/Items не включают старый сплошной glow, физическое попадание использует64px Hit/Slash-маску BattleUI. [Проверки и исходники](../PixelVisuals.md).

BattleVFXController использует до 24 простых Image в повторно используемом пуле. FireVisual показывает кадры пламени и оранжевые частицы, HealVisual — кадры кольца/искр и зелёные ромбы; Potion использует лечебный вариант. Play запускается после успешного применения ресурса в Manager, в тот же кадр, и не пишет HP/MP/предметы.

BattleSpell.visualEffect ссылается на BattleVFXProfile. Создать новый профиль через Create → FalseMemory → Battle VFX Profile: цвет, форма Rising/Orbit/Burst, duration, размер/радиус/количество и необязательный Sprite. Можно подготовить оформление Ice/Thunder/Holy/Dark, но сами новые заклинания сейчас не реализованы. Отсутствующие профили имеют fallback, отключение/выгрузка очищает пул. Эффекты не блокируют ход и не требуют Animator/частиц 3D/новых пакетов.

Детали: [профиль](../Scripts/Battle/BattleVFXProfile.md), [контроллер](../Scripts/Battle/BattleVFXController.md). Проверка Play Mode подтверждает один запуск, завершение и сохранение правил при отсутствии VFX; внешний вид оценивать в Game View.

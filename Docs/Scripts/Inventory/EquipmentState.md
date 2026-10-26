# EquipmentState

## Изменение5.4 —2026-10-26

ScaledBonuses(values,rules) начинает с raw Bonuses, один раз прибавляет armor.Bonuses для валидного явно включённого Armor Scaling. Повторный пересчёт не меняет raw bonuses/слоты. Primary attributes берутся из raw Bonuses, производные armor-прибавки считаются отдельно. Снятие вещи убирает её вклад; после утраты требования новый scaling-вклад отключён, вещь не удаляется. Исторические результаты ниже не являются результатами текущего запуска; см. [ClasslessStage54](../../ClasslessStage54.md).

## Текущее изменение5.3 —2026-10-26

BindRequirements(Func<ItemData,CharacterAttributeValues,bool>) назначает transient проверку владельца; отсутствие callback сохраняет автономную legacy-совместимость модели. AttributesExcluding(excluded,emptied?,replacement?) суммирует primary всех остальных предполагаемых слотов. RequirementsAllow проверяет кандидата без его бонусов/вытесняемой вещи. CanEquip, CanMove (оба участника) и TryUnequipTo проверяют требования до обмена; changing/атомарность/откат сохранены. Новый callback не Inspector и не static.

Фактические проверки и ограничения: [ClasslessStage53](../../ClasslessStage53.md). Старые датированные значения ниже сохраняют историю соответствующих режимов.

## 1. Название и назначение

Assets/Scripts/Inventory/EquipmentState.cs. Serializable владелец девяти надетых ItemData отдельно от исходного InventoryState. Статические определения не хранят количество.

## 2. Как работает

Надевание меняет одну bag вещь на надетую. Возврат идёт в свободное место либо указанную ячейку; compatible singleton допускает обмен. Перенос между gear слотами перемещает/обменивает ссылки. Все условия проверяются до изменения; changing исключает повторный вход из уведомлений. Отказ сохраняет исходное владение.

## 3. Методы и функции

GetSlot(slot) возвращает ItemData/null, неверный enum — исключение; EnsureSize сохраняет старые0…7 и расширяет массив до9. Bonuses каждый раз суммирует определения, не записывает в базовые характеристики.
Valid(slot) проверяет0…8. HandsAllow(target,incoming,emptied=null) проверяет будущую пару Main Weapon/OffHand: twoHanded Weapon несовместим с любым OffHand. Не снимает вещи автоматически.
CanEquip(inventory,index,slot,level) проверяет guard/индекс/Count1/Fits/level/hands. TryEquip повторно проверяет, задаёт gear перед InventoryState.TryExchangeSingle и откатывает при false; после успеха Changed.
TryUnequip(inventory,slot) проверяет CanAdd, снимает и TryAdd целиком; отказ откатывается. GetSlot обеспечивает legacy size.
CanMove(from,to,level) проверяет обоих участников обмена/Fits/уровень и будущую пару рук. Тот же nonempty slot разрешён как no-op. TryMove применяет подтверждённый swap, Changed один раз, no-op без Changed.
TryUnequipTo(inventory,from,destination,level) проверяет конкретный bag slot: пустой либо compatible equipment Count1/level/hands. Меняет gear/точный bag singleton, откатывает false; Changed после успеха. Full incompatible bag не теряет надетое.
Все Try/Can возвращают bool. Changing снимается finally. Unity callbacks отсутствуют.

## 4. Переменные и Inspector

SlotCount=9 const, private SerializeField slots ItemData[9], initial empty; changing private transient, Changed Action. Weapon0/Head1/Chest2/Legs3/Boots4/Accessory1=5/Accessory2=6/Gloves7/OffHand8. Legacy8 расширяется без переиндексации; реальных дисковых сохранений старой игры нет.

## 5. Связи

EquipmentController.State владеет моделью; ItemData.Fits/twoHanded/requiredLevel, InventoryState.TryExchangeSingle/CanAdd/TryAdd. BattleUnit читает Bonuses и подписан на Changed; UI только отправляет операции.

## 6. Пример

Sword перетаскивается из Weapon в bag12: один экземпляр оказывается именно в12, gear пуст и бонус снят. Ring изAccessory1 вAccessory2 меняет слот без удвоения бонуса. Two-handed sword при надетомShield отклоняется, обе вещи остаются.

## 7. Настройка в Unity

Не компонент и не ScriptableObject. Создаётся прежним EquipmentController. Для новых типов добавлять enum в конец, обновлять SlotCount/Fits/UI и тесты. В текущей SampleScene подключены9 views.

## 8. Возможные проблемы

Нет дискового SaveSystem; будущему loader нужны stable item IDs, версия и validation. JsonUtility ссылки Unity Object не являются переносимыми IDs между запусками. Подписчики Changed должны быть корректными; состояние уже согласовано до уведомления. OffHand weapon даёт обычные bonuses, отдельной второй атаки/dual-wield механики нет.

## 9. История изменений

- 2026-10-12 — Player rework5: OffHand8/atomic transfers или view silhouette/drag/compatibility; старые IDs сохранены.
2026-10-12 — добавлен восьмой слот Gloves; протокол атомарных операций сохранён.

2026-10-12 — 4.4: семь слотов, атомарный обмен/снятие/бонусы/защита событий.

- 2026-10-26 — подэтап5.3: бесклассовая прокачка/soft caps/Weapon Scaling; актуальные методы, значения и условия описаны в начале документа.

- 2026-10-26 — броня/Holy/рейтинговая защита подэтапа5.4; описание изменений приведено в начале документа.

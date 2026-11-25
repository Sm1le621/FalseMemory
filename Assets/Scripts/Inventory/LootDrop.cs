using UnityEngine;

// Нагорода з ворога. Вішається на ворога поруч з BattleUnit.
// Після перемоги BattleManager кладе предмет в інвентар гравця.
[DisallowMultipleComponent]
public sealed class LootDrop : MonoBehaviour
{
    // Який предмет випадає (ассет ItemData)
    public ItemData item;
    // Скільки штук
    [Min(1)] public int count = 1;

    // Віддати нагороду гравцю. Повертає текст для повідомлення в бою.
    public string GiveTo(BattleUnit player)
    {
        if (item == null || player == null || player.Inventory == null)
        {
            return "";
        }

        bool added = player.Inventory.TryAdd(item, count);
        if (!added)
        {
            return item.displayName + " is lost: the inventory is full.";
        }

        return "You found " + item.displayName + "!";
    }
}

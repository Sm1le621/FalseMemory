using UnityEngine;

// Як показувати предмети: кольори клітинок і назви слотів.
// Тільки вигляд - на бій це ніяк не впливає.
public static class ItemPresentation
{
    // Колір фону клітинки
    public static Color CellColor(ItemData item, bool highlighted)
    {
        Color dark = new Color32(27, 27, 31, 255);
        Color filled = new Color32(89, 77, 63, 255);
        Color empty = new Color32(60, 55, 48, 255);

        Color baseColor;
        if (item != null)
        {
            baseColor = filled;
        }
        else
        {
            baseColor = empty;
        }

        // Підсвічена клітинка - світліша
        float amount = 0.25f;
        if (highlighted)
        {
            amount = 0.55f;
        }

        return Color.Lerp(dark, baseColor, amount);
    }

    // Назва слота
    public static string SlotName(EquipmentSlot slot)
    {
        if (slot == EquipmentSlot.Weapon)
        {
            return "Weapon";
        }
        return "Chest";
    }
}

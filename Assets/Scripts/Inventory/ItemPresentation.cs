using System.Collections.Generic;
using UnityEngine;

// Як показувати предмети: кольори клітинок і тексти описів.
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

    // Назва типу предмета
    public static string TypeName(ItemData item)
    {
        if (item == null)
        {
            return "";
        }

        switch (item.category)
        {
            case ItemCategory.Weapon:
                return "Weapon";
            case ItemCategory.Armor:
                return SlotName(item.equipmentSlot);
            default:
                return "Consumable";
        }
    }

    // Текст бонусів, наприклад "ATK +5  •  DEF +4"
    public static string BonusText(ItemData item)
    {
        if (item == null)
        {
            return "";
        }

        List<string> parts = new List<string>();
        ItemStatBonus b = item.bonuses;
        Add(parts, "ATK", b.attack);
        Add(parts, "DEF", b.defense);

        // Для витратних предметів - що вони відновлюють
        BattleConsumable consumable = item as BattleConsumable;
        if (consumable != null)
        {
            if (consumable.healing > 0)
            {
                parts.Add("Restores up to " + consumable.healing + " HP");
            }
            if (consumable.manaRestoration > 0)
            {
                parts.Add("Restores up to " + consumable.manaRestoration + " MP");
            }
        }

        return string.Join("  •  ", parts);
    }

    // Додати "NAME +value", тільки якщо значення більше 0
    private static void Add(List<string> parts, string name, int value)
    {
        if (value > 0)
        {
            parts.Add(name + " +" + value);
        }
    }

    // Повний опис для підказки
    public static string Description(ItemData item, int count)
    {
        if (item == null)
        {
            return "";
        }

        List<string> lines = new List<string>();

        // Назва жирним (<b> - це тег TextMeshPro) і кількість, якщо більше 1
        string title = "<b>" + Escape(item.displayName) + "</b>";
        if (count > 1)
        {
            title = title + "  ×" + count;
        }
        lines.Add(title);

        lines.Add(TypeName(item));

        string bonuses = BonusText(item);
        if (bonuses.Length > 0)
        {
            lines.Add(bonuses);
        }

        if (!string.IsNullOrWhiteSpace(item.description))
        {
            lines.Add("\n" + Escape(item.description));
        }

        return string.Join("\n", lines);
    }

    // Назви та описи - це просто текст, а не теги TMP.
    // Замінюємо < і > на схожі символи, щоб ніхто не зламав розмітку
    private static string Escape(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }
        return text.Replace("<", "‹").Replace(">", "›");
    }
}

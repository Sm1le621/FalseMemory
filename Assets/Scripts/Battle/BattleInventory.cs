using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public sealed class BattleItemStack
{
    public BattleConsumable definition;
    [Min(0)] public int count;
}

// Session storage on the world player. Hide/show never resets counts.
[DisallowMultipleComponent]
public sealed class BattleInventory : MonoBehaviour
{
    public List<BattleItemStack> items = new List<BattleItemStack>();
    public IEnumerable<BattleConsumable> Definitions => items.Where(s => s != null && s.definition != null).Select(s => s.definition).Distinct();

    public int Count(BattleConsumable definition)
    {
        if (definition == null) return 0;
        long total = 0;
        foreach (var stack in items)
            if (stack != null && stack.definition == definition) total += Math.Max(0, stack.count);
        return (int)Math.Min(int.MaxValue, total);
    }

    public bool TryConsume(BattleConsumable definition)
    {
        if (definition == null) return false;
        foreach (var stack in items)
        {
            if (stack == null || stack.definition != definition || stack.count <= 0) continue;
            stack.count--;
            return true;
        }
        return false;
    }

    public string Summary => string.Join("  ", Definitions.Select(d => d.displayName + " ×" + Count(d)));
}

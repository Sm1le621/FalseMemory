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
    // Existing scene entries remain startup seeds, preserving asset/scene GUIDs.
    public List<BattleItemStack> items = new List<BattleItemStack>();
    public List<StartingItem> startingItems = new List<StartingItem>();
    [NonSerialized] private InventoryState state;
    public InventoryState State
    {
        get
        {
            if(state!=null)return state;
            var candidate=new InventoryState();
            foreach(var seed in items ?? new List<BattleItemStack>())
                if(seed!=null && seed.definition!=null && seed.count>0 && !candidate.TryAdd(seed.definition,seed.count))
                    throw new InvalidOperationException("Начальные предметы не помещаются в инвентарь30 слотов.");
            foreach(var seed in startingItems ?? new List<StartingItem>())
                if(seed!=null && seed.definition!=null && seed.count>0 && !candidate.TryAdd(seed.definition,seed.count))
                    throw new InvalidOperationException("Начальные предметы не помещаются в инвентарь30 слотов.");
            state=candidate;return state;
        }
    }
    // Keep exhausted starting consumables visible/disabled in the existing battle menu.
    public IEnumerable<BattleConsumable> Definitions => State.Definitions.OfType<BattleConsumable>()
        .Concat((items ?? new List<BattleItemStack>()).Where(s=>s!=null && s.definition!=null).Select(s=>s.definition)).Distinct();

    public int Count(ItemData definition)
    {
        return State.Count(definition);
    }

    public bool TryConsume(BattleConsumable definition)
    {
        return State.TryRemove(definition,1);
    }

    public bool TryAdd(ItemData definition,int amount) => State.TryAdd(definition,amount);
    public string Summary => string.Join("  ", Definitions.Select(d => GameText.Get("item.quantity", d.LocalizedName,Count(d))));
}

[Serializable] public sealed class StartingItem
{
    public ItemData definition;
    [Min(0)] public int count=1;
}

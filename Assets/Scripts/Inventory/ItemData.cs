using System;
using UnityEngine;

public enum ItemCategory { Consumable, Weapon, Armor }
public enum EquipmentSlot { Weapon, Chest }
[Flags] public enum ItemUseLocation { None=0, Exploration=1, Battle=2, Anywhere=3 }

[Serializable]
public struct ItemStatBonus
{
    [Min(0)] public int attack, defense;
    public static ItemStatBonus operator +(ItemStatBonus a, ItemStatBonus b) => new ItemStatBonus
    {
        attack=Sum(a.attack,b.attack), defense=Sum(a.defense,b.defense)
    };
    public static int Sum(int a,int b) => (int)Math.Min(int.MaxValue,(long)Math.Max(0,a)+Math.Max(0,b));
}

// Static definitions only. Quantities and equipped slots never live on this asset.
[CreateAssetMenu(menuName="FalseMemory/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string displayName="Item";
    [TextArea] public string description;
    public Sprite icon;
    public ItemCategory category=ItemCategory.Consumable;
    public bool stackable=true;
    [Min(1)] public int maxStack=99;
    [Min(0)] public int buyPrice, sellPrice;
    public ItemUseLocation useLocations=ItemUseLocation.Anywhere;
    public EquipmentSlot equipmentSlot=EquipmentSlot.Chest;
    public ItemStatBonus bonuses;
    public bool IsEquipment => category==ItemCategory.Weapon || category==ItemCategory.Armor;
    public int StackLimit => IsEquipment || !stackable ? 1 : Math.Max(1,maxStack);
    public bool CanSell => sellPrice>0;
    public bool Fits(EquipmentSlot slot) => category==ItemCategory.Weapon ? slot==EquipmentSlot.Weapon :
        category==ItemCategory.Armor && equipmentSlot==slot;
}

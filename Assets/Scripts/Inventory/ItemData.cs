using System;
using UnityEngine;

public enum ItemCategory { Consumable, Weapon, Armor, Accessory, Material, QuestItem }
// Append new slots: existing serialized numeric values must stay unchanged.
public enum EquipmentSlot { Weapon, Head, Chest, Legs, Boots, Accessory1, Accessory2, Gloves }
public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic }
[Flags] public enum ItemUseLocation { None=0, Exploration=1, Battle=2, Anywhere=3 }

[Serializable]
public struct ItemStatBonus
{
    [Min(0)] public int attack, defense, magicAttack, magicDefense, speed;
    public static ItemStatBonus operator +(ItemStatBonus a, ItemStatBonus b) => new ItemStatBonus
    {
        attack=Sum(a.attack,b.attack), defense=Sum(a.defense,b.defense),
        magicAttack=Sum(a.magicAttack,b.magicAttack), magicDefense=Sum(a.magicDefense,b.magicDefense), speed=Sum(a.speed,b.speed)
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
    public ItemRarity rarity=ItemRarity.Common;
    public bool stackable=true;
    [Min(1)] public int maxStack=99;
    [Min(0)] public int buyPrice, sellPrice;
    [Min(1)] public int requiredLevel=1;
    public ItemUseLocation useLocations=ItemUseLocation.Anywhere;
    public EquipmentSlot equipmentSlot=EquipmentSlot.Chest;
    public ItemStatBonus bonuses;
    public string LocalizedName=>GameText.Content(id+".name",displayName);
    public string LocalizedDescription=>GameText.Content(id+".description",description);
    public bool IsEquipment => category==ItemCategory.Weapon || category==ItemCategory.Armor || category==ItemCategory.Accessory;
    public int StackLimit => IsEquipment || !stackable ? 1 : Math.Max(1,maxStack);
    public bool CanSell => category!=ItemCategory.QuestItem && sellPrice>0;
    public bool Fits(EquipmentSlot slot) => category==ItemCategory.Weapon ? slot==EquipmentSlot.Weapon :
        category==ItemCategory.Accessory ? slot==EquipmentSlot.Accessory1 || slot==EquipmentSlot.Accessory2 :
        category==ItemCategory.Armor && equipmentSlot==slot &&
        ((slot>=EquipmentSlot.Head && slot<=EquipmentSlot.Boots) || slot==EquipmentSlot.Gloves);
}

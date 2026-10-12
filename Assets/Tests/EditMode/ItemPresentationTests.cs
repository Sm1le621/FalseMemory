using NUnit.Framework;
using UnityEngine;

public sealed class ItemPresentationTests
{
    [TestCase(ItemRarity.Common)] [TestCase(ItemRarity.Uncommon)] [TestCase(ItemRarity.Rare)]
    [TestCase(ItemRarity.Epic)] [TestCase(ItemRarity.Legendary)] [TestCase(ItemRarity.Mythic)]
    public void RarityIsNamedAndColoredWithoutChangingGameplay(ItemRarity rarity)
    {
        var item=ScriptableObject.CreateInstance<ItemData>();
        try
        {
            item.rarity=rarity;item.category=ItemCategory.Weapon;item.bonuses=new ItemStatBonus{attack=5};
            Assert.That(ItemPresentation.Description(item,1),Does.Contain(rarity.ToString()).And.Contain("ATK +5"));
            Assert.That(ItemPresentation.RarityColor(rarity).a,Is.EqualTo(1));
            Assert.That(item.StackLimit,Is.EqualTo(1));Assert.That(item.bonuses.attack,Is.EqualTo(5));
        }
        finally {Object.DestroyImmediate(item);}
    }
    [Test] public void TooltipHidesEmptyFieldsEscapesMarkupAndReportsRealConsumableEffect()
    {
        var item=ScriptableObject.CreateInstance<BattleConsumable>();
        try
        {
            item.displayName="<size=200>Potion</size>";item.description="";item.healing=30;
            var text=ItemPresentation.Description(item,3);
            Assert.That(text,Does.Contain("×3").And.Contain("Restores up to 30 HP"));
            Assert.That(text,Does.Not.Contain("<size=").And.Not.Contain("Buy:").And.Not.Contain("Sell:").And.Not.Contain("MATK"));
            item.healing=0;item.manaRestoration=20;item.buyPrice=50;item.sellPrice=25;
            Assert.That(ItemPresentation.Description(item,1),Does.Contain("Restores up to 20 MP").And.Contain("Buy: 50").And.Contain("Sell: 25").And.Not.Contain(" HP"));
        }
        finally {Object.DestroyImmediate(item);}
    }
    [Test] public void AppendingGlovesPreservesOriginalSerializedSlotsAndDoesNotAcceptAccessories()
    {
        Assert.That((int)EquipmentSlot.Accessory1,Is.EqualTo(5));Assert.That((int)EquipmentSlot.Accessory2,Is.EqualTo(6));
        var item=ScriptableObject.CreateInstance<ItemData>();
        try
        {
            item.category=ItemCategory.Armor;item.equipmentSlot=EquipmentSlot.Gloves;
            Assert.That(item.Fits(EquipmentSlot.Gloves),Is.True);Assert.That(item.Fits(EquipmentSlot.Boots),Is.False);
            item.category=ItemCategory.Accessory;Assert.That(item.Fits(EquipmentSlot.Gloves),Is.False);
            Assert.That(ItemPresentation.Description(null,1),Is.Empty);
        }
        finally {Object.DestroyImmediate(item);}
    }
}

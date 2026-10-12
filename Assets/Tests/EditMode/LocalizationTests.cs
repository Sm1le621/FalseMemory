using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Localization.Tables;

public sealed class LocalizationTests
{
    [Test] public void EnglishCatalogHasUniqueKeysNonemptyEnglishValuesAndValidFormats()
    {
        var table=Resources.Load<StringTable>("Localization/GameText_en-US");Assert.That(table,Is.Not.Null);
        Assert.That(table.LocaleIdentifier.Code,Is.EqualTo("en-US"));Assert.That(table.Count,Is.GreaterThan(100));
        var keys=new System.Collections.Generic.HashSet<string>();var args=new object[16];for(int i=0;i<args.Length;i++)args[i]=i;
        foreach(var pair in table)
        {
            string key=table.SharedData.GetEntry(pair.Key).Key;Assert.That(keys.Add(key),Is.True,key);
            Assert.That(pair.Value.Value,Is.Not.Empty,key);Assert.That(Regex.IsMatch(pair.Value.Value,"[А-Яа-яЁё]"),Is.False,key);
            Assert.DoesNotThrow(()=>string.Format(CultureInfo.GetCultureInfo("en-US"),pair.Value.Value,args),key);
            Assert.That(GameText.Contains(key),Is.True,key);
        }
    }
    [Test] public void DynamicResourcesAndExperienceUseEnglishTemplates()
    {
        Assert.That(GameText.Get("resources.hp_mp",90,100,5,30),Is.EqualTo("HP 90 / 100    MP 5 / 30"));
        Assert.That(GameText.Get("character.header","Player",2,20),Is.EqualTo("Player\nLevel 2  •  EXP 20"));
        Assert.That(GameText.Get("battle.spell_damage","Player","Fire",19),Is.EqualTo("Player casts Fire: 19 magic damage."));
    }
    [Test] public void MissingKeysBadArgumentsAndUnsupportedLanguagesAreSafe()
    {
        Assert.That(GameText.Get("test.missing.key"),Is.EqualTo("[test.missing.key]"));
        Assert.That(GameText.Get("resources.hp_mp",1),Is.EqualTo("[invalid: resources.hp_mp]"));
        Assert.That(GameText.Get("resources.hp_mp"),Is.EqualTo("[invalid: resources.hp_mp]"));
        Assert.That(GameText.Content("test.missing.key","English fallback"),Is.EqualTo("English fallback"));
        Assert.That(GameText.TrySetLocale("uk-UA"),Is.False);Assert.That(GameText.TrySetLocale(null),Is.False);
        Assert.That(GameText.LocaleCode,Is.EqualTo("en-US"));Assert.That(GameText.TrySetLocale("en-US"),Is.True);
    }
    [Test] public void EveryRarityAndEquipmentSlotHasACentralizedEnglishLabel()
    {
        foreach(ItemRarity rarity in Enum.GetValues(typeof(ItemRarity)))Assert.That(GameText.Contains("rarity."+rarity.ToString().ToLowerInvariant()),Is.True);
        foreach(EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))Assert.That(GameText.Contains("equipment.slot."+slot.ToString().ToLowerInvariant()),Is.True);
    }
}

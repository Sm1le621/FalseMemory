using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed partial class BattleFlowTests
{
    private static void AssertEnglishActiveUi()
    {
        foreach(var text in Object.FindObjectsByType<TMP_Text>())
        {
            if(!text.isActiveAndEnabled)continue;
            Assert.That(Regex.IsMatch(text.text,"[А-Яа-яЁё]"),Is.False,text.name+": "+text.text);
            Assert.That(text.text.StartsWith("[invalid:") || text.text.StartsWith("[inventory.") || text.text.StartsWith("[battle."),Is.False,text.name);
        }
    }
    [UnityTest] public IEnumerator MenuAndSettingsUseEnglishAndTheirButtonsStillWork()
    {
        yield return SceneManager.LoadSceneAsync("MainMenu",LoadSceneMode.Single);yield return null;
        Assert.That(GameObject.Find("StartButton").GetComponentInChildren<TMP_Text>().text,Is.EqualTo("New Game"));
        Assert.That(GameObject.Find("SettingsButton").GetComponentInChildren<TMP_Text>().text,Is.EqualTo("Settings"));
        foreach(var label in Object.FindObjectsByType<TMP_Text>())
            Assert.That(label.extraPadding,Is.False,"Bitmap menu glyphs must not sample their neighbors: "+label.name);
        AssertEnglishActiveUi();GameObject.Find("SettingsButton").GetComponent<Button>().onClick.Invoke();yield return null;
        Assert.That(GameObject.Find("SettingsTitle").GetComponent<TMP_Text>().text,Is.EqualTo("Settings"));
        Assert.That(GameObject.Find("VolumeLabel").GetComponent<TMP_Text>().text,Is.EqualTo("Volume"));AssertEnglishActiveUi();
        GameObject.Find("BackButton").GetComponent<Button>().onClick.Invoke();yield return null;
        Assert.That(GameObject.Find("SettingsTitle"),Is.Null);
        GameObject.Find("StartButton").GetComponent<Button>().onClick.Invoke();yield return WaitUntil(()=>SceneManager.GetSceneByName("SampleScene").isLoaded);yield return null;
        Assert.That(Player().Inventory.Count(Potion(Player())),Is.EqualTo(3));AssertEnglishActiveUi();
    }
    [UnityTest] public IEnumerator InventoryTooltipMagicItemsAndBattleResultsUseEnglish()
    {
        var player=Player();var enemy=Enemy();var window=player.GetComponent<InventoryWindow>();window.Open();yield return null;
        window.tooltip.Show(window.slots[0],Vector2.zero);AssertEnglishActiveUi();Assert.That(window.tooltip.content.text,Does.Contain("Restores up to 30 HP").And.Contain("Buy: 30 gold"));
        window.Close();yield return BeginBattle(player,enemy);var manager=Manager();AssertEnglishActiveUi();
        manager.OpenMagicMenu();Assert.That(manager.ui.messageText.text,Is.EqualTo("Choose a spell. Esc to go back."));AssertEnglishActiveUi();
        manager.ShowMainMenu();manager.OpenItemsMenu();Assert.That(manager.ui.messageText.text,Is.EqualTo("Choose an item. Esc to go back."));AssertEnglishActiveUi();
        manager.ShowMainMenu();enemy.Health.TakeDamage(50);manager.TryPlayerAction(BattleAction.Attack);yield return WaitUntil(()=>manager.State==BattleState.Victory);
        Assert.That(manager.ui.resultText.text,Does.Contain("Victory!").And.Contain("EXP Gained: +20"));AssertEnglishActiveUi();
    }
}

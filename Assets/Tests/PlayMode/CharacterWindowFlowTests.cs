using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed partial class BattleFlowTests
{
#if UNITY_EDITOR
    [UnityTest] public IEnumerator CharacterPagesAndEdgeTooltipsFitFullHdAndUltrawide()
    {
        var assembly=typeof(UnityEditor.Editor).Assembly;var type=assembly.GetType("UnityEditor.GameView");var view=UnityEditor.EditorWindow.GetWindow(type);
        var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic;
        var selected=type.GetProperty("selectedSizeIndex",flags);int original=(int)selected.GetValue(view,null);
        var sizes=assembly.GetType("UnityEditor.GameViewSizes");var instance=sizes.GetProperty("instance",System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.FlattenHierarchy).GetValue(null,null);
        var getGroup=sizes.GetMethod("GetGroup");var group=getGroup.Invoke(instance,new[]{System.Enum.Parse(getGroup.GetParameters()[0].ParameterType,"Standalone")});var groupType=group.GetType();
        int builtins=(int)groupType.GetMethod("GetBuiltinCount").Invoke(group,null);int custom=(int)groupType.GetMethod("GetCustomCount").Invoke(group,null);
        var sizeType=assembly.GetType("UnityEditor.GameViewSize");var enumType=assembly.GetType("UnityEditor.GameViewSizeType");
        var wide=System.Activator.CreateInstance(sizeType,new object[]{System.Enum.Parse(enumType,"FixedResolution"),3440,1440,"FalseMemory Character verification"});
        groupType.GetMethod("AddCustomSize").Invoke(group,new[]{wide});var window=Player().GetComponent<CharacterWindow>();
        try
        {
            window.Open();
            for(int resolution=0;resolution<2;resolution++)
            {
                selected.SetValue(view,resolution==0?3:builtins+custom,null);view.Repaint();yield return null;yield return null;yield return new WaitForEndOfFrame();
                Assert.That(Screen.width,Is.EqualTo(resolution==0?1920:3440));Assert.That(Screen.height,Is.EqualTo(resolution==0?1080:1440));
                for(int tab=0;tab<4;tab++)
                {
                    window.SelectTab(tab);Canvas.ForceUpdateCanvases();AssertOnScreen(window.panel);
                    foreach(var text in window.panel.GetComponentsInChildren<TMPro.TextMeshProUGUI>()){text.ForceMeshUpdate();Assert.That(text.isTextOverflowing,Is.False,text.name+": "+text.text);}
                }
                window.SelectTab(0);
                foreach(var edge in new[]{Vector2.zero,new Vector2(Screen.width,Screen.height)})
                {
                    window.ShowAttribute(CharacterAttribute.Wisdom,edge,true);yield return null;yield return new WaitForEndOfFrame();AssertOnScreen(window.tooltipPanel);
                }
                Assert.That(window.tooltipText.text,Does.Contain("Does not grant MP"));
            }
        }
        finally {window.Close();selected.SetValue(view,original,null);groupType.GetMethod("RemoveCustomSize").Invoke(group,new object[]{builtins+custom});}
    }
    private static void AssertOnScreen(RectTransform rect)
    {
        var corners=new Vector3[4];rect.GetWorldCorners(corners);
        foreach(var p in corners){Assert.That(p.x,Is.InRange(-.1f,Screen.width+.1f),rect.name);Assert.That(p.y,Is.InRange(-.1f,Screen.height+.1f),rect.name);}
    }
#endif
    [UnityTest] public IEnumerator CharacterKeysSwitchModalsBlockMovementAndRejectBattleAndOtherOwners()
    {
        var player=Player();var window=player.GetComponent<CharacterWindow>();var bag=player.GetComponent<InventoryWindow>();var gate=player.GetComponent<WorldInputGate>();
        var keyboard=InputSystem.AddDevice<Keyboard>();var body=player.GetComponent<Rigidbody2D>();Vector2 home=body.position;
        try
        {
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.C,Key.D));yield return null;yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
            Assert.That(window.IsOpen,Is.True);Assert.That(gate.IsBlocked,Is.True);Assert.That(body.position,Is.EqualTo(home));
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.C));yield return null;Assert.That(window.IsOpen,Is.False);Assert.That(gate.IsBlocked,Is.False);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.I));yield return null;Assert.That(bag.IsOpen,Is.True);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.C));yield return null;Assert.That(window.IsOpen,Is.True);Assert.That(bag.IsOpen,Is.False);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.I));yield return null;Assert.That(window.IsOpen,Is.False);Assert.That(bag.IsOpen,Is.True);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Escape));yield return null;Assert.That(bag.IsOpen,Is.False);
            var other=new object();gate.Acquire(other);Assert.That(window.Open(),Is.False);Assert.That(bag.Open(),Is.False);gate.Release(other);
            yield return BeginBattle(player,Enemy());Assert.That(window.Open(),Is.False);
            Manager().Enemy.Health.TakeDamage(999);yield return WaitUntil(()=>Manager().State==BattleState.Victory);
            BattleTransition.Instance.ReturnAfterVictory();yield return WaitUntil(()=>!BattleTransition.Instance.EncounterActive);
            Assert.That(window.Open(),Is.True);Assert.That(window.header.text,Does.Contain("EXP 20"));window.Close();
        }
        finally {InputSystem.RemoveDevice(keyboard);window.Close();bag.Close();}
    }
    [UnityTest] public IEnumerator CharacterAllocationTooltipsLearningAndEquipmentRenderRealStateWithoutDoubleSpend()
    {
        var player=Player();var window=player.GetComponent<CharacterWindow>();player.AddExperience(500);player.Health.TakeDamage(40);player.TrySpendMP(10);
        int points=player.Attributes.UnspentPoints;Assert.That(points,Is.GreaterThanOrEqualTo(2));window.Open();
        window.attributeRows[(int)CharacterAttribute.Vitality].increase.onClick.Invoke();window.attributeRows[(int)CharacterAttribute.Vitality].increase.onClick.Invoke();
        Assert.That(player.Attributes.BaseValues.vitality,Is.EqualTo(11));Assert.That(player.Attributes.UnspentPoints,Is.EqualTo(points-1));
        Assert.That(player.Health.maxHealth,Is.EqualTo(108));Assert.That(player.Health.currentHealth,Is.EqualTo(60));Assert.That(player.CurrentMP,Is.EqualTo(30));
        window.ShowAttribute(CharacterAttribute.Vitality,Vector2.zero,true);Assert.That(window.tooltipPanel.gameObject.activeSelf,Is.True);
        Assert.That(window.tooltipText.text,Does.Contain("Max HP: +8").And.Contain("Next soft cap: 40").And.Contain("Base: 11"));
        player.Attributes.SetBase(CharacterAttribute.Strength,20);window.SelectTab(2);window.SelectAbility(2);int lp=player.Learning.Points;
        Assert.That(window.learnButton.interactable,Is.True);window.learnButton.onClick.Invoke();window.learnButton.onClick.Invoke();
        Assert.That(player.Learning.Points,Is.EqualTo(lp-1));Assert.That(window.abilityDetail.text,Does.Contain("Learned"));
        var sword=GameObject.Find("Pickup_IronSword").GetComponent<WorldItemPickup>().item;player.Inventory.TryAdd(sword,1);Assert.That(player.Equipment.TryEquip(1),Is.True);
        window.SelectTab(1);Assert.That(window.combatStats.text,Does.Contain("Physical Attack: 30"));
        window.SelectTab(3);Assert.That(window.equipmentDetail.text,Does.Contain("Iron Sword"));
        player.Attributes.SetBase(CharacterAttribute.Vitality,99);window.SelectTab(0);Assert.That(window.attributeRows[(int)CharacterAttribute.Vitality].increase.interactable,Is.False);
        yield return null;Assert.That(window.Allocate(CharacterAttribute.Vitality),Is.False);window.Close();
    }
}

using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

public static class CharacterWindowSetup
{
    private static TMP_FontAsset font;
    public static void ApplyStage56()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first.");
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(scene.path!="Assets/Scenes/SampleScene.unity")throw new InvalidOperationException("Open SampleScene.");
        var table=AssetDatabase.LoadAssetAtPath<StringTable>("Assets/Resources/Localization/GameText_en-US.asset");
        string[,] entries={
            {"character.window_title","Character"},{"character.tab.attributes","Attributes"},{"character.tab.combat","Combat Stats"},{"character.tab.abilities","Abilities"},{"character.tab.equipment","Equipment"},
            {"attribute.wisdom.description","Magic Defense, WIS Scaling and additional Holy damage against Undead and Unholy foes. Does not grant MP."},
            {"character.window_header","{0}  |  Level {1}  |  EXP {2}  |  {3}\nHP {4} / {5}    MP {6} / {7}"},
            {"character.free_points","Attribute Points: {0}"},{"character.base_row","{0}  {1}: {2} ({3:+#;-#;0})  ->  {4}"},
            {"character.attribute_detail","{0}\n{1}\n\nBase: {2}  |  Equipment: {3:+#;-#;0}\nTemporary: {4:+#;-#;0}  |  Effective: {5}\nNext soft cap: {6}\n\nNext point:\n{7}"},
            {"character.next_delta","{0}: {1:+#;-#;0}"},{"character.next_percent","{0}: {1:+0.###;-0.###;0} percentage points"},
            {"character.next_mp","{0} MP Cost: {1:+#;-#;0}"},{"character.base_cap","Base attribute limit reached (99)."},
            {"character.no_immediate_change","No immediate combat change after rounding. Some effects are prepared for future systems."},
            {"character.combat_values","Physical Attack: {0}\nMagic Attack: {1}\nPhysical Defense: {2}\nMagic Defense: {3}\nCritical Chance: {4:0.#}%\nEvasion: {5:0.#}%\nInitiative: {6}\nHoly Damage Bonus: {7:0.#}%\nMiss Chance: {8:0.#}%"},
            {"character.weapon_power","Weapon Power"},{"character.critical","Critical Chance"},{"character.evasion","Evasion"},{"character.accuracy","Accuracy"},{"character.holy","Holy Damage Bonus"},{"character.healing","Healing Power"},
            {"character.unarmed","Unarmed\nNo weapon Scaling or requirements."},
            {"character.ability_detail","{0}\n{1}\n\nMP Cost: {2}\nRequires: {3}\nStatus: {4}\nDamage Type: {5}"},
            {"character.ability_row","{0}  |  {1}"},{"character.equipment_row","{0}: {1}"},{"character.empty_slot","Empty"},
            {"character.equipment_hint","Select an equipped item to inspect its bonuses, Scaling and requirements. Change equipment in Inventory (I)."},
            {"character.controls","C / Esc: Close   |   I: Inventory"},{"character.increase","+"},
            {"derived.attack","Physical Attack"},{"derived.defense","Physical Defense"},{"derived.magicattack","Magic Attack"},{"derived.magicdefense","Magic Defense"},
            {"derived.speed","Initiative"},{"derived.maximumhealth","Max HP"},{"derived.maximummana","Max MP"}
        };
        for(int i=0;i<entries.GetLength(0);i++)Add(table,entries[i,0],entries[i,1]);
        var player=GameObject.Find("Player");var inventory=player.GetComponent<InventoryWindow>();
        var window=player.GetComponent<CharacterWindow>();if(window!=null && window.panel!=null)throw new InvalidOperationException("Character UI already authored; do not overwrite it.");
        if(window==null)window=player.AddComponent<CharacterWindow>();
        var canvas=GameObject.Find("InventoryCanvas").GetComponent<RectTransform>();font=inventory.details.font;
        window.backdrop=Box("CharacterBackdrop",canvas,new Vector2(0,0),Vector2.zero,new Color(0,0,0,.78f)).GetComponent<Image>();
        var back=window.backdrop.rectTransform;back.anchorMin=Vector2.zero;back.anchorMax=Vector2.one;back.offsetMin=back.offsetMax=Vector2.zero;
        window.panel=Box("CharacterPanel",canvas,Vector2.zero,new Vector2(1120,680),new Color32(22,22,28,255));
        window.panel.anchorMin=window.panel.anchorMax=window.panel.pivot=new Vector2(.5f,.5f);Border(window.panel);
        Label("Title",window.panel,28,18,680,38,GameText.Get("character.window_title"),30);
        window.header=Label("Header",window.panel,28,64,800,66,"",20);
        window.points=Label("Points",window.panel,810,70,270,52,"",20);
        window.closeButton=Button("Close",window.panel,1024,18,68,36,"X");
        window.tabs=new Button[4];window.pages=new RectTransform[4];string[] keys={"attributes","combat","abilities","equipment"};
        for(int i=0;i<4;i++)
        {
            window.tabs[i]=Button("Tab_"+keys[i],window.panel,28+i*268,140,260,42,GameText.Get("character.tab."+keys[i]));
            window.pages[i]=Box("Page_"+keys[i],window.panel,new Vector2(28,-198),new Vector2(1064,420),new Color32(28,28,34,255));
        }
        window.attributeRows=new CharacterAttributeView[10];
        for(int i=0;i<10;i++)
        {
            var row=Box("Attribute_"+i,window.pages[0],new Vector2(12,-10-i*40),new Vector2(510,36),new Color32(36,35,41,255));
            var view=row.gameObject.AddComponent<CharacterAttributeView>();view.owner=window;view.attribute=(CharacterAttribute)i;
            view.label=Label("Value",row,10,3,446,30,"",17);view.increase=Button("Increase",row,470,2,34,32,GameText.Get("character.increase"));window.attributeRows[i]=view;
        }
        window.attributeDetail=Label("AttributeDetail",window.pages[0],552,12,490,400,"",20);
        window.combatStats=Label("CombatStats",window.pages[1],20,18,480,385,"",24);
        window.combatWeapon=Label("WeaponScaling",window.pages[1],552,18,490,385,"",19);
        window.learningPoints=Label("LearningPoints",window.pages[2],16,10,500,34,"",21);
        window.abilityButtons=new Button[6];
        for(int i=0;i<6;i++)window.abilityButtons[i]=Button("Ability_"+i,window.pages[2],16,56+i*54,502,48,"");
        window.abilityDetail=Label("AbilityDetail",window.pages[2],552,16,490,326,"",20);
        window.learnButton=Button("Learn",window.pages[2],552,358,490,44,"");
        window.equipmentButtons=new Button[9];
        for(int i=0;i<9;i++)window.equipmentButtons[i]=Button("Equipment_"+i,window.pages[3],16,14+i*44,502,38,"");
        window.equipmentDetail=Label("EquipmentDetail",window.pages[3],552,16,490,390,"",19);
        Label("Controls",window.panel,28,635,1064,28,GameText.Get("character.controls"),18);
        window.tooltipPanel=Box("CharacterTooltip",canvas,Vector2.zero,new Vector2(400,330),new Color32(17,17,23,255));Border(window.tooltipPanel);
        window.tooltipPanel.anchorMin=window.tooltipPanel.anchorMax=new Vector2(.5f,.5f);window.tooltipPanel.pivot=new Vector2(0,1);
        window.tooltipText=Label("Content",window.tooltipPanel,16,16,368,298,"",18);window.tooltipText.raycastTarget=false;
        var tr=window.tooltipText.rectTransform;tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=new Vector2(16,16);tr.offsetMax=new Vector2(-16,-16);
        window.tooltipPanel.GetComponent<Image>().raycastTarget=false;
        foreach(var page in window.pages)page.gameObject.SetActive(false);window.pages[0].gameObject.SetActive(true);
        window.panel.gameObject.SetActive(false);window.tooltipPanel.gameObject.SetActive(false);window.backdrop.gameObject.SetActive(false);
        EditorUtility.SetDirty(table);EditorUtility.SetDirty(table.SharedData);AssetDatabase.SaveAssetIfDirty(table.SharedData);AssetDatabase.SaveAssetIfDirty(table);
        EditorUtility.SetDirty(window);UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        if(!UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Scene save failed.");
    }
    private static RectTransform Box(string name,Transform parent,Vector2 position,Vector2 size,Color color)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));go.transform.SetParent(parent,false);
        var rect=go.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.anchoredPosition=position;rect.sizeDelta=size;go.GetComponent<Image>().color=color;return rect;
    }
    private static void Border(RectTransform rect){var outline=rect.gameObject.AddComponent<Outline>();outline.effectColor=new Color32(143,118,77,255);outline.effectDistance=new Vector2(2,-2);}
    private static TextMeshProUGUI Label(string name,Transform parent,float x,float y,float width,float height,string text,float size)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);
        var rect=go.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(width,height);
        var label=go.GetComponent<TextMeshProUGUI>();label.font=font;label.fontSize=size;label.text=text;label.color=new Color32(223,215,195,255);label.raycastTarget=false;label.textWrappingMode=TextWrappingModes.Normal;return label;
    }
    private static Button Button(string name,Transform parent,float x,float y,float width,float height,string text)
    {
        var rect=Box(name,parent,new Vector2(x,-y),new Vector2(width,height),new Color32(45,43,49,255));Border(rect);
        var button=rect.gameObject.AddComponent<Button>();button.navigation=new Navigation{mode=Navigation.Mode.None};
        var label=Label("Label",rect,8,2,width-16,height-4,text,18);label.alignment=TextAlignmentOptions.MidlineLeft;return button;
    }
    private static void Add(StringTable table,string key,string value){var entry=table.GetEntry(key);if(entry==null)table.AddEntry(key,value);else entry.Value=value;}
}

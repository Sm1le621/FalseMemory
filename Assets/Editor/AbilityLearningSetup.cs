using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Tables;

public static class AbilityLearningSetup
{
    public static void ApplyStage55()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first.");
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(scene.path!="Assets/Scenes/SampleScene.unity")throw new InvalidOperationException("Open SampleScene.");
        var table=AssetDatabase.LoadAssetAtPath<StringTable>("Assets/Resources/Localization/GameText_en-US.asset");
        var player=GameObject.Find("Player");var unit=player.GetComponent<BattleUnit>();
        var learn=player.GetComponent<AbilityLearning>();if(learn==null)learn=player.AddComponent<AbilityLearning>();
        var fire=unit.spells[0];var heal=unit.spells[1];
        fire.id=fire.localizationId;fire.description="A focused flame that deals magic damage.";fire.requirements=new CharacterAttributeValues{magicPower=15,intelligence=12};
        heal.id=heal.localizationId;heal.description="Restore your own health.";
        foreach(var spell in new[]{fire,heal}){Add(table,spell.localizationId+".description",spell.description);EditorUtility.SetDirty(spell);AssetDatabase.SaveAssetIfDirty(spell);}
        learn.catalog=new[]{fire,heal,
            Make(table,"PowerStrike","Power Strike","A powerful physical attack.",new CharacterAttributeValues{strength=20},BattleEffect.PhysicalDamage,6,0,1.5f,0,0),
            Make(table,"QuickStrike","Quick Strike","A precise physical attack with improved accuracy.",new CharacterAttributeValues{dexterity=18,agility=15},BattleEffect.PhysicalDamage,5,0,1.15f,.2f,0),
            Make(table,"GreaterHeal","Greater Heal","Restore a large amount of your own health.",new CharacterAttributeValues{magicPower=20,wisdom=20},BattleEffect.RestoreHealth,12,50,1,0,0),
            Make(table,"SacredStrike","Sacred Strike","A physical strike with additional Holy damage against Undead and Unholy foes.",new CharacterAttributeValues{wisdom=25,strength=15},BattleEffect.PhysicalDamage,8,0,1.2f,0,8)};
        learn.pointsPerLevel=1;
        Add(table,"ability.state.learned","Learned");Add(table,"ability.state.requirements","Requirements not met");Add(table,"ability.state.points","Not enough Learning Points");Add(table,"ability.state.available","Available to learn");
        Add(table,"ability.learning_points","Learning Points: {0}");Add(table,"ability.learn","Learn ({0} LP)");Add(table,"ability.requirements","Level {0} | {1}");
        EditorUtility.SetDirty(table);EditorUtility.SetDirty(table.SharedData);AssetDatabase.SaveAssetIfDirty(table.SharedData);AssetDatabase.SaveAssetIfDirty(table);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);if(!UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Scene save failed.");
    }
    private static BattleSpell Make(StringTable table,string file,string name,string description,CharacterAttributeValues req,BattleEffect effect,int cost,int power,float multiplier,float accuracy,int holy)
    {
        string path="Assets/Settings/Battle/Spells/"+file+".asset";var spell=AssetDatabase.LoadAssetAtPath<BattleSpell>(path);
        if(spell!=null && spell.StableId!="ability."+file.ToLowerInvariant())throw new InvalidOperationException("Unrelated asset: "+path);
        if(spell==null){spell=ScriptableObject.CreateInstance<BattleSpell>();AssetDatabase.CreateAsset(spell,path);}
        spell.id=spell.localizationId="ability."+file.ToLowerInvariant();spell.displayName=name;spell.description=description;spell.requirements=req;
        spell.effect=effect;spell.target=effect==BattleEffect.RestoreHealth?BattleTarget.Self:BattleTarget.Opponent;
        spell.mpCost=cost;spell.power=power;spell.attackMultiplier=multiplier;spell.accuracyBonus=accuracy;spell.holyPower=holy;
        spell.damageType=effect==BattleEffect.PhysicalDamage?WeaponDamageType.Physical:WeaponDamageType.Magic;
        if(effect==BattleEffect.RestoreHealth)spell.visualEffect=AssetDatabase.LoadAssetAtPath<BattleVFXProfile>("Assets/Settings/Battle/VFX/HealVisual.asset");
        Add(table,spell.localizationId+".name",name);Add(table,spell.localizationId+".description",description);EditorUtility.SetDirty(spell);AssetDatabase.SaveAssetIfDirty(spell);return spell;
    }
    private static void Add(StringTable table,string key,string value){var entry=table.GetEntry(key);if(entry==null)table.AddEntry(key,value);else entry.Value=value;}
}

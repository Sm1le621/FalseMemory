using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Tables;

// Addressed authoring only. Never imports art, replaces existing IDs, or saves unrelated assets.
public static class ClasslessProgressionSetup
{
    public static void ApplyStage53()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode first.");
        var rules=AssetDatabase.LoadAssetAtPath<CharacterAttributeRules>("Assets/Settings/Progression/DefaultCharacterAttributes.asset");
        if(rules==null)throw new InvalidOperationException("Existing attribute rules missing.");
        rules.classlessProgression=true;rules.attributePointsPerLevel=1;rules.referenceMana=40;rules.maximumManaReduction=.3f;
        rules.agilityMovement=0;rules.growthPerLevel=new CharacterAttributeValues();
        EditorUtility.SetDirty(rules);AssetDatabase.SaveAssetIfDirty(rules);
        var table=AssetDatabase.LoadAssetAtPath<StringTable>("Assets/Resources/Localization/GameText_en-US.asset");
        if(table==null)throw new InvalidOperationException("Existing English StringTable missing.");
        Add(table,"scaling.damage","{0} Damage: {1} + {2} = {3}");
        Add(table,"scaling.type.physical","Physical");Add(table,"scaling.type.magic","Magic");
        Add(table,"scaling.weapon_type","Weapon Type: {0}");Add(table,"scaling.grades","Scaling: {0}");
        Add(table,"scaling.none","None");Add(table,"scaling.requires","Requires: {0}");
        Add(table,"scaling.requirement","{0} {1} (current {2})");
        Add(table,"scaling.weapon.sword","Sword");Add(table,"scaling.weapon.greatsword","Greatsword");
        Add(table,"scaling.weapon.rapier","Rapier");Add(table,"scaling.weapon.dagger","Dagger");Add(table,"scaling.weapon.staff","Staff");
        Add(table,"scaling.requirements_failed","Attribute requirements not met. {0}");
        // Class labels are historical data only; the active tooltip shows no class modifier.
        Add(table,"character.attribute_tooltip_classless","{0} {1}\n{2}\nBase: {3}\nEquipment: {4}\nTemporary: {5}\nNext soft cap: {6}");
        Add(table,"attribute.agility.description","Evasion and initiative. Does not change walking speed.");
        MakeWeapon(table,"Greatsword","Greatsword",40,CharacterAttribute.Strength,ScalingGrade.S,CharacterAttribute.Dexterity,ScalingGrade.E,new CharacterAttributeValues{strength=25},WeaponDamageType.Physical,"greatsword");
        MakeWeapon(table,"Longsword","Longsword",28,CharacterAttribute.Strength,ScalingGrade.B,CharacterAttribute.Dexterity,ScalingGrade.C,new CharacterAttributeValues(),WeaponDamageType.Physical,"sword");
        MakeWeapon(table,"Rapier","Rapier",24,CharacterAttribute.Dexterity,ScalingGrade.S,CharacterAttribute.Strength,ScalingGrade.E,new CharacterAttributeValues{dexterity=18},WeaponDamageType.Physical,"rapier");
        MakeWeapon(table,"Dagger","Dagger",18,CharacterAttribute.Dexterity,ScalingGrade.A,CharacterAttribute.Agility,ScalingGrade.C,new CharacterAttributeValues(),WeaponDamageType.Physical,"dagger");
        MakeWeapon(table,"MagicStaff","Magic Staff",20,CharacterAttribute.MagicPower,ScalingGrade.S,CharacterAttribute.Intelligence,ScalingGrade.D,new CharacterAttributeValues{intelligence=20,magicPower=15},WeaponDamageType.Magic,"staff");
        MakeWeapon(table,"SacredBlade","Sacred Blade",30,CharacterAttribute.Wisdom,ScalingGrade.A,CharacterAttribute.Strength,ScalingGrade.B,new CharacterAttributeValues{wisdom=20},WeaponDamageType.Physical,"sword");
        EditorUtility.SetDirty(table);EditorUtility.SetDirty(table.SharedData);
        AssetDatabase.SaveAssetIfDirty(table.SharedData);AssetDatabase.SaveAssetIfDirty(table);
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(scene.path!="Assets/Scenes/SampleScene.unity")throw new InvalidOperationException("Open existing SampleScene.");
        var parent=GameObject.Find("TestLoot");if(parent==null)throw new InvalidOperationException("Existing TestLoot missing.");
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WorldItemPickup.prefab");
        if(prefab==null)throw new InvalidOperationException("Existing pickup prefab missing.");
        string[] files={"Greatsword","Longsword","Rapier","Dagger","MagicStaff","SacredBlade"};
        for(int i=0;i<files.Length;i++)
        {
            string objectName="Pickup_Scaling_"+files[i];
            var child=parent.transform.Find(objectName);
            var go=child!=null?child.gameObject:(GameObject)PrefabUtility.InstantiatePrefab(prefab,scene);
            if(child==null){go.name=objectName;go.transform.SetParent(parent.transform);go.transform.position=new Vector3(-5f-i%3,-1.7f-i/3,0);}
            var pickup=go.GetComponent<WorldItemPickup>();pickup.item=AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Settings/Inventory/"+files[i]+".asset");
            pickup.worldId="scaling_weapon_"+files[i].ToLowerInvariant();pickup.quantity=1;
            go.GetComponent<SpriteRenderer>().sprite=pickup.item.icon;
            EditorUtility.SetDirty(go);
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        if(!UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Scene save failed.");
        GameTextEditorInvalidation();
    }
    private static void Add(StringTable table,string key,string value)
    {
        var entry=table.GetEntry(key);if(entry==null)table.AddEntry(key,value);else entry.Value=value;
    }
    private static void MakeWeapon(StringTable table,string file,string name,int damage,CharacterAttribute first,ScalingGrade firstGrade,CharacterAttribute second,ScalingGrade secondGrade,CharacterAttributeValues requirements,WeaponDamageType type,string weaponType)
    {
        string path="Assets/Settings/Inventory/"+file+".asset";
        var item=AssetDatabase.LoadAssetAtPath<ItemData>(path);
        if(item!=null && item.id!="weapon."+file.ToLowerInvariant())throw new InvalidOperationException("Unrelated item exists: "+path);
        if(item==null){item=ScriptableObject.CreateInstance<ItemData>();AssetDatabase.CreateAsset(item,path);}
        item.id="weapon."+file.ToLowerInvariant();item.displayName=name;item.description="Scales with its listed attributes.";
        item.category=ItemCategory.Weapon;item.stackable=false;item.maxStack=1;
        item.weapon=new WeaponScalingData{baseDamage=damage,damageType=type,weaponType=weaponType,requirements=requirements,
            scaling=new[]{new AttributeScalingEntry{attribute=first,grade=firstGrade},new AttributeScalingEntry{attribute=second,grade=secondGrade}}};
        item.useWeaponScaling=true;item.twoHanded=file=="Greatsword";item.bonuses=new ItemStatBonus();
        if(item.icon==null)item.icon=AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Settings/Inventory/IronSword.asset")?.icon;
        Add(table,item.id+".name",name);Add(table,item.id+".description",item.description);
        EditorUtility.SetDirty(item);AssetDatabase.SaveAssetIfDirty(item);
    }
    private static void GameTextEditorInvalidation()
    {
        // Runtime reset will read the same table on the next Play; no second text store.
        EditorApplication.QueuePlayerLoopUpdate();
    }
}

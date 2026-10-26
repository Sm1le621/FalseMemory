using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Tables;

public static class ArmorHolySetup
{
    public static void ApplyStage54()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first.");
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(scene.path!="Assets/Scenes/SampleScene.unity")throw new InvalidOperationException("Open SampleScene.");
        var rules=AssetDatabase.LoadAssetAtPath<BattleRules>("Assets/Settings/Battle/DefaultBattleRules.asset");
        rules.useDiminishingDefense=true;rules.defenseRatingConstant=20;EditorUtility.SetDirty(rules);AssetDatabase.SaveAssetIfDirty(rules);
        var table=AssetDatabase.LoadAssetAtPath<StringTable>("Assets/Resources/Localization/GameText_en-US.asset");
        Add(table,"armor.defense","Physical Defense: {0} -> {1}\nMagic Defense: {2} -> {3}\nEvasion: {4:0.#}%");
        Add(table,"battle.holy_component"," (+{0} Holy)");Add(table,"scaling.type.holy","Holy");
        Add(table,"scaling.holy_damage","Additional Holy Damage: {0} + {1} = {2}\nApplies to Undead and Unholy foes.");
        string[] names={"Heavy Plate Armor","Knight Armor","Assassin Armor","Mage Robes","Holy Armor","Battlemage Armor"};
        string[] files={"HeavyPlateArmor","KnightArmor","AssassinArmor","MageRobes","HolyArmor","BattlemageArmor"};
        var first=new[]{CharacterAttribute.Endurance,CharacterAttribute.Vitality,CharacterAttribute.Agility,CharacterAttribute.Wisdom,CharacterAttribute.Wisdom,CharacterAttribute.MagicPower};
        var second=new[]{CharacterAttribute.Strength,CharacterAttribute.Endurance,CharacterAttribute.Dexterity,CharacterAttribute.MagicPower,CharacterAttribute.Vitality,CharacterAttribute.Endurance};
        var grade1=new[]{ScalingGrade.A,ScalingGrade.A,ScalingGrade.A,ScalingGrade.A,ScalingGrade.S,ScalingGrade.B};
        var grade2=new[]{ScalingGrade.B,ScalingGrade.B,ScalingGrade.C,ScalingGrade.C,ScalingGrade.D,ScalingGrade.B};
        int[] physical={20,16,8,4,12,10},magic={5,8,5,18,20,14};
        var parent=GameObject.Find("TestLoot");var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WorldItemPickup.prefab");
        for(int i=0;i<files.Length;i++)
        {
            string path="Assets/Settings/Inventory/"+files[i]+".asset";var item=AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if(item!=null && item.id!="armor."+files[i].ToLowerInvariant())throw new InvalidOperationException("Unrelated asset: "+path);
            if(item==null){item=ScriptableObject.CreateInstance<ItemData>();AssetDatabase.CreateAsset(item,path);}
            var entries=new[]{new AttributeScalingEntry{attribute=first[i],grade=grade1[i]},new AttributeScalingEntry{attribute=second[i],grade=grade2[i]}};
            item.id="armor."+files[i].ToLowerInvariant();item.displayName=names[i];item.description="Defensive equipment shaped by your attributes.";
            item.category=ItemCategory.Armor;item.equipmentSlot=EquipmentSlot.Chest;item.stackable=false;item.useArmorScaling=true;
            item.armor=new ArmorScalingData{physicalDefense=physical[i],magicDefense=magic[i],requirements=new CharacterAttributeValues(),
                physicalScaling=i<3?entries:Array.Empty<AttributeScalingEntry>(),magicScaling=i>=3?entries:Array.Empty<AttributeScalingEntry>(),
                evasion=i==2?.06f:0,evasionScaling=i==2?entries:Array.Empty<AttributeScalingEntry>()};
            item.armor.requirements.Set(first[i],i==4?25:15);
            if(item.icon==null)item.icon=AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Settings/Inventory/LeatherArmor.asset").icon;
            Add(table,item.id+".name",item.displayName);Add(table,item.id+".description",item.description);EditorUtility.SetDirty(item);AssetDatabase.SaveAssetIfDirty(item);
            string goName="Pickup_Scaling_"+files[i];var child=parent.transform.Find(goName);var go=child!=null?child.gameObject:(GameObject)PrefabUtility.InstantiatePrefab(prefab,scene);
            if(child==null){go.name=goName;go.transform.SetParent(parent.transform);go.transform.position=new Vector3(-5-i%3,-4-i/3,0);}
            var pickup=go.GetComponent<WorldItemPickup>();pickup.item=item;pickup.worldId="scaling_armor_"+files[i].ToLowerInvariant();pickup.quantity=1;go.GetComponent<SpriteRenderer>().sprite=item.icon;EditorUtility.SetDirty(go);
        }
        var sacred=AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Settings/Inventory/SacredBlade.asset");sacred.weapon.holyBaseDamage=5;EditorUtility.SetDirty(sacred);AssetDatabase.SaveAssetIfDirty(sacred);
        GameObject.Find("Enemy").GetComponent<BattleUnit>().creatureCategories=CreatureCategory.Undead;
        GameObject.Find("Player").GetComponent<BattleUnit>().creatureCategories=CreatureCategory.Human;
        EditorUtility.SetDirty(table);EditorUtility.SetDirty(table.SharedData);AssetDatabase.SaveAssetIfDirty(table.SharedData);AssetDatabase.SaveAssetIfDirty(table);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        if(!UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Scene save failed.");
    }
    private static void Add(StringTable table,string key,string value){var entry=table.GetEntry(key);if(entry==null)table.AddEntry(key,value);else entry.Value=value;}
}

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DetailedAnimationImporter
{
    public const string HeroRoot="Assets/Art/Characters/Player/AnimationV2";
    public const string SkeletonRoot="Assets/Art/Enemies/SkeletonDetailed";
    public const string HeroProfile="Assets/Settings/Battle/Visuals/HeroDetailed.asset";
    public const string SkeletonProfile="Assets/Settings/Battle/Visuals/SkeletonDetailed.asset";
    [Serializable] private sealed class Manifest { public Frame[] frames=Array.Empty<Frame>(); }
    [Serializable] private sealed class Frame
    {
        public string path="";
        public int canvasWidth=0,canvasHeight=0,pivotX=0,pivotY=0,ppu=0;
    }

    [MenuItem("FalseMemory/Art/Import detailed character animations")]
    public static void ImportAll()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before importing animations.");
        var manifest=JsonUtility.FromJson<Manifest>(File.ReadAllText("Assets/Art/AnimationV2Manifest.json"));
        foreach(var frame in manifest.frames)
        {
            AssetDatabase.ImportAsset(frame.path,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(frame.path);
            importer.textureType=TextureImporterType.Sprite;
            importer.spriteImportMode=SpriteImportMode.Single;
            importer.spritePixelsPerUnit=frame.ppu;
            Configure(importer);
            var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);
            settings.spriteMeshType=SpriteMeshType.FullRect;settings.spriteAlignment=(int)SpriteAlignment.Custom;
            settings.spritePivot=new Vector2((float)frame.pivotX/frame.canvasWidth,(float)frame.pivotY/frame.canvasHeight);
            importer.SetTextureSettings(settings);importer.SaveAndReimport();
        }
        foreach(var root in new[]{HeroRoot,SkeletonRoot})foreach(var path in Directory.GetFiles(root+"/Source","*.png"))
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path.Replace('\\','/'));
            importer.textureType=TextureImporterType.Default;Configure(importer);importer.SaveAndReimport();
        }
        SaveProfile(HeroProfile,HeroRoot,"Hero",false);
        SaveProfile(SkeletonProfile,SkeletonRoot,"Skeleton",true);
        // Extend the existing controller's clips, retaining all GUID and direction parameters.
        ApprovedHeroImporter.ImportAnimations();
    }
    private static void Configure(TextureImporter importer)
    {
        importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;
        importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;
        importer.npotScale=TextureImporterNPOTScale.None;importer.alphaSource=TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency=true;importer.wrapMode=TextureWrapMode.Clamp;
    }
    public static Sprite[] Frames(string folder,string prefix)
    {
        return Directory.GetFiles(folder,prefix+"*.png").OrderBy(p=>p,StringComparer.Ordinal)
            .Select(p=>AssetDatabase.LoadAssetAtPath<Sprite>(p.Replace('\\','/'))).ToArray();
    }
    private static void SaveProfile(string path,string root,string name,bool hasDirections)
    {
        var set=AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(path);
        if(set==null){set=ScriptableObject.CreateInstance<SpriteAnimationSet>();AssetDatabase.CreateAsset(set,path);}
        set.idle=Frames(root+"/Battle",name+"-Idle-");set.attack=Frames(root+"/Battle",name+"-Attack-");
        set.hurt=Frames(root+"/Battle",name+"-Hurt-");set.death=Frames(root+"/Battle",name+"-Death-");
        set.framesPerSecond=16;set.idleFramesPerSecond=4;set.attackContactProgress=.5f;
        set.move=Frames(root+"/World",hasDirections?"Skeleton-Move-Right-":"ApprovedHero_Right_");
        if(hasDirections)
        {
            var directions=new[]{Vector2.down,Vector2.up,Vector2.right,Vector2.left};
            var names=new[]{"Down","Up","Right","Left"};
            set.worldDirections=Enumerable.Range(0,4).Select(i=>
            {
                var walk=Frames(root+"/World","Skeleton-Move-"+names[i]+"-");
                return new DirectionalSpriteFrames{direction=directions[i],walk=walk,idle=new[]{walk[2]}};
            }).ToArray();
        }
        EditorUtility.SetDirty(set);AssetDatabase.SaveAssetIfDirty(set);
    }
    public static void WireWorld()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before wiring the scene.");
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(scene.path!="Assets/Scenes/SampleScene.unity")throw new InvalidOperationException("Open SampleScene first.");
        ApprovedHeroImporter.WireActivePlayer();
        var enemy=GameObject.Find("Enemy");var set=AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(SkeletonProfile);
        var unit=enemy.GetComponent<BattleUnit>();unit.visualAnimations=set;unit.battleSprite=set.idle[0];unit.spriteColor=Color.white;
        enemy.GetComponent<SpriteRenderer>().sprite=set.worldDirections[0].idle[0];
        enemy.GetComponent<SpriteRenderer>().color=Color.white;enemy.GetComponent<SpriteRenderer>().flipX=false;
        enemy.GetComponent<EnemySpriteAnimation>().movementHoldDuration=.08f;
        EditorSceneManager.MarkSceneDirty(scene);
        if(!EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Could not save SampleScene.");
    }
    public static void WireBattlePreview()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before wiring the scene.");
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if(scene.path!="Assets/Scenes/BattleScene.unity")throw new InvalidOperationException("Open BattleScene first.");
        var ui=UnityEngine.Object.FindAnyObjectByType<BattleUI>();
        ui.previewPlayerSprite=AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(HeroProfile).idle[0];
        ui.previewEnemySprite=AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(SkeletonProfile).idle[0];
        ui.playerPortrait.sprite=ui.previewPlayerSprite;ui.enemyPortrait.sprite=ui.previewEnemySprite;
        ui.playerPortrait.color=ui.enemyPortrait.color=Color.white;
        // Reserve room for the full blade while keeping both characters inside the existing battlefield.
        ui.playerPortrait.rectTransform.anchorMin=new Vector2(.65f,.46f);
        ui.playerPortrait.rectTransform.anchorMax=new Vector2(.90f,.80f);
        ui.enemyPortrait.rectTransform.anchorMin=new Vector2(.10f,.46f);
        ui.enemyPortrait.rectTransform.anchorMax=new Vector2(.35f,.80f);
        ui.playerPortrait.rectTransform.offsetMin=ui.playerPortrait.rectTransform.offsetMax=Vector2.zero;
        ui.enemyPortrait.rectTransform.offsetMin=ui.enemyPortrait.rectTransform.offsetMax=Vector2.zero;
        EditorSceneManager.MarkSceneDirty(scene);
        if(!EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Could not save BattleScene.");
    }
}

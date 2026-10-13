using System;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEditor.SceneManagement;
using UnityEngine;

// Authoring utility only. Never changes physics, input, stats, or source PNG pixels.
public static class ApprovedHeroImporter
{
    public const string Root = "Assets/Art/Characters/Player/ApprovedHero";
    public const float WalkFramesPerSecond = 12f;
    public static readonly string[] Directions = { "Up", "Down", "Right", "Left", "UpRight", "UpLeft", "DownRight", "DownLeft" };

    [MenuItem("FalseMemory/Art/Import approved hero animation sheets")]
    public static void ImportAnimations()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode before importing hero animations.");
        foreach (string direction in Directions)
        {
            ImportSheet(direction);
            var frames = Frames(direction);
            WriteClip("Walk", direction, frames);
            WriteClip("Idle", direction, new[] { frames[0] });
        }
        foreach (string source in new[] { "ApprovedHeroSource", "DirectionalSource" })
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(Root + "/" + source + ".png");
            importer.textureType = TextureImporterType.Default;
            ConfigureTexture(importer);
            importer.SaveAndReimport();
        }
        ConfigureController();
    }

    private static void ConfigureController()
    {
        var controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/Player.controller");
        if (controller == null) throw new InvalidOperationException("Missing existing player controller.");
        if (!controller.parameters.Any(p => p.name == "WalkSpeed"))
            controller.AddParameter(new AnimatorControllerParameter { name = "WalkSpeed", type = AnimatorControllerParameterType.Float, defaultFloat = 1f });
        var walk = controller.layers[0].stateMachine.states.First(s => s.state.name == "Player_DirectionalWalk").state;
        walk.speedParameter = "WalkSpeed";
        walk.speedParameterActive = true;
        EditorUtility.SetDirty(walk);
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssetIfDirty(controller);
    }

    private static void ImportSheet(string direction)
    {
        string path = Root + "/Walk" + direction + ".png";
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer == null) throw new InvalidOperationException("Missing sheet: " + path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = direction == "Down" ? 1100f : 640f;
        var textureSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(textureSettings);
        textureSettings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(textureSettings);
        ConfigureTexture(importer);
        var factories = new SpriteDataProviderFactories();
        factories.Init();
        var provider = factories.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();
        var previous = provider.GetSpriteRects();
        int width = direction == "Down" ? 512 : 384;
        int height = direction == "Down" ? 640 : 384;
        float feet = direction == "Down" ? 40f : 20f;
        var rects = Enumerable.Range(0, 4).Select(i =>
        {
            string name = "ApprovedHero_" + direction + "_" + i.ToString("00");
            var old = previous.FirstOrDefault(r => r.name == name);
            return new SpriteRect { name = name, rect = new Rect(i * width, 0, width, height),
                alignment = SpriteAlignment.Custom, pivot = new Vector2(.5f, feet / height),
                spriteID = old == null ? GUID.Generate() : old.spriteID };
        }).ToArray();
        provider.SetSpriteRects(rects);
        var names = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        names.SetNameFileIdPairs(rects.Select(r => new SpriteNameFileIdPair(r.name, r.spriteID)));
        provider.Apply();
        importer.SaveAndReimport();
    }

    private static void ConfigureTexture(TextureImporter importer)
    {
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 4096;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;
    }

    public static Sprite[] Frames(string direction)
    {
        const string prepared="Assets/Art/Characters/Player/AnimationV2/World";
        if(System.IO.Directory.Exists(prepared))
            return System.IO.Directory.GetFiles(prepared,"ApprovedHero_"+direction+"_*.png")
                .OrderBy(p=>p,StringComparer.Ordinal).Select(p=>AssetDatabase.LoadAssetAtPath<Sprite>(p.Replace('\\','/'))).ToArray();
        return AssetDatabase.LoadAllAssetsAtPath(Root + "/Walk" + direction + ".png")
            .OfType<Sprite>().OrderBy(s => s.name).ToArray();
    }

    private static void WriteClip(string motion, string direction, Sprite[] frames)
    {
        if (frames.Length == 0) throw new InvalidOperationException("Missing animation frames: " + direction);
        string path = "Assets/Animations/Player64/" + motion + "_" + direction + ".anim";
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (clip == null) throw new InvalidOperationException("Missing existing clip: " + path);
        clip.frameRate = motion=="Walk" ? WalkFramesPerSecond : 8f;
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        var keys = new ObjectReferenceKeyframe[frames.Length + 1];
        for (int i = 0; i < frames.Length; i++)
            keys[i] = new ObjectReferenceKeyframe { time = i / clip.frameRate, value = frames[i] };
        keys[frames.Length] = new ObjectReferenceKeyframe { time = frames.Length / clip.frameRate, value = frames[0] };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.startTime = 0f;
        settings.stopTime = frames.Length / clip.frameRate;
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
        AssetDatabase.SaveAssetIfDirty(clip);
    }

    public static void WireActivePlayer()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode before wiring the hero.");
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/SampleScene.unity") throw new InvalidOperationException("Open the existing SampleScene first.");
        var player = GameObject.Find("Player");
        if (player == null) throw new InvalidOperationException("Existing Player not found.");
        var sprite = Frames("Down").First();
        player.GetComponent<SpriteRenderer>().sprite = sprite;
        player.GetComponent<SpriteRenderer>().color = Color.white;
        var unit = player.GetComponent<BattleUnit>();
        var battleSet=AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>("Assets/Settings/Battle/Visuals/HeroDetailed.asset");
        if(battleSet!=null){unit.visualAnimations=battleSet;unit.battleSprite=battleSet.idle[0];}
        else unit.battleSprite = sprite;
        unit.spriteColor = Color.white;
        var window = player.GetComponent<InventoryWindow>();
        if(window.portraitSprite==null)
            window.portraitSprite=AssetDatabase.LoadAllAssetsAtPath(Root+"/WalkDown.png").OfType<Sprite>().FirstOrDefault(s=>s.name=="ApprovedHero_Down_00");
        if (window.characterPortrait != null)
        {
            window.characterPortrait.sprite = window.portraitSprite!=null?window.portraitSprite:unit.battleSprite;
            window.characterPortrait.color = Color.white;
        }
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Could not save SampleScene.");
    }
}

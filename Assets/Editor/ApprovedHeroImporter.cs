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
    public const float WalkFramesPerSecond = 16f;
    public const string ThirtyFrameRoot = "Assets/Art/Characters/Player/Animation30";
    public const float ThirtyFrameIdleFramesPerSecond = 16f;
    public const string CardinalRoot = "Assets/Art/Characters/Player/Cardinal4";
    public const string CardinalManifest = "Assets/Art/PlayerCardinal4Manifest.json";
    public static readonly string[] CardinalDirections = { "Down", "Left", "Right", "Up" };
    [Serializable] private sealed class PreparedManifest
    {
        public PreparedFrame[] frames = Array.Empty<PreparedFrame>();
        public PreparedWalkSequence[] walkSequences = Array.Empty<PreparedWalkSequence>();
    }
    [Serializable] private sealed class PreparedWalkSequence
    {
        public string direction = "";
        public string[] paths = Array.Empty<string>();
    }
    [Serializable] private sealed class PreparedFrame
    {
        public string path = "";
        public string motion = "", direction = "";
        public int index = -1;
        public int canvasWidth = 0, canvasHeight = 0, pivotX = 0, pivotY = 0, ppu = 0;
    }
    public static readonly string[] Directions = { "Up", "Down", "Right", "Left", "UpRight", "UpLeft", "DownRight", "DownLeft" };

    [MenuItem("FalseMemory/Art/Import approved hero animation sheets")]
    public static void ImportAnimations()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode before importing hero animations.");
        if (System.IO.File.Exists(CardinalManifest))
        {
            ImportPreparedFrames(CardinalManifest, CardinalRoot);
            foreach (string direction in CardinalDirections)
            {
                WriteClip("Idle", direction, CardinalFrames("Idle", direction), 8);
                WriteClip("Walk", direction, CardinalFrames("Walk", direction), 2, false);
            }
            ConfigureController(true);
            return;
        }
        ImportPreparedFrames();
        foreach (string direction in Directions)
        {
            ImportSheet(direction);
            var frames = Frames(direction);
            WriteClip("Walk", direction, frames);
            WriteClip("Idle", direction, IdleFrames(direction));
        }
        foreach (string source in new[] { "ApprovedHeroSource", "DirectionalSource" })
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(Root + "/" + source + ".png");
            importer.textureType = TextureImporterType.Default;
            ConfigureTexture(importer);
            importer.SaveAndReimport();
        }
        RetimeLegacyClips();
        ConfigureController();
    }

    // Preserve backup clips and GUIDs, but use the same 1/16 second frame grid.
    private static void RetimeLegacyClips()
    {
        foreach (string path in new[] { "Assets/Player_Walk.anim", "Assets/Player_Run.anim", "Assets/Player_Idle.anim", "Assets/PlayerMyproject_Idle.anim" })
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null || clip.frameRate <= 0f) throw new InvalidOperationException("Invalid existing hero clip: " + path);
            if (Mathf.Approximately(clip.frameRate, WalkFramesPerSecond)) continue;
            if (AnimationUtility.GetCurveBindings(clip).Length != 0)
                throw new InvalidOperationException("Review transform curves before retiming backup clip: " + path);
            float scale = clip.frameRate / WalkFramesPerSecond;
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                var keys = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                for (int i = 0; i < keys.Length; i++) keys[i].time *= scale;
                AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
            }
            var events = AnimationUtility.GetAnimationEvents(clip);
            foreach (var entry in events) entry.time *= scale;
            AnimationUtility.SetAnimationEvents(clip, events);
            clip.frameRate = WalkFramesPerSecond;
            settings.startTime *= scale; settings.stopTime *= scale;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            EditorUtility.SetDirty(clip);
            AssetDatabase.SaveAssetIfDirty(clip);
        }
    }

    private static void ConfigureController(bool cardinal = false)
    {
        var controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/Player.controller");
        if (controller == null) throw new InvalidOperationException("Missing existing player controller.");
        if (!controller.parameters.Any(p => p.name == "WalkSpeed"))
            controller.AddParameter(new AnimatorControllerParameter { name = "WalkSpeed", type = AnimatorControllerParameterType.Float, defaultFloat = 1f });
        var walk = controller.layers[0].stateMachine.states.First(s => s.state.name == "Player_DirectionalWalk").state;
        walk.speedParameter = "WalkSpeed";
        walk.speedParameterActive = true;
        if (cardinal)
        {
            var positions = new[] { Vector2.down, Vector2.left, Vector2.right, Vector2.up };
            foreach (string motion in new[] { "Idle", "Walk" })
            {
                var state = controller.layers[0].stateMachine.states.First(s => s.state.name == "Player_Directional" + motion).state;
                var tree = state.motion as UnityEditor.Animations.BlendTree;
                if (tree == null) throw new InvalidOperationException("Missing existing directional blend tree.");
                tree.children = CardinalDirections.Select((direction, i) => new UnityEditor.Animations.ChildMotion
                {
                    motion = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Player64/" + motion + "_" + direction + ".anim"),
                    position = positions[i], timeScale = 1f
                }).ToArray();
                EditorUtility.SetDirty(tree);
            }
        }
        EditorUtility.SetDirty(walk);
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssetIfDirty(controller);
    }

    private static void ImportPreparedFrames()
    {
        ImportPreparedFrames("Assets/Art/PlayerAnimation30Manifest.json", ThirtyFrameRoot);
    }

    private static void ImportPreparedFrames(string manifestPath, string assetRoot)
    {
        if (!System.IO.File.Exists(manifestPath)) return;
        var manifest = JsonUtility.FromJson<PreparedManifest>(System.IO.File.ReadAllText(manifestPath));
        foreach (var frame in manifest.frames)
        {
            if (!frame.path.StartsWith(assetRoot + "/", StringComparison.Ordinal) ||
                frame.canvasWidth <= 0 || frame.canvasHeight <= 0 || frame.ppu <= 0 || !System.IO.File.Exists(frame.path))
                throw new InvalidOperationException("Invalid prepared hero frame: " + frame.path);
            AssetDatabase.ImportAsset(frame.path, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(frame.path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = frame.ppu;
            ConfigureTexture(importer);
            var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = new Vector2((float)frame.pivotX / frame.canvasWidth, (float)frame.pivotY / frame.canvasHeight);
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }
        foreach (string path in System.IO.Directory.GetFiles(assetRoot + "/Source", "*.png"))
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
            importer.textureType = TextureImporterType.Default;
            ConfigureTexture(importer);
            importer.SaveAndReimport();
        }
    }

    private static Sprite[] CardinalFrames(string motion, string direction)
    {
        if (motion == "Walk")
        {
            var manifest = JsonUtility.FromJson<PreparedManifest>(System.IO.File.ReadAllText(CardinalManifest));
            var sequence = manifest.walkSequences.Single(s => s.direction == direction);
            if (sequence.paths.Length != 4 || sequence.paths[0] != sequence.paths[3])
                throw new InvalidOperationException("Walk requires stance / right / left / same stance: " + direction);
            var ordered = sequence.paths.Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p)).ToArray();
            if (ordered.Any(s => s == null) || ordered[1] == ordered[2] || ordered[0] == ordered[1] || ordered[0] == ordered[2])
                throw new InvalidOperationException("Missing or duplicate step poses: " + direction);
            return ordered;
        }
        var prepared = JsonUtility.FromJson<PreparedManifest>(System.IO.File.ReadAllText(CardinalManifest)).frames
            .Where(f => f.motion == motion && f.direction == direction).OrderBy(f => f.index).ToArray();
        if (prepared.Length != 4 || !prepared.Select(f => f.index).SequenceEqual(new[] { 0, 1, 2, 3 }))
            throw new InvalidOperationException("Exactly four ordered prepared frames required: " + motion + "/" + direction);
        var frames = prepared.Select(f => AssetDatabase.LoadAssetAtPath<Sprite>(f.path)).ToArray();
        if (frames.Length != 4 || frames.Any(s => s == null))
            throw new InvalidOperationException("Exactly four imported cardinal frames required: " + motion + "/" + direction);
        return frames;
    }

    private static Sprite[] PreparedFrames(string motion, string direction)
    {
        string folder = ThirtyFrameRoot + "/" + motion + "/" + direction;
        if (!System.IO.Directory.Exists(folder)) return Array.Empty<Sprite>();
        var frames = System.IO.Directory.GetFiles(folder, "*.png").OrderBy(p => p, StringComparer.Ordinal)
            .Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p.Replace('\\', '/'))).ToArray();
        if (frames.Length != 30 || frames.Any(s => s == null))
            throw new InvalidOperationException("A prepared motion requires exactly 30 imported sprites: " + folder);
        return frames;
    }

    public static Sprite[] IdleFrames(string direction)
    {
        if (System.IO.File.Exists(CardinalManifest) && CardinalDirections.Contains(direction)) return CardinalFrames("Idle", direction);
        var frames = PreparedFrames("Idle", direction);
        return frames.Length > 0 ? frames : new[] { Frames(direction)[0] };
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
        if (System.IO.File.Exists(CardinalManifest) && CardinalDirections.Contains(direction)) return CardinalFrames("Walk", direction);
        var thirtyFrames = PreparedFrames("Walk", direction);
        if (thirtyFrames.Length > 0) return thirtyFrames;
        const string prepared="Assets/Art/Characters/Player/AnimationV2/World";
        if(System.IO.Directory.Exists(prepared))
            return System.IO.Directory.GetFiles(prepared,"ApprovedHero_"+direction+"_*.png")
                .OrderBy(p=>p,StringComparer.Ordinal).Select(p=>AssetDatabase.LoadAssetAtPath<Sprite>(p.Replace('\\','/'))).ToArray();
        return AssetDatabase.LoadAllAssetsAtPath(Root + "/Walk" + direction + ".png")
            .OfType<Sprite>().OrderBy(s => s.name).ToArray();
    }

    private static void WriteClip(string motion, string direction, Sprite[] frames, int holdSamples = 1, bool appendLoopKey = true)
    {
        if (frames.Length == 0) throw new InvalidOperationException("Missing animation frames: " + direction);
        string path = "Assets/Animations/Player64/" + motion + "_" + direction + ".anim";
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (clip == null) throw new InvalidOperationException("Missing existing clip: " + path);
        clip.frameRate = WalkFramesPerSecond;
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        var keys = new ObjectReferenceKeyframe[frames.Length + (appendLoopKey ? 1 : 0)];
        // Four requested poses: brief stance, right step, left step, brief stance.
        var sampleStarts = appendLoopKey ? null : new[] { 0, 1, 4, 7 };
        for (int i = 0; i < frames.Length; i++)
            keys[i] = new ObjectReferenceKeyframe { time = (sampleStarts == null ? i * holdSamples : sampleStarts[i]) / clip.frameRate, value = frames[i] };
        if (appendLoopKey) keys[frames.Length] = new ObjectReferenceKeyframe { time = frames.Length * holdSamples / clip.frameRate, value = frames[0] };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.startTime = 0f;
        settings.stopTime = frames.Length * holdSamples / clip.frameRate;
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
        var sprite = IdleFrames("Down").First();
        player.GetComponent<SpriteRenderer>().sprite = sprite;
        player.GetComponent<SpriteRenderer>().color = Color.white;
        if (System.IO.File.Exists(CardinalManifest))
        {
            var control = new SerializedObject(player.GetComponent<PlayerController>());
            control.FindProperty("useCardinalAnimations").boolValue = true;
            control.ApplyModifiedPropertiesWithoutUndo();
        }
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

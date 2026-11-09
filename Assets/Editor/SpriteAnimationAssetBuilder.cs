using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

// Скрипт ТІЛЬКИ для редактора: збирає .anim кліпи та Animator-контролери
// з готових кадрів у Skeleton64 і FireVisual.
// Запускається з меню FalseMemory -> Animation. У грі він не працює.
public static class SpriteAnimationAssetBuilder
{
    // Корінь для всіх згенерованих анімацій
    public const string AnimationRoot = "Assets/Animations";
    public const string SkeletonWorldRoot = AnimationRoot + "/Enemies/Skeleton/World";
    public const string SkeletonPortraitRoot = AnimationRoot + "/Enemies/Skeleton/Portrait";
    public const string VfxAnimationRoot = AnimationRoot + "/VFX";

    // Звідки беремо кадри
    public const string SkeletonArtRoot = "Assets/Art/Enemies/Skeleton64";
    public const string FireArtRoot = "Assets/Art/VFX/Fire64";
    public const string SkeletonSetPath = "Assets/Settings/Battle/Visuals/Skeleton64.asset";
    public const string FireProfilePath = "Assets/Settings/Battle/VFX/FireVisual.asset";

    // Швидкість покадрової анімації
    public const float FramesPerSecond = 8f;

    // Імена станів у контролерах збігаються з SpritePose
    public const string VfxStateName = "Effect";

    // Перебудувати всі кліпи й контролери
    [MenuItem("FalseMemory/Animation/Rebuild sprite animations")]
    public static void BuildAll()
    {
        if (EditorApplication.isPlaying)
        {
            throw new System.InvalidOperationException("Stop Play Mode before building animations.");
        }

        SpriteAnimationSet set = AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>(SkeletonSetPath);
        if (set == null)
        {
            throw new System.InvalidOperationException("Sprite set not found: " + SkeletonSetPath);
        }

        BattleVFXProfile fire = AssetDatabase.LoadAssetAtPath<BattleVFXProfile>(FireProfilePath);
        if (fire == null)
        {
            throw new System.InvalidOperationException("VFX profile not found: " + FireProfilePath);
        }

        BuildSkeletonWorld(set);
        BuildSkeletonPortrait(set);
        BuildFire(fire);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Sprite animations rebuilt.");
    }

    // Службовий пункт для batch-режиму: анімації і модель боса за один запуск.
    // Готовий PNG боса не перезаписується.
    [MenuItem("FalseMemory/Animation/Rebuild sprite animations and boss art")]
    public static void BuildAllWithBoss()
    {
        BuildAll();

        string bossPath = PixelBattleAssetBuilder.BossRoot + "/BossSkeleton-Idle-00.png";
        if (!File.Exists(bossPath))
        {
            PixelBattleAssetBuilder.GenerateBossSkeleton();
        }
    }

    // Кліпи для скелета у світі (малює SpriteRenderer)
    private static void BuildSkeletonWorld(SpriteAnimationSet set)
    {
        AnimationClip move = CreateClip(
            SkeletonWorldRoot + "/Skeleton_Move.anim",
            LoadFrames(SkeletonArtRoot, "Skeleton-Move", 4),
            true,
            typeof(SpriteRenderer));

        AnimationClip hurt = CreateClip(
            SkeletonWorldRoot + "/Skeleton_Hurt.anim",
            LoadFrames(SkeletonArtRoot, "Skeleton-Hurt", 2),
            false,
            typeof(SpriteRenderer));

        AnimationClip death = CreateClip(
            SkeletonWorldRoot + "/Skeleton_Death.anim",
            LoadFrames(SkeletonArtRoot, "Skeleton-Death", 4),
            false,
            typeof(SpriteRenderer));

        string[] names = { "Move", "Hurt", "Death" };
        AnimationClip[] clips = { move, hurt, death };
        AnimatorController controller = CreateController(
            SkeletonWorldRoot + "/SkeletonWorld.controller",
            names,
            clips);

        SetReference(set, "worldController", controller);
    }

    // Кліпи для бойового портрета (малює UI Image)
    private static void BuildSkeletonPortrait(SpriteAnimationSet set)
    {
        AnimationClip attack = CreateClip(
            SkeletonPortraitRoot + "/Skeleton_Portrait_Attack.anim",
            LoadFrames(SkeletonArtRoot, "Skeleton-Attack", 3),
            false,
            typeof(Image));

        AnimationClip hurt = CreateClip(
            SkeletonPortraitRoot + "/Skeleton_Portrait_Hurt.anim",
            LoadFrames(SkeletonArtRoot, "Skeleton-Hurt", 2),
            false,
            typeof(Image));

        AnimationClip death = CreateClip(
            SkeletonPortraitRoot + "/Skeleton_Portrait_Death.anim",
            LoadFrames(SkeletonArtRoot, "Skeleton-Death", 4),
            false,
            typeof(Image));

        string[] names = { "Attack", "Hurt", "Death" };
        AnimationClip[] clips = { attack, hurt, death };
        AnimatorController controller = CreateController(
            SkeletonPortraitRoot + "/SkeletonPortrait.controller",
            names,
            clips);

        SetReference(set, "portraitController", controller);
    }

    // Кліп ефекту вогню
    private static void BuildFire(BattleVFXProfile fire)
    {
        Sprite[] frames = LoadFrames(FireArtRoot, "Fire", 8);

        AnimationClip clip = CreateClip(
            VfxAnimationRoot + "/Fire_Cast.anim",
            frames,
            false,
            typeof(Image));

        string[] names = { VfxStateName };
        AnimationClip[] clips = { clip };
        AnimatorController controller = CreateController(
            VfxAnimationRoot + "/FireVFX.controller",
            names,
            clips);

        SetReference(fire, "effectController", controller);

        SerializedObject serialized = new SerializedObject(fire);
        SerializedProperty frameCount = serialized.FindProperty("frameCount");
        frameCount.intValue = frames.Length;
        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(fire);
    }

    // Завантажити кадри з папки за іменем виду: Prefix-00.png, Prefix-01.png ...
    private static Sprite[] LoadFrames(string folder, string prefix, int count)
    {
        Sprite[] frames = new Sprite[count];

        for (int i = 0; i < count; i++)
        {
            string path = folder + "/" + prefix + "-" + i.ToString("00") + ".png";
            frames[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (frames[i] == null)
            {
                throw new System.InvalidOperationException("Sprite not found: " + path);
            }
        }

        return frames;
    }

    // Створити кліп, який міняє спрайт у компоненті targetType
    private static AnimationClip CreateClip(
        string path,
        Sprite[] frames,
        bool loop,
        System.Type targetType)
    {
        if (frames == null || frames.Length == 0)
        {
            throw new System.InvalidOperationException("No frames for clip: " + path);
        }

        EnsureFolder(path);
        DeleteIfExists(path);

        ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            ObjectReferenceKeyframe key = new ObjectReferenceKeyframe();
            key.time = i / FramesPerSecond;
            key.value = frames[i];
            keys[i] = key;
        }

        // Порожній шлях означає "компонент на цьому ж GameObject"
        EditorCurveBinding binding = EditorCurveBinding.PPtrCurve("", targetType, "m_Sprite");

        AnimationClip clip = new AnimationClip();
        clip.frameRate = FramesPerSecond;
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.startTime = 0f;
        settings.stopTime = frames.Length / FramesPerSecond;
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    // Створити контролер зі станами clips
    private static AnimatorController CreateController(
        string path,
        string[] stateNames,
        AnimationClip[] clips)
    {
        EnsureFolder(path);
        DeleteIfExists(path);

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        AnimatorStateMachine machine = controller.layers[0].stateMachine;

        for (int i = 0; i < stateNames.Length; i++)
        {
            AnimatorState state = machine.AddState(stateNames[i]);
            state.motion = clips[i];
            if (i == 0)
            {
                machine.defaultState = state;
            }
        }

        EditorUtility.SetDirty(controller);
        return controller;
    }

    // Записати посилання у ScriptableObject
    private static void SetReference(
        ScriptableObject asset,
        string fieldName,
        Object value)
    {
        SerializedObject serialized = new SerializedObject(asset);
        SerializedProperty property = serialized.FindProperty(fieldName);
        if (property == null)
        {
            throw new System.InvalidOperationException(
                "Field not found: " + fieldName + " on " + asset.name);
        }

        property.objectReferenceValue = value;
        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);
    }

    // Видалити старий згенерований ассет, щоб створити його заново
    private static void DeleteIfExists(string path)
    {
        if (AssetDatabase.LoadAssetAtPath<Object>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }
    }

    // Створити папки на шляху до файлу
    private static void EnsureFolder(string path)
    {
        string folder = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(folder))
        {
            return;
        }

        if (AssetDatabase.IsValidFolder(folder))
        {
            return;
        }

        string[] parts = folder.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }
}

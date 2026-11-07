using NUnit.Framework;
using UnityEditor;
using UnityEngine;

// Тести піксель-арту: розміри, налаштування імпорту, повнота кадрів
public sealed class PixelArtTests
{
    // Усі 32 кадри - рівно 64x64, Point-фільтр, без стиснення і без міпмапів
    [Test]
    public void EveryNewFrameIsNative64PointUncompressedAndWithoutMipmaps()
    {
        string[] folders = new string[] { "Assets/Art/Enemies/Skeleton64", "Assets/Art/VFX" };
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", folders);
        Assert.That(guids.Length, Is.EqualTo(32));

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);

            // path передаємо в повідомлення, щоб при помилці було видно, який файл
            Assert.That(sprite, Is.Not.Null, path);
            Assert.That(sprite.texture.width, Is.EqualTo(64), path);
            Assert.That(sprite.texture.height, Is.EqualTo(64), path);
            Assert.That(sprite.rect.size, Is.EqualTo(new Vector2(64, 64)), path);
            Assert.That(sprite.pixelsPerUnit, Is.EqualTo(100), path);
            Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point), path);
            Assert.That(importer.mipmapEnabled, Is.False, path);
            Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed), path);
            Assert.That(importer.spriteImportMode, Is.EqualTo(SpriteImportMode.Single), path);
        }
    }

    // У скелета немає анімації "стоїть", а у вогню є всі 8 кадрів
    [Test]
    public void SkeletonHasNoIdlePoseAndFireHasCompleteFrames()
    {
        SpriteAnimationSet set = AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>("Assets/Settings/Battle/Visuals/Skeleton64.asset");
        Assert.That(set, Is.Not.Null);

        foreach (SpritePose pose in System.Enum.GetValues(typeof(SpritePose)))
        {
            Sprite[] frames = set.Frames(pose);
            Assert.That(frames, Is.Not.Empty, pose.ToString());
            foreach (Sprite frame in frames)
            {
                Assert.That(frame, Is.Not.Null);
            }
        }

        Assert.That(System.Enum.GetNames(typeof(SpritePose)), Does.Not.Contain("Idle"));

        BattleVFXProfile fire = AssetDatabase.LoadAssetAtPath<BattleVFXProfile>("Assets/Settings/Battle/VFX/FireVisual.asset");
        Assert.That(fire, Is.Not.Null);
        Assert.That(fire.frames.Length, Is.EqualTo(8));
        foreach (Sprite frame in fire.frames)
        {
            Assert.That(frame, Is.Not.Null);
        }
        Assert.That(fire.duration, Is.InRange(0.1f, 1f));
    }

    // At() не виходить за межі масиву, а Loop() працює навіть з порожніми кадрами
    [Test]
    public void FiniteFramesClampAndLoopIsStableWithMissingFrames()
    {
        SpriteAnimationSet set = AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>("Assets/Settings/Battle/Visuals/Skeleton64.asset");

        Assert.That(set.At(SpritePose.Death, -1), Is.SameAs(set.death[0]));
        Assert.That(set.At(SpritePose.Death, 2), Is.SameAs(set.death[3]));
        Assert.That(set.Loop(SpritePose.Move, 0.125f), Is.SameAs(set.move[1]));

        // Копія, щоб не зіпсувати справжній ассет
        SpriteAnimationSet copy = Object.Instantiate(set);
        try
        {
            copy.attack = null;
            Assert.That(copy.At(SpritePose.Attack, 0.5f, set.death[0]), Is.SameAs(set.death[0]));

            copy.move = new Sprite[] { null };
            Assert.That(copy.Loop(SpritePose.Move, 99, set.death[0]), Is.SameAs(set.death[0]));
        }
        finally
        {
            Object.DestroyImmediate(copy);
        }
    }
}

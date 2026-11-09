using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

// Тести піксель-арту: розміри, налаштування імпорту, повнота анімацій
public sealed class PixelArtTests
{
    // Усі 25 кадрів - рівно 64x64, Point-фільтр, без стиснення і без міпмапів
    [Test]
    public void EveryNewFrameIsNative64PointUncompressedAndWithoutMipmaps()
    {
        string[] folders = new string[]
        {
            "Assets/Art/Enemies/Skeleton64",
            "Assets/Art/Enemies/BossSkeleton64",
            "Assets/Art/VFX"
        };
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", folders);
        Assert.That(guids.Length, Is.EqualTo(25));

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
        Assert.That(System.Enum.GetNames(typeof(SpritePose)), Does.Not.Contain("Idle"));

        // Контролери зібрані з кадрів: світ і бойовий портрет
        Assert.That(set.worldController, Is.Not.Null);
        Assert.That(set.portraitController, Is.Not.Null);
        AssertStateExists(set.worldController, "Move");
        AssertStateExists(set.worldController, "Hurt");
        AssertStateExists(set.worldController, "Death");
        AssertStateExists(set.portraitController, "Attack");
        AssertStateExists(set.portraitController, "Hurt");
        AssertStateExists(set.portraitController, "Death");

        BattleVFXProfile fire = AssetDatabase.LoadAssetAtPath<BattleVFXProfile>("Assets/Settings/Battle/VFX/FireVisual.asset");
        Assert.That(fire, Is.Not.Null);
        Assert.That(fire.frameCount, Is.EqualTo(8));
        Assert.That(fire.effectController, Is.Not.Null);
        AssertStateExists(fire.effectController, "Effect");
        Assert.That(fire.duration, Is.InRange(0.1f, 1f));
    }

    // Перевірити, що в контролері є стан з потрібним ім'ям
    private static void AssertStateExists(RuntimeAnimatorController controller, string stateName)
    {
        AnimatorController animatorController = controller as AnimatorController;
        Assert.That(animatorController, Is.Not.Null, stateName);

        AnimatorStateMachine machine = animatorController.layers[0].stateMachine;
        ChildAnimatorState[] states = machine.states;

        bool found = false;
        foreach (ChildAnimatorState entry in states)
        {
            if (entry.state != null && entry.state.name == stateName)
            {
                found = true;
            }
        }

        Assert.That(found, Is.True, stateName);
    }
}

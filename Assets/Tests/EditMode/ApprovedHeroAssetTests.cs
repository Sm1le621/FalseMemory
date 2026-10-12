using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public sealed class ApprovedHeroAssetTests
{
    private const string Root = "Assets/Art/Characters/Player/ApprovedHero/";
    private static readonly string[] Directions = { "Up", "Down", "Right", "Left", "UpRight", "UpLeft", "DownRight", "DownLeft" };

    [Test]
    public void SheetsKeepNativeResolutionAndConsistentGroundPivots()
    {
        foreach (string direction in Directions)
        {
            string path = Root + "Walk" + direction + ".png";
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            var frames = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
            Assert.That(frames.Length, Is.EqualTo(4), direction);
            Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point));
            Assert.That(importer.mipmapEnabled, Is.False);
            Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed));
            Assert.That(importer.npotScale, Is.EqualTo(TextureImporterNPOTScale.None));
            int width = direction == "Down" ? 512 : 384;
            int height = direction == "Down" ? 640 : 384;
            foreach (var sprite in frames)
            {
                Assert.That(sprite.rect.size, Is.EqualTo(new Vector2(width, height)));
                Assert.That(sprite.texture.width, Is.EqualTo(width * 4));
                Assert.That(sprite.texture.height, Is.EqualTo(height));
                Assert.That(sprite.pivot, Is.EqualTo(new Vector2(width / 2f, direction == "Down" ? 40 : 20)));
                Assert.That(sprite.pixelsPerUnit, Is.EqualTo(direction == "Down" ? 1100 : 640));
            }
        }
    }

    [Test]
    public void WalkClipsUseFourFramesForTheirDirectionWithoutTransformAnimation()
    {
        foreach (string direction in Directions)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Player64/Walk_" + direction + ".anim");
            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            Assert.That(bindings.Length, Is.EqualTo(1));
            Assert.That(bindings[0].type, Is.EqualTo(typeof(SpriteRenderer)));
            var keys = AnimationUtility.GetObjectReferenceCurve(clip, bindings[0]);
            Assert.That(keys.Length, Is.EqualTo(5));
            Assert.That(keys.Take(4).Select(k => k.value).Distinct().Count(), Is.EqualTo(4));
            Assert.That(keys.Take(4).All(k => k.value.name.StartsWith("ApprovedHero_" + direction + "_")), Is.True);
            Assert.That(keys[4].value, Is.SameAs(keys[0].value));
            Assert.That(keys[4].time, Is.EqualTo(.5f));
            Assert.That(AnimationUtility.GetCurveBindings(clip), Is.Empty);
            Assert.That(AnimationUtility.GetAnimationClipSettings(clip).loopTime, Is.True);
        }
    }
}

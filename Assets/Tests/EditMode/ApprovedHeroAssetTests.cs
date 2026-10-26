using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public sealed class ApprovedHeroAssetTests
{
    private const string Root = "Assets/Art/Characters/Player/ApprovedHero/";
    private static readonly string[] Directions = { "Up", "Down", "Right", "Left", "UpRight", "UpLeft", "DownRight", "DownLeft" };

    [Test]
    public void AllHeroClipsIncludingBackupsUseTheSixteenFpsTimeGrid()
    {
        var paths=Directions.SelectMany(d=>new[]{"Assets/Animations/Player64/Walk_"+d+".anim","Assets/Animations/Player64/Idle_"+d+".anim"})
            .Concat(new[]{"Assets/Player_Walk.anim","Assets/Player_Run.anim","Assets/Player_Idle.anim","Assets/PlayerMyproject_Idle.anim"}).ToArray();
        Assert.That(paths.Length,Is.EqualTo(20));
        foreach(string path in paths)
        {
            var clip=AssetDatabase.LoadAssetAtPath<AnimationClip>(path);Assert.That(clip,Is.Not.Null,path);
            Assert.That(clip.frameRate,Is.EqualTo(16f),path);
            foreach(var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            foreach(var key in AnimationUtility.GetObjectReferenceCurve(clip,binding))
                Assert.That(key.time*16f,Is.EqualTo(Mathf.Round(key.time*16f)).Within(.0001f),path+" must retime keys, not only metadata.");
            float ticks=AnimationUtility.GetAnimationClipSettings(clip).stopTime*16f;
            Assert.That(ticks,Is.EqualTo(Mathf.Round(ticks)).Within(.0001f),path);
        }
    }

    [Test]
    public void CardinalIdleKeepsFourFramesAndWalkUsesStanceRightLeftStance()
    {
        const string root="Assets/Art/Characters/Player/Cardinal4/";
        var active=new System.Collections.Generic.HashSet<Object>();
        foreach(string direction in new[]{"Down","Left","Right","Up"})
        {
            var idle=AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Player64/Idle_"+direction+".anim");
            var idleKeys=AnimationUtility.GetObjectReferenceCurve(idle,AnimationUtility.GetObjectReferenceCurveBindings(idle).Single());
            Assert.That(idleKeys.Length,Is.EqualTo(5));Assert.That(idleKeys.Take(4).Select(k=>k.value).Distinct().Count(),Is.EqualTo(4));
            Assert.That(idleKeys[4].value,Is.SameAs(idleKeys[0].value));
            Assert.That(AnimationUtility.GetAnimationClipSettings(idle).stopTime,Is.EqualTo(2f).Within(.0001f));
            var walk=AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Player64/Walk_"+direction+".anim");
            var walkKeys=AnimationUtility.GetObjectReferenceCurve(walk,AnimationUtility.GetObjectReferenceCurveBindings(walk).Single());
            Assert.That(walkKeys.Length,Is.EqualTo(4),"Exactly four keys, no extra fifth closing pose.");
            Assert.That(walkKeys[0].value,Is.SameAs(idleKeys[0].value));Assert.That(walkKeys[3].value,Is.SameAs(idleKeys[0].value));
            Assert.That(walkKeys[1].value.name,Is.EqualTo("ApprovedHero_"+direction+"_RightStep"));
            Assert.That(walkKeys[2].value.name,Is.EqualTo("ApprovedHero_"+direction+"_LeftStep"));
            Assert.That(walkKeys.Select(k=>k.value).Distinct().Count(),Is.EqualTo(3),"Repeated neutral is explicitly requested.");
            var times=new[]{0f,1f/16f,4f/16f,7f/16f};
            for(int i=0;i<4;i++)Assert.That(walkKeys[i].time,Is.EqualTo(times[i]).Within(.0001f));
            Assert.That(AnimationUtility.GetAnimationClipSettings(walk).stopTime,Is.EqualTo(.5f).Within(.0001f));
            float ppu=((Sprite)idleKeys[0].value).pixelsPerUnit;
            foreach(var key in idleKeys.Take(4).Concat(walkKeys))
            {
                active.Add(key.value);var sprite=(Sprite)key.value;string path=AssetDatabase.GetAssetPath(sprite);
                Assert.That(path,Does.StartWith(root));
                Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(512,512)));Assert.That(sprite.pivot,Is.EqualTo(new Vector2(256,20)));
                Assert.That(sprite.pixelsPerUnit,Is.EqualTo(ppu));
                var importer=(TextureImporter)AssetImporter.GetAtPath(path);
                Assert.That(importer.filterMode,Is.EqualTo(FilterMode.Point));Assert.That(importer.mipmapEnabled,Is.False);
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
            using(var sha=System.Security.Cryptography.SHA256.Create())
            {
                var hashes=walkKeys.Select(k=>System.Convert.ToBase64String(sha.ComputeHash(System.IO.File.ReadAllBytes(AssetDatabase.GetAssetPath(k.value))))).ToArray();
                Assert.That(hashes[0],Is.EqualTo(hashes[3]));Assert.That(hashes.Distinct().Count(),Is.EqualTo(3));
            }
        }
        Assert.That(active.Count,Is.EqualTo(24),"16 Idle + 8 step sprites, neutral shared by the Walk.");
    }

    [Test]
    public void ActiveControllerHasOnlyFourCardinalChildrenPerMotion()
    {
        var controller=AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/Player.controller");
        foreach(string motion in new[]{"Idle","Walk"})
        {
            var state=controller.layers[0].stateMachine.states.Single(s=>s.state.name=="Player_Directional"+motion).state;
            var tree=(UnityEditor.Animations.BlendTree)state.motion;Assert.That(tree.children.Length,Is.EqualTo(4));
            foreach(var child in tree.children)
            {
                Assert.That(Mathf.Abs(child.position.x)+Mathf.Abs(child.position.y),Is.EqualTo(1f));
                Assert.That(child.position.x==0 || child.position.y==0,Is.True,"No diagonal motion/flicker.");
                Assert.That(child.timeScale,Is.EqualTo(1f));
            }
        }
    }

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
    public void WalkClipsHaveOnlyFourOrderedKeysWithoutTransformAnimation()
    {
        foreach(string direction in new[]{"Down","Left","Right","Up"})
        {
            var clip=AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Player64/Walk_"+direction+".anim");
            var bindings=AnimationUtility.GetObjectReferenceCurveBindings(clip);
            Assert.That(bindings.Length,Is.EqualTo(1));Assert.That(bindings[0].type,Is.EqualTo(typeof(SpriteRenderer)));
            Assert.That(AnimationUtility.GetObjectReferenceCurve(clip,bindings[0]).Length,Is.EqualTo(4));
            Assert.That(clip.frameRate,Is.EqualTo(16f));Assert.That(clip.length,Is.EqualTo(.5f).Within(.0001f));
            Assert.That(AnimationUtility.GetCurveBindings(clip),Is.Empty);
            Assert.That(AnimationUtility.GetAnimationClipSettings(clip).loopTime,Is.True);
        }
    }
}

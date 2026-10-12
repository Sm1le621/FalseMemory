using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public sealed class PixelArtTests
{
    [Test] public void EveryNewFrameIsNative64PointUncompressedAndWithoutMipmaps()
    {
        var guids=AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/Art/Enemies/Skeleton64","Assets/Art/VFX"});
        Assert.That(guids.Length,Is.EqualTo(32));
        foreach(var guid in guids)
        {
            string path=AssetDatabase.GUIDToAssetPath(guid);var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);Assert.That(sprite,Is.Not.Null,path);
            Assert.That(sprite.texture.width,Is.EqualTo(64),path);Assert.That(sprite.texture.height,Is.EqualTo(64),path);
            Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(64,64)),path);Assert.That(sprite.pixelsPerUnit,Is.EqualTo(100),path);
            Assert.That(importer.filterMode,Is.EqualTo(FilterMode.Point),path);Assert.That(importer.mipmapEnabled,Is.False,path);
            Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed),path);
            Assert.That(importer.spriteImportMode,Is.EqualTo(SpriteImportMode.Single),path);
        }
    }
    [Test] public void SkeletonAndSpellsHaveCompleteAnimationReferences()
    {
        var set=AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>("Assets/Settings/Battle/Visuals/Skeleton64.asset");Assert.That(set,Is.Not.Null);
        foreach(SpritePose pose in System.Enum.GetValues(typeof(SpritePose)))
        {var frames=set.Frames(pose);Assert.That(frames,Is.Not.Empty,pose.ToString());foreach(var frame in frames)Assert.That(frame,Is.Not.Null);}
        foreach(var name in new[]{"Fire","Heal"})
        {var profile=AssetDatabase.LoadAssetAtPath<BattleVFXProfile>("Assets/Settings/Battle/VFX/"+name+"Visual.asset");Assert.That(profile.frames.Length,Is.EqualTo(8));foreach(var frame in profile.frames)Assert.That(frame,Is.Not.Null);Assert.That(profile.duration,Is.InRange(0.1f,1f));}
    }
    [Test] public void FiniteFramesClampAndLoopIsStableWithMissingFrames()
    {
        var set=AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>("Assets/Settings/Battle/Visuals/Skeleton64.asset");
        Assert.That(set.At(SpritePose.Death,-1),Is.SameAs(set.death[0]));Assert.That(set.At(SpritePose.Death,2),Is.SameAs(set.death[3]));
        Assert.That(set.Loop(SpritePose.Idle,0.5f),Is.SameAs(set.idle[1]));Assert.That(set.Loop(SpritePose.Move,0.125f),Is.SameAs(set.move[1]));
        var copy=Object.Instantiate(set);
        try{copy.attack=null;Assert.That(copy.At(SpritePose.Attack,0.5f,set.idle[0]),Is.SameAs(set.idle[0]));copy.move=new Sprite[]{null};Assert.That(copy.Loop(SpritePose.Move,99,set.idle[0]),Is.SameAs(set.idle[0]));}
        finally{Object.DestroyImmediate(copy);}
    }
}

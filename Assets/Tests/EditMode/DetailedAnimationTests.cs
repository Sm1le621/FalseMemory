using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public sealed class DetailedAnimationTests
{
    private static SpriteAnimationSet Profile(string name)=>AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>("Assets/Settings/Battle/Visuals/"+name+"Detailed.asset");
    [Test]
    public void BattleProfilesHaveDifferentIdleAttackFramesAndSharedCanvasWithinEachActor()
    {
        foreach(var name in new[]{"Hero","Skeleton"})
        {
            var set=Profile(name);Assert.That(set,Is.Not.Null);
            Assert.That(set.idle.Length,Is.EqualTo(4));Assert.That(set.attack.Length,Is.EqualTo(8));
            Assert.That(set.idle.Distinct().Count(),Is.EqualTo(4));Assert.That(set.attack.Distinct().Count(),Is.EqualTo(8));
            foreach(var sprite in set.idle.Concat(set.attack).Concat(set.hurt).Concat(set.death))
            {
                Assert.That(sprite,Is.Not.Null);
                Assert.That(sprite.rect.size,Is.EqualTo(set.idle[0].rect.size));
                Assert.That(sprite.pivot,Is.EqualTo(set.idle[0].pivot));
                Assert.That(sprite.pixelsPerUnit,Is.EqualTo(set.idle[0].pixelsPerUnit));
                var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite));
                Assert.That(importer.filterMode,Is.EqualTo(FilterMode.Point));Assert.That(importer.mipmapEnabled,Is.False);
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
        }
        Assert.That(Profile("Skeleton").hurt.Length,Is.EqualTo(2));Assert.That(Profile("Skeleton").death.Length,Is.EqualTo(4));
    }
    [Test]
    public void AttackContactSeparatesForwardWindupAndRecoveryWithoutResourceWrites()
    {
        var set=Profile("Hero");
        Assert.That(set.AttackAt(0,false),Is.SameAs(set.attack[0]));
        Assert.That(set.AttackAt(1,false),Is.SameAs(set.attack[4]));
        Assert.That(set.AttackAt(0,true),Is.SameAs(set.attack[4]));
        Assert.That(set.AttackAt(.5f,true),Is.SameAs(set.attack[6]));
        Assert.That(set.AttackAt(1,true),Is.SameAs(set.attack[7]));
        var old=AssetDatabase.LoadAssetAtPath<SpriteAnimationSet>("Assets/Settings/Battle/Visuals/Skeleton64.asset");
        Assert.That(old.HasWorldDirections,Is.False);Assert.That(old.LoopWorld(Vector2.up,true,0),Is.SameAs(old.move[0]));
    }
    [Test]
    public void WorldDirectionsSelectFourActualViewsAndKeepIdleWhenStopped()
    {
        var set=Profile("Skeleton");Assert.That(set.HasWorldDirections,Is.True);Assert.That(set.worldDirections.Length,Is.EqualTo(4));
        foreach(var entry in set.worldDirections)
        {
            Assert.That(entry.walk.Length,Is.EqualTo(8));Assert.That(entry.walk.Distinct().Count(),Is.EqualTo(8));
            Assert.That(set.LoopWorld(entry.direction,true,0),Is.SameAs(entry.walk[0]));
            Assert.That(set.LoopWorld(entry.direction,false,123),Is.SameAs(entry.idle[0]));
            Assert.That(entry.walk.All(s=>s.pivot==entry.walk[0].pivot && s.rect.size==entry.walk[0].rect.size && s.pixelsPerUnit==entry.walk[0].pixelsPerUnit),Is.True);
        }
    }
}

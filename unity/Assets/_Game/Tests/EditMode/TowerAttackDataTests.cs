using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the tower prefabs, not code: each one carries its sheet's attack row, wired to its own
    /// renderer and idle loop, with the blow on frame 2 (Build Sprites sets this).
    /// </summary>
    public class TowerAttackDataTests
    {
        [TestCase("Ant", "TowerSoldier")]
        [TestCase("BulletAnt", "TowerLinear")]
        [TestCase("CrazyAnt", "TowerKnockback")]
        [TestCase("FireAnt", "TowerMortar")]
        [TestCase("SapAnt", "TowerFrostAura")]
        [TestCase("CarpenterAnt", "TowerMineLayer")]
        public void EachTower_PlaysItsOwnAttackRow(string tower, string sheet)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Game/Features/Towers/Prefabs/Tower_{tower}.prefab");
            Assert.That(prefab, Is.Not.Null);

            var attack = prefab.GetComponent<TowerAttackAnimation>();
            Assert.That(attack, Is.Not.Null, $"Tower_{tower} has no attack animation.");

            var so = new SerializedObject(attack);
            Assert.That(so.FindProperty("_renderer").objectReferenceValue, Is.SameAs(prefab.GetComponent<SpriteRenderer>()));
            Assert.That(so.FindProperty("_idle").objectReferenceValue, Is.SameAs(prefab.GetComponent<SpriteFlipbook>()));
            Assert.That(so.FindProperty("_hitFrame").intValue, Is.EqualTo(2));

            SerializedProperty frames = so.FindProperty("_frames");
            Assert.That(frames.arraySize, Is.EqualTo(6));
            for (int i = 0; i < frames.arraySize; i++)
            {
                Object frame = frames.GetArrayElementAtIndex(i).objectReferenceValue;
                Assert.That(frame, Is.Not.Null);
                Assert.That(frame.name, Is.EqualTo($"{sheet} Attack {i}"));
            }
        }
    }
}

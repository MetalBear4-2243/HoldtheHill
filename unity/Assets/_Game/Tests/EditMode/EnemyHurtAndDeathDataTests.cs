using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Enemies;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the enemy prefabs, not code: each one plays its own sheet's Hurt and Death rows on its
    /// Body, holding its walk (Build Sprites sets this). The pill bug's sheet has neither.
    /// </summary>
    public class EnemyHurtAndDeathDataTests
    {
        private const string Folder = "Assets/_Game/Features/Enemies/Prefabs";

        [TestCase("Enemy_Basic", "EnemyGrunt")]
        [TestCase("Enemy_Scarab", "EnemyRunner")]
        [TestCase("Enemy_MantisSpawn", "EnemyHealer")]
        [TestCase("Enemy_RhinoBeetle", "EnemyShielded")]
        [TestCase("Enemy_Wasp", "EnemyWasp")]
        [TestCase("Enemy_Silverfish", "EnemySilverfish")]
        [TestCase("Enemy_Centipede", "EnemySplitter")]
        public void EachEnemy_FlinchesAndDiesWithItsOwnArt(string prefabName, string sheet)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{Folder}/{prefabName}.prefab");
            Assert.That(prefab, Is.Not.Null);

            var hurtAndDeath = prefab.GetComponent<EnemyHurtAndDeath>();
            Assert.That(hurtAndDeath, Is.Not.Null, $"{prefabName} has no hurt and death animation.");

            var so = new SerializedObject(hurtAndDeath);
            var body = so.FindProperty("_body").objectReferenceValue as SpriteRenderer;
            Assert.That(body, Is.Not.Null);
            Assert.That(body.name, Is.EqualTo("Body"));
            Assert.That(so.FindProperty("_walk").objectReferenceValue, Is.SameAs(body.GetComponent<SpriteFlipbook>()));

            AssertRow(so.FindProperty("_hurtFrames"), sheet, "Hurt", 2);
            AssertRow(so.FindProperty("_deathFrames"), sheet, "Death", 7);
        }

        [Test]
        public void TheRollyPolly_HasNoRowsToPlay_AndIsLeftAsItWas()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{Folder}/Enemy_RollyPolly.prefab");
            Assert.That(prefab, Is.Not.Null);
            Assert.That(prefab.GetComponent<EnemyHurtAndDeath>(), Is.Null);
        }

        private static void AssertRow(SerializedProperty frames, string sheet, string row, int count)
        {
            Assert.That(frames.arraySize, Is.EqualTo(count), $"{sheet} {row} should have {count} frames.");
            for (int i = 0; i < frames.arraySize; i++)
            {
                Object frame = frames.GetArrayElementAtIndex(i).objectReferenceValue;
                Assert.That(frame, Is.Not.Null);
                Assert.That(frame.name, Is.EqualTo($"{sheet} {row} {i}"));
            }
        }
    }
}

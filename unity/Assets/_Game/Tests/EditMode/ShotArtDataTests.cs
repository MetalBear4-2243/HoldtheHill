using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the shot and mine prefabs, not code: each loops its own sheet's row, a shot turns to
    /// face its flight, and each leaves an effect that plays its row once (Build Sprites sets this).
    /// </summary>
    public class ShotArtDataTests
    {
        private const string Folder = "Assets/_Game/Features/Towers/Prefabs";

        [TestCase("Projectile_Bullet", "ProjLinear", "Fly", 4, "_hitEffect", "Fx_BulletHit", "FxImpact", "Acid", 5)]
        [TestCase("Projectile_Lob", "ProjMortar", "Fly", 4, "_hitEffect", "Fx_LobBlast", "FxBlast", "Blast", 6)]
        [TestCase("Mine_Carpenter", "FxMine", "Armed", 2, "_explosionEffect", "Fx_MineBlast", "FxMine", "Explode", 7)]
        public void EachShot_LoopsItsArt_AndLeavesItsEffect(string prefabName, string sheet, string row, int count,
            string effectField, string effectName, string effectSheet, string effectRow, int effectCount)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{Folder}/{prefabName}.prefab");
            Assert.That(prefab, Is.Not.Null);

            var renderer = prefab.GetComponent<SpriteRenderer>();
            Assert.That(renderer, Is.Not.Null);
            Assert.That(renderer.color, Is.EqualTo(Color.white), "The art shows in its own colours.");

            var flipbook = prefab.GetComponent<SpriteFlipbook>();
            Assert.That(flipbook, Is.Not.Null, $"{prefabName} has no looping art.");
            var loop = new SerializedObject(flipbook);
            Assert.That(loop.FindProperty("_renderer").objectReferenceValue, Is.SameAs(renderer));
            AssertRow(loop.FindProperty("_frames"), sheet, row, count);

            bool isShot = prefab.GetComponent<Projectile>() != null;
            var face = prefab.GetComponent<FaceTravel>();
            if (isShot)
            {
                Assert.That(face, Is.Not.Null, $"{prefabName} does not turn to face its flight.");
                Assert.That(new SerializedObject(face).FindProperty("_body").objectReferenceValue, Is.SameAs(prefab.transform));
            }
            else
            {
                Assert.That(face, Is.Null, "A mine lies still.");
            }

            MonoBehaviour owner = isShot ? prefab.GetComponent<Projectile>() : prefab.GetComponent<Mine>();
            var effect = new SerializedObject(owner).FindProperty(effectField).objectReferenceValue as GameObject;
            Assert.That(effect, Is.Not.Null, $"{prefabName} leaves no effect.");
            Assert.That(effect.name, Is.EqualTo(effectName));

            var oneShot = effect.GetComponent<SpriteOneShot>();
            Assert.That(oneShot, Is.Not.Null, $"{effectName} does not play once and go.");
            AssertRow(new SerializedObject(oneShot).FindProperty("_frames"), effectSheet, effectRow, effectCount);
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

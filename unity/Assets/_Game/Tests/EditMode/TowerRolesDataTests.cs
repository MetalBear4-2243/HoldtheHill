using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the tower prefabs, not code: each of the four reworked towers has its new role and art
    /// (Build Tower Roles and Build Sprites set this).
    /// </summary>
    public class TowerRolesDataTests
    {
        private const string Folder = "Assets/_Game/Features/Towers/Prefabs";

        private static GameObject Load(string name)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{Folder}/{name}.prefab");
            Assert.That(prefab, Is.Not.Null, $"{name} is missing.");
            return prefab;
        }

        private static Object ShotOf(Tower tower) => new SerializedObject(tower).FindProperty("_projectilePrefab").objectReferenceValue;

        [Test]
        public void TheBulletAnt_FiresAFastBullet()
        {
            var bullet = Load("Projectile_Bullet").GetComponent<Projectile>();
            var lob = Load("Projectile_Lob").GetComponent<Projectile>();
            Assert.That(ShotOf(Load("Tower_BulletAnt").GetComponent<Tower>()), Is.SameAs(bullet));
            Assert.IsFalse(bullet.IsLobbed);
            Assert.That(new SerializedObject(bullet).FindProperty("_speed").floatValue, Is.GreaterThanOrEqualTo(20f));
            Assert.That(new SerializedObject(bullet).FindProperty("_speed").floatValue,
                Is.GreaterThan(new SerializedObject(lob).FindProperty("_speed").floatValue * 3f));
        }

        [Test]
        public void TheSapAnt_LobsASlowGlob_ThatLeavesAStickyPuddle()
        {
            GameObject ant = Load("Tower_SapAnt");
            Assert.That(ant.GetComponent<SlowAura>(), Is.Null, "the aura is gone");

            var sap = Load("Projectile_Sap").GetComponent<Projectile>();
            Assert.That(ShotOf(ant.GetComponent<Tower>()), Is.SameAs(sap));
            Assert.IsTrue(sap.IsLobbed, "a lob, so it lands on the road and leaves wasps alone");
            Assert.That(new SerializedObject(sap).FindProperty("_speed").floatValue,
                Is.LessThan(new SerializedObject(Load("Projectile_Lob").GetComponent<Projectile>()).FindProperty("_speed").floatValue));

            var left = new SerializedObject(sap).FindProperty("_hitEffect").objectReferenceValue as GameObject;
            Assert.That(left, Is.Not.Null);
            var puddle = left.GetComponent<SapPuddle>();
            Assert.That(puddle, Is.Not.Null, "it leaves a sap puddle");
            Assert.That(puddle.SlowMultiplier, Is.LessThan(1f));

            var so = new SerializedObject(puddle);
            AssertRow(so.FindProperty("_splatFrames"), "WpnHoneyGlob", "Hit", 5);
            AssertRow(so.FindProperty("_overlayFrames"), "FxStatusOverlay", "Honey", 4);
            AssertRow(new SerializedObject(sap.GetComponent<SpriteFlipbook>()).FindProperty("_frames"), "WpnHoneyGlob", "Fly", 4);
        }

        [Test]
        public void TheFireAnt_IsAShortFlamethrower()
        {
            GameObject ant = Load("Tower_FireAnt");
            var tower = ant.GetComponent<Tower>();
            Assert.That(ShotOf(tower), Is.Null, "no shot: it burns what is in front of it");
            Assert.That(tower.ConeDegrees, Is.GreaterThan(0f).And.LessThan(180f));
            Assert.That(tower.Range, Is.LessThan(Load("Tower_BulletAnt").GetComponent<Tower>().Range));
            Assert.That(tower.FireInterval, Is.LessThanOrEqualTo(0.25f), "many small hits");

            var jet = ant.GetComponent<FlameJet>();
            Assert.That(jet, Is.Not.Null);
            var puff = new SerializedObject(jet).FindProperty("_puffPrefab").objectReferenceValue as FlamePuff;
            Assert.That(puff, Is.Not.Null);
            AssertRow(new SerializedObject(puff.GetComponent<SpriteFlipbook>()).FindProperty("_frames"), "WpnFlameBurst", "Fly", 4);
        }

        [Test]
        public void TheCrazyAnt_HitsFasterAndWeakerThanTheAnt_AndPoisons()
        {
            var crazy = Load("Tower_CrazyAnt").GetComponent<Tower>();
            var ant = Load("Tower_Ant").GetComponent<Tower>();
            Assert.That(ShotOf(crazy), Is.Null);
            Assert.That(crazy.FireInterval, Is.LessThan(ant.FireInterval));
            Assert.That(crazy.Damage, Is.LessThan(ant.Damage));

            var poison = crazy.GetComponent<PoisonOnHits>();
            Assert.That(poison, Is.Not.Null);
            AssertRow(new SerializedObject(poison).FindProperty("_overlayFrames"), "FxStatusOverlay", "Poison", 4);
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

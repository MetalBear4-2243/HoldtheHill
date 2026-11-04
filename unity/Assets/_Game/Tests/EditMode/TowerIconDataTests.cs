using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the tower data, not code: each tower's shop card (Adam's TowerList) shows the same
    /// picture as the tower standing on the map, not the placeholder square.
    /// </summary>
    public class TowerIconDataTests
    {
        private const string DataFolder = "Assets/_Game/Features/Towers/Data";
        private const string PrefabFolder = "Assets/_Game/Features/Towers/Prefabs";

        [TestCase("Ant")]
        [TestCase("BulletAnt")]
        [TestCase("CrazyAnt")]
        [TestCase("FireAnt")]
        [TestCase("SapAnt")]
        [TestCase("CarpenterAnt")]
        public void TheShopIcon_IsTheTowersOwnArt(string tower)
        {
            var data = AssetDatabase.LoadAssetAtPath<TowerData>($"{DataFolder}/{tower}.asset");
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/Tower_{tower}.prefab");
            Assert.That(data, Is.Not.Null, $"{tower}.asset is missing.");
            Assert.That(prefab, Is.Not.Null, $"Tower_{tower}.prefab is missing.");

            var renderer = prefab.GetComponentInChildren<SpriteRenderer>();
            Assert.That(renderer, Is.Not.Null);
            Assert.That(data.towerIcon, Is.Not.Null, $"{tower} has no shop icon.");
            Assert.That(data.towerIcon, Is.SameAs(renderer.sprite), $"{tower}'s shop icon is not the art the tower shows on the map.");
            Assert.That(AssetDatabase.GetAssetPath(data.towerIcon), Does.Contain("/Towers/Art/Sheets/"));
        }
    }
}

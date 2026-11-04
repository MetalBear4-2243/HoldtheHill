using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks Adam's TowerList prefab, not code: its shop cards are the game's six towers, in the
    /// same order as the number keys, and none of the sandbox test towers that arm nothing.
    /// </summary>
    public class TowerListDataTests
    {
        private const string TowerListPrefab = "Assets/_Game/Features/UI/TowerListPrefab/TowerList.prefab";

        private static readonly string[] KeyOrder =
        {
            "Assets/_Game/Features/Towers/Data/Ant.asset",
            "Assets/_Game/Features/Towers/Data/BulletAnt.asset",
            "Assets/_Game/Features/Towers/Data/CrazyAnt.asset",
            "Assets/_Game/Features/Towers/Data/FireAnt.asset",
            "Assets/_Game/Features/Towers/Data/SapAnt.asset",
            "Assets/_Game/Features/Towers/Data/CarpenterAnt.asset",
        };

        [Test]
        public void TheShopListsTheSixTowers_InNumberKeyOrder()
        {
            var shop = AssetDatabase.LoadAssetAtPath<TowerShopManager>(TowerListPrefab);
            Assert.That(shop, Is.Not.Null, "TowerList.prefab should carry a TowerShopManager.");
            Assert.That(shop.availableTowers.Count, Is.EqualTo(KeyOrder.Length));

            for (int i = 0; i < KeyOrder.Length; i++)
            {
                TowerData data = shop.availableTowers[i];
                Assert.That(data, Is.Not.Null, $"Card {i + 1} has no tower.");
                Assert.That(AssetDatabase.GetAssetPath(data), Is.EqualTo(KeyOrder[i]), $"Card {i + 1} is the wrong tower.");
            }
        }
    }
}

using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the tower definitions' numbers, not code: a melee tower placed on the outer diagonal of a
    /// road corner must still reach the road. The road's centre line runs through the corner cell's
    /// centre, one cell across and one up from that tower: the square root of 2 away.
    /// </summary>
    public class TowerRangeDataTests
    {
        private const string TowerPrefabFolder = "Assets/_Game/Features/Towers/Prefabs";
        private const float CellSize = 1f;

        [Test]
        public void MeleeTowers_ReachTheRoadFromACornersOuterDiagonal_AtEveryLevel()
        {
            float diagonal = Mathf.Sqrt(2f) * CellSize;
            int meleeTowers = 0;

            foreach (string guid in AssetDatabase.FindAssets("t:Prefab Tower_", new[] { TowerPrefabFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var tower = AssetDatabase.LoadAssetAtPath<Tower>(path);
                if (tower == null || tower.GetComponent<ITowerAbility>() != null)
                {
                    continue;
                }

                var serialized = new SerializedObject(tower);
                if (serialized.FindProperty("_projectilePrefab").objectReferenceValue != null)
                {
                    continue;
                }

                meleeTowers++;
                Assert.That(tower.Definition, Is.Not.Null, $"{path} has no definition.");
                for (int level = 1; level <= tower.Definition.LevelCount; level++)
                {
                    float range = tower.Definition.LevelAt(level).Range;
                    Assert.That(range, Is.GreaterThan(diagonal), $"{path} level {level}: a range of {range} misses the road at a corner.");
                }
            }

            Assert.That(meleeTowers, Is.GreaterThanOrEqualTo(2), "The Ant and the Crazy Ant should both be checked.");
        }
    }
}

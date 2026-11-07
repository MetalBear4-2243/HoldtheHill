using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the tower data assets, prefabs and level scenes, not code: every tower's numbers are
    /// its definition's level table (the numbers the prefabs carried on 2026-11-06), the prefab and
    /// the definition point at each other, the prefab carries no numbers of its own any more, and
    /// each level's placer offers the six definitions in number-key order (Migrate Tuning Into Data).
    /// </summary>
    public class TowerDataTests
    {
        private const string DataFolder = "Assets/_Game/Features/Towers/Data";
        private const string PrefabFolder = "Assets/_Game/Features/Towers/Prefabs";

        private static readonly string[] KeyOrder = { "Ant", "BulletAnt", "CrazyAnt", "FireAnt", "SapAnt", "CarpenterAnt" };

        private static readonly string[] GameplayScenes =
        {
            "Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity",
            "Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity",
            "Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity",
        };

        private static TowerData Data(string tower)
        {
            var data = AssetDatabase.LoadAssetAtPath<TowerData>($"{DataFolder}/{tower}.asset");
            Assert.That(data, Is.Not.Null, $"{tower}.asset is missing.");
            return data;
        }

        private static Tower Prefab(string tower)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/Tower_{tower}.prefab");
            Assert.That(prefab, Is.Not.Null, $"Tower_{tower}.prefab is missing.");
            return prefab.GetComponent<Tower>();
        }

        // Build price, then per level: cost, range, damage, interval.
        [TestCase("Ant", 75, 1.5f, 8f, 0.8f, 45, 1.6f, 12f, 0.7f, 70, 1.7f, 18f, 0.6f)]
        [TestCase("BulletAnt", 100, 3f, 12f, 1.2f, 60, 3.3f, 18f, 1.1f, 90, 3.6f, 26f, 1f)]
        [TestCase("CrazyAnt", 125, 1.5f, 4f, 0.35f, 75, 1.6f, 6f, 0.3f, 110, 1.7f, 9f, 0.25f)]
        [TestCase("FireAnt", 200, 2f, 3f, 0.2f, 120, 2.2f, 4.5f, 0.2f, 180, 2.4f, 6f, 0.2f)]
        [TestCase("SapAnt", 225, 2f, 0f, 2f, 135, 2.4f, 0f, 1.8f, 200, 2.8f, 0f, 1.6f)]
        [TestCase("CarpenterAnt", 250, 2.5f, 30f, 3f, 150, 2.8f, 45f, 2.5f, 225, 3.1f, 65f, 2f)]
        public void EachTower_HasItsThreeLevels_WithTheNumbersThePrefabHad(string tower,
            int cost1, float range1, float damage1, float interval1,
            int cost2, float range2, float damage2, float interval2,
            int cost3, float range3, float damage3, float interval3)
        {
            TowerData data = Data(tower);
            Assert.That(data.LevelCount, Is.EqualTo(3));
            Assert.That(data.honeydewCost, Is.EqualTo(cost1), "the build price is level 1's cost");

            AssertLevel(data.LevelAt(1), cost1, range1, damage1, interval1);
            AssertLevel(data.LevelAt(2), cost2, range2, damage2, interval2);
            AssertLevel(data.LevelAt(3), cost3, range3, damage3, interval3);
        }

        private static void AssertLevel(TowerLevel level, int cost, float range, float damage, float interval)
        {
            Assert.That(level.Cost, Is.EqualTo(cost));
            Assert.That(level.Range, Is.EqualTo(range).Within(0.0001f));
            Assert.That(level.Damage, Is.EqualTo(damage).Within(0.0001f));
            Assert.That(level.AttackInterval, Is.EqualTo(interval).Within(0.0001f));
        }

        [Test]
        public void TheBulletAnt_Is10DpsFor100Honeydew()
        {
            TowerData data = Data("BulletAnt");

            Assert.That(data.honeydewCost, Is.EqualTo(100));
            Assert.That(data.LevelAt(1).SingleTargetDps, Is.EqualTo(10f).Within(0.0001f));
        }

        [TestCaseSource(nameof(KeyOrder))]
        public void EachDefinition_AndItsPrefab_PointAtEachOther(string tower)
        {
            TowerData data = Data(tower);
            Tower prefab = Prefab(tower);

            Assert.That(data.Prefab, Is.SameAs(prefab), $"{tower}.asset does not name Tower_{tower}.");
            Assert.That(prefab.Definition, Is.SameAs(data), $"Tower_{tower} does not point back at {tower}.asset.");
        }

        [TestCaseSource(nameof(KeyOrder))]
        public void EachPrefab_ReadsItsNumbersFromTheDefinition_AndCarriesNoneOfItsOwn(string tower)
        {
            TowerData data = Data(tower);
            Tower prefab = Prefab(tower);
            TowerLevel first = data.LevelAt(1);

            Assert.That(prefab.Range, Is.EqualTo(first.Range).Within(0.0001f));
            Assert.That(prefab.Damage, Is.EqualTo(first.Damage).Within(0.0001f));
            Assert.That(prefab.FireInterval, Is.EqualTo(first.AttackInterval).Within(0.0001f));
            Assert.That(prefab.ConeDegrees, Is.EqualTo(first.Ability.ConeDegrees).Within(0.0001f));
            Assert.That(prefab.MaxLevel, Is.EqualTo(3));
            Assert.That(prefab.NextUpgradeCost, Is.EqualTo(data.LevelAt(2).Cost));

            var so = new SerializedObject(prefab);
            foreach (string gone in new[] { "_range", "_damage", "_fireInterval", "_coneDegrees", "_upgrades" })
            {
                Assert.That(so.FindProperty(gone), Is.Null, $"Tower_{tower} still carries {gone}; the definition is the one place for it.");
            }
        }

        [Test]
        public void TheFireAnt_HasItsConeOnEveryLevel()
        {
            TowerData data = Data("FireAnt");
            for (int level = 1; level <= data.LevelCount; level++)
            {
                Assert.That(data.LevelAt(level).Ability.ConeDegrees, Is.EqualTo(70f).Within(0.0001f), $"level {level}");
            }
        }

        [Test]
        public void TheSapAnt_HasItsPuddleOnEveryLevel()
        {
            TowerData data = Data("SapAnt");
            for (int level = 1; level <= data.LevelCount; level++)
            {
                TowerAbilitySettings ability = data.LevelAt(level).Ability;
                Assert.That(ability.AreaRadius, Is.EqualTo(0.35f).Within(0.0001f), $"level {level} reach");
                Assert.That(ability.SlowMultiplier, Is.EqualTo(0.5f).Within(0.0001f), $"level {level} slow");
                Assert.That(ability.SlowSeconds, Is.EqualTo(4f).Within(0.0001f), $"level {level} seconds");
            }

            var puddle = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/Fx_SapPuddle.prefab").GetComponent<SapPuddle>();
            Assert.That(puddle, Is.InstanceOf<ITowerEffect>(), "the puddle takes the Sap Ant's level");
        }

        [Test]
        public void TheCarpenterAnt_HasItsMinesOnEveryLevel()
        {
            TowerData data = Data("CarpenterAnt");
            for (int level = 1; level <= data.LevelCount; level++)
            {
                TowerAbilitySettings ability = data.LevelAt(level).Ability;
                Assert.That(ability.MaxMines, Is.EqualTo(3), $"level {level} mines");
                Assert.That(ability.AreaRadius, Is.EqualTo(0.9f).Within(0.0001f), $"level {level} blast");
            }

            var layer = Prefab("CarpenterAnt").GetComponent<MineLayer>();
            Assert.That(layer, Is.Not.Null);
            Assert.That(layer.MaxMines, Is.EqualTo(3));
            Assert.That(new SerializedObject(layer).FindProperty("_maxMines"), Is.Null, "the cap moved to the definition");
        }

        [Test]
        public void TheCrazyAnt_HasItsPoisonOnEveryLevel()
        {
            TowerData data = Data("CrazyAnt");
            for (int level = 1; level <= data.LevelCount; level++)
            {
                TowerAbilitySettings ability = data.LevelAt(level).Ability;
                Assert.That(ability.PoisonMaxStacks, Is.EqualTo(5), $"level {level} stacks");
                Assert.That(ability.PoisonDamageShare, Is.EqualTo(0.25f).Within(0.0001f), $"level {level} share");
            }

            var poison = Prefab("CrazyAnt").GetComponent<PoisonOnHits>();
            var so = new SerializedObject(poison);
            Assert.That(so.FindProperty("_maxStacks"), Is.Null);
            Assert.That(so.FindProperty("_damagePerStackShare"), Is.Null);
            Assert.That(so.FindProperty("_hitsPerStack").intValue, Is.EqualTo(2), "how hits count stays on the component");
        }

        [Test]
        public void TheAntAndTheBulletAnt_HaveNoAbility()
        {
            foreach (string tower in new[] { "Ant", "BulletAnt" })
            {
                TowerData data = Data(tower);
                for (int level = 1; level <= data.LevelCount; level++)
                {
                    Assert.That(data.LevelAt(level).Ability, Is.EqualTo(default(TowerAbilitySettings)), $"{tower} level {level}");
                }
            }
        }

        [Test]
        public void LevelAt_ClampsToTheTable()
        {
            TowerData data = Data("Ant");

            Assert.That(data.LevelAt(0).Cost, Is.EqualTo(data.LevelAt(1).Cost));
            Assert.That(data.LevelAt(9).Cost, Is.EqualTo(data.LevelAt(3).Cost));
        }

        [TestCaseSource(nameof(GameplayScenes))]
        public void EachLevelsPlacer_OffersTheSixDefinitions_InKeyOrder_AndPairsNothingItself(string scenePath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            try
            {
                TowerPlacer placer = null;
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    placer = root.GetComponentInChildren<TowerPlacer>(true);
                    if (placer != null)
                    {
                        break;
                    }
                }

                Assert.That(placer, Is.Not.Null, $"{scene.name} has no Tower Placer.");
                Assert.That(placer.Offers.Count, Is.EqualTo(KeyOrder.Length));
                for (int i = 0; i < KeyOrder.Length; i++)
                {
                    Assert.That(placer.Offers[i], Is.SameAs(Data(KeyOrder[i])), $"{scene.name} key {i + 1} is the wrong tower.");
                }

                Assert.That(new SerializedObject(placer).FindProperty("_offers"), Is.Null, "the per-scene (data, prefab) pairs are gone");
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}

using System.Collections.Generic;
using HoldTheHill.EditorTools;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// The Balance Table window's checks: what they flag on made-up data, and that the real data
    /// assets pass them.
    /// </summary>
    public class BalanceValidationTests
    {
        private const string TowerDataFolder = "Assets/_Game/Features/Towers/Data";
        private const string EnemyDataFolder = "Assets/_Game/Features/Enemies/Data/Enemies";
        private const string EnemyPrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";

        private readonly List<Object> _made = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < _made.Count; i++)
            {
                Object.DestroyImmediate(_made[i]);
            }
            _made.Clear();
        }

        private TowerData MakeTower(params TowerLevel[] levels)
        {
            var data = ScriptableObject.CreateInstance<TowerData>();
            data.name = "Test Tower";
            _made.Add(data);
            var so = new SerializedObject(data);
            SerializedProperty table = so.FindProperty("_levels");
            table.arraySize = levels.Length;
            for (int i = 0; i < levels.Length; i++)
            {
                SerializedProperty element = table.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("Cost").intValue = levels[i].Cost;
                element.FindPropertyRelative("Damage").floatValue = levels[i].Damage;
                element.FindPropertyRelative("AttackInterval").floatValue = levels[i].AttackInterval;
                element.FindPropertyRelative("Range").floatValue = levels[i].Range;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            return data;
        }

        private static TowerLevel Level(int cost, float range, float damage, float interval)
        {
            return new TowerLevel { Cost = cost, Range = range, Damage = damage, AttackInterval = interval };
        }

        private static readonly BalanceValidation.TowerShape Melee = new BalanceValidation.TowerShape { PrefabPointsBack = true };
        private static readonly BalanceValidation.TowerShape Shooter = new BalanceValidation.TowerShape { HasProjectile = true, PrefabPointsBack = true };

        [Test]
        public void ATower_WithoutAPrefab_IsFlagged()
        {
            TowerData data = MakeTower(Level(75, 1.5f, 8f, 0.8f));

            List<string> problems = BalanceValidation.CheckTower(data, Melee);

            Assert.That(problems, Has.Exactly(1).Contains("no prefab"));
        }

        [Test]
        public void AFreeBuild_AndAFreeUpgrade_AreFlagged()
        {
            TowerData data = MakeTower(Level(0, 2f, 8f, 0.8f), Level(0, 2f, 12f, 0.8f));

            List<string> problems = BalanceValidation.CheckTower(data, Shooter);

            Assert.That(problems, Has.Some.Contains("build price"));
            Assert.That(problems, Has.Some.Contains("level 2: the upgrade is free"));
        }

        [Test]
        public void AMeleeRange_ThatMissesTheRoadFromACorner_IsFlagged()
        {
            TowerData data = MakeTower(Level(75, 1.4f, 8f, 0.8f));

            List<string> problems = BalanceValidation.CheckTower(data, Melee);

            Assert.That(problems, Has.Some.Contains("misses the road"));
            Assert.That(BalanceValidation.CheckTower(MakeTower(Level(75, 1.5f, 8f, 0.8f)), Melee), Has.None.Contains("misses the road"));
            Assert.That(BalanceValidation.CheckTower(MakeTower(Level(75, 1.4f, 8f, 0.8f)), Shooter), Has.None.Contains("misses the road"), "a shooter reaches past the corner");
        }

        [Test]
        public void AnIntervalUnderTheMinimum_IsFlagged()
        {
            TowerData data = MakeTower(Level(75, 2f, 8f, 0.01f));

            Assert.That(BalanceValidation.CheckTower(data, Shooter), Has.Some.Contains("attack interval is under"));
        }

        [Test]
        public void AnUpgrade_ThatLowersDps_IsFlagged()
        {
            TowerData data = MakeTower(Level(100, 3f, 12f, 1.2f), Level(60, 3.3f, 10f, 1.2f));

            Assert.That(BalanceValidation.CheckTower(data, Shooter), Has.Some.Contains("DPS"));
        }

        [Test]
        public void ASoundShooter_PassesClean()
        {
            TowerData data = MakeTower(Level(100, 3f, 12f, 1.2f), Level(60, 3.3f, 18f, 1.1f), Level(90, 3.6f, 26f, 1f));

            CollectionAssert.IsEmpty(BalanceValidation.CheckTower(data, Shooter));
        }

        [Test]
        public void AnEnemy_WithAShieldComponentButNoCapacity_IsFlagged()
        {
            var data = ScriptableObject.CreateInstance<EnemyData>();
            _made.Add(data);
            data.name = "Test Beetle";

            List<string> problems = BalanceValidation.CheckEnemy(data, new BalanceValidation.EnemyShape { HasShield = true });

            Assert.That(problems, Has.Exactly(1).Contains("shield capacity of 0"));
        }

        [Test]
        public void AnEnemy_ThatNeverArrives_IsFlagged()
        {
            var data = ScriptableObject.CreateInstance<EnemyData>();
            _made.Add(data);
            data.MoveSpeed = 0f;
            data.MaxHealth = 0.0;

            List<string> problems = BalanceValidation.CheckEnemy(data, default);

            Assert.That(problems, Has.Some.Contains("speed is 0"));
            Assert.That(problems, Has.Some.Contains("health is 0"));
        }

        [Test]
        public void AWaveSource_WithALookupListBesideTheAssignedAsset_IsFlagged()
        {
            var waves = ScriptableObject.CreateInstance<MapWaveDataSO>();
            _made.Add(waves);

            CollectionAssert.IsEmpty(BalanceValidation.CheckWaveSource("L01", waves, 0, 0));
            Assert.That(BalanceValidation.CheckWaveSource("L01", waves, 2, 0), Has.Exactly(1).Contains("lookup"));
            Assert.That(BalanceValidation.CheckWaveSource("L01", waves, 0, 1), Has.Exactly(1).Contains("inline"));
            Assert.That(BalanceValidation.CheckWaveSource("L01", null, 0, 0), Has.Exactly(1).Contains("no wave asset"));
        }

        [Test]
        public void AWave_ThatSendsNothing_OrPointsAtNothing_IsFlagged()
        {
            var waves = ScriptableObject.CreateInstance<MapWaveDataSO>();
            _made.Add(waves);
            waves.waves.Add(new Wave { waveName = "Empty" });
            waves.waves.Add(new Wave { waveName = "Blank", groups = { new EnemySpawnGroup { count = 2 } } });

            List<string> problems = BalanceValidation.CheckWaves(waves);

            Assert.That(problems, Has.Some.Contains("Empty: sends nothing"));
            Assert.That(problems, Has.Some.Contains("Blank row 1: no enemy"));
        }

        [Test]
        public void TheRealTowers_PassClean()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:TowerData", new[] { TowerDataFolder }))
            {
                var data = AssetDatabase.LoadAssetAtPath<TowerData>(AssetDatabase.GUIDToAssetPath(guid));
                Tower prefab = data.Prefab;
                var shape = new BalanceValidation.TowerShape
                {
                    HasProjectile = prefab != null && new SerializedObject(prefab).FindProperty("_projectilePrefab").objectReferenceValue != null,
                    HasAbility = prefab != null && prefab.GetComponent<ITowerAbility>() != null,
                    PrefabPointsBack = prefab != null && prefab.Definition == data,
                };

                CollectionAssert.IsEmpty(BalanceValidation.CheckTower(data, shape), data.name);
            }
        }

        [Test]
        public void TheRealEnemies_PassClean()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:EnemyData", new[] { EnemyDataFolder }))
            {
                var data = AssetDatabase.LoadAssetAtPath<EnemyData>(AssetDatabase.GUIDToAssetPath(guid));
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{EnemyPrefabFolder}/Enemy_{data.name}.prefab");
                Assert.That(prefab, Is.Not.Null, $"Enemy_{data.name}.prefab is missing.");
                var shape = new BalanceValidation.EnemyShape
                {
                    HasShield = prefab.GetComponent<RhinoBeetleShield>() != null,
                    HasCurl = prefab.GetComponent<RollyPollyCurler>() != null,
                    HasSplit = prefab.GetComponent<CentipedeSplitter>() != null,
                };

                CollectionAssert.IsEmpty(BalanceValidation.CheckEnemy(data, shape), data.name);
            }
        }

        [TestCase("Assets/_Game/Features/Enemies/Data/L01_Meadow_Waves.asset")]
        [TestCase("Assets/_Game/Features/Enemies/Data/L00_TutorialHill_Waves.asset")]
        [TestCase("Assets/_Game/Features/Enemies/Data/Hill01_Waves.asset")]
        public void TheRealWaves_PassClean(string path)
        {
            CollectionAssert.IsEmpty(BalanceValidation.CheckWaves(AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(path)));
        }
    }
}

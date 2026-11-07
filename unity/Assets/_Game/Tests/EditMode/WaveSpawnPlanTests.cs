using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Grouped wave rows: a row of N unrolls into N single entries with the row's interval between
    /// them and its delay after the last; single entries follow; folding entries back into rows is
    /// exact. Then the three level wave lists, as migrated: grouped rows only, the same enemies
    /// as the old per-entry lists; Jason's template keeps its entries.
    /// </summary>
    public class WaveSpawnPlanTests
    {
        private GameObject _basic;
        private GameObject _scarab;

        [SetUp]
        public void SetUp()
        {
            _basic = new GameObject("Basic");
            _scarab = new GameObject("Scarab");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_basic);
            Object.DestroyImmediate(_scarab);
        }

        private static EnemySpawnGroup Row(GameObject prefab, int count, float interval, float after)
        {
            return new EnemySpawnGroup { enemyPrefab = prefab, count = count, spawnInterval = interval, delayAfter = after };
        }

        [Test]
        public void ARowOfFive_IsFiveEntries_IntervalBetween_DelayAfterTheLast()
        {
            var wave = new Wave { groups = { Row(_basic, 5, 1.2f, 3f) } };

            List<EnemySpawnEntry> entries = WaveSpawnPlan.Expand(wave);

            Assert.That(entries.Count, Is.EqualTo(5));
            for (int i = 0; i < 4; i++)
            {
                Assert.That(entries[i].enemyPrefab, Is.SameAs(_basic));
                Assert.That(entries[i].delayBeforeNext, Is.EqualTo(1.2f));
            }
            Assert.That(entries[4].delayBeforeNext, Is.EqualTo(3f));
            Assert.That(WaveSpawnPlan.Count(wave), Is.EqualTo(5));
            Assert.That(WaveSpawnPlan.SpawnSeconds(wave), Is.EqualTo(4 * 1.2f + 3f).Within(0.0001f));
        }

        [Test]
        public void RowsComeFirst_ThenTheSingleEntries()
        {
            var wave = new Wave
            {
                groups = { Row(_basic, 2, 1f, 2f), Row(_scarab, 1, 0.5f, 0.5f) },
                enemies = { new EnemySpawnEntry { enemyPrefab = _basic, delayBeforeNext = 9f } },
            };

            List<EnemySpawnEntry> entries = WaveSpawnPlan.Expand(wave);

            Assert.That(entries.Count, Is.EqualTo(4));
            Assert.That(entries[0].enemyPrefab, Is.SameAs(_basic));
            Assert.That(entries[1].enemyPrefab, Is.SameAs(_basic));
            Assert.That(entries[1].delayBeforeNext, Is.EqualTo(2f));
            Assert.That(entries[2].enemyPrefab, Is.SameAs(_scarab));
            Assert.That(entries[3].delayBeforeNext, Is.EqualTo(9f));
        }

        [Test]
        public void ACountUnderOne_StillSendsOne()
        {
            var wave = new Wave { groups = { Row(_basic, 0, 1f, 1f) } };

            Assert.That(WaveSpawnPlan.Count(wave), Is.EqualTo(1));
        }

        [Test]
        public void AnEmptyOrMissingWave_SendsNothing()
        {
            Assert.That(WaveSpawnPlan.Expand(null), Is.Empty);
            Assert.That(WaveSpawnPlan.Expand(new Wave()), Is.Empty);
        }

        [Test]
        public void Group_FoldsRunsOfTheSameEnemyAndDelay_Exactly()
        {
            var entries = new List<EnemySpawnEntry>
            {
                new EnemySpawnEntry { enemyPrefab = _basic, delayBeforeNext = 1.2f },
                new EnemySpawnEntry { enemyPrefab = _basic, delayBeforeNext = 1.2f },
                new EnemySpawnEntry { enemyPrefab = _basic, delayBeforeNext = 1f },
                new EnemySpawnEntry { enemyPrefab = _scarab, delayBeforeNext = 0.35f },
                new EnemySpawnEntry { enemyPrefab = _scarab, delayBeforeNext = 0.35f },
            };
            var leftSingle = new List<EnemySpawnEntry>();

            List<EnemySpawnGroup> groups = WaveSpawnPlan.Group(entries, leftSingle);

            Assert.That(leftSingle, Is.Empty);
            Assert.That(groups.Count, Is.EqualTo(3));
            Assert.That(groups[0].count, Is.EqualTo(2));
            Assert.That(groups[0].spawnInterval, Is.EqualTo(1.2f));
            Assert.That(groups[1].count, Is.EqualTo(1));
            Assert.That(groups[1].delayAfter, Is.EqualTo(1f));
            Assert.That(groups[2].enemyPrefab, Is.SameAs(_scarab));
            Assert.That(groups[2].count, Is.EqualTo(2));

            List<EnemySpawnEntry> again = WaveSpawnPlan.Expand(new Wave { groups = groups });
            Assert.That(again.Count, Is.EqualTo(entries.Count));
            for (int i = 0; i < entries.Count; i++)
            {
                Assert.That(again[i].enemyPrefab, Is.SameAs(entries[i].enemyPrefab), $"entry {i}");
                Assert.That(again[i].delayBeforeNext, Is.EqualTo(entries[i].delayBeforeNext), $"entry {i}");
            }
        }

        [Test]
        public void Group_KeepsAnEntryWithItsOwnSpawnPoint_Single()
        {
            var point = new GameObject("Side door").transform;
            try
            {
                var entries = new List<EnemySpawnEntry>
                {
                    new EnemySpawnEntry { enemyPrefab = _basic, delayBeforeNext = 1f },
                    new EnemySpawnEntry { enemyPrefab = _basic, delayBeforeNext = 1f, spawnPointOverride = point },
                    new EnemySpawnEntry { enemyPrefab = _basic, delayBeforeNext = 1f },
                };
                var leftSingle = new List<EnemySpawnEntry>();

                List<EnemySpawnGroup> groups = WaveSpawnPlan.Group(entries, leftSingle);

                Assert.That(groups.Count, Is.EqualTo(1));
                Assert.That(groups[0].count, Is.EqualTo(1));
                Assert.That(leftSingle.Count, Is.EqualTo(2), "everything from the special entry on stays single, so the order holds");
                Assert.That(leftSingle[0].spawnPointOverride, Is.SameAs(point));
            }
            finally
            {
                Object.DestroyImmediate(point.gameObject);
            }
        }

        // The entry counts the per-entry lists had on Indev 67004d9a.
        [TestCase("Assets/_Game/Features/Enemies/Data/L01_Meadow_Waves.asset", 137)]
        [TestCase("Assets/_Game/Features/Enemies/Data/L00_TutorialHill_Waves.asset", 47)]
        [TestCase("Assets/_Game/Features/Enemies/Data/Hill01_Waves.asset", 25)]
        public void EachLevelWaveList_IsGroupedRows_SendingTheSameEnemies(string path, int enemies)
        {
            var asset = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(path);
            Assert.That(asset, Is.Not.Null, $"{path} is missing.");

            int total = 0;
            for (int w = 0; w < asset.waves.Count; w++)
            {
                Wave wave = asset.waves[w];
                Assert.That(wave.groups, Is.Not.Empty, $"{asset.name} {wave.waveName} has no grouped rows.");
                Assert.That(wave.enemies, Is.Empty, $"{asset.name} {wave.waveName} still has single entries beside its rows.");
                total += WaveSpawnPlan.Count(wave);
            }

            Assert.That(total, Is.EqualTo(enemies));
        }

        [Test]
        public void TheMeadowsFirstWave_IsFiveBasics_OneEvery1Point2Seconds()
        {
            var asset = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>("Assets/_Game/Features/Enemies/Data/L01_Meadow_Waves.asset");
            Wave first = asset.waves[0];

            Assert.That(first.groups.Count, Is.EqualTo(1));
            Assert.That(first.groups[0].enemyPrefab.name, Is.EqualTo("Enemy_Basic"));
            Assert.That(first.groups[0].count, Is.EqualTo(5));
            Assert.That(first.groups[0].spawnInterval, Is.EqualTo(1.2f).Within(0.0001f));
        }

        [Test]
        public void JasonsTemplate_KeepsItsSingleEntries()
        {
            var asset = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>("Assets/_Game/Features/Enemies/Scripts/EnemySpawner/Map_01.asset");
            Assert.That(asset, Is.Not.Null);

            for (int w = 0; w < asset.waves.Count; w++)
            {
                Assert.That(asset.waves[w].groups, Is.Empty);
                Assert.That(asset.waves[w].enemies, Is.Not.Empty);
            }
        }
    }
}

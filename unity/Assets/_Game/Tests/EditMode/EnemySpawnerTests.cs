using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using HoldTheHill.Features.Combat;
using HoldTheHill.Features.Enemies;

namespace HoldTheHill.Tests
{
    public class EnemySpawnerTests
    {
        private GameObject _spawnerObject;
        private EnemySpawner _spawner;
        private GameObject _enemyPrefab;
        private List<GameObject> _spawnedObjects;

        [SetUp]
        public void SetUp()
        {
            _spawnedObjects = new List<GameObject>();

            _spawnerObject = new GameObject("TestEnemySpawner");
            _spawner = _spawnerObject.AddComponent<EnemySpawner>();
            _spawner.SubscribeToEvents();
            _spawnedObjects.Add(_spawnerObject);

            _enemyPrefab = new GameObject("TestEnemyPrefab");
            _enemyPrefab.AddComponent<BasicEnemy>();
            _spawnedObjects.Add(_enemyPrefab);
        }

        [TearDown]
        public void TearDown()
        {
            if (_spawner != null)
            {
                _spawner.UnsubscribeFromEvents();
            }

            if (_spawnedObjects != null)
            {
                foreach (var obj in _spawnedObjects)
                {
                    if (obj != null)
                    {
                        Object.DestroyImmediate(obj);
                    }
                }
                _spawnedObjects.Clear();
            }
        }

        [Test]
        public void LivingEnemyCount_InitializesToZero()
        {
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
        }

        [Test]
        public void SpawnEnemy_IncrementsLivingEnemyCount()
        {
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));

            GameObject enemy1 = _spawner.SpawnEnemy(_enemyPrefab);
            _spawnedObjects.Add(enemy1);
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            GameObject enemy2 = _spawner.SpawnEnemy(_enemyPrefab);
            _spawnedObjects.Add(enemy2);
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(2));
        }

        [Test]
        public void EnemyDeath_ViaStaticEvent_DecrementsLivingEnemyCount()
        {
            GameObject enemyObj = _spawner.SpawnEnemy(_enemyPrefab);
            _spawnedObjects.Add(enemyObj);
            BasicEnemy enemy = enemyObj.GetComponent<BasicEnemy>();
            enemy.SetMaxHealth(50.0, resetCurrentHealth: true);

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            // Fatal damage triggers Die(), which fires OnAnyDied
            enemy.TakeDamage(100.0);

            Assert.That(enemy.IsDead, Is.True);
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
        }

        [Test]
        public void EnemyDestinationReached_ViaStaticEvent_DecrementsLivingEnemyCount()
        {
            GameObject enemyObj = _spawner.SpawnEnemy(_enemyPrefab);
            _spawnedObjects.Add(enemyObj);
            BasicEnemy enemy = enemyObj.GetComponent<BasicEnemy>();

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            // Enemy reaches destination, which fires OnAnyDestinationReached
            enemy.ReachDestination();

            Assert.That(enemy.ReachedEnd, Is.True);
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
        }

        [Test]
        public void CentipedeSplit_ViaStaticEvent_IncrementsLivingEnemyCountByChildCount()
        {
            var centipedePrefab = new GameObject("TestCentipedePrefab");
            var centipede = centipedePrefab.AddComponent<CentipedeEnemy>();
            _spawnedObjects.Add(centipedePrefab);

            // Spawner spawns the centipede
            GameObject spawnedCentipedeObj = _spawner.SpawnEnemy(centipedePrefab);
            _spawnedObjects.Add(spawnedCentipedeObj);
            CentipedeEnemy spawnedCentipede = spawnedCentipedeObj.GetComponent<CentipedeEnemy>();
            spawnedCentipede.SetMaxHealth(20.0, resetCurrentHealth: true);
            spawnedCentipede.SplitsRemaining = 1;

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            // Splitting produces 2 children
            List<CentipedeEnemy> children = spawnedCentipede.Split();
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
            }

            // Split event should increase counter by 2
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(3));
        }

        [Test]
        public void CentipedeDefeatAndSplit_EndToEnd_CorrectlyUpdatesCounter()
        {
            var centipedePrefab = new GameObject("TestCentipedePrefab");
            var centipede = centipedePrefab.AddComponent<CentipedeEnemy>();
            _spawnedObjects.Add(centipedePrefab);

            // 1. Spawner spawns centipede (count = 1)
            GameObject spawnedCentipedeObj = _spawner.SpawnEnemy(centipedePrefab);
            _spawnedObjects.Add(spawnedCentipedeObj);
            CentipedeEnemy spawnedCentipede = spawnedCentipedeObj.GetComponent<CentipedeEnemy>();
            spawnedCentipede.SetMaxHealth(20.0, resetCurrentHealth: true);
            spawnedCentipede.SplitsRemaining = 1;

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            // 2. Fatal hit: centipede splits into 2 children (+2) then parent dies (-1). Net count = 2
            spawnedCentipede.TakeDamage(50.0);

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(2));

            // Find spawned children in scene
            CentipedeEnemy[] livingCentipedes = Object.FindObjectsByType<CentipedeEnemy>(FindObjectsSortMode.None);
            Assert.That(livingCentipedes.Length, Is.EqualTo(2));

            foreach (var child in livingCentipedes)
            {
                _spawnedObjects.Add(child.gameObject);
            }

            // 3. Child 1 dies (-1) -> count = 1
            livingCentipedes[0].TakeDamage(livingCentipedes[0].CurrentHealth);
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            // 4. Child 2 dies (-1) -> count = 0
            livingCentipedes[1].TakeDamage(livingCentipedes[1].CurrentHealth);
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
        }

        [Test]
        public void GenericEnemiesSpawned_ViaStaticEvent_IncrementsLivingEnemyCount()
        {
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));

            BasicEnemy.NotifyEnemiesSpawned(3);
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(3));

            BasicEnemy.NotifyEnemiesSpawned(2);
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(5));
        }

        [Test]
        public void LivingEnemyCount_DoesNotDropBelowZero()
        {
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));

            GameObject enemyObj = new GameObject("OrphanEnemy");
            BasicEnemy enemy = enemyObj.AddComponent<BasicEnemy>();
            _spawnedObjects.Add(enemyObj);
            enemy.SetMaxHealth(10.0, resetCurrentHealth: true);

            // Kill enemy when spawner has count 0
            enemy.TakeDamage(20.0);

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
        }

        [Test]
        public void SetActiveMap_ResetsLivingEnemyCountToZero()
        {
            GameObject enemyObj1 = _spawner.SpawnEnemy(_enemyPrefab);
            GameObject enemyObj2 = _spawner.SpawnEnemy(_enemyPrefab);
            _spawnedObjects.Add(enemyObj1);
            _spawnedObjects.Add(enemyObj2);

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(2));

            _spawner.SetActiveMap("Map_01");

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
        }

        [Test]
        public void NotifyEnemyDefeated_DecrementsLivingEnemyCount()
        {
            GameObject enemyObj = _spawner.SpawnEnemy(_enemyPrefab);
            _spawnedObjects.Add(enemyObj);

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            _spawner.NotifyEnemyDefeated(enemyObj);

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
        }

        [Test]
        public void WaveCleared_DoesNotFireWhenSpawningStops_FiresWhenLastEnemyDies()
        {
            var spawnedEnemies = new List<BasicEnemy>();
            _spawner.onEnemySpawned.AddListener(go =>
            {
                _spawnedObjects.Add(go);
                BasicEnemy enemy = go.GetComponent<BasicEnemy>();
                enemy.SetMaxHealth(50.0, resetCurrentHealth: true);
                spawnedEnemies.Add(enemy);
            });

            int completedCount = 0;
            int clearedCount = 0;
            int lastClearedWave = 0;
            _spawner.onWaveCompleted.AddListener(_ => completedCount++);
            _spawner.onWaveCleared.AddListener(wave =>
            {
                clearedCount++;
                lastClearedWave = wave;
            });

            MapWaveDataSO waveData = ScriptableObject.CreateInstance<MapWaveDataSO>();
            waveData.mapId = "TestMap";
            waveData.waves = new List<Wave>
            {
                new Wave
                {
                    waveName = "Wave 1",
                    enemies = new List<EnemySpawnEntry>
                    {
                        new EnemySpawnEntry { enemyPrefab = _enemyPrefab, delayBeforeNext = 0f },
                        new EnemySpawnEntry { enemyPrefab = _enemyPrefab, delayBeforeNext = 0f }
                    }
                }
            };

            try
            {
                var spawnerSo = new UnityEditor.SerializedObject(_spawner);
                spawnerSo.FindProperty("activeMapConfigOverride").objectReferenceValue = waveData;
                spawnerSo.ApplyModifiedPropertiesWithoutUndo();
                _spawner.SetActiveMap("TestMap");

                _spawner.StartNextWave();

                // Spawning has completed, but enemies are still alive.
                Assert.That(completedCount, Is.EqualTo(1));
                Assert.That(clearedCount, Is.EqualTo(0));
                Assert.That(spawnedEnemies.Count, Is.EqualTo(2));

                // Killing the first enemy must not clear the wave.
                spawnedEnemies[0].TakeDamage(100.0);
                Assert.That(clearedCount, Is.EqualTo(0));
                Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

                // Killing the last enemy clears the wave.
                spawnedEnemies[1].TakeDamage(100.0);
                Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
                Assert.That(clearedCount, Is.EqualTo(1));
                Assert.That(lastClearedWave, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(waveData);
            }
        }

        [Test]
        public void WaveCleared_WaitsForCentipedeSplitChildrenToDie()
        {
            var centipedePrefab = new GameObject("TestCentipedeWavePrefab");
            centipedePrefab.AddComponent<CentipedeEnemy>();
            _spawnedObjects.Add(centipedePrefab);

            var spawnedCentipedes = new List<CentipedeEnemy>();
            _spawner.onEnemySpawned.AddListener(go =>
            {
                _spawnedObjects.Add(go);
                CentipedeEnemy c = go.GetComponent<CentipedeEnemy>();
                c.SetMaxHealth(20.0, resetCurrentHealth: true);
                c.SplitsRemaining = 1;
                spawnedCentipedes.Add(c);
            });

            int clearedCount = 0;
            _spawner.onWaveCleared.AddListener(_ => clearedCount++);

            MapWaveDataSO waveData = ScriptableObject.CreateInstance<MapWaveDataSO>();
            waveData.mapId = "TestCentipedeMap";
            waveData.waves = new List<Wave>
            {
                new Wave
                {
                    waveName = "Wave 1",
                    enemies = new List<EnemySpawnEntry>
                    {
                        new EnemySpawnEntry { enemyPrefab = centipedePrefab, delayBeforeNext = 0f }
                    }
                }
            };

            try
            {
                var spawnerSo = new UnityEditor.SerializedObject(_spawner);
                spawnerSo.FindProperty("activeMapConfigOverride").objectReferenceValue = waveData;
                spawnerSo.ApplyModifiedPropertiesWithoutUndo();
                _spawner.SetActiveMap("TestCentipedeMap");

                _spawner.StartNextWave();
                Assert.That(clearedCount, Is.EqualTo(0));

                // Kill parent: splits into 2 children -> wave is NOT cleared yet.
                spawnedCentipedes[0].TakeDamage(50.0);
                Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(2));
                Assert.That(clearedCount, Is.EqualTo(0));

                CentipedeEnemy[] children = Object.FindObjectsByType<CentipedeEnemy>(FindObjectsSortMode.None);
                var liveChildren = new List<CentipedeEnemy>();
                foreach (var c in children)
                {
                    if (c.gameObject != centipedePrefab && c.IsActive)
                    {
                        _spawnedObjects.Add(c.gameObject);
                        liveChildren.Add(c);
                    }
                }
                Assert.That(liveChildren.Count, Is.EqualTo(2));

                liveChildren[0].TakeDamage(50.0);
                Assert.That(clearedCount, Is.EqualTo(0));

                liveChildren[1].TakeDamage(50.0);
                Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
                Assert.That(clearedCount, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(waveData);
            }
        }

        [Test]
        public void WaveCleared_WaitsForExternalRegisteredEnemies()
        {
            _enemyPrefab.AddComponent<RegisteredEnemy>();

            var spawnedEnemies = new List<BasicEnemy>();
            _spawner.onEnemySpawned.AddListener(go =>
            {
                _spawnedObjects.Add(go);
                BasicEnemy enemy = go.GetComponent<BasicEnemy>();
                enemy.SetMaxHealth(20.0, resetCurrentHealth: true);
                spawnedEnemies.Add(enemy);
            });

            int clearedCount = 0;
            _spawner.onWaveCleared.AddListener(_ => clearedCount++);

            MapWaveDataSO waveData = ScriptableObject.CreateInstance<MapWaveDataSO>();
            waveData.mapId = "TestRegisteredMap";
            waveData.waves = new List<Wave>
            {
                new Wave
                {
                    waveName = "Wave 1",
                    enemies = new List<EnemySpawnEntry>
                    {
                        new EnemySpawnEntry { enemyPrefab = _enemyPrefab, delayBeforeNext = 0f }
                    }
                }
            };

            try
            {
                var spawnerSo = new UnityEditor.SerializedObject(_spawner);
                spawnerSo.FindProperty("activeMapConfigOverride").objectReferenceValue = waveData;
                spawnerSo.ApplyModifiedPropertiesWithoutUndo();
                _spawner.SetActiveMap("TestRegisteredMap");

                _spawner.StartNextWave();

                // Simulate an escort/husk spawned directly outside EnemySpawner.
                GameObject externalObj = Object.Instantiate(_enemyPrefab);
                _spawnedObjects.Add(externalObj);
                BasicEnemy externalEnemy = externalObj.GetComponent<BasicEnemy>();
                externalEnemy.SetMaxHealth(20.0, resetCurrentHealth: true);

                // Killing only the spawner-tracked enemy drops LivingEnemyCount to 0,
                // but the external registered enemy is still active, so wave is not cleared yet.
                spawnedEnemies[0].TakeDamage(50.0);
                Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(0));
                Assert.That(clearedCount, Is.EqualTo(0));

                // Killing the external registered enemy clears the wave.
                externalEnemy.TakeDamage(50.0);
                Assert.That(clearedCount, Is.EqualTo(1));
            }
            finally
            {
                Object.DestroyImmediate(waveData);
            }
        }
    }
}

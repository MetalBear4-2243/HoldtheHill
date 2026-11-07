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
            centipedePrefab.AddComponent<BasicEnemy>();
            centipedePrefab.AddComponent<CentipedeSplitter>();
            _spawnedObjects.Add(centipedePrefab);

            // Spawner spawns the centipede
            GameObject spawnedCentipedeObj = _spawner.SpawnEnemy(centipedePrefab);
            _spawnedObjects.Add(spawnedCentipedeObj);
            BasicEnemy spawnedCentipede = spawnedCentipedeObj.GetComponent<BasicEnemy>();
            CentipedeSplitter spawnedSplitter = spawnedCentipedeObj.GetComponent<CentipedeSplitter>();
            spawnedCentipede.SetMaxHealth(20.0, resetCurrentHealth: true);
            spawnedSplitter.SplitsRemaining = 1;

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            // Splitting produces 2 children
            List<BasicEnemy> children = spawnedSplitter.Split();
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
            centipedePrefab.AddComponent<BasicEnemy>();
            centipedePrefab.AddComponent<CentipedeSplitter>();
            _spawnedObjects.Add(centipedePrefab);

            // 1. Spawner spawns centipede (count = 1)
            GameObject spawnedCentipedeObj = _spawner.SpawnEnemy(centipedePrefab);
            _spawnedObjects.Add(spawnedCentipedeObj);
            BasicEnemy spawnedCentipede = spawnedCentipedeObj.GetComponent<BasicEnemy>();
            spawnedCentipede.SetMaxHealth(20.0, resetCurrentHealth: true);
            spawnedCentipedeObj.GetComponent<CentipedeSplitter>().SplitsRemaining = 1;

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            // 2. Fatal hit: centipede splits into 2 children (+2) then parent dies (-1). Net count = 2
            spawnedCentipede.TakeDamage(50.0);

            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(2));

            // Find spawned children in scene
            CentipedeSplitter[] livingCentipedes = Object.FindObjectsByType<CentipedeSplitter>();
            Assert.That(livingCentipedes.Length, Is.EqualTo(2));

            foreach (var child in livingCentipedes)
            {
                _spawnedObjects.Add(child.gameObject);
            }

            // 3. Child 1 dies (-1) -> count = 1
            BasicEnemy child0 = livingCentipedes[0].GetComponent<BasicEnemy>();
            child0.TakeDamage(child0.CurrentHealth);
            Assert.That(_spawner.LivingEnemyCount, Is.EqualTo(1));

            // 4. Child 2 dies (-1) -> count = 0
            BasicEnemy child1 = livingCentipedes[1].GetComponent<BasicEnemy>();
            child1.TakeDamage(child1.CurrentHealth);
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
    }
}

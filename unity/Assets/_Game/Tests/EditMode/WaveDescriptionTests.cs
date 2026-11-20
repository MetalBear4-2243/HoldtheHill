using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.UI;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    public class WaveDescriptionTests
    {
        private GameObject _spawnerObject;
        private EnemySpawner _spawner;
        private GameObject _enemyPrefab;
        private GameObject _labelObject;
        private TextMeshProUGUI _label;
        private WaveDescriptionLabel _descriptionLabel;
        private MapWaveDataSO _waveData;
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

            _spawner.onEnemySpawned.AddListener(go =>
            {
                _spawnedObjects.Add(go);
                BasicEnemy enemy = go.GetComponent<BasicEnemy>();
                enemy.SetMaxHealth(20.0, resetCurrentHealth: true);
            });

            _labelObject = new GameObject("TestWaveDescriptionLabel");
            _label = _labelObject.AddComponent<TextMeshProUGUI>();
            _descriptionLabel = _labelObject.AddComponent<WaveDescriptionLabel>();
            _spawnedObjects.Add(_labelObject);

            _waveData = ScriptableObject.CreateInstance<MapWaveDataSO>();
            _waveData.mapId = "WaveDescriptionTestMap";
        }

        [TearDown]
        public void TearDown()
        {
            if (_descriptionLabel != null)
            {
                _descriptionLabel.UnbindSpawner();
            }

            if (_spawner != null)
            {
                _spawner.UnsubscribeFromEvents();
            }

            if (_waveData != null)
            {
                Object.DestroyImmediate(_waveData);
            }

            if (_spawnedObjects != null)
            {
                for (int i = 0; i < _spawnedObjects.Count; i++)
                {
                    if (_spawnedObjects[i] != null)
                    {
                        Object.DestroyImmediate(_spawnedObjects[i]);
                    }
                }

                _spawnedObjects.Clear();
            }
        }

        [Test]
        public void WaveCleared_DisplaysUpcomingCustomDescription_AndHidesWhenNextWaveStarts()
        {
            _waveData.waves = new List<Wave>
            {
                MakeWave("Wave 1", "First wave text", delayBeforeNext: 0f),
                MakeWave("Wave 2", "  Incoming Scarabs! Move fast!  ", delayBeforeNext: 0f)
            };

            LoadWavesAndBind();

            _spawner.StartNextWave();
            Assert.IsFalse(_label.enabled);
            Assert.AreEqual(string.Empty, _label.text);

            KillAllSpawnedEnemies();

            Assert.IsTrue(_label.enabled);
            Assert.AreEqual("Incoming Scarabs! Move fast!", _label.text);

            _spawner.StartNextWave();

            Assert.IsFalse(_label.enabled);
            Assert.AreEqual(string.Empty, _label.text);
        }

        [TestCase(Wave.DefaultWaveDescription)]
        [TestCase("default description")]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void WaveCleared_HidesText_WhenUpcomingDescriptionIsBlankOrDefault(string upcomingDescription)
        {
            _waveData.waves = new List<Wave>
            {
                MakeWave("Wave 1", "First wave", delayBeforeNext: 0f),
                MakeWave("Wave 2", upcomingDescription, delayBeforeNext: 0f)
            };

            LoadWavesAndBind();

            _spawner.StartNextWave();
            KillAllSpawnedEnemies();

            Assert.IsFalse(_label.enabled);
            Assert.AreEqual(string.Empty, _label.text);
        }

        [Test]
        public void FinalWaveCleared_DisplaysNothing()
        {
            _waveData.waves = new List<Wave>
            {
                MakeWave("Wave 1", "Should not show after wave 1 if it is the last wave", delayBeforeNext: 0f)
            };

            LoadWavesAndBind();

            _spawner.StartNextWave();
            KillAllSpawnedEnemies();

            Assert.IsFalse(_label.enabled);
            Assert.AreEqual(string.Empty, _label.text);
        }

        [Test]
        public void WaveClearedDuringFinalSpawnDelay_ResolvesNextWaveDescriptionAccurately()
        {
            _waveData.waves = new List<Wave>
            {
                MakeWave("Wave 1", "First wave", delayBeforeNext: 2f),
                MakeWave("Wave 2", "Wasps ahead — build range!", delayBeforeNext: 0f)
            };

            LoadWavesAndBind();

            // With delayBeforeNext > 0 on the last entry, StartNextWave yields before currentWaveIndex++ runs.
            _spawner.StartNextWave();
            Assert.AreEqual(0, _spawner.CurrentWaveIndex);

            KillAllSpawnedEnemies();

            Assert.IsTrue(_label.enabled);
            Assert.AreEqual("Wasps ahead — build range!", _label.text);
        }

        private Wave MakeWave(string name, string description, float delayBeforeNext)
        {
            return new Wave
            {
                waveName = name,
                waveDescription = description,
                enemies = new List<EnemySpawnEntry>
                {
                    new EnemySpawnEntry
                    {
                        enemyPrefab = _enemyPrefab,
                        delayBeforeNext = delayBeforeNext
                    }
                }
            };
        }

        private void LoadWavesAndBind()
        {
            var spawnerSo = new SerializedObject(_spawner);
            spawnerSo.FindProperty("activeMapConfigOverride").objectReferenceValue = _waveData;
            spawnerSo.ApplyModifiedPropertiesWithoutUndo();

            _spawner.SetActiveMap(_waveData.mapId);
            _descriptionLabel.BindSpawner(_spawner);
        }

        private void KillAllSpawnedEnemies()
        {
            for (int i = 0; i < _spawnedObjects.Count; i++)
            {
                GameObject obj = _spawnedObjects[i];
                if (obj == null || obj == _enemyPrefab)
                {
                    continue;
                }

                BasicEnemy enemy = obj.GetComponent<BasicEnemy>();
                if (enemy != null && enemy.IsActive)
                {
                    enemy.TakeDamage(1000.0);
                }
            }
        }
    }
}

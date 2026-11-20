using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// EnemySpawner.WaitingForClear: on a map that holds the next wave until the last is cleared,
    /// the Next Wave button and Space must read "not yet" while enemies live, matching StartNextWave.
    /// </summary>
    public class NextWaveClearTests
    {
        private GameObject _spawnerObject;
        private EnemySpawner _spawner;
        private GameObject _enemyPrefab;
        private MapWaveDataSO _waveData;
        private readonly List<GameObject> _spawned = new List<GameObject>();

        [SetUp]
        public void SetUp()
        {
            _spawnerObject = new GameObject("TestEnemySpawner");
            _spawner = _spawnerObject.AddComponent<EnemySpawner>();
            _spawner.SubscribeToEvents();
            _spawner.onEnemySpawned.AddListener(go => _spawned.Add(go));

            _enemyPrefab = new GameObject("TestEnemyPrefab");
            _enemyPrefab.AddComponent<BasicEnemy>();

            _waveData = ScriptableObject.CreateInstance<MapWaveDataSO>();
            _waveData.mapId = "NextWaveClearTestMap";
            _waveData.waves = new List<Wave> { MakeWave(), MakeWave() };
        }

        [TearDown]
        public void TearDown()
        {
            _spawner.UnsubscribeFromEvents();
            foreach (GameObject go in _spawned)
            {
                if (go != null) Object.DestroyImmediate(go);
            }

            _spawned.Clear();
            Object.DestroyImmediate(_waveData);
            Object.DestroyImmediate(_enemyPrefab);
            Object.DestroyImmediate(_spawnerObject);
        }

        [Test]
        public void HoldsNextWave_WhileEnemiesLive_WhenMapRequiresClear()
        {
            Load(requireClear: true);
            Assert.IsFalse(_spawner.WaitingForClear, "nothing alive before the first wave");

            _spawner.StartNextWave();
            Assert.IsTrue(_spawner.WaitingForClear, "an enemy of wave 1 is alive");

            foreach (GameObject go in _spawned)
            {
                go.GetComponent<BasicEnemy>().TakeDamage(1000.0);
            }

            Assert.IsFalse(_spawner.WaitingForClear, "wave 1 is cleared");
        }

        [Test]
        public void NeverHolds_WhenMapAllowsEarlyCalls()
        {
            Load(requireClear: false);
            _spawner.StartNextWave();
            Assert.IsFalse(_spawner.WaitingForClear);
        }

        private void Load(bool requireClear)
        {
            var so = new SerializedObject(_spawner);
            so.FindProperty("activeMapConfigOverride").objectReferenceValue = _waveData;
            so.FindProperty("requireEnemiesClearedBeforeNextWave").boolValue = requireClear;
            so.ApplyModifiedPropertiesWithoutUndo();
            _spawner.SetActiveMap(_waveData.mapId);
        }

        private Wave MakeWave()
        {
            return new Wave
            {
                waveName = "Wave",
                waveDescription = Wave.DefaultWaveDescription,
                enemies = new List<EnemySpawnEntry> { new EnemySpawnEntry { enemyPrefab = _enemyPrefab, delayBeforeNext = 0f } }
            };
        }
    }
}

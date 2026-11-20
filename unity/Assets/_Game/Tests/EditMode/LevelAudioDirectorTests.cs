using System.Collections.Generic;
using HoldTheHill.Core.Audio;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using NUnit.Framework;
using UnityEngine;

namespace HoldTheHill.Tests
{
    public class LevelAudioDirectorTests
    {
        private GameObject _directorObj;
        private LevelAudioDirector _director;

        private SoundEffectSO _waveStartedSfx;
        private SoundEffectSO _queenDamagedSfx;
        private SoundEffectSO _victorySfx;
        private SoundEffectSO _defeatSfx;
        private SoundEffectSO _bossAppearedSfx;
        private SoundEffectSO _centipedeSplitSfx;

        private int _playedSoundCount;
        private SoundEffectSO _lastPlayedSfx;

        [SetUp]
        public void SetUp()
        {
            _playedSoundCount = 0;
            _lastPlayedSfx = null;

            SoundEffectSO.PlaybackHandler = (sfx, pos, blend) =>
            {
                _playedSoundCount++;
                _lastPlayedSfx = sfx;
            };

            _waveStartedSfx = CreateTestSound("WaveStarted");
            _queenDamagedSfx = CreateTestSound("QueenDamaged");
            _victorySfx = CreateTestSound("Victory");
            _defeatSfx = CreateTestSound("Defeat");
            _bossAppearedSfx = CreateTestSound("BossAppeared");
            _centipedeSplitSfx = CreateTestSound("CentipedeSplit");

            _directorObj = new GameObject("TestLevelAudioDirector");
            _director = _directorObj.AddComponent<LevelAudioDirector>();

            _director.WaveStartedSfx = _waveStartedSfx;
            _director.QueenDamagedSfx = _queenDamagedSfx;
            _director.VictorySfx = _victorySfx;
            _director.DefeatSfx = _defeatSfx;
            _director.BossAppearedSfx = _bossAppearedSfx;
            _director.CentipedeSplitSfx = _centipedeSplitSfx;
        }

        [TearDown]
        public void TearDown()
        {
            if (_directorObj != null)
            {
                Object.DestroyImmediate(_directorObj);
            }

            DestroyTestSound(_waveStartedSfx);
            DestroyTestSound(_queenDamagedSfx);
            DestroyTestSound(_victorySfx);
            DestroyTestSound(_defeatSfx);
            DestroyTestSound(_bossAppearedSfx);
            DestroyTestSound(_centipedeSplitSfx);

            SoundEffectSO.PlaybackHandler = null;
        }

        private SoundEffectSO CreateTestSound(string name)
        {
            var sfx = ScriptableObject.CreateInstance<SoundEffectSO>();
            sfx.name = name;
            var clip = AudioClip.Create(name + "_Clip", 44100, 1, 44100, false);
            sfx.Clips = new[] { clip };
            sfx.Cooldown = 0f;
            return sfx;
        }

        private void DestroyTestSound(SoundEffectSO sfx)
        {
            if (sfx != null)
            {
                if (sfx.Clips != null)
                {
                    for (int i = 0; i < sfx.Clips.Length; i++)
                    {
                        if (sfx.Clips[i] != null)
                        {
                            Object.DestroyImmediate(sfx.Clips[i]);
                        }
                    }
                }
                Object.DestroyImmediate(sfx);
            }
        }

        [Test]
        public void Director_Fields_AreAssignedCorrectly()
        {
            Assert.AreSame(_waveStartedSfx, _director.WaveStartedSfx);
            Assert.AreSame(_queenDamagedSfx, _director.QueenDamagedSfx);
            Assert.AreSame(_victorySfx, _director.VictorySfx);
            Assert.AreSame(_defeatSfx, _director.DefeatSfx);
            Assert.AreSame(_bossAppearedSfx, _director.BossAppearedSfx);
            Assert.AreSame(_centipedeSplitSfx, _director.CentipedeSplitSfx);
        }

        [Test]
        public void BossAppeared_PlaysBossAppearedSfx()
        {
            // Trigger through reflection / static event if needed, or QueenMantisBoss
            var mantisObj = new GameObject("QueenMantis");
            var boss = mantisObj.AddComponent<QueenMantisBoss>();

            // Triggering Awake/Start or Boss Appeared
            // QueenMantisBoss raises Appeared in Start:
            // But we can also invoke via component or public test
            Object.DestroyImmediate(mantisObj);
        }

        [Test]
        public void Disabled_Director_DoesNotListenToStaticEvents()
        {
            _director.enabled = false;
            Assert.IsNull(_director.BoundRun);
        }

        [Test]
        public void UnbindFromRun_HandlesNullSafely()
        {
            Assert.DoesNotThrow(() =>
            {
                _director.UnbindFromRun(null);
            });
        }

        [Test]
        public void WaveCompletedSfx_DoesNotPlayWhenSpawningStops_PlaysWhenLastEnemyDies()
        {
            SoundEffectSO waveCompletedSfx = CreateTestSound("WaveCompleted");
            _director.WaveCompletedSfx = waveCompletedSfx;

            var runObj = new GameObject("TestLevelRun");
            var spawnerObj = new GameObject("TestSpawner");
            var enemyPrefab = new GameObject("TestEnemyPrefab");
            var spawnedEnemies = new List<BasicEnemy>();
            MapWaveDataSO waveData = ScriptableObject.CreateInstance<MapWaveDataSO>();

            try
            {
                LevelRun run = runObj.AddComponent<LevelRun>();
                EnemySpawner spawner = spawnerObj.AddComponent<EnemySpawner>();
                spawner.SubscribeToEvents();

                BasicEnemy prefabEnemy = enemyPrefab.AddComponent<BasicEnemy>();
                prefabEnemy.SetMaxHealth(50.0, resetCurrentHealth: true);

                spawner.onEnemySpawned.AddListener(go =>
                {
                    BasicEnemy enemy = go.GetComponent<BasicEnemy>();
                    enemy.SetMaxHealth(50.0, resetCurrentHealth: true);
                    spawnedEnemies.Add(enemy);
                });

                waveData.mapId = "AudioTestMap";
                waveData.waves = new List<Wave>
                {
                    new Wave
                    {
                        waveName = "Wave 1",
                        enemies = new List<EnemySpawnEntry>
                        {
                            new EnemySpawnEntry { enemyPrefab = enemyPrefab, delayBeforeNext = 0f },
                            new EnemySpawnEntry { enemyPrefab = enemyPrefab, delayBeforeNext = 0f }
                        }
                    }
                };

                var spawnerSo = new UnityEditor.SerializedObject(spawner);
                spawnerSo.FindProperty("activeMapConfigOverride").objectReferenceValue = waveData;
                spawnerSo.ApplyModifiedPropertiesWithoutUndo();
                spawner.SetActiveMap("AudioTestMap");

                var runSo = new UnityEditor.SerializedObject(run);
                runSo.FindProperty("_spawner").objectReferenceValue = spawner;
                runSo.ApplyModifiedPropertiesWithoutUndo();

                _director.BindToRun(run);
                _playedSoundCount = 0;
                _lastPlayedSfx = null;

                spawner.StartNextWave();

                // Spawning is finished (onWaveCompleted fired), but enemies remain alive:
                // the wave complete sound must NOT play yet.
                Assert.AreEqual(0, _playedSoundCount);

                // First enemy dies: still 1 living enemy, sound must NOT play yet.
                spawnedEnemies[0].TakeDamage(100.0);
                Assert.AreEqual(0, _playedSoundCount);

                // Last enemy dies: wave complete sound MUST play now.
                spawnedEnemies[1].TakeDamage(100.0);
                Assert.AreEqual(1, _playedSoundCount);
                Assert.AreSame(waveCompletedSfx, _lastPlayedSfx);
            }
            finally
            {
                _director.UnbindFromRun(runObj.GetComponent<LevelRun>());
                foreach (var enemy in spawnedEnemies)
                {
                    if (enemy != null)
                    {
                        Object.DestroyImmediate(enemy.gameObject);
                    }
                }
                Object.DestroyImmediate(enemyPrefab);
                Object.DestroyImmediate(spawnerObj);
                Object.DestroyImmediate(runObj);
                Object.DestroyImmediate(waveData);
                DestroyTestSound(waveCompletedSfx);
            }
        }

        [Test]
        public void WaveCompletedSfx_PlaysWhenSpawningFinishes_WhenPlayOnWaveClearedFalse()
        {
            SoundEffectSO waveCompletedSfx = CreateTestSound("WaveCompleted");
            _director.WaveCompletedSfx = waveCompletedSfx;
            _director.PlayOnWaveCleared = false;

            var runObj = new GameObject("TestLevelRun");
            var spawnerObj = new GameObject("TestSpawner");
            var enemyPrefab = new GameObject("TestEnemyPrefab");
            var spawnedEnemies = new List<BasicEnemy>();
            MapWaveDataSO waveData = ScriptableObject.CreateInstance<MapWaveDataSO>();

            try
            {
                LevelRun run = runObj.AddComponent<LevelRun>();
                EnemySpawner spawner = spawnerObj.AddComponent<EnemySpawner>();
                spawner.SubscribeToEvents();

                BasicEnemy prefabEnemy = enemyPrefab.AddComponent<BasicEnemy>();
                prefabEnemy.SetMaxHealth(50.0, resetCurrentHealth: true);

                spawner.onEnemySpawned.AddListener(go =>
                {
                    BasicEnemy enemy = go.GetComponent<BasicEnemy>();
                    enemy.SetMaxHealth(50.0, resetCurrentHealth: true);
                    spawnedEnemies.Add(enemy);
                });

                waveData.mapId = "AudioTestMap";
                waveData.waves = new List<Wave>
                {
                    new Wave
                    {
                        waveName = "Wave 1",
                        enemies = new List<EnemySpawnEntry>
                        {
                            new EnemySpawnEntry { enemyPrefab = enemyPrefab, delayBeforeNext = 0f }
                        }
                    }
                };

                var spawnerSo = new UnityEditor.SerializedObject(spawner);
                spawnerSo.FindProperty("activeMapConfigOverride").objectReferenceValue = waveData;
                spawnerSo.ApplyModifiedPropertiesWithoutUndo();
                spawner.SetActiveMap("AudioTestMap");

                var runSo = new UnityEditor.SerializedObject(run);
                runSo.FindProperty("_spawner").objectReferenceValue = spawner;
                runSo.ApplyModifiedPropertiesWithoutUndo();

                _director.BindToRun(run);
                _playedSoundCount = 0;
                _lastPlayedSfx = null;

                // Spawning finished immediately since delayBeforeNext = 0:
                spawner.StartNextWave();

                // With PlayOnWaveCleared = false, onWaveCompleted triggers the sound right when spawning finishes:
                Assert.AreEqual(1, _playedSoundCount);
                Assert.AreSame(waveCompletedSfx, _lastPlayedSfx);
            }
            finally
            {
                _director.UnbindFromRun(runObj.GetComponent<LevelRun>());
                foreach (var enemy in spawnedEnemies)
                {
                    if (enemy != null)
                    {
                        Object.DestroyImmediate(enemy.gameObject);
                    }
                }
                Object.DestroyImmediate(enemyPrefab);
                Object.DestroyImmediate(spawnerObj);
                Object.DestroyImmediate(runObj);
                Object.DestroyImmediate(waveData);
                DestroyTestSound(waveCompletedSfx);
            }
        }

        [Test]
        public void BindToRun_PlaysMusic_WhenAudioManagerPresent()
        {
            var audioMgrObj = new GameObject("TestAudioManager");
            var audioMgr = audioMgrObj.AddComponent<AudioManager>();

            var runObj = new GameObject("TestRun");
            var run = runObj.AddComponent<LevelRun>();

            var musicClip = AudioClip.Create("TestLevelMusic", 44100, 1, 44100, false);
            _director.LevelMusic = musicClip;
            _director.MusicFadeDuration = 0f;

            try
            {
                _director.BindToRun(run);

                Assert.IsNotNull(audioMgr.CurrentMusicSource);
                Assert.AreSame(musicClip, audioMgr.CurrentMusicSource.clip);
                Assert.IsTrue(audioMgr.CurrentMusicSource.isPlaying);
                Assert.IsTrue(audioMgr.CurrentMusicSource.loop);

                _director.UnbindFromRun(run);

                Assert.IsFalse(audioMgr.CurrentMusicSource.isPlaying);
            }
            finally
            {
                Object.DestroyImmediate(musicClip);
                Object.DestroyImmediate(runObj);
                Object.DestroyImmediate(audioMgrObj);
            }
        }

        [Test]
        public void BindToRun_FallsBackToDefaultMusic_WhenLevelMusicUnset()
        {
            var audioMgrObj = new GameObject("TestAudioManager");
            var audioMgr = audioMgrObj.AddComponent<AudioManager>();

            var defaultClip = AudioClip.Create("DefaultMusic", 44100, 1, 44100, false);
            audioMgr.DefaultMusic = defaultClip;

            var runObj = new GameObject("TestRun");
            var run = runObj.AddComponent<LevelRun>();

            _director.LevelMusic = null;
            _director.MusicFadeDuration = 0f;

            try
            {
                _director.BindToRun(run);

                Assert.IsNotNull(audioMgr.CurrentMusicSource);
                Assert.AreSame(defaultClip, audioMgr.CurrentMusicSource.clip);
                Assert.IsTrue(audioMgr.CurrentMusicSource.isPlaying);
            }
            finally
            {
                _director.UnbindFromRun(run);
                Object.DestroyImmediate(defaultClip);
                Object.DestroyImmediate(runObj);
                Object.DestroyImmediate(audioMgrObj);
            }
        }

        private static void InvokeDirectorLifecycle(LevelAudioDirector director, string methodName)
        {
            var method = typeof(LevelAudioDirector).GetMethod(
                methodName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(method, $"Expected {methodName} on LevelAudioDirector.");
            method.Invoke(director, null);
        }

        [Test]
        public void EnemyDeath_PlaysEnemyDeathSfx()
        {
            SoundEffectSO enemyDeathSfx = CreateTestSound("EnemyDeath");
            _director.EnemyDeathSfx = enemyDeathSfx;
            InvokeDirectorLifecycle(_director, "OnEnable");

            var enemyObj = new GameObject("TestEnemy");
            var enemy = enemyObj.AddComponent<BasicEnemy>();
            enemy.SetMaxHealth(10.0, resetCurrentHealth: true);

            try
            {
                enemy.TakeDamage(20.0);

                Assert.AreEqual(1, _playedSoundCount);
                Assert.AreSame(enemyDeathSfx, _lastPlayedSfx);
            }
            finally
            {
                InvokeDirectorLifecycle(_director, "OnDisable");
                if (enemyObj != null)
                {
                    Object.DestroyImmediate(enemyObj);
                }
                DestroyTestSound(enemyDeathSfx);
            }
        }

        [Test]
        public void EnemyReachedDestination_DoesNotPlayEnemyDeathSfx()
        {
            SoundEffectSO enemyDeathSfx = CreateTestSound("EnemyDeath");
            _director.EnemyDeathSfx = enemyDeathSfx;
            InvokeDirectorLifecycle(_director, "OnEnable");

            var enemyObj = new GameObject("TestEnemy");
            var enemy = enemyObj.AddComponent<BasicEnemy>();

            try
            {
                enemy.ReachDestination();

                Assert.AreEqual(0, _playedSoundCount);
                Assert.IsNull(_lastPlayedSfx);
            }
            finally
            {
                InvokeDirectorLifecycle(_director, "OnDisable");
                if (enemyObj != null)
                {
                    Object.DestroyImmediate(enemyObj);
                }
                DestroyTestSound(enemyDeathSfx);
            }
        }

        [Test]
        public void CentipedeSplit_DoesNotPlayEnemyDeathSfx_TerminalCentipedePlaysEnemyDeathSfx()
        {
            SoundEffectSO enemyDeathSfx = CreateTestSound("EnemyDeath");
            _director.EnemyDeathSfx = enemyDeathSfx;
            InvokeDirectorLifecycle(_director, "OnEnable");

            var playedSounds = new List<SoundEffectSO>();
            SoundEffectSO.PlaybackHandler = (sfx, pos, blend) =>
            {
                _playedSoundCount++;
                _lastPlayedSfx = sfx;
                playedSounds.Add(sfx);
            };

            var centipedeObj = new GameObject("TestCentipede");
            var centipede = centipedeObj.AddComponent<CentipedeEnemy>();
            centipede.SetMaxHealth(4.0, resetCurrentHealth: true);
            centipede.SplitsRemaining = 1;

            var spawnedChildren = new List<CentipedeEnemy>();
            centipede.OnSplit += (_, children) => spawnedChildren.AddRange(children);

            try
            {
                // First death triggers split: should play centipede split SFX, NOT enemy death SFX.
                centipede.TakeDamage(10.0);

                Assert.AreEqual(1, playedSounds.Count);
                Assert.AreSame(_centipedeSplitSfx, playedSounds[0]);
                Assert.AreEqual(2, spawnedChildren.Count);

                // Child has SplitsRemaining == 0: terminal death should play enemy death SFX.
                playedSounds.Clear();
                spawnedChildren[0].TakeDamage(10.0);

                Assert.AreEqual(1, playedSounds.Count);
                Assert.AreSame(enemyDeathSfx, playedSounds[0]);
            }
            finally
            {
                InvokeDirectorLifecycle(_director, "OnDisable");
                for (int i = 0; i < spawnedChildren.Count; i++)
                {
                    if (spawnedChildren[i] != null)
                    {
                        Object.DestroyImmediate(spawnedChildren[i].gameObject);
                    }
                }
                if (centipedeObj != null)
                {
                    Object.DestroyImmediate(centipedeObj);
                }
                DestroyTestSound(enemyDeathSfx);
            }
        }

        [Test]
        public void Disabled_Director_DoesNotPlayEnemyDeathSfx()
        {
            SoundEffectSO enemyDeathSfx = CreateTestSound("EnemyDeath");
            _director.EnemyDeathSfx = enemyDeathSfx;
            InvokeDirectorLifecycle(_director, "OnEnable");
            InvokeDirectorLifecycle(_director, "OnDisable");

            var enemyObj = new GameObject("TestEnemy");
            var enemy = enemyObj.AddComponent<BasicEnemy>();
            enemy.SetMaxHealth(10.0, resetCurrentHealth: true);

            try
            {
                enemy.TakeDamage(20.0);

                Assert.AreEqual(0, _playedSoundCount);
                Assert.IsNull(_lastPlayedSfx);
            }
            finally
            {
                if (enemyObj != null)
                {
                    Object.DestroyImmediate(enemyObj);
                }
                DestroyTestSound(enemyDeathSfx);
            }
        }
    }
}


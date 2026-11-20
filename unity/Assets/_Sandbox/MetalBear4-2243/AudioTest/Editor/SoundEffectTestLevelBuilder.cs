using System;
using System.Collections.Generic;
using System.IO;
using HoldTheHill.Core.Audio;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.Hazards;
using HoldTheHill.Features.PowerUps;
using HoldTheHill.Features.Towers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Sandbox.AudioTest.Editor
{
    /// <summary>
    /// Builds a self-contained sandbox level at
    /// <c>Assets/_Sandbox/MetalBear4-2243/AudioTest/Scenes/SoundEffectTestLevel.unity</c>
    /// with synthesized placeholder WAV clips, SoundEffectSO assets, a 3-wave test map
    /// (Basic, Silverfish, Centipede, Scarab, Queen Mantis), AudioManager, LevelAudioDirector,
    /// and the SoundEffectTestPanel (F2).
    /// </summary>
    internal static class SoundEffectTestLevelBuilder
    {
        private const string RootFolder = "Assets/_Sandbox/MetalBear4-2243/AudioTest";
        private const string ClipsFolder = RootFolder + "/Audio/Clips";
        private const string SfxDataFolder = RootFolder + "/Audio/Data";
        private const string DataFolder = RootFolder + "/Data";
        private const string ScenesFolder = RootFolder + "/Scenes";

        private const string ScenePath = ScenesFolder + "/SoundEffectTestLevel.unity";
        private const string WavesPath = DataFolder + "/AudioTest_Waves.asset";
        private const string TemplateGameplayPath = "Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity";

        private const string BasicEnemyPath = "Assets/_Game/Features/Enemies/Prefabs/Enemy_Basic.prefab";
        private const string ScarabEnemyPath = "Assets/_Game/Features/Enemies/Prefabs/Enemy_Scarab.prefab";
        private const string SilverfishEnemyPath = "Assets/_Game/Features/Enemies/Prefabs/Enemy_Silverfish.prefab";
        private const string CentipedeEnemyPath = "Assets/_Game/Features/Enemies/Prefabs/Enemy_Centipede.prefab";
        private const string QueenMantisEnemyPath = "Assets/_Game/Features/Enemies/Prefabs/Enemy_QueenMantis.prefab";

        private const int SampleRate = 44100;

        [MenuItem("Tools/Hold the Hill/Sandbox/Build Sound Effect Test Level")]
        public static void Build()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EnsureFolder(ClipsFolder);
            EnsureFolder(SfxDataFolder);
            EnsureFolder(DataFolder);
            EnsureFolder(ScenesFolder);

            EnsureAllTestWavClips();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            SfxBundle sfx = EnsureSoundEffectAssets();
            MapWaveDataSO waves = EnsureTestWaves();

            if (!CopyTemplateSceneIfMissing())
            {
                return;
            }

            ConfigureTestScene(sfx, waves);
            AssetDatabase.SaveAssets();

            Debug.Log($"Sound Effect Test Level ready at '{ScenePath}'. Press Play and use F2 to open the Audio Test Panel.");
        }

        /// <summary>Batchmode entry point: -executeMethod HoldTheHill.Sandbox.AudioTest.Editor.SoundEffectTestLevelBuilder.BuildFromCommandLine</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        private sealed class SfxBundle
        {
            public SoundEffectSO WaveStarted;
            public SoundEffectSO WaveCompleted;
            public SoundEffectSO QueenDamaged;
            public SoundEffectSO Victory;
            public SoundEffectSO Defeat;
            public SoundEffectSO BossAppeared;
            public SoundEffectSO CentipedeSplit;
            public SoundEffectSO TowerPlaced;
            public AudioClip CalmMusic;
            public AudioClip IntenseMusic;
        }

        private static bool CopyTemplateSceneIfMissing()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
            {
                return true;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(TemplateGameplayPath) == null)
            {
                Debug.LogError($"Template gameplay scene missing at '{TemplateGameplayPath}'.");
                return false;
            }

            if (!AssetDatabase.CopyAsset(TemplateGameplayPath, ScenePath))
            {
                Debug.LogError($"Failed to copy '{TemplateGameplayPath}' to '{ScenePath}'.");
                return false;
            }

            return true;
        }

        private static void ConfigureTestScene(SfxBundle sfx, MapWaveDataSO waves)
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            // Re-load assets after OpenScene so references stay valid.
            waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(WavesPath);
            sfx = LoadSfxBundle();

            // Distinctive background color so the sandbox scene is immediately recognizable.
            Camera cam = FindInScene<Camera>(scene);
            if (cam != null)
            {
                cam.backgroundColor = new Color(0.22f, 0.36f, 0.31f, 1f);
            }

            // Remove HazardScheduler if copied from Tutorial Hill so tests stay predictable.
            HazardScheduler hazards = FindInScene<HazardScheduler>(scene);
            if (hazards != null)
            {
                UnityEngine.Object.DestroyImmediate(hazards.gameObject);
            }

            // Give plenty of starting Honeydew so all 6 tower types can be placed and upgraded right away.
            HoneydewWallet wallet = FindInScene<HoneydewWallet>(scene);
            if (wallet != null)
            {
                var walletSo = new SerializedObject(wallet);
                walletSo.FindProperty("_startingHoneydew").intValue = 1000;
                walletSo.ApplyModifiedPropertiesWithoutUndo();
            }

            WaveReward reward = FindInScene<WaveReward>(scene);
            if (reward != null)
            {
                var rewardSo = new SerializedObject(reward);
                rewardSo.FindProperty("_honeydewPerWave").intValue = 150;
                rewardSo.ApplyModifiedPropertiesWithoutUndo();
            }

            // Point EnemySpawner to the dedicated 3-wave audio test list.
            EnemySpawner spawner = FindInScene<EnemySpawner>(scene);
            if (spawner != null)
            {
                var spawnerSo = new SerializedObject(spawner);
                spawnerSo.FindProperty("activeMapConfigOverride").objectReferenceValue = waves;
                spawnerSo.FindProperty("autoStartFirstWave").boolValue = false;
                spawnerSo.ApplyModifiedPropertiesWithoutUndo();
            }

            // Ensure AudioManager exists locally so Play mode works even if Bootstrap hasn't run Setup Audio.
            AudioManager audioManager = FindInScene<AudioManager>(scene);
            if (audioManager == null)
            {
                var amGo = new GameObject("Audio Manager");
                audioManager = amGo.AddComponent<AudioManager>();
            }

            var amSo = new SerializedObject(audioManager);
            amSo.FindProperty("_sfxPoolSize").intValue = 16;
            amSo.FindProperty("_defaultMusic").objectReferenceValue = sfx.CalmMusic;
            amSo.FindProperty("_playMusicOnStart").boolValue = false;
            amSo.ApplyModifiedPropertiesWithoutUndo();

            // Ensure LevelAudioDirector is wired on Level Run with all 8 SoundEffectSO slots.
            LevelRun run = FindInScene<LevelRun>(scene);
            LevelAudioDirector director = FindInScene<LevelAudioDirector>(scene);
            if (director == null)
            {
                GameObject host = run != null ? run.gameObject : new GameObject("Level Audio Director");
                director = host.AddComponent<LevelAudioDirector>();
            }

            var dirSo = new SerializedObject(director);
            dirSo.FindProperty("_run").objectReferenceValue = run;
            dirSo.FindProperty("_waveStartedSfx").objectReferenceValue = sfx.WaveStarted;
            dirSo.FindProperty("_waveCompletedSfx").objectReferenceValue = sfx.WaveCompleted;
            dirSo.FindProperty("_queenDamagedSfx").objectReferenceValue = sfx.QueenDamaged;
            dirSo.FindProperty("_victorySfx").objectReferenceValue = sfx.Victory;
            dirSo.FindProperty("_defeatSfx").objectReferenceValue = sfx.Defeat;
            dirSo.FindProperty("_bossAppearedSfx").objectReferenceValue = sfx.BossAppeared;
            dirSo.FindProperty("_centipedeSplitSfx").objectReferenceValue = sfx.CentipedeSplit;
            dirSo.FindProperty("_towerPlacedSfx").objectReferenceValue = sfx.TowerPlaced;
            dirSo.ApplyModifiedPropertiesWithoutUndo();

            // Wire SoundEffectTestPanel (F2).
            SoundEffectTestPanel panel = FindInScene<SoundEffectTestPanel>(scene);
            if (panel == null)
            {
                var panelGo = new GameObject("Audio Test Panel");
                panel = panelGo.AddComponent<SoundEffectTestPanel>();
            }

            EnemyPath path = FindInScene<EnemyPath>(scene);
            var centipede = AssetDatabase.LoadAssetAtPath<GameObject>(CentipedeEnemyPath);
            var silverfish = AssetDatabase.LoadAssetAtPath<GameObject>(SilverfishEnemyPath);
            var queenMantis = AssetDatabase.LoadAssetAtPath<GameObject>(QueenMantisEnemyPath);

            var panelSo = new SerializedObject(panel);
            panelSo.FindProperty("_run").objectReferenceValue = run;
            panelSo.FindProperty("_director").objectReferenceValue = director;
            panelSo.FindProperty("_path").objectReferenceValue = path;
            panelSo.FindProperty("_waveStartedSfx").objectReferenceValue = sfx.WaveStarted;
            panelSo.FindProperty("_waveCompletedSfx").objectReferenceValue = sfx.WaveCompleted;
            panelSo.FindProperty("_queenDamagedSfx").objectReferenceValue = sfx.QueenDamaged;
            panelSo.FindProperty("_victorySfx").objectReferenceValue = sfx.Victory;
            panelSo.FindProperty("_defeatSfx").objectReferenceValue = sfx.Defeat;
            panelSo.FindProperty("_bossAppearedSfx").objectReferenceValue = sfx.BossAppeared;
            panelSo.FindProperty("_centipedeSplitSfx").objectReferenceValue = sfx.CentipedeSplit;
            panelSo.FindProperty("_towerPlacedSfx").objectReferenceValue = sfx.TowerPlaced;
            panelSo.FindProperty("_calmMusic").objectReferenceValue = sfx.CalmMusic;
            panelSo.FindProperty("_intenseMusic").objectReferenceValue = sfx.IntenseMusic;
            panelSo.FindProperty("_centipedePrefab").objectReferenceValue = centipede != null ? centipede.GetComponent<BasicEnemy>() : null;
            panelSo.FindProperty("_silverfishPrefab").objectReferenceValue = silverfish != null ? silverfish.GetComponent<BasicEnemy>() : null;
            panelSo.FindProperty("_queenMantisPrefab").objectReferenceValue = queenMantis != null ? queenMantis.GetComponent<BasicEnemy>() : null;
            panelSo.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // ---- Waves ----------------------------------------------------------------------------

        private static MapWaveDataSO EnsureTestWaves()
        {
            var basic = AssetDatabase.LoadAssetAtPath<GameObject>(BasicEnemyPath);
            var scarab = AssetDatabase.LoadAssetAtPath<GameObject>(ScarabEnemyPath);
            var silverfish = AssetDatabase.LoadAssetAtPath<GameObject>(SilverfishEnemyPath);
            var centipede = AssetDatabase.LoadAssetAtPath<GameObject>(CentipedeEnemyPath);
            var queenMantis = AssetDatabase.LoadAssetAtPath<GameObject>(QueenMantisEnemyPath);

            MapWaveDataSO waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(WavesPath);
            if (waves == null)
            {
                waves = ScriptableObject.CreateInstance<MapWaveDataSO>();
                AssetDatabase.CreateAsset(waves, WavesPath);
            }

            waves.mapId = "AudioTest";
            waves.waves = new List<Wave>
            {
                new Wave
                {
                    waveName = "Wave 1 - Warmup",
                    waveDescription = "Quick basic enemies to test Wave Start, Tower Place, and Wave Clear.",
                    enemies = new List<EnemySpawnEntry>
                    {
                        Entry(basic, 0.8f),
                        Entry(basic, 0.8f),
                        Entry(basic, 0.8f),
                    }
                },
                new Wave
                {
                    waveName = "Wave 2 - Centipede & Silverfish",
                    waveDescription = "Tests Centipede Split SFX and click-to-kill Silverfish.",
                    enemies = new List<EnemySpawnEntry>
                    {
                        Entry(silverfish != null ? silverfish : basic, 0.7f),
                        Entry(silverfish != null ? silverfish : basic, 0.9f),
                        Entry(centipede != null ? centipede : basic, 1.5f),
                    }
                },
                new Wave
                {
                    waveName = "Wave 3 - Boss Entrance",
                    waveDescription = "Tests Boss Appeared SFX and Victory SFX.",
                    enemies = new List<EnemySpawnEntry>
                    {
                        Entry(scarab != null ? scarab : basic, 0.35f),
                        Entry(scarab != null ? scarab : basic, 0.35f),
                        Entry(scarab != null ? scarab : basic, 1.2f),
                        Entry(queenMantis != null ? queenMantis : basic, 2.0f),
                    }
                }
            };

            EditorUtility.SetDirty(waves);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(WavesPath);
        }

        private static EnemySpawnEntry Entry(GameObject prefab, float delay)
        {
            return new EnemySpawnEntry
            {
                enemyPrefab = prefab,
                delayBeforeNext = delay,
            };
        }

        // ---- SoundEffectSO Assets -------------------------------------------------------------

        private static SfxBundle EnsureSoundEffectAssets()
        {
            EnsureSfxAsset("sfx_test_wave_start", new[] { "sfx_test_wave_start_01", "sfx_test_wave_start_02" },
                volume: 0.85f, pitchMin: 0.95f, pitchMax: 1.05f, cooldown: 0f, spatialBlend: 0f);

            EnsureSfxAsset("sfx_test_wave_clear", new[] { "sfx_test_wave_clear_01" },
                volume: 0.85f, pitchMin: 0.98f, pitchMax: 1.02f, cooldown: 0f, spatialBlend: 0f);

            EnsureSfxAsset("sfx_test_queen_damage", new[] { "sfx_test_queen_damage_01", "sfx_test_queen_damage_02" },
                volume: 0.9f, pitchMin: 0.92f, pitchMax: 1.08f, cooldown: 0f, spatialBlend: 0f);

            EnsureSfxAsset("sfx_test_victory", new[] { "sfx_test_victory_01" },
                volume: 0.9f, pitchMin: 1f, pitchMax: 1f, cooldown: 0f, spatialBlend: 0f);

            EnsureSfxAsset("sfx_test_defeat", new[] { "sfx_test_defeat_01" },
                volume: 0.9f, pitchMin: 1f, pitchMax: 1f, cooldown: 0f, spatialBlend: 0f);

            EnsureSfxAsset("sfx_test_boss_appear", new[] { "sfx_test_boss_appear_01" },
                volume: 0.95f, pitchMin: 0.97f, pitchMax: 1.03f, cooldown: 0f, spatialBlend: 0f);

            EnsureSfxAsset("sfx_test_centipede_split", new[] { "sfx_test_centipede_split_01" },
                volume: 0.85f, pitchMin: 0.9f, pitchMax: 1.1f, cooldown: 0f, spatialBlend: 0.35f);

            EnsureSfxAsset("sfx_test_tower_place", new[] { "sfx_test_tower_place_01", "sfx_test_tower_place_02" },
                volume: 0.8f, pitchMin: 0.94f, pitchMax: 1.06f, cooldown: 0f, spatialBlend: 0.35f);

            AssetDatabase.SaveAssets();
            return LoadSfxBundle();
        }

        private static SfxBundle LoadSfxBundle()
        {
            return new SfxBundle
            {
                WaveStarted = AssetDatabase.LoadAssetAtPath<SoundEffectSO>($"{SfxDataFolder}/sfx_test_wave_start.asset"),
                WaveCompleted = AssetDatabase.LoadAssetAtPath<SoundEffectSO>($"{SfxDataFolder}/sfx_test_wave_clear.asset"),
                QueenDamaged = AssetDatabase.LoadAssetAtPath<SoundEffectSO>($"{SfxDataFolder}/sfx_test_queen_damage.asset"),
                Victory = AssetDatabase.LoadAssetAtPath<SoundEffectSO>($"{SfxDataFolder}/sfx_test_victory.asset"),
                Defeat = AssetDatabase.LoadAssetAtPath<SoundEffectSO>($"{SfxDataFolder}/sfx_test_defeat.asset"),
                BossAppeared = AssetDatabase.LoadAssetAtPath<SoundEffectSO>($"{SfxDataFolder}/sfx_test_boss_appear.asset"),
                CentipedeSplit = AssetDatabase.LoadAssetAtPath<SoundEffectSO>($"{SfxDataFolder}/sfx_test_centipede_split.asset"),
                TowerPlaced = AssetDatabase.LoadAssetAtPath<SoundEffectSO>($"{SfxDataFolder}/sfx_test_tower_place.asset"),
                CalmMusic = AssetDatabase.LoadAssetAtPath<AudioClip>($"{ClipsFolder}/mus_test_calm.wav"),
                IntenseMusic = AssetDatabase.LoadAssetAtPath<AudioClip>($"{ClipsFolder}/mus_test_intense.wav"),
            };
        }

        private static void EnsureSfxAsset(string assetName, string[] clipNames, float volume,
            float pitchMin, float pitchMax, float cooldown, float spatialBlend)
        {
            string assetPath = $"{SfxDataFolder}/{assetName}.asset";
            SoundEffectSO sfx = AssetDatabase.LoadAssetAtPath<SoundEffectSO>(assetPath);
            if (sfx == null)
            {
                sfx = ScriptableObject.CreateInstance<SoundEffectSO>();
                AssetDatabase.CreateAsset(sfx, assetPath);
            }

            var clips = new List<AudioClip>(clipNames.Length);
            foreach (string clipName in clipNames)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>($"{ClipsFolder}/{clipName}.wav");
                if (clip != null)
                {
                    clips.Add(clip);
                }
            }

            sfx.Clips = clips.ToArray();
            sfx.Volume = volume;
            sfx.PitchRange = new Vector2(pitchMin, pitchMax);
            sfx.Cooldown = cooldown;
            sfx.SpatialBlend = spatialBlend;
            EditorUtility.SetDirty(sfx);
        }

        // ---- Synthesized Placeholder WAV Clips ------------------------------------------------

        private static void EnsureAllTestWavClips()
        {
            WriteWavIfMissing("sfx_test_wave_start_01", 0.36f, t =>
                t < 0.16f ? SineEnv(t, 0.16f, 523.25f) : SineEnv(t - 0.16f, 0.20f, 783.99f));

            WriteWavIfMissing("sfx_test_wave_start_02", 0.36f, t =>
                t < 0.16f ? SineEnv(t, 0.16f, 587.33f) : SineEnv(t - 0.16f, 0.20f, 880.00f));

            WriteWavIfMissing("sfx_test_wave_clear_01", 0.52f, t =>
                Arpeggio(t, new[] { 523.25f, 659.25f, 783.99f, 1046.50f }, 0.12f));

            WriteWavIfMissing("sfx_test_queen_damage_01", 0.26f, t =>
                PitchDrop(t, 0.26f, 190f, 85f, 0.5f));

            WriteWavIfMissing("sfx_test_queen_damage_02", 0.26f, t =>
                PitchDrop(t, 0.26f, 165f, 72f, 0.5f));

            WriteWavIfMissing("sfx_test_victory_01", 0.85f, t =>
                Arpeggio(t, new[] { 523.25f, 659.25f, 783.99f, 1046.50f }, 0.18f));

            WriteWavIfMissing("sfx_test_defeat_01", 0.78f, t =>
                Arpeggio(t, new[] { 392.00f, 311.13f, 261.63f }, 0.24f));

            WriteWavIfMissing("sfx_test_boss_appear_01", 0.70f, t =>
            {
                float env = Envelope(t, 0.70f, 0.08f, 0.25f);
                float tone = 0.55f * Mathf.Sin(2f * Mathf.PI * 110f * t)
                           + 0.30f * Mathf.Sin(2f * Mathf.PI * 164.81f * t)
                           + 0.15f * Mathf.Sin(2f * Mathf.PI * 220f * t);
                return tone * env * 0.65f;
            });

            WriteWavIfMissing("sfx_test_centipede_split_01", 0.24f, t =>
                t < 0.10f ? SineEnv(t, 0.09f, 640f) * 0.7f : SineEnv(t - 0.10f, 0.12f, 920f) * 0.7f);

            WriteWavIfMissing("sfx_test_tower_place_01", 0.15f, t =>
                PitchDrop(t, 0.15f, 340f, 150f, 0.45f));

            WriteWavIfMissing("sfx_test_tower_place_02", 0.15f, t =>
                PitchDrop(t, 0.15f, 380f, 175f, 0.45f));

            // 4-second seamless loopable calm pad (C major 7)
            WriteWavIfMissing("mus_test_calm", 4.0f, t =>
            {
                float pulse = 0.75f + 0.25f * Mathf.Sin(2f * Mathf.PI * 0.5f * t);
                float chord = 0.35f * Mathf.Sin(2f * Mathf.PI * 261.63f * t)
                            + 0.25f * Mathf.Sin(2f * Mathf.PI * 329.63f * t)
                            + 0.25f * Mathf.Sin(2f * Mathf.PI * 392.00f * t)
                            + 0.15f * Mathf.Sin(2f * Mathf.PI * 493.88f * t);
                return chord * pulse * 0.32f;
            });

            // 4-second seamless loopable rhythmic pulse (A minor)
            WriteWavIfMissing("mus_test_intense", 4.0f, t =>
            {
                float beat = (t * 2f) % 1f;
                float beatEnv = Mathf.Exp(-beat * 5f);
                float bass = 0.5f * Mathf.Sin(2f * Mathf.PI * 110f * t) * beatEnv;
                float leadFreq = (Mathf.FloorToInt(t * 2f) % 4) switch
                {
                    0 => 220f,
                    1 => 261.63f,
                    2 => 329.63f,
                    _ => 293.66f,
                };
                float lead = 0.3f * Mathf.Sin(2f * Mathf.PI * leadFreq * t) * (0.4f + 0.6f * beatEnv);
                return (bass + lead) * 0.38f;
            });
        }

        private static void WriteWavIfMissing(string fileName, float durationSeconds, Func<float, float> sampleFunc)
        {
            string assetPath = $"{ClipsFolder}/{fileName}.wav";
            if (File.Exists(assetPath))
            {
                return;
            }

            int totalSamples = Mathf.Max(1, Mathf.RoundToInt(durationSeconds * SampleRate));
            short[] pcm = new short[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SampleRate;
                float value = Mathf.Clamp(sampleFunc(t), -1f, 1f);
                pcm[i] = (short)Mathf.RoundToInt(value * 32767f);
            }

            using (var stream = new FileStream(assetPath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(stream))
            {
                int dataBytes = totalSamples * 2;
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + dataBytes);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
                writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16); // PCM chunk size
                writer.Write((short)1); // PCM format
                writer.Write((short)1); // Mono
                writer.Write(SampleRate);
                writer.Write(SampleRate * 2); // Byte rate
                writer.Write((short)2); // Block align
                writer.Write((short)16); // Bits per sample
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                writer.Write(dataBytes);

                for (int i = 0; i < totalSamples; i++)
                {
                    writer.Write(pcm[i]);
                }
            }
        }

        private static float SineEnv(float t, float duration, float freq)
        {
            if (t < 0f || t > duration) return 0f;
            return Mathf.Sin(2f * Mathf.PI * freq * t) * Envelope(t, duration, 0.01f, 0.06f) * 0.6f;
        }

        private static float PitchDrop(float t, float duration, float startFreq, float endFreq, float amp)
        {
            if (t < 0f || t > duration) return 0f;
            float u = t / duration;
            float freq = Mathf.Lerp(startFreq, endFreq, u);
            return Mathf.Sin(2f * Mathf.PI * freq * t) * Envelope(t, duration, 0.005f, duration * 0.7f) * amp;
        }

        private static float Arpeggio(float t, float[] notes, float step)
        {
            int idx = Mathf.FloorToInt(t / step);
            if (idx < 0 || idx >= notes.Length) return 0f;
            float localT = t - idx * step;
            float dur = idx == notes.Length - 1 ? step * 1.8f : step;
            return SineEnv(localT, dur, notes[idx]);
        }

        private static float Envelope(float t, float duration, float attack, float release)
        {
            float a = attack > 0f ? Mathf.Clamp01(t / attack) : 1f;
            float r = release > 0f ? Mathf.Clamp01((duration - t) / release) : 1f;
            return Mathf.Min(a, r);
        }

        // ---- Helpers --------------------------------------------------------------------------

        private static T FindInScene<T>(Scene scene) where T : Component
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                T found = root.GetComponentInChildren<T>(true);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }
    }
}

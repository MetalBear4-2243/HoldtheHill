using HoldTheHill.Core.Audio;
using HoldTheHill.Features.GameFlow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Editor builder tool that automates scene setup for the audio architecture:
    /// 1. Ensures <see cref="AudioManager"/> and an <see cref="AudioListener"/> exist in Bootstrap.unity.
    /// 2. Auto-wires available AudioMixer assets and default buses (SFX, Music).
    /// 3. Ensures <see cref="LevelAudioDirector"/> is configured on the Level Run object across all level scenes.
    /// 4. Auto-binds any matching <see cref="SoundEffectSO"/> assets found in the project.
    /// Leaves the Bootstrap scene open, ready for Play mode.
    /// </summary>
    /// <remarks>Safe to run repeatedly: preserves existing assignments and adds only missing elements.</remarks>
    internal static class AlphaAudioSetupBuilder
    {
        private const string BootstrapScenePath = "Assets/_Game/Scenes/Bootstrap.unity";
        private const string AudioManagerName = "Audio Manager";

        private static readonly string[] LevelGameplayScenes =
        {
            "Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity",
            "Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity",
            "Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity",
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Setup Audio (Manager and Directors)")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            SetupBootstrapAudio();
            SetupLevelDirectors();
            ReportAndReturnToBootstrap();
        }

        /// <summary>Ensures the Bootstrap scene contains an AudioManager and an active AudioListener.</summary>
        private static void SetupBootstrapAudio()
        {
            Scene bootstrap = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);

            AudioManager manager = AlphaRunFrameBuilder.FindInScene<AudioManager>(bootstrap);
            if (manager == null)
            {
                var go = new GameObject(AudioManagerName);
                manager = go.AddComponent<AudioManager>();
                Debug.Log($"Created '{AudioManagerName}' in {bootstrap.name}.");
            }

            // Ensure AudioListener is present in the persistent scene
            AudioListener listener = AlphaRunFrameBuilder.FindInScene<AudioListener>(bootstrap);
            if (listener == null)
            {
                Camera camera = AlphaRunFrameBuilder.FindInScene<Camera>(bootstrap);
                if (camera != null)
                {
                    camera.gameObject.AddComponent<AudioListener>();
                    Debug.Log("Added AudioListener to Bootstrap camera.");
                }
                else
                {
                    manager.gameObject.AddComponent<AudioListener>();
                    Debug.Log("Added AudioListener to Audio Manager.");
                }
            }

            // Auto-wire AudioMixer if present in project
            string[] mixerGuids = AssetDatabase.FindAssets("t:AudioMixer");
            if (mixerGuids.Length > 0)
            {
                string mixerPath = AssetDatabase.GUIDToAssetPath(mixerGuids[0]);
                var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(mixerPath);
                if (mixer != null)
                {
                    var so = new SerializedObject(manager);
                    if (so.FindProperty("_mixer").objectReferenceValue == null)
                    {
                        so.FindProperty("_mixer").objectReferenceValue = mixer;

                        AudioMixerGroup[] sfxGroups = mixer.FindMatchingGroups("SFX");
                        if (sfxGroups.Length > 0)
                        {
                            so.FindProperty("_sfxGroup").objectReferenceValue = sfxGroups[0];
                        }

                        AudioMixerGroup[] musicGroups = mixer.FindMatchingGroups("Music");
                        if (musicGroups.Length > 0)
                        {
                            so.FindProperty("_musicGroup").objectReferenceValue = musicGroups[0];
                        }

                        so.ApplyModifiedPropertiesWithoutUndo();
                        Debug.Log($"Auto-wired AudioMixer '{mixer.name}' to AudioManager.");
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(bootstrap);
            EditorSceneManager.SaveScene(bootstrap);
        }

        /// <summary>Ensures each level gameplay scene has a configured LevelAudioDirector on its Level Run.</summary>
        private static void SetupLevelDirectors()
        {
            foreach (string scenePath in LevelGameplayScenes)
            {
                if (!System.IO.File.Exists(scenePath))
                {
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                LevelRun run = AlphaRunFrameBuilder.FindInScene<LevelRun>(scene);
                LevelAudioDirector director = AlphaRunFrameBuilder.FindInScene<LevelAudioDirector>(scene);

                if (director == null)
                {
                    GameObject host = run != null ? run.gameObject : new GameObject("Level Audio Director");
                    director = host.AddComponent<LevelAudioDirector>();
                    Debug.Log($"Added LevelAudioDirector to '{scene.name}' on '{host.name}'.");
                }

                var so = new SerializedObject(director);
                if (run != null && so.FindProperty("_run").objectReferenceValue == null)
                {
                    so.FindProperty("_run").objectReferenceValue = run;
                }

                AutoWireMatchingSoundEffects(so);
                so.ApplyModifiedPropertiesWithoutUndo();

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        /// <summary>Scans the project for SoundEffectSO assets and auto-assigns matching fields if unassigned.</summary>
        private static void AutoWireMatchingSoundEffects(SerializedObject directorSo)
        {
            string[] sfxGuids = AssetDatabase.FindAssets("t:SoundEffectSO");
            foreach (string guid in sfxGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var sfx = AssetDatabase.LoadAssetAtPath<SoundEffectSO>(path);
                if (sfx == null)
                {
                    continue;
                }

                string name = sfx.name.ToLowerInvariant();

                TryAssignSfx(directorSo, "_waveStartedSfx", sfx, name.Contains("wave_start") || name.Contains("wavestart"));
                TryAssignSfx(directorSo, "_waveCompletedSfx", sfx, name.Contains("wave_clear") || name.Contains("waveclear") || name.Contains("wave_complete"));
                TryAssignSfx(directorSo, "_queenDamagedSfx", sfx, name.Contains("queen_damage") || name.Contains("queen_hurt") || name.Contains("queendamage"));
                TryAssignSfx(directorSo, "_victorySfx", sfx, name.Contains("victory") || name.Contains("win"));
                TryAssignSfx(directorSo, "_defeatSfx", sfx, name.Contains("defeat") || name.Contains("lose") || name.Contains("game_over"));
                TryAssignSfx(directorSo, "_bossAppearedSfx", sfx, name.Contains("boss") || name.Contains("mantis"));
                TryAssignSfx(directorSo, "_centipedeSplitSfx", sfx, name.Contains("centipede") || name.Contains("split"));
                TryAssignSfx(directorSo, "_towerPlacedSfx", sfx, name.Contains("tower_place") || name.Contains("towerplace") || name.Contains("place_tower"));
            }
        }

        private static void TryAssignSfx(SerializedObject so, string propertyName, SoundEffectSO sfx, bool condition)
        {
            if (!condition)
            {
                return;
            }

            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null && prop.objectReferenceValue == null)
            {
                prop.objectReferenceValue = sfx;
                Debug.Log($"Auto-assigned '{sfx.name}' to {propertyName}.");
            }
        }

        /// <summary>Verifies audio components across scenes and leaves Bootstrap open for Play mode.</summary>
        private static void ReportAndReturnToBootstrap()
        {
            Scene bootstrap = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            AudioManager manager = AlphaRunFrameBuilder.FindInScene<AudioManager>(bootstrap);
            AudioListener listener = AlphaRunFrameBuilder.FindInScene<AudioListener>(bootstrap);

            string report = $"Audio Setup Complete: Bootstrap AudioManager = {(manager != null ? "yes" : "MISSING")}, "
                          + $"AudioListener = {(listener != null ? "yes" : "MISSING")}";

            foreach (string path in LevelGameplayScenes)
            {
                if (!System.IO.File.Exists(path))
                {
                    continue;
                }

                Scene level = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                LevelAudioDirector director = AlphaRunFrameBuilder.FindInScene<LevelAudioDirector>(level);
                report += $", {level.name} Director = {(director != null ? "yes" : "MISSING")}";
            }

            // Return to Bootstrap as the active scene
            EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            Debug.Log(report);
        }
    }
}

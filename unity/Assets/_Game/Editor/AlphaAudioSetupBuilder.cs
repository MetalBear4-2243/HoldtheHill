using System;
using System.Collections.Generic;
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
    /// 2. Auto-wires <c>MainMixer.mixer</c> and default buses (<c>SFX</c>, <c>Music</c>) and validates exposed parameters.
    /// 3. Non-destructively routes shared <see cref="SoundEffectSO"/> assets under <c>Shared/Audio/SFX</c> to the <c>SFX</c> group.
    /// 4. Ensures <see cref="LevelAudioDirector"/> is configured on the Level Run object across all level scenes.
    /// 5. Auto-binds any matching <see cref="SoundEffectSO"/> assets found in the project.
    /// Leaves the Bootstrap scene open, ready for Play mode.
    /// </summary>
    /// <remarks>Safe to run repeatedly: preserves existing assignments and adds only missing elements.</remarks>
    internal static class AlphaAudioSetupBuilder
    {
        internal const string MainMixerPath = "Assets/_Game/Shared/Audio/MainMixer.mixer";
        private const string SharedSfxFolder = "Assets/_Game/Shared/Audio/SFX";
        private const string BootstrapScenePath = "Assets/_Game/Scenes/Bootstrap.unity";
        private const string AudioManagerName = "Audio Manager";
        private const string MasterGroupName = "Master";
        private const string SfxGroupName = "SFX";
        private const string MusicGroupName = "Music";

        private static readonly string[] LevelGameplayScenes =
        {
            "Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity",
            "Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity",
            "Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity",
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Setup Audio (Manager and Directors)")]
        public static void Build()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            AudioMixer mixer = SetupBootstrapAudio();
            WireSharedSoundEffectsMixerGroups(mixer);
            SetupLevelDirectors();
            ReportAndReturnToBootstrap();
        }

        /// <summary>Ensures the Bootstrap scene contains an AudioManager and an active AudioListener.</summary>
        private static AudioMixer SetupBootstrapAudio()
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

            // Auto-wire MainMixer and default buses independently without overwriting existing assignments
            AudioMixer foundMixer = FindProjectMixer();
            var so = new SerializedObject(manager);
            SerializedProperty mixerProp = so.FindProperty("_mixer");
            SerializedProperty sfxGroupProp = so.FindProperty("_sfxGroup");
            SerializedProperty musicGroupProp = so.FindProperty("_musicGroup");

            bool changed = false;
            if (mixerProp.objectReferenceValue == null && foundMixer != null)
            {
                mixerProp.objectReferenceValue = foundMixer;
                changed = true;
                Debug.Log($"Auto-wired AudioMixer '{foundMixer.name}' to AudioManager.");
            }

            AudioMixer targetMixer = mixerProp.objectReferenceValue as AudioMixer ?? foundMixer;
            if (targetMixer != null)
            {
                if (sfxGroupProp.objectReferenceValue == null)
                {
                    AudioMixerGroup sfxGroup = FindExactGroup(targetMixer, SfxGroupName);
                    if (sfxGroup != null)
                    {
                        sfxGroupProp.objectReferenceValue = sfxGroup;
                        changed = true;
                        Debug.Log($"Auto-wired SFX mixer group '{sfxGroup.name}' to AudioManager.");
                    }
                }

                if (musicGroupProp.objectReferenceValue == null)
                {
                    AudioMixerGroup musicGroup = FindExactGroup(targetMixer, MusicGroupName);
                    if (musicGroup != null)
                    {
                        musicGroupProp.objectReferenceValue = musicGroup;
                        changed = true;
                        Debug.Log($"Auto-wired Music mixer group '{musicGroup.name}' to AudioManager.");
                    }
                }

                if (changed)
                {
                    so.ApplyModifiedPropertiesWithoutUndo();
                }

                ValidateMixerConfiguration(targetMixer);
            }

            EditorSceneManager.MarkSceneDirty(bootstrap);
            EditorSceneManager.SaveScene(bootstrap);
            return targetMixer;
        }

        private static AudioMixer FindProjectMixer()
        {
            var mainMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MainMixerPath);
            if (mainMixer != null)
            {
                return mainMixer;
            }

            string[] mixerGuids = AssetDatabase.FindAssets("t:AudioMixer", new[] { "Assets/_Game" });
            AudioMixer fallback = null;
            for (int i = 0; i < mixerGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(mixerGuids[i]);
                var candidate = AssetDatabase.LoadAssetAtPath<AudioMixer>(path);
                if (candidate == null)
                {
                    continue;
                }

                if (string.Equals(candidate.name, "MainMixer", StringComparison.OrdinalIgnoreCase))
                {
                    return candidate;
                }

                if (fallback == null)
                {
                    fallback = candidate;
                }
            }

            if (fallback == null)
            {
                Debug.LogWarning($"No AudioMixer found at '{MainMixerPath}' or under Assets/_Game.");
            }

            return fallback;
        }

        private static AudioMixerGroup FindExactGroup(AudioMixer mixer, string groupName)
        {
            if (mixer == null || string.IsNullOrEmpty(groupName))
            {
                return null;
            }

            AudioMixerGroup[] groups = mixer.FindMatchingGroups(groupName);
            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] != null && string.Equals(groups[i].name, groupName, StringComparison.Ordinal))
                {
                    return groups[i];
                }
            }

            return groups.Length > 0 ? groups[0] : null;
        }

        private static bool HasExactGroup(AudioMixer mixer, string groupName)
        {
            if (mixer == null || string.IsNullOrEmpty(groupName))
            {
                return false;
            }

            AudioMixerGroup[] groups = mixer.FindMatchingGroups(groupName);
            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] != null && string.Equals(groups[i].name, groupName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static HashSet<string> GetExposedParameterNames(AudioMixer mixer)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            if (mixer == null)
            {
                return names;
            }

            var mixerSo = new SerializedObject(mixer);
            SerializedProperty exposedParams = mixerSo.FindProperty("m_ExposedParameters");
            if (exposedParams != null && exposedParams.isArray)
            {
                for (int i = 0; i < exposedParams.arraySize; i++)
                {
                    SerializedProperty element = exposedParams.GetArrayElementAtIndex(i);
                    string paramName = element?.FindPropertyRelative("name")?.stringValue;
                    if (!string.IsNullOrEmpty(paramName))
                    {
                        names.Add(paramName);
                    }
                }
            }

            return names;
        }

        private static string ValidateMixerConfiguration(AudioMixer mixer, bool logErrors = true)
        {
            if (mixer == null)
            {
                return "no mixer";
            }

            var missingGroups = new List<string>();
            string[] expectedGroups = { MasterGroupName, SfxGroupName, MusicGroupName };
            for (int i = 0; i < expectedGroups.Length; i++)
            {
                if (!HasExactGroup(mixer, expectedGroups[i]))
                {
                    missingGroups.Add(expectedGroups[i]);
                }
            }

            HashSet<string> exposedNames = GetExposedParameterNames(mixer);
            var missingParams = new List<string>();
            string[] expectedParams =
            {
                AudioManager.MasterVolumeParam,
                AudioManager.SfxVolumeParam,
                AudioManager.MusicVolumeParam,
            };

            for (int i = 0; i < expectedParams.Length; i++)
            {
                string param = expectedParams[i];
                if (!exposedNames.Contains(param) && !mixer.GetFloat(param, out _))
                {
                    missingParams.Add(param);
                }
            }

            if (missingGroups.Count == 0 && missingParams.Count == 0)
            {
                return "valid (Master/SFX/Music groups & MasterVolume/SfxVolume/MusicVolume params)";
            }

            var issues = new List<string>();
            if (missingGroups.Count > 0)
            {
                issues.Add($"missing groups: {string.Join(", ", missingGroups)}");
            }
            if (missingParams.Count > 0)
            {
                issues.Add($"missing exposed parameters: {string.Join(", ", missingParams)}");
            }

            string summary = string.Join("; ", issues);
            if (logErrors)
            {
                Debug.LogError($"AudioMixer '{mixer.name}' validation failed ({summary}). Expose MasterVolume, SfxVolume, and MusicVolume on Master, SFX, and Music in the Audio Mixer window.");
            }
            return summary;
        }

        private static void WireSharedSoundEffectsMixerGroups(AudioMixer mixer)
        {
            if (mixer == null || !AssetDatabase.IsValidFolder(SharedSfxFolder))
            {
                return;
            }

            AudioMixerGroup sfxGroup = FindExactGroup(mixer, SfxGroupName);
            if (sfxGroup == null)
            {
                return;
            }

            string[] sfxGuids = AssetDatabase.FindAssets("t:SoundEffectSO", new[] { SharedSfxFolder });
            int updatedCount = 0;

            for (int i = 0; i < sfxGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(sfxGuids[i]);
                var sfx = AssetDatabase.LoadAssetAtPath<SoundEffectSO>(path);
                if (sfx == null)
                {
                    continue;
                }

                var sfxSo = new SerializedObject(sfx);
                SerializedProperty groupProp = sfxSo.FindProperty("_mixerGroup");
                if (groupProp != null && groupProp.objectReferenceValue == null)
                {
                    groupProp.objectReferenceValue = sfxGroup;
                    sfxSo.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(sfx);
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                AssetDatabase.SaveAssets();
                Debug.Log($"Assigned '{sfxGroup.name}' AudioMixerGroup to {updatedCount} shared SoundEffectSO asset(s) in {SharedSfxFolder}.");
            }
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
                TryAssignSfx(directorSo, "_enemyDeathSfx", sfx, name.Contains("enemy_death") || name.Contains("enemydeath") || name.Contains("enemy_die") || name.Contains("enemydie"));
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

            string mixerReport = "MISSING";
            string sfxGroupReport = "MISSING";
            string musicGroupReport = "MISSING";
            string mixerValidation = "not checked";

            if (manager != null)
            {
                var so = new SerializedObject(manager);
                var mixer = so.FindProperty("_mixer").objectReferenceValue as AudioMixer;
                var sfxGroup = so.FindProperty("_sfxGroup").objectReferenceValue as AudioMixerGroup;
                var musicGroup = so.FindProperty("_musicGroup").objectReferenceValue as AudioMixerGroup;

                mixerReport = mixer != null ? mixer.name : "MISSING";
                sfxGroupReport = sfxGroup != null ? sfxGroup.name : "MISSING";
                musicGroupReport = musicGroup != null ? musicGroup.name : "MISSING";
                mixerValidation = ValidateMixerConfiguration(mixer, logErrors: false);
            }

            string report = $"Audio Setup Complete: Bootstrap AudioManager = {(manager != null ? "yes" : "MISSING")}, "
                          + $"AudioListener = {(listener != null ? "yes" : "MISSING")}, "
                          + $"Mixer = {mixerReport}, SFX Group = {sfxGroupReport}, Music Group = {musicGroupReport}, "
                          + $"Mixer Validation = {mixerValidation}";

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

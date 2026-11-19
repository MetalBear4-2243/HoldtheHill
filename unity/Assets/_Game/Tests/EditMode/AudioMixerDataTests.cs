using System;
using System.Collections.Generic;
using HoldTheHill.Core.Audio;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the MainMixer asset, its bus layout and exposed parameters, Bootstrap AudioManager wiring,
    /// and shared SoundEffectSO routing.
    /// </summary>
    public class AudioMixerDataTests
    {
        private const string MainMixerPath = "Assets/_Game/Shared/Audio/MainMixer.mixer";
        private const string BootstrapScenePath = "Assets/_Game/Scenes/Bootstrap.unity";
        private const string SharedSfxFolder = "Assets/_Game/Shared/Audio/SFX";

        [Test]
        public void MainMixerAsset_ExistsAtExpectedPath()
        {
            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MainMixerPath);
            Assert.That(mixer, Is.Not.Null, $"Expected MainMixer.mixer at {MainMixerPath}.");
        }

        [Test]
        public void MainMixerAsset_HasThreeExpectedGroups_MasterSfxMusic()
        {
            AudioMixer mixer = LoadRequiredMainMixer();
            AudioMixerGroup[] allGroups = mixer.FindMatchingGroups(string.Empty);

            Assert.That(allGroups.Length, Is.EqualTo(3), "MainMixer should have exactly 3 groups: Master, SFX, Music.");
            Assert.That(FindExactGroup(mixer, "Master"), Is.Not.Null, "MainMixer is missing the Master group.");
            Assert.That(FindExactGroup(mixer, "SFX"), Is.Not.Null, "MainMixer is missing the SFX group.");
            Assert.That(FindExactGroup(mixer, "Music"), Is.Not.Null, "MainMixer is missing the Music group.");
        }

        [Test]
        public void MainMixerAsset_ExposesRequiredVolumeParameters()
        {
            AudioMixer mixer = LoadRequiredMainMixer();
            HashSet<string> exposedNames = GetExposedParameterNames(mixer);

            string[] requiredParams =
            {
                AudioManager.MasterVolumeParam,
                AudioManager.SfxVolumeParam,
                AudioManager.MusicVolumeParam,
            };

            foreach (string param in requiredParams)
            {
                bool exposed = exposedNames.Contains(param) || mixer.GetFloat(param, out _);
                Assert.That(exposed, Is.True, $"MainMixer must expose volume parameter '{param}'.");
            }
        }

        [Test]
        public void BootstrapScene_AudioManager_HasMixerAndGroupsAssigned()
        {
            AudioMixer mainMixer = LoadRequiredMainMixer();
            AudioMixerGroup expectedSfx = FindExactGroup(mainMixer, "SFX");
            AudioMixerGroup expectedMusic = FindExactGroup(mainMixer, "Music");

            Scene bootstrap = SceneManager.GetSceneByPath(BootstrapScenePath);
            bool openedByTest = false;
            if (!bootstrap.isLoaded)
            {
                bootstrap = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Additive);
                openedByTest = true;
            }

            try
            {
                AudioManager manager = FindInScene<AudioManager>(bootstrap);
                Assert.That(manager, Is.Not.Null, "Bootstrap scene should contain an AudioManager.");

                var so = new SerializedObject(manager);
                var assignedMixer = so.FindProperty("_mixer").objectReferenceValue as AudioMixer;
                var assignedSfx = so.FindProperty("_sfxGroup").objectReferenceValue as AudioMixerGroup;
                var assignedMusic = so.FindProperty("_musicGroup").objectReferenceValue as AudioMixerGroup;

                Assert.That(assignedMixer, Is.SameAs(mainMixer), "Bootstrap AudioManager._mixer should reference MainMixer. Run Tools > Hold the Hill > Alpha > Setup Audio.");
                Assert.That(assignedSfx, Is.SameAs(expectedSfx), "Bootstrap AudioManager._sfxGroup should reference MainMixer/SFX.");
                Assert.That(assignedMusic, Is.SameAs(expectedMusic), "Bootstrap AudioManager._musicGroup should reference MainMixer/Music.");
            }
            finally
            {
                if (openedByTest && bootstrap.isLoaded)
                {
                    EditorSceneManager.CloseScene(bootstrap, removeScene: true);
                }
            }
        }

        [Test]
        public void SharedSoundEffectAssets_RouteToSfxGroup()
        {
            AudioMixer mainMixer = LoadRequiredMainMixer();
            AudioMixerGroup expectedSfx = FindExactGroup(mainMixer, "SFX");
            Assert.That(expectedSfx, Is.Not.Null, "MainMixer is missing the SFX group.");

            string[] guids = AssetDatabase.FindAssets("t:SoundEffectSO", new[] { SharedSfxFolder });
            Assert.That(guids, Is.Not.Empty, $"Expected SoundEffectSO assets in {SharedSfxFolder}.");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var sfx = AssetDatabase.LoadAssetAtPath<SoundEffectSO>(path);
                Assert.That(sfx, Is.Not.Null, $"Failed to load SoundEffectSO at {path}.");
                Assert.That(sfx.MixerGroup, Is.SameAs(expectedSfx),
                    $"{sfx.name} should route to MainMixer/SFX. Run Tools > Hold the Hill > Alpha > Setup Audio.");
            }
        }

        private static AudioMixer LoadRequiredMainMixer()
        {
            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MainMixerPath);
            Assert.That(mixer, Is.Not.Null, $"Expected MainMixer.mixer at {MainMixerPath}.");
            return mixer;
        }

        private static AudioMixerGroup FindExactGroup(AudioMixer mixer, string groupName)
        {
            AudioMixerGroup[] matches = mixer.FindMatchingGroups(groupName);
            for (int i = 0; i < matches.Length; i++)
            {
                if (matches[i] != null && string.Equals(matches[i].name, groupName, StringComparison.Ordinal))
                {
                    return matches[i];
                }
            }
            return null;
        }

        private static HashSet<string> GetExposedParameterNames(AudioMixer mixer)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
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
    }
}

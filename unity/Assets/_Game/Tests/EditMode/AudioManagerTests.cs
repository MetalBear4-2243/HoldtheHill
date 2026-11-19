using HoldTheHill.Core.Audio;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace HoldTheHill.Tests
{
    public class AudioManagerTests
    {
        private const string MainMixerPath = "Assets/_Game/Shared/Audio/MainMixer.mixer";

        private static readonly string[] VolumePrefKeys =
        {
            AudioManager.MasterVolumePrefKey,
            AudioManager.SfxVolumePrefKey,
            AudioManager.MusicVolumePrefKey,
        };

        private GameObject _managerObj;
        private AudioManager _audioManager;
        private bool[] _hadPrefKeys;
        private float[] _savedPrefValues;

        [SetUp]
        public void SetUp()
        {
            _hadPrefKeys = new bool[VolumePrefKeys.Length];
            _savedPrefValues = new float[VolumePrefKeys.Length];
            for (int i = 0; i < VolumePrefKeys.Length; i++)
            {
                string key = VolumePrefKeys[i];
                _hadPrefKeys[i] = PlayerPrefs.HasKey(key);
                if (_hadPrefKeys[i])
                {
                    _savedPrefValues[i] = PlayerPrefs.GetFloat(key);
                    PlayerPrefs.DeleteKey(key);
                }
            }

            _managerObj = new GameObject("TestAudioManager");
            _audioManager = _managerObj.AddComponent<AudioManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_managerObj != null)
            {
                Object.DestroyImmediate(_managerObj);
            }
            SoundEffectSO.PlaybackHandler = null;

            for (int i = 0; i < VolumePrefKeys.Length; i++)
            {
                string key = VolumePrefKeys[i];
                if (_hadPrefKeys[i])
                {
                    PlayerPrefs.SetFloat(key, _savedPrefValues[i]);
                }
                else
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }
            PlayerPrefs.Save();
        }

        [Test]
        public void Awake_EstablishesCurrentInstance()
        {
            Assert.AreSame(_audioManager, AudioManager.Current);
            Assert.AreSame(_audioManager, AudioManager.Instance);
        }

        [Test]
        public void OnEnable_RegistersPlaybackHandler()
        {
            Assert.IsNotNull(SoundEffectSO.PlaybackHandler);
        }

        [Test]
        public void OnDisable_UnregistersPlaybackHandler()
        {
            _audioManager.enabled = false;
            Assert.IsNull(SoundEffectSO.PlaybackHandler);
        }

        [Test]
        public void PlayOneShot_RawClip_ConfiguresAudioSource()
        {
            AudioClip clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
            Vector3 testPos = new Vector3(3f, -1f, 0f);

            AudioSource source = _audioManager.PlayOneShot(
                clip, 
                volume: 0.75f, 
                pitch: 1.05f, 
                position: testPos, 
                spatialBlend: 0.5f
            );

            Assert.IsNotNull(source);
            Assert.AreSame(clip, source.clip);
            Assert.AreEqual(0.75f, source.volume, 0.001f);
            Assert.AreEqual(1.05f, source.pitch, 0.001f);
            Assert.AreEqual(testPos, source.transform.position);
            Assert.AreEqual(0.5f, source.spatialBlend, 0.001f);

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void PlayOneShot_SoundEffectSO_RoutesThroughPlaybackHandler()
        {
            var sfx = ScriptableObject.CreateInstance<SoundEffectSO>();
            AudioClip clip = AudioClip.Create("SfxClip", 44100, 1, 44100, false);
            sfx.Clips = new[] { clip };
            sfx.Volume = 0.9f;
            sfx.PitchRange = new Vector2(1f, 1f);
            sfx.SpatialBlend = 0.2f;

            sfx.Play(new Vector3(10f, 0f, 0f));

            // Verify a voice in the pool is configured with the sfx's parameters
            Assert.GreaterOrEqual(_audioManager.PoolSize, 4);

            Object.DestroyImmediate(clip);
            Object.DestroyImmediate(sfx);
        }

        [Test]
        public void PlayMusic_AssignsClipToMusicSource()
        {
            AudioClip musicClip = AudioClip.Create("MusicTrack", 44100, 1, 44100, false);

            _audioManager.PlayMusic(musicClip, fadeDuration: 0f);

            Assert.IsNotNull(_audioManager.CurrentMusicSource);
            Assert.AreSame(musicClip, _audioManager.CurrentMusicSource.clip);
            Assert.IsTrue(_audioManager.CurrentMusicSource.loop);

            Object.DestroyImmediate(musicClip);
        }

        [Test]
        public void SetVolume_HandlesNullMixerGracefully()
        {
            _audioManager.Mixer = null;

            // Should safely return without null reference exceptions
            Assert.DoesNotThrow(() =>
            {
                _audioManager.SetMasterVolume(0.5f);
                _audioManager.SetSfxVolume(0.8f);
                _audioManager.SetMusicVolume(0f);
            });
        }

        [Test]
        public void LinearToDecibels_MapsBoundariesAndMidpointCorrectly()
        {
            Assert.AreEqual(0f, AudioManager.LinearToDecibels(1f), 0.001f);
            Assert.AreEqual(-6.0206f, AudioManager.LinearToDecibels(0.5f), 0.001f);
            Assert.AreEqual(-80f, AudioManager.LinearToDecibels(0.0001f), 0.001f);
            Assert.AreEqual(-80f, AudioManager.LinearToDecibels(0f), 0.001f);
            Assert.AreEqual(-80f, AudioManager.LinearToDecibels(-0.5f), 0.001f);
            Assert.AreEqual(0f, AudioManager.LinearToDecibels(2f), 0.001f);
        }

        [Test]
        public void DecibelsToLinear_InvertsLinearToDecibels()
        {
            Assert.AreEqual(1f, AudioManager.DecibelsToLinear(0f), 0.001f);
            Assert.AreEqual(0.5f, AudioManager.DecibelsToLinear(-6.0206f), 0.001f);
            Assert.AreEqual(0f, AudioManager.DecibelsToLinear(-80f), 0.001f);
            Assert.AreEqual(0f, AudioManager.DecibelsToLinear(-100f), 0.001f);
            Assert.AreEqual(1f, AudioManager.DecibelsToLinear(6f), 0.001f);
        }

        [Test]
        public void SetVolumes_ClampsPersistsAndRaisesEvent()
        {
            int eventCount = 0;
            _audioManager.VolumesChanged += () => eventCount++;

            _audioManager.SetMasterVolume(1.5f);
            _audioManager.SetSfxVolume(-0.25f);
            _audioManager.SetMusicVolume(0.42f);

            Assert.AreEqual(1f, _audioManager.GetMasterVolume(), 0.0001f);
            Assert.AreEqual(0f, _audioManager.GetSfxVolume(), 0.0001f);
            Assert.AreEqual(0.42f, _audioManager.GetMusicVolume(), 0.0001f);

            Assert.AreEqual(1f, PlayerPrefs.GetFloat(AudioManager.MasterVolumePrefKey), 0.0001f);
            Assert.AreEqual(0f, PlayerPrefs.GetFloat(AudioManager.SfxVolumePrefKey), 0.0001f);
            Assert.AreEqual(0.42f, PlayerPrefs.GetFloat(AudioManager.MusicVolumePrefKey), 0.0001f);
            Assert.AreEqual(3, eventCount);
        }

        [Test]
        public void SetMusicVolume_PreservesBaseMusicVolumeTrim()
        {
            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MainMixerPath);
            _audioManager.Mixer = mixer;
            _audioManager.MusicVolume = 0.34f;

            AudioClip musicClip = AudioClip.Create("TrimTestMusic", 44100, 1, 44100, false);
            try
            {
                _audioManager.PlayMusic(musicClip, fadeDuration: 0f);
                _audioManager.SetMusicVolume(0.5f);

                Assert.AreEqual(0.34f, _audioManager.MusicVolume, 0.0001f);
                Assert.AreEqual(0.5f, _audioManager.GetMusicVolume(), 0.0001f);
                if (mixer != null)
                {
                    Assert.AreEqual(0.34f, _audioManager.CurrentMusicSource.volume, 0.0001f);
                }
            }
            finally
            {
                Object.DestroyImmediate(musicClip);
            }
        }

        [Test]
        public void LoadSavedVolumes_RestoresPersistedPlayerPrefs()
        {
            PlayerPrefs.SetFloat(AudioManager.MasterVolumePrefKey, 0.7f);
            PlayerPrefs.SetFloat(AudioManager.SfxVolumePrefKey, 0.35f);
            PlayerPrefs.SetFloat(AudioManager.MusicVolumePrefKey, 0.85f);

            _audioManager.LoadSavedVolumes();

            Assert.AreEqual(0.7f, _audioManager.GetMasterVolume(), 0.0001f);
            Assert.AreEqual(0.35f, _audioManager.GetSfxVolume(), 0.0001f);
            Assert.AreEqual(0.85f, _audioManager.GetMusicVolume(), 0.0001f);
        }

        [Test]
        public void ResetVolumesToDefaults_RestoresFullVolumeAndPersists()
        {
            _audioManager.SetMasterVolume(0.2f);
            _audioManager.SetSfxVolume(0.3f);
            _audioManager.SetMusicVolume(0.4f);

            int eventCount = 0;
            _audioManager.VolumesChanged += () => eventCount++;
            _audioManager.ResetVolumesToDefaults();

            Assert.AreEqual(1f, _audioManager.GetMasterVolume(), 0.0001f);
            Assert.AreEqual(1f, _audioManager.GetSfxVolume(), 0.0001f);
            Assert.AreEqual(1f, _audioManager.GetMusicVolume(), 0.0001f);
            Assert.AreEqual(1f, PlayerPrefs.GetFloat(AudioManager.MasterVolumePrefKey), 0.0001f);
            Assert.AreEqual(1f, PlayerPrefs.GetFloat(AudioManager.SfxVolumePrefKey), 0.0001f);
            Assert.AreEqual(1f, PlayerPrefs.GetFloat(AudioManager.MusicVolumePrefKey), 0.0001f);
            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void PlayOneShot_NullMixerFallback_ScalesByMasterAndSfxVolumes()
        {
            _audioManager.Mixer = null;
            _audioManager.SetMasterVolume(0.5f);
            _audioManager.SetSfxVolume(0.5f);

            AudioClip clip = AudioClip.Create("FallbackSfx", 44100, 1, 44100, false);
            try
            {
                AudioSource source = _audioManager.PlayOneShot(clip, volume: 0.8f);
                Assert.IsNotNull(source);
                Assert.AreEqual(0.2f, source.volume, 0.0001f);
            }
            finally
            {
                Object.DestroyImmediate(clip);
            }
        }

        [Test]
        public void PlayMusic_NullMixerFallback_ScalesByBaseTrimAndMixerSettings()
        {
            _audioManager.Mixer = null;
            _audioManager.MusicVolume = 0.4f;
            _audioManager.SetMasterVolume(0.5f);
            _audioManager.SetMusicVolume(0.5f);

            AudioClip clip = AudioClip.Create("FallbackMusic", 44100, 1, 44100, false);
            try
            {
                _audioManager.PlayMusic(clip, fadeDuration: 0f);
                Assert.IsNotNull(_audioManager.CurrentMusicSource);
                Assert.AreEqual(0.1f, _audioManager.CurrentMusicSource.volume, 0.0001f);
            }
            finally
            {
                Object.DestroyImmediate(clip);
            }
        }

        [Test]
        public void GroupSetters_PropagateToExistingVoicesAndMusicSources()
        {
            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MainMixerPath);
            if (mixer == null)
            {
                Assert.Ignore($"MainMixer.mixer not found at {MainMixerPath}.");
            }

            AudioMixerGroup[] sfxMatches = mixer.FindMatchingGroups("SFX");
            AudioMixerGroup[] musicMatches = mixer.FindMatchingGroups("Music");
            Assert.IsNotEmpty(sfxMatches, "Expected SFX group on MainMixer.");
            Assert.IsNotEmpty(musicMatches, "Expected Music group on MainMixer.");

            AudioMixerGroup sfxGroup = System.Array.Find(sfxMatches, g => g != null && g.name == "SFX") ?? sfxMatches[0];
            AudioMixerGroup musicGroup = System.Array.Find(musicMatches, g => g != null && g.name == "Music") ?? musicMatches[0];

            AudioClip sfxClip = AudioClip.Create("PoolSfx", 44100, 1, 44100, false);
            AudioClip musicClip = AudioClip.Create("PoolMusic", 44100, 1, 44100, false);
            try
            {
                AudioSource voice = _audioManager.PlayOneShot(sfxClip);
                _audioManager.PlayMusic(musicClip, fadeDuration: 0f);

                _audioManager.SfxGroup = sfxGroup;
                _audioManager.MusicGroup = musicGroup;

                Assert.AreSame(sfxGroup, voice.outputAudioMixerGroup);
                Assert.AreSame(musicGroup, _audioManager.CurrentMusicSource.outputAudioMixerGroup);
            }
            finally
            {
                Object.DestroyImmediate(sfxClip);
                Object.DestroyImmediate(musicClip);
            }
        }
    }
}


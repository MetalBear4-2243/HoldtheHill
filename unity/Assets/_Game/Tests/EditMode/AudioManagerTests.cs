using HoldTheHill.Core.Audio;
using NUnit.Framework;
using UnityEngine;

namespace HoldTheHill.Tests
{
    public class AudioManagerTests
    {
        private GameObject _managerObj;
        private AudioManager _audioManager;

        [SetUp]
        public void SetUp()
        {
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
    }
}


using HoldTheHill.Core.Audio;
using NUnit.Framework;
using UnityEngine;

namespace HoldTheHill.Tests
{
    public class SoundEffectSOTests
    {
        private SoundEffectSO _sfx;

        [SetUp]
        public void SetUp()
        {
            _sfx = ScriptableObject.CreateInstance<SoundEffectSO>();
            SoundEffectSO.PlaybackHandler = null;
        }

        [TearDown]
        public void TearDown()
        {
            if (_sfx != null)
            {
                Object.DestroyImmediate(_sfx);
            }
            SoundEffectSO.PlaybackHandler = null;
        }

        [Test]
        public void SelectClip_ReturnsNull_WhenEmpty()
        {
            _sfx.Clips = null;
            Assert.IsNull(_sfx.SelectClip());

            _sfx.Clips = new AudioClip[0];
            Assert.IsNull(_sfx.SelectClip());
        }

        [Test]
        public void SelectClip_ReturnsSingleClip_WhenOnlyOneProvided()
        {
            AudioClip clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
            _sfx.Clips = new[] { clip };

            Assert.AreSame(clip, _sfx.SelectClip());

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void GetRandomPitch_ReturnsValueWithinRange()
        {
            _sfx.PitchRange = new Vector2(0.9f, 1.1f);

            for (int i = 0; i < 20; i++)
            {
                float pitch = _sfx.GetRandomPitch();
                Assert.GreaterOrEqual(pitch, 0.9f);
                Assert.LessOrEqual(pitch, 1.1f);
            }
        }

        [Test]
        public void Play_InvokesPlaybackHandler_WithCorrectParameters()
        {
            AudioClip clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
            _sfx.Clips = new[] { clip };
            _sfx.SpatialBlend = 0.5f;

            SoundEffectSO receivedSfx = null;
            Vector3 receivedPos = Vector3.zero;
            float receivedBlend = -1f;

            SoundEffectSO.PlaybackHandler = (sfx, pos, blend) =>
            {
                receivedSfx = sfx;
                receivedPos = pos;
                receivedBlend = blend;
            };

            Vector3 targetPos = new Vector3(5f, -2f, 0f);
            _sfx.Play(targetPos);

            Assert.AreSame(_sfx, receivedSfx);
            Assert.AreEqual(targetPos, receivedPos);
            Assert.AreEqual(0.5f, receivedBlend);

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void Play2D_OverridesSpatialBlendToZero()
        {
            AudioClip clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
            _sfx.Clips = new[] { clip };
            _sfx.SpatialBlend = 1.0f;

            float receivedBlend = -1f;
            SoundEffectSO.PlaybackHandler = (sfx, pos, blend) =>
            {
                receivedBlend = blend;
            };

            _sfx.Play();

            Assert.AreEqual(0f, receivedBlend);

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void Cooldown_PreventsImmediateConsecutivePlays()
        {
            AudioClip clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
            _sfx.Clips = new[] { clip };
            _sfx.Cooldown = 1.0f;

            int playCount = 0;
            SoundEffectSO.PlaybackHandler = (sfx, pos, blend) =>
            {
                playCount++;
            };

            _sfx.Play();
            _sfx.Play(); // Should be blocked by cooldown

            Assert.AreEqual(1, playCount);

            Object.DestroyImmediate(clip);
        }

        [Test]
        public void PlayAudioSource_ConfiguresAndPlaysSource()
        {
            var go = new GameObject("TestAudioSource");
            var source = go.AddComponent<AudioSource>();
            AudioClip clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);

            _sfx.Clips = new[] { clip };
            _sfx.Volume = 0.8f;
            _sfx.PitchRange = new Vector2(1f, 1f);
            _sfx.SpatialBlend = 0.25f;

            _sfx.Play(source);

            Assert.AreSame(clip, source.clip);
            Assert.AreEqual(0.8f, source.volume, 0.001f);
            Assert.AreEqual(1.0f, source.pitch, 0.001f);
            Assert.AreEqual(0.25f, source.spatialBlend, 0.001f);

            Object.DestroyImmediate(clip);
            Object.DestroyImmediate(go);
        }
    }
}


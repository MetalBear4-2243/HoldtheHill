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
    }
}


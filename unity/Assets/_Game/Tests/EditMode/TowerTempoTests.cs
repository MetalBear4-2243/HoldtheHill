using HoldTheHill.Features.Towers;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class TowerTempoTests
    {
        private const float Frame = 1f / 60f;

        [Test]
        public void NextCooldown_KeepsThePartOfTheFrameTheShotWasLateBy()
        {
            // Ready 0.01 s before the frame ended: the next shot comes 0.01 s sooner.
            Assert.AreEqual(0.34f, TowerTempo.NextCooldown(-0.01f, 0.35f, Frame, 1f), 1e-5f);
        }

        [Test]
        public void NextCooldown_GivesNoCreditForSittingIdle()
        {
            // Idle for ten seconds with nothing in range: at most one frame is kept.
            Assert.AreEqual(0.35f - Frame, TowerTempo.NextCooldown(-10f, 0.35f, Frame, 1f), 1e-5f);
        }

        [Test]
        public void NextCooldown_ExactlyOnTime_IsTheFullInterval()
        {
            Assert.AreEqual(0.8f, TowerTempo.NextCooldown(0f, 0.8f, Frame, 1f), 1e-6f);
        }

        [Test]
        public void NextCooldown_FollowsTheSwarmFrenzyTempo_AndNeverGoesBelowNormal()
        {
            Assert.AreEqual(0.4f, TowerTempo.NextCooldown(0f, 0.8f, Frame, 2f), 1e-6f);
            Assert.AreEqual(0.8f, TowerTempo.NextCooldown(0f, 0.8f, Frame, 0.5f), 1e-6f);
        }

        [Test]
        public void FiresAtItsStatedRate_OverManyFrames()
        {
            // 0.26 s is 15.6 frames at 60 fps. Without the carry each shot waits for the 16th frame
            // and the tower fires about 2.5% too slowly (97 shots in 26 s); with it, 100.
            float cooldown = 0f;
            int shots = 0;
            for (int frame = 0; frame < 26 * 60; frame++)
            {
                cooldown -= Frame;
                if (cooldown <= 0f)
                {
                    cooldown = TowerTempo.NextCooldown(cooldown, 0.26f, Frame, 1f);
                    shots++;
                }
            }

            Assert.That(shots, Is.InRange(99, 101));
        }
    }
}

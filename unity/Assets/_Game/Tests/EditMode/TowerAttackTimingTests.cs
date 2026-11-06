using HoldTheHill.Features.Towers;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class TowerAttackTimingTests
    {
        [Test]
        public void FrameSeconds_UsesTheSheetsTime_WhenTheTowerIsSlowerThanTheRow()
        {
            // Ant: 6 frames of 60 ms is 0.36 s, inside its 0.8 s between swings.
            Assert.AreEqual(0.06f, TowerAttackTiming.FrameSeconds(0.06f, 6, 0.8f, 1f), 1e-6f);
        }

        [Test]
        public void FrameSeconds_SqueezesTheRow_IntoTheTimeBetweenAttacks()
        {
            // Crazy Ant at level 3 (0.25 s) under Swarm Frenzy (x2): 0.125 s for 6 frames.
            float frame = TowerAttackTiming.FrameSeconds(0.075f, 6, 0.25f, 2f);
            Assert.AreEqual(0.125f / 6f, frame, 1e-6f);
            Assert.That(frame * 6f, Is.LessThanOrEqualTo(0.125f + 1e-6f));
        }

        [Test]
        public void FrameSeconds_NeverDropsBelowTheFloor()
        {
            Assert.AreEqual(TowerAttackTiming.MinFrameSeconds, TowerAttackTiming.FrameSeconds(0.09f, 6, 0.01f, 4f));
        }

        [Test]
        public void WindUp_IsTheFramesBeforeTheHit()
        {
            Assert.AreEqual(0.18f, TowerAttackTiming.WindUpSeconds(2, 0.09f), 1e-6f);
            Assert.AreEqual(0f, TowerAttackTiming.WindUpSeconds(0, 0.09f));
        }

        [Test]
        public void ShouldWindUp_OnlyForAReadyArmedTowerWithATarget()
        {
            Assert.IsTrue(TowerAttackTiming.ShouldWindUp(true, true, true, 0.1f, 0.18f));
            Assert.IsFalse(TowerAttackTiming.ShouldWindUp(true, true, true, 0.5f, 0.18f), "Not ready yet.");
            Assert.IsFalse(TowerAttackTiming.ShouldWindUp(true, true, false, 0f, 0.18f), "Nothing to hit.");
            Assert.IsFalse(TowerAttackTiming.ShouldWindUp(true, false, true, 0f, 0.18f), "Stunned or burrowed.");
            Assert.IsFalse(TowerAttackTiming.ShouldWindUp(false, true, true, 0f, 0.18f), "Sap and Carpenter play on their ability.");
        }

        [Test]
        public void FrameAt_StepsThroughTheRow_ThenEnds()
        {
            Assert.AreEqual(0, TowerAttackTiming.FrameAt(0f, 0.09f, 6));
            Assert.AreEqual(2, TowerAttackTiming.FrameAt(0.18f + 1e-4f, 0.09f, 6));
            Assert.AreEqual(5, TowerAttackTiming.FrameAt(0.53f, 0.09f, 6));
            Assert.AreEqual(-1, TowerAttackTiming.FrameAt(0.55f, 0.09f, 6));
            Assert.AreEqual(-1, TowerAttackTiming.FrameAt(0f, 0.09f, 0));
        }

        [Test]
        public void TheHitFrame_ShowsAsTheTowerBecomesReady()
        {
            // The wind-up starts when the cooldown reaches the wind-up time, so the clock at the
            // attack is exactly the wind-up: the hit frame.
            float frame = 0.07f;
            float windUp = TowerAttackTiming.WindUpSeconds(2, frame);
            Assert.AreEqual(2, TowerAttackTiming.FrameAt(windUp + 1e-4f, frame, 6));
        }
    }
}

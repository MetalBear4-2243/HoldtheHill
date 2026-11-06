using HoldTheHill.Features.GameFlow;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class RunClockTests
    {
        [Test]
        public void StartsRunning_AtNormalSpeed()
        {
            var clock = new RunClock(fastScale: 2f);

            Assert.IsFalse(clock.Paused);
            Assert.IsFalse(clock.Fast);
            Assert.AreEqual(1f, clock.TimeScale);
        }

        [Test]
        public void ToggleSpeed_SwitchesBetweenNormalAndFast()
        {
            var clock = new RunClock(fastScale: 2f);

            clock.ToggleSpeed();
            Assert.AreEqual(2f, clock.TimeScale);

            clock.ToggleSpeed();
            Assert.AreEqual(1f, clock.TimeScale);
        }

        [Test]
        public void Pausing_StopsTime_AndResumingKeepsTheChosenSpeed()
        {
            var clock = new RunClock(fastScale: 2f);
            clock.ToggleSpeed();

            clock.TogglePause();
            Assert.IsTrue(clock.Paused);
            Assert.AreEqual(0f, clock.TimeScale);

            clock.TogglePause();
            Assert.IsFalse(clock.Paused);
            Assert.AreEqual(2f, clock.TimeScale, "resuming goes back to the fast speed");
        }

        [Test]
        public void Changed_IsRaised_OnlyWhenSomethingChanges()
        {
            var clock = new RunClock(fastScale: 2f);
            int raised = 0;
            clock.Changed += () => raised++;

            clock.SetPaused(false);
            Assert.AreEqual(0, raised, "already running");

            clock.SetPaused(true);
            clock.ToggleSpeed();
            clock.SetPaused(false);
            Assert.AreEqual(3, raised);
        }

        [Test]
        public void FastScale_FallsBackToDouble_WhenNotFasterThanNormal()
        {
            Assert.AreEqual(2f, new RunClock(fastScale: 1f).FastScale);
            Assert.AreEqual(2f, new RunClock(fastScale: 0f).FastScale);
            Assert.AreEqual(3f, new RunClock(fastScale: 3f).FastScale);
        }
    }
}

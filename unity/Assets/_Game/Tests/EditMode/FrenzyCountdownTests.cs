using HoldTheHill.Features.UI;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    /// <summary>The Swarm Frenzy countdown on the power-up slot shows whole seconds and redraws once per second.</summary>
    public class FrenzyCountdownTests
    {
        [TestCase(10f, 10)]
        [TestCase(9.01f, 10)]
        [TestCase(9f, 9)]
        [TestCase(0.2f, 1)]
        [TestCase(0f, -1)]
        [TestCase(-0.5f, -1)]
        public void CountdownSeconds_RoundsUp_AndIsMinusOneWhenNoFrenzyRuns(float secondsLeft, int shown)
        {
            Assert.AreEqual(shown, PowerUpSlotPanel.CountdownSeconds(secondsLeft));
        }

        [Test]
        public void ATenSecondFrenzyAt60Fps_ChangesTheShownSecondTenTimes()
        {
            // Shown 10 at the start, then 9 ... 1 and -1 when it ends: ten redraws in about 600 frames.
            float left = 10f;
            int shown = PowerUpSlotPanel.CountdownSeconds(left);
            int changes = 0;
            int frames = 0;
            while (left > 0f)
            {
                left -= 1f / 60f;
                frames++;
                int now = PowerUpSlotPanel.CountdownSeconds(left);
                if (now != shown)
                {
                    changes++;
                    shown = now;
                }
            }

            Assert.That(frames, Is.GreaterThan(590));
            Assert.AreEqual(10, changes);
            Assert.AreEqual(-1, shown);
        }
    }
}

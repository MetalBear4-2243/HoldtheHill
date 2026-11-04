using HoldTheHill.Features.Hazards;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class DiatomaceousEarthTests
    {
        [Test]
        public void SettleShare_FadesInFromAHaze()
        {
            float start = DiatomaceousEarth.SettleShare(0f, 1.5f);
            float middle = DiatomaceousEarth.SettleShare(0.75f, 1.5f);
            Assert.That(start, Is.GreaterThan(0f).And.LessThan(0.5f), "The patch is faintly visible at once, so the player sees where it lands.");
            Assert.That(middle, Is.GreaterThan(start).And.LessThan(1f));
        }

        [Test]
        public void SettleShare_IsSolidOnceSettled()
        {
            Assert.AreEqual(1f, DiatomaceousEarth.SettleShare(1.5f, 1.5f));
            Assert.AreEqual(1f, DiatomaceousEarth.SettleShare(9f, 1.5f));
        }

        [Test]
        public void SettleShare_NoSettleTime_IsSolidAtOnce()
        {
            Assert.AreEqual(1f, DiatomaceousEarth.SettleShare(0f, 0f));
        }
    }
}

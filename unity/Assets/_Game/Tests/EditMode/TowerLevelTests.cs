using HoldTheHill.Features.Towers;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// The numbers the balance table shows beside a level: attacks per second is 1 over the attack
    /// interval, single-target DPS is damage times that, and the tempo divides the interval.
    /// </summary>
    public class TowerLevelTests
    {
        [Test]
        public void TheBulletAnt_12DamageEvery1Point2Seconds_Is10Dps()
        {
            var level = new TowerLevel { Damage = 12f, AttackInterval = 1.2f };

            Assert.AreEqual(1f / 1.2f, level.AttacksPerSecond, 1e-6f);
            Assert.AreEqual(10f, level.SingleTargetDps, 1e-5f);
        }

        [Test]
        public void AttacksPerSecond_IsOneOverTheInterval()
        {
            Assert.AreEqual(2f, TowerLevel.AttacksPerSecondFor(0.5f), 1e-6f);
            Assert.AreEqual(5f, TowerLevel.AttacksPerSecondFor(0.2f), 1e-6f);
        }

        [Test]
        public void ANonPositiveInterval_GivesNoAttacksAndNoDps()
        {
            Assert.AreEqual(0f, TowerLevel.AttacksPerSecondFor(0f));
            Assert.AreEqual(0f, TowerLevel.AttacksPerSecondFor(-1f));
            Assert.AreEqual(0f, TowerLevel.DpsFor(30f, 0f));
        }

        [Test]
        public void TheSapAnt_WithNoDamage_HasNoDps_ButStillAttacks()
        {
            var level = new TowerLevel { Damage = 0f, AttackInterval = 2f };

            Assert.AreEqual(0.5f, level.AttacksPerSecond, 1e-6f);
            Assert.AreEqual(0f, level.SingleTargetDps);
        }

        [Test]
        public void TheTempo_DividesTheInterval_SoDpsDoublesAtTwo()
        {
            float interval = TowerTempo.IntervalUnder(1.2f, 2f);

            Assert.AreEqual(0.6f, interval, 1e-6f);
            Assert.AreEqual(20f, TowerLevel.DpsFor(12f, interval), 1e-5f);
        }

        [Test]
        public void TheTempo_NeverSlowsATower()
        {
            Assert.AreEqual(1.2f, TowerTempo.IntervalUnder(1.2f, 0.5f), 1e-6f);
            Assert.AreEqual(1.2f, TowerTempo.IntervalUnder(1.2f, 1f), 1e-6f);
        }

        [Test]
        public void TheDefaultLevel_IsAPlainMeleeTower()
        {
            TowerLevel level = TowerLevel.Default;

            Assert.AreEqual(2.5f, level.Range);
            Assert.AreEqual(10f, level.Damage);
            Assert.AreEqual(1f, level.AttackInterval);
            Assert.AreEqual(0f, level.Ability.ConeDegrees);
            Assert.AreEqual(3, level.Ability.MaxMines);
        }
    }
}

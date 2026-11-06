using HoldTheHill.Features.Enemies;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class EnemyShieldTests
    {
        private static EnemyShield NewShield()
        {
            return new EnemyShield(capacity: 40.0, regrowDelay: 3f, regrowPerSecond: 20.0);
        }

        [Test]
        public void StartsFull_AndSoaksAHitSmallerThanItself()
        {
            EnemyShield shield = NewShield();

            Assert.AreEqual(40.0, shield.Current);
            Assert.AreEqual(0.0, shield.Soak(15.0));
            Assert.AreEqual(25.0, shield.Current);
            Assert.IsTrue(shield.IsUp);
        }

        [Test]
        public void OnlyTheExcessGetsThrough_WhenAHitEmptiesTheShield()
        {
            EnemyShield shield = NewShield();

            Assert.AreEqual(10.0, shield.Soak(50.0));
            Assert.AreEqual(0.0, shield.Current);
            Assert.IsFalse(shield.IsUp);
            Assert.AreEqual(7.0, shield.Soak(7.0), "An empty shield passes everything through.");
        }

        [Test]
        public void ZeroOrNegativeDamage_ChangesNothing()
        {
            EnemyShield shield = NewShield();
            shield.Tick(2f);

            Assert.AreEqual(0.0, shield.Soak(0.0));
            Assert.AreEqual(0.0, shield.Soak(-5.0));
            Assert.AreEqual(40.0, shield.Current);
            Assert.AreEqual(2f, shield.UntouchedFor, 0.0001f, "A hit of nothing does not count as a hit.");
        }

        [Test]
        public void DoesNotRegrow_BeforeTheDelayHasPassed()
        {
            EnemyShield shield = NewShield();
            shield.Soak(40.0);

            shield.Tick(1f);
            shield.Tick(1.9f);

            Assert.AreEqual(0.0, shield.Current);
        }

        [Test]
        public void RegrowsAtTheRate_OnceLeftAloneForTheDelay()
        {
            EnemyShield shield = NewShield();
            shield.Soak(40.0);

            shield.Tick(3f);
            Assert.AreEqual(0.0, shield.Current, "The delay has only just passed; nothing has grown yet.");
            shield.Tick(0.5f);
            Assert.AreEqual(10.0, shield.Current, 0.0001);
            shield.Tick(10f);
            Assert.AreEqual(40.0, shield.Current, "Never above the capacity.");
        }

        [Test]
        public void AnyHit_RestartsTheWait_EvenOneFullySoaked()
        {
            EnemyShield shield = NewShield();
            shield.Soak(30.0);
            shield.Tick(2.5f);

            shield.Soak(5.0);
            Assert.AreEqual(0f, shield.UntouchedFor);
            shield.Tick(2.5f);
            Assert.AreEqual(5.0, shield.Current, "Still waiting: the second hit reset the timer.");
        }

        [Test]
        public void Fraction_FollowsTheShieldLeft()
        {
            EnemyShield shield = NewShield();
            Assert.AreEqual(1f, shield.Fraction, 0.0001f);
            shield.Soak(30.0);
            Assert.AreEqual(0.25f, shield.Fraction, 0.0001f);
            Assert.AreEqual(0f, new EnemyShield(0.0, 1f, 1.0).Fraction, "No shield at all reads empty, never a divide by zero.");
        }
    }
}

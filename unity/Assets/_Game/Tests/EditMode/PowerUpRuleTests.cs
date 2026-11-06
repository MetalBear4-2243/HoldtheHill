using System;
using HoldTheHill.Features.PowerUps;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class PowerUpRuleTests
    {
        [Test]
        public void RollDrop_DropsOnlyBelowTheChance()
        {
            Assert.IsTrue(PowerUpRule.RollDrop(0.0, 0.1f, out _));
            Assert.IsTrue(PowerUpRule.RollDrop(0.0999, 0.1f, out _));
            Assert.IsFalse(PowerUpRule.RollDrop(0.1, 0.1f, out _), "the chance itself is not under the chance");
            Assert.IsFalse(PowerUpRule.RollDrop(0.9, 0.1f, out _));
        }

        [Test]
        public void RollDrop_SplitsTheDroppingRange_EvenlyBetweenTheKinds()
        {
            Assert.AreEqual(2, PowerUpRule.KindCount, "the Alpha has exactly two power-ups");

            PowerUpRule.RollDrop(0.0, 0.1f, out PowerUpKind first);
            PowerUpRule.RollDrop(0.049, 0.1f, out PowerUpKind stillFirst);
            PowerUpRule.RollDrop(0.05, 0.1f, out PowerUpKind second);
            PowerUpRule.RollDrop(0.0999, 0.1f, out PowerUpKind stillSecond);

            Assert.AreEqual(PowerUpKind.SwarmFrenzy, first);
            Assert.AreEqual(PowerUpKind.SwarmFrenzy, stillFirst);
            Assert.AreEqual(PowerUpKind.RoyalGift, second);
            Assert.AreEqual(PowerUpKind.RoyalGift, stillSecond);
        }

        [Test]
        public void RollDrop_NeverDrops_WithNoChance_OrABadRoll()
        {
            Assert.IsFalse(PowerUpRule.RollDrop(0.0, 0f, out _));
            Assert.IsFalse(PowerUpRule.RollDrop(-0.5, 1f, out _));
            Assert.IsFalse(PowerUpRule.RollDrop(1.0, 1f, out _));
        }

        [Test]
        public void RollDrop_SeededRandom_DropsAboutTheChance()
        {
            // Seeded, so the count is the same every run.
            var random = new Random(20261016);
            int drops = 0;
            for (int i = 0; i < 10000; i++)
            {
                if (PowerUpRule.RollDrop(random.NextDouble(), 0.08f, out _))
                {
                    drops++;
                }
            }

            Assert.That(drops, Is.InRange(700, 900), "about 8 in 100 kills drop something");
        }

        [Test]
        public void CanGift_OnlyBelowTheLastLevel()
        {
            Assert.IsTrue(PowerUpRule.CanGift(1, 3));
            Assert.IsTrue(PowerUpRule.CanGift(2, 3));
            Assert.IsFalse(PowerUpRule.CanGift(3, 3));
            Assert.IsFalse(PowerUpRule.CanGift(1, 1), "a tower with no upgrades has nothing to gain");
        }
    }
}

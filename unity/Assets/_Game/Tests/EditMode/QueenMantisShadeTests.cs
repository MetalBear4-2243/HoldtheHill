using HoldTheHill.Features.Enemies;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class QueenMantisShadeTests
    {
        [TestCase(1.0, 0)]
        [TestCase(0.9, 0)]
        [TestCase(0.67, 0)]
        [TestCase(0.66, 1)]
        [TestCase(0.5, 1)]
        [TestCase(0.34, 1)]
        [TestCase(0.33, 2)]
        [TestCase(0.1, 2)]
        [TestCase(0.0, 2)]
        public void ThirdsLost_FollowsTheHealthLeft(double fraction, int expected)
        {
            Assert.AreEqual(expected, QueenMantisShadeRule.ThirdsLost(fraction));
        }

        [Test]
        public void ExactThirds_CountAsLost()
        {
            Assert.AreEqual(1, QueenMantisShadeRule.ThirdsLost(2.0 / 3.0));
            Assert.AreEqual(2, QueenMantisShadeRule.ThirdsLost(1.0 / 3.0));
            Assert.AreEqual(1, QueenMantisShadeRule.ThirdsLost(200.0 / 300.0), "200 of 300 is the first third, in double.");
        }

        [Test]
        public void OutOfRangeFractions_ReadAsFullOrEmpty()
        {
            Assert.AreEqual(0, QueenMantisShadeRule.ThirdsLost(1.5));
            Assert.AreEqual(2, QueenMantisShadeRule.ThirdsLost(-0.5));
        }

        [Test]
        public void TheShades_GoWhiteBlackWhite_AsInDecision108()
        {
            Assert.AreEqual(QueenMantisShade.White, QueenMantisShadeRule.ForThirdsLost(0));
            Assert.AreEqual(QueenMantisShade.Black, QueenMantisShadeRule.ForThirdsLost(1));
            Assert.AreEqual(QueenMantisShade.White, QueenMantisShadeRule.ForThirdsLost(2));
        }

        [Test]
        public void TheCycle_StartsWhite()
        {
            var cycle = new QueenMantisShadeCycle();

            Assert.AreEqual(QueenMantisShade.White, cycle.Shade);
            Assert.AreEqual(0, cycle.ThirdsLost);
            Assert.IsFalse(cycle.TryAdvance(1.0));
        }

        [Test]
        public void TheCycle_ChangesAtEachThird()
        {
            var cycle = new QueenMantisShadeCycle();

            Assert.IsFalse(cycle.TryAdvance(0.7));
            Assert.IsTrue(cycle.TryAdvance(0.6));
            Assert.AreEqual(QueenMantisShade.Black, cycle.Shade);
            Assert.IsFalse(cycle.TryAdvance(0.5), "Still in the same third.");
            Assert.IsTrue(cycle.TryAdvance(0.3));
            Assert.AreEqual(QueenMantisShade.White, cycle.Shade);
        }

        [Test]
        public void HealingBackUp_NeverReturnsAShade()
        {
            var cycle = new QueenMantisShadeCycle();
            cycle.TryAdvance(0.6);

            Assert.IsFalse(cycle.TryAdvance(1.0));
            Assert.AreEqual(QueenMantisShade.Black, cycle.Shade);
            Assert.AreEqual(1, cycle.ThirdsLost);
        }

        [Test]
        public void ABigHitThatSkipsAThird_StillStepsThroughBlack()
        {
            var cycle = new QueenMantisShadeCycle();

            Assert.IsTrue(cycle.TryAdvance(0.2));
            Assert.AreEqual(QueenMantisShade.Black, cycle.Shade, "One third per step, so black shows first.");
            Assert.IsTrue(cycle.TryAdvance(0.2));
            Assert.AreEqual(QueenMantisShade.White, cycle.Shade);
        }

        [Test]
        public void AfterTheLastThird_NothingMoreChanges()
        {
            var cycle = new QueenMantisShadeCycle();
            cycle.TryAdvance(0.5);
            cycle.TryAdvance(0.1);

            Assert.IsFalse(cycle.TryAdvance(0.0));
            Assert.AreEqual(2, cycle.ThirdsLost);
        }
    }
}

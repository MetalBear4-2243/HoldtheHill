using HoldTheHill.Features.Enemies;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class QueenMantisShadeTests
    {
        [TestCase(1.0, QueenMantisShade.Green)]
        [TestCase(0.9, QueenMantisShade.Green)]
        [TestCase(0.67, QueenMantisShade.Green)]
        [TestCase(0.66, QueenMantisShade.White)]
        [TestCase(0.5, QueenMantisShade.White)]
        [TestCase(0.34, QueenMantisShade.White)]
        [TestCase(0.33, QueenMantisShade.Black)]
        [TestCase(0.1, QueenMantisShade.Black)]
        [TestCase(0.0, QueenMantisShade.Black)]
        public void TheShade_FollowsTheThirdOfHealthLeft(double fraction, QueenMantisShade expected)
        {
            Assert.AreEqual(expected, QueenMantisShadeRule.ForHealthFraction(fraction));
        }

        [Test]
        public void ExactThirds_BelongToTheLowerShade()
        {
            Assert.AreEqual(QueenMantisShade.White, QueenMantisShadeRule.ForHealthFraction(2.0 / 3.0));
            Assert.AreEqual(QueenMantisShade.Black, QueenMantisShadeRule.ForHealthFraction(1.0 / 3.0));
        }

        [Test]
        public void OutOfRangeFractions_ReadAsFullOrEmpty()
        {
            Assert.AreEqual(QueenMantisShade.Green, QueenMantisShadeRule.ForHealthFraction(1.5));
            Assert.AreEqual(QueenMantisShade.Black, QueenMantisShadeRule.ForHealthFraction(-0.5));
        }
    }
}

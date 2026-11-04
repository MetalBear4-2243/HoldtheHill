using HoldTheHill.Features.Enemies;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class QueenMantisHealRuleTests
    {
        private const float Radius = 3f;

        [Test]
        public void AHurtEnemyWithinReach_IsHealed()
        {
            Assert.IsTrue(QueenMantisHealRule.ShouldHeal(false, false, 5.0, 12.0, 1f, 1f, Radius));
        }

        [Test]
        public void SheNeverHealsHerself()
        {
            Assert.IsFalse(QueenMantisHealRule.ShouldHeal(true, false, 100.0, 300.0, 0f, 0f, Radius));
        }

        [Test]
        public void TheDead_AreNotHealed()
        {
            Assert.IsFalse(QueenMantisHealRule.ShouldHeal(false, true, 0.0, 12.0, 0f, 0f, Radius));
        }

        [Test]
        public void AnEnemyAtFullHealth_IsLeftAlone()
        {
            Assert.IsFalse(QueenMantisHealRule.ShouldHeal(false, false, 12.0, 12.0, 0f, 0f, Radius));
        }

        [Test]
        public void AnEnemyOutOfReach_IsNotHealed()
        {
            Assert.IsFalse(QueenMantisHealRule.ShouldHeal(false, false, 5.0, 12.0, 3f, 0.5f, Radius));
        }

        [Test]
        public void TheEdgeOfHerReach_Counts()
        {
            Assert.IsTrue(QueenMantisHealRule.InReach(3f, 0f, Radius));
            Assert.IsTrue(QueenMantisHealRule.InReach(0f, -3f, Radius));
            Assert.IsFalse(QueenMantisHealRule.InReach(3.01f, 0f, Radius));
        }
    }
}

using HoldTheHill.Features.Towers;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class TowerSellRuleTests
    {
        [Test]
        public void Refund_IsTheFractionOfEverythingSpent_RoundedDown()
        {
            // Built for 75, upgraded for 60 and 90: 225 spent, three quarters back.
            Assert.AreEqual(168, TowerSellRule.Refund(225, 0.75f));
            Assert.AreEqual(75, TowerSellRule.Refund(75, 1f));
            Assert.AreEqual(0, TowerSellRule.Refund(75, 0f));
        }

        [Test]
        public void Refund_NeverGoesNegative_OrAboveWhatWasSpent()
        {
            Assert.AreEqual(0, TowerSellRule.Refund(0, 0.75f));
            Assert.AreEqual(0, TowerSellRule.Refund(-50, 0.75f));
            Assert.AreEqual(100, TowerSellRule.Refund(100, 1.5f), "a fraction above 1 is treated as 1");
            Assert.AreEqual(0, TowerSellRule.Refund(100, -0.5f), "a fraction below 0 is treated as 0");
        }
    }
}

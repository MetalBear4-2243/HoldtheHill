using HoldTheHill.Features.Towers;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class TowerUpgradeRuleTests
    {
        [Test]
        public void Ready_WhenBelowMaxLevelAndTheNextLevelIsAffordable()
        {
            Assert.AreEqual(TowerUpgradeRule.Verdict.Ready, TowerUpgradeRule.Check(level: 1, maxLevel: 3, cost: 50, honeydew: 50));
            Assert.AreEqual(TowerUpgradeRule.Verdict.Ready, TowerUpgradeRule.Check(level: 2, maxLevel: 3, cost: 80, honeydew: 300));
        }

        [Test]
        public void CannotAfford_WhenShortByEvenOne()
        {
            Assert.AreEqual(TowerUpgradeRule.Verdict.CannotAfford, TowerUpgradeRule.Check(level: 1, maxLevel: 3, cost: 50, honeydew: 49));
        }

        [Test]
        public void AtMaxLevel_BeatsThePriceCheck()
        {
            Assert.AreEqual(TowerUpgradeRule.Verdict.AtMaxLevel, TowerUpgradeRule.Check(level: 3, maxLevel: 3, cost: 0, honeydew: 999));
            Assert.AreEqual(TowerUpgradeRule.Verdict.AtMaxLevel, TowerUpgradeRule.Check(level: 3, maxLevel: 3, cost: 50, honeydew: 0));
        }

        [Test]
        public void AtMaxLevel_ForATowerWithNoUpgradesAtAll()
        {
            Assert.AreEqual(TowerUpgradeRule.Verdict.AtMaxLevel, TowerUpgradeRule.Check(level: 1, maxLevel: 1, cost: 0, honeydew: 500));
        }
    }
}

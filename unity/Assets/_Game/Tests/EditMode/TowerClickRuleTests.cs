using HoldTheHill.Features.Towers;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class TowerClickRuleTests
    {
        [Test]
        public void ClickOnStandingTower_SelectsIt_WithOrWithoutATowerInHand()
        {
            Assert.AreEqual(TowerClickRule.Outcome.SelectStanding,
                TowerClickRule.Decide(armed: false, towerOnCell: true, blocked: true, affordable: true));
            Assert.AreEqual(TowerClickRule.Outcome.SelectStanding,
                TowerClickRule.Decide(armed: true, towerOnCell: true, blocked: true, affordable: true),
                "a tower in hand must not stop the player selecting another tower");
            Assert.AreEqual(TowerClickRule.Outcome.SelectStanding,
                TowerClickRule.Decide(armed: true, towerOnCell: true, blocked: true, affordable: false));
        }

        [Test]
        public void ClickOnEmptyCell_WithNothingInHand_ClearsTheSelection()
        {
            Assert.AreEqual(TowerClickRule.Outcome.ClearSelection,
                TowerClickRule.Decide(armed: false, towerOnCell: false, blocked: false, affordable: true));
            Assert.AreEqual(TowerClickRule.Outcome.ClearSelection,
                TowerClickRule.Decide(armed: false, towerOnCell: false, blocked: true, affordable: false),
                "clicking the path with nothing in hand clears too");
        }

        [Test]
        public void ClickWithTowerInHand_BuildsOnlyOnAFreeAffordableCell()
        {
            Assert.AreEqual(TowerClickRule.Outcome.Build,
                TowerClickRule.Decide(armed: true, towerOnCell: false, blocked: false, affordable: true));
            Assert.AreEqual(TowerClickRule.Outcome.Nothing,
                TowerClickRule.Decide(armed: true, towerOnCell: false, blocked: true, affordable: true),
                "the path is blocked");
            Assert.AreEqual(TowerClickRule.Outcome.Nothing,
                TowerClickRule.Decide(armed: true, towerOnCell: false, blocked: false, affordable: false),
                "not enough Honeydew");
        }
    }
}

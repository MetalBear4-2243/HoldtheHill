using HoldTheHill.Features.GameFlow;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class RunOutcomeRuleTests
    {
        [Test]
        public void Playing_WhileWavesRemain()
        {
            Assert.AreEqual(RunOutcome.Playing, RunOutcomeRule.Decide(queenDefeated: false, allWavesSent: false, livingEnemies: 0));
            Assert.AreEqual(RunOutcome.Playing, RunOutcomeRule.Decide(queenDefeated: false, allWavesSent: false, livingEnemies: 4));
        }

        [Test]
        public void Playing_WhileTheLastWaveStillHasEnemies()
        {
            Assert.AreEqual(RunOutcome.Playing, RunOutcomeRule.Decide(queenDefeated: false, allWavesSent: true, livingEnemies: 1));
        }

        [Test]
        public void Won_WhenEveryWaveIsSentAndNoEnemyIsLeft()
        {
            Assert.AreEqual(RunOutcome.Won, RunOutcomeRule.Decide(queenDefeated: false, allWavesSent: true, livingEnemies: 0));
        }

        [Test]
        public void Lost_WhenTheQueenFalls_EvenWithTheMapCleared()
        {
            Assert.AreEqual(RunOutcome.Lost, RunOutcomeRule.Decide(queenDefeated: true, allWavesSent: false, livingEnemies: 3));
            Assert.AreEqual(RunOutcome.Lost, RunOutcomeRule.Decide(queenDefeated: true, allWavesSent: true, livingEnemies: 0));
        }
    }
}

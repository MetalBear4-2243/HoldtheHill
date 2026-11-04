using HoldTheHill.Features.GameFlow;
using NUnit.Framework;
using State = EnemySpawner.SpawnerState;

namespace HoldTheHill.Tests
{
    public class NextWaveRuleTests
    {
        [Test]
        public void CanSend_BeforeTheFirstWave_AndBetweenWaves()
        {
            Assert.IsTrue(NextWaveRule.CanSend(RunOutcome.Playing, paused: false, State.Idle));
            Assert.IsTrue(NextWaveRule.CanSend(RunOutcome.Playing, paused: false, State.WaitingForNextWave));
        }

        [Test]
        public void CannotSend_WhileAWaveIsComingOut_OrAfterTheLast()
        {
            Assert.IsFalse(NextWaveRule.CanSend(RunOutcome.Playing, paused: false, State.Spawning));
            Assert.IsFalse(NextWaveRule.CanSend(RunOutcome.Playing, paused: false, State.Completed));
        }

        [Test]
        public void CannotSend_WhilePaused()
        {
            Assert.IsFalse(NextWaveRule.CanSend(RunOutcome.Playing, paused: true, State.WaitingForNextWave));
        }

        [Test]
        public void CannotSend_OnceTheRunIsOver()
        {
            Assert.IsFalse(NextWaveRule.CanSend(RunOutcome.Won, paused: false, State.WaitingForNextWave));
            Assert.IsFalse(NextWaveRule.CanSend(RunOutcome.Lost, paused: false, State.Idle));
        }
    }
}

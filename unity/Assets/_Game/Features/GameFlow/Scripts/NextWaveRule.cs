namespace HoldTheHill.Features.GameFlow
{
    /// <summary>
    /// When the player may send the next wave, as a pure function so EditMode tests cover it: only
    /// while the run is on and not paused, and only when the spawner is waiting (before the first
    /// wave, or between two). Not while a wave is still coming out, and not after the last one.
    /// The Next Wave button greys out by it, and Space goes through the same check.
    /// </summary>
    public static class NextWaveRule
    {
        /// <param name="outcome">Playing until the run is won or lost.</param>
        /// <param name="paused">True under the pause menu.</param>
        /// <param name="spawnerState">The level spawner's state.</param>
        public static bool CanSend(RunOutcome outcome, bool paused, EnemySpawner.SpawnerState spawnerState)
        {
            if (outcome != RunOutcome.Playing || paused)
            {
                return false;
            }

            return spawnerState == EnemySpawner.SpawnerState.Idle
                || spawnerState == EnemySpawner.SpawnerState.WaitingForNextWave;
        }
    }
}

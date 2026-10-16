namespace HoldTheHill.Features.GameFlow
{
    /// <summary>
    /// The win and lose rule of a level, as a pure function so EditMode tests cover it.
    /// Lost when the Queen is beaten; won when every wave has been sent and no enemy is left;
    /// a lost Queen wins over a cleared map.
    /// </summary>
    public static class RunOutcomeRule
    {
        /// <param name="queenDefeated">True once the Queen's health hit zero.</param>
        /// <param name="allWavesSent">True once the spawner has finished its last wave.</param>
        /// <param name="livingEnemies">Enemies still alive on the map.</param>
        public static RunOutcome Decide(bool queenDefeated, bool allWavesSent, int livingEnemies)
        {
            if (queenDefeated)
            {
                return RunOutcome.Lost;
            }

            if (allWavesSent && livingEnemies <= 0)
            {
                return RunOutcome.Won;
            }

            return RunOutcome.Playing;
        }
    }
}

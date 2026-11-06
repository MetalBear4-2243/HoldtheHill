namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// Whether a tower may be upgraded right now. Plain C# so EditMode tests can cover it;
    /// <see cref="TowerPlacer.UpgradeSelectedTower"/> applies the answer.
    /// </summary>
    public static class TowerUpgradeRule
    {
        public enum Verdict
        {
            /// <summary>The upgrade can be bought.</summary>
            Ready,

            /// <summary>The tower is already at its last level.</summary>
            AtMaxLevel,

            /// <summary>Not enough Honeydew for the next level.</summary>
            CannotAfford
        }

        /// <summary>
        /// Decides for a tower at <paramref name="level"/> of <paramref name="maxLevel"/> whose next
        /// level costs <paramref name="cost"/>, with <paramref name="honeydew"/> in the wallet.
        /// The level check comes first, so a maxed tower never reports a price.
        /// </summary>
        public static Verdict Check(int level, int maxLevel, int cost, int honeydew)
        {
            if (level >= maxLevel)
            {
                return Verdict.AtMaxLevel;
            }

            return honeydew >= cost ? Verdict.Ready : Verdict.CannotAfford;
        }
    }
}

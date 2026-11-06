using System;
using HoldTheHill.Features.Towers;

namespace HoldTheHill.Features.PowerUps
{
    /// <summary>
    /// The power-up rules as plain C#, so EditMode tests can cover them: whether a kill drops one and
    /// which, and whether a Royal Gift can do anything for a tower. <see cref="LevelPowerUps"/> applies them.
    /// </summary>
    public static class PowerUpRule
    {
        /// <summary>How many kinds a roll is split between.</summary>
        public static readonly int KindCount = Enum.GetValues(typeof(PowerUpKind)).Length;

        /// <summary>
        /// Decides from one random <paramref name="roll"/> in [0, 1) whether a kill drops a power-up and
        /// which: a roll under <paramref name="dropChance"/> drops, and that range is split evenly between
        /// the kinds, so one roll does both jobs and a chance of zero never drops.
        /// </summary>
        public static bool RollDrop(double roll, float dropChance, out PowerUpKind kind)
        {
            kind = default;
            // The chance is a float from the data asset; compare in float so 0.1 is not "under 0.1f".
            float r = (float)roll;
            if (dropChance <= 0f || r < 0f || r >= dropChance)
            {
                return false;
            }

            int index = (int)Math.Floor(r / dropChance * KindCount);
            kind = (PowerUpKind)Math.Min(Math.Max(index, 0), KindCount - 1);
            return true;
        }

        /// <summary>
        /// A Royal Gift is a free upgrade, so it helps any tower that is not at its last level.
        /// Reuses the upgrade rule with nothing to pay.
        /// </summary>
        public static bool CanGift(int level, int maxLevel)
        {
            return TowerUpgradeRule.Check(level, maxLevel, cost: 0, honeydew: 0) == TowerUpgradeRule.Verdict.Ready;
        }
    }
}

using System;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// What the player gets back for selling a tower: a fraction of every Honeydew spent on it
    /// (the build price and each upgrade), rounded down. Plain C# so EditMode tests can cover it;
    /// <see cref="TowerPlacer.SellSelectedTower"/> applies the answer.
    /// </summary>
    public static class TowerSellRule
    {
        /// <summary>
        /// Honeydew returned for a tower that cost <paramref name="honeydewSpent"/> in all, at a
        /// refund <paramref name="fraction"/> between 0 and 1. Never negative, never more than was spent.
        /// </summary>
        public static int Refund(int honeydewSpent, float fraction)
        {
            if (honeydewSpent <= 0)
            {
                return 0;
            }

            double clamped = Math.Min(1.0, Math.Max(0.0, fraction));
            return (int)Math.Floor(honeydewSpent * clamped);
        }
    }
}

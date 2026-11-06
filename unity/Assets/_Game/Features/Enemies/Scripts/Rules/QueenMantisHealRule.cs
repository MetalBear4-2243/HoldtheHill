namespace HoldTheHill.Features.Enemies
{
    /// <summary>Who her white heal reaches, as plain C# so EditMode tests can cover it.</summary>
    public static class QueenMantisHealRule
    {
        /// <summary>True when a point <paramref name="dx"/>, <paramref name="dy"/> away from her is within <paramref name="radius"/>; the edge counts.</summary>
        public static bool InReach(float dx, float dy, float radius)
        {
            return dx * dx + dy * dy <= radius * radius;
        }

        /// <summary>
        /// True for a living, hurt enemy within reach. Never herself: she heals her escort, so
        /// focusing her down stays the answer.
        /// </summary>
        public static bool ShouldHeal(bool isHer, bool isDead, double currentHealth, double maxHealth,
            float dx, float dy, float radius)
        {
            return !isHer && !isDead && currentHealth < maxHealth && InReach(dx, dy, radius);
        }
    }
}

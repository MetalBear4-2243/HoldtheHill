namespace HoldTheHill.Features.Enemies
{
    /// <summary>The Queen Mantis's two shades (GDD decision 98).</summary>
    public enum QueenMantisShade
    {
        /// <summary>Praying: she heals the enemies around her.</summary>
        White,

        /// <summary>Necromancer: she raises the enemies killed near her as husks.</summary>
        Black
    }

    /// <summary>
    /// Which shade each third of her health gives, as plain C# so EditMode tests can cover it.
    /// Decision 108: she changes shade each time she loses a third of her health, white, black, white.
    /// </summary>
    public static class QueenMantisShadeRule
    {
        /// <summary>How many thirds of her health are gone: 0, 1 or 2. An exact third counts as gone.</summary>
        /// <param name="healthFraction">Current health over maximum health; above 1 reads as full, below 0 as empty.</param>
        public static int ThirdsLost(double healthFraction)
        {
            if (healthFraction > 2.0 / 3.0)
            {
                return 0;
            }

            if (healthFraction > 1.0 / 3.0)
            {
                return 1;
            }

            return 2;
        }

        /// <summary>Her shade once <paramref name="thirdsLost"/> thirds are gone: white, then black, then white again.</summary>
        public static QueenMantisShade ForThirdsLost(int thirdsLost)
        {
            return thirdsLost == 1 ? QueenMantisShade.Black : QueenMantisShade.White;
        }
    }
}

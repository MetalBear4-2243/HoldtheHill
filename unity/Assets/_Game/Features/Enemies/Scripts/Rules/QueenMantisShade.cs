namespace HoldTheHill.Features.Enemies
{
    /// <summary>The Queen Mantis's three shades, one per third of her health.</summary>
    public enum QueenMantisShade
    {
        /// <summary>Above two thirds of her health: she only walks.</summary>
        Green,

        /// <summary>Between one and two thirds: she heals the enemies around her.</summary>
        White,

        /// <summary>Below one third: she raises the enemies killed near her as husks.</summary>
        Black
    }

    /// <summary>Which shade a health fraction gives, as plain C# so EditMode tests can cover it.</summary>
    public static class QueenMantisShadeRule
    {
        /// <param name="healthFraction">Current health over maximum health; above 1 reads as full, below 0 as empty.</param>
        public static QueenMantisShade ForHealthFraction(double healthFraction)
        {
            if (healthFraction > 2.0 / 3.0)
            {
                return QueenMantisShade.Green;
            }

            if (healthFraction > 1.0 / 3.0)
            {
                return QueenMantisShade.White;
            }

            return QueenMantisShade.Black;
        }
    }
}

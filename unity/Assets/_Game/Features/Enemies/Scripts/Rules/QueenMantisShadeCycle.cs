namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Her shade over one fight, as plain C# so EditMode tests can cover it. It only moves forward:
    /// healing never takes her back a third. It moves one third per <see cref="TryAdvance"/>, so a
    /// hit that takes two thirds at once still shows black before the last white, each change with
    /// its own pray pause.
    /// </summary>
    public sealed class QueenMantisShadeCycle
    {
        /// <summary>Thirds she has changed shade for so far: 0, 1 or 2.</summary>
        public int ThirdsLost { get; private set; }

        /// <summary>Her shade now.</summary>
        public QueenMantisShade Shade => QueenMantisShadeRule.ForThirdsLost(ThirdsLost);

        /// <summary>
        /// Steps into the next third if her health says she has lost more than she has shown.
        /// Returns true when she stepped, which always changes her shade.
        /// </summary>
        public bool TryAdvance(double healthFraction)
        {
            if (QueenMantisShadeRule.ThirdsLost(healthFraction) <= ThirdsLost)
            {
                return false;
            }

            ThirdsLost++;
            return true;
        }
    }
}

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// What a left-click on the map does. Plain C# so EditMode tests can cover it;
    /// <see cref="TowerPlacer"/> applies the answer. A standing tower under the cursor is always
    /// selected, even with a tower in hand, so the card, U and Sell act on the tower the player clicked.
    /// </summary>
    public static class TowerClickRule
    {
        public enum Outcome
        {
            /// <summary>Nothing happens: a tower in hand over the path, or one the player can't afford.</summary>
            Nothing,

            /// <summary>The tower under the cursor becomes the selected one; any tower in hand is put away.</summary>
            SelectStanding,

            /// <summary>An empty cell with nothing in hand: the selection is cleared.</summary>
            ClearSelection,

            /// <summary>The tower in hand is built on this cell.</summary>
            Build,
        }

        /// <param name="armed">A tower is in hand (picked from the shop or with a number key).</param>
        /// <param name="towerOnCell">A tower already stands on the clicked cell.</param>
        /// <param name="blocked">The cell can't be built on (a tower or the path).</param>
        /// <param name="affordable">The wallet covers the tower in hand.</param>
        public static Outcome Decide(bool armed, bool towerOnCell, bool blocked, bool affordable)
        {
            if (towerOnCell)
            {
                return Outcome.SelectStanding;
            }

            if (!armed)
            {
                return Outcome.ClearSelection;
            }

            return !blocked && affordable ? Outcome.Build : Outcome.Nothing;
        }
    }
}

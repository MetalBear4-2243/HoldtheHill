namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// Which enemy a tower attacks when several are in range. The player sets it per tower
    /// from the tower card (the GDD's standing orders).
    /// </summary>
    public enum TargetingPriority
    {
        /// <summary>Furthest along the path, so closest to the Queen. The default.</summary>
        First,

        /// <summary>Least far along the path, hitting new arrivals first.</summary>
        Last,

        /// <summary>Nearest to the tower in a straight line.</summary>
        Closest
    }
}

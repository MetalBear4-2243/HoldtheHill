namespace HoldTheHill.Features.Combat
{
    /// <summary>
    /// An IDamageable that exposes its current and maximum health values.
    /// Used by towers for Strongest and Weakest priority targeting modes.
    /// </summary>
    public interface IHealthReadable
    {
        /// <summary>
        /// Current health remaining.
        /// </summary>
        float CurrentHealth { get; }

        /// <summary>
        /// Maximum health pool.
        /// </summary>
        float MaxHealth { get; }
    }
}

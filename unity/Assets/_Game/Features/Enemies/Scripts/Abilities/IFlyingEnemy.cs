namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// An enemy that may be in the air. Melee and lobbed towers skip an enemy whose
    /// <see cref="IsFlying"/> is true (the wasp rule); straight shots, mines and the slow aura
    /// still reach it.
    /// </summary>
    public interface IFlyingEnemy
    {
        /// <summary>True while this enemy is off the ground.</summary>
        bool IsFlying { get; }
    }
}

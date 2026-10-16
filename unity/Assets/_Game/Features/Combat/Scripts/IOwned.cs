using UnityEngine;

namespace HoldTheHill.Features.Combat
{
    /// <summary>
    /// Contract for objects that act on behalf of another entity (e.g., projectiles, hazards left by towers).
    /// Damage dealt is credited to Owner rather than the loose object itself.
    /// </summary>
    public interface IOwned
    {
        /// <summary>
        /// The GameObject to credit for actions performed by this object.
        /// </summary>
        GameObject Owner { get; }
    }
}

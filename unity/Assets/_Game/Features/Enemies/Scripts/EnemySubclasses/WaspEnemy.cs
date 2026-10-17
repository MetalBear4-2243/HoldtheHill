using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Wasp: a <see cref="BasicEnemy"/> that flies, so melee and lobbed towers cannot reach it
    /// (see <see cref="IFlyingEnemy"/>). Its stingers (hazard 2 in the GDD) come with the hazards
    /// step; for now it only flies.
    /// </summary>
    public class WaspEnemy : BasicEnemy, IFlyingEnemy
    {
        [Header("Wasp")]
        [Tooltip("While on, melee and lobbed towers skip this enemy. Off makes it an ordinary walker.")]
        [SerializeField] private bool _isFlying = true;

        /// <summary>True while this wasp is in the air.</summary>
        public bool IsFlying
        {
            get => _isFlying;
            set => _isFlying = value;
        }
    }
}

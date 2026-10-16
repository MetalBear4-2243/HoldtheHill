using UnityEngine;

namespace HoldTheHill.Features.Combat
{
    /// <summary>
    /// Contract for anything a tower, projectile or hazard can hurt or affect.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply one hit to this target.
        /// </summary>
        void TakeDamage(DamageInfo info);

        /// <summary>
        /// Apply a damage-over-time or slow status effect. Re-applying refreshes the effect.
        /// </summary>
        void ApplyStatusEffect(StatusEffectData status);

        /// <summary>
        /// True once health has run out or death sequence has started. Targeting queries skip dead targets.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Transform of this target, used for aiming, proximity, and distance checks.
        /// </summary>
        Transform Transform { get; }
    }
}

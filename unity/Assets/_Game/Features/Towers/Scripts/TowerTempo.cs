using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// How fast every tower fires right now, as one factor: 1 is normal, 2 is twice as fast.
    /// <see cref="Tower"/> divides its cooldown by it in one place; the Swarm Frenzy power-up sets
    /// it for a while. A static, like the registries, so no tower needs a reference to anything.
    /// </summary>
    public static class TowerTempo
    {
        private static float s_fireRateFactor = 1f;

        /// <summary>Multiplier on every tower's fire rate. Never below 1.</summary>
        public static float FireRateFactor
        {
            get => s_fireRateFactor;
            set => s_fireRateFactor = Mathf.Max(1f, value);
        }

        /// <summary>
        /// The cooldown after a shot. A shot fired part-way through a frame keeps the part of the
        /// frame it was late by, so a tower fires at its stated rate at any frame rate; a tower that
        /// sat idle gets no credit for the wait (<paramref name="cooldown"/> never counts more than
        /// one frame late).
        /// </summary>
        /// <param name="cooldown">What was left of the cooldown when the shot fired: zero or below.</param>
        /// <param name="interval">Seconds between shots at normal tempo.</param>
        /// <param name="deltaTime">This frame's length.</param>
        /// <param name="fireRateFactor">The tempo; 2 halves the interval.</param>
        public static float NextCooldown(float cooldown, float interval, float deltaTime, float fireRateFactor)
        {
            float late = Mathf.Clamp(-cooldown, 0f, Mathf.Max(0f, deltaTime));
            return interval / Mathf.Max(1f, fireRateFactor) - late;
        }

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_fireRateFactor = 1f;
        }
    }
}

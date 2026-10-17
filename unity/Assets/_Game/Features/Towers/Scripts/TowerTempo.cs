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

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_fireRateFactor = 1f;
        }
    }
}

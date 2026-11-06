using UnityEngine;

namespace HoldTheHill.Features.PowerUps
{
    /// <summary>
    /// The numbers behind power-ups: how often they drop, how long a drop waits on the map, and what
    /// each one does. One asset per game; data, not code (UNITY_RULES 32).
    /// </summary>
    [CreateAssetMenu(fileName = "PowerUpSettings", menuName = "Hold the Hill/Power-up Settings")]
    public class PowerUpSettings : ScriptableObject
    {
        [Header("Drops")]
        [Tooltip("Chance, 0 to 1, that a killed enemy leaves a power-up behind. Which one is an even split.")]
        [Range(0f, 1f)] public float DropChance = 0.08f;

        [Tooltip("Seconds a drop waits on the map before it fades away.")]
        [Min(1f)] public float DropLifetime = 15f;

        [Header("Swarm Frenzy")]
        [Tooltip("Seconds every tower fires faster after the slot is used.")]
        [Min(1f)] public float FrenzyDuration = 10f;

        [Tooltip("How many times faster towers fire during a frenzy. 2 halves the time between attacks.")]
        [Min(1f)] public float FrenzyFireRateFactor = 2f;

        [Header("Looks")]
        [Tooltip("Sprite for a Swarm Frenzy drop and its HUD slot.")]
        public Sprite FrenzyIcon;

        [Tooltip("Sprite for a Royal Gift drop and its HUD slot. A different shape from the frenzy, not only a different colour.")]
        public Sprite GiftIcon;

        public Color FrenzyColor = new Color(1f, 0.75f, 0.2f);
        public Color GiftColor = new Color(0.75f, 0.45f, 1f);

        /// <summary>The sprite for <paramref name="kind"/>.</summary>
        public Sprite IconFor(PowerUpKind kind)
        {
            return kind == PowerUpKind.SwarmFrenzy ? FrenzyIcon : GiftIcon;
        }

        /// <summary>The colour for <paramref name="kind"/>.</summary>
        public Color ColorFor(PowerUpKind kind)
        {
            return kind == PowerUpKind.SwarmFrenzy ? FrenzyColor : GiftColor;
        }
    }
}

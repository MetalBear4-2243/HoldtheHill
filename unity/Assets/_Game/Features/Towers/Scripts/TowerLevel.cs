using System;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// One level of a tower: what it costs to reach and the numbers the tower has while on it.
    /// A <see cref="TowerData"/> lists one per level, level 1 first; its Cost is the build price.
    /// The attack interval is the one timing number: attacks per second and DPS follow from it.
    /// Data, not code (UNITY_RULES 32).
    /// </summary>
    [Serializable]
    public struct TowerLevel
    {
        [Tooltip("Honeydew to reach this level: the build price at level 1, the upgrade price after.")]
        [Min(0)] public int Cost;

        [Tooltip("Damage per hit, projectile or mine at this level.")]
        [Min(0f)] public float Damage;

        [Tooltip("Seconds between attacks (and between mines laid). The authoritative timing value.")]
        [Min(0.05f)] public float AttackInterval;

        [Tooltip("Reach in world units at this level.")]
        [Min(0f)] public float Range;

        [Tooltip("The tower's ability at this level. Only the fields its components read matter.")]
        public TowerAbilitySettings Ability;

        /// <summary>The numbers a tower has with no definition: a plain melee tower.</summary>
        public static TowerLevel Default => new TowerLevel
        {
            Cost = 0,
            Damage = 10f,
            AttackInterval = 1f,
            Range = 2.5f,
            Ability = TowerAbilitySettings.Default,
        };

        /// <summary>Attacks per second: 1 / interval. Zero for a non-positive interval.</summary>
        public float AttacksPerSecond => AttacksPerSecondFor(AttackInterval);

        /// <summary>Theoretical damage per second against one target: damage times attacks per second.</summary>
        public float SingleTargetDps => DpsFor(Damage, AttackInterval);

        /// <summary>Attacks per second for <paramref name="attackInterval"/>; zero when the interval is not positive.</summary>
        public static float AttacksPerSecondFor(float attackInterval)
        {
            return attackInterval > 0f ? 1f / attackInterval : 0f;
        }

        /// <summary>Damage per second for one target: <paramref name="damage"/> every <paramref name="attackInterval"/> seconds.</summary>
        public static float DpsFor(float damage, float attackInterval)
        {
            return damage * AttacksPerSecondFor(attackInterval);
        }
    }
}

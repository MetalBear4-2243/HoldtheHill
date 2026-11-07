using System;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// The strength of a tower's ability at one level, beside its damage, interval and range in a
    /// <see cref="TowerLevel"/>. One flat set of fields covers every ability, so a level is one row
    /// in a table; each component reads only the fields that are its own:
    /// <see cref="Tower"/> reads the cone, <see cref="SapPuddle"/> the slow and the area,
    /// <see cref="MineLayer"/> the mines and the area, <see cref="PoisonOnHits"/> the poison.
    /// </summary>
    [Serializable]
    public struct TowerAbilitySettings
    {
        [Tooltip("Fire Ant: how wide the flame cone is, in degrees, pointed at the target. 0 hits all round.")]
        [Range(0f, 360f)] public float ConeDegrees;

        [Tooltip("Sap Ant: the puddle's reach. Carpenter Ant: the mine's blast. In world units; 0 keeps the effect prefab's own.")]
        [Min(0f)] public float AreaRadius;

        [Tooltip("Sap Ant: movement speed inside the puddle. 0.5 is half speed; 0 keeps the puddle prefab's own.")]
        [Range(0f, 1f)] public float SlowMultiplier;

        [Tooltip("Sap Ant: seconds a puddle lies there. 0 keeps the puddle prefab's own.")]
        [Min(0f)] public float SlowSeconds;

        [Tooltip("Carpenter Ant: how many of the tower's mines can lie on the path at once.")]
        [Min(0)] public int MaxMines;

        [Tooltip("Crazy Ant: the most poison stacks one enemy can carry.")]
        [Min(0)] public int PoisonMaxStacks;

        [Tooltip("Crazy Ant: damage each poison stack deals per tick, as a share of the hit damage (0.25 = a quarter).")]
        [Min(0f)] public float PoisonDamageShare;

        /// <summary>What a tower without a definition uses: the numbers the prefabs carried before 2026-11-06.</summary>
        public static TowerAbilitySettings Default => new TowerAbilitySettings
        {
            ConeDegrees = 0f,
            AreaRadius = 0f,
            SlowMultiplier = 0f,
            SlowSeconds = 0f,
            MaxMines = 3,
            PoisonMaxStacks = 5,
            PoisonDamageShare = 0.25f,
        };
    }
}

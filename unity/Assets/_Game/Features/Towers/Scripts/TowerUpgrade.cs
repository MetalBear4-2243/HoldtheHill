using System;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// One upgrade step of a tower: what it costs and the numbers the tower has afterwards.
    /// A tower prefab lists two of these for levels 2 and 3. Data, not code (UNITY_RULES 32).
    /// </summary>
    [Serializable]
    public struct TowerUpgrade
    {
        [Tooltip("Honeydew paid to reach this level.")]
        [Min(0)] public int Cost;

        [Tooltip("Reach in world units at this level.")]
        [Min(0f)] public float Range;

        [Tooltip("Damage per hit, projectile or mine at this level.")]
        [Min(0f)] public float Damage;

        [Tooltip("Seconds between attacks at this level.")]
        [Min(0.05f)] public float FireInterval;
    }
}

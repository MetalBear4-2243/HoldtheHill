using System;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The strength of an enemy's abilities, kept on its <see cref="EnemyData"/> beside its health
    /// and speed so one table holds every number. The behaviour stays on the variant's components
    /// (<see cref="RhinoBeetleShield"/>, <see cref="RollyPollyCurler"/>, <see cref="CentipedeSplitter"/>,
    /// <see cref="CentipedeSpeedUp"/>); each reads only the fields that are its own, and an enemy
    /// without the component ignores the rest.
    /// </summary>
    [Serializable]
    public struct EnemyAbilitySettings
    {
        [Header("Shield (Rhino Beetle)")]
        [Tooltip("How much damage a full shield soaks before anything reaches health. Shown apart from health.")]
        [Min(0f)] public float ShieldCapacity;

        [Tooltip("Seconds without a hit before the shield starts to grow back.")]
        [Min(0f)] public float ShieldRegrowDelay;

        [Tooltip("Shield points regained per second once it is growing back.")]
        [Min(0f)] public float ShieldRegrowPerSecond;

        [Header("Curl (Rolly Polly)")]
        [Tooltip("How many times it can curl in one life.")]
        [Min(0)] public int MaxCurls;

        [Tooltip("Path distance rolled while curled before it uncurls, in tiles.")]
        [Min(0f)] public float RollDistance;

        [Tooltip("The share of each hit that lands while curled. 0.4 means it takes 40 percent.")]
        [Range(0f, 1f)] public float CurledDamageShare;

        [Tooltip("Movement speed multiplier while rolling.")]
        [Min(0f)] public float CurledSpeedMultiplier;

        [Header("Split (Centipede)")]
        [Tooltip("How many times it splits in two on death. -1 splits until the halves have 1 HP.")]
        [Min(-1)] public int Splits;

        [Tooltip("How much smaller each half is drawn than its parent.")]
        [Range(0.1f, 1f)] public float SplitSizeMultiplier;

        [Tooltip("How far forward along the path the second half appears, in world units.")]
        [Min(0f)] public float SplitForwardDistance;

        [Tooltip("Each half's speed is its parent's times this. 1.25 means a quarter faster per split.")]
        [Min(1f)] public float ChildSpeedFactor;

        /// <summary>What a component uses on an enemy with no data asset: the numbers the components carried before 2026-11-06.</summary>
        public static EnemyAbilitySettings Default => new EnemyAbilitySettings
        {
            ShieldCapacity = 40f,
            ShieldRegrowDelay = 3f,
            ShieldRegrowPerSecond = 25f,
            MaxCurls = 3,
            RollDistance = 3f,
            CurledDamageShare = 0.4f,
            CurledSpeedMultiplier = 2f,
            Splits = -1,
            SplitSizeMultiplier = 0.8f,
            SplitForwardDistance = 1f,
            ChildSpeedFactor = 1.25f,
        };
    }
}

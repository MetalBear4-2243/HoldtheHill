using System;
using UnityEngine;

namespace HoldTheHill.Features.Combat
{
    /// <summary>
    /// A damage-over-time and/or slow that a hit or area hazard can leave behind.
    /// Serializable so designers can configure it on projectile and weapon prefabs.
    /// </summary>
    [Serializable]
    public struct StatusEffectData
    {
        [Tooltip("Identifies the effect. Re-applying the same name refreshes it instead of stacking.")]
        public string EffectName;

        [Tooltip("Damage dealt each tick. Zero for a pure slow.")]
        [Min(0)]
        public float DamagePerTick;

        [Tooltip("Seconds between ticks. Values at or below zero default to 1 second.")]
        [Min(0)]
        public float TickInterval;

        [Tooltip("Total seconds the effect lasts. Zero or less means this effect does nothing.")]
        [Min(0)]
        public float Duration;

        [Tooltip("Movement speed multiplier while active. 1 = no slow, 0.5 = half speed.")]
        [Range(0f, 1f)]
        public float SlowMultiplier;

        /// <summary>
        /// True if this effect would actually do something: Duration > 0 and (DamagePerTick > 0 or SlowMultiplier < 1).
        /// </summary>
        public bool IsValid => Duration > 0f && (DamagePerTick > 0f || SlowMultiplier < 1f);

        /// <summary>
        /// Tick interval clamped to something safe to divide by: TickInterval when above 0.01, otherwise 1.
        /// </summary>
        public float SafeTickInterval => TickInterval > 0.01f ? TickInterval : 1f;

        /// <summary>
        /// A no-op effect with SlowMultiplier 1. Use for "this weapon applies nothing".
        /// </summary>
        public static StatusEffectData None => new StatusEffectData { SlowMultiplier = 1f };
    }
}

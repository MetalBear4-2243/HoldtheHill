using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Hazards
{
    /// <summary>
    /// When the hazards of one level happen: which wave, which hazard, where, how long after the
    /// wave starts. One asset per level, read by the level's <see cref="HazardScheduler"/>.
    /// No random director (the Alpha plan): the list is the whole schedule.
    /// </summary>
    [CreateAssetMenu(fileName = "NewHazardSchedule", menuName = "Hold The Hill/Level Hazard Schedule")]
    public class LevelHazardSchedule : ScriptableObject
    {
        /// <summary>One scheduled hazard.</summary>
        [Serializable]
        public struct Entry
        {
            [Tooltip("The wave (from 1) whose start triggers this hazard.")]
            [Min(1)] public int Wave;

            [Tooltip("Seconds after the wave starts.")]
            [Min(0f)] public float Delay;

            [Tooltip("The hazard prefab to spawn.")]
            public GameObject Hazard;

            [Tooltip("Where it appears, in world units. A bird strikes here; a patch lies here; the pesticide ignores it.")]
            public Vector2 Position;
        }

        [Tooltip("Every hazard of the level, in any order.")]
        public List<Entry> Entries = new List<Entry>();
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// Every standing <see cref="Tower"/>, so hazards can find the towers under them without a
    /// scene search. The mirror of <see cref="Enemies.EnemyRegistry"/>; towers add themselves.
    /// </summary>
    public static class TowerRegistry
    {
        private static readonly List<Tower> s_all = new List<Tower>();

        /// <summary>Towers currently standing and enabled, in placement order.</summary>
        public static IReadOnlyList<Tower> All => s_all;

        internal static void Add(Tower tower)
        {
            if (!s_all.Contains(tower))
            {
                s_all.Add(tower);
            }
        }

        internal static void Remove(Tower tower)
        {
            s_all.Remove(tower);
        }

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_all.Clear();
        }
    }
}

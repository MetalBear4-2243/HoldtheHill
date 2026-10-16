using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Every living enemy that carries a <see cref="RegisteredEnemy"/>, so towers can look for
    /// targets without a scene search or physics queries. Enemies made outside the spawner
    /// (centipede children, husks) register themselves the same way.
    /// </summary>
    public static class EnemyRegistry
    {
        private static readonly List<BasicEnemy> s_all = new List<BasicEnemy>();

        /// <summary>Enemies currently alive and enabled, in registration order.</summary>
        public static IReadOnlyList<BasicEnemy> All => s_all;

        internal static void Add(BasicEnemy enemy)
        {
            if (!s_all.Contains(enemy))
            {
                s_all.Add(enemy);
            }
        }

        internal static void Remove(BasicEnemy enemy)
        {
            s_all.Remove(enemy);
        }

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_all.Clear();
        }
    }
}

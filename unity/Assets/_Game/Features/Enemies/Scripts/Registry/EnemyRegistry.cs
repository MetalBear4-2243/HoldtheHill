using System;
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

        // The same enemies as s_all, for a cheap Contains: a shot in flight asks every frame.
        private static readonly HashSet<BasicEnemy> s_registered = new HashSet<BasicEnemy>();

        /// <summary>
        /// Enemies currently alive and enabled and still on the path, in registration order. One that
        /// has reached the Queen leaves at once, before its end-of-frame Destroy.
        /// </summary>
        public static IReadOnlyList<BasicEnemy> All => s_all;

        /// <summary>
        /// True while <paramref name="enemy"/> is registered. False once it has died, reached the
        /// Queen or been disabled, so anything holding a reference can let it go.
        /// </summary>
        public static bool Contains(BasicEnemy enemy)
        {
            return enemy != null && s_registered.Contains(enemy);
        }

        /// <summary>
        /// A registered enemy was killed (not: reached the Queen). Raised by <see cref="RegisteredEnemy"/>
        /// while the enemy's transform is still valid, so the Queen Mantis can note where it fell.
        /// </summary>
        public static event Action<BasicEnemy> EnemyDied;

        internal static void Add(BasicEnemy enemy)
        {
            if (s_registered.Add(enemy))
            {
                s_all.Add(enemy);
            }
        }

        internal static void Remove(BasicEnemy enemy)
        {
            if (s_registered.Remove(enemy))
            {
                s_all.Remove(enemy);
            }
        }

        internal static void NotifyDied(BasicEnemy enemy)
        {
            EnemyDied?.Invoke(enemy);
        }

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_all.Clear();
            s_registered.Clear();
            EnemyDied = null;
        }
    }
}

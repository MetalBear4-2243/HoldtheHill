using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// What a wave sends, in order, as the spawner plays it: each grouped row
    /// (<see cref="EnemySpawnGroup"/>) unrolled into single entries, then the wave's own single
    /// entries. Plain C# so EditMode tests cover it; Jason's <see cref="EnemySpawner"/> plays the
    /// result one entry at a time, exactly as before.
    /// </summary>
    public static class WaveSpawnPlan
    {
        /// <summary>
        /// The wave's entries in spawn order. A group of N becomes N entries: the spawn interval
        /// after each but the last, the group's delay after the last. An empty or null wave gives
        /// an empty list.
        /// </summary>
        public static List<EnemySpawnEntry> Expand(Wave wave)
        {
            var entries = new List<EnemySpawnEntry>();
            if (wave == null)
            {
                return entries;
            }

            if (wave.groups != null)
            {
                for (int g = 0; g < wave.groups.Count; g++)
                {
                    EnemySpawnGroup group = wave.groups[g];
                    if (group == null)
                    {
                        continue;
                    }

                    int count = Mathf.Max(1, group.count);
                    for (int i = 0; i < count; i++)
                    {
                        entries.Add(new EnemySpawnEntry
                        {
                            enemyPrefab = group.enemyPrefab,
                            delayBeforeNext = i < count - 1 ? group.spawnInterval : group.delayAfter,
                            spawnPointOverride = null,
                        });
                    }
                }
            }

            if (wave.enemies != null)
            {
                entries.AddRange(wave.enemies);
            }

            return entries;
        }

        /// <summary>How many enemies the wave sends, groups and single entries together.</summary>
        public static int Count(Wave wave)
        {
            return Expand(wave).Count;
        }

        /// <summary>
        /// Seconds from the first spawn to the end of the last delay: how long the spawner is busy
        /// with the wave.
        /// </summary>
        public static float SpawnSeconds(Wave wave)
        {
            float total = 0f;
            List<EnemySpawnEntry> entries = Expand(wave);
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] != null)
                {
                    total += Mathf.Max(0f, entries[i].delayBeforeNext);
                }
            }

            return total;
        }

        /// <summary>
        /// Folds single entries into groups: every run of the same prefab with the same delay
        /// becomes one row (its interval and its delay after both that delay). Entries with a
        /// custom spawn point stay single, since a group has none. Exact: expanding the result
        /// gives the same prefabs with the same delays in the same order.
        /// </summary>
        public static List<EnemySpawnGroup> Group(IReadOnlyList<EnemySpawnEntry> entries, List<EnemySpawnEntry> leftSingle)
        {
            var groups = new List<EnemySpawnGroup>();
            leftSingle.Clear();
            if (entries == null)
            {
                return groups;
            }

            EnemySpawnGroup open = null;
            for (int i = 0; i < entries.Count; i++)
            {
                EnemySpawnEntry entry = entries[i];
                if (entry == null)
                {
                    continue;
                }

                if (entry.spawnPointOverride != null)
                {
                    // Order matters, and a single entry can only follow every group: keep it single
                    // only when nothing grouped would have to come after it.
                    leftSingle.Add(entry);
                    open = null;
                    continue;
                }

                if (leftSingle.Count > 0)
                {
                    // Something single already sits after the groups; everything from here stays single too.
                    leftSingle.Add(entry);
                    continue;
                }

                if (open != null && open.enemyPrefab == entry.enemyPrefab && Mathf.Approximately(open.spawnInterval, entry.delayBeforeNext))
                {
                    open.count++;
                    open.delayAfter = entry.delayBeforeNext;
                    continue;
                }

                open = new EnemySpawnGroup
                {
                    enemyPrefab = entry.enemyPrefab,
                    count = 1,
                    spawnInterval = entry.delayBeforeNext,
                    delayAfter = entry.delayBeforeNext,
                };
                groups.Add(open);
            }

            return groups;
        }
    }
}

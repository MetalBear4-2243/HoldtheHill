using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEngine;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// The checks the Balance Table window shows beside the tables, as plain functions over the
    /// data so EditMode tests cover them. Each returns the problems found, in the words the window
    /// shows; an empty list means the data is sound.
    /// </summary>
    public static class BalanceValidation
    {
        /// <summary>The shortest attack interval the game accepts (TowerLevel's Min).</summary>
        public const float MinAttackInterval = 0.05f;

        /// <summary>
        /// A melee tower on the outer diagonal of a road corner is the square root of 2 cells from
        /// the road's centre line; its range must beat that at every level (TowerRangeDataTests).
        /// </summary>
        public static readonly float MeleeMinRange = Mathf.Sqrt(2f);

        /// <summary>What the window knows about a tower's prefab, so the rules can tell a melee tower from the rest.</summary>
        public struct TowerShape
        {
            /// <summary>The prefab fires a projectile.</summary>
            public bool HasProjectile;

            /// <summary>The prefab works through an ability (mines, a puddle) instead of attacking.</summary>
            public bool HasAbility;

            /// <summary>The prefab's Tower points back at the data asset.</summary>
            public bool PrefabPointsBack;
        }

        /// <summary>Problems with one tower's definition.</summary>
        public static List<string> CheckTower(TowerData data, TowerShape shape)
        {
            var problems = new List<string>();
            string name = data != null ? data.name : "tower";
            if (data == null)
            {
                problems.Add("A tower definition is missing.");
                return problems;
            }

            if (data.Prefab == null)
            {
                problems.Add($"{name}: no prefab; the shop card would build nothing.");
            }
            else if (!shape.PrefabPointsBack)
            {
                problems.Add($"{name}: its prefab's Tower points at a different definition (or none).");
            }

            if (data.LevelCount == 0)
            {
                problems.Add($"{name}: no levels; it needs at least level 1.");
                return problems;
            }

            if (data.honeydewCost <= 0)
            {
                problems.Add($"{name}: the build price (level 1 cost) is 0.");
            }

            bool melee = !shape.HasProjectile && !shape.HasAbility;
            for (int i = 0; i < data.LevelCount; i++)
            {
                TowerLevel level = data.Levels[i];
                int number = i + 1;
                if (i > 0 && level.Cost <= 0)
                {
                    problems.Add($"{name} level {number}: the upgrade is free.");
                }

                if (level.AttackInterval < MinAttackInterval)
                {
                    problems.Add($"{name} level {number}: the attack interval is under {MinAttackInterval} s.");
                }

                if (level.Range <= 0f)
                {
                    problems.Add($"{name} level {number}: the range is 0.");
                }
                else if (melee && level.Range <= MeleeMinRange)
                {
                    problems.Add($"{name} level {number}: a melee range of {level.Range} misses the road from a corner's outer diagonal (needs more than {MeleeMinRange:F2}).");
                }

                if (melee && level.Damage <= 0f)
                {
                    problems.Add($"{name} level {number}: a melee tower with 0 damage attacks nothing.");
                }

                if (i > 0 && level.SingleTargetDps < data.Levels[i - 1].SingleTargetDps && level.Damage > 0f)
                {
                    problems.Add($"{name} level {number}: its DPS ({level.SingleTargetDps:F1}) is below level {i}'s ({data.Levels[i - 1].SingleTargetDps:F1}).");
                }
            }

            return problems;
        }

        /// <summary>Which ability components an enemy's prefab carries.</summary>
        public struct EnemyShape
        {
            public bool HasShield;
            public bool HasCurl;
            public bool HasSplit;
        }

        /// <summary>Problems with one enemy's definition.</summary>
        public static List<string> CheckEnemy(EnemyData data, EnemyShape shape)
        {
            var problems = new List<string>();
            if (data == null)
            {
                problems.Add("An enemy definition is missing.");
                return problems;
            }

            string name = data.name;
            if (data.MaxHealth <= 0.0)
            {
                problems.Add($"{name}: health is 0; it would die on spawn.");
            }

            if (data.MoveSpeed <= 0f)
            {
                problems.Add($"{name}: speed is 0; it would never arrive.");
            }

            EnemyAbilitySettings a = data.Abilities;
            if (shape.HasShield && a.ShieldCapacity <= 0f)
            {
                problems.Add($"{name}: has a shield component but a shield capacity of 0.");
            }

            if (shape.HasCurl && a.MaxCurls <= 0)
            {
                problems.Add($"{name}: has a curl component but 0 curls.");
            }

            if (shape.HasSplit && a.Splits == 0)
            {
                problems.Add($"{name}: has a splitter but 0 splits (-1 is unlimited).");
            }

            if (shape.HasSplit && a.ChildSpeedFactor < 1f)
            {
                problems.Add($"{name}: the halves' speed factor is under 1, so they would slow down.");
            }

            return problems;
        }

        /// <summary>Problems with a wave list: empty waves, rows that send nothing, entries pointing at nothing.</summary>
        public static List<string> CheckWaves(MapWaveDataSO waves)
        {
            var problems = new List<string>();
            if (waves == null)
            {
                problems.Add("No wave asset.");
                return problems;
            }

            if (waves.waves == null || waves.waves.Count == 0)
            {
                problems.Add($"{waves.name}: no waves.");
                return problems;
            }

            for (int w = 0; w < waves.waves.Count; w++)
            {
                Wave wave = waves.waves[w];
                string label = $"{waves.name} {wave.waveName}";
                if (WaveSpawnPlan.Count(wave) == 0)
                {
                    problems.Add($"{label}: sends nothing.");
                    continue;
                }

                for (int g = 0; wave.groups != null && g < wave.groups.Count; g++)
                {
                    EnemySpawnGroup group = wave.groups[g];
                    if (group == null || group.enemyPrefab == null)
                    {
                        problems.Add($"{label} row {g + 1}: no enemy.");
                    }
                    else if (group.enemyPrefab.GetComponent<BasicEnemy>() == null)
                    {
                        problems.Add($"{label} row {g + 1}: {group.enemyPrefab.name} is no enemy.");
                    }

                    if (group != null && group.count < 1)
                    {
                        problems.Add($"{label} row {g + 1}: count under 1.");
                    }
                }

                for (int e = 0; wave.enemies != null && e < wave.enemies.Count; e++)
                {
                    if (wave.enemies[e] == null || wave.enemies[e].enemyPrefab == null)
                    {
                        problems.Add($"{label} entry {e + 1}: no enemy.");
                    }
                }
            }

            return problems;
        }

        /// <summary>
        /// Problems with where a level's spawner takes its waves from. The override is the one
        /// place to edit; a lookup list or inline configurations beside it are a second place that
        /// does nothing while the override is set.
        /// </summary>
        public static List<string> CheckWaveSource(string level, MapWaveDataSO assigned, int lookupCount, int inlineCount)
        {
            var problems = new List<string>();
            if (assigned == null)
            {
                problems.Add($"{level}: no wave asset assigned to the spawner's override; it would fall back to the lookup list or inline waves.");
            }

            if (lookupCount > 0)
            {
                problems.Add($"{level}: the spawner also lists {lookupCount} lookup asset(s), which the assigned asset overrides. Clear the list so there is one place to edit.");
            }

            if (inlineCount > 0)
            {
                problems.Add($"{level}: the spawner also holds {inlineCount} inline configuration(s), which the assigned asset overrides. Clear them so there is one place to edit.");
            }

            return problems;
        }
    }
}

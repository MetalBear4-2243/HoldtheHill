//Adam Montgomery
//Gemini generated code
//10.9.2026

using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// One tower, as the designer tunes it: what the shop shows (name, icon), the prefab the placer
    /// builds, and a table of levels (price, damage, attack interval, range, ability strength).
    /// The shop cards, the placer and the placed towers all read these assets (in
    /// Features/Towers/Data); the Balance Table window (Tools > Hold the Hill) edits them side by side.
    /// </summary>
    [CreateAssetMenu(fileName = "New Tower", menuName = "Hold the Hill/Tower Data")]
    public class TowerData : ScriptableObject
    {
        [Header("Shop Display")]
        public string towerName;
        public Sprite towerIcon;

        [Header("Tower")]
        [Tooltip("The prefab the placer builds for this card. Its Tower points back here.")]
        [SerializeField] private Tower _prefab;

        [Header("Levels")]
        [Tooltip("Level 1 first; its Cost is the build price. Each later entry is an upgrade: its price and the numbers the tower has afterwards.")]
        [SerializeField] private TowerLevel[] _levels = { TowerLevel.Default };

        /// <summary>The build price: level 1's cost. The shop card and the placer read it under this name.</summary>
        public int honeydewCost => LevelCount > 0 ? _levels[0].Cost : 0;

        /// <summary>The prefab the placer builds for this tower.</summary>
        public Tower Prefab => _prefab;

        /// <summary>How many levels the tower has, counting level 1.</summary>
        public int LevelCount => _levels != null ? _levels.Length : 0;

        /// <summary>Every level, level 1 first.</summary>
        public IReadOnlyList<TowerLevel> Levels => _levels;

        /// <summary>
        /// The numbers at <paramref name="level"/> (from 1). A level past the table gives the last
        /// one; an empty table gives <see cref="TowerLevel.Default"/>.
        /// </summary>
        public TowerLevel LevelAt(int level)
        {
            if (LevelCount == 0)
            {
                return TowerLevel.Default;
            }

            return _levels[Mathf.Clamp(level, 1, LevelCount) - 1];
        }
    }
}

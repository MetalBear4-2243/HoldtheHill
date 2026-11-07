using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// The Crazy Ant's poison: every few hits in a row on the same enemy (two by default) add a poison
    /// stack to it (<see cref="PoisonStacks"/>). "In a row" means on consecutive swings; a swing that
    /// misses the enemy (it walked out, or the tower was stunned) starts its count again. Each stack
    /// ticks a share of the tower's hit damage; the share and the most stacks an enemy carries are
    /// the tower's <see cref="TowerAbilitySettings"/> at its level, so both grow with upgrades.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tower))]
    public class PoisonOnHits : MonoBehaviour
    {
        [Tooltip("Hits in a row on one enemy that add a stack.")]
        [SerializeField, Min(1)] private int _hitsPerStack = 2;

        [Tooltip("Seconds between poison ticks.")]
        [SerializeField, Min(0.05f)] private float _tickInterval = 0.5f;

        [Tooltip("Seconds the poison lasts after the last new stack.")]
        [SerializeField, Min(0.1f)] private float _seconds = 3f;

        [Header("Look")]
        [Tooltip("Optional: worn by a poisoned enemy (FxStatusOverlay Poison).")]
        [SerializeField] private Sprite[] _overlayFrames = System.Array.Empty<Sprite>();

        [Tooltip("Seconds each overlay frame shows.")]
        [SerializeField, Min(0.01f)] private float _overlayFrameSeconds = 0.15f;

        private struct Count
        {
            public int Swing;
            public int Hits;
        }

        private readonly Dictionary<BasicEnemy, Count> _counts = new Dictionary<BasicEnemy, Count>();
        private readonly List<BasicEnemy> _stale = new List<BasicEnemy>();
        private Tower _tower;
        private int _swing = 1;

        private void Awake()
        {
            _tower = GetComponent<Tower>();
        }

        private void OnEnable()
        {
            _tower.HitEnemy += HandleHit;
            _tower.Attacked += HandleSwingDone;
        }

        private void OnDisable()
        {
            _tower.HitEnemy -= HandleHit;
            _tower.Attacked -= HandleSwingDone;
            _counts.Clear();
        }

        /// <summary>
        /// The count rule: the hits in a row after a hit on swing <paramref name="swing"/>, given the
        /// enemy was last hit on <paramref name="lastSwing"/> with <paramref name="hits"/> in a row.
        /// <paramref name="addsStack"/> is true when that reaches <paramref name="hitsPerStack"/>, and
        /// the count then starts again from none.
        /// </summary>
        public static int NextHits(int lastSwing, int hits, int swing, int hitsPerStack, out bool addsStack)
        {
            int next = lastSwing == swing - 1 ? hits + 1 : 1;
            addsStack = next >= Mathf.Max(1, hitsPerStack);
            return addsStack ? 0 : next;
        }

        private void HandleHit(BasicEnemy enemy)
        {
            _counts.TryGetValue(enemy, out Count count);
            int hits = NextHits(count.Swing, count.Hits, _swing, _hitsPerStack, out bool addsStack);
            _counts[enemy] = new Count { Swing = _swing, Hits = hits };

            if (addsStack)
            {
                TowerAbilitySettings ability = _tower.Ability;
                PoisonStacks.AddStack(enemy, _tower.Damage * ability.PoisonDamageShare, _tickInterval, _seconds,
                    Mathf.Max(1, ability.PoisonMaxStacks), _overlayFrames, _overlayFrameSeconds);
            }
        }

        // After each swing: forget enemies this swing did not hit (their run is over) and the dead.
        private void HandleSwingDone()
        {
            _stale.Clear();
            foreach (KeyValuePair<BasicEnemy, Count> pair in _counts)
            {
                if (pair.Key == null || pair.Key.IsDead || pair.Value.Swing != _swing)
                {
                    _stale.Add(pair.Key);
                }
            }

            for (int i = 0; i < _stale.Count; i++)
            {
                _counts.Remove(_stale[i]);
            }

            _swing++;
        }
    }
}

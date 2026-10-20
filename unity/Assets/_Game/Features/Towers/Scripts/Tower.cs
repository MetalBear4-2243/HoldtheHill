using System;
using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// A placed tower. Knows its grid cell, picks a target in range by its
    /// <see cref="TargetingPriority"/>, and attacks it on a cooldown: with a projectile when
    /// it has one, otherwise with a direct melee hit. Numbers live on the prefab variants,
    /// one set per level; <see cref="Upgrade"/> moves to the next set.
    /// </summary>
    /// <remarks>
    /// Towers that do something else in range (slow, lay mines) carry a second component that
    /// reads <see cref="Range"/> and <see cref="Damage"/>; a tower with no damage and no
    /// projectile does not attack on its own.
    /// </remarks>
    public class Tower : MonoBehaviour
    {
        [Header("Range")]
        [Tooltip("How far this tower can reach, in world units.")]
        [SerializeField, Min(0f)] private float _range = 2.5f;

        [Header("Attack")]
        [Tooltip("Damage dealt per hit or per projectile. Zero with no projectile means the tower does not attack.")]
        [SerializeField, Min(0f)] private float _damage = 10f;

        [Tooltip("Seconds between attacks.")]
        [SerializeField, Min(0.05f)] private float _fireInterval = 1f;

        [Tooltip("Shot fired at the target. Leave empty for a melee tower that hits directly.")]
        [SerializeField] private Projectile _projectilePrefab;

        [Tooltip("Where shots leave the tower. Defaults to the tower itself.")]
        [SerializeField] private Transform _muzzle;

        [Header("Targeting")]
        [Tooltip("Which enemy to attack when several are in range.")]
        [SerializeField] private TargetingPriority _priority = TargetingPriority.First;

        [Header("Upgrades")]
        [Tooltip("Levels 2 and 3, in order: what each costs and the numbers the tower has afterwards.")]
        [SerializeField] private TowerUpgrade[] _upgrades = new TowerUpgrade[0];

        [Tooltip("How much bigger the tower is drawn with each level, so an upgrade is visible.")]
        [SerializeField, Min(1f)] private float _scalePerLevel = 1.12f;

        private readonly List<TargetPicker.Candidate> _candidates = new List<TargetPicker.Candidate>();
        private readonly List<BasicEnemy> _inRange = new List<BasicEnemy>();
        private float _cooldown;

        /// <summary>Raised after an upgrade, with the new level. The card and visuals listen.</summary>
        public event Action<int> Upgraded;

        /// <summary>How far this tower can reach, in world units.</summary>
        public float Range => _range;

        /// <summary>Damage dealt per hit.</summary>
        public float Damage => _damage;

        /// <summary>Seconds between attacks.</summary>
        public float FireInterval => _fireInterval;

        /// <summary>Which grid cell this tower occupies.</summary>
        public Vector2Int Cell { get; set; }

        /// <summary>The shop entry this tower was bought from, for its name on the card. Set by the placer.</summary>
        public TowerData ShopData { get; set; }

        /// <summary>Current level, from 1.</summary>
        public int Level { get; private set; } = 1;

        /// <summary>The last level this tower can reach.</summary>
        public int MaxLevel => 1 + _upgrades.Length;

        /// <summary>Honeydew for the next level, or 0 at the last level.</summary>
        public int NextUpgradeCost => Level < MaxLevel ? _upgrades[Level - 1].Cost : 0;

        /// <summary>Which enemy this tower prefers to attack. Set from the tower card.</summary>
        public TargetingPriority Priority
        {
            get => _priority;
            set => _priority = value;
        }

        /// <summary>The enemy being attacked this frame, or null.</summary>
        public BasicEnemy CurrentTarget { get; private set; }

        private bool Attacks => _damage > 0f || _projectilePrefab != null;

        private void Awake()
        {
            if (_muzzle == null)
            {
                _muzzle = transform;
            }
        }

        private void Update()
        {
            CurrentTarget = AcquireTarget();

            _cooldown -= Time.deltaTime;
            if (CurrentTarget == null || _cooldown > 0f || !Attacks)
            {
                return;
            }

            _cooldown = _fireInterval;
            Attack(CurrentTarget);
        }

        /// <summary>
        /// Moves to the next level and takes its numbers. False at the last level. The caller
        /// (the placer) has already checked the price and paid.
        /// </summary>
        public bool Upgrade()
        {
            if (Level >= MaxLevel)
            {
                return false;
            }

            TowerUpgrade next = _upgrades[Level - 1];
            Level++;
            _range = next.Range;
            _damage = next.Damage;
            _fireInterval = next.FireInterval;
            transform.localScale *= _scalePerLevel;

            Upgraded?.Invoke(Level);
            return true;
        }

        /// <summary>Picks the best living enemy in range for this tower's priority, or null.</summary>
        public BasicEnemy AcquireTarget()
        {
            _inRange.Clear();
            _candidates.Clear();

            Vector3 here = transform.position;
            float rangeSqr = _range * _range;
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;

            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy == null || enemy.IsDead)
                {
                    continue;
                }

                Vector3 position = enemy.transform.position;
                float sqrDistance = (position - here).sqrMagnitude;
                if (sqrDistance > rangeSqr)
                {
                    continue;
                }

                _inRange.Add(enemy);
                _candidates.Add(new TargetPicker.Candidate
                {
                    PathProgress = TargetPicker.PathProgress(enemy.PathCoordinates, enemy.CurrentWaypointIndex, position),
                    SqrDistance = sqrDistance,
                });
            }

            int best = TargetPicker.Pick(_candidates, _priority);
            return best >= 0 ? _inRange[best] : null;
        }

        private void Attack(BasicEnemy target)
        {
            if (_projectilePrefab == null)
            {
                target.TakeDamage(_damage);
                return;
            }

            Projectile shot = Instantiate(_projectilePrefab, _muzzle.position, Quaternion.identity);
            shot.Launch(target, _damage);
        }

        // Draws the range circle, and the current target, in the Scene view when the tower is selected.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _range);

            if (Application.isPlaying && CurrentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, CurrentTarget.transform.position);
            }
        }
    }
}

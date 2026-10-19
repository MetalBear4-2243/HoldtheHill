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
    /// Towers that do something else in range (slow, lay mines) carry an <see cref="ITowerAbility"/>
    /// component that reads <see cref="Range"/> and <see cref="Damage"/>; such a tower, or one with
    /// no damage and no projectile, does not attack on its own.
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

        [Header("Condition")]
        [Tooltip("How the sprite is tinted while burrowed: sunk into the ground, out of the fight.")]
        [SerializeField] private Color _burrowedTint = new Color(0.5f, 0.45f, 0.4f, 0.6f);

        [Tooltip("How the sprite is tinted while stunned by a hazard.")]
        [SerializeField] private Color _stunnedTint = new Color(0.55f, 0.7f, 1f, 1f);

        [Tooltip("How the sprite is tinted while a hazard makes the tower miss.")]
        [SerializeField] private Color _missingTint = new Color(1f, 0.85f, 0.45f, 1f);

        private readonly List<TargetPicker.Candidate> _candidates = new List<TargetPicker.Candidate>();
        private readonly List<BasicEnemy> _inRange = new List<BasicEnemy>();
        private readonly TowerCondition _condition = new TowerCondition();
        private SpriteRenderer _sprite;
        private Color _baseColor = Color.white;
        private float _cooldown;
        private bool _hasAbility;

        /// <summary>Raised after an upgrade, with the new level. The card and visuals listen.</summary>
        public event Action<int> Upgraded;

        /// <summary>Raised when the tower is burrowed or brought back up, with the new state.</summary>
        public event Action<bool> BurrowChanged;

        /// <summary>Raised just before a hazard removes the tower, so the placer can free its cell.</summary>
        public event Action<Tower> Removed;

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

        /// <summary>Every Honeydew paid for this tower so far: the build price plus each upgrade. The placer keeps it.</summary>
        public int HoneydewSpent { get; set; }

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

        /// <summary>What is being done to this tower and what it may still do.</summary>
        public TowerCondition Condition => _condition;

        /// <summary>
        /// The burrow order. A burrowed tower does not act and is safe from the bird and the
        /// pesticide. Set from the tower card or the B key.
        /// </summary>
        public bool Burrowed
        {
            get => _condition.Burrowed;
            set
            {
                if (_condition.Burrowed == value)
                {
                    return;
                }

                _condition.Burrowed = value;
                CurrentTarget = null;
                RefreshTint();
                BurrowChanged?.Invoke(value);
            }
        }

        /// <summary>True when the tower may aim, fire, slow or lay mines. Companions check this.</summary>
        public bool CanAct => _condition.CanAct;

        public bool IsStunned => _condition.IsStunned;

        public bool IsMissing => _condition.IsMissing;

        /// <summary>
        /// False for a melee or lobbed tower: it cannot reach a flying enemy (the wasp rule).
        /// Follows from how the tower attacks, so no prefab needs a flag.
        /// </summary>
        public bool ReachesFlying => TargetPicker.CanReach(
            enemyFlying: true, hasProjectile: _projectilePrefab != null,
            lobbed: _projectilePrefab != null && _projectilePrefab.IsLobbed);

        // A mine layer or a slow aura does the work; the tower only lends it its numbers.
        private bool Attacks => !_hasAbility && (_damage > 0f || _projectilePrefab != null);

        private void Awake()
        {
            if (_muzzle == null)
            {
                _muzzle = transform;
            }

            _hasAbility = GetComponent<ITowerAbility>() != null;

            _sprite = GetComponentInChildren<SpriteRenderer>();
            if (_sprite != null)
            {
                _baseColor = _sprite.color;
            }
        }

        private void OnEnable()
        {
            TowerRegistry.Add(this);
        }

        private void OnDisable()
        {
            TowerRegistry.Remove(this);
        }

        private void Update()
        {
            if (_condition.Tick(Time.deltaTime))
            {
                RefreshTint();
            }

            _cooldown -= Time.deltaTime;
            if (!_condition.CanAct)
            {
                CurrentTarget = null;
                return;
            }

            CurrentTarget = AcquireTarget();
            if (CurrentTarget == null || _cooldown > 0f || !Attacks)
            {
                return;
            }

            // The one place the Swarm Frenzy's tempo applies.
            _cooldown = _fireInterval / TowerTempo.FireRateFactor;
            Attack(CurrentTarget);
        }

        /// <summary>A hazard stuns the tower for <paramref name="seconds"/>: it does nothing meanwhile.</summary>
        public void Stun(float seconds)
        {
            _condition.Stun(seconds);
            RefreshTint();
        }

        /// <summary>A hazard makes the tower miss for <paramref name="seconds"/>: it fires, to no effect.</summary>
        public void Miss(float seconds)
        {
            _condition.Miss(seconds);
            RefreshTint();
        }

        /// <summary>A hazard takes the tower off the map. Tells the placer first, then destroys it.</summary>
        public void Remove()
        {
            Removed?.Invoke(this);
            Destroy(gameObject);
        }

        // One tint at a time, worst first, so every state is visible and not by colour alone:
        // a burrowed tower is also faded, a stunned one stops moving its target line.
        private void RefreshTint()
        {
            if (_sprite == null)
            {
                return;
            }

            Color tint = _condition.Burrowed ? _burrowedTint
                : _condition.IsStunned ? _stunnedTint
                : _condition.IsMissing ? _missingTint
                : Color.white;
            _sprite.color = _baseColor * tint;
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
            bool reachesFlying = ReachesFlying;
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;

            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy == null || enemy.IsDead)
                {
                    continue;
                }

                // The wasp rule: a melee or lobbed tower does not even aim at a flyer.
                if (!reachesFlying && enemy is IFlyingEnemy flyer && flyer.IsFlying)
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
            // A missing tower goes through the motions; its hits carry no damage.
            float damage = _condition.HitsLand ? _damage : 0f;

            if (_projectilePrefab == null)
            {
                target.TakeDamage(damage);
                return;
            }

            Projectile shot = Instantiate(_projectilePrefab, _muzzle.position, Quaternion.identity);
            shot.Launch(target, damage);
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

using System;
using System.Collections.Generic;
using UnityEngine;
using HoldTheHill.Features.Combat;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Baseline component providing core stats, health management, status effect handling,
    /// and waypoint traversal along a coordinate path.
    /// Implements IDamageable and IHealthReadable to interact seamlessly with towers, weapons, and hazards.
    /// </summary>
    [DisallowMultipleComponent]
    public class BasicEnemy : MonoBehaviour, IDamageable, IHealthReadable
    {
        [Header("Health & Combat")]
        [Tooltip("Maximum health of this enemy.")]
        [SerializeField] protected double _maxHealth = 100.0;

        [Tooltip("Damage dealt by this enemy (e.g. to the hill or player base upon reaching destination).")]
        [SerializeField] protected int _damage = 1;

        [Header("Path & Movement")]
        [Tooltip("Movement speed along the path in world units per second.")]
        [SerializeField] protected float _moveSpeed = 2f;

        [Tooltip("Distance threshold to consider a waypoint coordinate reached.")]
        [SerializeField] protected float _waypointTolerance = 0.05f;

        [Tooltip("Initial path coordinates in world space. Can also be assigned dynamically via SetPath().")]
        [SerializeField] protected List<Vector3> _pathCoordinates = new List<Vector3>();

        [Header("Visuals")]
        [Tooltip("If true and a SpriteRenderer is present, flips sprite on the X axis to match movement direction.")]
        [SerializeField] protected bool _flipSpriteFacing = true;

        protected double _currentHealth;
        protected int _currentWaypointIndex;
        protected float _speedMultiplier = 1f;
        protected SpriteRenderer _spriteRenderer;

        /// <summary>
        /// Internal tracking container for an active status effect (slow / DoT).
        /// </summary>
        protected class ActiveStatusEffect
        {
            public StatusEffectData Data;
            public float RemainingDuration;
            public float TimeUntilNextTick;
        }

        protected readonly Dictionary<string, ActiveStatusEffect> _activeStatusEffects = new Dictionary<string, ActiveStatusEffect>();

        // Events for decoupled listeners (UI, spawner tracking, audio, combat, tower credit)
        public event Action<BasicEnemy> OnDied;
        public event Action<BasicEnemy> OnDestinationReached;
        public static event Action<BasicEnemy> OnAnyDestinationReached;
        public event Action<double, double> OnHealthChanged;
        public event Action<BasicEnemy, DamageInfo, float> OnDamaged;
        public static event Action<BasicEnemy, DamageInfo, float> OnAnyDamaged;

        /// <summary>Maximum health value as a double.</summary>
        public double MaxHealth => _maxHealth;

        /// <summary>Current health value as a double.</summary>
        public double CurrentHealth => _currentHealth;

        /// <summary>Damage value as an int.</summary>
        public int Damage
        {
            get => _damage;
            set => _damage = value;
        }

        /// <summary>Movement speed in world units per second.</summary>
        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = Mathf.Max(0f, value);
        }

        /// <summary>Current movement speed multiplier applied by active status effects (slows).</summary>
        public float SpeedMultiplier => _speedMultiplier;

        /// <summary>List of coordinates that this enemy follows.</summary>
        public IReadOnlyList<Vector3> PathCoordinates => _pathCoordinates;

        /// <summary>Index of the next waypoint coordinate being pursued.</summary>
        public int CurrentWaypointIndex
        {
            get => _currentWaypointIndex;
            set => _currentWaypointIndex = Mathf.Max(0, value);
        }

        /// <summary>True if health has been depleted and death sequence has started.</summary>
        public bool IsDead { get; protected set; }

        /// <summary>Transform of this enemy, satisfying IDamageable for aiming and distance queries.</summary>
        public Transform Transform => transform;

        /// <summary>Current health exposed as float for tower targeting priorities (IHealthReadable).</summary>
        float IHealthReadable.CurrentHealth => (float)_currentHealth;

        /// <summary>Max health exposed as float for tower targeting priorities (IHealthReadable).</summary>
        float IHealthReadable.MaxHealth => (float)_maxHealth;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticEvents()
        {
            OnAnyDestinationReached = null;
            OnAnyDamaged = null;
        }

        protected virtual void Awake()
        {
            _currentHealth = _maxHealth;
            _speedMultiplier = 1f;
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected virtual void Start()
        {
            // If coordinates were not assigned prior to Start, attempt to locate an EnemyPath in the scene.
            if (_pathCoordinates.Count == 0)
            {
                var scenePath = FindAnyObjectByType<EnemyPath>();
                if (scenePath != null && scenePath.Waypoints != null && scenePath.Waypoints.Count > 0)
                {
                    SetPath(scenePath.Waypoints, teleportToStart: false);
                }
            }
        }

        protected virtual void Update()
        {
            if (IsDead)
            {
                return;
            }

            UpdateStatusEffects(Time.deltaTime);

            if (IsDead)
            {
                return;
            }

            TraversePath();
        }

        protected virtual void OnDisable()
        {
            _activeStatusEffects.Clear();
            _speedMultiplier = 1f;
        }

        /// <summary>
        /// Ticks all active status effects, handles periodic damage-over-time, and clears expired effects.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last frame.</param>
        public virtual void UpdateStatusEffects(float deltaTime)
        {
            if (_activeStatusEffects.Count == 0)
            {
                return;
            }

            List<string> expiredKeys = null;

            foreach (var kvp in _activeStatusEffects)
            {
                ActiveStatusEffect effect = kvp.Value;
                effect.RemainingDuration -= deltaTime;

                if (effect.Data.DamagePerTick > 0f)
                {
                    effect.TimeUntilNextTick -= deltaTime;
                    if (effect.TimeUntilNextTick <= 0f)
                    {
                        effect.TimeUntilNextTick += effect.Data.SafeTickInterval;
                        TakeDamage(new DamageInfo(
                            effect.Data.DamagePerTick,
                            gameObject,
                            transform.position,
                            DamageType.Poison
                        ));

                        if (IsDead)
                        {
                            break;
                        }
                    }
                }

                if (effect.RemainingDuration <= 0f)
                {
                    if (expiredKeys == null)
                    {
                        expiredKeys = new List<string>();
                    }
                    expiredKeys.Add(kvp.Key);
                }
            }

            if (expiredKeys != null)
            {
                for (int i = 0; i < expiredKeys.Count; i++)
                {
                    _activeStatusEffects.Remove(expiredKeys[i]);
                }
                RecalculateSpeedMultiplier();
            }
        }

        /// <summary>
        /// Recalculates the speed multiplier based on the strongest active slow effect.
        /// </summary>
        protected virtual void RecalculateSpeedMultiplier()
        {
            if (_activeStatusEffects.Count == 0)
            {
                _speedMultiplier = 1f;
                return;
            }

            float minSlow = 1f;
            foreach (var effect in _activeStatusEffects.Values)
            {
                if (effect.Data.SlowMultiplier < minSlow)
                {
                    minSlow = effect.Data.SlowMultiplier;
                }
            }

            _speedMultiplier = Mathf.Clamp(minSlow, 0f, 1f);
        }

        /// <summary>
        /// Moves the enemy towards the current target waypoint, scaled by moveSpeed and active slows.
        /// </summary>
        protected virtual void TraversePath()
        {
            if (_pathCoordinates == null || _currentWaypointIndex >= _pathCoordinates.Count)
            {
                return;
            }

            Vector3 target = _pathCoordinates[_currentWaypointIndex];
            target.z = transform.position.z; // Keep within the 2D plane

            Vector3 currentPos = transform.position;
            transform.position = Vector3.MoveTowards(
                currentPos,
                target,
                _moveSpeed * _speedMultiplier * Time.deltaTime
            );

            UpdateFacing(target - currentPos);

            if (Vector3.Distance(transform.position, target) <= _waypointTolerance)
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex >= _pathCoordinates.Count)
                {
                    OnReachedDestination();
                }
            }
        }

        /// <summary>
        /// Flips the sprite horizontally according to travel direction.
        /// </summary>
        protected virtual void UpdateFacing(Vector3 direction)
        {
            if (_flipSpriteFacing && _spriteRenderer != null && Mathf.Abs(direction.x) > 0.001f)
            {
                _spriteRenderer.flipX = direction.x < 0f;
            }
        }

        /// <summary>
        /// Applies damage via DamageInfo (IDamageable).
        /// Subtracts health, raises OnDamaged and OnAnyDamaged with the hit details, and triggers Die() at 0 health.
        /// </summary>
        /// <param name="info">Details of the incoming hit from a tower, weapon, or hazard.</param>
        public virtual void TakeDamage(DamageInfo info)
        {
            if (info.Amount <= 0f || IsDead)
            {
                return;
            }

            double previousHealth = _currentHealth;
            TakeDamage((double)info.Amount);
            float appliedDamage = (float)(previousHealth - _currentHealth);

            OnDamaged?.Invoke(this, info, appliedDamage);
            OnAnyDamaged?.Invoke(this, info, appliedDamage);
        }

        /// <summary>
        /// Applies damage directly as a raw double amount.
        /// </summary>
        /// <param name="damageAmount">Amount of damage to inflict.</param>
        public virtual void TakeDamage(double damageAmount)
        {
            if (damageAmount <= 0.0 || IsDead)
            {
                return;
            }

            _currentHealth = Math.Max(0.0, _currentHealth - damageAmount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0.0)
            {
                Die();
            }
        }

        /// <summary>
        /// Applies a damage-over-time and/or slow status effect (IDamageable).
        /// Re-applying an effect with the same EffectName refreshes it.
        /// </summary>
        /// <param name="status">The status effect data to apply.</param>
        public virtual void ApplyStatusEffect(StatusEffectData status)
        {
            if (!status.IsValid || IsDead)
            {
                return;
            }

            string effectKey = string.IsNullOrEmpty(status.EffectName) ? "Unnamed" : status.EffectName;

            ActiveStatusEffect effect = new ActiveStatusEffect
            {
                Data = status,
                RemainingDuration = status.Duration,
                TimeUntilNextTick = status.SafeTickInterval
            };

            _activeStatusEffects[effectKey] = effect;
            RecalculateSpeedMultiplier();
        }

        /// <summary>
        /// Restores health up to the maximum health.
        /// </summary>
        /// <param name="healAmount">Amount of health to restore.</param>
        public virtual void Heal(double healAmount)
        {
            if (healAmount <= 0.0 || IsDead)
            {
                return;
            }

            _currentHealth = Math.Min(_maxHealth, _currentHealth + healAmount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Sets the maximum health and optionally resets current health to match.
        /// </summary>
        /// <param name="newMaxHealth">New maximum health value.</param>
        /// <param name="resetCurrentHealth">If true, sets current health equal to new max health.</param>
        public virtual void SetMaxHealth(double newMaxHealth, bool resetCurrentHealth = true)
        {
            _maxHealth = Math.Max(0.0, newMaxHealth);
            if (resetCurrentHealth)
            {
                _currentHealth = _maxHealth;
            }
            else
            {
                _currentHealth = Math.Min(_currentHealth, _maxHealth);
            }
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Assigns a new ordered sequence of coordinates for the enemy to traverse.
        /// </summary>
        /// <param name="coordinates">The coordinates in world space.</param>
        /// <param name="teleportToStart">If true, positions the enemy at the first coordinate immediately.</param>
        public void SetPath(IEnumerable<Vector3> coordinates, bool teleportToStart = false)
        {
            _pathCoordinates.Clear();
            if (coordinates != null)
            {
                _pathCoordinates.AddRange(coordinates);
            }

            _currentWaypointIndex = 0;

            if (teleportToStart && _pathCoordinates.Count > 0)
            {
                transform.position = _pathCoordinates[0];
            }
        }

        /// <summary>
        /// Assigns a new ordered sequence of coordinates and specifies the starting waypoint index.
        /// </summary>
        /// <param name="coordinates">The coordinates in world space.</param>
        /// <param name="startingWaypointIndex">Index of the waypoint to pursue first.</param>
        /// <param name="teleportToStart">If true, positions the enemy at the starting waypoint immediately.</param>
        public void SetPath(IEnumerable<Vector3> coordinates, int startingWaypointIndex, bool teleportToStart = false)
        {
            SetPath(coordinates, teleportToStart: false);
            _currentWaypointIndex = Mathf.Clamp(startingWaypointIndex, 0, _pathCoordinates.Count);

            if (teleportToStart && _pathCoordinates.Count > 0)
            {
                int index = Mathf.Clamp(startingWaypointIndex, 0, _pathCoordinates.Count - 1);
                transform.position = _pathCoordinates[index];
            }
        }

        /// <summary>
        /// Overload for setting path coordinates using 2D coordinates (Vector2).
        /// </summary>
        public void SetPath(IEnumerable<Vector2> coordinates, bool teleportToStart = false)
        {
            _pathCoordinates.Clear();
            if (coordinates != null)
            {
                foreach (Vector2 coord in coordinates)
                {
                    _pathCoordinates.Add(new Vector3(coord.x, coord.y, transform.position.z));
                }
            }

            _currentWaypointIndex = 0;

            if (teleportToStart && _pathCoordinates.Count > 0)
            {
                transform.position = _pathCoordinates[0];
            }
        }

        /// <summary>
        /// Assigns the path using an existing <see cref="EnemyPath"/> component.
        /// </summary>
        public void SetPath(EnemyPath enemyPath, bool teleportToStart = false)
        {
            if (enemyPath != null && enemyPath.Waypoints != null)
            {
                SetPath(enemyPath.Waypoints, teleportToStart);
            }
        }

        /// <summary>
        /// Pushes the enemy backward along its route coordinates by the specified distance.
        /// </summary>
        /// <param name="distance">Distance in world units to push backward.</param>
        public virtual void Knockback(float distance)
        {
            if (distance <= 0f || IsDead || _pathCoordinates == null || _pathCoordinates.Count == 0)
            {
                return;
            }

            Vector3 currentPos = transform.position;
            float remaining = distance;

            while (remaining > 0.0001f && _currentWaypointIndex > 0)
            {
                Vector3 previousWaypoint = _pathCoordinates[_currentWaypointIndex - 1];
                previousWaypoint.z = currentPos.z;

                float distToPrev = Vector3.Distance(currentPos, previousWaypoint);
                if (remaining <= distToPrev)
                {
                    Vector3 dir = (previousWaypoint - currentPos).normalized;
                    transform.position = currentPos + dir * remaining;
                    return;
                }

                remaining -= distToPrev;
                currentPos = previousWaypoint;
                transform.position = currentPos;
                _currentWaypointIndex--;
            }
        }

        /// <summary>
        /// Invoked when the enemy reaches the final waypoint in the path.
        /// </summary>
        protected virtual void OnReachedDestination()
        {
            OnDestinationReached?.Invoke(this);
            OnAnyDestinationReached?.Invoke(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// Triggers the destination reached sequence manually (useful for testing or external events).
        /// </summary>
        public void ReachDestination()
        {
            OnReachedDestination();
        }

        /// <summary>
        /// Invoked when health reaches zero.
        /// </summary>
        protected virtual void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            _activeStatusEffects.Clear();
            _speedMultiplier = 1f;
            OnDied?.Invoke(this);
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            if (_pathCoordinates == null || _pathCoordinates.Count == 0)
            {
                return;
            }

            Gizmos.color = Color.red;
            for (int i = 0; i < _pathCoordinates.Count; i++)
            {
                Gizmos.DrawWireSphere(_pathCoordinates[i], 0.15f);
                if (i < _pathCoordinates.Count - 1)
                {
                    Gizmos.DrawLine(_pathCoordinates[i], _pathCoordinates[i + 1]);
                }
            }

            if (_currentWaypointIndex < _pathCoordinates.Count)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _pathCoordinates[_currentWaypointIndex]);
            }
        }
    }
}

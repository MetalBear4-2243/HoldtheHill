using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Baseline component providing core stats and movement functionality for basic enemies.
    /// Manages health (double), damage (int), and waypoint traversal along a coordinate path.
    /// </summary>
    [DisallowMultipleComponent]
    public class BasicEnemy : MonoBehaviour
    {
        [Header("Health & Combat")]
        [Tooltip("Maximum health of this enemy.")]
        [SerializeField] private double _maxHealth = 100.0;

        [Tooltip("Damage dealt by this enemy (e.g. to the hill or player base upon reaching destination).")]
        [SerializeField] private int _damage = 1;

        [Header("Path & Movement")]
        [Tooltip("Movement speed along the path in world units per second.")]
        [SerializeField] private float _moveSpeed = 2f;

        [Tooltip("Distance threshold to consider a waypoint coordinate reached.")]
        [SerializeField] private float _waypointTolerance = 0.05f;

        [Tooltip("Initial path coordinates in world space. Can also be assigned dynamically via SetPath().")]
        [SerializeField] private List<Vector3> _pathCoordinates = new List<Vector3>();

        [Header("Visuals")]
        [Tooltip("If true and a SpriteRenderer is present, flips sprite on the X axis to match movement direction.")]
        [SerializeField] private bool _flipSpriteFacing = true;

        private double _currentHealth;
        private int _currentWaypointIndex;
        private SpriteRenderer _spriteRenderer;

        // Events for decoupled listeners (UI, spawner tracking, audio, combat)
        public event Action<BasicEnemy> OnDied;
        public event Action<BasicEnemy> OnDestinationReached;
        public event Action<double, double> OnHealthChanged;

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

        /// <summary>List of coordinates that this enemy follows.</summary>
        public IReadOnlyList<Vector3> PathCoordinates => _pathCoordinates;

        /// <summary>Index of the next waypoint coordinate being pursued.</summary>
        public int CurrentWaypointIndex => _currentWaypointIndex;

        /// <summary>True if health has been depleted and death sequence has started.</summary>
        public bool IsDead { get; private set; }

        protected virtual void Awake()
        {
            _currentHealth = _maxHealth;
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

            TraversePath();
        }

        /// <summary>
        /// Moves the enemy towards the current target waypoint.
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
                _moveSpeed * Time.deltaTime
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
        /// Applies damage to the enemy.
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
        /// Invoked when the enemy reaches the final waypoint in the path.
        /// </summary>
        protected virtual void OnReachedDestination()
        {
            OnDestinationReached?.Invoke(this);
            Destroy(gameObject);
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

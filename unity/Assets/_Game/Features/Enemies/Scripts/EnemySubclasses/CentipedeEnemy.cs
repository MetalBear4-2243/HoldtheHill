using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Specialized subclass of <see cref="BasicEnemy"/> representing a Centipede enemy.
    /// When defeated, it splits into 2 smaller enemies with half of the parent's maximum health.
    /// The newly spawned enemies continue to split upon reaching 0 HP until they only have 1 HP.
    /// Can also be constrained by a "splits remaining" counter (-1 for infinite splits until 1 HP).
    /// </summary>
    public class CentipedeEnemy : BasicEnemy
    {
        [Header("Centipede Splitting")]
        [Tooltip("Number of remaining splits allowed. Decrements with each split. Set to -1 for unlimited splits until 1 HP.")]
        [SerializeField] private int _splitsRemaining = -1;

        [Tooltip("Optional prefab to instantiate for child segments; it must carry a CentipedeEnemy. If null, this GameObject is used as the template.")]
        [SerializeField] private GameObject _splitPrefab = null;

        [Tooltip("Visual scale multiplier applied to child enemies when splitting.")]
        [SerializeField] private float _sizeMultiplier = 0.8f;

        [Tooltip("Distance forward along the path where the second child enemy is spawned.")]
        [SerializeField] private float _splitForwardDistance = 1.0f;

        // Events for split detection (useful for audio, VFX, and wave monitoring)
        public event Action<CentipedeEnemy, List<CentipedeEnemy>> OnSplit;
        public static event Action<CentipedeEnemy, List<CentipedeEnemy>> OnAnyCentipedeSplit;

        /// <summary>
        /// Gets or sets the number of splits remaining.
        /// -1 enables splitting until the 1 HP condition is reached.
        /// </summary>
        public int SplitsRemaining
        {
            get => _splitsRemaining;
            set => _splitsRemaining = value;
        }

        /// <summary>
        /// Gets or sets the visual scale factor applied to child segments upon splitting.
        /// </summary>
        public float SizeMultiplier
        {
            get => _sizeMultiplier;
            set => _sizeMultiplier = Mathf.Max(0.1f, value);
        }

        /// <summary>
        /// Gets or sets the distance forward along the path where the second child enemy is spawned.
        /// </summary>
        public float SplitForwardDistance
        {
            get => _splitForwardDistance;
            set => _splitForwardDistance = Mathf.Max(0f, value);
        }

        /// <summary>
        /// Backward-compatible alias for <see cref="SplitForwardDistance"/>.
        /// </summary>
        public float SplitOffsetDistance
        {
            get => _splitForwardDistance;
            set => _splitForwardDistance = value;
        }

        /// <summary>
        /// Optional prefab template for child enemies. If null, copies of this instance are instantiated.
        /// </summary>
        public GameObject SplitPrefab
        {
            get => _splitPrefab;
            set => _splitPrefab = value;
        }

        /// <summary>
        /// Determines whether this centipede can split upon death based on remaining splits and health constraints.
        /// </summary>
        /// <returns>True if the enemy is eligible to split; otherwise false.</returns>
        public bool CanSplit()
        {
            // Splits remaining constraint: must be -1 (unlimited) or > 0
            bool hasSplitsRemaining = _splitsRemaining == -1 || _splitsRemaining > 0;
            if (!hasSplitsRemaining)
            {
                return false;
            }

            // Health constraint: must have more than 1 HP, and halved health must not drop below 1 HP
            double halvedHealth = MaxHealth / 2.0;
            if (MaxHealth <= 1.0 || halvedHealth < 1.0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Overrides Die() to spawn 2 smaller child enemies when eligible before completing the death sequence.
        /// </summary>
        protected override void Die()
        {
            if (!IsActive)
            {
                return;
            }

            if (CanSplit())
            {
                Split();
            }

            base.Die();
        }

        /// <summary>
        /// Calculates the world position and next target waypoint index after advancing a given distance forward along the path.
        /// </summary>
        /// <param name="forwardDistance">Distance in world units to advance forward along the path.</param>
        /// <param name="targetWaypointIndex">The next waypoint index that the advanced position should target.</param>
        /// <returns>The calculated position in world space.</returns>
        public Vector3 CalculatePositionAlongPath(float forwardDistance, out int targetWaypointIndex)
        {
            targetWaypointIndex = _currentWaypointIndex;

            if (forwardDistance <= 0f)
            {
                return transform.position;
            }

            if (_pathCoordinates == null || _pathCoordinates.Count == 0)
            {
                Vector3 forward = transform.right.sqrMagnitude > 0.001f ? transform.right.normalized : Vector3.right;
                Vector3 fallbackPos = transform.position + forward * forwardDistance;
                fallbackPos.z = transform.position.z;
                return fallbackPos;
            }

            if (_currentWaypointIndex >= _pathCoordinates.Count)
            {
                targetWaypointIndex = _pathCoordinates.Count;
                return transform.position;
            }

            float remainingDist = forwardDistance;
            Vector3 currentPoint = transform.position;
            int waypointIdx = _currentWaypointIndex;

            while (remainingDist > 0.0001f && waypointIdx < _pathCoordinates.Count)
            {
                Vector3 nextWaypoint = _pathCoordinates[waypointIdx];
                nextWaypoint.z = currentPoint.z;

                Vector3 segment = nextWaypoint - currentPoint;
                float segmentDist = segment.magnitude;

                if (segmentDist <= 0.0001f)
                {
                    waypointIdx++;
                    continue;
                }

                if (remainingDist <= segmentDist)
                {
                    Vector3 result = currentPoint + (segment.normalized * remainingDist);
                    result.z = transform.position.z;
                    targetWaypointIndex = waypointIdx;
                    return result;
                }

                remainingDist -= segmentDist;
                currentPoint = nextWaypoint;
                waypointIdx++;
            }

            // Path ended before covering full distance: clamp to final waypoint
            targetWaypointIndex = Mathf.Max(0, _pathCoordinates.Count - 1);
            currentPoint.z = transform.position.z;
            return currentPoint;
        }

        /// <summary>
        /// Distance left to walk along the path, from where the centipede stands to the last waypoint.
        /// Zero with no path or once the last waypoint is passed.
        /// </summary>
        public float RemainingPathDistance()
        {
            if (_pathCoordinates == null || _currentWaypointIndex >= _pathCoordinates.Count)
            {
                return 0f;
            }

            float total = 0f;
            Vector3 from = transform.position;
            for (int i = _currentWaypointIndex; i < _pathCoordinates.Count; i++)
            {
                Vector3 to = _pathCoordinates[i];
                to.z = from.z;
                total += Vector3.Distance(from, to);
                from = to;
            }

            return total;
        }

        /// <summary>
        /// Each part's damage to the Queen: half its parent's, rounded up, never below 1. The parts of
        /// one centipede together hit the Queen about as hard as the whole centipede would have.
        /// </summary>
        public static int ChildDamage(int parentDamage)
        {
            return Mathf.Max(1, (parentDamage + 1) / 2);
        }

        /// <summary>
        /// Spawns 2 smaller centipede enemies with half maximum health, half damage to the Queen and
        /// decremented splits remaining. The first child is spawned at the original enemy's position,
        /// and the second child 1 unit forward along the path, unless that would put it at the end of
        /// the path, where it would reach the Queen at once: then it starts beside the first.
        /// </summary>
        /// <returns>List of the newly instantiated child centipede enemies.</returns>
        public List<CentipedeEnemy> Split()
        {
            double newMaxHealth = MaxHealth / 2.0;
            int childSplitsRemaining = (_splitsRemaining == -1) ? -1 : (_splitsRemaining - 1);
            int childDamage = ChildDamage(Damage);

            List<CentipedeEnemy> spawnedChildren = new List<CentipedeEnemy>(2);
            GameObject template = SplitTemplate();

            // First child is spawned at the original enemy's position
            Vector3 child0Pos = transform.position;
            int child0WaypointIndex = ChildWaypointIndex(_currentWaypointIndex);

            // Second child is spawned forward along the path, if the path goes on that far
            Vector3 child1Pos = child0Pos;
            int child1WaypointIndex = child0WaypointIndex;
            if (RemainingPathDistance() > _splitForwardDistance + _waypointTolerance)
            {
                child1Pos = CalculatePositionAlongPath(_splitForwardDistance, out child1WaypointIndex);
                child1WaypointIndex = ChildWaypointIndex(child1WaypointIndex);
            }

            Vector3[] spawnPositions = new Vector3[] { child0Pos, child1Pos };
            int[] waypointIndices = new int[] { child0WaypointIndex, child1WaypointIndex };

            for (int i = 0; i < 2; i++)
            {
                Vector3 spawnPos = spawnPositions[i];
                spawnPos.z = transform.position.z;

                GameObject childObj = Instantiate(template, spawnPos, transform.rotation, transform.parent);
                CentipedeEnemy childCentipede = childObj.GetComponent<CentipedeEnemy>();

                // Apply half max health, half damage and child splits remaining
                childCentipede.SetMaxHealth(newMaxHealth, resetCurrentHealth: true);
                childCentipede.SplitsRemaining = childSplitsRemaining;
                childCentipede.Damage = childDamage;
                childCentipede.MoveSpeed = MoveSpeed;

                // Scale down visuals to represent a smaller enemy
                childCentipede.transform.localScale = transform.localScale * _sizeMultiplier;

                // Inherit path and appropriate waypoint index so the children smoothly continue towards the goal
                if (_pathCoordinates != null && _pathCoordinates.Count > 0)
                {
                    childCentipede.SetPath(_pathCoordinates, waypointIndices[i], teleportToStart: false);
                }

                spawnedChildren.Add(childCentipede);
            }

            OnSplit?.Invoke(this, spawnedChildren);
            OnAnyCentipedeSplit?.Invoke(this, spawnedChildren);

            return spawnedChildren;
        }

        // The split prefab must be a centipede: a plain BasicEnemy can't take a CentipedeEnemy as
        // well (one enemy component per object), and the parent would walk on at 0 HP. Such a
        // prefab is refused with an error, and the parent itself is copied instead.
        private GameObject SplitTemplate()
        {
            if (_splitPrefab == null)
            {
                return gameObject;
            }

            if (_splitPrefab.GetComponent<CentipedeEnemy>() == null)
            {
                Debug.LogError($"{name}: the split prefab '{_splitPrefab.name}' has no CentipedeEnemy, so the centipede splits into copies of itself instead.", this);
                return gameObject;
            }

            return _splitPrefab;
        }

        // A child must have a waypoint left to walk to: an index past the last waypoint would leave
        // it standing still at the end of the path forever, and the run could never end.
        private int ChildWaypointIndex(int index)
        {
            if (_pathCoordinates == null || _pathCoordinates.Count == 0)
            {
                return 0;
            }

            return Mathf.Clamp(index, 0, _pathCoordinates.Count - 1);
        }
    }
}

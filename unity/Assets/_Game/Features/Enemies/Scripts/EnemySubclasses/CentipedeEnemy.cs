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

        [Tooltip("Optional prefab to instantiate for child segments. If null, this GameObject is used as the template.")]
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
            if (IsDead)
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
        /// Spawns 2 smaller centipede enemies with half maximum health and decremented splits remaining.
        /// The first child is spawned at the original enemy's position, and the second child is spawned
        /// 1 unit forward along the path.
        /// </summary>
        /// <returns>List of the newly instantiated child centipede enemies.</returns>
        public List<CentipedeEnemy> Split()
        {
            double newMaxHealth = MaxHealth / 2.0;
            int childSplitsRemaining = (_splitsRemaining == -1) ? -1 : (_splitsRemaining - 1);

            List<CentipedeEnemy> spawnedChildren = new List<CentipedeEnemy>(2);
            GameObject template = _splitPrefab != null ? _splitPrefab : gameObject;

            // First child is spawned at the original enemy's position
            Vector3 child0Pos = transform.position;
            int child0WaypointIndex = _currentWaypointIndex;

            // Second child is spawned forward along the path
            Vector3 child1Pos = CalculatePositionAlongPath(_splitForwardDistance, out int child1WaypointIndex);

            Vector3[] spawnPositions = new Vector3[] { child0Pos, child1Pos };
            int[] waypointIndices = new int[] { child0WaypointIndex, child1WaypointIndex };

            for (int i = 0; i < 2; i++)
            {
                Vector3 spawnPos = spawnPositions[i];
                spawnPos.z = transform.position.z;

                GameObject childObj = Instantiate(template, spawnPos, transform.rotation, transform.parent);
                CentipedeEnemy childCentipede = childObj.GetComponent<CentipedeEnemy>();
                if (childCentipede == null)
                {
                    childCentipede = childObj.AddComponent<CentipedeEnemy>();
                }

                // Apply half max health and child splits remaining
                childCentipede.SetMaxHealth(newMaxHealth, resetCurrentHealth: true);
                childCentipede.SplitsRemaining = childSplitsRemaining;
                childCentipede.Damage = Damage;
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
    }
}

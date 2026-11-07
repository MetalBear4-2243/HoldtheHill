using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Centipede's split, an ability beside <see cref="BasicEnemy"/> (Jason's CentipedeEnemy
    /// subclass folded into a component). When the enemy dies it splits into 2 smaller enemies with
    /// half of the parent's maximum health and damage. They split again on death until they only
    /// have 1 HP, or until the "splits remaining" counter runs out (-1 for unlimited splits until 1 HP).
    /// How many splits, how small the halves are and how far forward the second one appears are the
    /// enemy's <see cref="EnemyAbilitySettings"/> (its data asset), read once when the enemy is made;
    /// the counter then counts down on this instance (a half starts at its parent's count less one).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BasicEnemy))]
    public class CentipedeSplitter : MonoBehaviour, IDeathHandler
    {
        [Header("Centipede Splitting")]
        [Tooltip("Optional prefab to instantiate for child segments; it must carry a CentipedeSplitter. If null, this GameObject is used as the template.")]
        [SerializeField] private GameObject _splitPrefab = null;

        // Per-instance state, taken from the data asset once (ApplySettings); -1 is unlimited.
        private int _splitsRemaining = -1;
        private float _sizeMultiplier = 0.8f;
        private float _splitForwardDistance = 1.0f;
        private bool _settingsApplied;

        private BasicEnemy _enemy;

        /// <summary>This centipede split: the parent and its two children.</summary>
        public event Action<BasicEnemy, List<BasicEnemy>> OnSplit;

        /// <summary>Any centipede split: the parent and its two children (the spawner counts them).</summary>
        public static event Action<BasicEnemy, List<BasicEnemy>> OnAnyCentipedeSplit;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticEvents()
        {
            OnAnyCentipedeSplit = null;
        }

        /// <summary>
        /// Gets or sets the number of splits remaining.
        /// -1 enables splitting until the 1 HP condition is reached.
        /// </summary>
        public int SplitsRemaining
        {
            get
            {
                ApplySettings();
                return _splitsRemaining;
            }
            set
            {
                ApplySettings();
                _splitsRemaining = value;
            }
        }

        /// <summary>
        /// Gets or sets the visual scale factor applied to child segments upon splitting.
        /// </summary>
        public float SizeMultiplier
        {
            get
            {
                ApplySettings();
                return _sizeMultiplier;
            }
            set
            {
                ApplySettings();
                _sizeMultiplier = Mathf.Max(0.1f, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance forward along the path where the second child enemy is spawned.
        /// </summary>
        public float SplitForwardDistance
        {
            get
            {
                ApplySettings();
                return _splitForwardDistance;
            }
            set
            {
                ApplySettings();
                _splitForwardDistance = Mathf.Max(0f, value);
            }
        }

        // Takes the data asset's numbers once per instance, on demand as well as in Awake, so a
        // prefab asset and an EditMode test (no Awake) read the same values. An enemy without a
        // data asset keeps the defaults; a value set through a property afterwards wins.
        private void ApplySettings()
        {
            if (_settingsApplied)
            {
                return;
            }

            _settingsApplied = true;
            BasicEnemy enemy = Enemy;
            if (enemy == null || enemy.Data == null)
            {
                return;
            }

            EnemyAbilitySettings settings = enemy.Data.Abilities;
            _splitsRemaining = Mathf.Max(-1, settings.Splits);
            _sizeMultiplier = Mathf.Max(0.1f, settings.SplitSizeMultiplier);
            _splitForwardDistance = Mathf.Max(0f, settings.SplitForwardDistance);
        }

        /// <summary>
        /// Optional prefab template for child enemies. If null, copies of this instance are instantiated.
        /// </summary>
        public GameObject SplitPrefab
        {
            get => _splitPrefab;
            set => _splitPrefab = value;
        }

        // Found on demand as well as in Awake, so EditMode tests, which never run Awake, work too.
        private BasicEnemy Enemy
        {
            get
            {
                if (_enemy == null)
                {
                    _enemy = GetComponent<BasicEnemy>();
                }
                return _enemy;
            }
        }

        private void Awake()
        {
            _enemy = GetComponent<BasicEnemy>();
            ApplySettings();
        }

        /// <summary>
        /// Determines whether this centipede can split upon death based on remaining splits and health constraints.
        /// </summary>
        /// <returns>True if the enemy is eligible to split; otherwise false.</returns>
        public bool CanSplit()
        {
            ApplySettings();

            // Splits remaining constraint: must be -1 (unlimited) or > 0
            bool hasSplitsRemaining = _splitsRemaining == -1 || _splitsRemaining > 0;
            if (!hasSplitsRemaining)
            {
                return false;
            }

            // Health constraint: must have more than 1 HP, and halved health must not drop below 1 HP
            double maxHealth = Enemy.MaxHealth;
            double halvedHealth = maxHealth / 2.0;
            if (maxHealth <= 1.0 || halvedHealth < 1.0)
            {
                return false;
            }

            return true;
        }

        /// <summary>Splits, if eligible, the moment the enemy dies and before its death is announced.</summary>
        public void BeforeDeath(BasicEnemy enemy)
        {
            if (CanSplit())
            {
                Split();
            }
        }

        /// <summary>
        /// Calculates the world position and next target waypoint index after advancing a given distance forward along the path.
        /// </summary>
        /// <param name="forwardDistance">Distance in world units to advance forward along the path.</param>
        /// <param name="targetWaypointIndex">The next waypoint index that the advanced position should target.</param>
        /// <returns>The calculated position in world space.</returns>
        public Vector3 CalculatePositionAlongPath(float forwardDistance, out int targetWaypointIndex)
        {
            IReadOnlyList<Vector3> path = Enemy.PathCoordinates;
            int currentWaypointIndex = Enemy.CurrentWaypointIndex;
            targetWaypointIndex = currentWaypointIndex;

            if (forwardDistance <= 0f)
            {
                return transform.position;
            }

            if (path == null || path.Count == 0)
            {
                Vector3 forward = transform.right.sqrMagnitude > 0.001f ? transform.right.normalized : Vector3.right;
                Vector3 fallbackPos = transform.position + forward * forwardDistance;
                fallbackPos.z = transform.position.z;
                return fallbackPos;
            }

            if (currentWaypointIndex >= path.Count)
            {
                targetWaypointIndex = path.Count;
                return transform.position;
            }

            float remainingDist = forwardDistance;
            Vector3 currentPoint = transform.position;
            int waypointIdx = currentWaypointIndex;

            while (remainingDist > 0.0001f && waypointIdx < path.Count)
            {
                Vector3 nextWaypoint = path[waypointIdx];
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
            targetWaypointIndex = Mathf.Max(0, path.Count - 1);
            currentPoint.z = transform.position.z;
            return currentPoint;
        }

        /// <summary>
        /// Distance left to walk along the path, from where the centipede stands to the last waypoint.
        /// Zero with no path or once the last waypoint is passed.
        /// </summary>
        public float RemainingPathDistance()
        {
            IReadOnlyList<Vector3> path = Enemy.PathCoordinates;
            int currentWaypointIndex = Enemy.CurrentWaypointIndex;
            if (path == null || currentWaypointIndex >= path.Count)
            {
                return 0f;
            }

            float total = 0f;
            Vector3 from = transform.position;
            for (int i = currentWaypointIndex; i < path.Count; i++)
            {
                Vector3 to = path[i];
                to.z = from.z;
                total += Vector3.Distance(from, to);
                from = to;
            }

            return total;
        }

        /// <summary>
        /// Spawns 2 smaller centipede enemies with half maximum health, half damage to the Queen and
        /// decremented splits remaining. The first child is spawned at the original enemy's position,
        /// and the second child forward along the path, unless that would put it at the end of
        /// the path, where it would reach the Queen at once: then it starts beside the first.
        /// </summary>
        /// <returns>List of the newly instantiated child enemies.</returns>
        public List<BasicEnemy> Split()
        {
            ApplySettings();
            BasicEnemy parent = Enemy;
            double newMaxHealth = parent.MaxHealth / 2.0;
            int childSplitsRemaining = (_splitsRemaining == -1) ? -1 : (_splitsRemaining - 1);
            int childDamage = parent.Damage <= 0 ? 0 : Mathf.Max(1, parent.Damage / 2);

            List<BasicEnemy> spawnedChildren = new List<BasicEnemy>(2);
            GameObject template = SplitTemplate();
            IReadOnlyList<Vector3> path = parent.PathCoordinates;

            // First child is spawned at the original enemy's position
            Vector3 child0Pos = transform.position;
            int child0WaypointIndex = ChildWaypointIndex(parent.CurrentWaypointIndex);

            // Second child is spawned forward along the path, if the path goes on that far
            Vector3 child1Pos = child0Pos;
            int child1WaypointIndex = child0WaypointIndex;
            if (RemainingPathDistance() > _splitForwardDistance + parent.WaypointTolerance)
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
                BasicEnemy child = childObj.GetComponent<BasicEnemy>();
                CentipedeSplitter childSplitter = childObj.GetComponent<CentipedeSplitter>();

                // Apply half max health, half damage, and child splits remaining
                child.SetMaxHealth(newMaxHealth, resetCurrentHealth: true);
                childSplitter.SplitsRemaining = childSplitsRemaining;
                child.Damage = childDamage;
                child.MoveSpeed = parent.MoveSpeed;

                // Scale down visuals to represent a smaller enemy
                child.transform.localScale = transform.localScale * _sizeMultiplier;

                // Inherit path and appropriate waypoint index so the children smoothly continue towards the goal
                if (path != null && path.Count > 0)
                {
                    child.SetPath(path, waypointIndices[i], teleportToStart: false);
                }

                spawnedChildren.Add(child);
            }

            OnSplit?.Invoke(parent, spawnedChildren);
            OnAnyCentipedeSplit?.Invoke(parent, spawnedChildren);

            return spawnedChildren;
        }

        // The split prefab must carry a splitter: without one a half would walk on at 0 HP, and the
        // halves could never halve again. Such a prefab is refused with an error, and the parent
        // itself is copied instead.
        private GameObject SplitTemplate()
        {
            if (_splitPrefab == null)
            {
                return gameObject;
            }

            if (_splitPrefab.GetComponent<CentipedeSplitter>() == null)
            {
                Debug.LogError($"{name}: the split prefab '{_splitPrefab.name}' has no CentipedeSplitter, so the centipede splits into copies of itself instead.", this);
                return gameObject;
            }

            return _splitPrefab;
        }

        // A child must have a waypoint left to walk to: an index past the last waypoint would leave
        // it standing still at the end of the path forever, and the run could never end.
        private int ChildWaypointIndex(int index)
        {
            IReadOnlyList<Vector3> path = Enemy.PathCoordinates;
            if (path == null || path.Count == 0)
            {
                return 0;
            }

            return Mathf.Clamp(index, 0, path.Count - 1);
        }
    }
}

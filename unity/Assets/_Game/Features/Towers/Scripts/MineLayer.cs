using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// The Carpenter Ant: keeps a few <see cref="Mine"/>s lying on the stretch of path inside the
    /// tower's range, laying a new one on the tower's cooldown whenever one has gone off. Each mine
    /// carries the tower's damage at the time it was laid.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tower))]
    public class MineLayer : MonoBehaviour
    {
        [Tooltip("The mine laid on the path.")]
        [SerializeField] private Mine _minePrefab;

        [Tooltip("How many of this tower's mines can lie on the path at once.")]
        [SerializeField, Min(1)] private int _maxMines = 3;

        [Tooltip("Gap in world units between the candidate spots sampled along the path.")]
        [SerializeField, Min(0.1f)] private float _spotSpacing = 0.5f;

        [Tooltip("The path mines go on. Found automatically if left empty.")]
        [SerializeField] private EnemyPath _path;

        private readonly List<Vector3> _spots = new List<Vector3>();
        private readonly List<Mine> _mines = new List<Mine>();
        private Tower _tower;
        private float _cooldown;

        /// <summary>Points on the path inside range where a mine may be laid. Empty if none.</summary>
        public IReadOnlyList<Vector3> Spots => _spots;

        private void Awake()
        {
            _tower = GetComponent<Tower>();
        }

        private void Start()
        {
            if (_path == null)
            {
                _path = FindAnyObjectByType<EnemyPath>();
            }

            CollectSpots();
            _tower.Upgraded += HandleUpgraded;
        }

        private void OnDestroy()
        {
            _tower.Upgraded -= HandleUpgraded;
            for (int i = 0; i < _mines.Count; i++)
            {
                if (_mines[i] != null)
                {
                    _mines[i].Exploded -= HandleExploded;
                }
            }
        }

        private void Update()
        {
            _cooldown -= Time.deltaTime;
            if (_cooldown > 0f || _minePrefab == null || _spots.Count == 0 || _mines.Count >= _maxMines)
            {
                return;
            }

            _cooldown = _tower.FireInterval;
            Lay(_spots[Random.Range(0, _spots.Count)]);
        }

        private void Lay(Vector3 spot)
        {
            Mine mine = Instantiate(_minePrefab, spot, Quaternion.identity, transform);
            mine.Arm(_tower.Damage);
            mine.Exploded += HandleExploded;
            _mines.Add(mine);
        }

        private void HandleExploded(Mine mine)
        {
            mine.Exploded -= HandleExploded;
            _mines.Remove(mine);
        }

        // A longer reach after an upgrade covers more of the path.
        private void HandleUpgraded(int level)
        {
            CollectSpots();
        }

        // Walks the path in small steps and keeps every step that lies inside the tower's range.
        private void CollectSpots()
        {
            _spots.Clear();
            if (_path == null || _path.Waypoints == null)
            {
                return;
            }

            IReadOnlyList<Vector3> points = _path.Waypoints;
            Vector3 here = transform.position;
            float rangeSqr = _tower.Range * _tower.Range;

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 a = points[i];
                Vector3 b = points[i + 1];
                float length = Vector2.Distance(a, b);
                int steps = Mathf.Max(1, Mathf.FloorToInt(length / _spotSpacing));

                for (int s = 0; s <= steps; s++)
                {
                    Vector3 spot = Vector3.Lerp(a, b, (float)s / steps);
                    spot.z = 0f;
                    if ((spot - here).sqrMagnitude <= rangeSqr)
                    {
                        _spots.Add(spot);
                    }
                }
            }
        }
    }
}

using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEngine;

namespace HoldTheHill.Features.Hazards
{
    /// <summary>
    /// A bird's shadow drifting across the map. It is placed on the spot it will strike, flies in
    /// from one side, and when it passes over that spot it takes whatever is under it: towers that
    /// are not burrowed are removed, enemies are killed (a centipede whole, without splitting).
    /// Then it drifts off the other side.
    /// The approach is the warning: time enough to burrow.
    /// </summary>
    [DisallowMultipleComponent]
    public class BirdShadow : MonoBehaviour
    {
        [Tooltip("How far from the strike spot the shadow starts and ends, in world units.")]
        [SerializeField, Min(1f)] private float _travel = 12f;

        [Tooltip("Drift speed in world units per second.")]
        [SerializeField, Min(0.1f)] private float _speed = 3f;

        [Tooltip("Everything this close to the strike spot is taken.")]
        [SerializeField, Min(0.1f)] private float _radius = 0.8f;

        [Tooltip("Direction of flight. Positive x flies left to right.")]
        [SerializeField] private Vector2 _direction = Vector2.right;

        private readonly List<BasicEnemy> _enemiesTaken = new List<BasicEnemy>();
        private readonly List<Tower> _towersTaken = new List<Tower>();

        private Vector3 _strikeSpot;
        private Vector3 _from;
        private Vector3 _to;
        private float _flown;
        private bool _struck;

        private void Start()
        {
            _strikeSpot = transform.position;
            Vector3 along = (Vector3)_direction.normalized * _travel;
            _from = _strikeSpot - along;
            _to = _strikeSpot + along;
            transform.position = _from;
        }

        private void Update()
        {
            _flown += _speed * Time.deltaTime;
            float total = _travel * 2f;
            transform.position = Vector3.Lerp(_from, _to, _flown / total);

            if (!_struck && _flown >= _travel)
            {
                _struck = true;
                Strike();
            }

            if (_flown >= total)
            {
                Destroy(gameObject);
            }
        }

        private void Strike()
        {
            float radiusSqr = _radius * _radius;

            // Collect first: removing changes the registries while they are read.
            _towersTaken.Clear();
            IReadOnlyList<Tower> towers = TowerRegistry.All;
            for (int i = 0; i < towers.Count; i++)
            {
                Tower tower = towers[i];
                if (tower != null && !tower.Burrowed && (tower.transform.position - _strikeSpot).sqrMagnitude <= radiusSqr)
                {
                    _towersTaken.Add(tower);
                }
            }

            _enemiesTaken.Clear();
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;
            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy != null && !enemy.IsDead && (enemy.transform.position - _strikeSpot).sqrMagnitude <= radiusSqr)
                {
                    _enemiesTaken.Add(enemy);
                }
            }

            for (int i = 0; i < _towersTaken.Count; i++)
            {
                _towersTaken[i].Remove();
            }

            // Taken by the bird counts as a kill, so the bounty is paid and the wave can still clear.
            for (int i = 0; i < _enemiesTaken.Count; i++)
            {
                // The bird takes a centipede whole: no halves split off under the shadow.
                if (_enemiesTaken[i] is CentipedeEnemy centipede)
                {
                    centipede.SplitsRemaining = 0;
                }

                _enemiesTaken[i].TakeDamage(double.MaxValue);
            }
        }
    }
}

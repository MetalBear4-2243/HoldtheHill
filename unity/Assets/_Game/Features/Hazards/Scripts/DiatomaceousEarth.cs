using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEngine;

namespace HoldTheHill.Features.Hazards
{
    /// <summary>
    /// A patch of diatomaceous earth on the map. While it lasts it hurts every enemy on the ground
    /// inside it on a tick, and stuns every tower inside it (the stun is renewed each tick, so a
    /// tower is free again right after the patch is gone or it is no longer inside).
    /// </summary>
    [DisallowMultipleComponent]
    public class DiatomaceousEarth : MonoBehaviour
    {
        [Tooltip("Radius of the patch in world units. The sprite is scaled to match.")]
        [SerializeField, Min(0.1f)] private float _radius = 1.2f;

        [Tooltip("Seconds the patch stays on the map.")]
        [SerializeField, Min(0.1f)] private float _duration = 12f;

        [Tooltip("Seconds between ticks of damage and stun.")]
        [SerializeField, Min(0.05f)] private float _tickInterval = 0.5f;

        [Tooltip("Damage dealt to each enemy inside on every tick.")]
        [SerializeField, Min(0f)] private float _damagePerTick = 4f;

        private readonly List<BasicEnemy> _hit = new List<BasicEnemy>();
        private float _age;
        private float _untilNextTick;

        /// <summary>Radius of the patch in world units.</summary>
        public float Radius => _radius;

        private void Start()
        {
            transform.localScale = Vector3.one * (_radius * 2f);
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age >= _duration)
            {
                Destroy(gameObject);
                return;
            }

            _untilNextTick -= Time.deltaTime;
            if (_untilNextTick > 0f)
            {
                return;
            }

            _untilNextTick = _tickInterval;
            Tick();
        }

        private void Tick()
        {
            Vector3 here = transform.position;
            float radiusSqr = _radius * _radius;

            // Collect first: a death removes the enemy from the registry while it is read.
            _hit.Clear();
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;
            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy == null || enemy.IsDead || (enemy.transform.position - here).sqrMagnitude > radiusSqr)
                {
                    continue;
                }

                // Dust on the ground does nothing to a flyer.
                if (enemy is IFlyingEnemy flyer && flyer.IsFlying)
                {
                    continue;
                }

                _hit.Add(enemy);
            }

            for (int i = 0; i < _hit.Count; i++)
            {
                _hit[i].TakeDamage(_damagePerTick);
            }

            // Twice the tick, so the stun never flickers off between two ticks.
            IReadOnlyList<Tower> towers = TowerRegistry.All;
            for (int i = 0; i < towers.Count; i++)
            {
                Tower tower = towers[i];
                if (tower != null && (tower.transform.position - here).sqrMagnitude <= radiusSqr)
                {
                    tower.Stun(_tickInterval * 2f);
                }
            }
        }
    }
}

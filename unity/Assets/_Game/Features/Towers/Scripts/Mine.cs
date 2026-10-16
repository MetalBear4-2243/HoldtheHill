using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// A mine laid by the Carpenter Ant. Waits on the path; the first living enemy to come within
    /// its trigger radius sets it off, and everything within the blast radius takes the damage.
    /// </summary>
    [DisallowMultipleComponent]
    public class Mine : MonoBehaviour
    {
        [Tooltip("An enemy this close sets the mine off.")]
        [SerializeField, Min(0.05f)] private float _triggerRadius = 0.35f;

        [Tooltip("Every enemy this close to the mine takes the damage when it goes off.")]
        [SerializeField, Min(0.05f)] private float _blastRadius = 0.9f;

        private static readonly List<BasicEnemy> s_blastBuffer = new List<BasicEnemy>();

        private double _damage;

        /// <summary>Raised when the mine goes off. The layer uses it to count its mines.</summary>
        public event System.Action<Mine> Exploded;

        /// <summary>Arms the mine with the damage it will deal. Called by the layer.</summary>
        public void Arm(double damage)
        {
            _damage = damage;
        }

        private void Update()
        {
            if (AnyEnemyWithin(_triggerRadius))
            {
                Explode();
            }
        }

        private bool AnyEnemyWithin(float radius)
        {
            Vector3 here = transform.position;
            float radiusSqr = radius * radius;
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;

            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy != null && !enemy.IsDead && (enemy.transform.position - here).sqrMagnitude <= radiusSqr)
                {
                    return true;
                }
            }

            return false;
        }

        private void Explode()
        {
            Vector3 here = transform.position;
            float radiusSqr = _blastRadius * _blastRadius;
            s_blastBuffer.Clear();

            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;
            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy != null && !enemy.IsDead && (enemy.transform.position - here).sqrMagnitude <= radiusSqr)
                {
                    s_blastBuffer.Add(enemy);
                }
            }

            // Damage after collecting: a death removes the enemy from the registry while it is read.
            for (int i = 0; i < s_blastBuffer.Count; i++)
            {
                s_blastBuffer[i].TakeDamage(_damage);
            }

            Exploded?.Invoke(this);
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _triggerRadius);
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawWireSphere(transform.position, _blastRadius);
        }
    }
}

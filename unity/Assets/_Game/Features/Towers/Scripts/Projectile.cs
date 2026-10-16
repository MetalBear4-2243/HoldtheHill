using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// One shot from a ranged tower. Flies at its target and deals the tower's damage on arrival.
    /// If the target dies first it carries on straight and expires.
    /// </summary>
    [DisallowMultipleComponent]
    public class Projectile : MonoBehaviour
    {
        [Tooltip("Travel speed in world units per second.")]
        [SerializeField, Min(0.01f)] private float _speed = 10f;

        [Tooltip("How close the shot must get to the target's centre to count as a hit.")]
        [SerializeField, Min(0.01f)] private float _hitRadius = 0.2f;

        [Tooltip("Seconds before a shot that missed gives up.")]
        [SerializeField, Min(0.1f)] private float _lifetime = 3f;

        private BasicEnemy _target;
        private double _damage;
        private Vector3 _direction;
        private float _age;

        /// <summary>Aims the shot. Called by the tower right after it is spawned.</summary>
        public void Launch(BasicEnemy target, double damage)
        {
            _target = target;
            _damage = damage;
            _age = 0f;
            _direction = Vector3.right;
            if (_target != null)
            {
                _direction = Direction(_target.transform.position);
            }
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age > _lifetime)
            {
                Destroy(gameObject);
                return;
            }

            float step = _speed * Time.deltaTime;

            // A destroyed target compares equal to null, so the shot just keeps its last heading.
            if (_target != null && !_target.IsDead)
            {
                Vector3 targetPosition = _target.transform.position;
                float distance = Vector2.Distance(transform.position, targetPosition);
                if (distance <= _hitRadius + step)
                {
                    _target.TakeDamage(_damage);
                    Destroy(gameObject);
                    return;
                }

                _direction = Direction(targetPosition);
            }

            transform.position += _direction * step;
        }

        private Vector3 Direction(Vector3 towards)
        {
            Vector3 delta = towards - transform.position;
            delta.z = 0f;
            return delta.sqrMagnitude > 0f ? delta.normalized : _direction;
        }
    }
}

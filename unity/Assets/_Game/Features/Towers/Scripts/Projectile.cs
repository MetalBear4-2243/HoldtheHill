using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// One shot from a ranged tower. A straight shot flies at its target and deals the tower's
    /// damage on arrival; if the target dies or reaches the Queen first it carries on and expires.
    /// A lobbed shot is drawn along an arc and follows its target while the target lives, so it
    /// lands on a moving enemy; if the target dies or leaves the map first, it lands where the
    /// target was last seen. Where it lands it damages every enemy still in the
    /// <see cref="EnemyRegistry"/> within its splash radius.
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

        [Header("Lobbed")]
        [Tooltip("If on, the shot arcs onto its target and splashes there instead of flying straight at it.")]
        [SerializeField] private bool _lobbed;

        [Tooltip("Lobbed only: how high the arc is drawn above the straight line, in world units.")]
        [SerializeField, Min(0f)] private float _arcHeight = 1.5f;

        [Tooltip("Lobbed only: every enemy within this distance of the landing point takes the damage.")]
        [SerializeField, Min(0f)] private float _splashRadius = 1f;

        [Header("Look")]
        [Tooltip("Optional: an effect (a splash, a blast) left where the shot hits or lands. It removes itself.")]
        [SerializeField] private GameObject _hitEffect;

        private static readonly List<BasicEnemy> s_splashBuffer = new List<BasicEnemy>();

        private BasicEnemy _target;
        private double _damage;
        private Vector3 _direction;
        private float _age;

        // Lobbed flight: the shot's true position moves in a straight line; the arc is only drawn.
        // The landing point follows the target, and the flight time is fixed at launch.
        private Vector3 _origin;
        private Vector3 _landing;
        private float _flightTime;

        /// <summary>True for a shot that lands and splashes rather than chasing its target.</summary>
        public bool IsLobbed => _lobbed;

        /// <summary>Aims the shot. Called by the tower right after it is spawned.</summary>
        public void Launch(BasicEnemy target, double damage)
        {
            _target = target;
            _damage = damage;
            _age = 0f;
            _origin = transform.position;
            _direction = Vector3.right;
            if (_target != null)
            {
                _landing = LobLanding(_target.transform.position, _origin.z);
                _direction = Direction(_landing);
                _flightTime = Vector2.Distance(_origin, _landing) / _speed;
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

            if (_lobbed)
            {
                Lob();
                return;
            }

            float step = _speed * Time.deltaTime;

            // A target that died or reached the Queen has left the registry: drop it, and the shot
            // keeps its last heading and expires. A hit on the frame it arrived would pay a bounty.
            if (_target != null && !EnemyRegistry.Contains(_target))
            {
                _target = null;
            }

            if (_target != null && !_target.IsDead)
            {
                Vector3 targetPosition = _target.transform.position;
                float distance = Vector2.Distance(transform.position, targetPosition);
                if (distance <= _hitRadius + step)
                {
                    _target.TakeDamage(_damage);
                    LeaveHitEffect(transform.position);
                    Destroy(gameObject);
                    return;
                }

                _direction = Direction(targetPosition);
            }

            transform.position += _direction * step;
        }

        private void Lob()
        {
            // Follow a living target, so the shot lands on it instead of on the spot it has left
            // (a Scarab is gone from there by the time the shot comes down).
            if (_target != null && !_target.IsDead && EnemyRegistry.Contains(_target))
            {
                _landing = LobLanding(_target.transform.position, _origin.z);
            }

            float t = _flightTime > 0f ? Mathf.Clamp01(_age / _flightTime) : 1f;
            Vector3 along = Vector3.Lerp(_origin, _landing, t);
            transform.position = along + Vector3.up * (_arcHeight * Mathf.Sin(t * Mathf.PI));

            if (t < 1f)
            {
                return;
            }

            Splash(_landing);
            LeaveHitEffect(_landing);
            Destroy(gameObject);
        }

        // Not a child of the shot, which goes now; kept in the shot's scene, so it goes with the level.
        private void LeaveHitEffect(Vector3 at)
        {
            if (_hitEffect == null)
            {
                return;
            }

            GameObject effect = Instantiate(_hitEffect, at, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(effect, gameObject.scene);
        }

        // Damages every living enemy on the ground near the point; a splash on the ground misses
        // anything flying (the wasp rule). The target itself counts only if it is still there.
        private void Splash(Vector3 centre)
        {
            float radiusSqr = _splashRadius * _splashRadius;
            s_splashBuffer.Clear();

            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;
            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy == null || enemy.IsDead || (enemy.transform.position - centre).sqrMagnitude > radiusSqr)
                {
                    continue;
                }

                if (enemy is IFlyingEnemy flyer && flyer.IsFlying)
                {
                    continue;
                }

                s_splashBuffer.Add(enemy);
            }

            // Damage after collecting: a death removes the enemy from the registry while it is read.
            for (int i = 0; i < s_splashBuffer.Count; i++)
            {
                s_splashBuffer[i].TakeDamage(_damage);
            }
        }

        /// <summary>Where a lob comes down for a target at <paramref name="target"/>: on it, in the shot's own plane.</summary>
        public static Vector3 LobLanding(Vector3 target, float z)
        {
            target.z = z;
            return target;
        }

        private Vector3 Direction(Vector3 towards)
        {
            Vector3 delta = towards - transform.position;
            delta.z = 0f;
            return delta.sqrMagnitude > 0f ? delta.normalized : _direction;
        }
    }
}

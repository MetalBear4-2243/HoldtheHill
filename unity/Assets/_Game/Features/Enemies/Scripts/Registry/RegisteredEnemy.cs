using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Puts this enemy in the <see cref="EnemyRegistry"/> while it is alive, so towers can target it,
    /// and tells the registry when it is killed. Goes on every enemy prefab next to <see cref="BasicEnemy"/>.
    /// An enemy that reaches the Queen leaves the registry at once: it is only destroyed at the end of
    /// the frame, and a hit before then would pay its bounty or split a centipede at the path's end.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BasicEnemy))]
    public class RegisteredEnemy : MonoBehaviour
    {
        private BasicEnemy _enemy;

        private void Awake()
        {
            _enemy = GetComponent<BasicEnemy>();
        }

        private void OnEnable()
        {
            EnemyRegistry.Add(_enemy);
            _enemy.OnDied += HandleDied;
            _enemy.OnDestinationReached += HandleDestinationReached;
        }

        private void OnDisable()
        {
            _enemy.OnDied -= HandleDied;
            _enemy.OnDestinationReached -= HandleDestinationReached;
            EnemyRegistry.Remove(_enemy);
        }

        private void HandleDied(BasicEnemy enemy)
        {
            EnemyRegistry.NotifyDied(enemy);
        }

        private void HandleDestinationReached(BasicEnemy enemy)
        {
            EnemyRegistry.Remove(enemy);
        }
    }
}

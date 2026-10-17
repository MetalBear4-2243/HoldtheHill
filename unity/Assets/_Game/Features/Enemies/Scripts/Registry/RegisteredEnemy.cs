using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Puts this enemy in the <see cref="EnemyRegistry"/> while it is alive, so towers can target it,
    /// and tells the registry when it is killed. Goes on every enemy prefab next to <see cref="BasicEnemy"/>.
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
        }

        private void OnDisable()
        {
            _enemy.OnDied -= HandleDied;
            EnemyRegistry.Remove(_enemy);
        }

        private void HandleDied(BasicEnemy enemy)
        {
            EnemyRegistry.NotifyDied(enemy);
        }
    }
}

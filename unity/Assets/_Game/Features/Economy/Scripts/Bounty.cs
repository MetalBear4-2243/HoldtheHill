using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Economy
{
    /// <summary>
    /// Honeydew paid to the player when this enemy dies. Sits on the enemy prefab next to
    /// <see cref="BasicEnemy"/> and listens to its death; the enemy class itself is untouched.
    /// The amount comes from the enemy's <see cref="EnemyData"/> when it has one. Bosses carry a
    /// bigger bounty.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BasicEnemy))]
    public class Bounty : MonoBehaviour
    {
        [Tooltip("Fallback, used only without Enemy Data on the enemy: Honeydew paid when this enemy is killed. Nothing is paid for reaching the Queen.")]
        [SerializeField, Min(0)] private int _honeydew = 5;

        private BasicEnemy _enemy;
        private HoneydewWallet _wallet;

        /// <summary>Honeydew paid on death.</summary>
        public int Amount => _honeydew;

        private void Awake()
        {
            _enemy = GetComponent<BasicEnemy>();
            if (_enemy.Data != null)
            {
                _honeydew = _enemy.Data.Honeydew;
            }
        }

        // Enemies are spawned from prefabs, so the wallet can't be a serialized reference.
        // Found once per enemy, the same way BasicEnemy finds its path.
        private void Start()
        {
            FindWallet();
        }

        private void OnEnable()
        {
            _enemy.OnDied += Pay;
        }

        private void OnDisable()
        {
            _enemy.OnDied -= Pay;
        }

        private void Pay(BasicEnemy enemy)
        {
            // A centipede half killed in the frame it split off dies before its Start runs:
            // look the wallet up now, or that kill pays nothing.
            FindWallet();
            if (_wallet != null)
            {
                _wallet.Honeydew.Earn(_honeydew);
            }
        }

        private void FindWallet()
        {
            if (_wallet == null)
            {
                _wallet = FindAnyObjectByType<HoneydewWallet>();
            }
        }
    }
}

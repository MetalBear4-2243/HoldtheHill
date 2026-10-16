using UnityEngine;

namespace HoldTheHill.Features.Economy
{
    /// <summary>
    /// Pays a flat Honeydew reward each time the spawner finishes a wave.
    /// Lives next to the <see cref="HoneydewWallet"/> in the gameplay scene.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HoneydewWallet))]
    public class WaveReward : MonoBehaviour
    {
        [Tooltip("The level's spawner. Its wave-completed event triggers the reward.")]
        [SerializeField] private EnemySpawner _spawner;

        [Tooltip("Honeydew paid when a wave has finished spawning.")]
        [SerializeField, Min(0)] private int _honeydewPerWave = 50;

        private HoneydewWallet _wallet;

        private void Awake()
        {
            _wallet = GetComponent<HoneydewWallet>();
        }

        private void OnEnable()
        {
            if (_spawner != null)
            {
                _spawner.onWaveCompleted.AddListener(Pay);
            }
        }

        private void OnDisable()
        {
            if (_spawner != null)
            {
                _spawner.onWaveCompleted.RemoveListener(Pay);
            }
        }

        private void Pay(int completedWaveNumber)
        {
            _wallet.Honeydew.Earn(_honeydewPerWave);
        }
    }
}

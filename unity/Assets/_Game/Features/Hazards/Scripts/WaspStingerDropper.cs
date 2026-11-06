using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Hazards
{
    /// <summary>
    /// Makes a wasp drop a <see cref="WaspStinger"/> every few seconds while it is alive and in the
    /// air. Sits on the Wasp prefab beside <see cref="WaspEnemy"/>; the wasp class is untouched.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BasicEnemy))]
    public class WaspStingerDropper : MonoBehaviour
    {
        [Tooltip("The stinger left on the ground.")]
        [SerializeField] private WaspStinger _stingerPrefab;

        [Tooltip("Seconds after spawning before the first stinger.")]
        [SerializeField, Min(0f)] private float _firstDropDelay = 2f;

        [Tooltip("Seconds between stingers after the first.")]
        [SerializeField, Min(0.1f)] private float _interval = 3.5f;

        private BasicEnemy _enemy;
        private IFlyingEnemy _flyer;
        private float _untilNextDrop;

        private void Awake()
        {
            _enemy = GetComponent<BasicEnemy>();
            _flyer = _enemy as IFlyingEnemy;
            _untilNextDrop = _firstDropDelay;
        }

        private void Update()
        {
            if (_stingerPrefab == null || _enemy.IsDead)
            {
                return;
            }

            // A grounded wasp has nothing to drop from.
            if (_flyer != null && !_flyer.IsFlying)
            {
                return;
            }

            _untilNextDrop -= Time.deltaTime;
            if (_untilNextDrop > 0f)
            {
                return;
            }

            _untilNextDrop = _interval;
            Vector3 spot = transform.position;
            spot.z = 0f;
            Instantiate(_stingerPrefab, spot, Quaternion.identity);
        }
    }
}

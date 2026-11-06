using System.Collections.Generic;
using HoldTheHill.Features.Towers;
using UnityEngine;

namespace HoldTheHill.Features.Hazards
{
    /// <summary>
    /// A stinger a wasp dropped. Lies where it fell, then bursts: every tower within reach misses
    /// for a while. The sprite swells over the fuse so the player sees it coming.
    /// </summary>
    [DisallowMultipleComponent]
    public class WaspStinger : MonoBehaviour
    {
        [Tooltip("Seconds between landing and bursting.")]
        [SerializeField, Min(0.1f)] private float _fuse = 2f;

        [Tooltip("Towers this close miss when the stinger bursts.")]
        [SerializeField, Min(0.1f)] private float _radius = 1.5f;

        [Tooltip("Seconds the towers caught in the burst keep missing.")]
        [SerializeField, Min(0.1f)] private float _missDuration = 4f;

        [Tooltip("How much bigger the sprite is drawn at the moment it bursts.")]
        [SerializeField, Min(1f)] private float _swell = 1.8f;

        private Vector3 _baseScale;
        private float _age;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        private void Update()
        {
            _age += Time.deltaTime;
            transform.localScale = _baseScale * Mathf.Lerp(1f, _swell, _age / _fuse);

            if (_age < _fuse)
            {
                return;
            }

            Burst();
            Destroy(gameObject);
        }

        private void Burst()
        {
            Vector3 here = transform.position;
            float radiusSqr = _radius * _radius;
            IReadOnlyList<Tower> towers = TowerRegistry.All;

            for (int i = 0; i < towers.Count; i++)
            {
                Tower tower = towers[i];
                if (tower != null && (tower.transform.position - here).sqrMagnitude <= radiusSqr)
                {
                    tower.Miss(_missDuration);
                }
            }
        }
    }
}

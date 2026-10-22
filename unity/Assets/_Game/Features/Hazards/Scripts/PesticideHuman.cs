using System.Collections.Generic;
using HoldTheHill.Features.Towers;
using UnityEngine;

namespace HoldTheHill.Features.Hazards
{
    /// <summary>
    /// The human with the pesticide. A cloud thickens over the map for a warning spell, then
    /// every tower that is not burrowed is stunned for a while. Burrow first and the colony is safe.
    /// </summary>
    [DisallowMultipleComponent]
    public class PesticideHuman : MonoBehaviour
    {
        [Tooltip("Seconds the cloud thickens before it lands. Time enough to burrow.")]
        [SerializeField, Min(0f)] private float _warningSeconds = 2.5f;

        [Tooltip("Seconds every unburrowed tower is stunned when the cloud lands.")]
        [SerializeField, Min(0.1f)] private float _stunSeconds = 6f;

        [Tooltip("The cloud sprite. Fades in over the warning and out over the stun.")]
        [SerializeField] private SpriteRenderer _cloud;

        [Tooltip("How opaque the cloud is at its thickest.")]
        [SerializeField, Range(0f, 1f)] private float _peakAlpha = 0.45f;

        private float _age;
        private bool _landed;

        private void Update()
        {
            _age += Time.deltaTime;

            if (!_landed && _age >= _warningSeconds)
            {
                _landed = true;
                Spray();
            }

            float end = _warningSeconds + _stunSeconds;
            if (_cloud != null)
            {
                float alpha = _landed
                    ? Mathf.Lerp(_peakAlpha, 0f, (_age - _warningSeconds) / _stunSeconds)
                    : Mathf.Lerp(0f, _peakAlpha, _warningSeconds > 0f ? _age / _warningSeconds : 1f);
                Color colour = _cloud.color;
                colour.a = alpha;
                _cloud.color = colour;
            }

            if (_age >= end)
            {
                Destroy(gameObject);
            }
        }

        private void Spray()
        {
            IReadOnlyList<Tower> towers = TowerRegistry.All;
            for (int i = 0; i < towers.Count; i++)
            {
                Tower tower = towers[i];
                if (tower != null && !tower.Burrowed)
                {
                    tower.Stun(_stunSeconds);
                }
            }
        }
    }
}

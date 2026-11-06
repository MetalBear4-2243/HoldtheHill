using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Shows which bodies she can raise, so the player can keep the fallen away from her: a ring on
    /// each remembered body within her raise reach, fading as it gets older, and a growing, brighter
    /// ring on the bodies chosen in a wind-up, just before they rise. The rings lie on the ground
    /// beside her (not under her transform, since she walks on) and go when she does.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(QueenMantisRaiser))]
    public class QueenMantisBodyMarkers : QueenMantisPart
    {
        [Tooltip("World units across the ring of a body she can raise.")]
        [SerializeField, Min(0.05f)] private float _markerSize = 0.5f;

        [Tooltip("World units across a chosen body's ring at the end of the wind-up.")]
        [SerializeField, Min(0.05f)] private float _windUpSize = 1f;

        [Tooltip("Sorting order of the rings: over the level's terrain (0), under the hill (4) and the enemies (5).")]
        [SerializeField] private int _sortingOrder = 1;

        private readonly List<SpriteRenderer> _pool = new List<SpriteRenderer>();
        private QueenMantisRaiser _raiser;

        protected override void Awake()
        {
            base.Awake();
            _raiser = GetComponent<QueenMantisRaiser>();
        }

        private void LateUpdate()
        {
            BossDeathLog log = _raiser.DeathLog;
            if (Data == null || Data.RingSprite == null || log == null)
            {
                return;
            }

            int used = 0;
            Vector3 here = transform.position;
            float now = Time.time;
            for (int i = 0; i < log.Count; i++)
            {
                DeathRecord death = log[i];
                float freshness = log.Freshness(death, now);
                if (freshness <= 0f || !QueenMantisHealRule.InReach(death.X - here.x, death.Y - here.y, Data.RaiseRadius))
                {
                    continue;
                }

                Color color = Data.MarkerColor;
                color.a *= freshness;
                Place(used++, death, _markerSize, color);
            }

            IReadOnlyList<DeathRecord> chosen = _raiser.WindingUp;
            float size = Mathf.Lerp(_markerSize, _windUpSize, _raiser.WindUpProgress);
            for (int i = 0; i < chosen.Count; i++)
            {
                Place(used++, chosen[i], size, Data.WindUpColor);
            }

            for (int i = used; i < _pool.Count; i++)
            {
                _pool[i].enabled = false;
            }
        }

        private void Place(int index, DeathRecord death, float size, Color color)
        {
            SpriteRenderer ring = index < _pool.Count ? _pool[index] : AddRing();
            ring.transform.position = new Vector3(death.X, death.Y, transform.position.z);
            ring.transform.localScale = new Vector3(size, size, 1f);
            ring.color = color;
            ring.enabled = true;
        }

        // Grows the pool only when more bodies show than ever before, so at most a dozen or so are made.
        private SpriteRenderer AddRing()
        {
            var go = new GameObject("Raisable Body Marker");
            go.transform.SetParent(transform.parent, worldPositionStays: false);
            if (transform.parent == null)
            {
                SceneManager.MoveGameObjectToScene(go, gameObject.scene);
            }

            var ring = go.AddComponent<SpriteRenderer>();
            ring.sprite = Data.RingSprite;
            ring.sortingOrder = _sortingOrder;
            _pool.Add(ring);
            return ring;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i] != null)
                {
                    Destroy(_pool[i].gameObject);
                }
            }
            _pool.Clear();
        }
    }
}

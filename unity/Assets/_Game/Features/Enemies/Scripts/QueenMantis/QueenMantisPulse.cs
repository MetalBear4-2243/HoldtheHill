using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The ring that shows a Queen Mantis heal: it grows from her out to the heal radius and fades.
    /// Sits on a child of the Queen Mantis with its own <see cref="SpriteRenderer"/> (a ring sprite
    /// one world unit across), hidden between pulses.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class QueenMantisPulse : MonoBehaviour
    {
        [Tooltip("Seconds one pulse takes to reach the heal radius and fade out.")]
        [SerializeField, Min(0.05f)] private float _duration = 0.5f;

        private SpriteRenderer _renderer;
        private Color _color;
        private float _radius;
        private float _elapsed;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.enabled = false;
            _elapsed = _duration;
        }

        /// <summary>Starts a pulse out to <paramref name="radius"/> world units, starting at <paramref name="color"/>.</summary>
        public void Play(float radius, Color color)
        {
            _radius = radius;
            _color = color;
            _elapsed = 0f;
            _renderer.enabled = true;
            Show(0f);
        }

        private void Update()
        {
            if (_elapsed >= _duration)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed >= _duration)
            {
                _renderer.enabled = false;
                return;
            }
            Show(_elapsed / _duration);
        }

        private void Show(float t)
        {
            // The sprite is one unit across, so the scale is the diameter; undo the parents' scale.
            float diameter = Mathf.Lerp(0.3f, _radius * 2f, t);
            Vector3 parentScale = transform.parent != null ? transform.parent.lossyScale : Vector3.one;
            transform.localScale = new Vector3(diameter / Mathf.Max(0.0001f, parentScale.x), diameter / Mathf.Max(0.0001f, parentScale.y), 1f);

            Color color = _color;
            color.a *= 1f - t;
            _renderer.color = color;
        }
    }
}

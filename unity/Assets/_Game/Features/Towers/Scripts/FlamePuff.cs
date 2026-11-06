using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// One puff of the Fire Ant's flame stream: flies straight out, grows a little and fades, then
    /// goes. Only a picture; the tower's cone does the damage. Its loop is a <c>SpriteFlipbook</c> on
    /// the same object (the sheet's fire-jet row), and its art faces right, the way it flies.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class FlamePuff : MonoBehaviour
    {
        [Tooltip("How much bigger the puff is when it dies out than when it leaves.")]
        [SerializeField, Min(0.1f)] private float _endScale = 1.4f;

        [Tooltip("Share of its life over which it fades at the end (0.4 = the last 40 percent).")]
        [SerializeField, Range(0f, 1f)] private float _fadeShare = 0.4f;

        private SpriteRenderer _renderer;
        private Color _baseColor;
        private Vector3 _baseScale;
        private Vector3 _velocity;
        private float _life = 0.3f;
        private float _age;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _baseColor = _renderer.color;
            _baseScale = transform.localScale;
        }

        /// <summary>Sends the puff off at <paramref name="velocity"/> for <paramref name="seconds"/>, turned to face it.</summary>
        public void Launch(Vector3 velocity, float seconds)
        {
            velocity.z = 0f;
            _velocity = velocity;
            _life = Mathf.Max(0.05f, seconds);
            _age = 0f;
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age >= _life)
            {
                Destroy(gameObject);
                return;
            }

            transform.position += _velocity * Time.deltaTime;
            float t = _age / _life;
            transform.localScale = _baseScale * Mathf.Lerp(1f, _endScale, t);

            Color color = _baseColor;
            color.a *= _fadeShare > 0f ? Mathf.Clamp01((1f - t) / _fadeShare) : 1f;
            _renderer.color = color;
        }
    }
}

using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Rolly Polly: a <see cref="BasicEnemy"/> that curls up when hit (see
    /// <see cref="RollyPollyCurl"/>). Curled, it takes only a share of each hit and rolls faster
    /// for a few tiles of path, then uncurls. At most a few curls per life; after that it is an
    /// ordinary walker. The curl shows as a smaller, darker sprite.
    /// </summary>
    public class RollyPollyEnemy : BasicEnemy
    {
        [Header("Curl")]
        [Tooltip("How many times it can curl in one life.")]
        [SerializeField, Min(0)] private int _maxCurls = 3;

        [Tooltip("Path distance rolled while curled before it uncurls, in tiles (one tile is one world unit).")]
        [SerializeField, Min(0f)] private float _rollDistance = 3f;

        [Tooltip("The share of each hit that lands while curled. 0.4 means it takes 40 percent.")]
        [SerializeField, Range(0f, 1f)] private float _curledDamageShare = 0.4f;

        [Tooltip("Movement speed multiplier while rolling.")]
        [SerializeField, Min(0f)] private float _curledSpeedMultiplier = 2f;

        [Tooltip("How much smaller the sprite is drawn while curled.")]
        [SerializeField, Range(0.1f, 1f)] private float _curledScale = 0.75f;

        [Tooltip("Sprite tint while curled; the uncurled tint is whatever the prefab has.")]
        [SerializeField] private Color _curledTint = new Color(0.55f, 0.55f, 0.65f);

        private RollyPollyCurl _curl;
        private float _uncurledSpeed;
        private Vector3 _uncurledScale;
        private Color _uncurledTint;

        /// <summary>The curl rule, for tests and the visual.</summary>
        public RollyPollyCurl Curl => _curl;

        protected override void Awake()
        {
            base.Awake();
            _curl = new RollyPollyCurl(_maxCurls, _rollDistance, _curledDamageShare, _curledSpeedMultiplier);
            _uncurledSpeed = _moveSpeed;
            _uncurledScale = transform.localScale;
            _uncurledTint = _spriteRenderer != null ? _spriteRenderer.color : Color.white;
        }

        protected override void Update()
        {
            Vector3 before = transform.position;
            base.Update();

            if (IsDead || !_curl.IsCurled)
            {
                return;
            }

            if (_curl.Advance(Vector3.Distance(before, transform.position)))
            {
                ShowCurled(false);
            }
        }

        /// <inheritdoc />
        public override void TakeDamage(double damageAmount)
        {
            if (damageAmount <= 0.0 || IsDead)
            {
                return;
            }

            bool wasCurled = _curl.IsCurled;
            double landed = _curl.Hit(damageAmount);
            if (_curl.IsCurled && !wasCurled)
            {
                ShowCurled(true);
            }

            base.TakeDamage(landed);
        }

        // The roll speed is the prefab's walking speed times the multiplier; Jason's slows still
        // apply on top through the speed multiplier in BasicEnemy.
        private void ShowCurled(bool curled)
        {
            _moveSpeed = _uncurledSpeed * _curl.SpeedMultiplier;
            transform.localScale = curled ? _uncurledScale * _curledScale : _uncurledScale;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = curled ? _curledTint : _uncurledTint;
            }
        }
    }
}

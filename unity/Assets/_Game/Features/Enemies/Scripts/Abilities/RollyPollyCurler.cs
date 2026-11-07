using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Rolly Polly's curl, an ability beside <see cref="BasicEnemy"/> around a
    /// <see cref="RollyPollyCurl"/>. A hit makes it curl up: curled, it takes only a share of each
    /// hit and rolls faster for a few tiles of path, then uncurls. At most a few curls per life;
    /// after that it is an ordinary walker. The curl shows as a smaller, darker sprite.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BasicEnemy))]
    public class RollyPollyCurler : MonoBehaviour, IDamageModifier
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

        private BasicEnemy _enemy;
        private SpriteRenderer _sprite;
        private RollyPollyCurl _curl;
        private float _uncurledSpeed;
        private Vector3 _uncurledScale;
        private Color _uncurledTint;
        private float _walkedAtLastCheck;

        /// <summary>The curl rule, for tests and the visual.</summary>
        public RollyPollyCurl Curl
        {
            get
            {
                EnsureCurl();
                return _curl;
            }
        }

        private void Awake()
        {
            _enemy = GetComponent<BasicEnemy>();
            _sprite = GetComponentInChildren<SpriteRenderer>();
            EnsureCurl();
        }

        private void Update()
        {
            // Only the road walked counts (BasicEnemy's odometer), not a knockback or a teleport.
            float walked = _enemy.DistanceWalked;
            float rolled = walked - _walkedAtLastCheck;
            _walkedAtLastCheck = walked;

            if (_enemy.IsDead || !_curl.IsCurled)
            {
                return;
            }

            if (_curl.Advance(rolled))
            {
                ShowCurled(false);
            }
        }

        /// <summary>The hit that curls it lands in full; hits while curled land at the curled share.</summary>
        public double ModifyDamage(double damage)
        {
            EnsureCurl();
            bool wasCurled = _curl.IsCurled;
            double landed = _curl.Hit(damage);
            if (_curl.IsCurled && !wasCurled)
            {
                ShowCurled(true);
            }

            return landed;
        }

        // Made on demand as well as in Awake, so EditMode tests, which never run Awake, work too.
        private void EnsureCurl()
        {
            if (_curl == null)
            {
                _curl = new RollyPollyCurl(_maxCurls, _rollDistance, _curledDamageShare, _curledSpeedMultiplier);
            }
        }

        // The roll speed is the walking speed times the multiplier; Jason's slows still apply on top
        // through the speed multiplier in BasicEnemy. The walking speed, size and tint are taken as
        // the curl starts and put back when it ends.
        private void ShowCurled(bool curled)
        {
            if (_enemy == null)
            {
                _enemy = GetComponent<BasicEnemy>();
                _sprite = GetComponentInChildren<SpriteRenderer>();
            }

            if (curled)
            {
                _uncurledSpeed = _enemy.MoveSpeed;
                _uncurledScale = transform.localScale;
                _uncurledTint = _sprite != null ? _sprite.color : Color.white;
                _walkedAtLastCheck = _enemy.DistanceWalked;
            }

            _enemy.MoveSpeed = _uncurledSpeed * _curl.SpeedMultiplier;
            transform.localScale = curled ? _uncurledScale * _curledScale : _uncurledScale;
            if (_sprite != null)
            {
                _sprite.color = curled ? _curledTint : _uncurledTint;
            }
        }
    }
}

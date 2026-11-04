using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Combat;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Plays the enemy sheet's Hurt row over the walk each time a hit takes health, and leaves an
    /// <see cref="EnemyDeathEffect"/> playing the Death row where it dies (the enemy itself is removed
    /// at once). An enemy that reaches the Queen leaves no body. The walk loop keeps time under the
    /// hurt frames, so it carries on from where it was. Frames come from Build Sprites; with none,
    /// the enemy looks as it did.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BasicEnemy))]
    public class EnemyHurtAndDeath : MonoBehaviour
    {
        [Tooltip("The renderer the enemy is drawn with (its Body).")]
        [SerializeField] private SpriteRenderer _body;

        [Tooltip("The walk loop, held while the hurt frames show.")]
        [SerializeField] private SpriteFlipbook _walk;

        [Header("Hurt")]
        [SerializeField] private Sprite[] _hurtFrames = System.Array.Empty<Sprite>();
        [SerializeField, Min(0.01f)] private float _hurtFrameSeconds = 0.08f;

        [Header("Death")]
        [SerializeField] private Sprite[] _deathFrames = System.Array.Empty<Sprite>();
        [SerializeField, Min(0.01f)] private float _deathFrameSeconds = 0.08f;

        [Tooltip("Seconds the body takes to fade after the last death frame.")]
        [SerializeField, Min(0f)] private float _deathFadeSeconds = 0.35f;

        private BasicEnemy _enemy;
        private bool _hurting;
        private float _clock;
        private int _shown = -1;

        /// <summary>True while the hurt frames show.</summary>
        public bool Hurting => _hurting;

        private void Awake()
        {
            _enemy = GetComponent<BasicEnemy>();
            if (_body == null)
            {
                _body = GetComponentInChildren<SpriteRenderer>();
            }

            if (_walk == null && _body != null)
            {
                _walk = _body.GetComponent<SpriteFlipbook>();
            }
        }

        private void OnEnable()
        {
            _enemy.OnDamaged += HandleDamaged;
            _enemy.OnDied += HandleDied;
        }

        private void OnDisable()
        {
            _enemy.OnDamaged -= HandleDamaged;
            _enemy.OnDied -= HandleDied;
            StopHurt();
        }

        private void Update()
        {
            if (!_hurting)
            {
                return;
            }

            _clock += Time.deltaTime;
            int index = SpriteRow.FrameAt(_clock, _hurtFrameSeconds, _hurtFrames.Length);
            if (index < 0)
            {
                StopHurt();
                return;
            }

            Show(index);
        }

        // A hit that took health, on a living enemy: a killing blow plays the death instead.
        private void HandleDamaged(BasicEnemy enemy, DamageInfo hit, float applied)
        {
            if (applied <= 0f || !_enemy.IsActive || _body == null || _hurtFrames == null || _hurtFrames.Length == 0)
            {
                return;
            }

            _hurting = true;
            _clock = 0f;
            _shown = -1;
            if (_walk != null)
            {
                _walk.Held = true;
            }

            Show(0);
        }

        private void HandleDied(BasicEnemy enemy)
        {
            StopHurt();
            EnemyDeathEffect.Spawn(_body, _deathFrames, _deathFrameSeconds, _deathFadeSeconds, gameObject.scene);
        }

        private void StopHurt()
        {
            if (!_hurting)
            {
                return;
            }

            _hurting = false;
            _shown = -1;
            if (_walk != null)
            {
                _walk.Held = false;
            }
        }

        private void Show(int index)
        {
            if (index < 0 || index == _shown || _body == null || _hurtFrames[index] == null)
            {
                return;
            }

            _shown = index;
            _body.sprite = _hurtFrames[index];
        }
    }
}

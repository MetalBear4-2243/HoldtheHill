using HoldTheHill.Core.Animation;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// Plays the tower's attack row from its sprite sheet over the idle loop. A tower that attacks on
    /// its own winds up just before it is ready, so the hit frame shows as the swing lands or the shot
    /// leaves; an attack that came without warning (an enemy walking into range of a ready tower) jumps
    /// straight to the hit frame. The Carpenter Ant plays it as it lays a mine, the Sap Ant while its
    /// aura slows someone. Stunned or burrowed, the tower goes back to idle. The idle
    /// <see cref="SpriteFlipbook"/> is switched off while the attack plays. Frames come from Build Sprites.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tower))]
    public class TowerAttackAnimation : MonoBehaviour
    {
        [Tooltip("The renderer the tower is drawn with.")]
        [SerializeField] private SpriteRenderer _renderer;

        [Tooltip("The idle loop, paused while the attack plays.")]
        [SerializeField] private SpriteFlipbook _idle;

        [Tooltip("The attack row, in order.")]
        [SerializeField] private Sprite[] _frames = System.Array.Empty<Sprite>();

        [Tooltip("The sheet's seconds per attack frame. Shortened when the tower fires faster than the row plays.")]
        [SerializeField, Min(0.01f)] private float _frameSeconds = 0.09f;

        [Tooltip("The frame where the blow lands (0 is the first). Frames before it are the wind-up.")]
        [SerializeField, Min(0)] private int _hitFrame = 2;

        private Tower _tower;
        private bool _playing;
        private float _clock;
        private float _playFrameSeconds;
        private int _shown = -1;

        /// <summary>True while the attack row is showing.</summary>
        public bool Playing => _playing;

        private void Awake()
        {
            _tower = GetComponent<Tower>();
            if (_renderer == null)
            {
                _renderer = GetComponent<SpriteRenderer>();
            }

            if (_idle == null)
            {
                _idle = GetComponent<SpriteFlipbook>();
            }
        }

        private void OnEnable()
        {
            _tower.Attacked += HandleAttacked;
            _tower.AbilityUsed += HandleAbilityUsed;
        }

        private void OnDisable()
        {
            _tower.Attacked -= HandleAttacked;
            _tower.AbilityUsed -= HandleAbilityUsed;
            Stop();
        }

        // After the tower's Update, so this frame's target and cooldown are known.
        private void LateUpdate()
        {
            if (_frames == null || _frames.Length == 0)
            {
                return;
            }

            if (!_tower.CanAct)
            {
                Stop();
                return;
            }

            if (!_playing)
            {
                float frameSeconds = CurrentFrameSeconds();
                bool windUp = TowerAttackTiming.ShouldWindUp(
                    _tower.UsesTargeting, _tower.CanAct, _tower.CurrentTarget != null,
                    _tower.CooldownRemaining, TowerAttackTiming.WindUpSeconds(_hitFrame, frameSeconds));
                if (!windUp)
                {
                    return;
                }

                Play(fromFrame: 0);
            }
            else
            {
                _clock += Time.deltaTime;
            }

            int index = TowerAttackTiming.FrameAt(_clock, _playFrameSeconds, _frames.Length);
            if (index < 0)
            {
                Stop();
                return;
            }

            Show(index);
        }

        // The blow lands now: show the hit frame, whether or not the wind-up ran.
        private void HandleAttacked()
        {
            if (!_playing)
            {
                Play(fromFrame: _hitFrame);
            }
            else
            {
                _clock = Mathf.Min(_hitFrame, _frames.Length - 1) * _playFrameSeconds;
            }

            Show(TowerAttackTiming.FrameAt(_clock, _playFrameSeconds, _frames.Length));
        }

        // An ability plays the whole row once; a cast already showing is not restarted.
        private void HandleAbilityUsed()
        {
            if (!_playing && _tower.CanAct)
            {
                Play(fromFrame: 0);
                Show(0);
            }
        }

        private void Play(int fromFrame)
        {
            if (_frames == null || _frames.Length == 0)
            {
                return;
            }

            _playing = true;
            _playFrameSeconds = CurrentFrameSeconds();
            _clock = Mathf.Clamp(fromFrame, 0, _frames.Length - 1) * _playFrameSeconds;
            _shown = -1;
            if (_idle != null)
            {
                _idle.enabled = false;
            }
        }

        private void Stop()
        {
            if (!_playing)
            {
                return;
            }

            _playing = false;
            _shown = -1;
            if (_idle != null)
            {
                // Its OnEnable shows an idle frame at once.
                _idle.enabled = true;
            }
        }

        private void Show(int index)
        {
            if (index < 0 || index == _shown || _renderer == null || _frames[index] == null)
            {
                return;
            }

            _shown = index;
            _renderer.sprite = _frames[index];
        }

        private float CurrentFrameSeconds()
        {
            return TowerAttackTiming.FrameSeconds(_frameSeconds, _frames.Length, _tower.FireInterval, TowerTempo.FireRateFactor);
        }
    }
}

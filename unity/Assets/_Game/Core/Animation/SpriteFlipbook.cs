using UnityEngine;

namespace HoldTheHill.Core.Animation
{
    /// <summary>
    /// Plays a looping row of sprites on one <see cref="SpriteRenderer"/>, at a fixed time per frame.
    /// No Animator: the frames and their timing come from the sheet (see AlphaSpritesBuilder). Runs on
    /// scaled time, so a paused game holds the frame.
    /// </summary>
    [DisallowMultipleComponent]
    public class SpriteFlipbook : MonoBehaviour
    {
        [Tooltip("The renderer to animate. Empty means the one on this object.")]
        [SerializeField] private SpriteRenderer _renderer;

        [Tooltip("The loop, in order.")]
        [SerializeField] private Sprite[] _frames = System.Array.Empty<Sprite>();

        [Tooltip("Seconds each frame shows (the sheet's frame duration).")]
        [SerializeField, Min(0.01f)] private float _frameSeconds = 0.1f;

        [Tooltip("Start each copy on a random frame, so a crowd does not step in time.")]
        [SerializeField] private bool _randomStart = true;

        private float _clock;
        private int _shown = -1;

        /// <summary>How many frames the loop has.</summary>
        public int FrameCount => _frames != null ? _frames.Length : 0;

        private void Awake()
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<SpriteRenderer>();
            }
        }

        private void OnEnable()
        {
            _clock = _randomStart && FrameCount > 1 ? Random.Range(0f, FrameCount * _frameSeconds) : 0f;
            _shown = -1;
            Show();
        }

        private void Update()
        {
            int count = FrameCount;
            if (count == 0)
            {
                return;
            }

            // Kept inside one loop, so a long run never loses float precision.
            _clock = (_clock + Time.deltaTime) % (count * _frameSeconds);
            Show();
        }

        private void Show()
        {
            int count = FrameCount;
            if (_renderer == null || count == 0)
            {
                return;
            }

            int index = Mathf.Clamp((int)(_clock / _frameSeconds), 0, count - 1);
            if (index != _shown && _frames[index] != null)
            {
                _shown = index;
                _renderer.sprite = _frames[index];
            }
        }
    }
}

using UnityEngine;

namespace HoldTheHill.Core.Animation
{
    /// <summary>
    /// Plays a row of sprites once on one <see cref="SpriteRenderer"/>, then removes its object: a
    /// splash where a shot hits, a blast where a mine goes off. The frames and their timing come from
    /// the sheet (see AlphaSpritesBuilder). Runs on scaled time, so a paused game holds the frame.
    /// </summary>
    [DisallowMultipleComponent]
    public class SpriteOneShot : MonoBehaviour
    {
        [Tooltip("The renderer to animate. Empty means the one on this object.")]
        [SerializeField] private SpriteRenderer _renderer;

        [Tooltip("The row, in order. Played once; the object is removed after the last frame.")]
        [SerializeField] private Sprite[] _frames = System.Array.Empty<Sprite>();

        [Tooltip("Seconds each frame shows (the sheet's frame duration).")]
        [SerializeField, Min(0.01f)] private float _frameSeconds = 0.06f;

        private float _clock;
        private int _shown = -1;

        private void Awake()
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<SpriteRenderer>();
            }
        }

        private void OnEnable()
        {
            _clock = 0f;
            _shown = -1;
            Show();
        }

        private void Update()
        {
            _clock += Time.deltaTime;
            Show();
        }

        private void Show()
        {
            int count = _frames != null ? _frames.Length : 0;
            int index = SpriteRow.FrameAt(_clock, _frameSeconds, count);
            if (index < 0)
            {
                Destroy(gameObject);
                return;
            }

            if (index != _shown && _renderer != null && _frames[index] != null)
            {
                _shown = index;
                _renderer.sprite = _frames[index];
            }
        }
    }
}

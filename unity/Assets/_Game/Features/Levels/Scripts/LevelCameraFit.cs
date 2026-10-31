using UnityEngine;

namespace HoldTheHill.Features.Levels
{
    /// <summary>
    /// Frames the level's map on any screen shape, with bands left free above and below for the HUD,
    /// so the top and bottom rows are not under the Honeydew line, the power-up slot or Next Wave.
    /// Sets the orthographic size and height of the camera it sits on, again whenever the screen
    /// size changes. The map's centre stays at the world origin.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class LevelCameraFit : MonoBehaviour
    {
        [Tooltip("Half the map's width to keep on screen, in world units. The spawn and the Queen stand at x -9.5 and 9.5.")]
        [SerializeField, Min(1f)] private float _halfWidth = 10f;

        [Tooltip("Half the map's height to keep on screen, in world units.")]
        [SerializeField, Min(1f)] private float _halfHeight = 5f;

        [Tooltip("Share of the screen height kept free at the bottom, for the power-up slot and Next Wave.")]
        [SerializeField, Range(0f, 0.4f)] private float _bottomBand = 0.09f;

        [Tooltip("Share of the screen height kept free at the top, for the Honeydew and wave lines.")]
        [SerializeField, Range(0f, 0.4f)] private float _topBand = 0.07f;

        private Camera _camera;
        private Vector2Int _fittedFor;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            var screen = new Vector2Int(Screen.width, Screen.height);
            if (screen != _fittedFor && screen.y > 0)
            {
                _fittedFor = screen;
                Fit((float)screen.x / screen.y);
            }
        }

        private void Fit(float aspect)
        {
            float size = OrthographicSize(aspect, _halfWidth, _halfHeight, _bottomBand, _topBand);
            _camera.orthographicSize = size;

            Vector3 position = transform.position;
            position.y = CameraHeight(size, _bottomBand, _topBand);
            transform.position = position;
        }

        /// <summary>
        /// The smallest orthographic size that shows the whole map: its width across the screen and
        /// its height between the two HUD bands.
        /// </summary>
        public static float OrthographicSize(float aspect, float halfWidth, float halfHeight, float bottomBand, float topBand)
        {
            float mapShare = Mathf.Max(0.1f, 1f - bottomBand - topBand);
            return Mathf.Max(halfHeight / mapShare, halfWidth / Mathf.Max(0.1f, aspect));
        }

        /// <summary>The camera height that puts the map's centre in the middle of the space between the bands.</summary>
        public static float CameraHeight(float size, float bottomBand, float topBand)
        {
            return (topBand - bottomBand) * size;
        }
    }
}

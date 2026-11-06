using UnityEngine;

namespace HoldTheHill.Features.Levels
{
    /// <summary>
    /// Frames the level's map on any screen shape, with bands left free above and below for the HUD,
    /// so the top and bottom rows are not under the Honeydew line, the Queen bar, the power-up slot or
    /// Next Wave, and a band on the right for the shop list, so the Queen is not under it. Sets the
    /// orthographic size and position of the camera it sits on, again whenever the screen size
    /// changes. The map's centre stays at the world origin.
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

        [Tooltip("Share of the screen height kept free at the top, for the Honeydew and wave lines and the Queen bar under them.")]
        [SerializeField, Range(0f, 0.4f)] private float _topBand = 0.15f;

        [Tooltip("Share of the screen width kept free on the right, for Adam's tower list. 0 uses the whole width.")]
        [SerializeField, Range(0f, 0.5f)] private float _rightBand = 0.3f;

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
            float size = OrthographicSize(aspect, _halfWidth, _halfHeight, _bottomBand, _topBand, _rightBand);
            _camera.orthographicSize = size;

            Vector3 position = transform.position;
            position.x = CameraX(size, aspect, _rightBand);
            position.y = CameraHeight(size, _bottomBand, _topBand);
            transform.position = position;
        }

        /// <summary>
        /// The smallest orthographic size that shows the whole map: its width left of the right band
        /// and its height between the two HUD bands.
        /// </summary>
        public static float OrthographicSize(float aspect, float halfWidth, float halfHeight, float bottomBand, float topBand,
            float rightBand = 0f)
        {
            float heightShare = Mathf.Max(0.1f, 1f - bottomBand - topBand);
            float widthShare = Mathf.Max(0.1f, 1f - rightBand);
            return Mathf.Max(halfHeight / heightShare, halfWidth / (Mathf.Max(0.1f, aspect) * widthShare));
        }

        /// <summary>The camera's x that puts the map's centre in the middle of the space left of the right band.</summary>
        public static float CameraX(float size, float aspect, float rightBand)
        {
            return rightBand * size * aspect;
        }

        /// <summary>The camera height that puts the map's centre in the middle of the space between the bands.</summary>
        public static float CameraHeight(float size, float bottomBand, float topBand)
        {
            return (topBand - bottomBand) * size;
        }
    }
}

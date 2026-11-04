using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// A thin circle showing how far a tower reaches. The placer shows one around the ghost before a
    /// tower is bought, and around the selected tower, so the player sees what a cell covers before
    /// paying. Made at run time, so no prefab or scene needs one.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public class RangeRing : MonoBehaviour
    {
        /// <summary>Points on the circle; enough to look round at the largest tower range.</summary>
        public const int Segments = 64;

        private const float Width = 0.05f;

        private readonly Vector3[] _points = new Vector3[Segments];
        private LineRenderer _line;
        private Vector3 _centre;
        private float _radius = -1f;

        /// <summary>
        /// Makes a hidden ring in <paramref name="scene"/>, so it unloads with the level. It is drawn
        /// with <paramref name="look"/>'s material and sorting layer, at <paramref name="sortingOrder"/>.
        /// </summary>
        public static RangeRing Create(string name, Scene scene, SpriteRenderer look, Color color, int sortingOrder)
        {
            var holder = new GameObject(name);
            SceneManager.MoveGameObjectToScene(holder, scene);
            RangeRing ring = holder.AddComponent<RangeRing>();
            ring.Init(look, color, sortingOrder);
            return ring;
        }

        private void Init(SpriteRenderer look, Color color, int sortingOrder)
        {
            _line = GetComponent<LineRenderer>();
            _line.useWorldSpace = true;
            _line.loop = true;
            _line.positionCount = Segments;
            _line.widthMultiplier = Width;
            _line.startColor = color;
            _line.endColor = color;
            _line.sortingOrder = sortingOrder;
            if (look != null)
            {
                _line.sharedMaterial = look.sharedMaterial;
                _line.sortingLayerID = look.sortingLayerID;
            }

            _line.enabled = false;
        }

        /// <summary>Shows the ring around <paramref name="centre"/>; a radius of zero or less hides it.</summary>
        public void Show(Vector3 centre, float radius)
        {
            if (radius <= 0f)
            {
                Hide();
                return;
            }

            if (radius != _radius || centre != _centre)
            {
                _centre = centre;
                _radius = radius;
                CirclePoints(centre, radius, _points);
                _line.SetPositions(_points);
            }

            _line.enabled = true;
        }

        /// <summary>Hides the ring.</summary>
        public void Hide()
        {
            _line.enabled = false;
        }

        /// <summary>Fills <paramref name="into"/> with evenly spaced points on the circle, at the centre's z.</summary>
        public static void CirclePoints(Vector3 centre, float radius, Vector3[] into)
        {
            int count = into.Length;
            for (int i = 0; i < count; i++)
            {
                float angle = Mathf.PI * 2f * i / count;
                into[i] = new Vector3(centre.x + Mathf.Cos(angle) * radius, centre.y + Mathf.Sin(angle) * radius, centre.z);
            }
        }
    }
}

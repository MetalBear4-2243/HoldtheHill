using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// A ring around a stunned tower that empties like a clock as the stun wears off, so the player
    /// can see when the tower fights again. Hidden while the tower is not stunned. <see cref="Tower"/>
    /// attaches one the first time it is stunned, so no prefab needs it.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public class TowerStunRing : MonoBehaviour
    {
        private const int FullCircleSegments = 48;

        // In world units, so the ring is the same size at every tower level and stays inside the cell.
        private const float Radius = 0.45f;
        private const float Width = 0.07f;

        private Tower _tower;
        private LineRenderer _line;

        /// <summary>
        /// Makes a ring for <paramref name="tower"/>, drawn with <paramref name="look"/>'s material
        /// just above it, in <paramref name="color"/>.
        /// </summary>
        public static TowerStunRing Attach(Tower tower, SpriteRenderer look, Color color)
        {
            var holder = new GameObject("Stun Ring");
            holder.transform.SetParent(tower.transform, worldPositionStays: false);
            TowerStunRing ring = holder.AddComponent<TowerStunRing>();
            ring.Init(tower, look, color);
            return ring;
        }

        private void Init(Tower tower, SpriteRenderer look, Color color)
        {
            _tower = tower;
            _line = GetComponent<LineRenderer>();
            _line.useWorldSpace = true;
            _line.loop = false;
            _line.widthMultiplier = Width;
            _line.numCapVertices = 2;
            _line.startColor = color;
            _line.endColor = color;
            if (look != null)
            {
                // The tower's own sprite material, so the ring is lit and sorted like the tower.
                _line.sharedMaterial = look.sharedMaterial;
                _line.sortingLayerID = look.sortingLayerID;
                _line.sortingOrder = look.sortingOrder + 1;
            }

            Refresh();
        }

        private void LateUpdate()
        {
            Refresh();
        }

        private void Refresh()
        {
            float share = _tower != null ? _tower.Condition.StunLeftShare : 0f;
            if (share <= 0f)
            {
                _line.enabled = false;
                return;
            }

            _line.enabled = true;
            int points = Mathf.Max(2, Mathf.CeilToInt(FullCircleSegments * share) + 1);
            _line.positionCount = points;

            Vector3 centre = _tower.transform.position;
            float sweep = share * Mathf.PI * 2f;
            for (int i = 0; i < points; i++)
            {
                // From twelve o'clock, clockwise; the gap grows as the stun runs out.
                float angle = Mathf.PI * 0.5f - sweep * i / (points - 1);
                _line.SetPosition(i, centre + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * Radius);
            }
        }
    }
}

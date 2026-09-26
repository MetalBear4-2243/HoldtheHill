using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// A placed tower. For now it only knows its grid cell and range;
    /// targeting and shooting come later.
    /// </summary>
    public class Tower : MonoBehaviour
    {
        [Tooltip("How far this tower can reach, in world units.")]
        [SerializeField] private float _range = 2.5f;

        public float Range => _range;

        public Vector2Int Cell { get; set; }

        // Draws the range circle in the Scene view when the tower is selected.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}

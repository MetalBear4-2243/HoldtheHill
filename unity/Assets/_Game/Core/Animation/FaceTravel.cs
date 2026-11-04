using UnityEngine;

namespace HoldTheHill.Core.Animation
{
    /// <summary>
    /// Turns a child (the drawn body) to face the way this object moved since the last frame. Reads
    /// only the object's own position, after the movement code has run, so it never moves the object
    /// or needs to know how it walks.
    /// </summary>
    [DisallowMultipleComponent]
    public class FaceTravel : MonoBehaviour
    {
        [Tooltip("The child that turns. Its sprite's front points along Art Facing.")]
        [SerializeField] private Transform _body;

        [Tooltip("Which way the art faces, in degrees: 0 is right, 90 is up.")]
        [SerializeField] private float _artFacingDegrees;

        [Tooltip("How fast the body turns, in degrees per second.")]
        [SerializeField, Min(0f)] private float _turnDegreesPerSecond = 720f;

        [Tooltip("A jump longer than this in one frame is a teleport (spawn, split), not a step, and is ignored.")]
        [SerializeField, Min(0f)] private float _teleportDistance = 1f;

        private Vector3 _lastPosition;
        private bool _hasFaced;

        private void OnEnable()
        {
            _lastPosition = transform.position;
            _hasFaced = false;
        }

        private void LateUpdate()
        {
            Vector3 moved = transform.position - _lastPosition;
            _lastPosition = transform.position;
            moved.z = 0f;

            float step = moved.sqrMagnitude;
            if (_body == null || step < 0.0000001f || step > _teleportDistance * _teleportDistance)
            {
                return;
            }

            float target = Mathf.Atan2(moved.y, moved.x) * Mathf.Rad2Deg - _artFacingDegrees;
            // The first step snaps, so a new enemy does not spin round on the road.
            float next = _hasFaced
                ? Mathf.MoveTowardsAngle(_body.localEulerAngles.z, target, _turnDegreesPerSecond * Time.deltaTime)
                : target;
            _body.localRotation = Quaternion.Euler(0f, 0f, next);
            _hasFaced = true;
        }
    }
}

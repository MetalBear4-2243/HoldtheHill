using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// A looping picture worn over an enemy while something lasts on it: stuck in sap, poisoned. Made
    /// on the enemy by what causes it (<see cref="SapPuddle"/>, <see cref="PoisonStacks"/>), one per
    /// kind, and hidden again when its time runs out; it goes with the enemy. Only a picture: the
    /// slow or the poison itself is the enemy's own status effect.
    /// </summary>
    [DisallowMultipleComponent]
    public class StatusOverlay : MonoBehaviour
    {
        private const string NamePrefix = "Overlay ";

        private SpriteRenderer _renderer;
        private Sprite[] _frames;
        private float _frameSeconds = 0.2f;
        private float _clock;
        private float _remaining;

        /// <summary>
        /// Shows the <paramref name="kind"/> overlay on <paramref name="enemy"/> for
        /// <paramref name="seconds"/> more, making it the first time. Frames are drawn at one sheet
        /// frame to the enemy's body frame (both are one unit across before the enemy's own scale),
        /// just above the body.
        /// </summary>
        public static void Show(BasicEnemy enemy, string kind, Sprite[] frames, float frameSeconds, float seconds)
        {
            if (enemy == null || frames == null || frames.Length == 0 || seconds <= 0f)
            {
                return;
            }

            Transform holder = enemy.transform.Find(NamePrefix + kind);
            StatusOverlay overlay = holder != null ? holder.GetComponent<StatusOverlay>() : null;
            if (overlay == null)
            {
                overlay = Make(enemy, kind);
            }

            overlay._frames = frames;
            overlay._frameSeconds = Mathf.Max(0.01f, frameSeconds);
            overlay._remaining = Mathf.Max(overlay._remaining, seconds);
            if (!overlay.gameObject.activeSelf)
            {
                overlay._clock = 0f;
                overlay.gameObject.SetActive(true);
            }
        }

        private static StatusOverlay Make(BasicEnemy enemy, string kind)
        {
            // The body is the enemy's first SpriteRenderer (the sprites builder puts it first).
            SpriteRenderer body = enemy.GetComponentInChildren<SpriteRenderer>();

            var go = new GameObject(NamePrefix + kind);
            go.transform.SetParent(enemy.transform, worldPositionStays: false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var renderer = go.AddComponent<SpriteRenderer>();
            if (body != null)
            {
                renderer.sharedMaterial = body.sharedMaterial;
                renderer.sortingLayerID = body.sortingLayerID;
                renderer.sortingOrder = body.sortingOrder + 1;
            }

            var overlay = go.AddComponent<StatusOverlay>();
            overlay._renderer = renderer;
            return overlay;
        }

        private void Update()
        {
            _remaining -= Time.deltaTime;
            if (_remaining <= 0f)
            {
                gameObject.SetActive(false);
                return;
            }

            _clock += Time.deltaTime;
            int count = _frames != null ? _frames.Length : 0;
            if (_renderer != null && count > 0)
            {
                _renderer.sprite = _frames[(int)(_clock / _frameSeconds) % count];
            }
        }
    }
}
